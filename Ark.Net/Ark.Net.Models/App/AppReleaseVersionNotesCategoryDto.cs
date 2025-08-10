namespace Ark.Net.Models
{
    /// <summary>
    /// Some notes grouped by category for an app release version.
    /// </summary>
    public class AppReleaseVersionNotesCategoryDto
    {
        #region Properties (Public)

        /// <summary>
        /// The title of the notes category (corrections, new features, ...).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The release notes for this category.
        /// </summary>
        public string[] Notes { get; set; }

        #endregion Properties (Public)
    }
}