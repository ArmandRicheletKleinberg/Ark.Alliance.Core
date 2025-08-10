// ReSharper disable UnusedMember.Global

namespace Ark.Net.Models
{
    /// <summary>
    /// This class has the information about a type of item that can be searched in an application.
    /// </summary>
    public class SearchItemTypeDto
    {
        #region Properties (Public)

        /// <summary>
        /// The code that uniquely identifies a search item type for all applications.
        /// This code is always prefixed by the application name or by COMMON_ if common type.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The search item type label in the current culture.
        /// </summary>
        public string Label { get; set; }

        #endregion Properties (Public)
    }
}