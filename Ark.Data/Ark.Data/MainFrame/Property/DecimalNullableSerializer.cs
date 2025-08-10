using System;
using System.Globalization;
using System.Linq;

namespace Ark.Data
{
    /// <inheritdoc />
    internal class MainFramePropertyDecimalNullableSerializer<TMfo> : MainFramePropertySerializer<TMfo, decimal?>
        where TMfo : class, new()
    {
        /// <inheritdoc />
        internal override string ConvertValueToString(decimal? value)
        {
            if (!value.HasValue)
                return new string(' ', Length);

            var decimalLength = Attribute.DecimalLength;
            var integerLength = Length - decimalLength;
            var leftPart = (int)value;
            if (value >= (decimal)Math.Pow(10, integerLength))
                throw new Exception($"The decimal property {PropertyName} of the mainframe object {typeof(TMfo).Name} accepts only {integerLength}-figures integer and the integer value {leftPart} has more figures.");

            var zeroToAddRight = integerLength - leftPart.ToString().Length;

            value = decimal.Round(value.Value, decimalLength, MidpointRounding.AwayFromZero);
            if (((int)value).ToString().Length > leftPart.ToString().Length)
                value -= (decimal)Math.Pow(0.1, decimalLength);

            if (Attribute.IsNotDefault && value == 0m)
                throw new Exception($"The decimal property {PropertyName} of the mainframe object {typeof(TMfo).Name} is equal to 0 and the property is not equal to zero is set to true.");

            return (new string('0', zeroToAddRight) + new string(value.Value.ToString(CultureInfo.InvariantCulture).Where(char.IsDigit).ToArray())).PadRight(Length, '0');
        }

        /// <inheritdoc />
        internal override decimal? ConvertStringToValue(string data)
        {
            if (data.IsNullOrWhiteSpace())
                return null;

            var decimalLength = Attribute.DecimalLength;
            var integerLength = Length - decimalLength;
            var integerString = data.Substring(0, integerLength).Trim();
            var integerPart = integerString.Length > 0 ? integerString.ToInt32(-1) : 0; // empty (spacing) becomes 0
            if (integerPart < 0)
                throw new Exception($"The decimal property {PropertyName} of the mainframe object {typeof(TMfo).Name} can not parse the integer value part : \"{data}\".");
            var decimalString = data.Substring(integerLength, decimalLength).Trim();
            var decimalPart = decimalString.Length > 0 ? decimalString.ToInt32(-1) : 0; // empty (spacing) becomes 0
            if (decimalPart < 0)
                throw new Exception($"The decimal property {PropertyName} of the mainframe object {typeof(TMfo).Name} can not parse the decimal value part : \"{data}\".");

            var value = integerPart + decimalPart / (decimal)Math.Pow(10, decimalLength);

            if (Attribute.IsNotDefault && value == 0m)
                throw new Exception($"The decimal property {PropertyName} of the mainframe object {typeof(TMfo).Name} is equal to 0 and the property is not equal to zero is set to true.");

            return value;
        }

        /// <inheritdoc />
        internal override int GetStringDataLength()
            => Length;
    }
}