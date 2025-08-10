using System;
using System.Threading.Tasks;
using Ark.App.Diagnostics;
using Ark.Net.Models;
using Microsoft.Extensions.Logging;

namespace Ark.Net.CrossCutting
{
    /// <summary>
    /// The main class of the cross cutting services.
    /// Static and only used for services initialization.
    /// </summary>
    public class CrossCuttingDynamicMappingServices
    {
        #region Fields

        /// <summary>
        /// The cross cutting HTTP repository is needed.
        /// </summary>
        internal CrossCuttingHttpRepository CrossCuttingHttpRepository = new CrossCuttingHttpRepository();

        /// <summary>
        /// The cross cutting cache repository is needed.
        /// </summary>
        internal DynamicMappingCacheRepository DynamicMappingCacheRepository = new DynamicMappingCacheRepository();

        #endregion Fields

        #region Methods (Public)

        /// <summary>
        /// Executes a dynamic mapping method given an object.
        /// </summary>
        /// <typeparam name="TObject">The type of the object to map.</typeparam>
        /// <param name="mapping">The mapping to use.</param>
        /// <param name="obj">The object to map.</param>
        /// <returns>
        /// Success : The execution is successfull.
        /// NotFound : The dynamic mapping has not been found on the Cross Cutting services.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual async Task<Result> ExecuteDynamicMapping<TObject>(DynamicMappingEnum mapping, TObject obj)
        {
            try
            {
                // TODO : Check if the dynamic mappings should be refreshed from CrossCutting services BEWARE A MUTEX MUST BE USED TO SYNCHRONIZE THE INITIAL SETUP
                var mappingCache = DynamicMappingCacheRepository.Get(mapping);
                if (mappingCache?.Config == null)
                {
                    // TODO : REWORK
                    var mappingsResult = await CrossCuttingHttpRepository.GetDynamicMapping((int)mapping);
                    if (mappingsResult.IsNotSuccess)
                    {
                        CrossCuttingServices.Logger?.LogError($"Unable to find the dynamic mapping in the application cache : {mapping}");
                        return Result.NotFound.WithReason("Unable to find the dynamic mapping in the application cache");
                    }

                    mappingCache = new DynamicMappingCache { Config = mappingsResult.Data };
                    DynamicMappingCacheRepository.Set(mapping, mappingCache);
                }

                if (mappingCache.CompiledMethod == null)
                {
                    var compileResult = await new DynamicMappingCompiler<TObject>(mappingCache.Config).Compile();


                    if (compileResult.IsNotSuccess)
                    {
                        CrossCuttingServices.Logger?.LogResult(compileResult);
                        return compileResult;
                    }

                    mappingCache.CompiledMethod = compileResult.Data;
                }


                mappingCache.CompiledMethod(obj);

                return Result.Success;
            }
            catch (Exception exception)
            {
                CrossCuttingServices.Logger?.LogError(exception, "Unexpected error when executing ExecuteDynamicMapping");
                return new Result(exception);
            }
        }


        /// <summary>
        /// This method returns all the dynamic methods for an application.
        /// </summary>
        /// <returns>
        /// Success : All dynamic mappings of the application are returned.
        /// Unauthorized : The user is not allowed to access the app.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<DynamicMappingDto[]>> GetDynamicMappings()
            => CrossCuttingHttpRepository.GetDynamicMappings();


        /// <summary>
        /// This method returns a dynamic mapping with his last version.
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <returns>
        /// Success: The dynamic mapping has been successfully returned.
        /// BadParameters: The predicate is null.
        /// Failure: The predicate returns more than one dynamic mapping.
        /// NotFound: No dynamic mapping was found using this predicate.
        /// Unexpected: Unexpected failure.
        /// </returns>
        public Task<Result<DynamicMappingDto>> GetDynamicMapping(int id)
            => CrossCuttingHttpRepository.GetDynamicMapping(id);


        /// <summary>
        /// This method returns versions of a dynamic mapping
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <returns>
        /// Success: All the versions have been successfully returned.
        /// BadParameters: The identifier is null.
        /// Failure: The identifier returns more than one dynamic mapping.
        /// NotFound: No dynamic mapping was found using this predicate.
        /// Unexpected: Unexpected failure.
        /// Unauthorized: The user is not allowed to access the app.
        /// </returns>
        public Task<Result<DynamicMappingDto[]>> GetDynamicMappingVersions(int id)
            => CrossCuttingHttpRepository.GetDynamicMappingVersions(id);


        /// <summary>
        /// Return excel file of a version of a dynamic mapping.
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <param name="version">Version of the dynamic mapping.</param>
        /// <returns>
        /// Success: The file has been successfully returned.
        /// BadParameters: The identifier is null.
        /// Failure: The predicate returns more than one version.
        /// NotFound: No version was found using this predicate.
        /// Unexpected: Unexpected failure.
        /// Unauthorized: The user is not allowed to access the app.
        /// </returns>
        public Task<Result<FileDto>> GetDynamicMappingVersionExcel(int id, int version)
            => CrossCuttingHttpRepository.GetDynamicMappingVersionExcel(id, version);


        /// <summary>
        /// Create a new version of a dynamic mapping based on a excel file.
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <param name="dynamicMappingFile">The Dto with all the necessary informations to save a new version of a dynamic mapping.</param>
        /// <param name="isSaving">If it's true, save the new dynamic mapping, if it's false only make the validation.</param>
        /// <returns>
        /// Success: The excel file is uploaded and the new version is created.
        /// BadParameters: the excel file contains errors. 
        /// Failure: The identifier returns more than one entity.
        /// NotFound: No dynamic mapping was for this identifier.
        /// Unexpected: Unexpected failure.
        /// </returns>
        public async Task<Result<DynamicMappingValidationResultDto>> PostNewDynamicMappingVersionExcel(int id, DynamicMappingFileDto dynamicMappingFile, bool isSaving)
        {
            var newDynamicMappingResult = await CrossCuttingHttpRepository.PostNewDynamicMappingVersionExcel(id, dynamicMappingFile, isSaving);

            if (newDynamicMappingResult.IsSuccess && DynamicMappingCacheRepository.Contains((DynamicMappingEnum)id))
                DynamicMappingCacheRepository.Remove((DynamicMappingEnum)id);

            return newDynamicMappingResult;

        }


        /// <summary>
        /// Rollback to a previous version of the dynamic mapping. 
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <param name="version">Version of the dynamic mapping.</param>
        /// <param name="userId">The identifier of the user which launches the rollback</param>
        /// <param name="remark">Remark by the user.</param>
        /// <returns>
        /// Success: The rollback has been successfully done.
        /// BadParameters: The identifier is null.
        /// Failure: The predicate returns more than one version.
        /// NotFound: No version was found using this predicate.
        /// Already : The version is already the last version of the dynamic mapping.
        /// Unexpected: Unexpected failure.
        /// Unauthorized: The user is not allowed to access the app.
        /// </returns>
        public async Task<Result> PostDynamicMappingVersionRollback(int id, int version, string userId, string remark)
        {
            var postRollbackResult = await CrossCuttingHttpRepository.PostDynamicMappingVersionRollback(id, version, userId, remark);

            if (postRollbackResult.IsSuccess && DynamicMappingCacheRepository.Contains((DynamicMappingEnum)id))
                DynamicMappingCacheRepository.Remove((DynamicMappingEnum)id);

            return postRollbackResult;
        }

        #endregion Methods (Public)
    }
}