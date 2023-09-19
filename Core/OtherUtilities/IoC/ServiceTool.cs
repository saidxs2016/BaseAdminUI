using Microsoft.Extensions.DependencyInjection;


namespace Core.OtherUtilities.IoC;

public static class ServiceTool
{
    public static IServiceProvider ServiceProvider { get; private set; }
    internal static void RegisterServiceProviderInstance(IServiceCollection services) => ServiceProvider = services.BuildServiceProvider();


    //internal static void RegisterServiceProviderInstance(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
}
