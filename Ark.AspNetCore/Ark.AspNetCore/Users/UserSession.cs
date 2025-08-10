using System;
using System.Collections.Generic;
using Ark.Net.CrossCutting;
using Ark.Net.Models;

namespace Ark.AspNetCore
{
    /// <summary>
    /// The user session long with the user profile data.
    /// </summary>
    public abstract class UserSession
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
        /// The bytes content of the user picture if any.
        /// </summary>
        public byte[] UserPicture { get; set; }

        /// <summary>
        /// The permissions given to the user in the application.
        /// </summary>
        public HashSet<string> Permissions { get; set; }

        /// <summary>
        /// Some optional profile data of the user in the application.
        /// </summary>
        public object ProfileData { get; set; }

        /// <summary>
        /// The information about the connected app for the user.
        /// </summary>
        public UserAppDto App { get; set; }

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

    /// <inheritdoc />
    /// <summary>
    /// A generic UserSession with the strongly typed profile data.
    /// </summary>
    /// <typeparam name="TProfileData">The type of the profile data.</typeparam>
    public class UserSession<TProfileData> : UserSession
    {
        #region Properties (Public)

        /// <summary>
        /// Some optional profile data of the user in the application.
        /// </summary>
        public new TProfileData ProfileData
        {
            get => (TProfileData)base.ProfileData;
            set => base.ProfileData = value;
        }

        #endregion Properties (Public)
    }
}