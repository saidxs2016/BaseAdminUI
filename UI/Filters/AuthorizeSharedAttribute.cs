using Application.DTO.Exceptions;
using Application.DTO.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Core.Services.CacheService.MicrosoftInMemory;
using UI.Resources;
using Core.DTO.Helpers;

namespace UI.Filters;

public class AuthorizeSharedAttribute : TypeFilterAttribute
{

    public AuthorizeSharedAttribute(string? type = null) : base(typeof(AuthorizeFilter))
    {

        Arguments = new object[] { type };
    }
    private class AuthorizeFilter : IAsyncActionFilter
    {
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly ILocService _locService;
        private string Type;
        public AuthorizeFilter(IMemoryCacheService memoryCacheService, ILocService locService, string type)
        {
            _memoryCacheService = memoryCacheService;
            _locService = locService;
            Type = string.IsNullOrEmpty(type) ? ModuleHelper.Page : type;
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

            await next();
        }


    }
}