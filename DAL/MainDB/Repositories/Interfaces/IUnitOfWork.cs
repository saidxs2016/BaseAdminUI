using DAL.MainDB.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace DAL.MainDB.Repositories.Interfaces;


public interface IUnitOfWork
{

    Guid GetInstanceId();
    MDbContext DbContext { get; }
    int SaveChange();
    Task<int> SaveChangesAsync(CancellationToken token = default);


    // ========================= Transctions =========================    
    void BeginTransaction();
    Task BeginTransactionAsync(CancellationToken token = default);
    void RollbackTransaction();
    Task RollbackTransactionAsync(CancellationToken token = default);
    void CommitTransaction();
    Task CommitTransactionAsync(CancellationToken token = default);
    void DisposeTransaction();
    Task DisposeTransactionAsync(CancellationToken token = default);

}
