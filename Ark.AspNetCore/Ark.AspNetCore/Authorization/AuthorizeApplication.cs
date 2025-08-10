#nullable enable

using System;


using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ark.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute must be set on resources that need to be accessed by one or more specific applications defined in the Authorization/Applications section of the  appsettings.json file.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class AuthorizeApplicationAttribute : TypeFilterAttribute
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="AuthorizeAttribute" /> instance.
        /// By default, this allows all the applications defined in the Authorization/Applications section of the appsettings.json file.
        /// But this can be restrain to some allowed applications which will need to be defined.
        /// </summary>
        /// <param name="allowedApplications">The applications allowed to access this resource, if none then all the applications defined in the settings.</param>
        public AuthorizeApplicationAttribute(params string[] allowedApplications)
            : base(typeof(AuthorizeApplicationFilter))
        {
            Arguments = new object[] { allowedApplications };
        }

        #endregion Constructors
    }

    /// <inheritdoc />
    /// <summary>
    /// This is the base class for the application authorization filter used in the DSS application.
    /// </summary>
    public class AuthorizeApplicationFilter : IAuthorizationFilter
    {
        #region Static

        /// <summary>
        /// The application 
        /// Keeps this in static with lazy loading for performance purpose.
        /// </summary>
        private static Dictionary<string, string[]> _applicationAllowedIdentities = new();

        #endregion Static

        #region Fields

        /// <summary>
        /// The permission required to access this resource.
        /// </summary>
        private readonly string[] _allowedApplications;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="AuthorizeFilter"/> instance.
        /// </summary>
        /// <param name="allowedApplications">The applications allowed to access this resource, if none then all the applications defined in the settings.</param>
        public AuthorizeApplicationFilter(string[] allowedApplications)
        {
            _allowedApplications = allowedApplications;
        }

        #endregion Constructor

        #region IAuthorizationFilter

        /// <inheritdoc />
        /// <summary>
        /// Checks the authorization.
        /// </summary>
        /// <param name="context">The HTTP context to search for data.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Lazy gets the applications authorization from the settings
            if (_applicationAllowedIdentities.Count == 0)
            {
                var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
                _applicationAllowedIdentities = configuration?.GetSection("Authorization").GetSection("Applications").GetChildren()
                                                         .ToDictionary(a => a.Key, a => (a.Value ?? string.Empty).ToLower().Split(';'))
                                             ?? new Dictionary<string, string[]>();
            }


            // First checks that the caller has added an "Application" header with the name of the application to access this resource
            var applicationName = context.HttpContext.Request.Headers.GetValue("Application").FirstOrDefault();
            if (applicationName == null)
            {
                context.Result = CreateResult(HttpStatusCode.Unauthorized, "To access this resource, an application must be provided.");
                return;
            }
            if (!_applicationAllowedIdentities.ContainsKey(applicationName))
            {
                context.Result = CreateResult(HttpStatusCode.Unauthorized, $"The application {applicationName} can not access this resource, it must be defined in the Authorization/Applications section of the appsettings.json file.");
                return;
            }
            if (_allowedApplications.HasAnElement() && !_allowedApplications.Contains(applicationName))
            {
                context.Result = CreateResult(HttpStatusCode.Unauthorized, $"The application {applicationName} can not access this resource which is only available to these defined applications : {string.Join(", ", _allowedApplications)}.");
                return;
            }



            // Then checks the identity of the user (technical account used by the other application) which must be one of those defined in the settings.
            string? adUserName = null;
            if (OperatingSystem.IsWindows())
                adUserName = (context.HttpContext.User.Identity as WindowsIdentity)?.Name.ToLowerInvariant();
            if (adUserName == null)
            {
                context.Result = CreateResult(HttpStatusCode.Unauthorized, "No windows identity has been found for the HTTP context.");
                return;
            }
            if (!_applicationAllowedIdentities.GetValue(applicationName, Array.Empty<string>()).Contains(adUserName))
                context.Result = CreateResult(HttpStatusCode.Unauthorized, $"The identity {adUserName} of the application {applicationName} is not allowed to access this resource, specify the technical account identity with domain name of the application in the ApplicationAuthorization section of the appsettings.json file");
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
    }
}