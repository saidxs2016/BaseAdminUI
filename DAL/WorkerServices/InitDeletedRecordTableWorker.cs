using DAL.MainSysDB.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DAL.WorkerServices;

public class InitDeletedRecordTableWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SysDbContext _sysDbContext;
    private readonly ILogger<InitDeletedRecordTableWorker> _logger;
    public InitDeletedRecordTableWorker(IServiceScopeFactory serviceScopeFactory, SysDbContext sysDbContext, ILogger<InitDeletedRecordTableWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _sysDbContext = sysDbContext;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            token.ThrowIfCancellationRequested();
            Task.Run(async () => await DoWork(token), token);
        }
        catch (Exception) { }

        return Task.CompletedTask;

    }

    private async Task DoWork(CancellationToken token)
    {
        try
        {
            string sql = _ = """
                                create table if not exists deleted_record(

                                id serial not null primary key,
                                entity varchar(255),
                                date timestamp(0) without time zone,
                                data jsonb,
                                instance_id uuid,
                                confirmed boolean

                                );         
                            """;
            _ = await _sysDbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken: token);


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }


        //return Task.CompletedTask;

    }
}
