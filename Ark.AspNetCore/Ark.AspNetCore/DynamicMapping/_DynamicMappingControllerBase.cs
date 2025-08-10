using System.Threading.Tasks;
using Ark.Net.CrossCutting;
using Ark.Net.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ark.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    /// This is the base class used to manage the dynamic mappings of an application.
    /// This class should be overriden in another project controller to allow access to the dynamic mappings management.
    /// </summary>
    /// <typeparam name="TUserProfileData">The type of the user profile data if any, object if none.</typeparam>
    [Authorize(nameof(UserCommonPermissionEnum.ManageDynamicMappings))]
    [ApiExplorerSettings(GroupName = "ο DynamicMapping")]
    public abstract class DynamicMappingControllerBase<TUserProfileData> : ControllerBase<TUserProfileData>
        where TUserProfileData : new()
    {
        #region Fields

        /// <summary>
        /// The dynamic mapping cross cutting services are needed.
        /// </summary>
        internal CrossCuttingDynamicMappingServices CrossCuttingDynamicMappingServices = new CrossCuttingDynamicMappingServices();

        #endregion Fields

        #region Methods (Public)

        /// <summary>
        /// This method returns all the dynamic methods for an application.
        /// </summary>
        /// <remarks>
        /// ## Description ##
        /// This service is used by some application to get their dynamic mappings
        /// ## Example ##
        /// ```
        /// GET dynamicmappings
        /// ```
        /// </remarks>
        /// <response code="200">
        /// Success : All dynamic mappings of the application are returned.
        /// Unauthorized : The user is not allowed to access the app.
        /// Unexpected : An unexpected error occurs.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [HttpGet("dynamicmappings")]
        public Task<ResultDto<DynamicMappingDto[]>> GetDynamicMappings()
            => ExecuteBlAsync(() => CrossCuttingDynamicMappingServices.GetDynamicMappings());



        /// <summary>
        /// This method returns a dynamic mapping with his last version.
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <remarks>
        /// ## Description ##
        /// This service is used by applications to get and compile a dynamic mapping.
        /// ## Example ##
        /// ```
        /// GET dynamicmapping/1
        /// ```
        /// </remarks>
        /// <response code="200">
        /// Success: The dynamic mapping has been successfully returned.
        /// BadParameters: The predicate is null.
        /// Failure: The predicate returns more than one dynamic mapping.
        /// NotFound: No dynamic mapping was found using this predicate.
        /// Unexpected: Unexpected failure.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [HttpGet("dynamicmapping/{id}")]
        public Task<ResultDto<DynamicMappingDto>> GetDynamicMapping(int id)
            => ExecuteBlAsync(() => CrossCuttingDynamicMappingServices.GetDynamicMapping(id));

        /// <summary>
        /// This method returns versions of a dynamic mapping
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <remarks>
        /// ## Description ##
        /// This service is used by some application to see the different versions of a dynamic mapping.
        /// ## Example ##
        /// ```
        /// GET dynamicmapping/1/versions
        /// ```
        /// </remarks>
        /// <response code="200">
        /// Success: All the versions have been successfully returned.
        /// BadParameters: The identifier is null.
        /// Failure: The identifier returns more than one dynamic mapping.
        /// NotFound: No dynamic mapping was found using this predicate.
        /// Unexpected: Unexpected failure.
        /// Unauthorized: The user is not allowed to access the app.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [HttpGet("dynamicmapping/{id}/versions")]
        public Task<ResultDto<DynamicMappingDto[]>> GetDynamicMappingVersions(int id)
            => ExecuteBlAsync(() => CrossCuttingDynamicMappingServices.GetDynamicMappingVersions(id));


        /// <summary>
        /// Return excel file of a version of a dynamic mapping.
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <param name="version">Version of the dynamic mapping.</param>
        /// <remarks>
        /// ## Description ##
        /// This service is used by some application to download the excel document of the dynamic mapping.
        /// ## Example ##
        /// ```
        /// GET dynamicmapping/1/version/1/excel
        /// ```
        /// </remarks>
        /// <response code="200">
        /// Success: The file has been successfully returned.
        /// BadParameters: The identifier is null.
        /// Failure: The predicate returns more than one version.
        /// NotFound: No version was found using this predicate.
        /// Unexpected: Unexpected failure.
        /// Unauthorized: The user is not allowed to access the app.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [HttpGet("dynamicmapping/{id}/version/{version}/excel")]
        public Task<IActionResult> GetDynamicMappingVersionExcel(int id, int version)
            => ExecuteBlAndReturnFile(() => CrossCuttingDynamicMappingServices.GetDynamicMappingVersionExcel(id, version));

        /// <summary>
        /// Create a new version of a dynamic mapping based on a excel file.
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <param name="dynamicMappingFile">The Dto with all the necessary informations to save a new version of a dynamic mapping.</param>
        /// <param name="isSaving">If it's true, save the new dynamic mapping, if it's false only make the validation.</param>
        /// <remarks>
        /// ## Description ##
        /// This service is used by some application to create a new version of the dynamic mapping.
        /// ## Example ##
        /// ```
        /// POST dynamicmapping/1/excel
        /// ```
        /// </remarks>
        /// <response code="200">      
        /// Success: The excel file is uploaded and the new version is created.
        /// BadParameters: the excel file contains errors which are in the Validation result Dto. 
        /// Failure: The identifier returns more than one entity.
        /// NotFound: No dynamic mapping was for this identifier.
        /// Unexpected: Unexpected failure.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [HttpPost("dynamicmapping/{id}/excel")]
        public Task<ResultDto<DynamicMappingValidationResultDto>> PostNewDynamicMappingVersionExcel(int id, [FromBody] DynamicMappingFileDto dynamicMappingFile, [FromQuery] bool isSaving)
            => ExecuteBlAsync(() => CrossCuttingDynamicMappingServices.PostNewDynamicMappingVersionExcel(id, dynamicMappingFile, isSaving));



        /// <summary>
        /// Rollback to a previous version of the dynamic mapping. 
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <param name="version">Version of the dynamic mapping.</param>
        /// <param name="userId">The identifier of the user which launches the rollback</param>
        /// <param name="remark">Remark by the user.</param>
        /// <remarks>
        /// ## Description ##
        /// This service is used by some application to rollback to a previous version of a dynamic mapping.
        /// ## Example ##
        /// ```
        /// POST dynamicmapping/1/version/1/rollback?userId=E029888&amp;remark=correction
        /// ```
        /// </remarks>
        /// <response code="200">
        /// Success: The rollback has been successfully done.
        /// BadParameters: The identifier is null.
        /// Failure: The predicate returns more than one version.
        /// NotFound: No version was found using this predicate.
        /// Already : The version is already the last version of the dynamic mapping.
        /// Unexpected: Unexpected failure.
        /// Unauthorized: The user is not allowed to access the app.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [HttpPost("dynamicmapping/{id}/version/{version}/rollback")]
        public Task<ResultDto> PostDynamicMappingVersionRollback(int id, int version, [FromQuery] string userId, [FromQuery] string remark)
            => ExecuteBlAsync(() => CrossCuttingDynamicMappingServices.PostDynamicMappingVersionRollback(id, version, userId, remark));

        #endregion Methods (Public)
    }
}