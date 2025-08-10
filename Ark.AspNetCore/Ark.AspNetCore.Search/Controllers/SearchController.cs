using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ark.Data.EFCore;
using Ark.Net.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace Ark.AspNetCore.Search
{
    /// <inheritdoc />
    /// <summary>
    /// This is the base class used to make a global search on an application.
    /// This class should be overriden in another project controller to allow access to the search functionality.
    /// </summary>
    [Authorize]
    [ApiExplorerSettings(GroupName = "ο Search")]
    [EnableCors("SearchPolicy")]
    public class SearchController : ControllerBase
    {
        #region Static

        /// <summary>
        /// The different types ordered by code.
        /// Setup by the <see cref="IWebHostBuilderExtensions.UseSearch{TContext}"/> method.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        internal static Dictionary<string, SearchItemType> TypesByCode;

        /// <summary>
        /// The function used to create a database context.
        /// Setup by the <see cref="IWebHostBuilderExtensions.UseSearch{TContext}"/> method.
        /// </summary>
        internal static Func<DbContextEx> CreateDbContextFunc;

        #endregion Static

        #region Fields

        /// <summary>
        /// The search item database repository is needed.
        /// </summary>
        internal SearchItemDbServices SearchItemDbServices = new SearchItemDbServices();

        #endregion Fields

        #region Methods (Search)

        /// <summary>
        /// Searches globally the application for a text with a possible filter of item type code.
        /// </summary>
        /// <param name="text">The text to search by, for performance purpose, the searched item should start by this text.</param>
        /// <param name="take">The number of search items to return. Default to 20.</param>
        /// <param name="typeCodes">Optional filter by search item type codes.</param>
        /// <param name="globalSearch">Whether this is a global search (needed URL to navigate) otherwise it is a simple quick item search (stripped from URL information and label). Default to false.</param>
        /// <remarks>
        /// ## Description ##
        /// This service is used by some application to have a global search functionality.
        /// ## Example ##
        /// ```
        /// GET search?text=G4911&amp;takes=100&amp;codes=COMMON_UM
        /// ```
        /// Searches the application for UM beginning with G4911 and returns the first 100 matches.
        /// </remarks>
        /// <response code="200">
        /// Success : The search items matching are returned.
        /// BadParameters : A text must be given.
        /// BadPrerequisites : The types defined in the local SearchController implementation are not valid.
        /// Unauthorized : The user is not allowed to access the app.
        /// Unexpected : An unexpected error occurs.
        /// </response>
        /// <response code="401">The user is not authenticated.</response>
        /// <returns></returns>
        [HttpGet("search")]
        public Task<ResultDto<SearchItemDto[]>> Search([FromQuery] string text, [FromQuery] int take = 20, [FromQuery] string[] typeCodes = null, [FromQuery] bool globalSearch = false)
            => ExecuteBlAsync(() => Result<SearchItemDto[]>.SafeExecute(async () =>
            {
                if (text.IsNullOrEmpty())
                    return Result<SearchItemDto[]>.BadParameters.WithReason("A text must be given.");

                var searchResult = await SearchItemDbServices.GetSearchItems(TypesByCode, text, take, typeCodes);
                if (searchResult.IsNotSuccess)
                    return new Result<SearchItemDto[]>(searchResult).AddReason("Error while searching the item in the database.");
                var dbItems = searchResult.Data;

                if (globalSearch)
                {
                    var fillResult = await SearchItemDbServices.FillItemsWithExtraData(TypesByCode, dbItems);
                    if (fillResult.IsNotSuccess)
                        return new Result<SearchItemDto[]>(searchResult).AddReason("Error while filling the extra data of the item.");
                }

                var items = dbItems.Select(item =>
                {
                    var type = TypesByCode.GetValue(item.Type);
                    return MapSearchItem(item, type, globalSearch);
                }).ToArray();
                return new Result<SearchItemDto[]>(items);
            }));

        /// <summary>
        /// Converts a search database entity to a DTO.
        /// </summary>
        /// <param name="item">The entity to convert.</param>
        /// <param name="type">The information of the item type.</param>
        /// <param name="globalSearch">Whether this is a global search (needed URL to navigate) otherwise it is a simple quick item search (stripped from URL information and label).</param>
        /// <returns>The converted DTO.</returns>
        private static SearchItemDto MapSearchItem(SearchItemDbEntity item, SearchItemType type, bool globalSearch)
            => new SearchItemDto
            {
                Id = item.Id,
                Value = item.Value,
                Application = globalSearch ? Assembly.GetEntryAssembly()?.GetProductNameSafe() : null,
                TypeCode = type.Code,
                TypeLabel = type.GetLabelFunc(),
                FrontUrl = type.FrontUrlSuffixPattern.ReplaceSections("{", "}", section => section == "id" ? item.Id : item.Value),
                FrontUrlDescription = globalSearch
                    ? type.GetFrontUrlDescriptionFunc().ReplaceSections("{", "}", section => section == "id" ? item.Id : item.Value)
                    : null,
                LastUpdatedTime = item.DateAndTime,
                ExtraDataSummary = item.SummaryText
            };

        #endregion Methods (Search)

        #region Methods (GetTypes)

        /// <summary>
        /// Gets all the item that can be search by the application with their identification code and label.
        /// </summary>
        /// <remarks>
        /// ## Description ##
        /// This service is used by some application to have a global search functionality.
        /// ## Example ##
        /// ```
        /// GET search/types
        /// ```
        /// Gets all the item that can be search by the application with their identification code and label.
        /// </remarks>
        /// <response code="200">
        /// Success : The search item types are returned.
        /// BadPrerequisites : The types defined in the local SearchController implementation are not valid.
        /// Unauthorized : The user is not allowed to access the app.
        /// Unexpected : An unexpected error occurs.
        /// </response>
        /// <response code="401">The user is not authenticated.</response>
        /// <returns></returns>
        [HttpGet("search/types")]
        public ResultDto<SearchItemTypeDto[]> GetTypes()
            => ExecuteBl(() => Result<SearchItemTypeDto[]>.SafeExecute(() =>
            {
                var types = TypesByCode.Values.Select(MapSearchItemType).ToArray();
                return new Result<SearchItemTypeDto[]>(types);
            }));

        /// <summary>
        /// Converts a search item type to a DTO.
        /// </summary>
        /// <param name="type">The type to convert into a DTO.</param>
        /// <returns>The DTO with the search item type data.</returns>
        private static SearchItemTypeDto MapSearchItemType(SearchItemType type)
            => new SearchItemTypeDto
            {
                Code = type.Code,
                Label = type.GetLabelFunc()
            };

        #endregion Methods (GetTypes)
    }
}