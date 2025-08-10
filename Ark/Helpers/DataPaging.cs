namespace Ark
{
    /// <summary>
    /// this class holds the data paging info.
    /// </summary>
    public class DataPaging
    {
        #region Constructors

        /// <summary>
        /// Creates a DataPaging instance.
        /// </summary>
        /// <param name="pageNumber">The page number (beginning from 0).</param>
        /// <param name="pageSize">The page size (number of items).</param>
        public DataPaging(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The page number (beginning from 0).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The page size (number of items).
        /// </summary>
        public int PageSize { get; set; }

        #endregion Properties
    }
}
