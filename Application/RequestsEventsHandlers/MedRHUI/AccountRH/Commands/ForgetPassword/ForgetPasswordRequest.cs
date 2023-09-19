using Application.DTO.Models;
using Application.DTO.ResultType;
using Application.Functions_Extensions;
using Core.DTO.Models;
using Core.Security.Jwt;
using Core.Services.MailService;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.ForgetPassword;

public class ForgetPasswordHandler : IRequestHandler<ForgetPasswordRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly EmailSendService _mailSendService;
    private readonly IJwtHelper _jwtHelper;
    private readonly IConfiguration _configuration;
    private readonly IRoleRepository _roleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;

    public ForgetPasswordHandler(IAdminRepository adminRepository, EmailSendService mailSendService, IJwtHelper jwtHelper, IConfiguration configuration, IRoleRepository roleRepository, IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mailSendService = mailSendService;
        _jwtHelper = jwtHelper;
        _configuration = configuration;
        _roleRepository = roleRepository;
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ForgetPasswordRequest request, CancellationToken cancellationToken)
    {

        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Email == request.Email, cancellationToken);
        if (admin == null)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Mail Adresi Sistemde Bulunmamaktadır.", ResponseLogging: true);
        if (admin.IsConfirmed != true)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Aktivasyon işlemi gerçekleştirmemişsiniz.", ResponseLogging: true);

        if (admin.RoleUid == Guid.Empty)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Sistem kullanıcısı değilsiniz henüz.", ResponseLogging: true);

        if (!string.IsNullOrEmpty(admin.Token))
        {
            var (full_name, _, _, _, _, _, _) = _jwtHelper.ValidateToken(admin.Token);
            if (!string.IsNullOrEmpty(full_name))
                return new Result<ResponseModel>(IsSuccess: false, Message: "Şifre sıfırlama linki e-postanıza daha önce gönderildi.", ResponseLogging: true);
        }

        var role = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == admin.RoleUid, cancellationToken);

        var auth_id = $"{role.Slug}.{admin.Uid}.{Guid.NewGuid()}";
        var claims = new { FullName = admin.Name + " " + admin.Surname, UserName = admin.Username, RoleName = role.Name, AuthID = auth_id, AppKey = Guid.Empty.ToString() };
        var token = _jwtHelper.CreateToken(claims, role.Expiration);

        admin.Token = token.Token;
        admin.UpdateDate = DateTime.Now;


        _adminRepository.Update(admin);
        var update = await _uow.SaveChangesAsync(cancellationToken);
        if (update > 0)
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
                    ResetPasswordPath = _configuration["EmailOptions:Configuration:ResetPassword"],
                }
            };
            var alternateView = MailFunctions.PasswordForgetTrTemplate(mailModel);
            _ = _mailSendService.SendAsync(alternateView, mailModel);

            result.Message = "İşlem başarılı. Mailinizi kontrol ediniz!";
        }
        else
            result.Message = "İşlem başarısız!.";

        //model.TerminalListPaginated = data;
        result.IsSuccess = true;
        //result.Data = model;

        return result;
    }
}


public class ForgetPasswordRequest : IRequest<Result<ResponseModel>>
{
    public string? Email { get; set; }
}

public class ForgetPasswordValidator : AbstractValidator<ForgetPasswordRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public ForgetPasswordValidator()
    {
        RuleFor(p => p.Email)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Email alanı boş olamaz!.")
           .MinimumLength(11).WithMessage("Email formatı doğru değildir!.")
           .MaximumLength(255).WithMessage("Email formatı doğru değildir!.");

    }
}
