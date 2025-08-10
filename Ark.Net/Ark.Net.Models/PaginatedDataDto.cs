namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO is a wrapper around paginated data which are basically a list of data along with the paging information.
    /// </summary>
    /// <typeparam name="TDto">The type of the data included.</typeparam>
    public class PaginatedDataDto<TDto>
    {
        /// <summary>
        /// The total number of data items available in the request.
        /// This should be more than the paginated data returned.
        /// </summary>
        public int TotalItem { get; set; }

        /// <summary>
        /// The number of data items to skip before getting the items.
        /// </summary>
        public int SkipItem { get; set; }

        /// <summary>
        /// The maximum number of items to take from the skip position.
        /// </summary>
        public int TakeItem { get; set; }

        /// <summary>
        /// The paginated data to return.
        /// </summary>
        public TDto[] Data { get; set; }
    }
}