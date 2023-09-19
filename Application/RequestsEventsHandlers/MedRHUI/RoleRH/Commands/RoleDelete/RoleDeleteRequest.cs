using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleDelete;

public class RoleDeleteHandler : IRequestHandler<RoleDeleteRequest, Result<ResponseModel>>
{
    private readonly IRoleRepository _roleRepository;

    public RoleDeleteHandler(IRoleRepository adminRolRepository)
    {
        _roleRepository = adminRolRepository;
    }

    public Task<Result<ResponseModel>> Handle(RoleDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();



        return Task.FromResult(result);
    }
}


public class RoleDeleteRequest : IRequest<Result<ResponseModel>>
{
    public RoleDO? Role { get; set; }

}

public class RoleDeleteValidator : AbstractValidator<RoleDeleteRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public RoleDeleteValidator()
    {

    }
}
