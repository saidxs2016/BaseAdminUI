using DAL.DalNotificationsHandlers.Contracts;
using DAL.MainSysDB.Context;
using DAL.MainSysDB.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.Json;

namespace DAL.DalNotificationsHandlers.Consumers;


public class DeletedRecordMNH : INotificationHandler<DeletedRecordMN>
{

    private readonly ILogger<DeletedRecordMNH> _logger;
    private readonly SysDbContext _sysContext;

    public DeletedRecordMNH(ILogger<DeletedRecordMNH> logger, SysDbContext sysContext)
    {
        _logger = logger;
        _sysContext = sysContext;
    }

    public async Task Handle(DeletedRecordMN notification, CancellationToken cancellationToken)
    {
        try
        {

            if (notification.Operation.ToLowerInvariant() == "update")
            {
                Expression<Func<DeletedRecord, bool>> predicate = i => notification.InstanceId != Guid.Empty && i.InstanceId == notification.InstanceId;
                Expression<Func<SetPropertyCalls<DeletedRecord>, SetPropertyCalls<DeletedRecord>>> props = i => i.SetProperty(_ => _.Date, notification.Date).SetProperty(_ => _.Confirmed, notification.Confirmed);

                _ = await _sysContext.DeletedRecords.Where(predicate).ExecuteUpdateAsync(props, cancellationToken);
            }
            else
            {
                var data = JsonSerializer.Serialize(notification.Data);

                DeletedRecord entity = new()
                {
                    Entity = notification.Entity,
                    Date = notification.Date,
                    Confirmed = notification.Confirmed,
                    InstanceId = notification.InstanceId,
                    Data = data,
                };
                _ = await _sysContext.DeletedRecords.AddAsync(entity, cancellationToken);
                _ = await _sysContext.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
