namespace Ark.Net.Models
{

    /// <summary>
    /// This DTO contains the information necessary to post a new version of a dynamic mapping
    /// </summary>
    public class DynamicMappingFileDto
    {
        /// <summary>
        /// The identifier of the dynamic mapping.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The identifier of the poster of the dynamic mapping.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// A remark of the poster of the dynamic mapping
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Excel file send by the user.
        /// </summary>
        public FileDto File { get; set; }
    }
}
