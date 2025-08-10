using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Ark.Net.CrossCutting.Models;
using Ark.Net.Http;
using Ark.Net.Models;

namespace Ark.Net.CrossCutting
{
    /// <inheritdoc />
    /// <summary>
    /// This HTTP repository is used to access the cross cutting services on a server.
    /// It manages to cross cutting root URL server and adds the application name/client version in the HTTP headers.
    /// </summary>
    internal class CrossCuttingHttpRepository : HttpRepositoryBase
    {
        #region Methods (User)

        /// <summary>
        /// Authenticates an user for a specific application.
        /// </summary>
        /// <param name="userId">The identifier of the user that request access.</param>
        /// <returns>
        /// Success : The user has access to the application, the profile data with specific app data and permissions are returned.
        /// Unauthorized : The user has no access to the application.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<UserAppProfileDto>> AuthenticateUserForApp(string userId)
            => Get<UserAppProfileDto>($"user/{HttpUtility.UrlEncode(userId)}/auth");

        /// <summary>
        /// Saves the user profile data for a specific application.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="profileData">The profile data.</param>
        /// <returns>
        /// Success : The user has been found for this app and its data has been saved.
        /// BadParameters : The profile data must not be null or empty.
        /// NotFound : No user has been found for this app.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result> SaveUserProfileDataForApp(string userId, object profileData)
            => Put($"user/{HttpUtility.UrlEncode(userId)}/profile", profileData);

        /// <summary>
        /// Saves the user avatar picture.
        /// The picture will be resized and converted to jpeg image format.
        /// </summary>
        /// <param name="userId">The identifier of the user to save its avatar picture.</param>
        /// <param name="pictureContent">The picture content.</param>
        /// <returns>
        /// Success : The user avatar has been successfully updated.
        /// BadParameters : No picture file content has been sent.
        /// NotFound : No user has been found for this app.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result> SaveUserAvatar(string userId, byte[] pictureContent)
            => Put($"user/{HttpUtility.UrlEncode(userId)}/avatar", new FileDto { Content = pictureContent, Name = "picture.jpg" });

        /// <summary>
        /// Lists all the applications available for an user along with its permissions.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <returns>
        /// Success : The apps allowed for the user are returned along with his permissions.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<AppWithUserPermissionsDto[]>> GetUserApps(string userId)
            => Get<AppWithUserPermissionsDto[]>($"user/{HttpUtility.UrlEncode(userId)}/apps");

        /// <summary>
        /// Gets all the user roles for an application.
        /// </summary>
        /// <returns>
        /// Success : All the user roles of the application are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<UserRoleDto[]>> GetAllUserRoles()
            => Get<UserRoleDto[]>("users/roles");

        /// <summary>
        /// Gets all the users allowed for an application.
        /// </summary>
        /// <returns>
        /// Success : The users found are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<UserDto[]>> GetAllAllowedUsers()
            => Get<UserDto[]>("users");

        /// <summary>
        /// Allows a new user for the app.
        /// If the user does not exist in the Cross Cutting database, he is created.
        /// If the user already exists, creates or allows the link with the application.
        /// </summary>
        /// <param name="user">The user to link to the application.</param>
        /// <returns>
        /// Success : The users found are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result> AllowNewUserToApp(UserDto user)
            => Post("user", user);

        /// <summary>
        /// Updates the user information with its role.
        /// </summary>
        /// <param name="user">The data of the user to update.</param>
        /// <returns>
        /// Success : The update succeeded.
        /// NotFound : No link between the user and the app has been found.
        /// Unauthorized : The user is not allowed to access the app.
        /// Unexpected : Unexpected failure.
        /// </returns>
        public Task<Result> UpdateUser(UserDto user)
            => Put("user", user);

        /// <summary>
        /// Removes an user from an application access.
        /// It keeps the data of the user but he is flagged as not allowed for this app.
        /// </summary>
        /// <param name="userId">The identifier of the user to remove.</param>
        /// <returns>
        /// Success : The update succeeded, the user is no more allowed for this application.
        /// NotFound : No link between the user and the app has been found.
        /// Unexpected : Unexpected failure.
        /// </returns>
        public Task<Result> RemoveUserFromApplication(string userId)
            => Delete($"user/{WebUtility.UrlEncode(userId)}");

        #endregion Methods (User)

        #region Methods (Email)

        /// <summary>
        /// Calls the POST /email services to send an email message.
        /// </summary>
        /// <param name="request">The request data with the email message to send.</param>
        /// <returns>
        /// Success : The email message has been successfully sent.
        /// BadPrerequisites : No root URL was defined.
        /// BadParameters : The email message data have not been validated.
        /// NoConnection : Unable to connect to the cross cutting services.
        /// Unauthorized : The application has not the right to access this cross cutting service.
        /// Failure : The response coming from the server has an unexpected model and the deserialization failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        internal Task<Result> PostEmailSendRequest(EmailSendRequestDto request)
            => Post("email", request);

        #endregion Methods (Email)

