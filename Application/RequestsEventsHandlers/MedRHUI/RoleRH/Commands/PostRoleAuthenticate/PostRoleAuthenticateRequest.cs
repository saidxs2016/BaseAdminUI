using Application.DTO.DataObjects;
using Application.DTO.Exceptions;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.PostRoleAuthenticate;

public class PostRoleAuthenticateHandler : IRequestHandler<PostRoleAuthenticateRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IUnitOfWork _uow;

    public PostRoleAuthenticateHandler(IAdminRepository adminRepository, IRoleRepository roleRepository, IModuleRepository moduleRepository, IPermissionRepository permissionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IMemoryCacheService memoryCacheService, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
        _moduleRepository = moduleRepository;
        _permissionRepository = permissionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _memoryCacheService = memoryCacheService;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(PostRoleAuthenticateRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false);
        ResponseModel model = new();

        var AuthID = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.AuthID).Value;
        var byAdmin = Guid.Parse(AuthID.Split(".")[1]);

        var selectedRole = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid, cancellationToken);
        if (selectedRole == null)
            throw new ModelException(new Result<object>(IsSuccess: false, Message: "Seçilen Rol bulunamadı!.", ResponseLogging: true));


        var roleName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.RoleName).Value;
        var role = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Name == roleName);
        if (role == null)
            throw new ModelException(new Result<object>(IsSuccess: false, Message: "Rol bulunamadı!.", ResponseLogging: true));


        List<string> ignoredSections = new();
        var pages = request.Pages ?? new List<ModuleDO>();
        pages = pages.Where(i => i.Type == ModuleHelper.Page).ToList() ?? new List<ModuleDO> { };

        var permissionsEntities = await _permissionRepository.GetAsWhereAsync(w => w.RoleUid == request.Uid, cancellationToken) ?? new List<Permission>(); //mevcut
                                                                                                                                                           // var permissions = _mapper.Map<List<PermissionDO>>(permissionsEntities);
        permissionsEntities.ForEach(p =>
        {
            var page = pages.FirstOrDefault(i => i.Uid == p.ModuleUid) ?? new ModuleDO();
            p.UpdateDate = DateTime.Now;
            page.IgnoredSectionList ??= new List<string> { };
            p.IgnoredSections = page.IgnoredSectionList.SerializeWithCamelCase();
        });
        _permissionRepository.UpdateRange(permissionsEntities);


        var oldModuleUids = permissionsEntities.Select(w => w.ModuleUid).ToList();
        var newAddedUids = request.CheckedArr.Where(w => !oldModuleUids.Contains(w)).ToList();
        var deletedUids = oldModuleUids.Where(w => !request.CheckedArr.Contains(w.Value)).ToList();

        if (deletedUids != null)
        {
            var rolesEntity = await _roleRepository.GetAllAsync(cancellationToken);
            var parentRoleEntity = rolesEntity.FirstOrDefault(w => w.Uid == request.Uid);
            var parentRole = RoleToTree(_mapper.Map<RoleDO>(parentRoleEntity), _mapper.Map<List<RoleDO>>(rolesEntity));
            var targetRoles = TreeToRole(parentRole);
            targetRoles.Add(parentRole);
            var targetRolesUid = targetRoles.Select(w => w.Uid).ToList();


            var deleteList = permissionsEntities.Where(w => deletedUids.Contains(w.ModuleUid) && targetRolesUid.Contains(w.RoleUid.Value)).ToList();
            deleteList.ForEach((item) =>
            {
                item.UpdateDate = DateTime.Now;
            });

            _permissionRepository.DeleteRange(deleteList);
        }
        if (newAddedUids != null)
        {
            var newPermissions = newAddedUids.Select((item) =>
            {

                var page = pages.FirstOrDefault(i => i.Uid == item) ?? new ModuleDO();

                page.IgnoredSectionList ??= new List<string> { };

                return new PermissionDO
                {
                    RoleUid = request.Uid,
                    ModuleUid = item,
                    AddDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    IgnoredSections = page.IgnoredSectionList.SerializeWithCamelCase(),
                    ByAdmin = byAdmin,

                };
            }).ToList();

            await _permissionRepository.AddRangeAsync(_mapper.Map<List<Permission>>(newPermissions), cancellationToken);
        }

        var state = await _uow.SaveChangesAsync(cancellationToken);
        if (state > 0)
        {
            _memoryCacheService.RemoveByPattern($"^{selectedRole.Slug}\\.");
        }

        result.Message = "İşlem Başarılı";
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

public class PostRoleAuthenticateRequest : IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; } // role uid
    public List<Guid>? CheckedArr { get; set; } // seçilen modül uid'ler

    public List<ModuleDO>? Pages { get; set; } // seçilen modül'ler, n-bu modüller içinde ignore'ler


}
public class PostRoleAuthenticateValidator : AbstractValidator<PostRoleAuthenticateRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public PostRoleAuthenticateValidator()
    {

    }
}
