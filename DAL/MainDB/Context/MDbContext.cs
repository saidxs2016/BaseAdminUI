using DAL.MainDB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.MainDB.Context;

public partial class MDbContext : DbContext
{
    public MDbContext()
    {
    }

    public MDbContext(DbContextOptions<MDbContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseNpgsql("Host=localhost;Database=main_db;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("admin_pkey");

            entity.ToTable("admin", tb => tb.HasComment("adminler tablosu"));

            entity.Property(e => e.Uid).ValueGeneratedNever();
            entity.Property(e => e.IsConfirmed).HasComment("hesab aktifleştirildi mi");
            entity.Property(e => e.IsSuspend).HasComment("hesap askıya alındı mı");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("module_pkey");

            entity.ToTable("module", tb => tb.HasComment("admin panel modulleri"));

            entity.Property(e => e.Uid).ValueGeneratedNever();
            entity.Property(e => e.Order).HasDefaultValueSql("9999");
            entity.Property(e => e.Type).HasComment("module tipi: genel de 3 tip var: Category, Page, Feature");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("permission_pkey");

            entity.ToTable("permission", tb => tb.HasComment("yetkiler tablosu"));

            entity.Property(e => e.Uid).ValueGeneratedNever();
            entity.Property(e => e.IgnoredSections).HasComment("[.authorization-key,input[type=\"password\"], input[name=\"City\"]]");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("role_pkey");

            entity.ToTable("role", tb => tb.HasComment("roller tablosu"));

            entity.Property(e => e.Uid).ValueGeneratedNever();
            entity.Property(e => e.Expiration).HasComment("oturum süresi belirleme örn: 1 saat, 2 gün /// 1 Minute gibi\r\n\r\n1 m = 1 dakika\r\n1 h = 60 dakika\r\n1 d = 24*60 dakika\r\n1 y = 365*24*60 dakika");
            entity.Property(e => e.LoginCount).HasComment("aynı anda kaç farklı cihazda ourum açabilir");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
