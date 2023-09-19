using DAL.MainDB.Context;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;

namespace DAL.MainDB.Repositories.Concretes;

public class ModuleRepository : BaseRepository<Module>, IModuleRepository
{
    //private readonly MDbContext _db;

    public ModuleRepository(MDbContext db) : base(db)
    {
        //_db = db;
    }




}