        #region Methods (Dynamic mapping)

        /// <summary>
        /// This method returns all the dynamic methods for an application.
        /// </summary>
        /// <returns>
        /// Success : All dynamic mappings of the application are returned.
        /// Unauthorized : The user is not allowed to access the app.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<DynamicMappingDto[]>> GetDynamicMappings()
            => Get<DynamicMappingDto[]>("dynamicmappings");


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
            => Get<DynamicMappingDto>($"dynamicmapping/{id}");

        /// <summary>
        /// This method returns versions of a dynamic mapping
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <returns>
        /// Success: All the entities haven been successfully returned.
        /// BadParameters: The predicate is null.
        /// Failure: The predicate returns more than one entity.
        /// NotFound: No entity was found using this predicate.
        /// Unexpected: Unexpected failure.
        /// Unauthorized: The user is not allowed to access the app.
        /// </returns>
        public Task<Result<DynamicMappingDto[]>> GetDynamicMappingVersions(int id)
            => Get<DynamicMappingDto[]>($"dynamicmapping/{id}/versions");


        /// <summary>
        /// Return excel file of a version of a dynamic mapping.
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <param name="version">Version of the dynamic mapping.</param>
        /// <returns>
        /// Success: The file has been successfully returned.
        /// BadParameters: The predicate is null.
        /// Failure: The predicate returns more than one entity.
        /// NotFound: No entity was found using this predicate.
        /// Unexpected: Unexpected failure.
        /// Unauthorized: The user is not allowed to access the app.
        /// </returns>
        public Task<Result<FileDto>> GetDynamicMappingVersionExcel(int id, int version)
            => Get<FileDto>($"dynamicmapping/{id}/version/{version}/excel");



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
        public Task<Result<DynamicMappingValidationResultDto>> PostNewDynamicMappingVersionExcel(int id, DynamicMappingFileDto dynamicMappingFile, bool isSaving)
            => Post<DynamicMappingFileDto, DynamicMappingValidationResultDto>($"dynamicmapping/{id}/excel", dynamicMappingFile, new Dictionary<string, object>
            {
                [nameof(isSaving)] = isSaving.ToString()
            });



        /// <summary>
        /// Rollback to a previous version of the dynamic mapping. 
        /// </summary>
        /// <param name="id">The identifier of the dynamic mapping.</param>
        /// <param name="version">Version of the dynamic mapping.</param>
        /// <param name="userId">The identifier of the user which launches the rollback</param>
        /// <param name="remark">Remark by the user.</param>
        /// <returns>
        /// Success: The rollback has been successfully done.
        /// BadParameters: The predicate is null.
        /// Failure: The predicate returns more than one entity.
        /// NotFound: No entity was found using this predicate.
        /// Already : The version is already the last version of the dynamic mapping.
        /// Unexpected: Unexpected failure.
        /// Unauthorized: The user is not allowed to access the app.
        /// </returns>
        public Task<Result> PostDynamicMappingVersionRollback(int id, int version, string userId, string remark)
            => Post($"dynamicmapping/{id}/version/{version}/rollback", queryParameters: new Dictionary<string, object>
            {
                [nameof(userId)] = userId,
                [nameof(remark)] = remark
            });

        #endregion Methods (Dynamic mapping)


        #region Methods (Archives) 

        /// <summary>
        /// Stores a document archive in the database along with its metadata.
        /// </summary>
        /// <param name="archive">The document archive to upload.</param>
        /// <returns>
        /// **Success** - The document has been saved successfully in database.
        /// **BadParameters** - The file has not been provided.
        /// **Unexpected** - An unexpected error occurs.
        /// </returns>
        public Task<Result> PostStoreDocument(ArchiveToCreateDto archive)
            => Post($"archive", archive);

        #endregion Methods (Archives) 


        #region Methods (Override)

        /// <inheritdoc />
        protected override string RootUrl => CrossCuttingServices.CrossCuttingServerUrl;

        /// <inheritdoc />
        protected override bool UseDefaultCredentials => true;

        /// <inheritdoc />
        /// <summary>
        /// This override allows to add an Application header in all the HTTP request and to convert the ResultDto to a simple Result.
        /// </summary>
        protected override async Task<Result<TResponseData>> Request<TRequestData, TResponseData>(string urlRelativePath, HttpMethod httpMethod, TRequestData body = default, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
        {
            headers ??= new Dictionary<string, string>();
            headers.Add("Application", CrossCuttingServices.ApplicationId);
            headers.Add("ClientVersion", CrossCuttingServices.ClientVersion);

            var result = await base.Request<TRequestData, ResultDto<TResponseData>>(urlRelativePath, httpMethod, body, queryParameters, headers);
            if (result.Data == null)
                return new Result<TResponseData>(result);

            var exception = result.Data.Exception != null ? new WebApiServerException(result.Data.Exception) : null;
            return new Result<TResponseData>(result.Data.Status, result.Data.Data, exception).WithReason(result.Data.Reason);
        }

        #endregion Methods (Override) 
    }
}