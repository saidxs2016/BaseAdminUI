using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DAL.MainDB.Context;

public class TrackeRecordsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    private readonly ILogger<TrackeRecordsInterceptor> _logger;
    //private bool _includeDeletedState = false;

    public TrackeRecordsInterceptor(ILogger<TrackeRecordsInterceptor> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }



    // ============================ Before Save ============================
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {

        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {

        //var entries = eventData.Context.ChangeTracker.Entries().ToArray();
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }



    // ============================ After Save ============================
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {

        return base.SavedChanges(eventData, result);
    }
    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        //var entries = eventData.Context.ChangeTracker.Entries().ToArray();
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }


    // ============================ Failed ============================
    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        base.SaveChangesFailed(eventData);
    }
    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        //var entries = eventData.Context.ChangeTracker.Entries().ToArray();
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }





}
