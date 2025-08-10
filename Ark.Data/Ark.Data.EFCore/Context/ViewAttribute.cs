using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// A view must be set on entity used as database views.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="ViewAttribute"/> instance.
        /// </summary>
        /// <param name="viewName">The name of the view in the database.</param>
        public ViewAttribute(string viewName)
        {
            ViewName = viewName;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The name of the view in the database.
        /// </summary>
        public string ViewName { get; set; }

        #endregion Properties (Public)
    }
}