using Demo.Api.Contracts.Auditing;
using Demo.Api.Contracts.Concurrancy;
using Demo.Api.Contracts.Exceptions;
using Demo.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Demo.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        internal readonly IProfileAccessor? _accessor;

        public Guid? CurrentTenantId { get { return _accessor?.AuthProfile?.TenantId; } }
        public Guid? CurrentUserId { get { return _accessor?.AuthProfile?.UserId; } }
        public bool IsSqlServer { get; set; }
        public bool IsSqlite { get; set; }

        protected ApplicationDbContext()
        {
        }

        protected ApplicationDbContext(DbContextOptions options, IProfileAccessor accessor) : base(options)
        {
            _accessor = accessor;
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
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


        internal void IncludeTenantInIndexes(ModelBuilder modelBuilder, IMutableEntityType x)
        {
            var modelType = modelBuilder.Model.FindEntityType(x.ClrType);
            List<IMutableIndex> indexes = modelType!
                .GetIndexes()
                .Where(i => !(i.IsClustered() ?? false))
                .ToList();

            if (indexes.Count == 0)
            {
                modelBuilder.Entity(x.ClrType)
                    .HasIndex(nameof(ITenantData.TenantId));
            }
            indexes.ForEach(i =>
            {
                var props = i.Properties;
                if (!props.Any(p => p.Name == nameof(ITenantData.TenantId)))
                {
                    modelType.RemoveIndex(i);
                    var newIndex = CloneIndexWithTenantId(modelType, i, props);
                }
            });
        }

        internal IMutableIndex CloneIndexWithTenantId(IMutableEntityType modelType, IMutableIndex i, IEnumerable<IMutableProperty>? props)
        {
            var newProps = (props ?? Array.Empty<IMutableProperty>()).ToList();
            newProps.Insert(0, modelType.FindProperty(nameof(ITenantData.TenantId))!);
            var newIndex = modelType.AddIndex(newProps);
            newIndex.IsDescending = i.IsDescending;
            newIndex.IsUnique = i.IsUnique;
            newIndex.AddAnnotations(i.GetAnnotations());

            if (i.GetFillFactor() is not null)
                newIndex.SetFillFactor(i.GetFillFactor());
            if (i.GetFilter() is not null)
                newIndex.SetFilter(i.GetFilter());
            if (i.GetIncludeProperties() is not null)
                newIndex.SetIncludeProperties(i.GetIncludeProperties()!);
            if (i.IsClustered() is not null)
                newIndex.SetIsClustered(i.IsClustered());
            if (i.IsCreatedOnline() is not null)
                newIndex.SetIsCreatedOnline(i.IsCreatedOnline());
            return newIndex;
        }
        internal void PreProcessing()
        {
            var now = DateTimeOffset.UtcNow;

            if (CurrentTenantId != default)
            {
                // Set TenantId to the current user's tenant if not set
                ChangeTracker.Entries()
                    .Where(x => x.Entity is ITenantData e && e.TenantId == default && new[] { EntityState.Added, EntityState.Modified }.Contains(x.State))
                    .Select(x => x.Entity as ITenantData).ToList()
                    .ForEach(x => x!.TenantId = CurrentTenantId!.Value);
            }

            if (CurrentTenantId != default && ChangeTracker.Entries()
                .Any(x => x.Entity is ITenantData e && e.TenantId != CurrentTenantId && new[] { EntityState.Added, EntityState.Modified, EntityState.Deleted }.Contains(x.State)))
            {
                throw new UpdateSystemDataException();
            }

            ChangeTracker.Entries()
                .Where(x => x.Entity is ISoftDelete && new[] { EntityState.Deleted }.Contains(x.State))
                .ToList().ForEach(x => { x.State = EntityState.Modified; ((ISoftDelete)x.Entity).IsDeleted = true; });

            ChangeTracker.Entries()
                .Where(x => x.Entity is ITrackCreated && new[] { EntityState.Added }.Contains(x.State))
                .Select(x => x.Entity as ITrackCreated).ToList()
                .ForEach(x => { x!.CreatedOn = x.CreatedOn == default ? now : x.CreatedOn; });

            ChangeTracker.Entries()
                .Where(x => x.Entity is ITrackModified && new[] { EntityState.Added, EntityState.Modified }.Contains(x.State))
                .Select(x => x.Entity as ITrackModified).ToList()
                .ForEach(x => { x!.ModifiedOn = this.IsEntryModified(x, y => y.ModifiedOn) ? x!.ModifiedOn : now; });

            //ChangeTracker.Entries()
            //    .Where(x => x.Entity is IRemoteData && new[] { EntityState.Added, EntityState.Modified }.Contains(x.State))
            //    .Select(x => x.Entity as IRemoteData).ToList()
            //    .ForEach(x => { x!.LocalUpdateTime = now; });

            //ChangeTracker.Entries()
            //    .Where(x => x.Entity is IHasTimeZone && new[] { EntityState.Added, EntityState.Modified }.Contains(x.State))
            //    .Select(x => x.Entity as IHasTimeZone).ToList()
            //    .ForEach(x => TimeZoneHelper.ApplyTimeZones(x!));

            if (default != CurrentUserId)
            {
                ChangeTracker.Entries()
                    .Where(x => x.Entity is IAuditCreated<Guid> && new[] { EntityState.Added }.Contains(x.State))
                    .Select(x => x.Entity as IAuditCreated<Guid>).ToList()
                    .ForEach(x => { x!.CreatedBy = x.CreatedBy == default ? CurrentUserId!.Value : x.CreatedBy; });

                ChangeTracker.Entries()
                    .Where(x => x.Entity is IAuditModified<Guid> && new[] { EntityState.Added, EntityState.Modified }.Contains(x.State))
                    .Select(x => x.Entity as IAuditModified<Guid>).ToList()
                    .ForEach(x => { x!.ModifiedBy = CurrentUserId!.Value; });
            }
        }

        protected static void SetUpMaxLengthViaInterfaces(ModelBuilder modelBuilder)
        {
            foreach (var entityType in GetEntityTypes(modelBuilder))
            {
                var clrType = entityType.ClrType;
                var interfaces = clrType.GetInterfaces();

                foreach (var interfaceType in interfaces)
                {
                    var interfaceProperties = interfaceType.GetProperties();

                    foreach (var interfaceProperty in interfaceProperties)
                    {
                        var maxLengthAttribute = interfaceProperty.GetCustomAttribute<MaxLengthAttribute>();
                        if (maxLengthAttribute != null)
                        {
                            var entityProperty = clrType.GetProperty(interfaceProperty.Name);
                            if (entityProperty != null)
                            {
                                modelBuilder.Entity(clrType)
                                    .Property(entityProperty.Name)
                                    .HasMaxLength(maxLengthAttribute.Length);
                            }
                        }
                    }
                }
            }
        }
        private static IEnumerable<IMutableEntityType> GetEntityTypes(ModelBuilder modelBuilder)
        {
            return modelBuilder.Model.GetEntityTypes();
        }

        private static IEnumerable<IMutableEntityType> GetBaseEntityTypes(ModelBuilder modelBuilder)
        {
            return modelBuilder.Model.GetEntityTypes().Where(x => x.BaseType == null);
        }
        // Work around for DateTimeOffset order by not being supported by SQL Lite
        class DateTimeOffsetConverter : ValueConverter<DateTimeOffset?, string?>
        {
            public DateTimeOffsetConverter()
                : base(d => d != null ? d.Value.UtcDateTime.ToString("O") : null,   // Serialize to string using the round-trip format
                    s => s != null ? DateTimeOffset.Parse(s) : null)             // Deserialize from string
            {
            }
        }
    }
}
