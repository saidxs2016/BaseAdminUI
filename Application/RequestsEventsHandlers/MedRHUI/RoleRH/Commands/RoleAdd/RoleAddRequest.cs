using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleAdd;

public class RoleAddHandler : IRequestHandler<RoleAddRequest, Result<ResponseModel>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUnitOfWork _uow;

    public RoleAddHandler(IRoleRepository adminRolRepository, IMapper mapper, IHttpContextAccessor contextAccessor, IUnitOfWork uow)
    {
        _roleRepository = adminRolRepository;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(RoleAddRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>();
        var model = new ResponseModel();

        var roleCount = await _roleRepository.GetCountAsync(w => true);
        if (roleCount == 0)
        {
            request.Role.ParentUid = null;
        }
        else
        {
            var roleName = _contextAccessor.HttpContext.User?.Claims.FirstOrDefault(w => w.Type == ClaimHelper.RoleName)?.Value ?? "Super Admin";
            var adminRole = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Name == roleName);
            request.Role.ParentUid = adminRole.Uid;
        }

        var existRole = await _roleRepository.ExistAsync(w => w.Name == request.Role.Name, cancellationToken);
        if (existRole)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Lütfen Başka Ad Deneyiniz!.", ResponseLogging: true);



        request.Role.Slug = BaseFunctions.FriendlyUrl(request.Role.Name);
        request.Role.Expiration = "1 " + DateIntervalHelpers.Day;
        request.Role.AddDate = DateTime.Now;
        request.Role.UpdateDate = DateTime.Now;
        await _roleRepository.AddAsync(_mapper.Map<Role>(request.Role));
        var add = await _uow.SaveChangesAsync();
        if (add > 0)
            result.Message = "Kayıt işlemi başarılı.";
        else
            result.Message = "Kayıt işlemi başarısız!.";
        result.IsSuccess = true;

        return result;
    }
}

public class RoleAddRequest : IRequest<Result<ResponseModel>>
{
    public RoleDO? Role { get; set; }
}


public class RoleAddValidator : AbstractValidator<RoleAddRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public RoleAddValidator()
    {
        RuleFor(p => p.Role)
            .NotNull().WithMessage("Adam ol veriyi düzgün gir");
        RuleFor(p => p.Role.Name)
           .NotNull().WithMessage("Rol adı boş olamaz!")
           .NotEmpty().WithMessage("Rol adı boş olamaz!")
           .MinimumLength(3).WithMessage("En az 3 karakter girmelisiniz!.")
           .Must(i => !i.Contains('.')).WithMessage("Rol adı nokta içeremez.");

    }
}
