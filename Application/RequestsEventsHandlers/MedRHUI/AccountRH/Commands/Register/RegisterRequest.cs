using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using Application.Functions_Extensions;
using AutoMapper;
using Core.DTO.Models;
using Core.Security.Hashing;
using Core.Security.Jwt;
using Core.Services.MailService;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly EmailSendService _mailSendService;
    private readonly IJwtHelper _jwtHelper;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _uow;

    public RegisterHandler(IAdminRepository adminRepository, IMapper mapper, IJwtHelper jwtHelper, EmailSendService mailSendService, IConfiguration configuration, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _jwtHelper = jwtHelper;
        _mailSendService = mailSendService;
        _configuration = configuration;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var existAdmin = await _adminRepository.ExistAsync(w =>
        w.Username == request.Username || w.Username.ToLower() == request.Username.ToLower() || w.Username.ToUpper() == request.Username.ToUpper() ||
        w.Email.ToLower() == request.Email.ToLower()
        , cancellationToken);
        if (existAdmin)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Lütfen Başka Kullanıcı Ad yada Email Deneyiniz!.", ResponseLogging: true);

        HashingTool.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

        var adminUid = Guid.NewGuid();
        var auth_id = $"{Guid.Empty}.{adminUid}.{Guid.NewGuid()}";
        var claims = new
        {
            FullName = request.Name + " " + request.Surname,
            UserName = request.Username,
            RoleName = Guid.Empty.ToString(),
            AuthID = auth_id,
            AppKey = Guid.Empty.ToString()
        };
        var token = _jwtHelper.CreateToken(claims, $"1 {DateIntervalHelpers.Hour}", $"1 {DateIntervalHelpers.Day}"); // (1 = Gün) Tokenexpirationa eklenecek gün sayısı, Aktivasyon için süreyi uzun tuttuk
        var admin = new AdminDO
        {
            Uid = adminUid,
            Name = request.Name,
            Surname = request.Surname,
            Username = request.Username,
            Email = request.Email,
            AddDate = DateTime.Now,
            IsConfirmed = false,
            IsSuspend = false,
            Phone = request.Phone,
            Password = request.Password,
            UpdateDate = DateTime.Now,
            RoleUid = Guid.Empty,
            PasswordHash = hash,
            PasswordSalt = salt,
            Token = token.Token
        };


        await _adminRepository.AddAsync(_mapper.Map<Admin>(admin), cancellationToken);
        var add = await _uow.SaveChangesAsync(cancellationToken);
        if (add > 0)
        {

            var mailModel = new EmailModel
            {
                DisplayName = "Şifremi Unuttum.",
                Subject = "Lütfen adımları takip ediniz.",
                ReceiveUser = admin.Email,
                MailInfo = new
                {
                    admin.Name,
                    admin.Surname,
                    token.Token,
                    BaseAddress = _configuration["EmailOptions:Configuration:BaseAddress"],
                    ActivationPath = _configuration["EmailOptions:Configuration:Activation"],
                }
            };
            var alternateView = MailFunctions.PasswordForgetTrTemplate(mailModel);
            _ = _mailSendService.SendAsync(alternateView, mailModel);

            result.Message = "Kayıt işlemi başarılı.";
        }
        else
            result.Message = "Kayıt işlemi başarısız!.";

        result.IsSuccess = true;
        //result.Data = model;

        return result;
    }
}


public class RegisterRequest : IRequest<Result<ResponseModel>>
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? RePassword { get; set; }
    public bool TermsAndConditions { get; set; }
}

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public RegisterValidator()
    {
        RuleFor(p => p.Username)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Kullanıcı adı boş olamaz!.")
           .MinimumLength(4).WithMessage("Kullanıcı adı alanına en az 4 karakter girmelisiniz!.")
           .MaximumLength(255).WithMessage("Kullanıcı adı alanına en fazla 255 karakter girmelisiniz!.");

        RuleFor(p => p.Password)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Kullanıcı şifre boş olamaz!.")
           .MinimumLength(4).WithMessage("Kullanıcı şifre alanına en az 4 karakter girmelisiniz!.")
           .MaximumLength(255).WithMessage("Kullanıcı şifre alanına en fazla 255 karakter girmelisiniz!.");
        RuleFor(p => p.RePassword)
            .NotNull().WithMessage("Şifre boş olmamalı")
            .NotEmpty().WithMessage("Şifre boş olmamalı");
        RuleFor(p => p).Must(w => w.Password == w.RePassword).WithMessage("Şifreler eşleşmiyor!");

        RuleFor(p => p.Name)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Ad alanı boş olamaz!.")
           .MinimumLength(3).WithMessage("Ad alanını en az 3 karakter girmelisiniz!.")
           .MaximumLength(255).WithMessage("Ad alanını en fazla 255 karakter girmelisiniz!.");
        RuleFor(p => p.Surname)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Soyad alanı boş olamaz!.")
           .MinimumLength(3).WithMessage("Soyad alanını  adını en az 3 karakter girmelisiniz!.")
           .MaximumLength(255).WithMessage("Soyad alanını  adını en fazla 255 karakter girmelisiniz!.");
        RuleFor(p => p.Email)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Email alanı boş olamaz!.")
           .MinimumLength(11).WithMessage("Email formatı doğru değildir!.")
           .MaximumLength(255).WithMessage("Email formatı doğru değildir!.");
        RuleFor(p => p.Phone)
           .NotNull().WithMessage("Telefon alanı boş olamaz!.")
           .NotEmpty().WithMessage("Telefon alanı boş olamaz!.")
           .Must(w => w.ToCharArray().Where(s => char.IsNumber(s)).Count() == 11).WithMessage("Telefon formatı doğdu değildir!.")
           .MinimumLength(17).WithMessage("Telefon formatı doğru değildir!.")
           .MaximumLength(17).WithMessage("Telefon formatı doğru değildir!.");
        RuleFor(p => p.TermsAndConditions)
           .NotNull().WithMessage("Lütfen koşulları onaylayınız.")
           .Must(w => w == true).WithMessage("Lütfen koşulları onaylayınız.");

        //RuleFor(p => p.Admin.RoleUid)
        //   .Must(w => !w.HasValue).WithMessage("Rol alanı boş olamaz!.");

    }
}
