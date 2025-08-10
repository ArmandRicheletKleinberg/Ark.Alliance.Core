using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Ark.Net.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Generic database services with CRUD support and function/stored procedure execution features.
    /// They generally target a single entity type with its relations.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context for which the entity is linked to.</typeparam>
    /// <typeparam name="TEntity">The entity of the data context used by these services.</typeparam>
    public class DbServices<TContext, TEntity> : DbServices<TContext>
        where TContext : DbContextEx, new()
        where TEntity : DbEntity<TContext>, new()
    {
        #region Methods (Get)

        /// <summary>
        /// Gets a list of entities from the database given a properly built query.
        /// </summary>
        /// <param name="func">The built query to get the entities from the database.</param>
        /// <returns>
        /// Success : The entities are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> Get(Func<IQueryable<TEntity>, IQueryable<TEntity>> func)
            => Select(dbSet => func(dbSet).ToArrayAsync());

        /// <summary>
        /// Gets all entities from the database.
        /// </summary>
        /// <returns>
        /// Success : The entities are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> GetAll()
            => Get(query => query);

        /// <summary>
        /// Gets a list of filtered entities from the database based on a predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the list.</param>
        /// <returns>
        /// Success : The entities are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> GetWhere(Expression<Func<TEntity, bool>> predicate)
            => Get(query => query.Where(predicate));

        /// <summary>
        /// Gets a list of 'x' filtered entities from the database based on a predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the list.</param>
        /// <param name="take">The number of entites max to return.</param>
        /// <returns>
        /// Success : The entities are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> GetWhereTake(Expression<Func<TEntity, bool>> predicate, int take)
            => Get(query => query.Where(predicate).Take(take));

        /// <summary>
        /// Gets an ascending ordered list of entities from the database.
        /// </summary>
        /// <param name="orderBy">The property to order by.</param>
        /// <returns>
        /// Success : The entities are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> GetOrderByAsc<TProperty>(Expression<Func<TEntity, TProperty>> orderBy)
            => Get(query => query.OrderBy(orderBy));

        /// <summary>
        /// Gets a descending ordered list of entities from the database.
        /// </summary>
        /// <param name="orderBy">The property to order by.</param>
        /// <returns>
        /// Success : The entities are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> GetOrderByDesc<TProperty>(Expression<Func<TEntity, TProperty>> orderBy)
            => Get(query => query.OrderByDescending(orderBy));

        /// <summary>
        /// Gets an ascending order list of filtered entities from the database based on a predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the list.</param>
        /// <param name="orderBy">The property to order by.</param>
        /// <returns>
        /// Success : The entities are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> GetWhereOrderByAsc<TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> orderBy)
            => Get(query => query.Where(predicate).OrderBy(orderBy));

        /// <summary>
        /// Gets a descending order list of filtered entities from the database based on a predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the list.</param>
        /// <param name="orderBy">The property to order by.</param>
        /// <returns>
        /// Success : The entities are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> GetWhereOrderByDesc<TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> orderBy)
            => Get(query => query.Where(predicate).OrderByDescending(orderBy));

        #endregion Methods (Get)

        #region Methods (Find)

        /// <summary>
        /// Finds the first entity from the database given a properly built query.
        /// </summary>
        /// <param name="func">The built query to get the entities from the database.</param>
        /// <returns>
        /// Success : The entity is returned.
        /// NotFound : No entity has been found for this query.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity>> Find(Func<IQueryable<TEntity>, IQueryable<TEntity>> func)
            => SelectFirst(dbSet => func(dbSet).FirstOrDefaultAsync());

        /// <summary>
        /// Finds the first entity from the database given a predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to find the entity.</param>
        /// <returns>
        /// Success : The entity is returned.
        /// NotFound : No entity has been found for this predicate.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity>> FindWhere(Expression<Func<TEntity, bool>> predicate)
            => Find(query => query.Where(predicate));

        /// <summary>
        /// Finds the first entity from the database of an ascending ordered list.
        /// </summary>
        /// <param name="orderBy">The property to order by before getting the first entity.</param>
        /// <returns>
        /// Success : The entity is returned.
        /// NotFound : No entity has been found.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity>> FindOrderByAsc<TProperty>(Expression<Func<TEntity, TProperty>> orderBy)
            => Find(query => query.OrderBy(orderBy));

        /// <summary>
        /// Finds the first entity from the database of a descending ordered list.
        /// </summary>
        /// <param name="orderBy">The property to order by before getting the first entity.</param>
        /// <returns>
        /// Success : The entity is returned.
        /// NotFound : No entity has been found.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity>> FindOrderByDesc<TProperty>(Expression<Func<TEntity, TProperty>> orderBy)
            => Find(query => query.OrderByDescending(orderBy));

        /// <summary>
        /// Finds the first entity from the database given a predicate of an ascending ordered list.
        /// </summary>
        /// <param name="predicate">The predicate used to find the entity.</param>
        /// <param name="orderBy">The property to order by before getting the first entity.</param>
        /// <returns>
        /// Success : The entity is returned.
        /// NotFound : No entity has been found for this predicate.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity>> FindWhereOrderByAsc<TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> orderBy)
            => Find(query => query.Where(predicate).OrderBy(orderBy));

        /// <summary>
        /// Finds the first entity from the database given a predicate of a descending ordered list.
        /// </summary>
        /// <param name="predicate">The predicate used to find the entity.</param>
        /// <param name="orderBy">The property to order by before getting the first entity.</param>
        /// <returns>
        /// Success : The entity is returned.
        /// NotFound : No entity has been found for this predicate.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity>> FindWhereOrderByDesc<TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> orderBy)
            => Find(query => query.Where(predicate).OrderByDescending(orderBy));

        /// <summary>
        /// Finds a single entity from the database given its primary key.
        /// </summary>
        /// <param name="primaryKey">The value of the primary key to find.</param>
        /// <returns>
        /// Success : The entity is returned.
        /// NotFound : No entity has been found for this primary key value.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity>> FindByPrimaryKey(object primaryKey)
            => SelectFirst(dbSet => dbSet.FindAsync(primaryKey).AsTask());

        /// <summary>
        /// Finds a single entity from the database given the values of its composed primary key.
        /// </summary>
        /// <param name="primaryKeys">The values of the composed primary key to find.</param>
        /// <returns>
        /// Success : The entity is returned.
        /// NotFound : No entity has been found for this primary key values.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity>> FindByPrimaryKeys(object[] primaryKeys)
            => SelectFirst(dbSet => dbSet.FindAsync(primaryKeys).AsTask());

        #endregion Methods (Find)

        #region Methods (Count)

        /// <summary>
        /// Counts the number of entities matching a given query.
        /// </summary>
        /// <param name="func">The query to count the entities number.</param>
        /// <returns>
        /// Success : The entities count for this query is returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<long>> Count(Func<IQueryable<TEntity>, IQueryable<TEntity>> func)
            => Select(dbSet => func(dbSet).LongCountAsync());

        /// <summary>
        /// Counts the total number of entities in a database table.
        /// </summary>
        /// <returns>
        /// Success : The total number of entities in a database table is returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<long>> CountAll()
            => Count(query => query);

        /// <summary>
        /// Counts the number of entities matching a given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to count the entities number.</param>
        /// <returns>
        /// Success : The entities count for this predicate is returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<long>> CountWhere(Expression<Func<TEntity, bool>> predicate)
            => Count(query => query.Where(predicate));

        #endregion Methods (Count)

        #region Methods (Select Helpers)

        /// <summary>
        /// Returns some result from the database given a properly built query.
        /// It is the underlying method used by Get and Count methods.
        /// </summary>
        /// <param name="func">The built query to get the entities from the database.</param>
        /// <returns>
        /// Success : The result is returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result<TReturn>> Select<TReturn>(Func<DbSet<TEntity>, Task<TReturn>> func)
        {
            try
            {
                await using var db = new TContext();
                var dbSet = db.Set<TEntity>();
                var data = await func(dbSet);

                return new Result<TReturn>(data);
            }
            catch (Exception exception)
            {
                return new Result<TReturn>(exception);
            }
        }

        /// <summary>
        /// Returns the first entity from the database given a properly built query.
        /// It is the underlying method used by Find methods.
        /// </summary>
        /// <param name="func">The built query to get the entities from the database.</param>
        /// <returns>
        /// Success : The result with the entity found is returned.
        /// NotFound : No entity was found given this query.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result<TEntity>> SelectFirst(Func<DbSet<TEntity>, Task<TEntity>> func)
        {
            try
            {
                await using var db = new TContext();
                var dbSet = db.Set<TEntity>();
                var data = await func(dbSet);
                if (data == null)
                    return Result<TEntity>.NotFound;

                return new Result<TEntity>(data);
            }
            catch (Exception exception)
            {
                return new Result<TEntity>(exception);
            }
        }

        #endregion Methods (Select Helpers)

        #region Methods (Query)

        /// <summary>
        /// Perform a performance-optimized query (filter, order, paging) on a table or better a dedicated view.
        /// </summary>
        /// <param name="dataQuery">The info(filter, order, paging) of the query.</param>
        /// <returns>
        /// Success : The paged data found is returned.
        /// BadPrerequisites : the context of the database must inherit from <see cref="DbContextEx"/> and the entity must not have a composed key.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<PaginatedDataDbEntity<TEntity>>> Query(DataQueryDbEntity<TContext, TEntity> dataQuery)
            => Result<PaginatedDataDbEntity<TEntity>>.SafeExecute(async () =>
            {
                var properties = typeof(TEntity).GetProperties().Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null).ToArray();
                var propertyMappings = properties.ToDictionary(p => p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name);
                var keys = properties.Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute))).Select(p => propertyMappings.GetValue(p.Name)).ToArray();
                if (keys.HasNoElements())
                    return Result<PaginatedDataDbEntity<TEntity>>.BadPrerequisites.WithReason("An entity key must be defined for the entity.");
                if (keys.Length > 1)
                    return Result<PaginatedDataDbEntity<TEntity>>.BadPrerequisites.WithReason($"The entity key must be a single key and not composite : {string.Join(", ", keys.Select(k => k.Name))}.");
                var key = keys.First();

                await using var db = new TContext();
                var whereQuery = CreateWhereQuery(db.Set<TEntity>(), dataQuery, propertyMappings);

                var orderQuery = whereQuery;
                var orderColumnName = propertyMappings.GetValue(dataQuery.OrderByPropertyName ?? "")?.Name ?? key.Name;
                if (orderColumnName.IsNotNullOrEmpty())
                    orderQuery = dataQuery.OrderByDescending
                        ? orderQuery.OrderByDescending(c => EF.Property<object>(c, dataQuery.OrderByPropertyName))
                        : orderQuery.OrderBy(c => EF.Property<object>(c, dataQuery.OrderByPropertyName));

                var paging = dataQuery.PagingSkip != null && dataQuery.PagingTake != null;
                var pagedQuery = orderQuery;
                if (paging)
                    pagedQuery = pagedQuery.Skip(dataQuery.PagingSkip.Value).Take(dataQuery.PagingTake.Value);

                long total;
                TEntity[] entities;
                if (paging)
                {
                    var result = await pagedQuery.Select(entity => new { Entity = entity, Total = whereQuery.Count() }).ToArrayAsync();
                    entities = result.Select(entity => entity.Entity).ToArray();
                    total = result.FirstOrDefault()?.Total ?? 0;
                }
                else
                {
                    entities = await pagedQuery.ToArrayAsync();
                    total = entities.Length;
                }

                var pagedResult = new PaginatedDataDbEntity<TEntity> { Data = entities, TotalItem = total, SkipItem = 3000, TakeItem = 100 };
                return new Result<PaginatedDataDbEntity<TEntity>>(pagedResult);
            });

        private IQueryable<TEntity> CreateWhereQuery(IQueryable<TEntity> query, DataQueryDbEntity<TContext, TEntity> dataQuery, Dictionary<string, PropertyInfo> propertyMappings)
        {
            dataQuery.Filters.ForEach(filter =>
            {
                var itemParameterExpression = Expression.Parameter(typeof(TEntity), "item");
                var expression = GetFilterExpression(filter, itemParameterExpression, propertyMappings);
                var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(expression, itemParameterExpression);
                query = query.Where(lambdaExpression);
            });
            return query;
        }

        private Expression GetFilterExpression(DataQueryFilterDbEntity filter, ParameterExpression itemParameterExpression, Dictionary<string, PropertyInfo> propertyMappings)
        {
            var propertyInfo = propertyMappings.GetValue(filter.PropertyName);
            if (propertyInfo == null)
                throw new Exception($"The property name {filter.PropertyName} of the data query filter does not exist on the entity");

            var leftExpression = Expression.Property(itemParameterExpression, propertyInfo);
            Expression<Func<object>> valueExpression = () => filter.Value;
            var rightExpression = Expression.Convert(Expression.Invoke(valueExpression), propertyInfo.PropertyType);
            Expression comparisonExpression = null;
            switch (filter.Comparison)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                case DataQueryFilterComparisonEnum.StartsWith: comparisonExpression = Expression.Call(leftExpression, typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) }), rightExpression); break;
                case DataQueryFilterComparisonEnum.Contains: comparisonExpression = Expression.Call(leftExpression, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) }), rightExpression); break;
                case DataQueryFilterComparisonEnum.EndsWith: comparisonExpression = Expression.Call(leftExpression, typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) }), rightExpression); break;
                // ReSharper restore AssignNullToNotNullAttribute
                case DataQueryFilterComparisonEnum.Equals: comparisonExpression = Expression.Equal(leftExpression, rightExpression); break;
                case DataQueryFilterComparisonEnum.NotEquals: comparisonExpression = Expression.NotEqual(leftExpression, rightExpression); break;
                case DataQueryFilterComparisonEnum.GreaterThan: comparisonExpression = Expression.GreaterThan(leftExpression, rightExpression); break;
                case DataQueryFilterComparisonEnum.GreaterOrEqualThan: comparisonExpression = Expression.GreaterThanOrEqual(leftExpression, rightExpression); break;
                case DataQueryFilterComparisonEnum.LessThan: comparisonExpression = Expression.LessThan(leftExpression, rightExpression); break;
                case DataQueryFilterComparisonEnum.LessOrEqualThan: comparisonExpression = Expression.LessThan(leftExpression, rightExpression); break;
            }
            if (comparisonExpression == null)
                throw new Exception($"The comparison {filter.Comparison} is not known");

            (filter.LinkedFilters ?? new List<DataQueryFilterDbEntity>()).ForEach(f =>
            {
                if (filter.Link.Is(DataQueryFilterLinkEnum.And))
                    comparisonExpression = Expression.AndAlso(comparisonExpression, GetFilterExpression(f, itemParameterExpression, propertyMappings));
                else if (filter.Link.Is(DataQueryFilterLinkEnum.Or))
                    comparisonExpression = Expression.OrElse(comparisonExpression, GetFilterExpression(f, itemParameterExpression, propertyMappings));
            });

            return comparisonExpression;
        }

        #endregion Methods (Query)

        #region Methods (Create)

        /// <summary>
        /// Creates a new entity in the database.
        /// Some generated properties of the entity may be updated by the database (ie. Identity column, default constraints, triggers, ...).
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>
        /// Success : The entity has been created successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> Create(TEntity entity)
            => Manipulate((dbSet, notNullEntities) => dbSet.Add(entity), entity);

        /// <summary>
        /// Creates some new entities in the database in a single transaction.
        /// Some generated properties of the entity may be updated by the database (ie. Identity column, default constraints, triggers, ...).
        /// </summary>
        /// <param name="entities">The entities to create.</param>
        /// <returns>
        /// Success : The entities has been created successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> Create(params TEntity[] entities)
            => Manipulate((dbSet, notNullEntities) => dbSet.AddRange(notNullEntities), entities);

        #endregion Methods (Create)

        #region Methods (Update)

        /// <summary>
        /// Updates an existing entity in the database.
        /// The entity is found given its primary key value(s).
        /// Some generated properties of the entity may be updated by the database (ie. Identity column, default constraints, triggers, ...).
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>
        /// Success : The entity has been updated successfully.
        /// NotFound : The entity has not been found given its primary key value(s).
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> Update(TEntity entity)
            => Manipulate((dbSet, notNullEntities) => dbSet.Update(entity), entity);

        /// <summary>
        /// Updates some existing entities in the database in a single transaction.
        /// The entity is found given its primary key value(s).
        /// Some generated properties of the entity may be updated by the database (ie. Identity column, default constraints, triggers, ...).
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>
        /// Success : The entity has been updated successfully.
        /// NotFound : At least one entity has not been found given its primary key value(s).
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> Update(params TEntity[] entities)
            => Manipulate((dbSet, notNullEntities) => dbSet.UpdateRange(notNullEntities), entities);

        /// <summary>
        /// Updates only some properties of an existing entity in the database.
        /// The entity is found given its primary key value(s).
        /// Some generated properties of the entity may be updated by the database (ie. Identity column, default constraints, triggers, ...).
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="properties">The only properties to update. The other properties are ignored.</param>
        /// <returns>
        /// Success : The entity has been updated successfully.
        /// NotFound : The entity has not been found given its primary key value(s).
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> UpdatePartial(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
            => Manipulate((dbSet, notNullEntities) =>
            {
                var db = dbSet.GetService<ICurrentDbContext>().Context;
                var entry = db.Entry(entity);
                dbSet.Attach(entity);
                properties.IfNotNull().ForEach(f => entry.Property(f).IsModified = true);
            }, entity);

        /// <summary>
        /// Updates only some properties of an existing entity in the database.
        /// The entity is found given its primary key value(s).
        /// Some generated properties of the entity may be updated by the database (ie. Identity column, default constraints, triggers, ...).
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="properties">The only properties to update. The other properties are ignored.</param>
        /// <returns>
        /// Success : The entity has been updated successfully.
        /// NotFound : The entity has not been found given its primary key value(s).
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> UpdatePartial(TEntity entity, params string[] properties)
            => Manipulate((dbSet, notNullEntities) =>
            {
                var db = dbSet.GetService<ICurrentDbContext>().Context;
                var entry = db.Entry(entity);
                dbSet.Attach(entity);
                properties.IfNotNull().ForEach(n => entry.Property(n).IsModified = true);
            }, entity);

        /// <summary>
        /// Updates only some properties of existing entities in the database in a single transaction.
        /// The entity is found given its primary key value(s).
        /// Some generated properties of the entity may be updated by the database (ie. Identity column, default constraints, triggers, ...).
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <param name="properties">The only properties to update. The other properties are ignored.</param>
        /// <returns>
        /// Success : The entities has been updated successfully.
        /// NotFound : At least one entity has not been found given its primary key value(s).
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> UpdatePartial(TEntity[] entities, params Expression<Func<TEntity, object>>[] properties)
            => Manipulate((dbSet, notNullEntities) =>
            {
                var db = dbSet.GetService<ICurrentDbContext>().Context;
                notNullEntities.ForEach(entity =>
                {
                    var entry = db.Entry(entity);
                    dbSet.Attach(entity);
                    properties.IfNotNull().ForEach(f => entry.Property(f).IsModified = true);
                });

            }, entities);

        /// <summary>
        /// Updates only some properties of existing entities in the database in a single transaction.
        /// The entity is found given its primary key value(s).
        /// Some generated properties of the entity may be updated by the database (ie. Identity column, default constraints, triggers, ...).
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <param name="properties">The only properties to update. The other properties are ignored.</param>
        /// <returns>
        /// Success : The entities has been updated successfully.
        /// NotFound : At least one entity has not been found given its primary key value(s).
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> UpdatePartial(TEntity[] entities, params string[] properties)
            => Manipulate((dbSet, notNullEntities) =>
            {
                var db = dbSet.GetService<ICurrentDbContext>().Context;
                notNullEntities.ForEach(entity =>
                {
                    var entry = db.Entry(entity);
                    dbSet.Attach(entity);
                    properties.IfNotNull().ForEach(n => entry.Property(n).IsModified = true);
                });

            }, entities);

        #endregion Methods (Update)

        #region Methods (Remove)

        /// <summary>
        /// Removes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>
        /// Success : The entity has been removed successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> Remove(TEntity entity)
            => Manipulate((dbSet, notNullEntities) => dbSet.Remove(entity), entity);

        /// <summary>
        /// Removes some entities from the database into a single transaction.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        /// <returns>
        /// Success : The entities has been removed successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> Remove(params TEntity[] entities)
            => Manipulate((dbSet, notNullEntities) => dbSet.RemoveRange(notNullEntities), entities);

        /// <summary>
        /// Removes some entities from the database given a predicate into a single transaction.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the entities to remove.</param>
        /// <returns>
        /// Success : The entities has been removed successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> RemoveWhere(Expression<Func<TEntity, bool>> predicate)
            => Manipulate((dbSet, notNullEntities) => dbSet.RemoveRange(dbSet.Where(predicate)), new TEntity[1]);

        #endregion Methods (Remove)

        #region Methods (CreateOrUpdate)

        /// <summary>
        /// Creates a new entity in the database if it does not exist or updates it otherwise.
        /// It uses a super fast UPSERT (or MERGE) query based on the primary key but does not update the generated property of the entity.
        /// Mainly used for performance improvement.
        /// </summary>
        /// <param name="entity">The entity to create or update.</param>
        /// <returns>
        /// Success : The entity has been created or updated successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> CreateOrUpdate(TEntity entity)
            => Manipulate((db, notNullEntities) => db.Set<TEntity>().Upsert(entity).RunAsync(), entity);

        /// <summary>
        /// Creates some entities in the database or updating them depending if they already exist or not.
        /// It uses a super fast UPSERT (or MERGE) query based on the primary key but does not update the generated property of the entity.
        /// Mainly used for performance improvement.
        /// </summary>
        /// <param name="entities">The entities to create or update.</param>
        /// <returns>
        /// Success : The entities has been created or updated successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> CreateOrUpdateRange(params TEntity[] entities)
            => Manipulate((db, notNullEntities) => db.Set<TEntity>().UpsertRange(notNullEntities).RunAsync(), entities);

        #endregion Methods (CreateOrUpdate)

        #region Methods (Manipulation Helpers)

        /// <summary>
        /// Manipulates (CUD operation) on a set of entities.
        /// </summary>
        /// <param name="action">The action used to manipulate a set of entities.</param>
        /// <param name="entities">The entities to manipulate in the way given by the action.</param>
        /// <returns>
        /// Success : The entities has been manipulated successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual Task<Result> Manipulate(Action<DbSet<TEntity>, TEntity[]> action, params TEntity[] entities)
            => Manipulate(async (db, notNullEntities) =>
            {
                var dbEntity = db.Set<TEntity>();
                action(dbEntity, entities);
                await db.SaveChangesAsync();
            }, entities);

        /// <summary>
        /// Manipulates (CUD operation) on a set of entities.
        /// </summary>
        /// <param name="func">The function used to manipulate a set of entities.</param>
        /// <param name="entities">The entities to manipulate in the way given by the function.</param>
        /// <returns>
        /// Success : The entities has been manipulated successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result> Manipulate(Func<DbContextEx, TEntity[], Task> func, params TEntity[] entities)
        {
            try
            {
                entities = entities?.IfNotNull().ToArray() ?? new TEntity[0];
                if (entities.HasNoElements())
                    return new Result(ResultStatus.BadParameters).WithReason("The entities must be filled with at least one entity");

                await using var db = new TContext();
                await func(db, entities);

                return Result.Success;
            }
            catch (AggregateException exception)
            {
                return new Result<TEntity>(exception.InnerException is DbUpdateConcurrencyException ? ResultStatus.NotFound : ResultStatus.Unexpected, exception);
            }
            catch (Exception exception)
            {
                if (exception.InnerException?.GetType().FullName == "System.Data.SqlClient.SqlException" && exception.InnerException.Data.Contains("HelpLink.EvtID"))
                {
                    var errorCode = (exception.InnerException.Data["HelpLink.EvtID"] as string)?.ToInt32(-1);
                    switch (errorCode)
                    {
                        case 2601:
                        case 2627: return Result.Already.WithException(exception);
                    }
                }

                return new Result(exception);
            }
        }

        #endregion Methods (Manipulation Helpers)

        #region Methods (Synchronize)

        /// <summary>
        /// Synchronize a full entities database subset with a given new subset (Insert/Update/Delete).
        /// /!\ Only works with natural key /!\
        /// </summary>
        /// <param name="predicate">The predicate to get only a subset of entities from database.</param>
        /// <param name="newEntities">The new entities to synchronize.</param>
        /// <param name="keyExpression">The key get expression function to compare entities between them to define the action Insert/Update/Delete.</param>
        /// <param name="entityPropertiesNamesToUpdate">The names of the entity properties to check between old and new entities to know whether an update is needed. Optional but great for performance.</param>
        /// <returns>
        /// Success: The entities has been deleted.
        /// BadParameters: The entities is null.
        /// Unexpected: Unexpected failure.
        /// </returns>
        public virtual async Task<Result> Synchronize(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<TEntity> newEntities,
            Expression<Func<TEntity, object>> keyExpression,
            string[] entityPropertiesNamesToUpdate = null)
        {
            try
            {
                if (newEntities == null)
                    return new Result(ResultStatus.BadParameters);

                // Gets the old database subset to compare with the new
                await using var db = new TContext();
                var source = db.Set<TEntity>();
                var query = (IQueryable<TEntity>)source;
                if (predicate != null)
                    query = query.Where(predicate);

                // Converts the old entities fetched and new entities in a dictionary for faster comparison check
                var getFunc = keyExpression.Compile();
                var oldEntitiesDictionary = await query.ToDictionaryAsync(e => getFunc(e));
                var newEntitiesDictionary = newEntities.ToDictionary(e => getFunc(e));

                // First applies the INSERT/DELETE to the source dataset depending on the new entities
                await source.AddRangeAsync(newEntitiesDictionary.Keys.Where(k => !oldEntitiesDictionary.ContainsKey(k)).Select(k => newEntitiesDictionary[k]));
                source.RemoveRange(oldEntitiesDictionary.Keys.Where(k => !newEntitiesDictionary.ContainsKey(k)).Select(k => oldEntitiesDictionary[k]));

                // Then the UPDATE which could be unconditional if no valuesToUpdate are specified or conditional and updates only if changes
                entityPropertiesNamesToUpdate ??= db.Entry(new TEntity())
                    .Properties.Where(p => p.Metadata.ValueGenerated == ValueGenerated.Never)
                    .Select(p => p.Metadata.Name).ToArray();

                newEntitiesDictionary.Keys.Where(oldEntitiesDictionary.ContainsKey).ForEach(key =>
                {
                    var newEntity = newEntitiesDictionary[key];
                    var oldEntity = oldEntitiesDictionary[key];
                    entityPropertiesNamesToUpdate.ForEach(propertyName =>
                        oldEntity.SetPropertyValue(propertyName, newEntity.GetPropertyValue<object>(propertyName)));
                });

                await db.SaveChangesAsync();
                return Result.Success;
            }
            catch (Exception exception)
            {
                return new Result(exception);
            }
        }

        #endregion Methods (Synchronize)

        #region Methods (ExecuteFunction)

        /// <summary>
        /// Executes a raw SQL function which returns some entities.
        /// </summary>
        /// <param name="sqlFunctionName">The SQL function name to call with its schema (ie dbo.Int).</param>
        /// <param name="parameters">The parameters to give to the function (only primitive types).</param>
        /// <returns>
        /// Success : The entities returned from the function.
        /// BadParameters : The parameters given are not valid.
        /// Unexpected: An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> ExecuteFunctionWithEntitiesReturn(string sqlFunctionName, DbParameterCollection parameters)
            => ExecuteSqlCommand(sqlFunctionName != null ? $"SELECT * FROM {sqlFunctionName} ({parameters?.GetSqlParameterNamesList()})" : null, parameters);

        #endregion Methods (ExecuteFunction)

        #region Methods (ExecuteStoredProcedure)

        /// <summary>
        /// Executes a stored procedure which returns some entities.
        /// </summary>
        /// <param name="sqlProcedureName">The SQL stored procedure name to call with its schema (ie dbo.TestStoredProcedure).</param>
        /// <param name="parameters">The parameters to give to the stored procedure (only primitive types).</param>
        /// <returns>
        /// Success : The stored procedure execution has succeeded and the entities are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TEntity[]>> ExecuteStoredProcedureWithEntitiesReturn(string sqlProcedureName, DbParameterCollection parameters)
            => ExecuteSqlCommand(sqlProcedureName != null ? $"EXECUTE {sqlProcedureName} {parameters.GetSqlParameterNamesList()}" : null, parameters);

        #endregion Methods (ExecuteStoredProcedure)

        #region Methods (ExecuteCommand)

        /// <summary>
        /// Executes a raw SQL command that returns some entities.
        /// It is the underlying method of many function and stored procedure execution.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <param name="parameters">The parameters to give to the function (only primitive types).</param>
        /// <returns>
        /// Success : The data returned from the function.
        /// BadParameters : The parameters given are not valid.
        /// Unexpected: An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result<TEntity[]>> ExecuteSqlCommand(string sqlQuery, DbParameterCollection parameters)
        {
            try
            {
                if (sqlQuery.IsNullOrEmpty())
                    return Result<TEntity[]>.BadParameters.WithReason("The execution object name must be provided");
                if (!parameters?.Validate() ?? true)
                    return Result<TEntity[]>.BadParameters.WithReason("The parameters must be provided, even an empty list");

                await using var db = new TContext();
                await using var connection = db.Database.GetDbConnection();
                await using var command = connection.CreateCommand();

                command.CommandText = sqlQuery;
                parameters.FillCommandWithParameters(command);

                await connection.OpenAsync();
                var entities = await db.Set<TEntity>().FromSqlRaw(command.CommandText, parameters).ToArrayAsync();
                return new Result<TEntity[]>(entities);
            }
            catch (Exception exception)
            {
                return new Result<TEntity[]>(exception);
            }
        }

        #endregion Methods (ExecuteCommand)
    }
}