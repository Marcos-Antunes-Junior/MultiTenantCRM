using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Models;
using multiTenantCRM.Services;

namespace multiTenantCRM.Data
{
    // Database context. It is responsible for the database connection, migrations,
    // and tenant application logic.
    public class CrmDbContext : DbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public CrmDbContext(DbContextOptions<CrmDbContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Deal> Deals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>().ToTable("Tenants");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Deal>().ToTable("Deals");

            modelBuilder.Entity<Customer>().HasIndex(c => new { c.TenantId, c.Email });
            modelBuilder.Entity<Deal>().HasIndex(d => new { d.TenantId});

            // Global query filters (Tenant-aware)
            modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == _tenantProvider.TenantId);
            modelBuilder.Entity<Customer>().HasQueryFilter(c => c.TenantId == _tenantProvider.TenantId);
            modelBuilder.Entity<Deal>().HasQueryFilter(d => d.TenantId == _tenantProvider.TenantId);
        }

         public override int SaveChanges()
        {
            ApplyTenantId();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTenantId();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyTenantId()
        {
            var tenantId = _tenantProvider.TenantId;

            foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TenantId = tenantId;
                }
            }
        }
    }
}
