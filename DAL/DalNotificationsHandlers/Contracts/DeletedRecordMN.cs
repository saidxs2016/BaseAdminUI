using MediatR;

namespace DAL.DalNotificationsHandlers.Contracts;


public record DeletedRecordMN : INotification
{
    public string Operation { get; set; } // add, update
    public Guid InstanceId { get; set; }
    public DateTime Date { get; set; }

    public string? Entity { get; set; }

    public bool Confirmed { get; set; }

    public dynamic? Data { get; set; }
}
