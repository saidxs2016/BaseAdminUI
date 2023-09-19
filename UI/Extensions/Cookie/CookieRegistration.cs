using Microsoft.AspNetCore.CookiePolicy;

namespace UI.Extensions.Cookie;

public static class CookieRegistration
{
    public static IServiceCollection ConfigureCookie(this IServiceCollection services)
    {
        services.AddCookiePolicy(options =>
        {
            options.HttpOnly = HttpOnlyPolicy.None;
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
        });


        return services;
    }

    public static WebApplication UseCookieMiddleware(this WebApplication app)
    {

        var cookieOptions = app.Services.GetService<CookiePolicyOptions>();
        app.UseCookiePolicy(cookieOptions);
        return app;
    }
}
