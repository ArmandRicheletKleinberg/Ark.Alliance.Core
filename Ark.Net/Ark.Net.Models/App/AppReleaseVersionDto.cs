using System;

namespace Ark.Net.Models
{
    /// <summary>
    /// The information about the app.
    /// </summary>
    public class AppInfoDto
    {
        #region Properties (Public)

        /// <summary>
        /// The version number for this release.
        /// </summary>
        public EnvironmentEnum Environment { get; set; }

        /// <summary>
        /// The version of the application build.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The time when the application has been built successfully.
        /// </summary>
        public DateTime BuildTime { get; set; }

        #endregion Properties (Public)
    }
}