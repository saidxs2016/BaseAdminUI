using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
namespace DAL.MainDB.Repositories.Interfaces;

// Base Repository İçinde Bir Değişiklik Yapmak Yasaktır. (SAİD'e Sorunuz.)
public interface IBaseRepository<T> where T : class
{

    // ========================= Queries =========================
    Guid GetInstanceId();
    DbSet<T> Entity();
    IQueryable<T> AsQueryable();
    IQueryable<T> AsQueryable(Expression<Func<T, bool>> predicate);


    List<T> GetAll();
    Task<List<T>> GetAllAsync(CancellationToken token = default);

    List<T> GetAsWhere(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetAsWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default);

    T GetAsFirstOrDefault(Expression<Func<T, bool>> predicate);
    Task<T> GetAsFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default);

    int GetCount(Expression<Func<T, bool>> predicate);
    Task<int> GetCountAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default);

    bool Exist(Expression<Func<T, bool>> predicate);
    Task<bool> ExistAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default);


    // ========================= Commands =========================
    T Add(T entity);
    ValueTask<T> AddAsync(T entity, CancellationToken token = default);

    void AddRange(List<T> entities);
    Task AddRangeAsync(List<T> entities, CancellationToken token = default);

    T Update(T entity);
    void UpdateRange(List<T> entities);

    T Delete(T entity);
    void DeleteRange(List<T> entities);
    ValueTask<int> ExecuteUpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> props, CancellationToken token = default);
    ValueTask<int> ExecuteDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default);
}
