namespace Ark.Net.Models
{
    /// <summary>
    /// This class is used to specify all the parameters to query some data on a data source.
    /// The data can be paginated, ordered and filtered.
    /// </summary>
    public class DataQueryDto
    {
        #region Properties (Public)

        /// <summary>
        /// The number of rows that the paging should skip.
        /// Null for no paging.
        /// </summary>
        public int? PagingSkip { get; set; }

        /// <summary>
        /// The number of rows that the paging should take.
        /// Null for no paging.
        /// </summary>
        public int? PagingTake { get; set; }

        /// <summary>
        /// The name of the property the data should be ordered by.
        /// Null for no ordering.
        /// </summary>
        public string OrderByPropertyName { get; set; }

        /// <summary>
        /// Whether the data should be ordered by descending order.
        /// </summary>
        public bool OrderByDescending { get; set; }

        /// <summary>
        /// The filters to apply to filter the queried data.
        /// </summary>
        public DataQueryFilterDto[] Filters { get; set; }

        #endregion Properties (Public)
    }
}