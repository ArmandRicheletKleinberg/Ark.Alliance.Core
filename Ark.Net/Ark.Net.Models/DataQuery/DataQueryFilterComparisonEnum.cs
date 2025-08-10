namespace Ark.Net.Models
{
    /// <summary>
    /// This enumeration lists all the different comparison that could be done on a data query filtering.
    /// Specific to the type of the property to filter on.
    /// </summary>
    public enum DataQueryFilterComparisonEnum
    {
        /// <summary>
        /// No filter comparison is specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The comparison check the equality between the property and the value.
        /// Could be used by all types.
        /// </summary>
        Equals = 10,

        /// <summary>
        /// The comparison check the non equality between the property and the value.
        /// Could be used by all types.
        /// </summary>
        NotEquals = 11,

        /// <summary>
        /// Whether a string property starts with a specified text.
        /// Only for string type.
        /// </summary>
        StartsWith = 20,

        /// <summary>
        /// Whether a string property contains a specified text.
        /// Only for string type.
        /// </summary>
        Contains = 21,

        /// <summary>
        /// Whether a string property ends with a specified text.
        /// Only for string type.
        /// </summary>
        EndsWith = 22,

        /// <summary>
        /// Whether a property should be greater than a value.
        /// Only for numeric and date time types.
        /// </summary>
        GreaterThan = 30,

        /// <summary>
        /// Whether a property should be greater or equal than a value.
        /// Only for numeric and date time types.
        /// </summary>
        GreaterOrEqualThan = 31,

        /// <summary>
        /// Whether a property should be less than a value.
        /// Only for numeric and date time types.
        /// </summary>
        LessThan = 32,

        /// <summary>
        /// Whether a property should be less or equal than a value.
        /// Only for numeric and date time types.
        /// </summary>
        LessOrEqualThan = 33
    }
}