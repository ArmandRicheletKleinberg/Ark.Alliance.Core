using System;

namespace Ark.AspNetCore.Search
{
    /// <summary>
    /// This is the data that needs to be extracted from the database for each search item type.
    /// The common data contains the value that contains text searched, the identifier of the entity.
    /// </summary>
    internal class SearchItemDbEntity
    {
        #region Properties (Public)

        /// <summary>
        /// The identifier of the entity item found.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The value that contains the text searched.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The type of the item to search in the database.
        /// Will be set automatically by the services.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The date and time when the item has been lastly created/updated.
        /// </summary>
        internal DateTime? DateAndTime { get; set; }

        /// <summary>
        /// A summary text with some extra data information about the searched item.
        /// </summary>
        internal string SummaryText { get; set; }

        #endregion Properties (Public)
    }
}