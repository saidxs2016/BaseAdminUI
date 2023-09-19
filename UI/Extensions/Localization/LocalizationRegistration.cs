using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;
using UI.Resources;

namespace UI.Extensions.Localization;

public static class LocalizationRegistration
{
    public static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {

        services.AddLocalization(opts => opts.ResourcesPath = "Resources");
        services.AddMvc(options => { options.MaxModelValidationErrors = 10; })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                opts => opts.ResourcesPath = "Resources")
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                    return factory.Create("SharedResource", assemblyName.Name);
                };
            });

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("tr"),
            };

            options.DefaultRequestCulture = new RequestCulture(culture: "tr", uiCulture: "tr");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
        });

        services.AddSingleton<ILocService, LocService>();

        return services;
    }


    public static WebApplication UseLocalizationMiddleware(this WebApplication app)
    {
        var locOption = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(locOption.Value);

        return app;
    }
}
