using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.GetAllAdmins;

public class GetAllAdminsHandler : IRequestHandler<GetAllAdminsRequest, Result<List<AdminDO>>>
{
    private readonly ILogger<GetAllAdminsHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllAdminsHandler(ILogger<GetAllAdminsHandler> logger, IMapper mapper, IAdminRepository adminRepository, IRoleRepository roleRepository, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _mapper = mapper;
        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
        _httpContextAccessor = contextAccessor;
    }

    public async Task<Result<List<AdminDO>>> Handle(GetAllAdminsRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<AdminDO>>(IsSuccess: false);

        //var roleName = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.RoleName)?.Value;
        //var authId = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.AuthID)?.Value;
        //var adminUid = Guid.Parse(authId.Split(".")[1]);
        var rolesEntities = await _roleRepository.GetAllAsync(cancellationToken);
        var roles = _mapper.Map<List<RoleDO>>(rolesEntities);

        //var superAdminRole = rolesEntities.FirstOrDefault(i => !i.ParentUid.HasValue);
        //if (!request.Uid.HasValue && superAdminRole.Name == roleName)
        //    request.Uid = superAdminRole.Uid;
        //else if (!request.Uid.HasValue)
        //    request.Uid = roles.FirstOrDefault(w => w.Name == roleName)?.Uid;

        var role = roles.FirstOrDefault(i => i.Uid == request.Uid);

        role = RoleToTree(role, roles);
        var targetRoles = TreeToRole(role);
        targetRoles.Add(role);

        var targetRolesUids = targetRoles.Select(i => i.Uid).ToList();

        var adminsEntities = await _adminRepository.GetAsWhereAsync(i => targetRolesUids.Contains(i.RoleUid.Value));

        var admins = _mapper.Map<List<AdminDO>>(adminsEntities) ?? new List<AdminDO>();

        //var deletedAdmins = admins.Where(i => i.RoleUid == role.Uid && i.Uid != adminUid).Select(w => w.Uid).ToList();
        //admins = admins.Where(i => !deletedAdmins.Contains(i.Uid)).ToList();

        admins.ForEach(i => i.CanEdit = false);
        result.IsSuccess = true;
        //result.Data = _mapper.Map<List<AdminDO>>(sub_roles); 
        result.Data = admins;

        return result;
    }

    public List<RoleDO> TreeToRole(RoleDO parent)
    {
        List<RoleDO> result = new();

        if (parent.SubRoleList != null && parent.SubRoleList.Count > 0)
            foreach (RoleDO t in parent.SubRoleList)
            {
                if (t.SubRoleList != null && t.SubRoleList.Count > 0)
                    result.AddRange(TreeToRole(t));
                result.Add(t);
            }
        return result;
    }
    public RoleDO RoleToTree(RoleDO parent, List<RoleDO> arr)
    {
        var tmp_arr = arr.Where(i => i.ParentUid == parent.Uid).ToList();
        if (tmp_arr != null && tmp_arr.Count > 0)
            foreach (RoleDO t in tmp_arr)
            {
                var sub_arr = arr.Where(i => i.ParentUid == t.Uid).ToList();
                if (sub_arr != null && sub_arr.Count > 0)
                    RoleToTree(t, arr);
                parent.SubRoleList ??= new List<RoleDO>();
                parent.SubRoleList.Add(t);
            }

        parent.SubRoleList ??= new List<RoleDO>();
        parent.SubRoleList = tmp_arr;
        return parent;
    }
}

public class GetAllAdminsRequest : RequestModel, IRequest<Result<List<AdminDO>>>
{
    public Guid? Uid { get; set; } // role uid
}


public class GetAllAdminsValidator : AbstractValidator<GetAllAdminsRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public GetAllAdminsValidator()
    {
        RuleFor(p => p.Uid)
            .NotNull().WithMessage("Bu alana erişiminiz bulunmamaktadır!")
            .NotEmpty().WithMessage("Bu alana erişiminiz bulunmamaktadır!");

    }
}
