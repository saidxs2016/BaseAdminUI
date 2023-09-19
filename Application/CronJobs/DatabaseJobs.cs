using DAL.DalNotificationsHandlers.Contracts;
using MediatR;

namespace Application.CronJobs;

public class DatabaseJobs
{
    private readonly IMediator _mediator;

    public DatabaseJobs(IMediator mediator) =>
        _mediator = mediator;

    public async Task DeleteHealthLog() => await _mediator.Publish(new DeleteHealthLogMN());
    public async Task DeleteSystemLog() => await _mediator.Publish(new DeleteSystemLogMN());
    public async Task DeleteUserLog() => await _mediator.Publish(new DeleteUserLogMN());
    public async Task DeleteHealthUIHistory() => await _mediator.Publish(new DeleteHealthUIHistoryMN());
    public async Task DeleteHangfireHistory() => await _mediator.Publish(new DeleteHangfireHistoryMN());

}
