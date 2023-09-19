using DAL.DalNotificationsHandlers.Contracts;
using DAL.MainDB.Context;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace DAL.MainDB.Repositories.Concretes;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly MDbContext _db;
    private IDbContextTransaction transaction = null;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IMediator _mediator;
    private bool _includeDeletedState = false;

    public UnitOfWork(MDbContext db, ILogger<UnitOfWork> logger, IMediator mediator)
    {
        _db = db;
        _logger = logger;
        TransctionHealth();
        _mediator = mediator;
    }
    private Task TransctionHealth()
    {
        _db.SavingChanges += DbSavingChanges;
        _db.SavedChanges += DbSavedChanges;
        _db.SaveChangesFailed += DbSaveChangesFailed;

        //var db_state = _db.Database.EnsureCreatedAsync().Result;
        //if (!db_state) { }

        //var connect_state = _db.Database.CanConnectAsync().Result;
        //if (!connect_state) { }

        return Task.CompletedTask;

    }

    private void DbSavingChanges(object? sender, SavingChangesEventArgs e)
    {
        var context = sender as MDbContext;
        _includeDeletedState = context.ChangeTracker.Entries().Any(i => i.State == EntityState.Deleted);
    }

    private void DbSavedChanges(object? sender, SavedChangesEventArgs e)
    {
        var context = sender as MDbContext;
        if (_includeDeletedState)
        {
            var notification = new DeletedRecordMN
            {
                Operation = "update",
                InstanceId = context.ContextId.InstanceId,
                Confirmed = true,
                Date = DateTime.Now
            };
            _ = _mediator.Publish(notification);
            _includeDeletedState = false;
        }
    }

    private void DbSaveChangesFailed(object? sender, SaveChangesFailedEventArgs e)
    {
        var context = sender as MDbContext;
        var enrichers = new List<ILogEventEnricher>
        {
            new PropertyEnricher("InstanceId", context.ContextId.InstanceId.ToString())
        };

        using (LogContext.Push(enrichers.ToArray()))
        {
            var message = $"exception details: {e.Exception.Message}";
            _logger.LogError(e.Exception, message);
        }
    }



    public Guid GetInstanceId() => _db.ContextId.InstanceId;
    public virtual MDbContext DbContext => _db;

    public int SaveChange() => _db.SaveChanges();
    public async Task<int> SaveChangesAsync(CancellationToken token = default) => await _db.SaveChangesAsync(cancellationToken: token);

    // ================ Transaction ================    
    public virtual void BeginTransaction()
    {
        transaction = _db.Database.BeginTransaction();
    }
    public virtual async Task BeginTransactionAsync(CancellationToken token = default)
    {
        transaction = await _db.Database.BeginTransactionAsync(cancellationToken: token);
    }
    public virtual void CommitTransaction() => transaction.Commit();
    public virtual async Task CommitTransactionAsync(CancellationToken token = default) => await transaction.CommitAsync(cancellationToken: token);
    public virtual void RollbackTransaction() => transaction.Rollback();
    public virtual async Task RollbackTransactionAsync(CancellationToken token = default) => await transaction.RollbackAsync(cancellationToken: token);
    public virtual void DisposeTransaction() => transaction.Dispose();
    public virtual async Task DisposeTransactionAsync(CancellationToken token = default) => await transaction.DisposeAsync();




    // ================ Disposable ================
    public void Dispose() => transaction?.Dispose();


}
