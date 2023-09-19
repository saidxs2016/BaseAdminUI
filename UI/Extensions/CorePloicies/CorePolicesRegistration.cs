using Core.Security.Jwt;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace UI.Extensions.Auth;


public static class CorePolicesRegistration
{

    public static IServiceCollection ConfigureCorePolices(this IServiceCollection services)
    {

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        return services;
    }    

}