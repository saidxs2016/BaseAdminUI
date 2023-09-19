// ================== <Start> Global Varaibles ==================
global using Core.DTO.Helpers;
global using Core.DTO.Enums;
global using Core.DTO.Options;
global using Core.DTO.Models;
global using Core.DTO.ResultType;
global using Core.Functions_Extensions;

// ================== </End> ==================


using Core.OtherUtilities.IoC;
using Core.Security.Jwt;
using Core.Services.CacheService.MicrosoftInMemory;
using Core.Services.MailService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;
public static class CoreRegistration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    { 
        // ======== Cache Microsoft Service ========
        services.AddMemoryCache();
        services.AddSingleton<IMemoryCacheService, MemoryCacheService>();

        // ======== Cache Redis Service ========
        //var multiplexer = ConnectionMultiplexer.Connect(configuration["ConnectionStrings:Redis"]);
        //services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        //services.AddSingleton<IRedisCacheService, RedisCacheService>();


        // ======== Mail Service ========
        services.Configure<EmailSettings>(configuration.GetSection("EmailOptions:Settings"));
        services.AddScoped<EmailSendService>();

        // ======== JWT Helper Service ========
        services.Configure<JwtSetting>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtHelper, JwtHelper>();

        // ======== (Valid-Invalid) File Extensions Configration ========
        services.Configure<ExtensionSelector>(configuration.GetSection("FileExtensions"));

        //services.AddSingleton<ICoreLocalizer, CoreLocalizer>();

        // Build Service provider instane 
        ServiceTool.RegisterServiceProviderInstance(services);


        return services;
    }

    
}
