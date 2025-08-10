#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ark.App.Diagnostics;
using Ark.Net.CrossCutting;
using Ark.Net.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Ark.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute must be set on resources that need to be accessed by an user with the correct permission.
    /// </summary>
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="AuthorizeAttribute" /> instance.
        /// </summary>
        public AuthorizeAttribute()
            : this("")
        { }

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="AuthorizeAttribute" /> instance.
        /// </summary>
        /// <param name="permission">The permission required to access this resource.</param>
        public AuthorizeAttribute(string permission)
            : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { permission };
        }

        #endregion Constructors
    }

    /// <inheritdoc />
    /// <summary>
    /// This is the base class for the authorization filter used in the DSS application.
    /// </summary>
    public class AuthorizeFilter : IAsyncAuthorizationFilter
    {
        #region Fields

        /// <summary>
        /// The cross cutting user services are needed.
        /// </summary>
        internal CrossCuttingUserServices CrossCuttingUserServices = new CrossCuttingUserServices();

        /// <summary>
        /// The user session business logic is needed for all authorization.
        /// </summary>
        internal UserSessionCacheRepository UserSessionCacheRepository = new UserSessionCacheRepository();

        /// <summary>
        /// The type of the class that contains the user data profile.
        /// </summary>
        private readonly Type _userProfileDateType;

        /// <summary>
        /// The permission required to access this resource.
        /// </summary>
        private readonly string _permission;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="AuthorizeFilter"/> instance.
        /// </summary>
        /// <param name="serverConfig">The configuration of the server (injected).</param>
        /// <param name="permission">The permission needed to access the resource.</param>
        public AuthorizeFilter(ServerConfig serverConfig, string permission)
        {
            _userProfileDateType = serverConfig.UserDataProfileType;
            _permission = permission;
        }

        #endregion Constructor

        #region IAuthorizationFilter

        /// <inheritdoc />
        public virtual async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // If the user is not authenticated then forces it to authenticate
            if (!(context.HttpContext.User.Identity?.IsAuthenticated ?? false))
            {
                await context.HttpContext.ChallengeAsync();
                context.Result = new EmptyResult();
                return;
            }

            // A windows identity must have been fetched from the request
            string? adUserName = null;
            if (OperatingSystem.IsWindows())
                adUserName = (context.HttpContext.User.Identity as WindowsIdentity)?.Name.ToUpperInvariant();

            if (adUserName == null)
            {
                context.Result = CreateResult(HttpStatusCode.Unauthorized, "No windows identity has been found for the HTTP context.");
                return;
            }

            // If no user session is found then creates it by authenticating against the Cross Cutting services
            var session = UserSessionCacheRepository.Get(adUserName);
            if (session == null)
            {
                var authenticateResult = await CrossCuttingUserServices.AuthenticateUserForApp(adUserName);
                if (authenticateResult.IsNotSuccess)
                {
                    context.Result = CreateResult(HttpStatusCode.Unauthorized, authenticateResult.Reason);
                    return;
                }

                session = CreateUserSession(_userProfileDateType, authenticateResult.Data);
                UserSessionCacheRepository.Set(session.UserId, session);
                DiagBase.Logs.GetValue(nameof(LoggersBase.Controllers))?.LogInformation($"User {session.UserFirstName} {session.UserLastName} ({session.UserId}) authenticated successfully.");
            }

            // Checks the permission
            if (_permission.IsNotNullOrEmpty() && !session.Permissions.Contains(_permission))
            {
                context.Result = CreateResult(HttpStatusCode.Forbidden, "The user has no permission to access the resource.");
                return;
            }

            // Adds the user session to the context to get it in the Controllers
            context.HttpContext.Items.AddOrUpdate(nameof(UserSession), session);
        }

        #endregion IAuthorizationFilter

        #region Methods (Protected)

        /// <summary>
        /// Creates a response result with an HTTP code and a reason.
        /// The reason will be displayed only in some environments.
        /// </summary>
        /// <param name="code">The HTTP code to set in the response.</param>
        /// <param name="reason">The reason to give to the user (only in some environments).</param>
        /// <returns>The created ContentResult.</returns>
        protected ContentResult CreateResult(HttpStatusCode code, string? reason)
        {
            reason = !EnvironmentHelper.IsEnvironment(EnvironmentEnum.Prod) ? reason : null;
            return new ContentResult { StatusCode = (int)code, Content = reason };
        }

        #endregion Methods (Protected)

        #region Methods (Helpers)

        /// <summary>
        /// Converts a user app profile DTO to an user session.
        /// </summary>
        /// <param name="profileDataType">The type of the profile data to convert.</param>
        /// <param name="profile">The profile to convert.</param>
        /// <returns>The user session to create.</returns>
        private UserSession CreateUserSession(Type profileDataType, UserAppProfileDto profile)
        {
            var userSession = typeof(UserSession<>).MakeGenericType(profileDataType).New<UserSession>();
            userSession.UserId = profile.UserId;
            userSession.UserFirstName = profile.UserFirstName;
            userSession.UserLastName = profile.UserLastName;
            userSession.UserPhone = profile.UserPhone;
            userSession.UserEmail = profile.UserEmail;
            userSession.UserPicture = profile.UserPicture;
            userSession.Permissions = new HashSet<string>(profile.Permissions);
            userSession.ProfileData = DeserializeProfileData(profileDataType, profile.ProfileDataJson);
            userSession.AppEnvironments = profile.AppEnvironments;
            userSession.AppsAvailable = profile.AppsAvailable;

            return userSession;
        }

        /// <summary>
        /// Deserializes the profile data JSON into the needed type.
        /// </summary>
        /// <param name="profileDataType">The type of the profile data to convert.</param>
        /// <param name="profileDataJson">The rax JSON of the profile data.</param>
        /// <returns>The deserialized project of the needed type if success or an empty object if none.</returns>
        private static object DeserializeProfileData(Type profileDataType, string profileDataJson)
            {
                try
                {
                    return JsonSerializer.Deserialize(profileDataJson, profileDataType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } })
                           ?? profileDataType.New();
                }
                catch (Exception)
                {
                    return profileDataType.New();
                }
            }

        #endregion Methods (Helpers)
    }
}