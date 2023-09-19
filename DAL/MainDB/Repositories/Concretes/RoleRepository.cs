using DAL.MainDB.Context;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;

namespace DAL.MainDB.Repositories.Concretes;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    //private readonly MDbContext _db;

    public RoleRepository(MDbContext db) : base(db)
    {
        //_db = db;
    }



}
