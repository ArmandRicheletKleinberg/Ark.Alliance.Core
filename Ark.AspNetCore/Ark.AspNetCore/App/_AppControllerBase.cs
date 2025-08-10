using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ark.Net.Models;
using Microsoft.AspNetCore.Mvc;


// ReSharper disable StaticMemberInGenericType
// ReSharper disable MemberCanBePrivate.Global

namespace Ark.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    /// This controller allows the client to get some information about this application like version and release notes.
    /// </summary>
    /// <typeparam name="TUserProfileData">The type of the user profile data if any, object if none.</typeparam>
    [ApiExplorerSettings(GroupName = "ο App")]
    public abstract class AppControllerBase<TUserProfileData> : ControllerBase<TUserProfileData>
        where TUserProfileData : new()
    {
        #region Static

        /// <summary>
        /// The application release versions are kept in memory cache to avoid recreating them at each call.
        /// </summary>
        protected static AppReleaseVersionDto[] Releases;

        #endregion Fields

        #region Methods (Releases)

        /// <summary>
        /// Gets the app info.
        /// </summary>
        /// <remarks>
        /// ## Permissions ##
        /// The user is authenticated for the app.
        /// ## Description ##
        /// This service is used by the client to show to the user the release notes.
        /// ## Example ##
        /// ```
        /// GET app/info
        /// ```
        /// *The app info are returned.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The app info are returned.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The user is not authenticated.</response>
        /// <returns></returns>
        [HttpGet("app/info")]
        public ResultDto<AppInfoDto> GetInfo()
            => ExecuteBl(() =>
            {
                var info = new AppInfoDto
                {
                    Environment = EnvironmentHelper.Current,
                    Version = Assembly.GetEntryAssembly()?.GetVersion(),
                    BuildTime = Assembly.GetEntryAssembly()?.GetBuildTime() ?? DateTime.MinValue
                };

                return new Result<AppInfoDto>(info);
            });

        /// <summary>
        /// Gets the app releases notes info optionally from a given version.
        /// </summary>
        /// <remarks>
        /// ## Permissions ##
        /// The user is authenticated for the app.
        /// ## Description ##
        /// This service is used by the client to show to the user the release notes.
        /// ## Example ##
        /// ```
        /// GET app/releases?fromVersion=1.04.00
        /// ```
        /// *All the release notes from the version 1.04.00 not included are returned.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The app releases notes info are returned.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The user is not authenticated.</response>
        /// <returns></returns>
        [HttpGet("app/releases")]
        public ResultDto<AppReleaseVersionDto[]> GetReleases(string fromVersion = null)
            => ExecuteBl(() =>
            {
                Releases ??= ExtractReleases();

                var releases = fromVersion != null ? Releases.Where(r => string.Compare(fromVersion, r.VersionNumber, StringComparison.Ordinal) < 0).ToArray() : Releases;
                return new Result<AppReleaseVersionDto[]>(releases);
            });

        /// <summary>
        /// Extract the release info from the readme.md content file.
        /// Searches for the part starting with H1 title # Release Notes until the next H1 title.
        /// </summary>
        /// <returns>The extracted release info.</returns>
        private AppReleaseVersionDto[] ExtractReleases()
        {
            var lines = System.IO.File.ReadAllLines("readme.md");
            var releaseNoteLines = lines
                .SkipWhile(l => l.StartsWith("# Release Notes"))
                .Skip(1)
                .TakeWhile(l => !l.StartsWith("# "))
                .Select(line => line.Replace("\t", "").Trim())
                .Where(l => l.IsNotNullOrWhiteSpace())
                .ToList();

            var releases = CreateReleases(releaseNoteLines).OrderByDescending(v => v.VersionNumber).ToArray();
            return releases;

        }

        /// <summary>
        /// Creates the releases which begin with line starting with ##.
        /// ie ## V1.01.00 : 27/04/2020.
        /// </summary>
        /// <param name="releaseNoteLines">All the lines from the release notes.</param>
        /// <returns>An enumerable with the release created.</returns>
        private IEnumerable<AppReleaseVersionDto> CreateReleases(List<string> releaseNoteLines)
        {
            while (releaseNoteLines.Count > 0)
            {
                releaseNoteLines = releaseNoteLines.SkipWhile(line => !line.StartsWith("## ")).ToList();
                var releaseLines = releaseNoteLines.TakeWhile((line, index) => index == 0 || !line.StartsWith("## ")).ToList();
                releaseNoteLines.RemoveRange(0, releaseLines.Count);

                var release = CreateRelease(releaseLines);
                if (release != null)
                    yield return release;
            }
        }

        /// <summary>
        /// Creates a single release given the release lines.
        /// </summary>
        /// <param name="releaseLines">The release lines to parse for info.</param>
        /// <returns>The created release.</returns>
        private AppReleaseVersionDto CreateRelease(List<string> releaseLines)
        {
            if (releaseLines.HasNoElements())
                return null;

            var header = releaseLines.First();
            var release = new AppReleaseVersionDto
            {
                VersionNumber = header.SubstringFrom('V').SubstringUntil(':').Trim(),
                ReleaseDate = header.SubstringFrom(':').Trim().ToDateTimeExact("dd/MM/yyyy"),
                NotesCategories = CreateReleaseNotesCategories(releaseLines).ToArray()
            };
            return release;
        }

        /// <summary>
        /// Creates the release categories from the release lines which starts with ### title.
        /// </summary>
        /// <param name="releaseLines">The lines from a single release.</param>
        /// <returns>The release categories from the release lines which starts with ### title.</returns>
        private IEnumerable<AppReleaseVersionNotesCategoryDto> CreateReleaseNotesCategories(List<string> releaseLines)
        {
            while (releaseLines.Count > 0)
            {
                releaseLines = releaseLines.SkipWhile(line => !line.StartsWith("### ")).ToList();
                var releaseCategoryLines = releaseLines.TakeWhile((line, index) => index == 0 || !line.StartsWith("### ")).ToList();
                releaseLines.RemoveRange(0, releaseCategoryLines.Count);

                if (releaseCategoryLines.Count < 2)
                    continue;

                var category = new AppReleaseVersionNotesCategoryDto
                {
                    Category = new string(releaseCategoryLines.First().Skip(4).ToArray()),
                    Notes = releaseCategoryLines.Skip(1).Select(l => l.Trim('*').Trim()).ToArray()
                };
                yield return category;
            }
        }

        #endregion Methods (Releases)
    }
}