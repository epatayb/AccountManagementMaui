using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Entities.Common;
using AccountManagementMaui.Api.Services.CurrentUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountManagementMaui.Api.Data.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    private readonly ICurrentUserService _currentUserService;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<City> Cities => Set<City>();

    public DbSet<District> Districts => Set<District>();

    public DbSet<TaxOffice> TaxOffices => Set<TaxOffice>();

    public DbSet<AccountCardType> AccountCardTypes => Set<AccountCardType>();

    public DbSet<AccountCardKind> AccountCardKinds => Set<AccountCardKind>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureIdentityTables(modelBuilder);
        ConfigureAppUser(modelBuilder);
        ConfigureCity(modelBuilder);
        ConfigureDistrict(modelBuilder);
        ConfigureTaxOffice(modelBuilder);
        ConfigureAccountCardType(modelBuilder);
        ConfigureAccountCardKind(modelBuilder);
    }

    private static void ConfigureIdentityTables(
       ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>()
            .ToTable("Users");

        modelBuilder.Entity<AppRole>()
            .ToTable("Roles");

        modelBuilder.Entity<IdentityUserRole<int>>()
            .ToTable("UserRoles");

        modelBuilder.Entity<IdentityUserClaim<int>>()
            .ToTable("UserClaims");

        modelBuilder.Entity<IdentityUserLogin<int>>()
            .ToTable("UserLogins");

        modelBuilder.Entity<IdentityRoleClaim<int>>()
            .ToTable("RoleClaims");

        modelBuilder.Entity<IdentityUserToken<int>>()
            .ToTable("UserTokens");
    }

    private static void ConfigureAppUser(
        ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<AppUser>();

        entity.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(x => x.DeleteReason)
            .HasMaxLength(500)
            .IsRequired(false);

        entity.Property(x => x.CreatedDate)
            .IsRequired();

        entity.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(x => x.ModifiedByUser)
            .WithMany()
            .HasForeignKey(x => x.ModifiedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasIndex(x => x.IsDeleted);
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
            .HasMaxLength(500)
            .IsRequired(false);

        entity.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(x => x.ModifiedByUser)
            .WithMany()
            .HasForeignKey(x => x.ModifiedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasQueryFilter(x => !x.IsDeleted);
    }

    private static void ConfigureTaxOffice(
    ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TaxOffice>();

        entity.ToTable("TaxOffices");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.TaxOfficeCode)
            .IsRequired()
            .HasMaxLength(20);

        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        ConfigureBaseEntity(entity);

        entity.HasIndex(x => x.TaxOfficeCode)
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
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureAccountCardType(
    ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<AccountCardType>();

        entity.ToTable("AccountCardTypes");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.TypeCode)
            .IsRequired()
            .HasMaxLength(20);

        entity.Property(x => x.TypeName)
            .IsRequired()
            .HasMaxLength(50);

        ConfigureBaseEntity(entity);

        entity.HasIndex(x => x.TypeCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        entity.HasIndex(x => x.TypeName)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }

    private static void ConfigureAccountCardKind(
    ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<AccountCardKind>();

        entity.ToTable("AccountCardKinds");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.KindCode)
            .IsRequired()
            .HasMaxLength(20);

        entity.Property(x => x.KindName)
            .IsRequired()
            .HasMaxLength(50);

        ConfigureBaseEntity(entity);

        entity.HasIndex(x => x.KindCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        entity.HasIndex(x => new
        {
            x.AccountCardTypeId,
            x.KindName
        })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        entity.HasOne(x => x.AccountCardType)
            .WithMany(x => x.AccountCardKinds)
            .HasForeignKey(x => x.AccountCardTypeId)
            .OnDelete(DeleteBehavior.Restrict);
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
        var currentUserId = _currentUserService.UserId;

        ApplyBaseEntityAudit(
            now,
            currentUserId);

        ApplyAppUserAudit(
            now,
            currentUserId);
    }

    private void ApplyBaseEntityAudit(
        DateTime now,
        int? currentUserId)
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.IsDeleted = false;

                if (!entry.Entity.CreatedByUserId.HasValue)
                {
                    entry.Entity.CreatedByUserId =
                        currentUserId;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedDate = now;
                entry.Entity.ModifiedByUserId =
                    currentUserId;
            }
        }
    }

    private void ApplyAppUserAudit(
       DateTime now,
       int? currentUserId)
    {
        var entries = ChangeTracker
            .Entries<AppUser>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.IsDeleted = false;

                if (!entry.Entity.CreatedByUserId.HasValue)
                {
                    entry.Entity.CreatedByUserId =
                        currentUserId;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedDate = now;
                entry.Entity.ModifiedByUserId =
                    currentUserId;
            }
        }
    }
}