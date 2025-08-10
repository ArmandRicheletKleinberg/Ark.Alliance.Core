using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Ark.Data.EFCore
{
    /// <inheritdoc />
    /// <summary>
    /// The extended DB context allows more operation on a DBContext :
    /// - Defining indexes on the model using <see cref="IndexAttribute"/>.
    /// - Defining composite keys on the model using <see cref="CompositeKeyAttribute"/>.
    /// - Defining the kind of datetime (UTC or local) on the model using <see cref="DateTimeKindAttribute"/> or globally using <see cref="DatabaseOptions.GlobalDateTimeGlobalKind"/>.
    /// - Allows to use a built-in key-value table to store settings.
    /// </summary>
    public abstract class DbContextEx : DbContext
    {
        #region Static

        /// <summary>
        /// Stores the database options for each context type. This allows
        /// injecting options into a <see cref="DbContextEx"/> without requiring
        /// constructor parameters.
        /// Made public so external assemblies can configure contexts directly.
        /// </summary>
        public static readonly ConcurrentDictionary<Type, DatabaseOptions> OptionsByType = new ConcurrentDictionary<Type, DatabaseOptions>();

        #endregion Static

        #region Fields

        /// <summary>
        /// The database options which manage the database context coming from the app settings or hardcoded.
        /// </summary>
        protected readonly DatabaseOptions Options;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="DbContextEx"/> instance.
        /// </summary>
        /// <exception cref="Exception">The database context must be previously setup by using the UseDatabase method in the IHostBuilder.</exception>
        protected DbContextEx()
        {
            Options = OptionsByType.GetValue(GetType());
        }

        #endregion Constructors

        #region Methods (OnModelCreating)

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // First search all the entities in the assembly of the context which inherits EntityBase and which are not abstract
            var thisType = GetType();
            var dbEntityType = typeof(DbEntity<>).MakeGenericType(GetType());
            var entityTypes = thisType.Assembly.DefinedTypes
                .Where(t => dbEntityType.IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .ToArray();

            entityTypes.ForEach(entityType =>
            {
                if (entityType.GetCustomAttribute<NotMappedAttribute>() != null)
                    return;

                // Gets or adds the entity to the model
                var entity = modelBuilder.Model.FindEntityType(entityType.UnderlyingSystemType) ?? modelBuilder.Model.AddEntityType(entityType.UnderlyingSystemType);

                // If the entity is linked to a view then specify it in the model
                var viewAttribute = entityType.GetCustomAttribute<ViewAttribute>();
                if (viewAttribute != null)
                    modelBuilder.Entity(entityType, e => e.ToView(viewAttribute.ViewName));

                // By default the foreign key constraint is enforced by RESTRICT instead of EF Core default which is CASCADE
                modelBuilder.Model.GetEntityTypes()
                    .SelectMany(t => t.GetForeignKeys())
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
                // TODO : add a CascadeAttribute

                // Sets the index if the Index attribute has been set
                //var indexProperties = entity.GetProperties().Where(p => (p.PropertyInfo?.GetCustomAttributes<IndexAttribute>() ?? new IndexAttribute[0]).Any()).ToArray();
                //indexProperties.ForEach(indexProperty => entity.AddIndex(new[] { indexProperty }));

                var entityTypeBuilder = modelBuilder.Entity(entityType);
                foreach (var property in entity.GetProperties())
                {
                    if (property.PropertyInfo == null)
                        return;

                    var indexAttribute = property.PropertyInfo.GetCustomAttribute<IndexAttribute>();
                    if (indexAttribute != null)
                        entity.AddIndex(new[] { property });


                    var hasDefaultValueAttribute = property.PropertyInfo.GetCustomAttribute<HasDefaultValueAttribute>();
                    if (hasDefaultValueAttribute != null)
                        entityTypeBuilder.Property(property.Name).HasDefaultValue(hasDefaultValueAttribute.DefaultValue);

                    var hasDefaultValueSqlAttribute = property.PropertyInfo.GetCustomAttribute<HasDefaultValueSqlAttribute>();
                    if (hasDefaultValueSqlAttribute != null)
                        entityTypeBuilder.Property(property.Name).HasDefaultValueSql(hasDefaultValueSqlAttribute.DefaultValueSql);
                }

                // Creates a composite key if needed
                var compositeKeyProperties = entity.GetProperties().Where(p => (p.PropertyInfo?.GetCustomAttributes<CompositeKeyAttribute>() ?? new CompositeKeyAttribute[0]).Any()).ToArray();
                if (compositeKeyProperties.Length > 1)
                    compositeKeyProperties.ForEach(compositeKeyProperty => modelBuilder.Entity(entityType).HasKey(compositeKeyProperties.Select(p => p.Name).ToArray()));
            });

            if (Options?.UseSettingsTable ?? false)
            {
                var settingsType = typeof(SettingsDbEntity<>).MakeGenericType(GetType());
                modelBuilder.Entity(settingsType).ToTable(Options.SettingsTableName);
            }

            CheckModelForDataTimeKind(modelBuilder);
        }

        /// <summary>
        /// Checks the model for date time kind specific attribute.
        /// It searches the whole model that is all properties of all entities.
        /// It applies a conversion in both direction to transform the datetime in the correct kind.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to create the EF Core model.</param>
        private void CheckModelForDataTimeKind(ModelBuilder modelBuilder)
        {
            modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties())
                .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?))
                .ForEach(property =>
                {
                    var attribute = property.PropertyInfo.GetCustomAttribute<DateTimeKindAttribute>();
                    var kind = attribute?.Kind ?? Options?.GlobalDateTimeGlobalKind ?? DateTimeKind.Utc;
                    if (kind == DateTimeKind.Unspecified)
                        return;

                    if (property.ClrType == typeof(DateTime))
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.ToKind(kind),
                            v => DateTime.SpecifyKind(v, kind)));
                    else if (property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? v.Value.ToKind(kind) : (DateTime?)null,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, kind) : (DateTime?)null));
                });
        }

        #endregion Methods (OnModelCreating)
    }
}