using Microsoft.EntityFrameworkCore;
using TestBackNuxiba.Models;

namespace TestBackNuxiba.Data;

public class CCenterDbContext : DbContext
{
    public CCenterDbContext(
        DbContextOptions<CCenterDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Login> Logins => Set<Login>();

    public DbSet<Area> Areas => Set<Area>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureArea(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureLogin(modelBuilder);
    }

    private static void ConfigureArea(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("ccRIACat_Areas");

            entity.HasKey(a => a.IDArea);

            entity.Property(a => a.IDArea)
                .ValueGeneratedOnAdd();

            entity.Property(a => a.AreaName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(a => a.StatusArea)
                .IsRequired();

            entity.Property(a => a.CreateDate)
                .IsRequired();

            entity.HasMany(a => a.Users)
                .WithOne(u => u.Area)
                .HasForeignKey(u => u.IDArea)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("ccUsers");

            entity.HasKey(u => u.User_id);

            entity.Property(u => u.User_id)
                .ValueGeneratedOnAdd();

            entity.Property(u => u.Login)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(u => u.Nombres)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(u => u.ApellidoPaterno)
                .HasMaxLength(100);

            entity.Property(u => u.ApellidoMaterno)
                .HasMaxLength(100);

            entity.Property(u => u.Password)
                .HasMaxLength(255);

            entity.Property(u => u.TipoUser_id);

            entity.Property(u => u.Status)
                .IsRequired();

            entity.Property(u => u.fCreate)
                .IsRequired();

            entity.Property(u => u.IDArea);

            entity.Property(u => u.LastLoginAttempt);

            entity.HasIndex(u => u.Login)
                .IsUnique();

            entity.HasMany(u => u.Logins)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.User_id)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureLogin(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Login>(entity =>
        {
            entity.ToTable("ccloglogin");

            entity.HasKey(l => l.LogLoginId);

            entity.Property(l => l.LogLoginId)
                .ValueGeneratedOnAdd();

            entity.Property(l => l.User_id)
                .IsRequired();

            entity.Property(l => l.Extension)
                .IsRequired();

            entity.Property(l => l.TipoMov)
                .IsRequired();

            entity.Property(l => l.fecha)
                .IsRequired();

            entity.HasCheckConstraint(
                "CK_ccloglogin_TipoMov",
                "[TipoMov] IN (0, 1)"
            );

            entity.HasIndex(l => new
            {
                l.User_id,
                l.fecha
            });

            entity.HasIndex(l => new
            {
                l.User_id,
                l.TipoMov,
                l.fecha
            });
        });
    }
}