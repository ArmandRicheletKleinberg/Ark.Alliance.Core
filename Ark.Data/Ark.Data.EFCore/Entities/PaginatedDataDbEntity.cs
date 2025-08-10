using System;
using System.Linq;
using Ark.Net.Models;

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Wraps paginated query results with related metadata.
    /// <para>+ Couples page data with total count for API responses.</para>
    /// <para>- Allocates arrays which may impact memory on large result sets.</para>
    /// Ref: <see href="https://learn.microsoft.com/ef/core/querying/"/>
    /// </summary>
    /// <typeparam name="TDbEntity">Type of the data items returned.</typeparam>
    public class PaginatedDataDbEntity<TDbEntity>
    {
        #region Properties (Public)

        /// <summary>
        /// Total number of items available for the query.
        /// <para>+ Enables clients to compute page counts.</para>
        /// <para>- Requires an extra COUNT query which can be expensive.</para>
        /// </summary>
        public long TotalItem { get; set; }

        /// <summary>
        /// Number of items to skip before returning results.
        /// <para>+ Supports standard pagination mechanics.</para>
        /// <para>- Large skips may degrade performance on some providers.</para>
        /// </summary>
        public long SkipItem { get; set; }

        /// <summary>
        /// Maximum number of items to return from the skip position.
        /// <para>+ Limits payload size for bandwidth efficiency.</para>
        /// <para>- Too small values may require multiple round-trips.</para>
        /// </summary>
        public long TakeItem { get; set; }

        /// <summary>
        /// Page data returned by the query.
        /// <para>+ Provides direct access to serialized entities.</para>
        /// <para>- Null when no data matches the query.</para>
        /// </summary>
        public TDbEntity[] Data { get; set; }

        #endregion Properties (Public)

        #region Methods (Public)

        /// <summary>
        /// Converts this paginated data database entity into a DTO entity.
        /// </summary>
        /// <typeparam name="TDto">The type of the DTO data to return.</typeparam>
        /// <param name="convertFunc">The method used to convert a data entity to a DTO.</param>
        /// <returns>The converted paginated data DTO.</returns>
        public PaginatedDataDto<TDto> ToDto<TDto>(Func<TDbEntity, TDto> convertFunc)
            => new PaginatedDataDto<TDto>
            {
                TotalItem = (int)TotalItem,
                SkipItem = (int)SkipItem,
                TakeItem = (int)TakeItem,
                Data = Data?.Select(convertFunc).ToArray()
            };

        #endregion Methods (Public)
    }
}