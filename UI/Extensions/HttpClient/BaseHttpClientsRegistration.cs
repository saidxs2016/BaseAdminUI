namespace UI.Extensions.HttpClient;


public static class BaseHttpClientsRegistration
{

    public static IServiceCollection ConfigureHttpClientsApp(this IServiceCollection services)
    {

        //services.AddScoped<ApiGatewayHttpClient>();

        //services.AddHttpClient("ApiGatewayHttpClient", client =>
        //{
        //    client.BaseAddress = new Uri("http://localhost:5010/");
        //})
        //.AddHttpMessageHandler<ApiGatewayHttpClient>();

        //services.AddScoped(sp =>
        //{
        //    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();

        //    return clientFactory.CreateClient("ApiGatewayHttpClient");
        //});


        //services.AddScoped<AuthSystemUserHttpClient>();

        //services.AddHttpClient("AuthSystemUserHttpRequest", client =>
        //{
        //    client.BaseAddress = new Uri("https://localhost:7060/validateauth");
        //})
        //.AddHttpMessageHandler<AuthSystemUserHttpClient>();


        return services;
    }



}