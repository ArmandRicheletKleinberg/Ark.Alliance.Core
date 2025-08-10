using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Extension utilities for <see cref="DbContext"/> to simplify generic-less operations.
    /// + Centralizes reflection-based access to <see cref="DbContext.Set{TEntity}()"/>.
    /// - Misuse can bypass compile-time safety and validation.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/dbcontext-configuration/"/>
    /// </summary>
    public static class DbContextExtensions
    {


        #region Methods (Public)

        /// <summary>
        /// Creates an <see cref="IQueryable"/> backed by the set for a runtime-provided entity type.
        /// + Enables scenarios where the entity type is resolved dynamically.
        /// - Uses reflection which incurs additional overhead.
        /// </summary>
        /// <param name="context">Database context containing the set.</param>
        /// <param name="typeEntity">CLR type of the entity to query.</param>
        /// <returns>
        /// Queryable exposing data for <paramref name="typeEntity"/>. The sequence is serialized as JSON when enumerated.
        /// </returns>
        public static IQueryable Set(this DbContext context, Type typeEntity)
            => typeof(DbContext)
                .GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance)
                ?.MakeGenericMethod(typeEntity)
                .Invoke(context, null) as IQueryable;

        /// <summary>
        /// Retrieves all entities whose primary keys match the supplied values.
        /// + Uses
        /// <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{System.Func{TSource, bool}})"/>
        /// to minimize data transfer.
        /// - Returns an empty array when no entities match the keys.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/querying/"/>
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to retrieve.</typeparam>
        /// <param name="dbContext">Context used to execute the query.</param>
        /// <param name="keyValues">Primary key values to search for.</param>
        /// <returns>
        /// Result wrapping the matching entities serialized to JSON, e.g.:
        /// <code language="json">
        /// [
        ///   { "Id": 1 },
        ///   { "Id": 2 }
        /// ]
        /// </code>
        /// </returns>
        public static Task<Result<TEntity[]>> FindAllAsync<TEntity>(this DbContext dbContext, params object[] keyValues)
            where TEntity : class
            => Result<TEntity[]>.SafeExecute(async () =>
            {
                var items = await dbContext.Set<TEntity>().WhereContainsPrimaryKeys(keyValues).ToArrayAsync();
                return new Result<TEntity[]>(items);
            });

        #endregion Methods (Public)
    }
}
