using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Whether this column has this default SQL value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HasDefaultValueSqlAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="HasDefaultValueAttribute"/> instance.
        /// </summary>
        /// <param name="defaultValueSql">The default SQL value.</param>
        public HasDefaultValueSqlAttribute(string defaultValueSql)
        {
            DefaultValueSql = defaultValueSql;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The default SQL value.
        /// </summary>
        public string DefaultValueSql { get; set; }

        #endregion Properties (Public)
    }
}