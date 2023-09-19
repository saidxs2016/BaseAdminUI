using Core.DTO.Helpers;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace UI.Extensions.Health;


public static class HealthServiceRegistration
{

    public static IServiceCollection ConfigureHealthApp(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<HealthCheckPublisherOptions>(conf =>
        //{
        //    conf.Period = TimeSpan.FromMinutes(2);
        //    conf.Delay = TimeSpan.FromMinutes(2);
        //});
        //services.AddSingleton<IHealthCheckPublisher, HealthPublisher>();
        services.AddHealthChecks()
            .AddNpgSql(configuration["ConnectionStrings:MDbContext"], name: "Main DB")
            .AddNpgSql(configuration["ConnectionStrings:SysDbContext"], name: "Main System DB")            
            .AddSignalRHub(configuration["SignalRClientOptions:Hubs:NotificationHub"], name: "Notification Hub")

            .AddCheck<UserHealth>("UserHealth");

        services
            .AddHealthChecksUI()
            .AddPostgreSqlStorage(configuration["ConnectionStrings:HealthChecksUI"]);


        return services;
    }

    public static WebApplication UseHealthApp(this WebApplication app)
    {
        app.UseHealthChecks("/_healthy_api", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        // ============== Health Check ============== 
        app.MapHealthChecksUI(setupOptions =>
        {
            setupOptions.UIPath = "/health-ui";
        }).RequireAuthorization(policy => policy
            .RequireClaim(ClaimHelper.RoleName, "Süper Admin")
        );


        return app;
    }

}