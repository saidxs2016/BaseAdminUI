namespace UI.Extensions;
public static class ConfigurationHelperRegistration
{

    public static WebApplicationBuilder ReadConfigurations(this WebApplicationBuilder builder, string env)
    {
        // ================= Add New Configration From json File =================
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"Configurations/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Configurations/appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        IConfiguration RateLimitConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"Configurations/ratelimit.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Configurations/ratelimit.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        builder.Configuration.AddConfiguration(RateLimitConfiguration);
        return builder;
    }
}
