using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManagementMaui.Api.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<City> Cities => Set<City>();

    public DbSet<District> Districts => Set<District>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCity(modelBuilder);
        ConfigureDistrict(modelBuilder);
    }

    private static void ConfigureCity(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<City>();

        entity.ToTable("Cities");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.CityCode)
            .IsRequired()
            .HasMaxLength(2);

        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(20);

        ConfigureBaseEntity(entity);

        entity.HasIndex(x => x.CityCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        entity.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }

    private static void ConfigureDistrict(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<District>();

        entity.ToTable("Districts");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.DistrictCode)
            .IsRequired()
            .HasMaxLength(20);

        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        ConfigureBaseEntity(entity);

        entity.HasIndex(x => x.DistrictCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        entity.HasIndex(x => new
        {
            x.CityId,
            x.Name
        })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        entity.HasOne(x => x.City)
            .WithMany(x => x.Districts)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureBaseEntity<TEntity>(
        EntityTypeBuilder<TEntity> entity)
        where TEntity : BaseEntity
    {
        entity.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(x => x.CreatedDate)
            .IsRequired();

        entity.Property(x => x.DeleteReason)
            .HasMaxLength(500);

        entity.HasQueryFilter(x => !x.IsDeleted);
    }

    public override int SaveChanges()
    {
        ApplyAuditInformation();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var now = DateTime.UtcNow;

        var entries = ChangeTracker
            .Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.IsDeleted = false;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedDate = now;
            }
        }
    }
}