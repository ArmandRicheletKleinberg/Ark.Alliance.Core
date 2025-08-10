using System;

namespace Ark.Net.CrossCutting
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute allows to allow an application to use this dynamic mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DynamicMappingAppAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="DynamicMappingAppAttribute"/> instance.
        /// </summary>
        public DynamicMappingAppAttribute(string allowedApp)
        {
            AllowedApp = allowedApp;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The allowed application for this dynamic mapping.
        /// </summary>
        public string AllowedApp { get; }

        #endregion Properties (Public)
    }
}