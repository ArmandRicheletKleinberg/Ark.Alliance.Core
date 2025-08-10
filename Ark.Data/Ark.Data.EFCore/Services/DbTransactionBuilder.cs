using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ark;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Builds multi-entity database transactions.
    /// + Chains create, update, or delete operations before execution.
    /// - Does not execute transactions; used with <see cref="DbServices{TContext}.UseTransaction"/>.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/saving/transactions"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    public class DbTransactionBuilder<TContext>
        where TContext : DbContextEx, new()
    {
        #region Fields

        /// <summary>
        /// The internal list of the builder items. Each item is a manipulation on a single or multiple entities.
        /// </summary>
        internal readonly List<DbTransactionBuilderItem> Items = new List<DbTransactionBuilderItem>();

        #endregion Fields

        #region Methods (Create)

        /// <summary>
        /// Queues entities for creation in the database.
        /// + Allows multiple inserts in a single transaction.
        /// - Entities are not validated until execution.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to create.</typeparam>
        /// <param name="entities">The entities to create.</param>
        /// <returns>The same builder to chain.</returns>
        public DbTransactionBuilder<TContext> Create<TEntity>(params TEntity[] entities)
            where TEntity : DbEntity<TContext>, new()
            => AddItem(EntityState.Added, entities);

        #endregion Methods (Create)

        #region Methods (Update)

        /// <summary>
        /// Queues entities for update in the database.
        /// + Enables batching of modifications.
        /// - Uses Added state, requiring accurate property tracking.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to update.</typeparam>
        /// <param name="entities">The entities to update.</param>
        /// <returns>The same builder to chain.</returns>
        public DbTransactionBuilder<TContext> Update<TEntity>(params TEntity[] entities)
        where TEntity : DbEntity<TContext>, new()
        => AddItem(EntityState.Added, entities);

        /// <summary>
        /// Updates only specified properties of an entity in the database.
        /// + Limits data transfer to changed fields.
        /// - Developer must ensure property names are valid.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to update.</typeparam>
        /// <param name="entity">The entity to update.</param>
        /// <param name="properties">The only properties to update. The other properties are ignored.</param>
        /// <returns>The same builder to chain.</returns>
        public DbTransactionBuilder<TContext> UpdatePartial<TEntity>(TEntity entity,
        params Expression<Func<TEntity, object>>[] properties)
        where TEntity : DbEntity<TContext>, new()
        => AddItem(EntityState.Added, new[] { entity }, properties.Select(f => f.GetFirstMemberOrMethodName()).ToArray());

        /// <summary>
        /// Updates only specified properties of multiple entities in the database.
        /// + Reduces partial update boilerplate.
        /// - Large entity arrays may increase memory usage.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to update.</typeparam>
        /// <param name="entities">The entities to update.</param>
        /// <param name="properties">The only properties to update. The other properties are ignored.</param>
        /// <returns>The same builder to chain.</returns>
        public DbTransactionBuilder<TContext> UpdatePartial<TEntity>(TEntity[] entities,
            params Expression<Func<TEntity, object>>[] properties)
            where TEntity : DbEntity<TContext>, new()
            => AddItem(EntityState.Added, entities, properties.Select(f => f.GetFirstMemberOrMethodName()).ToArray());

        #endregion Methods (Update)

        #region Methods (Remove)

        /// <summary>
        /// Queues entities for removal from the database.
        /// + Deletions participate in the encompassing transaction.
        /// - Removal is deferred until execution.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to remove.</typeparam>
        /// <param name="entities">The entities to remove.</param>
        /// <returns>The same builder to chain.</returns>
        protected DbTransactionBuilder<TContext> Remove<TEntity>(params TEntity[] entities)
            where TEntity : DbEntity<TContext>, new()
            => AddItem(EntityState.Deleted, entities);

        #endregion Methods (Remove)

        #region Methods (Helpers)

        /// <summary>
        /// Helper to add some item given the manipulation to do.
        /// + Centralizes item creation logic.
        /// - Accepts raw property names, which may be brittle.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to manipulate.</typeparam>
        /// <param name="state">The type of the manipulation to execute.</param>
        /// <param name="entities">The entities to manipulate.</param>
        /// <param name="updateOnlyTheseProperties">For update only, the properties to update. The other properties are ignored and thus not updated.</param>
        /// <returns>The same builder to chain.</returns>
        private DbTransactionBuilder<TContext> AddItem<TEntity>(EntityState state, TEntity[] entities,
            string[] updateOnlyTheseProperties = null)
        {
            Items.Add(new DbTransactionBuilderItem
            {
                State = state,
                Entities = entities.IfNotNull().Cast<object>().ToArray(),
                UpdateOnlyTheseProperties = updateOnlyTheseProperties
            });
            return this;
        }

        #endregion Methods (Helpers)

    }
}