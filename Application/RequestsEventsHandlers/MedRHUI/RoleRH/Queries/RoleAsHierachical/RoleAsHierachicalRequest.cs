using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Queries.RoleAsHierachical
{
    public class RoleAsHierachicalHandler : IRequestHandler<RoleAsHierachicalRequest, Result<ResponseModel>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleAsHierachicalHandler(IRoleRepository roleRepository, IAdminRepository adminRepository, IPermissionRepository permissionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _roleRepository = roleRepository;
            _adminRepository = adminRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<ResponseModel>> Handle(RoleAsHierachicalRequest request, CancellationToken cancellationToken)
        {
            var result = new Result<ResponseModel>();
            var model = new ResponseModel();

            var roleName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.RoleName)?.Value;

            var roleEntities = await _roleRepository.GetAllAsync(cancellationToken);
            var roleEntity = roleEntities.FirstOrDefault(i => i.Name == roleName);
            var roles = _mapper.Map<List<RoleDO>>(roleEntities);
            var role = _mapper.Map<RoleDO>(roleEntity);


            role = RoleToTree(role, roles);
            var targetRoles = TreeToRole(role);
            var reOrderTargetRoles = new List<RoleDO>() { role };
            reOrderTargetRoles.AddRange(targetRoles);

            foreach (var item in reOrderTargetRoles)
            {
                item.AdminsCount = await _adminRepository.GetCountAsync(i => i.RoleUid == item.Uid);
                item.PermissionsCount = await _permissionRepository.GetCountAsync(i => i.RoleUid == item.Uid);
            }


            model.RoleList = reOrderTargetRoles;
            result.IsSuccess = true;
            result.Data = model;
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
    public class RoleAsHierachicalRequest : RequestModel, IRequest<Result<ResponseModel>>
    {

    }
}
