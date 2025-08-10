using System;
using System.Globalization;

namespace Ark.Data
{
    /// <inheritdoc />
    internal class MainFramePropertyDateTimeNullableSerializer<TMfo> : MainFramePropertySerializer<TMfo, DateTime?>
        where TMfo : class, new()
    {
        /// <inheritdoc />
        internal override string ConvertValueToString(DateTime? value)
            => value == null ? new string(' ', GetStringDataLength()) : value.Value.ToString(Attribute.DateTimeFormat);

        /// <inheritdoc />
        internal override DateTime? ConvertStringToValue(string data)
        {
            data = data.Trim();
            if (data.Length == 0)
                return null;

            var success = DateTime.TryParseExact(data.Trim(), Attribute.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result);
            if (!success)
                throw new Exception($"Unable to parse the property {PropertyName} of the mainframe object {typeof(TMfo).Name}, the string received \"{data}\" could not be parsed in a valid date time with format {Attribute.DateTimeFormat}");

            return result;
        }

        /// <inheritdoc />
        internal override int GetStringDataLength()
            => Attribute.DateTimeFormat.Length;
    }
}