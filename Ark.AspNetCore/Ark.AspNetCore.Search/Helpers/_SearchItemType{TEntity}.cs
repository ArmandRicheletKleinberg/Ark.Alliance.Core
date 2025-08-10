using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ark.Data.EFCore;
using Ark.Net.CrossCutting;
using Ark.Net.Models;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedMember.Global

namespace Ark.AspNetCore.Search
{
    /// <summary>
    /// This class has the information about a specific search item type.
    /// The item type instance  must be created using a static method.
    /// Some common search item type creator are available for common resource search (UM, order, ...).
    /// </summary>
    public abstract class SearchItemType<TEntity> : SearchItemType
        where TEntity : DbEntity
    {
        #region Properties (Abstract)

        /// <summary>
        /// The function used to filter the entities to search.
        /// This generally will be text => entity => entity.Property.StartsWith(text).
        /// </summary>
        public abstract Func<string, Expression<Func<TEntity, bool>>> WhereClauseFunc { get; }

        /// <summary>
        /// The function used to get the identifier/value of the entity to search.
        /// This normally is generally the primary key with another value ie entity => new KeyValuePair{string, string}(entity.Id, entity.Numero).
        /// </summary>
        public abstract Expression<Func<TEntity, SearchItem>> SelectItemFunc { get; }

        /// <summary>
        /// This expression is used to select all the data needed to extract the extra information about the item to search.
        /// This is used to restrain the data set fetch to enhance performance.
        /// </summary>
        public abstract Expression<Func<TEntity, TEntity>> SelectExtraDataFunc { get; }

        /// <summary>
        /// This function is used to get the identifier of the searched entity used to match with the searched item.
        /// This is extracted from the extra data that have been selected previously in the <see cref="SelectExtraDataFunc"/> method.
        /// </summary>
        public abstract Func<TEntity, object> GetExtraIdFunc { get; }

        /// <summary>
        /// This function is used to get the datetime of the searched entity.
        /// This is extracted from the extra data that have been selected previously in the <see cref="SelectExtraDataFunc"/> method.
        /// </summary>
        public abstract Func<TEntity, DateTime?> GetExtraDateTimeFunc { get; }

        /// <summary>
        /// This function is used to get the summary text about the searched entity.
        /// This is extracted from the extra data that have been selected previously in the <see cref="SelectExtraDataFunc"/> method.
        /// </summary>
        public abstract Func<TEntity, string> GetExtraSummaryFunc { get; }

        #endregion Properties (Abstract)

        #region Methods (Internal Abstract Implementation)

        /// <inheritdoc />
        internal override IQueryable<SearchItem> GetEfCoreQuery(DbContextEx db, string text, int take)
        {
            var query = db.Set<TEntity>() as IQueryable<TEntity>;
            return query.Where(WhereClauseFunc(text)).Select(SelectItemFunc);
        }

        /// <inheritdoc />
        internal override async Task FillItemsWithExtraData(SearchItemDbEntity[] items)
        {
            await using var db = SearchController.CreateDbContextFunc();
            var query = db.Set<TEntity>() as IQueryable<TEntity>;
            var primaryKeyType = db.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties.First().ClrType;
            var keys = items.Select(item => primaryKeyType switch
                {
                    _ when primaryKeyType == typeof(int) => item.Id.ToInt32(),
                    _ when primaryKeyType == typeof(long) => item.Id.ToInt64(),
                    _ when primaryKeyType == typeof(Guid) => Guid.Parse(item.Id),
                    _ => (object)item.Id
                }).ToArray();
            var entities = await query.WhereContainsPrimaryKeys(keys).Select(SelectExtraDataFunc).ToArrayAsync();
            var entitiesById = entities.ToDictionary(entity => GetExtraIdFunc(entity));

            foreach (var item in items)
            {
                var entity = entitiesById.GetValue(item.Id);
                if (entity == null)
                    continue;

                item.DateAndTime = GetExtraDateTimeFunc(entity);
                item.SummaryText = GetExtraSummaryFunc(entity);
            }
        }

        /// <inheritdoc />
        internal override string Validate()
        {
            var errorBuilder = new StringBuilder();

            if (Code == null || !Code.StartsWith("COMMON_") && !Code.StartsWith($"{CrossCuttingServices.ApplicationId.ToUpper()}_"))
                errorBuilder.AppendLine($"The code must not be empty and should be prefixed by COMMON_ or {CrossCuttingServices.ApplicationId.ToUpper()}_");

            if (GetLabelFunc?.Invoke().IsNullOrEmpty() ?? true)
                errorBuilder.AppendLine("The label get function must not be null and returns a not empty text");

            if (FrontUrlSuffixPattern != null && FrontUrlSuffixPattern.IsNullOrWhiteSpace() || FrontUrlSuffixPattern.GetAllSectionContent("{", "}").Any(c => c != "id" && c != "value"))
                errorBuilder.AppendLine("The front URL pattern suffix must be specified and the key between {} must be only either id or value");

            if (FrontUrlSuffixPattern != null && (GetFrontUrlDescriptionFunc?.Invoke().IsNullOrEmpty() ?? true))
                errorBuilder.AppendLine("The front URL description get function must not be null and returns a not empty text");

            if (WhereClauseFunc == null)
                errorBuilder.AppendLine("The query EF Core where clause function must not be null");

            if (errorBuilder.Length > 0)
            {
                errorBuilder.Insert(0, $"Validation error for the search item type {Code}");
                return errorBuilder.ToString();
            }

            return null;
        }

        #endregion Methods (Internal)
    }
}