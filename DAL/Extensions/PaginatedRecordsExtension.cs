using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace DAL.Extensions;

public static class PaginatedRecordsExtension
{
    public static async Task<List<T>> PaginatedRecordsAsync<T>(this IQueryable<T> query, PaginatedProps props, CancellationToken token = default)
    {
        // sıralama şekli
        if (!string.IsNullOrEmpty(props.OrderBy) && !string.IsNullOrEmpty(props.OrderByDirection))
            query = query.OrderBy(props.OrderBy + " " + props.OrderByDirection);


        // kaç kayıt atlanacak.
        if (props.Skip > 0)
            query = query.Skip(props.Skip);

        // kaç adet çekileceği
        if (props.PageSize > 0)
            query = query.Take(props.PageSize);

        // query'i çalıştır.
        return await query.ToListAsync(cancellationToken: token);

    }

    public static List<T> PaginatedRecords<T>(this IQueryable<T> query, PaginatedProps props)
    {
        // sıralama şekli
        if (!string.IsNullOrEmpty(props.OrderBy) && !string.IsNullOrEmpty(props.OrderByDirection))
            query = query.OrderBy(props.OrderBy + " " + props.OrderByDirection);


        // kaç kayıt atlanacak.
        if (props.Skip > 0)
            query = query.Skip(props.Skip);

        // kaç adet çekileceği
        if (props.PageSize > 0)
            query = query.Take(props.PageSize);

        // query'i çalıştır.
        return query.ToList();

    }
}

public partial class PaginatedProps
{
    // =============== genel =============== 
    public virtual string? OrderBy { get; set; } = null;
    public virtual string? OrderByDirection { get; set; } = "asc"; // asc | desc
    public virtual int Page { get; set; } = 1;
    public virtual int PageSize { get; set; } = 10; // default == 10 , if take == -1 its meean take all 

    private int _skip = -1;
    public virtual int Skip
    {
        get => _skip != -1 ? _skip : (Page - 1) * PageSize;
        set { _skip = value; }
    }

}
