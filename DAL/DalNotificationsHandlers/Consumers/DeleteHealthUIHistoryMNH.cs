using DAL.DalNotificationsHandlers.Contracts;
using DAL.MainSysDB.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.DalNotificationsHandlers.Consumers;


public class DeleteHealthUIHistoryMNH : INotificationHandler<DeleteHealthUIHistoryMN>
{

    private readonly ILogger<DeleteHealthUIHistoryMNH> _logger;
    private readonly SysDbContext _sysContext;

    public DeleteHealthUIHistoryMNH(ILogger<DeleteHealthUIHistoryMNH> logger, SysDbContext sysContext)
    {
        _logger = logger;
        _sysContext = sysContext;
    }

    public async Task Handle(DeleteHealthUIHistoryMN notification, CancellationToken cancellationToken)
    {
        try
        {
            var sql = $"""
                          delete form Failures where LastNotified < {DateTime.Now.AddMonths(-1)};
                          delete form HealthCheckExecutionHistories where On < {DateTime.Now.AddMonths(-1)};
                       """;
            _ = await _sysContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
