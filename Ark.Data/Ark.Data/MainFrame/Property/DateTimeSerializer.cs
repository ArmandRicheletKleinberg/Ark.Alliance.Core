using System;
using System.Globalization;

namespace Ark.Data
{
    /// <inheritdoc />
    internal class MainFramePropertyDateTimeSerializer<TMfo> : MainFramePropertySerializer<TMfo, DateTime>
        where TMfo : class, new()
    {
        /// <inheritdoc />
        internal override string ConvertValueToString(DateTime value)
            => value.ToString(Attribute.DateTimeFormat);

        /// <inheritdoc />
        internal override DateTime ConvertStringToValue(string data)
        {
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