using Application.DTO.Models;
using Application.DTO.ResultType;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleChangeParent;

public class ModuleChangeParentHandler : IRequestHandler<ModuleChangeParentRequest, Result<ResponseModel>>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;

    public ModuleChangeParentHandler(IModuleRepository adminRolRepository, IPermissionRepository permissionRepository, IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
    {
        _moduleRepository = adminRolRepository;
        _permissionRepository = permissionRepository;
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ModuleChangeParentRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var moduleEntity = await _moduleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid, cancellationToken);
        if (moduleEntity == null)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Modül bulunamadı.");

        moduleEntity.ParentUid = request.ParentUid;
        moduleEntity.UpdateDate = DateTime.Now;

        _moduleRepository.Update(moduleEntity);
        var update = await _uow.SaveChangesAsync();
        if (update > 0)
        {
            result.IsSuccess = true;
            result.Message = "Parent Change işlemi başarılı.";
        }
        else
            result.Message = "Parent Change işlemi başarısız!.";

        return result;
    }
}


public class ModuleChangeParentRequest : IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; }
    public Guid? ParentUid { get; set; }

}

public class ModuleChangeParentValidator : AbstractValidator<ModuleChangeParentRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public ModuleChangeParentValidator()
    {

        RuleFor(p => p.Uid)
            .NotNull().WithMessage("Uid boş olmalı!")
            .NotEmpty().WithMessage("Uid boş olmamalı!");
        RuleFor(p => p.ParentUid)
            .NotNull().WithMessage("Parent boş olmalı!")
            .NotEmpty().WithMessage("Parent boş olmamalı!");
    }
}
