using Application.RequestsEventsHandlers.MedNotificationsHandlers.Contracts;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;

namespace Application.RequestsEventsHandlers.MedNotificationsHandlers.Consumers;


public class BackJobMNH : INotificationHandler<BackJobMN>
{
    private readonly IUnitOfWork _uow;

    public BackJobMNH(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(BackJobMN notification, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(1);
            Console.WriteLine(notification.message);
            Console.WriteLine($" ++++++++++++ BackJobMNHandler. {_uow.GetInstanceId()} ++++++++++++++");

        }
        catch (Exception)
        {

        }

    }

}

