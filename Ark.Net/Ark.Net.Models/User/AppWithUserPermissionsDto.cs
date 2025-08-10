namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO holds the application identification along with the user permissions.
    /// Used to list the applications available for an user.
    /// </summary>
    public class AppWithUserPermissionsDto
    {
        #region Properties (Public)

        /// <summary>
        /// The identifier of the application.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// The name of the role.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// The permissions of this role.
        /// </summary>
        public string[] UserPermissions { get; set; }

        #endregion Properties (Public)
    }
}