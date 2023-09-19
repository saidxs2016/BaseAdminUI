using DAL.DalNotificationsHandlers.Contracts;
using DAL.MainSysDB.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.DalNotificationsHandlers.Consumers;


public class DeleteHealthLogMNH : INotificationHandler<DeleteHealthLogMN>
{

    private readonly ILogger<DeleteHealthLogMNH> _logger;
    private readonly SysDbContext _sysContext;

    public DeleteHealthLogMNH(ILogger<DeleteHealthLogMNH> logger, SysDbContext sysContext)
    {
        _logger = logger;
        _sysContext = sysContext;
    }

    public async Task Handle(DeleteHealthLogMN notification, CancellationToken cancellationToken)
    {
        try
        {
            var sql = $"""
                          delete form health_log where raise_date < {DateTime.Now.AddMonths(-1)};
                       """;


            _ = await _sysContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
