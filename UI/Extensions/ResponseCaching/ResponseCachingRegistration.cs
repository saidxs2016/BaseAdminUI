namespace UI.Extensions.ResponseCaching;

public static class ResponseCachingRegistration
{
    public static IServiceCollection ConfigureResponseCaching(this IServiceCollection services)
    {

        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 250;
            options.SizeLimit = 250;
            options.UseCaseSensitivePaths = false;
        });

        return services;
    }
}
