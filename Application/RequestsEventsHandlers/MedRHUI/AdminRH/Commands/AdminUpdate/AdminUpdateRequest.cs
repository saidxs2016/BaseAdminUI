using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.AdminUpdate;

public class AdminUpdateHandler : IRequestHandler<AdminUpdateRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _uow;

    public AdminUpdateHandler(IAdminRepository adminRepository, IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IRoleRepository roleRepository, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _env = env;
        _httpContextAccessor = httpContextAccessor;
        _roleRepository = roleRepository;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(AdminUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var existAdmin = await _adminRepository.ExistAsync(w => w.Uid != request.Admin.Uid &&
            (w.Username == request.Admin.Username || w.Username.ToLower() == request.Admin.Username.ToLower() || w.Username.ToUpper() == request.Admin.Username.ToUpper() ||
            w.Email.ToLower() == request.Admin.Email.ToLower())
            , cancellationToken);
        if (existAdmin)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Lütfen Başka Kullanıcı Ad yada Email Deneyiniz!.", ResponseLogging: true);


        var oldAdmin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Admin.Uid, cancellationToken);
        oldAdmin.UpdateDate = DateTime.Now;

        oldAdmin.Email = request.Admin.Email;
        oldAdmin.Name = request.Admin.Name;
        oldAdmin.Surname = request.Admin.Surname;
        oldAdmin.Phone = request.Admin.Phone;
        oldAdmin.Title = request.Admin.Title;
        oldAdmin.Username = request.Admin.Username;

        if (!_env.IsDevelopment())
            request.Admin.Password = null;


        _adminRepository.Update(oldAdmin);
        var update = await _uow.SaveChangesAsync();
        if (update > 0)
            result.Message = "Güncelleme işlemi başarılı.";
        else
            result.Message = "Güncelleme işlemi başarısız!.";

        model.Role = new RoleDO { Uid = oldAdmin.RoleUid.Value };
        result.IsSuccess = true;
        result.Data = model;

        return result;
    }
}


public class AdminUpdateRequest : IRequest<Result<ResponseModel>>
{
    public AdminDO? Admin { get; set; }
}

public class AdminUpdateValidator : AbstractValidator<AdminUpdateRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public AdminUpdateValidator()
    {
        RuleFor(p => p.Admin)
            .NotNull().WithMessage("Kullanıcı bilgileri eksik!:");
        RuleFor(p => p.Admin.Username)
           .Must(w => !string.IsNullOrEmpty(w)).WithMessage("Kullanıcı adı boş olamaz!.")
           .MinimumLength(4).WithMessage("Kullanıcı adı alanına en az 4 karakter girmelisiniz!.")
           .MaximumLength(255).WithMessage("Kullanıcı adı alanına en fazla 255 karakter girmelisiniz!.");
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
