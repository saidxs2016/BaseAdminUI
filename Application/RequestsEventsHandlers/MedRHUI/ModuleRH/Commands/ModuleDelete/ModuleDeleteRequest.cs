using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleDelete;

public class ModuleDeleteHandler : IRequestHandler<ModuleDeleteRequest, Result<ResponseModel>>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _uow;

    public ModuleDeleteHandler(IModuleRepository adminRolRepository, IPermissionRepository permissionRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IMemoryCacheService memoryCacheService, IRoleRepository roleRepository, IUnitOfWork uow)
    {
        _moduleRepository = adminRolRepository;
        _permissionRepository = permissionRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _memoryCacheService = memoryCacheService;
        _roleRepository = roleRepository;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ModuleDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var authId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.AuthID).Value;
        var adminUid = Guid.Parse(authId.Split(".")[1]);

        var moduleEntity = await _moduleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid, cancellationToken);
        if (moduleEntity == null)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Modül bulunamadı.");


        var modulesEntities = await _moduleRepository.GetAllAsync(cancellationToken);
        var modules = _mapper.Map<List<ModuleDO>>(modulesEntities);
        var module = modules.FirstOrDefault(i => i.Uid == request.Uid);

        module = ModuleToTree(module, modules);
        var targetModules = TreeToModule(module);
        targetModules.Add(module);
        var targetModulesUids = targetModules.Select(w => w.Uid).ToList();


        var permissions = await _permissionRepository.GetAsWhereAsync(w => targetModulesUids.Contains(w.ModuleUid.Value), cancellationToken);
        if (permissions != null)
            permissions.ForEach((item) =>
            {
                item.ByAdmin = adminUid;
                item.UpdateDate = DateTime.Now;
            });
        _permissionRepository.DeleteRange(permissions);


        var roles_uids = permissions.Select(i => i.RoleUid).Distinct().ToList();
        var roles = await _roleRepository.GetAsWhereAsync(r => roles_uids.Contains(r.Uid));
        if (roles != null)
        {
            roles = roles.Where(i => i.ParentUid.HasValue).ToList();
            roles.ForEach(i => _memoryCacheService.RemoveByPattern($"{i.Slug}\\."));
        }


        moduleEntity.UpdateDate = DateTime.Now;

        _moduleRepository.Delete(moduleEntity);
        var delete = await _uow.SaveChangesAsync();
        if (delete > 0)
        {
            /// -SignalR bildirim gönderilecek
            result.IsSuccess = true;
            result.Message = "Silme işlemi başarılı.";
        }
        else
            result.Message = "Silme işlemi başarısız!.";

        return result;
    }
    public List<ModuleDO> TreeToModule(ModuleDO parent)
    {
        List<ModuleDO> result = new();

        if (parent.SubModuleList != null && parent.SubModuleList.Count > 0)
            foreach (ModuleDO t in parent.SubModuleList)
            {
                if (t.SubModuleList != null && t.SubModuleList.Count > 0)
                    result.AddRange(TreeToModule(t));
                result.Add(t);
            }
        return result;
    }
    public ModuleDO ModuleToTree(ModuleDO parent, List<ModuleDO> arr)
    {
        var tmp_arr = arr.Where(i => i.ParentUid == parent.Uid).ToList();
        if (tmp_arr != null && tmp_arr.Count > 0)
            foreach (ModuleDO t in tmp_arr)
            {
                var sub_arr = arr.Where(i => i.ParentUid == t.Uid).ToList();
                if (sub_arr != null && sub_arr.Count > 0)
                    ModuleToTree(t, arr);
                parent.SubModuleList ??= new List<ModuleDO>();
                parent.SubModuleList.Add(t);
            }

        parent.SubModuleList ??= new List<ModuleDO>();
        parent.SubModuleList = tmp_arr;
        return parent;
    }
}


public class ModuleDeleteRequest : IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; }

}

public class ModuleDeleteValidator : AbstractValidator<ModuleDeleteRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public ModuleDeleteValidator()
    {

    }
}
