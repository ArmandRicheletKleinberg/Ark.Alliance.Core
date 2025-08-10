using Ark;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Ark.Net.Http
{
    /// <inheritdoc />
    /// <summary>
    /// This is the base class for all HTTP REST API service client repository that needs to be impersonated as a specific user Windows identity.
    /// It is used by the Blazor on the server to impersonate the call to avoid to use the service account gMSA to call the API server but the user that calls the Blazor server.
    /// </summary>
    public abstract class WindowsIdentityImpersonateHttpRepositoryBase : HttpRepositoryBase
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="WindowsIdentityImpersonateHttpRepositoryBase" /> instance.
        /// </summary>
        /// <param name="getWindowsIdentityFunc">The method to get the Windows identity to impersonate the HTTP client.</param>
        protected WindowsIdentityImpersonateHttpRepositoryBase(Func<WindowsIdentity> getWindowsIdentityFunc)
        {
            GetWindowsIdentityFunc = getWindowsIdentityFunc;
        }

        #endregion Constructors

        #region Properties (Protected)

        /// <summary>
        /// The method to get the Windows identity to impersonate the HTTP client.
        /// </summary>
        protected Func<WindowsIdentity> GetWindowsIdentityFunc { get; }

        #endregion Properties (Protected)

        #region Methods (Override)

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        protected override Task<Result<TResponseData>> Request<TRequestData, TResponseData>(string urlRelativePath, HttpMethod httpMethod, TRequestData body = default,
            Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
        {
            var windowsIdentity = GetWindowsIdentityFunc();
            if (windowsIdentity == null)
                return base.Request<TRequestData, TResponseData>(urlRelativePath, httpMethod, body, queryParameters, headers);

            return WindowsIdentity.RunImpersonated(windowsIdentity.AccessToken, ()
                => base.Request<TRequestData, TResponseData>(urlRelativePath, httpMethod, body, queryParameters, headers));
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        protected override Task<Result<string>> DownloadString(string url)
        {
            var windowsIdentity = GetWindowsIdentityFunc();
            if (windowsIdentity == null)
                return base.DownloadString(url);

            return WindowsIdentity.RunImpersonated(windowsIdentity.AccessToken, ()
                => base.DownloadString(url));
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        protected override Task<Result<byte[]>> DownloadBytes(string url)
        {
            var windowsIdentity = GetWindowsIdentityFunc();
            if (windowsIdentity == null)
                return base.DownloadBytes(url);

            return WindowsIdentity.RunImpersonated(windowsIdentity.AccessToken, ()
                => base.DownloadBytes(url));
        }

        #endregion Methods (Override)
    }
}