using DAL.MainSysDB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.MainSysDB.Context;

public partial class SysDbContext : DbContext
{
    public SysDbContext()
    {
    }

    public SysDbContext(DbContextOptions<SysDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DeletedRecord> DeletedRecords { get; set; }


    public virtual DbSet<UserLog> UserLogs { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseNpgsql("Host=localhost;Database=main_sys_db;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeletedRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("deleted_record_pkey");

            entity.Property(e => e.InstanceId).HasComment("// dbcontext unique uid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
