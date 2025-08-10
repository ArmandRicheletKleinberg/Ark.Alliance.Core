using System;

namespace Ark.Data
{
    internal class MainFramePropertyStringSerializer<TMfo> : MainFramePropertySerializer<TMfo, string>
        where TMfo : class, new()
    {
        internal override string ConvertValueToString(string value)
        {
            if (Attribute.IsNotDefault && value.Trim().Length == 0)
                throw new Exception($"The string property {PropertyName} of the mainframe object {typeof(TMfo).Name} is empty and the property is not empty is set to true.");

            return value.ToFixedSize(Length);
        }


        internal override string ConvertStringToValue(string data)
        {
            if (Attribute.IsNotDefault && data.Trim().Length == 0)
                throw new Exception($"The string property {PropertyName} of the mainframe object {typeof(TMfo).Name} is empty and the property is not empty is set to true.");

            return data.Trim();
        }


        internal override int GetStringDataLength()
            => Length;
    }
}