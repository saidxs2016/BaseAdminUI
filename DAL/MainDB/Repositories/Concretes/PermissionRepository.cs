using DAL.MainDB.Context;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;

namespace DAL.MainDB.Repositories.Concretes;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    //private readonly MDbContext _db;

    public PermissionRepository(MDbContext db) : base(db)
    {
        // _db = db;
    }

}
