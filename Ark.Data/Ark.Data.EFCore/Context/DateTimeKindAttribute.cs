using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace Ark.Data.EFCore
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute is to be set on a date time property of an entity to specify the kind of datetime to be stored in the database (UTC or local).
    /// If the whole database is managed into a specific kind, this attribute override the database default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeKindAttribute : Attribute
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="T:Ark.Data.EFCore.DateTimeKindAttribute" /> instance.
        /// </summary>
        /// <param name="kind">The kind of date time UTC or local.</param>
        public DateTimeKindAttribute(DateTimeKind kind)
        {
            Kind = kind;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The kind of date time UTC or local.
        /// </summary>
        public DateTimeKind Kind { get; }

        #endregion Properties (Public)
    }
}