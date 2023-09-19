using Application.CronJobs;
using DAL.MainSysDB.Context;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace Application.WorkerServices;

public class HangfireWorker : BackgroundService
{
    private readonly IBackgroundJobClient _backgroundJob;
    private readonly IRecurringJobManager _recurringJob;

    public HangfireWorker(IBackgroundJobClient backgroundJob, IRecurringJobManager recurringJob)
    {
        _backgroundJob = backgroundJob;
        _recurringJob = recurringJob;
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

    private Task DoWork(CancellationToken token)
    {

        _recurringJob.RemoveIfExists("Kullanıcı Loglarını Sil");
        _recurringJob.AddOrUpdate<DatabaseJobs>("Kullanıcı Loglarını Sil", _ => _.DeleteUserLog(), Cron.Yearly());

        _recurringJob.RemoveIfExists("Sistem Loglarını Sil");
        _recurringJob.AddOrUpdate<DatabaseJobs>("Sistem Loglarını Sil", _ => _.DeleteSystemLog(), Cron.Monthly());

        _recurringJob.RemoveIfExists("Health İşlemi Sonucu Oluşan Hataları Sil");
        _recurringJob.AddOrUpdate<DatabaseJobs>("Health İşlemi Sonucu Oluşan Hataları Sil", _ => _.DeleteHealthLog(), Cron.Monthly());

        _recurringJob.RemoveIfExists("Eski Health UI Loglarını Sil");
        _recurringJob.AddOrUpdate<DatabaseJobs>("Eski Health UI Loglarını Sil", _ => _.DeleteHealthUIHistory(), Cron.Monthly());

        _recurringJob.RemoveIfExists("Eski Hangfire Loglarını Sil");
        _recurringJob.AddOrUpdate<DatabaseJobs>("Eski Hangfire Loglarını Sil", _ => _.DeleteHangfireHistory(), Cron.Yearly());


        return Task.CompletedTask;

    }
}
