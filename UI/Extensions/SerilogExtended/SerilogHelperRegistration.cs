using Serilog;
using Serilog.Filters;

namespace UI.Extensions.SerilogExtended;

public static class SerilogHelperRegistration
{    
    public static WebApplicationBuilder InitSerilog(this WebApplicationBuilder builder, string env)
    {
        // ======== Add Serilog Configration ========
        var serilogConfigration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"Configurations/serilog.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Configurations/serilog.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var healthSerilogConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"Configurations/healthserilog.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Configurations/healthserilog.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var userBasedSerilogConfigration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"Configurations/userbasedserilog.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Logger(_ => _
                .ReadFrom.Configuration(serilogConfigration)
                .Filter.ByExcluding(Matching.FromSource("Microsoft.Extensions.Diagnostics.HealthChecks"))
                .Filter.ByExcluding(Matching.FromSource("HealthChecks.UI.Core.HostedService.HealthCheckCollectorHostedService"))
                .Filter.ByExcluding(Matching.WithProperty<bool>("UserLog", _ => _ == true))

            )
            .WriteTo.Logger(_ => _
                .ReadFrom.Configuration(healthSerilogConfiguration)
                .Filter.ByIncludingOnly(Matching.FromSource("Microsoft.Extensions.Diagnostics.HealthChecks"))
            )
            .WriteTo.Logger(_ => _
                .ReadFrom.Configuration(userBasedSerilogConfigration)
                .Filter.ByIncludingOnly(Matching.WithProperty<bool>("UserLog", _ => _ == true))
            )            
            .CreateLogger();

        builder.Logging.ClearProviders().AddSerilog(Log.Logger);
        return builder;

    }
}
