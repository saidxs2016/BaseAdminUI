using DAL.DalNotificationsHandlers.Contracts;
using DAL.MainSysDB.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.DalNotificationsHandlers.Consumers;


public class DeleteHangfireHistoryMNH : INotificationHandler<DeleteHangfireHistoryMN>
{

    private readonly ILogger<DeleteHangfireHistoryMNH> _logger;
    private readonly SysDbContext _sysContext;

    public DeleteHangfireHistoryMNH(ILogger<DeleteHangfireHistoryMNH> logger, SysDbContext sysContext)
    {
        _logger = logger;
        _sysContext = sysContext;
    }

    public async Task Handle(DeleteHangfireHistoryMN notification, CancellationToken cancellationToken)
    {
        try
        {
            var sql = $"""
                          delete form job where expireat < {DateTime.Now.AddMonths(-1)};
                       """;
            _ = await _sysContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
