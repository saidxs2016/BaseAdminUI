using Application.RequestsEventsHandlers.MedNotificationsHandlers.Contracts;
using Application.RequestsEventsHandlers.MessageEventsHandlers.Contracts;
using DAL.MainDB.Repositories.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.RequestsEventsHandlers.MessageEventsHandlers.Consumers;



public class HellowWorldEventHandler : IConsumer<HelloWorldEvent>
{

    private readonly IServiceScopeFactory _serviceScopeFactory;
    public HellowWorldEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Consume(ConsumeContext<HelloWorldEvent> context)
    {
        // =============== Create New Scope ===============
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        // =============== Get Your Services ===============
        var _mediator = scope.ServiceProvider.GetService<IMediator>();
        var _uow = scope.ServiceProvider.GetService<IUnitOfWork>();

        // =============== Write Your Code ===============
        Console.WriteLine($" +++++ {context.Message.message} its recieved. +++++ ");
        await Task.Delay(1000);

    }
}
