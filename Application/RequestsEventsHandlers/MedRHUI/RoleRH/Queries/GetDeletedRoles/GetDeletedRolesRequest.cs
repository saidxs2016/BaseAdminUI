using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Queries.GetDeletedRoles;

public class GetDeletedRolesHandler : IRequestHandler<GetDeletedRolesRequest, Result<List<RoleDO>>>
{
    private readonly ILogger<GetDeletedRolesHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;

    public GetDeletedRolesHandler(ILogger<GetDeletedRolesHandler> logger, IMapper mapper, IRoleRepository roleRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _roleRepository = roleRepository;
    }

    public Task<Result<List<RoleDO>>> Handle(GetDeletedRolesRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<RoleDO>>(IsSuccess: false);
        //List<RoleDO> roles = new();

        //var rolesEntity = await _roleRepository.AsQueryable(w => w.IsDeleted == true).ToListAsync(cancellationToken);

        //roles = _mapper.Map<List<RoleDO>>(rolesEntity); 

        //result.IsSuccess = true;
        //result.Data = roles; 
        return Task.FromResult(result);
    }
}

public class GetDeletedRolesRequest : RequestModel, IRequest<Result<List<RoleDO>>>
{
}
