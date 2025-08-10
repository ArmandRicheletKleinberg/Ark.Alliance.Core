using Ark.Pattern.Mvvm;
using System;

namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO contains the info about an app that the user has access to.
    /// </summary>
    public class UserAppDto
    {
        #region Properties (Public)

        /// <summary>
        /// The app identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The icon to display for this application.
        /// </summary>
        public WebIconEnum Icon { get; set; } = WebIconEnum.QuestionCircle;

        /// <summary>
        /// The app name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A little description about the app in the user language.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The last time a user has connected the application.
        /// </summary>
        public DateTime? LastConnectionTime { get; set; }

        /// <summary>
        /// The URL to access the frontend application.
        /// </summary>
        public string FrontUrl { get; set; }

        /// <summary>
        /// The URL to access the backend application.
        /// </summary>
        public string BackUrl { get; set; }

        /// <summary>
        /// Indicates if the application has archives. 
        /// </summary>
        public bool HasArchives { get; set; }

        #endregion Properties (Public)
    }
}