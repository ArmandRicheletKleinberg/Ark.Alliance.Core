using System;
using System.Linq;
using System.Threading.Tasks;
using Ark.Data.EFCore;
using Ark.Net.Models;

// ReSharper disable UnusedMember.Global

namespace Ark.AspNetCore.Search
{
    /// <summary>
    /// This class has the information about a specific search item type.
    /// Some common search item type creator are available for common resource search (UM, order, ...).
    /// </summary>
    public abstract class SearchItemType
    {
        #region Properties (Abstract)

        /// <summary>
        /// The code that uniquely identifies a search item type for all applications.
        /// This code is always prefixed by the application name or by COMMON_ if common type.
        /// A list of common type is defined in <see cref="SearchItemTypeEnum"/>.
        /// ie SearchItemTypeEnum.CommonOrderNumberWithItem.Code.
        /// </summary>
        public abstract string Code { get; }

        /// <summary>
        /// The function used to get the search item type in the current culture.
        /// In most cases, should be () => Texts.MySearchTextResourceKey.
        /// </summary>
        public abstract Func<string> GetLabelFunc { get; }

        /// <summary>
        /// The front URL pattern suffix to navigate to.
        /// The pattern can specified a {id} or {value} format to be filled by the value.
        /// </summary>
        /// <example>
        /// material/{0} with value G491158754 for navigating to http://myapplicationurl/material/G491158754.
        /// </example>
        public abstract string FrontUrlSuffixPattern { get; }

        /// <summary>
        /// The description that describes the best the page/behavior where to navigate in the current culture.
        /// The pattern can specified a {id} or {value} format to be filled by the value.
        /// In most cases, should be () => Texts.MySearchTextResourceKey.
        /// </summary>
        public abstract Func<string> GetFrontUrlDescriptionFunc { get; }

        #endregion Properties (Abstract)

        #region Methods (Internal Abstract)

        /// <summary>
        /// Gets the EF core query to execute for this search item type.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <param name="text">The text to search.</param>
        /// <param name="take">The maximum number of element to take.</param>
        /// <returns>The .NET EF Core query.</returns>
        internal abstract IQueryable<SearchItem> GetEfCoreQuery(DbContextEx db, string text, int take);

        /// <summary>
        /// Fills the search item with extra data such as date and time and text summary.
        /// </summary>
        /// <param name="items">The items to fill with extra data.</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        internal abstract Task FillItemsWithExtraData(SearchItemDbEntity[] items);

        /// <summary>
        /// Validates the search item type by checking all the needed info of the type :
        /// - The code must not be empty and should be prefixed by COMMON_ or the application name (ie AVIS_);
        /// - The label get function must not be null and returns a not empty text;
        /// - The front URL pattern suffix must be specified and the key between {} must be either id or value only;
        /// - The front URL description get function mut not be null and returns a not empty text;
        /// - The query entity type must be set;
        /// - The query EF Core function must not be null;
        /// </summary>
        /// <returns>All the validation error if any or null if validation OK.</returns>
        internal abstract string Validate();

        #endregion Methods (Internal Abstract)
    }
}