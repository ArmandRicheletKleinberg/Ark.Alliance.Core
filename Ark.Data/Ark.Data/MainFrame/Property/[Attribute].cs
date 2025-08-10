using System;
using System.Runtime.CompilerServices;

namespace Ark.Data
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute is to be set on inner properties of a mainframe object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MainFramePropertyAttribute : Attribute
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="T:Ark.Data.MainFramePropertyAttribute" /> instance.
        /// It auto sets the order by setting it to the caller line number.
        /// The order is important for the serialization.
        /// </summary>
        public MainFramePropertyAttribute([CallerLineNumber] int order = 0)
        {
            Order = order;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The length of data to serialize.
        /// The length is not necessarily the serialization length. It depends on the property type.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Only for decimal values.
        /// The number of the decimal figures (figures beyond the dot).
        /// </summary>
        public int DecimalLength { get; set; }

        /// <summary>
        /// Only for array.
        /// The number of occurrences of the data in an array.
        /// </summary>
        public int ArrayOccurrencesNumber { get; set; } = 1;

        /// <summary>
        /// Only for DateTime.
        /// The C# format to format the datetime ie yyMMddHHmmss.
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// The order of the property in the mainframe object.
        /// It is auto generated in the constructor.
        /// It is really important as the order of the properties are used in the serialization process.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Whether an integer should be serialized/deserialized into a hex string instead of normal string format.
        /// </summary>
        public bool IsHexInteger { get; set; }

        /// <summary>
        /// Check the value to be not to be not the default value. Empty for a string, 0 for a decimal or an integer.
        /// </summary>
        public bool IsNotDefault { get; set; }


        #endregion Properties (Public)
    }
}