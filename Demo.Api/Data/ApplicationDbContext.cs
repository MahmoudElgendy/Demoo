using Demo.Api.Middlewares;
using Demo.Api.Models;
using gendiLib.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUser _current;

        protected ApplicationDbContext(DbContextOptions options, ICurrentUser current) : base(options)
        {
          
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUser current)
            : base(options)
        {
            _current = current;
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);


        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            PreProcessing();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            PreProcessing();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void PreProcessing()
        {
            var now = DateTime.UtcNow;
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .ToList();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is BaseModel<Guid> createdAudited)
                    {
                        if (!_current.HasTenant)
                        {
                            throw new InvalidOperationException("Tenant information is missing.");
                        }
                        if (!_current.HasUser)
                        {
                            throw new InvalidOperationException("User  information is missing.");
                        }
                        createdAudited.CreatedBy = _current.UserId.Value;
                        createdAudited.CreatedOn = now;
                        createdAudited.ModifiedBy = _current.UserId.Value;
                        createdAudited.ModifiedOn = now;
                        createdAudited.TenantId = _current.TenantId.Value;
                    }
                }
                else
                {
                    if (entry.Entity is BaseModel<Guid> modifiedAudited)
                    {
                        if (!_current.HasTenant)
                        {
                            throw new InvalidOperationException("Tenant information is missing.");
                        }
                        if (!_current.HasUser)
                        {
                            throw new InvalidOperationException("User  information is missing.");
                        }
                        modifiedAudited.ModifiedBy = _current.UserId.Value;
                        modifiedAudited.ModifiedOn = now;
                    }
                }
            }
        }
    }
}
