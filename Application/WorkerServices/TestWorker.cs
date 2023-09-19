using Application.RequestsEventsHandlers.MessageEventsHandlers.Contracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.WorkerServices;

public class TestWorker : BackgroundService
{
    private readonly IBus _bus;
    private readonly IMediator _mediator;
    //private readonly IServiceScope _serviceScope;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IServiceProvider _serviceProvider;
    public TestWorker(IMediator mediator, IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider, IBus bus)
    {
        _mediator = mediator;
        //_serviceScope = serviceScope;
        _serviceScopeFactory = serviceScopeFactory;
        _serviceProvider = serviceProvider;
        _bus = bus;
    }

    protected override Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            token.ThrowIfCancellationRequested();
            Task.Run(async () => await DoWork(token), token);
        }
        catch (Exception) { }

        return Task.CompletedTask;

    }

    private async Task DoWork(CancellationToken token)
    {
        for (int i = 0; i < 100; i++)
        {

            await _bus.Publish<HelloWorldEvent>(new($" id: {i} - ."));
            await Task.Delay(100, token);
        }


        //return Task.CompletedTask;

    }
}
