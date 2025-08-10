using System;

namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO contains the user profile information for an application.
    /// </summary>
    public class UserDto
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
        /// The identifier of the user role in the application.
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// The last time the user has connect the app.
        /// </summary>
        public DateTime? LastConnectionTime { get; set; }

        /// <summary>
        /// The picture of the user. 
        /// </summary>
        public byte[] Picture { get; set; }

        #endregion Properties (Public)
    }
}