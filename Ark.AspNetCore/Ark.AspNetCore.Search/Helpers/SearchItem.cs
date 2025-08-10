// ReSharper disable UnusedMember.Global
using Ark.Net.Models;

namespace Ark.AspNetCore.Search
{
    /// <summary>
    /// This class has the minimum data for an item to search that is an identifier and a value.
    /// </summary>
    public class SearchItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="SearchItem"/> instance.
        /// </summary>
        /// <param name="id">The identifier of the entity item found.</param>
        /// <param name="value">The value that contains the text searched.</param>
        public SearchItem(string id, string value)
        {
            Id = id;
            Value = value;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The identifier of the entity item found.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The value that contains the text searched.
        /// </summary>
        public string Value { get; set; }

        #endregion Properties (Public)
    }
}