using Application.RequestsEventsHandlers.MedNotificationsHandlers.Contracts;
using Application.RequestsEventsHandlers.MedRHApplication.Queries.BackJob;
using MediatR;

namespace Application.CronJobs;

public class SharedJobs
{

    private readonly IMediator _mediator;

    public SharedJobs(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task HandlerRequestJob() => await _mediator.Send(new BackJobAR(true));

    public async Task HandlerNotificationJob() => await _mediator.Publish(new BackJobMN(" ===========  Push New HandlerNotificationJob  =========== "));


}
