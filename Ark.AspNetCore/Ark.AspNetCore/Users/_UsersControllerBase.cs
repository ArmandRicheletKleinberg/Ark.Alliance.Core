using System.Linq;
using System.Threading.Tasks;
using Ark.Net.CrossCutting;
using Ark.Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ark.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    /// This is the base class used to authenticate/authorize and mange the users.
    /// This class should be overriden in another project controller to allow access to the users management.
    /// Security must be set to allow only super admin to connect this controller.
    /// </summary>
    /// <typeparam name="TUserProfileData">The type of the user profile data if any, object if none.</typeparam>
    [Authorize]
    [ApiExplorerSettings(GroupName = "ο Users")]
    public abstract class UsersControllerBase<TUserProfileData> : ControllerBase<TUserProfileData>
        where TUserProfileData : new()
    {
        #region Fields

        /// <summary>
        /// The user cross cutting services are needed.
        /// </summary>
        internal CrossCuttingUserServices CrossCuttingUserServices = new CrossCuttingUserServices();

        #endregion Fields

        #region Methods (Authentication and profile)

        /// <summary>
        /// Gets the current logged user session info along with its profile data and permissions.
        /// </summary>
        /// <remarks>
        /// ## Permissions ##
        /// The user is authenticated for the app.
        /// ## Description ##
        /// This service is used by the client to get the user profile data.
        /// ## Example ##
        /// ```
        /// GET user
        /// BODY { ... }
        /// ```
        /// *The logged user profile data are returned.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The user has been found for this app and its profile data are returned.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The user is not authenticated.</response>
        /// <returns></returns>
        [HttpGet("user")]
        public ResultDto<UserSessionDto> GetCurrentUserSession()
        => ExecuteBl(() => new Result<UserSessionDto>(new UserSessionDto
        {
            Id = UserSession.UserId,
            FirstName = UserSession.UserFirstName,
            LastName = UserSession.UserLastName,
            ProfileData = UserSession.ProfileData,
            Permissions = UserSession.Permissions,
            AppEnvironments = UserSession.AppEnvironments.ToDictionary(kvp => kvp.Key, kvp => new UserSessionAppDto { FrontUrl = kvp.Value.FrontUrl }),
            OtherAppsAvailable = UserSession.AppsAvailable.Where(app => app.Id != CrossCuttingServices.ApplicationId).Select(app => new UserSessionAppDto
            {
                Icon = app.Icon,
                Name = app.Name,
                Description = app.Description,
                LastConnectionTime = app.LastConnectionTime,
                FrontUrl = app.FrontUrl,
                BackUrl = app.BackUrl,
                HasArchives = app.HasArchives
            }).ToArray()
        }));

        /// <summary>
        /// Returns the user avatar from the current logged user session if any.
        /// </summary>
        /// <remarks>
        /// ## Permissions ##
        /// The user is authenticated for the app.
        /// ## Description ##
        /// This service is used by the client to display an user avatar.
        /// ## Example ##
        /// ```
        /// GET user/avatar
        /// ```
        /// *The user avatar picture if any or 404 otherwise.*
        /// </remarks>
        /// <response code="200">The user avatar file content is returned.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="404">The user avatar has not been provided</response>
        /// <returns></returns>
        [HttpGet("user/avatar")]
        public Task<IActionResult> GetCurrentUserAvatar()
            => ExecuteBlAndReturnFile(() => Task.Run(() =>
                UserSession.UserPicture != null
                    ? new Result<FileDto>(new FileDto { MimeType = "image/jpeg", Content = UserSession.UserPicture })
                    : Result<FileDto>.NotFound));

        /// <summary>
        /// Saves the user avatar picture.
        /// The picture will be resized and converted to webp image format.
        /// </summary>
        /// <param name="file">The uploaded file.</param>
        /// <remarks>
        /// ## Permissions ##
        /// Request coming from allowed remote endpoint.
        /// ## Description ##
        /// This service is used by all application to allow the user to change its avatar picture.
        /// ## Example ##
        /// ```
        /// PUT user/avatar
        /// BODY { Form file }
        /// ```
        /// *The current user avatar is updated in database.*
        /// </remarks>
        /// <response code="200">
        /// **Success** - The user avatar has been successfully updated.
        /// **BadParameters** - No picture file content has been sent.
        /// **NotFound** - No user has been found for this app.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [HttpPut("user/avatar")]
        public Task<ResultDto> SaveUserAvatar(IFormFile file)
            => ExecuteBlAsync(async () =>
            {
                if (file == null)
                    return Result.BadParameters.WithReason("A file must be uploaded");

                var fileContent = await file.GetFileContent();
                var updateResult = await CrossCuttingUserServices.SaveUserAvatar(UserSession.UserId, fileContent);
                if (updateResult.IsNotSuccess)
                    return updateResult;

                var authenticateResult = await CrossCuttingUserServices.AuthenticateUserForApp(UserSession.UserId);
                if (authenticateResult.IsNotSuccess)
                    return authenticateResult;
                UserSession.UserPicture = authenticateResult.Data.UserPicture;

                return Result.Success;
            });

        /// <summary>
        /// Saves the user profile data for the application.
        /// </summary>
        /// <param name="profileData">The new profile data of the user.</param>
        /// <remarks>
        /// ## Permissions ##
        /// The user is authenticated for the app.
        /// ## Description ##
        /// This service is used by some application to keep the user preferences.
        /// ## Example ##
        /// ```
        /// PUT user/profile
        /// BODY { ... }
        /// ```
        /// *The logged user preferences are stored in the application user profile data.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The user has been found for this app and its data has been saved.
        /// **BadParameters** - The profile data must not be null or empty.
        /// **NotFound** - No user has been found for this app.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The user is not authenticated.</response>
        /// <returns></returns>
        [HttpPut("user/profile")]
        public Task<ResultDto> SaveUserProfileDataForApp([FromBody] TUserProfileData profileData)
            => ExecuteBlAsync(() =>
            {
                UserSession.ProfileData = profileData;
                return CrossCuttingUserServices.SaveUserProfileDataForApp(UserSession.UserId, profileData);
            });

        #endregion Methods (Authentication and profile)

        #region Methods (Applications & Roles)

        /// <summary>
        /// Lists all the applications available for an user along with its permissions.
        /// </summary>
        /// <remarks>
        /// ## Permissions ##
        /// Request coming from allowed remote endpoint.
        /// ## Description ##
        /// This service is used by some application to list the apps allowed for the user.
        /// ## Example ##
        /// ```
        /// GET user/apps
        /// ```
        /// *Returns the applications allowed for the connected user.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The apps allowed for the user are returned along with his permissions.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [Authorize(nameof(UserCommonPermissionEnum.ManageUsers))]
        [HttpGet("user/apps")]
        public Task<ResultDto<AppWithUserPermissionsDto[]>> GetApplicationsForUserWithPermissions()
            => ExecuteBlAsync(() => CrossCuttingUserServices.GetUserApps(UserSession.UserId));

        /// <summary>
        /// Gets all the user roles for an application.
        /// </summary>
        /// <remarks>
        /// ## Permissions ##
        /// Request coming from allowed remote endpoint.
        /// ## Description ##
        /// This service is used by some application to manage the users.
        /// ## Example ##
        /// ```
        /// GET users
        /// ```
        /// *Lists all the users allowed for this application.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The users allowed for this application.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [Authorize(nameof(UserCommonPermissionEnum.ManageUsers))]
        [HttpGet("users/roles")]
        public Task<ResultDto<UserRoleDto[]>> GetAllUserRoles()
            => ExecuteBlAsync(() => CrossCuttingUserServices.GetAllUserRoles());

        #endregion Methods (Applications & Roles)

        #region Methods (Management CRUD)

        /// <summary>
        /// Gets all the users allowed for an application.
        /// </summary>
        /// <remarks>
        /// ## Permissions ##
        /// Request coming from allowed remote endpoint.
        /// ## Description ##
        /// This service is used by some application to manage the users.
        /// ## Example ##
        /// ```
        /// GET users
        /// ```
        /// *Lists all the users allowed for this application.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The users allowed for this application.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [Authorize(nameof(UserCommonPermissionEnum.ManageUsers))]
        [HttpGet("users")]
        public Task<ResultDto<UserDto[]>> GetAllAllowedUsers()
            => ExecuteBlAsync(() => CrossCuttingUserServices.GetAllAllowedUsers());

        /// <summary>
        /// Allows a new user for the app.
        /// If the user does not exist in the Cross Cutting database, he is created.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <remarks>
        /// ## Permissions ##
        /// Request coming from allowed remote endpoint.
        /// ## Description ##
        /// This service is used by some application to manage the users.
        /// ## Example ##
        /// ```
        /// POST user
        /// BODY { ... }
        /// ```
        /// *Adds a new user allowed for this application.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The user has been allowed for this application.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [Authorize(nameof(UserCommonPermissionEnum.ManageUsers))]
        [HttpPost("user")]
        public Task<ResultDto> AllowNewUserToApp([FromBody] UserDto user)
            => ExecuteBlAsync(() => CrossCuttingUserServices.AllowNewUserToApp(user));

        /// <summary>
        /// Updates the user information with its role.
        /// </summary>
        /// <param name="user">The data of the user to update.</param>
        /// <remarks>
        /// ## Permissions ##
        /// Request coming from allowed remote endpoint.
        /// ## Description ##
        /// This service is used by some application to manage the users.
        /// ## Example ##
        /// ```
        /// PUT user
        /// BODY { ... }
        /// ```
        /// *Updates an existing user for this application.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The update succeeded.
        /// **NotFound** - No link between the user and the app has been found.
        /// **Unauthorized** - The user is not allowed to access the app.
        /// **Unexpected** - Unexpected failure.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [Authorize(nameof(UserCommonPermissionEnum.ManageUsers))]
        [HttpPut("user")]
        public Task<ResultDto> UpdateUser([FromBody] UserDto user)
            => ExecuteBlAsync(() => CrossCuttingUserServices.UpdateUser(user));

        /// <summary>
        /// Removes an user from an application access.
        /// It keeps the data of the user but he is flagged as not allowed for this app.
        /// </summary>
        /// <param name="userId">The identifier of the user to remove.</param>
        /// <remarks>
        /// ## Permissions ##
        /// Request coming from allowed remote endpoint.
        /// ## Description ##
        /// This service is used by some application to manage the users.
        /// ## Example ##
        /// ```
        /// DELETE user/ARMONY&#x5c;e029900
        /// ```
        /// *Removes an existing user from this application access.*  
        /// </remarks>
        /// <response code="200">
        /// **Success** - The update succeeded, the user is no more allowed for this application.
        /// **NotFound** - No link between the user and the app has been found.
        /// **Unexpected** - Unexpected failure.
        /// </response>
        /// <response code="401">The application is not allowed to access the user services from the CrossCutting services.</response>
        /// <returns></returns>
        [Authorize(nameof(UserCommonPermissionEnum.ManageUsers))]
        [HttpDelete("user/{userId}")]
        public Task<ResultDto> RemoveUserFromApplication(string userId)
            => ExecuteBlAsync(() => CrossCuttingUserServices.RemoveUserFromApplication(userId));

        #endregion Methods (Management CRUD)
    }
}