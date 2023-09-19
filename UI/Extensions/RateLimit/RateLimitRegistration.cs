using AspNetCoreRateLimit;

namespace UI.Extensions.RateLimit;

public static class RateLimitRegistration
{
    public static IServiceCollection ConfigureRateLimit(this IServiceCollection services, IConfiguration configuration)
    {

        // configure ip rate limiting middleware
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

        // configure client rate limiting middleware
        //services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));
        //services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

        // register stores
        services.AddInMemoryRateLimiting();
        //services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();
        //services.AddDistributedRateLimiting<RedisProcessingStrategy>();
        //services.AddRedisRateLimiting();

        // configure the resolvers
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;

    }
    public static WebApplication UseMyRateLimit(this WebApplication app)
    {
        app.UseIpRateLimiting();
        //app.UseClientRateLimiting();

        return app;

    }
}
