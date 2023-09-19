using Application.DTO.DataObjects;
using Application.DTO.Exceptions;
using Application.DTO.ResultType;
using Application.Functions_Extensions;
using AutoMapper;
using Core.Security.Hashing;
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

namespace Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Login;

public class LoginHandler : IRequestHandler<LoginRequest, Result<LoginResponse>>
{
    private readonly ILogger<LoginHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IJwtHelper _jwtHelper;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUnitOfWork _uow;

    private readonly IMediator _mediator;



    public LoginHandler(ILogger<LoginHandler> logger, IMapper mapper, IAdminRepository adminRepository, IRoleRepository roleRepository, IModuleRepository moduleRepository, IJwtHelper jwtHelper, IMemoryCacheService memoryCacheService, IHttpContextAccessor contextAccessor, IPermissionRepository permissionRepository, IUnitOfWork uow, IMediator mediator)
    {
        _logger = logger;
        _mapper = mapper;
        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
        _moduleRepository = moduleRepository;
        _jwtHelper = jwtHelper;
        _memoryCacheService = memoryCacheService;
        _contextAccessor = contextAccessor;
        _permissionRepository = permissionRepository;
        _uow = uow;
        _mediator = mediator;
    }
    public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<LoginResponse>(IsSuccess: true, Message: "", ResponseLogging: true);
        var model = new LoginResponse();
        try
        {

            var entity = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Username == request.Username || w.Email == request.Username, cancellationToken) ?? new Admin();
            var role_entity = await _roleRepository.GetAsFirstOrDefaultAsync(i => i.Uid == entity.RoleUid, cancellationToken);

            _ = Validate(request, entity, role_entity);

            var role = _mapper.Map<RoleDO>(role_entity);

            Expression<Func<PermissionDO, bool>> PermissionDOPredicate = i => i.RoleUid == entity.RoleUid;


            var permissions = await GetPermissionsByRole(PermissionDOPredicate, cancellationToken);

            if (permissions == null || permissions.Count < 1)
            {
                Result<object> log_result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "" };
                throw new ModelException(log_result, "Role ait yetki eklenmemiştir.");
            }







            var auth_id = $"{role.Slug}.{entity.Uid}.{Guid.NewGuid()}";
            var claims = new
            {
                FullName = entity.Name + " " + entity.Surname,
                UserName = entity.Username,
                RoleName = role.Name,
                AuthID = auth_id,
                AppKey = Guid.Empty.ToString()
            };

            var token = _jwtHelper.CreateToken(claims, role.Expiration);

            if (string.IsNullOrEmpty(token.Token))
            {
                Result<object> log_result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Yetkisiz erişim!" };
                throw new ModelException(log_result, "Token oluşturulamadı!");
            }


            entity.RefreshToken = token.ReToken;
            entity.RefreshTokenExpiration = token.ReTokenExpiration;
            entity.UpdateDate = DateTime.Now;
            _adminRepository.Update(entity);
            await _uow.SaveChangesAsync(cancellationToken);



            CacheAuthInfoHelper cacheAuthInfo = new()
            {
                Permissions = permissions,
                RoleRoute = role.Route
            };

            var logins = _memoryCacheService.GetByPattern($"^{role.Slug + "\\." + entity.Uid}\\.");
            if (logins != null && logins.Count >= role_entity.LoginCount)
            {
                //logins = logins.OrderBy(w => w.AbsoluteExpiration).ToList();
                //_memoryCacheService.Remove(logins.FirstOrDefault().Key.ToString());

                Result<object> log_result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Yeni oturum açmanıza izin verilmedi. diğer cihazlardan çıkış yapınız." };
                throw new ModelException(log_result, "Maximum Oturm Açma Hakkınız Doldurdunuz.");
            }


            _memoryCacheService.Add(auth_id, cacheAuthInfo, token.Expiration);

            Expression<Func<Permission, bool>> PermissionPredicate = i => i.RoleUid == entity.RoleUid;
            Expression<Func<Module, bool>> ModulePredicate = w => w.IsMenu == true && w.Type != ModuleHelper.Feature;
            var modulesAsHierarchical = await GetModulesAsHierarchical(PermissionPredicate, ModulePredicate, cancellationToken);

            //kullanıcıya ait menu oluşturma işlemi
            BaseFunctions.WriteToFile(Functions.UserLayoutMenuDir, $"{entity.Uid}.txt", Functions.MakeUserMenuHtml(modulesAsHierarchical));

            //jwt token cookie ekleme işlemi
            CookieOptions cookie_options = new()
            {
                IsEssential = true,
                Expires = token.Expiration
            };
            _contextAccessor.HttpContext.Response.Cookies.Append("Access-Token", token.Token, cookie_options);


            result.Redirect = "/Admin/Index";
            if (!string.IsNullOrEmpty(_contextAccessor.HttpContext.Request.Query["returnURL"]))
                result.Redirect = _contextAccessor.HttpContext.Request.Query["returnURL"];

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

    // ========================== Other Methods ==========================
    private Result<object> Validate(LoginRequest request, Admin admin, Role role)
    {
        Result<object> result;


        if (admin == null || admin.Uid == Guid.Empty)
        {
            result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Böyle bir kullanıcı yoktur." };
            throw new ModelException(result);
        }

        if (!HashingTool.VerifyPasswordHash(request.Password, admin.PasswordHash, admin.PasswordSalt))
        {
            result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Kullanıcı adı yada şifre yanlışır." };
            throw new ModelException(result, "Girilen şifre yanlıştır.");
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

        if (role == null || role.Uid == Guid.Empty)
        {
            result = new() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Yetkiniz bulunmamaktadır." };
            throw new ModelException(result);
        }


        return new() { IsSuccess = true, ResultType = ResultTypeEnum.Success, Data = request };
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


    // ========================== Repository Methods ==========================

    private async Task<List<PermissionDO>> GetPermissionsByRole(Expression<Func<PermissionDO, bool>> predicate, CancellationToken token = default)
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
                            AddDate = per.AddDate,
                            UpdateDate = per.UpdateDate
                        };
            //predicate
            if (predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync(cancellationToken: token);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
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
            _logger.LogError(ex, "{Message}", ex.Message);
        }

        return null;
    }



}

