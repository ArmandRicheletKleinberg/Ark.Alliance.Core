using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Whether this column has this default decimal value.
    /// Mandatory because decimal can not be passed as arguments to attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HasDefaultDecimalValueAttribute : HasDefaultValueAttribute
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="HasDefaultDecimalValueAttribute"/> instance.
        /// </summary>
        /// <param name="defaultDecimalValue">The default decimal value in string because .NET attribute don't allow decimal in attributes.</param>
        public HasDefaultDecimalValueAttribute(string defaultDecimalValue)
            : base(defaultDecimalValue.ToDecimal())
        { }

        #endregion Constructors
    }
}