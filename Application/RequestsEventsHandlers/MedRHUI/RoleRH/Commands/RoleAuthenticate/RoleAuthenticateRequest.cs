using Application.DTO.DataObjects;
using Application.DTO.Exceptions;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleAuthenticate;

public class RoleAuthenticateHandler : IRequestHandler<RoleAuthenticateRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoleAuthenticateHandler(IAdminRepository adminRepository, IRoleRepository roleRepository, IModuleRepository moduleRepository, IPermissionRepository permissionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
        _moduleRepository = moduleRepository;
        _permissionRepository = permissionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<ResponseModel>> Handle(RoleAuthenticateRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false);
        ResponseModel model = new();

        var roleExist = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid);
        if (roleExist == null)
            throw new ModelException(new Result<object>(IsSuccess: false, Message: "Seçilen Rol bulunamadı!.", ResponseLogging: true));


        var roleName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.RoleName).Value;
        var role = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Name == roleName);
        if (role == null)
            throw new ModelException(new Result<object>(IsSuccess: false, Message: "Rol bulunamadı!.", ResponseLogging: true));


        var permissions = await _permissionRepository.GetAsWhereAsync(w => w.RoleUid == role.Uid, cancellationToken);


        var moduleUids = permissions.Select(w => w.ModuleUid).ToArray();


        var modulesEntity = await _moduleRepository.AsQueryable(w => moduleUids.Contains(w.Uid)).OrderBy(w => w.Order).ToListAsync(cancellationToken: cancellationToken);

        if (!role.ParentUid.HasValue)
            modulesEntity = await _moduleRepository.GetAllAsync(cancellationToken);


        var modules = _mapper.Map<List<ModuleDO>>(modulesEntity);

        var targetRolePermissions = await _permissionRepository.GetAsWhereAsync(w => w.RoleUid == request.Uid, cancellationToken);

        List<Guid> checkedModules = new();


        modules.ForEach((item) =>
        {
            item.IsChecked = targetRolePermissions.Exists(w => w.ModuleUid == item.Uid);
            if (item.IsChecked)
                checkedModules.Add(item.Uid);

            var selectedPermission = targetRolePermissions.FirstOrDefault(i => i.ModuleUid == item.Uid);

            if (selectedPermission != null && string.IsNullOrEmpty(selectedPermission.IgnoredSections))
                selectedPermission.IgnoredSections = "[]";


            if (selectedPermission != null && selectedPermission.Uid != Guid.Empty)
            {
                var oldIgnoredSections = selectedPermission.IgnoredSections.DeserializeFromCamelCase<List<string>>() ?? new List<string>();
                item.IgnoredSectionList = oldIgnoredSections;
            }


        });

        var mainModules = modules.Where(w => !w.ParentUid.HasValue).ToList();
        if (mainModules != null)
            foreach (var category in mainModules)
            {
                category.SubModuleList = modules.Where(w => w.ParentUid == category.Uid).ToList();
                if (category.SubModuleList != null)
                    foreach (var page in category.SubModuleList)
                        page.SubModuleList = modules.Where(w => w.ParentUid == page.Uid).ToList();
            }

        model.ModuleList = mainModules;
        model.CheckedModules = checkedModules;

        result.IsSuccess = true;
        result.Data = model;

        return result;
    }
}
public class RoleAuthenticateRequest : IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; } // role uid

}
public class RoleAuthenticateValidator : AbstractValidator<RoleAuthenticateRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public RoleAuthenticateValidator()
    {

    }
}
