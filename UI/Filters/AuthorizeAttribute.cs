using Application.DTO.Exceptions;
using Application.DTO.Helpers;
using Core.DTO.Helpers;
using Core.Services.CacheService.MicrosoftInMemory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UI.Resources;

namespace UI.Filters;

public class AuthorizeAttribute : TypeFilterAttribute
{

    public AuthorizeAttribute(string? type = null) : base(typeof(Authorize))
    {

        Arguments = new object[] { type };
    }
    private class Authorize : IAsyncActionFilter
    {
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly ILocService _locService;
        private string Type;
        public Authorize(IMemoryCacheService memoryCacheService, ILocService locService, string type = null)
        {
            Type = string.IsNullOrEmpty(type) ? ModuleHelper.Page : type;
            _memoryCacheService = memoryCacheService;
            _locService = locService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            //if (context?.HttpContext.User != null || context.HttpContext.User.Identity.IsAuthenticated
            //|| context.HttpContext.User.Claims != null || context.HttpContext.User.Identity.AuthenticationType == "google")
            //{
            //    await next();
            //    return;
            //}

            // User var mı yok mu kontrolü
            if (context?.HttpContext.User == null || !context.HttpContext.User.Identity.IsAuthenticated
            || context.HttpContext.User.Claims == null || string.IsNullOrEmpty(context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == "AuthID")?.Value))
            {
                var message = _locService.GetLocalizedString("Kimliğinizi Doğrulayınız!. Tekrar Giriş Yapınız! - 1");
                if (Type == ModuleHelper.Page)
                    throw new MVCUnauthorizedAccessException(message);
                else
                    throw new JsUnauthorizedAccessException(message);
            }

            //// Inmemory içinde Usere Ait PermissionS'lar var mı yok mu kontrolü
            var auth_id_key = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AuthID");

            var adminAuthInfo = _memoryCacheService.Get<CacheAuthInfoHelper>(auth_id_key.Value);
            if (adminAuthInfo == null || adminAuthInfo.Permissions == null || adminAuthInfo.Permissions.Count == 0)
            {
                var message = _locService.GetLocalizedString("Kimliğinizi Doğrulayınız!. Tekrar Giriş Yapınız! - 2");
                if (Type == ModuleHelper.Page)
                    throw new MVCUnauthorizedAccessException(message);
                else
                    throw new JsUnauthorizedAccessException(message);
            }

            //// Kullanıcı Super User mi yoksa Super User değil
            if (auth_id_key.ToString().ToLowerInvariant().StartsWith("super-admin."))
            {
                await next();
                return;
            }


            var type = context.HttpContext.Request.Method.ToLowerInvariant();
            if (Type == ModuleHelper.Page && type == "get")
            {
                var page = context.HttpContext.Request.Path;
                var permissions = adminAuthInfo.Permissions.Where(w => w.Module.Type == ModuleHelper.Page && page.ToString().ToLowerInvariant().Contains(w.Module.Address.ToLowerInvariant())).ToList();
                if (permissions == null || permissions.Count == 0)
                {
                    var message = _locService.GetLocalizedString("Kimliğinizi Doğrulayınız!. Tekrar Giriş Yapınız! - 3");
                    throw new MVCUnauthorizedAccessException(message);
                }

            }
            else if (Type == ModuleHelper.Feature)
            {
                var page = context.HttpContext.Request.Headers.Referer;
                Uri uri = new(page);
                var permissions = adminAuthInfo.Permissions.Where(w => w.Module.Type == ModuleHelper.Page && uri.LocalPath.ToLowerInvariant().Contains(w.Module.Address.ToLowerInvariant())).ToList();
                if (permissions == null || permissions.Count == 0)
                {
                    var message = _locService.GetLocalizedString("Kimliğinizi Doğrulayınız!. Tekrar Giriş Yapınız! - 4");
                    throw new JsUnauthorizedAccessException(message);
                }

                var modulesUid = permissions.Select(w => w.ModuleUid).ToList();
                var allModules = adminAuthInfo.Permissions.Select(w => w.Module).ToList();
                var targetFeatures = allModules.Where(w => modulesUid.Contains(w.ParentUid)).ToList();

                if (targetFeatures != null && targetFeatures.Count > 0)
                {
                    var featurePath = context.HttpContext.Request.Path;
                    var targetPermissions = targetFeatures.Where(w => w.Type == ModuleHelper.Feature && featurePath.ToString().ToLowerInvariant().Contains(w.Address.ToLowerInvariant())).ToList();
                    if (targetPermissions == null || targetPermissions.Count == 0)
                    {
                        var message = _locService.GetLocalizedString("Kimliğinizi Doğrulayınız!. Tekrar Giriş Yapınız! - 5");
                        throw new Exception(message);
                    }
                }
            }



            await next();
        }


    }
}