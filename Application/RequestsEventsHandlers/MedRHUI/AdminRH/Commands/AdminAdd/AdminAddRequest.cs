using Application.DTO.DataObjects;
using Application.DTO.Exceptions;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Security.Hashing;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.AdminAdd;

public class AdminAddHandler : IRequestHandler<AdminAddRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _uow;

    public AdminAddHandler(IAdminRepository adminRepository, IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IRoleRepository roleRepository, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _env = env;
        _httpContextAccessor = httpContextAccessor;
        _roleRepository = roleRepository;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(AdminAddRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var adminCount = await _adminRepository.GetCountAsync(w => true);
        if (adminCount == 0)
        {
            var existRole = await _roleRepository.AsQueryable().SingleAsync(w => w.ParentUid == null);
            if (existRole == null)
                throw new ModelException(new Result<object>(IsSuccess: false, Message: "Lütfen önce bir ROL ekleyiniz!.", ResponseLogging: true));

            request.Admin.RoleUid = existRole.Uid;
        }
        else
        {
            var existAdmin = await _adminRepository.ExistAsync(w =>
            w.Username == request.Admin.Username || w.Username.ToLower() == request.Admin.Username.ToLower() || w.Username.ToUpper() == request.Admin.Username.ToUpper() ||
            w.Email == request.Admin.Email
            , cancellationToken);
            if (existAdmin)
                return new Result<ResponseModel>(IsSuccess: false, Message: "Lütfen Başka Kullanıcı Ad yada Email Deneyiniz!.", ResponseLogging: true);

            var roleName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.RoleName).Value;
            var role = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Name == roleName, cancellationToken);
            if (role == null)
                throw new ModelException(new Result<object>(IsSuccess: false, Message: "Lütfen Başka Kullanıcı Ad yada Email Deneyiniz!.", ResponseLogging: true));
            var selectedRole = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Admin.RoleUid, cancellationToken);
            if (selectedRole != null && selectedRole.ParentUid != role.Uid)
                throw new ModelException(new Result<object>(IsSuccess: false, Message: "Yetkisiz erişim tespit edildi!", ResponseLogging: true));
        }

        request.Admin.AddDate = DateTime.Now;
        request.Admin.UpdateDate = DateTime.Now;
        //request.adminDO.AccountConfirm = false;

        HashingTool.CreatePasswordHash(request.Admin.Password, out byte[] hash, out byte[] salt);

        request.Admin.PasswordHash = hash;
        request.Admin.PasswordSalt = salt;

        request.Admin.IsConfirmed = true;
        request.Admin.IsSuspend = false;

        if (!_env.IsDevelopment())
            request.Admin.Password = null;


        await _adminRepository.AddAsync(_mapper.Map<Admin>(request.Admin));
        var add = await _uow.SaveChangesAsync();
        if (add > 0)
            result.Message = "Kayıt işlemi başarılı.";
        else
            result.Message = "Kayıt işlemi başarısız!.";

        //model.TerminalListPaginated = data;
        result.IsSuccess = true;
        //result.Data = model;

        return result;
    }
}


public class AdminAddRequest : IRequest<Result<ResponseModel>>
{
    public AdminDO? Admin { get; set; }
}

public class AdminAddValidator : AbstractValidator<AdminAddRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public AdminAddValidator()
    {
        RuleFor(p => p.Admin)
            .NotNull().WithMessage("Kullanıcı bilgileri eksik!:");
        RuleFor(p => p.Admin.Username)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Kullanıcı adı boş olamaz!.")
           .MinimumLength(4).WithMessage("Kullanıcı adı alanına en az 4 karakter girmelisiniz!.")
           .MaximumLength(255).WithMessage("Kullanıcı adı alanına en fazla 255 karakter girmelisiniz!.");
        RuleFor(p => p.Admin.Password)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Kullanıcı şifresi boş olamaz!.")
           .MinimumLength(4).WithMessage("Kullanıcı şifre alanına en az 4 karakter girmelisiniz!.")
           .MaximumLength(255).WithMessage("Kullanıcı şifre alanına en fazla 255 karakter girmelisiniz!.");

        RuleFor(p => p.Admin.Name)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Ad alanı boş olamaz!.")
           .MinimumLength(3).WithMessage("Ad alanını en az 3 karakter girmelisiniz!.")
           .MaximumLength(255).WithMessage("Ad alanını en fazla 255 karakter girmelisiniz!.");
        RuleFor(p => p.Admin.Surname)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Soyad alanı boş olamaz!.")
           .MinimumLength(3).WithMessage("Soyad alanını  adını en az 3 karakter girmelisiniz!.")
           .MaximumLength(255).WithMessage("Soyad alanını  adını en fazla 255 karakter girmelisiniz!.");
        RuleFor(p => p.Admin.Email)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Email alanı boş olamaz!.")
           .MinimumLength(11).WithMessage("Email formatı doğru değildir!.")
           .MaximumLength(255).WithMessage("Email formatı doğru değildir!.");
        RuleFor(p => p.Admin.Phone)
           .NotNull().WithMessage("Telefon alanı boş olamaz!.")
           .NotEmpty().WithMessage("Telefon alanı boş olamaz!.")
           .Must(w => w.ToCharArray().Where(s => char.IsNumber(s)).Count() == 11).WithMessage("Telefon formatı doğdu değildir!.")
           .MinimumLength(17).WithMessage("Telefon formatı doğru değildir!.")
           .MaximumLength(17).WithMessage("Telefon formatı doğru değildir!.");

        //RuleFor(p => p.Admin.RoleUid)
        //   .Must(w => !w.HasValue).WithMessage("Rol alanı boş olamaz!.");

    }
}
