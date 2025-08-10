using System;

namespace Ark.Net.Models
{
    /// <summary>
    /// The information about the app release version.
    /// </summary>
    public class AppReleaseVersionDto
    {
        #region Properties (Public)

        /// <summary>
        /// The version number for this release.
        /// </summary>
        public string VersionNumber { get; set; }

        /// <summary>
        /// The date when this version was released.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// The notes 
        /// </summary>
        public AppReleaseVersionNotesCategoryDto[] NotesCategories { get; set; }

        #endregion Properties (Public)
    }
}