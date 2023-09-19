using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.RequestsEventsHandlers.MessageEventsHandlers;

public static class MassTransitRegistration
{

    // bu Service ekleme işlemi: mediatr, auto mapper ve repsitorileri servis olarak sisteme dahil etme
    public static IServiceCollection InjectMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        // ======== MassTransit ========
        services.AddMassTransit(cfg =>
        {
            var assembly = typeof(MassTransitRegistration).Assembly;


            //cfg.SetKebabCaseEndpointNameFormatter();
            cfg.SetInMemorySagaRepositoryProvider();

            cfg.AddConsumers(assembly);
            cfg.AddSagaStateMachines(assembly);
            cfg.AddSagas(assembly);
            cfg.AddActivities(assembly);

            cfg.UsingRabbitMq((ctx, confg) =>
            {
                // ister Host Url İle bağlanabiliriz
                // confg.Host(configuration["ConnectionStrings:RabbitMQHost"]);

                // ister de connection factory bilgilerimizle
                _ = ushort.TryParse(configuration["RabbitMQ:Port"], out ushort port);
                confg.Host(configuration["RabbitMQ:Host"], port: port, configuration["RabbitMQ:Vhost"], rmqc =>
                {
                    rmqc.Username(configuration["RabbitMQ:Username"]);
                    rmqc.Password(configuration["RabbitMQ:Password"]);
                });
                confg.ConfigureEndpoints(ctx);
                confg.UseMessageRetry(c =>
                {
                    c.Interval(10, TimeSpan.FromSeconds(5));
                });
            });
        });


        return services;
    }

}
