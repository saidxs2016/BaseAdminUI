using DAL.DalNotificationsHandlers.Contracts;
using DAL.MainSysDB.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.DalNotificationsHandlers.Consumers;


public class DeleteSystemLogMNH : INotificationHandler<DeleteSystemLogMN>
{

    private readonly ILogger<DeleteSystemLogMNH> _logger;
    private readonly SysDbContext _sysContext;

    public DeleteSystemLogMNH(ILogger<DeleteSystemLogMNH> logger, SysDbContext sysContext)
    {
        _logger = logger;
        _sysContext = sysContext;
    }

    public async Task Handle(DeleteSystemLogMN notification, CancellationToken cancellationToken)
    {
        try
        {
            var sql = $"""
                          delete form system_log where raise_date < {DateTime.Now.AddMonths(-1)};
                       """;

            _ = await _sysContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
