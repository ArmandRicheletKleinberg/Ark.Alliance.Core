using System.Threading.Tasks;
using Ark.Net.CrossCutting;
using Ark.Net.CrossCutting.Models;
using Ark.Net.Models;

using Microsoft.AspNetCore.Mvc;

namespace Ark.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    /// This is the base class used to manage the archives of an application.
    /// This class should be overriden in another project controller to allow access to the users management.
    /// </summary>
    /// <typeparam name="TUserProfileData">The type of the user profile data if any, object if none.</typeparam>
    [Authorize]
    [ApiExplorerSettings(GroupName = "ο Archive")]
    public abstract class ArchiveControllerBase<TUserProfileData> : ControllerBase<TUserProfileData>
        where TUserProfileData : new()
    {
        #region Fields

        /// <summary>
        /// The archive cross cutting services are needed.
        /// </summary>
        internal CrossCuttingArchiveService CrossCuttingArchiveService = new CrossCuttingArchiveService();

        #endregion Fields

        #region Methods (Public)

        /// <summary>
        /// Stores a document archive in the database along with its metadata.
        /// </summary>
        /// <param name="archive">The document archive to upload.</param>
        /// <remarks>
        /// ## Permissions ##
        /// Request coming from allowed remote endpoint.
        /// ## Description ##
        /// This service is used by some application to add archives.
        /// ## Example ##
        /// ```
        /// POST archive
        /// BODY { ... }
        /// ```
        /// *Add a new archive.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The document has been saved successfully in database.
        /// **BadParameters** - The file has not been provided.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The application is not allowed to access the archive services from the CrossCutting services.</response>
        /// <returns></returns>
        [HttpPost("archive")]
        public Task<ResultDto> PostStoreDocument([FromBody] ArchiveToCreateDto archive)
            => ExecuteBlAsync(() => CrossCuttingArchiveService.StoreDocument(archive));

        #endregion Methods (Management CRUD)
    }
}
