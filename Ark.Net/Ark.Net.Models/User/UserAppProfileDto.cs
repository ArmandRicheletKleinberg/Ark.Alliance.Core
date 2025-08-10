using System.Collections.Generic;

namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO contains the user profile information for an application.
    /// </summary>
    public class UserAppProfileDto
    {
        #region Properties (Public)

        /// <summary>
        /// The identifier of the user.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The first name of the user.
        /// </summary>
        public string UserFirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        public string UserLastName { get; set; }

        /// <summary>
        /// The email address of the user if any.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// The user phone number if any.
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// The user picture if any.
        /// </summary>
        public byte[] UserPicture { get; set; }

        /// <summary>
        /// The permissions given to the user in the application.
        /// </summary>
        public string[] Permissions { get; set; }

        /// <summary>
        /// Some optional profile data of the user in the application in JSON.
        /// </summary>
        public string ProfileDataJson { get; set; }

        /// <summary>
        /// The different environments of the connected app.
        /// </summary>
        public Dictionary<string, UserAppDto> AppEnvironments { get; set; }

        /// <summary>
        /// The other apps available to the user.
        /// </summary>
        public UserAppDto[] AppsAvailable { get; set; }

        #endregion Properties (Public)
    }
}