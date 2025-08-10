namespace Ark.AspNetCore
{
    /// <summary>
    /// Defines supported authorization strategies for the API.
    /// + Encapsulates differing back-end validation mechanisms.
    /// - Introducing a new strategy requires code changes across the stack.
    /// Ref: <see href="https://learn.microsoft.com/aspnet/core/security/authorization/"/>
    /// </summary>
    public enum AuthorizationTypeEnum
    {
        /// <summary>
        /// No authorization is enforced; use only for development scenarios.
        /// + Simplifies initial prototypes.
        /// - Exposes every endpoint publicly.
        /// </summary>
        None = 0,

        /// <summary>
        /// Uses in-memory fake authentication for testing flows.
        /// + Allows UI tests without external dependencies.
        /// - Must never be enabled in production.
        /// </summary>
        Fake = 0,

        /// <summary>
        /// Relies on centralized Cross Cutting Services to validate users.
        /// + Provides unified permissions across applications.
        /// - Requires network connectivity to the Cross Cutting service.
        /// </summary>
        CrossCutting = 1,

        /// <summary>
        /// Uses Windows Active Directory for authorization.
        /// + Leverages existing domain accounts.
        /// - Tightly couples the application to AD infrastructure.
        /// Ref: <see href="https://learn.microsoft.com/aspnet/core/security/authentication/windowsauth"/>
        /// </summary>
        ActiveDirectory = 2,


    }
}