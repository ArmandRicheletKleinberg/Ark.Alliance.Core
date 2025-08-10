using System.Collections.Generic;
using System.Linq;
using Ark.Net.Models;

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Describes paging, ordering and filtering options for database queries.
    /// <para>+ Centralizes query logic for <see cref="DbServices{TContext, TEntity}"/> consumers.</para>
    /// <para>- Exposes entity property names which may leak internal structure.</para>
    /// Ref: <see href="https://learn.microsoft.com/ef/core/querying/"/>
    /// </summary>
    public class DataQueryDbEntity<TContext, TEntity>
        where TEntity : DbEntity<TContext>
        where TContext : DbContextEx, new()
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="DataQueryDbEntity{TContext, TEntity}"/> instance.
        /// </summary>
        public DataQueryDbEntity()
        { }

        /// <summary>
        /// Creates a new <see cref="DataQueryDbEntity{TContext, TEntity}"/> instance given a DTO.
        /// </summary>
        /// <param name="query">The DTO containing the query parameters.</param>
        /// <param name="dtoToEntityNamesMapping">The dictionary of the corresponding DTO/entity names if different.</param>
        public DataQueryDbEntity(DataQueryDto query, IDictionary<string, string> dtoToEntityNamesMapping = null)
        {
            string GetEntityName(string dtoName) => dtoToEntityNamesMapping?.GetValue(dtoName) ?? dtoName;

            PagingSkip = query.PagingSkip;
            PagingTake = query.PagingTake;
            OrderByPropertyName = GetEntityName(query.OrderByPropertyName);
            OrderByDescending = query.OrderByDescending;
            var propertiesByName = typeof(TEntity).GetProperties().ToDictionary(p => p.Name);
            Filters = query.Filters?.Select(f => new DataQueryFilterDbEntity(f, dtoToEntityNamesMapping, propertiesByName)).ToArray();
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// Number of rows to skip before returning results.
        /// <para>+ Enables page navigation.</para>
        /// <para>- Large values can degrade query performance.</para>
        /// </summary>
        public int? PagingSkip { get; set; }

        /// <summary>
        /// Maximum number of rows to return.
        /// <para>+ Limits payload size for large datasets.</para>
        /// <para>- Too small values may require additional requests.</para>
        /// </summary>
        public int? PagingTake { get; set; }

        /// <summary>
        /// Name of the property used for ordering.
        /// <para>+ Supports ascending and descending sorting.</para>
        /// <para>- Typographical errors throw runtime exceptions.</para>
        /// </summary>
        public string OrderByPropertyName { get; set; }

        /// <summary>
        /// Indicates whether ordering should be descending.
        /// <para>+ Mirrors SQL <c>ORDER BY ... DESC</c>.</para>
        /// <para>- Ignored when <see cref="OrderByPropertyName"/> is not specified.</para>
        /// </summary>
        public bool OrderByDescending { get; set; }

        /// <summary>
        /// Filters applied to the query.
        /// <para>+ Composes advanced predicates through <see cref="DataQueryFilterDbEntity.LinkedFilters"/>.</para>
        /// <para>- Complex trees may be expensive to translate to SQL.</para>
        /// </summary>
        public DataQueryFilterDbEntity[] Filters { get; set; }

        #endregion Properties (Public)
    }
}