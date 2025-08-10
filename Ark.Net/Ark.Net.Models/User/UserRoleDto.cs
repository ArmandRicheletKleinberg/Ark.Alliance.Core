namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO contains a possible role for an user in an application.
    /// </summary>
    public class UserRoleDto
    {
        #region Properties (Public)

        /// <summary>
        /// The identifier of the role.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the role.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The permissions of this role.
        /// </summary>
        public string[] Permissions { get; set; }

        #endregion Properties (Public)
    }
}