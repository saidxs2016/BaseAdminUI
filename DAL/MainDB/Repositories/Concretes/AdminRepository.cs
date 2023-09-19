using DAL.MainDB.Context;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;

namespace DAL.MainDB.Repositories.Concretes;

public class AdminRepository : BaseRepository<Admin>, IAdminRepository
{
    //private readonly MDbContext _db;

    public AdminRepository(MDbContext db) : base(db)
    {
        //_db = db;
    }



}
