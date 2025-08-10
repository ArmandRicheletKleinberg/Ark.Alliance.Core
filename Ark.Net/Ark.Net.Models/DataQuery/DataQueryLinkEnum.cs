namespace Ark.Net.Models
{
    /// <summary>
    /// This enumeration lists all the link between filters (OR or AND).
    /// </summary>
    public enum DataQueryFilterLinkEnum
    {
        /// <summary>
        /// This filter is not link with any other filter.
        /// </summary>
        None = 0,

        /// <summary>
        /// This filter and all the linked filters must be valid for the filter to be valid.
        /// </summary>
        And = 10,

        /// <summary>
        /// This filter or one of all the linked filters must be valid for the filter to be valid.
        /// </summary>
        Or = 20
    }
}