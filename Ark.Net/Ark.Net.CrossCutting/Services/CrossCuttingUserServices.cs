using System.Threading.Tasks;
using Ark.Net.Models;

namespace Ark.Net.CrossCutting
{
    /// <summary>
    /// This class is used to access the cross cutting email services.
    /// Mainly used to send emails.
    /// </summary>
    public class CrossCuttingUserServices
    {
        #region Fields

        /// <summary>
        /// The cross cutting HTTP repository is needed.
        /// </summary>
        internal CrossCuttingHttpRepository CrossCuttingHttpRepository = new CrossCuttingHttpRepository();

        #endregion Fields

        #region Properties (Public)

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
            => CrossCuttingHttpRepository.AuthenticateUserForApp(userId);

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
            => CrossCuttingHttpRepository.SaveUserProfileDataForApp(userId, profileData);

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
            => CrossCuttingHttpRepository.SaveUserAvatar(userId, pictureContent);

        /// <summary>
        /// Lists all the applications available for an user along with its permissions.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <returns>
        /// Success : The apps allowed for the user are returned along with his permissions.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<AppWithUserPermissionsDto[]>> GetUserApps(string userId)
            => CrossCuttingHttpRepository.GetUserApps(userId);

        /// <summary>
        /// Gets all the user roles for an application.
        /// </summary>
        /// <returns>
        /// Success : All the user roles of the application are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<UserRoleDto[]>> GetAllUserRoles()
            => CrossCuttingHttpRepository.GetAllUserRoles();

        /// <summary>
        /// Gets all the users allowed for an application.
        /// </summary>
        /// <returns>
        /// Success : The users found are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<UserDto[]>> GetAllAllowedUsers()
            => CrossCuttingHttpRepository.GetAllAllowedUsers();

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
            => CrossCuttingHttpRepository.AllowNewUserToApp(user);

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
            => CrossCuttingHttpRepository.UpdateUser(user);

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
            => CrossCuttingHttpRepository.RemoveUserFromApplication(userId);

        #endregion Properties (Public)
    }
}