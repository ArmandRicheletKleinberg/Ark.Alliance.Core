using System;
using System.Collections.Generic;

namespace Ark.AspNetCore
{
    /// <summary>
    /// This manager is used to manage the user sessions.
    /// </summary>
    public class UserSessionManager
    {
        #region Static

        internal static Type ProfileDataType;

        internal static int SessionTimeout;

        internal static void Init(Type profileDataType, int sessionTimeout)
        {
            ProfileDataType = profileDataType;
            SessionTimeout = sessionTimeout;
        }

        #endregion Static
    }
}