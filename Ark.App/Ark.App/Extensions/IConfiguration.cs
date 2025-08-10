using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

// ReSharper disable UnusedMember.Global

namespace Ark.App
{
    /// <summary>
    /// Extension helpers for <see cref="IConfiguration"/> to simplify common
    /// queries. These methods help avoid repetitive <c>GetSection</c> calls when
    /// retrieving application settings.
    ///
    /// <para>Performance impact is minimal and equivalent to calling
    /// <c>IConfiguration</c> APIs directly.</para>
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IConfigurationExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Get the settings node children keys if any.
        /// </summary>
        /// <param name="configuration">The app configuration instance.</param>
        /// <param name="settingsKey">The parent settings key.</param>
        /// <returns>The keys of the found children.</returns>
        public static IEnumerable<string> GetChildrenKeys(this IConfiguration configuration, string settingsKey)
            => configuration.GetSection(settingsKey).GetChildren().Select(c => c.Key);

        #endregion Methods (Public)
    }
}