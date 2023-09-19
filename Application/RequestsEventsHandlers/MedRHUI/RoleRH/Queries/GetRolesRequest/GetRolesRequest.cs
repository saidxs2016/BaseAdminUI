using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Queries.GetRolesRequest;

public class GetRolesHandler : IRequestHandler<GetRolesRequest, Result<List<RoleDO>>>
{
    private readonly ILogger<GetRolesHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _uow;

    public GetRolesHandler(ILogger<GetRolesHandler> logger, IMapper mapper, IAdminRepository adminRepository, IRoleRepository roleRepository, IHttpContextAccessor httpContextAccessor, IPermissionRepository permissionRepository, IMemoryCacheService memoryCacheService, IMediator mediator, IUnitOfWork uow)
    {
        _logger = logger;
        _mapper = mapper;
        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
        _httpContextAccessor = httpContextAccessor;
        _permissionRepository = permissionRepository;
        _memoryCacheService = memoryCacheService;
        _mediator = mediator;
        _uow = uow;
    }

    public async Task<Result<List<RoleDO>>> Handle(GetRolesRequest request, CancellationToken cancellationToken)
    {

        var result = new Result<List<RoleDO>>(IsSuccess: false);
        List<RoleDO> roles = new();
        var roleName = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.RoleName)?.Value ?? null;

        var role = await _roleRepository.GetAsFirstOrDefaultAsync(i => i.Name == roleName);

        var sub_roles = await _roleRepository.GetAsWhereAsync(i => i.ParentUid == role.Uid);

        if (!role.ParentUid.HasValue)
            sub_roles.Add(role);


        roles = _mapper.Map<List<RoleDO>>(sub_roles);
        foreach (var item in roles)
        {
            item.AdminsCount = await _adminRepository.GetCountAsync(i => i.RoleUid == item.Uid);
            item.PermissionsCount = await _permissionRepository.GetCountAsync(i => i.RoleUid == item.Uid);
        }

        result.IsSuccess = true;
        result.Data = roles;

        return result;
    }
}

public class GetRolesRequest : RequestModel, IRequest<Result<List<RoleDO>>>
{
}
