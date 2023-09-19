using Application.DTO.DataObjects;
using Application.DTO.Exceptions;
using Application.DTO.Models;
using Application.DTO.ResultType;
using Application.Functions_Extensions;
using AutoMapper;
using Core.Security.Jwt;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.LoginByConnectionKey;

public class LoginByConnectionKeyHandler : IRequestHandler<LoginByConnectionKeyRequest, Result<ResponseModel>>
{
    private readonly ILogger<LoginByConnectionKeyHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IJwtHelper _jwtHelper;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;



    public LoginByConnectionKeyHandler(ILogger<LoginByConnectionKeyHandler> logger, IMapper mapper, IAdminRepository adminRepository, IRoleRepository roleRepository, IModuleRepository moduleRepository, IJwtHelper jwtHelper, IMemoryCacheService memoryCacheService, IHttpContextAccessor contextAccessor, IPermissionRepository permissionRepository, IUnitOfWork uow)
    {
        _logger = logger;
        _mapper = mapper;
        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
        _moduleRepository = moduleRepository;
        _jwtHelper = jwtHelper;
        _memoryCacheService = memoryCacheService;
        _httpContextAccessor = contextAccessor;
        _permissionRepository = permissionRepository;
        _uow = uow;
    }
    public async Task<Result<ResponseModel>> Handle(LoginByConnectionKeyRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: true, Message: "", ResponseLogging: true);
        var model = new ResponseModel();
        try
        {
            var authId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.AuthID).Value;
            var roleName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.RoleName)?.Value;

            var role_entity = await _roleRepository.GetAsFirstOrDefaultAsync(i => i.Name == roleName, cancellationToken);
            if (role_entity == null)
                return new Result<ResponseModel>(IsSuccess: false, Message: "Kimliğinizi Doğrulayınız!. Tekrar Giriş Yapınız!", ResponseLogging: true);

            if (role_entity.ParentUid.HasValue && !request.Key.HasValue)
                return new Result<ResponseModel>(IsSuccess: false, Message: "Kimliğinizi Doğrulayınız!. Tekrar Giriş Yapınız!", ResponseLogging: true);

            var targetAdmin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid, cancellationToken);
            if (targetAdmin == null)
                return new Result<ResponseModel>(IsSuccess: false, Message: "Mevcut Kullanıcı Bulunamadı!", ResponseLogging: true);

            var connectionKeys = targetAdmin.ConnectionKeys?.DeserializeFromCamelCase<List<ConnectionKeyHelper>>() ?? new List<ConnectionKeyHelper>();
            if (role_entity.ParentUid.HasValue)
            {
                var connectionKey = connectionKeys.FirstOrDefault(w => w.Key == request.Key && DateTime.Now <= w.ValidTo);
                if (connectionKey == null)
                    return new Result<ResponseModel>(IsSuccess: false, Message: "Bağlantı Anahtarının Süresi Dolmuştur!", ResponseLogging: true);
            }

            var targetRole = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == targetAdmin.RoleUid);

            _ = Validate(request, targetAdmin, targetRole);

            var role = _mapper.Map<RoleDO>(targetRole);

            Expression<Func<PermissionDO, bool>> PermissionDOPredicate = i => i.RoleUid == targetAdmin.RoleUid;

            var permissions = await GetPermissionsByRole(PermissionDOPredicate, cancellationToken);

            if (permissions == null || permissions.Count < 1)
            {
                Result<object> log_result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "" };
                throw new ModelException(log_result, "Role ait modül eklenmemiştir.");
            }


            var auth_id = role.Slug + "." + targetAdmin.Uid + "." + Guid.NewGuid().ToString();
            var claims = new
            {
                FullName = targetAdmin.Name + " " + targetAdmin.Surname,
                UserName = targetAdmin.Username,
                RoleName = role.Name,
                AuthID = auth_id,
                ByConnectionKey = request.Key.ToString(),
                ByUser = authId.Split(".")[1],
                AppKey = Guid.Empty.ToString()
            };
            var token = _jwtHelper.CreateToken(claims, targetRole.Expiration);

            if (string.IsNullOrEmpty(token.Token))
            {
                Result<object> log_result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Yetkisiz erişim!" };
                throw new ModelException(log_result, "Token oluşturulamadı!");
            }
            if (role_entity.ParentUid.HasValue)
            {
                var tmpConnectionKeys = targetAdmin.ConnectionKeys?.DeserializeFromCamelCase<List<ConnectionKeyHelper>>() ?? new List<ConnectionKeyHelper>();
                var connectionKey = tmpConnectionKeys.FirstOrDefault(w => w.Key == request.Key);

                connectionKey.Connected ??= new List<Guid>();
                var uid = Guid.Parse(authId.Split(".")[1]);
                connectionKey.Connected.Add(uid);
                connectionKey.Connected = connectionKey.Connected.Distinct().ToList();

                targetAdmin.ConnectionKeys = tmpConnectionKeys.SerializeWithCamelCase();
            }

            targetAdmin.RefreshToken = token.ReToken;
            targetAdmin.RefreshTokenExpiration = token.ReTokenExpiration;
            targetAdmin.UpdateDate = DateTime.Now;
            _adminRepository.Update(targetAdmin);
            await _uow.SaveChangesAsync(cancellationToken);


            if (_memoryCacheService.IsAdd(authId))
                _memoryCacheService.Remove(authId);

            _httpContextAccessor.HttpContext.Response.Cookies.Delete("Access-Token");

            CacheAuthInfoHelper cacheAuthInfo = new()
            {
                Permissions = permissions,
                RoleRoute = role.Route
            };

            _memoryCacheService.Add(auth_id, cacheAuthInfo, token.Expiration);

            Expression<Func<Permission, bool>> PermissionPredicate = i => i.RoleUid == targetAdmin.RoleUid;
            Expression<Func<Module, bool>> ModulePredicate = w => w.IsMenu == true;
            var modulesAsHierarchical = await GetModulesAsHierarchical(PermissionPredicate, ModulePredicate, cancellationToken);

            //kullanıcıya ait menu oluşturma işlemi
            BaseFunctions.WriteToFile(Functions.UserLayoutMenuDir, $"{targetAdmin.Uid}.txt", Functions.MakeUserMenuHtml(modulesAsHierarchical));

            //jwt token cookie ekleme işlemi
            CookieOptions cookie_options = new()
            {
                IsEssential = true,
                Expires = token.Expiration
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("Access-Token", token.Token, cookie_options);

            result.Redirect = "/Admin/Index";
            if (!string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Query["returnURL"]))
                result.Redirect = _httpContextAccessor.HttpContext.Request.Query["returnURL"];

            else if (!string.IsNullOrEmpty(role.Route))
                result.Redirect = role.Route;

            result.Message = "Kullanıcı başarılı giriş yaptı.";
            result.IsSuccess = true;
        }
        catch (ModelException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ModelException(new Result<object> { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = ex.Message });
        }
        return result;
    }


    internal Result<object> Validate(LoginByConnectionKeyRequest request, Admin admin, Role role)
    {
        Result<object> result;


        if (admin == null)
        {
            result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Böyle bir kullanıcı yoktur." };
            throw new ModelException(result);
        }


        //kullanılıyor açılacak
        //if (admin.IsConfirmed != true)
        //{
        //    result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Hesap onaylanmamıştır.", AccountInfo = request.AccountInfo };
        //    throw new ModelException(result);
        //}

        if (admin.IsSuspend == true)
        {
            result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Hesabınız askıya alınmıştır." };
            throw new ModelException(result);
        }

        if (role == null)
        {
            result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Yetkiniz bulunmamaktadır." };
            throw new ModelException(result);
        }


        return new() { IsSuccess = true, ResultType = ResultTypeEnum.Success, Data = request };
    }


    private async Task<List<PermissionDO>> GetPermissionsByRole(Expression<Func<PermissionDO, bool>> permissionPredicate, CancellationToken token = default)
    {
        var permissions = new List<PermissionDO>();
        try
        {
            var query = from per in _permissionRepository.AsQueryable()
                        join module in _moduleRepository.AsQueryable() on per.ModuleUid equals module.Uid
                        select new PermissionDO
                        {
                            Uid = per.Uid,
                            Module = new ModuleDO
                            {
                                Uid = module.Uid,
                                Address = module.Address,
                                Action = module.Action,
                                Controller = module.Controller,
                                ParentUid = module.ParentUid,
                                Name = module.Name,
                                Type = module.Type,
                            },
                            RoleUid = per.RoleUid,
                            IgnoredSections = per.IgnoredSections,
                            ByAdmin = per.ByAdmin,
                            ModuleUid = per.ModuleUid,
                        };
            //predicate
            if (permissionPredicate != null)
                query = query.Where(permissionPredicate);

            return await query.ToListAsync(cancellationToken: token);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return null;
    }

    private async Task<List<ModuleDO>> GetModulesAsHierarchical(Expression<Func<Permission, bool>> permissionPredicate, Expression<Func<Module, bool>> modulePredicate, CancellationToken token = default)
    {
        var modules = new List<ModuleDO>();
        try
        {
            var permissions_entities = await _permissionRepository.AsQueryable().Where(permissionPredicate).ToListAsync(cancellationToken: token);

            var permissions_modules_uids = permissions_entities.Select(i => i.ModuleUid).ToList();

            var modules_entities = await _moduleRepository.AsQueryable().Where(modulePredicate).Where(i => permissions_modules_uids.Contains(i.Uid)).ToListAsync(cancellationToken: token);
            if (modules_entities != null && modules_entities.Count > 0)
                modules = _mapper.Map<List<ModuleDO>>(modules_entities);


            var parents = modules.Where(w => !w.ParentUid.HasValue).ToList();

            parents.ForEach(parent => ModuleToTree(parent, modules));
            //parents.ForEach(parent => parent.SubModuleList = modules.Where(w => w.ParentUid == parent.Uid).ToList());



            return parents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return null;
    }

    private ModuleDO ModuleToTree(ModuleDO parent, List<ModuleDO> arr)
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

public class LoginByConnectionKeyRequest : IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; }
    public Guid? Key { get; set; }
}

public class LoginByConnectionKeyValidator : AbstractValidator<LoginByConnectionKeyRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public LoginByConnectionKeyValidator()
    {
        RuleFor(w => w.Uid)
            .NotNull().WithMessage("Kullanıcı Seçmelisiniz")
            .NotEmpty().WithMessage("Kullanıcı Seçmelisiniz");

    }
}
