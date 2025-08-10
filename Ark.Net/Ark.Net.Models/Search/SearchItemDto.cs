using System;

namespace Ark.Net.Models
{
    /// <summary>
    /// This class has the information about a specific search item type.
    /// The item type instance  must be created using a static method.
    /// Some common search item type creator are available for common resource search (UM, order, ...).
    /// </summary>
    public class SearchItemDto
    {
        #region Properties (Public)

        /// <summary>
        /// The code that uniquely identifies a search item type for all applications.
        /// This code is always prefixed by the application name or by COMMON_ if common type.
        /// Always in upper case, this code must be specified in the VwSearch SQL Server view for each item type.
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// The label of the search item type in the user current culture. 
        /// </summary>
        public string TypeLabel { get; set; }

        /// <summary>
        /// The identifier of the search table item cast in string.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The value of the search item that is the complete found identifier. 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The front URL to navigate to.
        /// </summary>
        public string FrontUrl { get; set; }

        /// <summary>
        /// The description that describes the best the page/behavior where to navigate in the user current culture.
        /// </summary>
        public string FrontUrlDescription { get; set; }

        /// <summary>
        /// The time where the data item to search has been lastly updated.
        /// Only for global search.
        /// </summary>
        public DateTime? LastUpdatedTime { get; set; }

        /// <summary>
        /// The summary that adds some relevant extra data for this search item.
        /// Only for global search.
        /// </summary>
        public string ExtraDataSummary { get; set; }

        /// <summary>
        /// The application which returns the search item.
        /// Only used for global search.
        /// </summary>
        public string Application { get; set; }

        #endregion Properties (Public)
    }
}