using System.Collections.Generic;

namespace Ark.Net.Models
{
    /// <summary>
    /// The user session information.
    /// </summary>
    public class UserSessionDto
    {
        #region Properties (Public)

        /// <summary>
        /// The identifier of the user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The permissions given to the user in the application.
        /// </summary>
        public HashSet<string> Permissions { get; set; }

        /// <summary>
        /// Some optional profile data of the user in the application.
        /// </summary>
        public object ProfileData { get; set; }

        /// <summary>
        /// The different environments of the connected app.
        /// </summary>
        public Dictionary<string, UserSessionAppDto> AppEnvironments { get; set; }

        /// <summary>
        /// The other apps available to the user.
        /// </summary>
        public UserSessionAppDto[] OtherAppsAvailable { get; set; }

        #endregion Properties (Public)
    }
}