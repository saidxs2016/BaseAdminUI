using DAL.DalNotificationsHandlers.Contracts;
using DAL.MainSysDB.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.DalNotificationsHandlers.Consumers;


public class DeleteUserLogMNH : INotificationHandler<DeleteUserLogMN>
{

    private readonly ILogger<DeleteUserLogMNH> _logger;
    private readonly SysDbContext _sysContext;

    public DeleteUserLogMNH(ILogger<DeleteUserLogMNH> logger, SysDbContext sysContext)
    {
        _logger = logger;
        _sysContext = sysContext;
    }

    public async Task Handle(DeleteUserLogMN notification, CancellationToken cancellationToken)
    {
        try
        {
            var sql = $"""
                          delete form user_log where raise_date < {DateTime.Now.AddYears(-1)};
                       """;
            _ = await _sysContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
