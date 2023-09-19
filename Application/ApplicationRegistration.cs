// ================== <Start> Global Varaibles ==================
global using Application.DTO.Helpers;
global using Application.Functions_Extensions;
global using Core.DTO.Enums;
global using Core.DTO.Helpers;
global using Core.Functions_Extensions;
// ================== </End> ==================

using Application.CronJobs.HangfireFilter;
using Application.LocalQueues;
using Application.RequestsEventsHandlers.MedBehaviors;
using Application.RequestsEventsHandlers.MessageEventsHandlers;
using Application.SignalRHubs;
using Application.SignalRHubs.Clients;
using Application.SignalRHubs.Hubs;
using Application.WorkerServices;
using Core;
using DAL;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationRegistration
{

    // bu Service ekleme işlemi: mediatr, auto mapper ve repsitorileri servis olarak sisteme dahil etme
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ======== Add Mapper Service ========
        services.AddAutoMapper(typeof(ApplicationRegistration));

        // ======== Add Mediatr Service ========
        services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(typeof(ApplicationRegistration).Assembly);
            conf.AddOpenBehavior(typeof(MedPipelineFilter<,>));
            //conf.AddOpenRequestPreProcessor(typeof(RequestFilter<>));
            //conf.AddOpenRequestPostProcessor(typeof(ResponseFilter<,>));
        });

        // ======== MassTransit ========
        services.InjectMassTransit(configuration);


        // ======== Hangfire ========
        services.AddHangfire(conf => conf
            .UseDefaultCulture(System.Globalization.CultureInfo.CurrentCulture)
            .UsePostgreSqlStorage(configuration["ConnectionStrings:Hangfire"],
            new Hangfire.PostgreSql.PostgreSqlStorageOptions
            {
                /* other configs */
                SchemaName = "public",
            })
            .UseSimpleAssemblyNameTypeSerializer()
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        );
        services.AddHangfireServer();

        // ======== SignalR ========       
        services.AddSignalR(options =>
        {
            options.MaximumReceiveMessageSize = 1024 * 1024;// 1 MB;

            // Global filters will run first
            // options.AddFilter<GlobalHubFilter>();
        })
        .AddHubOptions<NotificationHub>(options =>
        {
            // Local filters will run second
            options.AddFilter<NotificationHubFilter>();
        });
        //.AddMessagePackProtocol();

        // ======== Get SignalRCilent Setting ========
        services.Configure<List<SignalRClientOptions>>(configuration.GetSection("SignalRClientOptions:Clients"));

        // ======== Inject SignalR Client Services ========
        services.AddSingleton<NotificationClient1>();

        // ======== Inject HostedServices ========
        services.AddHostedService<TestWorker>();
        services.AddHostedService<HangfireWorker>();
        services.AddHostedService<SignalRWorker>();


        // ======== Inject Queue Services ========
        services.AddSingleton<ISignalRLogQueue, SignalRLogQueue>();


        services.AddDalServices(configuration);
        services.AddCoreServices(configuration);
        return services;
    }

    public static WebApplication UseApplicationsMiddleWares(this WebApplication app)
    {
        app.UseHangfireDashboard("/app-hangfire", new DashboardOptions
        {
            DashboardTitle = "Admin UI Base HangFire Panel",
            DarkModeEnabled = true,
            Authorization = new[] { new HangfireAuthorizationFilter() }
        });
        var notif_hub_uri = new Uri(app.Configuration[$"SignalRClientOptions:Hubs:{typeof(NotificationHub).Name}"]);
       
        app.MapHub<NotificationHub>(notif_hub_uri.LocalPath, opt =>
        {
            opt.CloseOnAuthenticationExpiration = true;
            //opt.AuthorizationData.Contains();
            //opt.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
            //opt.ApplicationMaxBufferSize = 1024 * 1024;
        });

        return app;
    }
}
