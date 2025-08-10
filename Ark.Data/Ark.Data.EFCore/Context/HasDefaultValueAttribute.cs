using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Specifies a default value applied when the column is omitted in INSERT statements.
    /// + Simplifies migrations for non-nullable fields.
    /// - Hard-coded defaults may drift from evolving domain rules.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/modeling/relational/default-values"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HasDefaultValueAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="HasDefaultValueAttribute"/> instance.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public HasDefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// Gets or sets the default value expressed as an <see cref="object"/>.
        /// + Allows flexible configuration for scalar and complex types.
        /// - Consumers must cast to the concrete type before use.
        /// </summary>
        public object DefaultValue { get; set; }

        #endregion Properties (Public)
    }
}