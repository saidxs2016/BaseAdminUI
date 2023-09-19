using Application.DTO.DataObjects;
using Application.DTO.Exceptions;
using Application.DTO.Models;
using Application.DTO.ResultType;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleAdd;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleUpdate;

public class RoleUpdateHandler : IRequestHandler<RoleUpdateRequest, Result<ResponseModel>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public RoleUpdateHandler(IRoleRepository adminRolRepository, IMapper mapper, IUnitOfWork uow)
    {
        _roleRepository = adminRolRepository;
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(RoleUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();


        var roleEntity = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Role.Uid, cancellationToken);
        if (roleEntity == null)
        {
            var logResult = new Result<object>() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Rol bulunamadı." };
            throw new ModelException(logResult);
        }

        var existRole = await _roleRepository.ExistAsync(w => w.Uid != request.Role.Uid && w.Name == request.Role.Name, cancellationToken);
        if (existRole)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Lütfen Başka Ad Deneyiniz!.", ResponseLogging: true);

        roleEntity.Slug = BaseFunctions.FriendlyUrl(request.Role.Name);
        roleEntity.UpdateDate = DateTime.Now;

        roleEntity.Name = request.Role.Name;
        roleEntity.Route = request.Role.Route;

        _roleRepository.Update(roleEntity);
        var update = await _uow.SaveChangesAsync();
        if (update > 0)
            return new Result<ResponseModel>(IsSuccess: true, Message: "Güncelleme işlemi başarılı.", ResponseLogging: true);
        else
            return new Result<ResponseModel>(IsSuccess: false, Message: "Güncelleme işlemi başarısız!.", ResponseLogging: true);

    }
}
public class RoleUpdateRequest : IRequest<Result<ResponseModel>>
{
    public RoleDO? Role { get; set; }

}
public class RoleUpdateValidator : AbstractValidator<RoleUpdateRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public RoleUpdateValidator()
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
