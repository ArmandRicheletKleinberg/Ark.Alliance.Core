using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Ark.Data
{
    /// <inheritdoc />
    internal class MainFramePropertyInt32Serializer<TMfo> : MainFramePropertySerializer<TMfo, int>
        where TMfo : class, new()
    {
        /// <inheritdoc />
        internal override string ConvertValueToString(int value)
        {
            if (Attribute.IsHexInteger)
            {
                if (Length > 4)
                    throw new Exception($"The integer property {PropertyName} of the mainframe object {typeof(TMfo).Name} accepts max 4 bytes length and is {Length}.");

                return value.ToBytes().Skip(4 - Length).Take(Length).ToArray().ToAscii();
            }

            if (value >= (int)Math.Pow(10, Length))
                throw new Exception($"The integer property {PropertyName} of the mainframe object {typeof(TMfo).Name} accepts only {Length}-figures integer and the integer value {value} has more figures.");

            if (Attribute.IsNotDefault && value == 0)
                throw new Exception($"The integer property {PropertyName} of the mainframe object {typeof(TMfo).Name} is equal to 0 and the property is not equal to zero is set to true.");

            return value.ToString($"d{Length}");
        }

        /// <inheritdoc />
        internal override int ConvertStringToValue(string data)
        {
            if (Attribute.IsHexInteger)
                return new byte[4 - Length].Concat(data.ToBytes(Encoding.ASCII)).ToArray().ToInt32();

            data = data.Trim();
            if (data.Length == 0)
                return 0;

            var success = int.TryParse(data.Trim(), NumberStyles.None, null, out var result);
            if (!success)
                throw new Exception($"Unable to parse the property {PropertyName} of the mainframe object {typeof(TMfo).Name}, the string received \"{data}\" could not be parsed in a {Length}-figures long integer");

            if (Attribute.IsNotDefault && result == 0)
                throw new Exception($"The integer property {PropertyName} of the mainframe object {typeof(TMfo).Name} is equal to 0 and the property is not equal to zero is set to true.");

            return result;
        }

        /// <inheritdoc />
        internal override int GetStringDataLength()
            => Length;
    }
}