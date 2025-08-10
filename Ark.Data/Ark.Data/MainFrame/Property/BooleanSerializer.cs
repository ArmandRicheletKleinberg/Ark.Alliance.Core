namespace Ark.Data
{
    /// <inheritdoc />
    internal class MainFramePropertyBooleanSerializer<TMfo> : MainFramePropertySerializer<TMfo, bool>
        where TMfo : class, new()
    {
        /// <inheritdoc />
        internal override string ConvertValueToString(bool value)
            => value ? "O" : "N";

        /// <inheritdoc />
        internal override bool ConvertStringToValue(string data)
            => data[0] == 'O' || data[0] == 'Y' || data[0] == '1' || data[0] == 'o' || data[0] == 'y';

        /// <inheritdoc />
        internal override int GetStringDataLength()
            => 1;
    }
}