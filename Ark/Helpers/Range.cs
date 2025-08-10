using Newtonsoft.Json;

namespace Ark
{
    /// <summary>
    /// This class is used to specify a range of numeric values.
    /// </summary>
    public class Range<TValue>
        where TValue : IComparable
    {
        #region Constructors

        /// <summary>
        /// Creates a Range instance.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        public Range(TValue minValue, TValue maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        #endregion Constructors

        #region Properties (Models)

        /// <summary>
        /// The minimum value.
        /// </summary>
        public TValue MinValue { get; set; }

        /// <summary>
        /// The maximum value.
        /// </summary>
        public TValue MaxValue { get; set; }

        #endregion Properties (Models)

        #region Properties (Validation)

        /// <summary>
        /// Whether this range is valid (min less or equal to max).
        /// </summary>
        [JsonIgnore]
        public bool IsValid => MinValue.CompareTo(MaxValue) <= 0;

        #endregion Properties (Validation)

        #region Methods (Public)

        /// <summary>
        /// Whether the value is contained within the range (equals counts).
        /// </summary>
        /// <param name="val">The value to check within the range.</param>
        /// <returns>True if is contained, false otherwise.</returns>
        public bool Contains(TValue val)
        {
            return MinValue.CompareTo(val) <= 0 && MaxValue.CompareTo(val) >= 0;
        }

        /// <summary>
        /// Gets the range intersection between this range and an another one.
        /// </summary>
        /// <param name="range">The another range to intersect.</param>
        /// <returns>A range as the intersection of both ranges.</returns>
        public Range<TValue> Intersect(Range<TValue> range)
        {
            return new Range<TValue>(
                ((IComparable)MinValue).CompareTo(range.MinValue) >= 0 ? MinValue : range.MinValue,
                ((IComparable)MaxValue).CompareTo(range.MaxValue) <= 0 ? MaxValue : range.MaxValue);
        }

        /// <summary>
        /// Gets the range union between this range and an another one.
        /// </summary>
        /// <param name="range">The another range to union.</param>
        /// <returns>A range as the union of both ranges.</returns>
        public Range<TValue> Union(Range<TValue> range)
        {
            return new Range<TValue>(
                ((IComparable)MinValue).CompareTo(range.MinValue) <= 0 ? MinValue : range.MinValue,
                ((IComparable)MaxValue).CompareTo(range.MaxValue) >= 0 ? MaxValue : range.MaxValue);
        }

        /// <summary>
        /// Clones this range to another exact same range instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public Range<TValue> Clone()
        {
            return new Range<TValue>(MinValue, MaxValue);
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>The string representation of the object.</returns>
        public override string ToString()
        {
            if (MinValue is DateTime) return $"{MinValue:G} - {MaxValue:G}";

            return $"{MinValue} - {MaxValue}";
        }

        #endregion Methods (Public)
    }
}
