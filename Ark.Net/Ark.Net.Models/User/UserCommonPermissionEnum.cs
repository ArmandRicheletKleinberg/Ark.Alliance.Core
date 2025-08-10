namespace Ark.Net.Models
{
    /// <summary>
    /// The enumeration of all the user applications common permission such as SeeDiagnostics or ManageUsers.
    /// </summary>
    public enum UserCommonPermissionEnum
    {
        /// <summary>
        /// The user is allowed to see the application diagnostics.
        /// </summary>
        SeeDiagnostics,

        /// <summary>
        /// The user is allowed to manage the dynamic mappings of the application.
        /// </summary>
        ManageDynamicMappings,

        /// <summary>
        /// The user can manage the application users.
        /// </summary>
        ManageUsers,

        /// <summary>
        /// The user can manage the application.
        /// </summary>
        SysAdmin,
    }
}