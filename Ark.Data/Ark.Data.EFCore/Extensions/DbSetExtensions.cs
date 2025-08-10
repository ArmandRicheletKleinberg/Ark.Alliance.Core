using System;
using System.Collections.Generic;
using System.Linq;
using Ark;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Extension methods for <see cref="DbSet{TEntity}"/> providing simple upsert semantics.
    /// + Consolidates repetitive add/update logic.
    /// - Reflection-based key discovery may impact performance on large models.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/saving/basic"/>
    /// </summary>
    public static class DbSetExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Inserts a new entity or updates an existing one based on its primary key values.
        /// + Avoids separate existence checks before saving.
        /// - Requires default-valued key properties to detect new instances.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/saving/"/>
        /// </summary>
        /// <typeparam name="TEntity">Entity type to insert or update.</typeparam>
        /// <param name="dbSet">Database set managing <typeparamref name="TEntity"/>.</param>
        /// <param name="data">Entity instance to persist.</param>
        public static void CreateOrUpdate<TEntity>(this DbSet<TEntity> dbSet, TEntity data)
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            var context = dbSet.GetService<ICurrentDbContext>().Context;
            var ids = new HashSet<string>(context.Model.FindEntityType(entityType).FindPrimaryKey().Properties.Select(x => x.Name));
            var keyProperties = entityType.GetProperties().Where(p => ids.Contains(p.Name)).ToArray();
            if (keyProperties.HasNoElements())
                throw new Exception($"{entityType.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");

            var primaryKeyIsNotSet = keyProperties.All(p => p.GetValue(data) == p.PropertyType.GetDefaultValue());
            if (primaryKeyIsNotSet)
                dbSet.Add(data);
            else
                dbSet.Update(data);
        }

        #endregion Methods (Public)
    }
}
