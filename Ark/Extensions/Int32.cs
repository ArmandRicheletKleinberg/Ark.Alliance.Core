namespace Ark
{
    /// <summary>
    /// Extensibility method for int and Int32.
    /// </summary>
    public static class Int32Extensibility
    {
        #region Methods (Conversion)

        /// <summary>
        /// Converts an integer to a 24 bites byte array given the needed endian type.
        /// </summary>
        /// <param name="integer">This integer.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian.</param>
        /// <returns>The byte array filled if success, null otherwise.</returns>
        public static byte[] To24BitBytes(this int integer, bool isLittleEndian = true)
        {
            var bytes = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian != isLittleEndian) bytes = bytes.Reverse().ToArray();

            return isLittleEndian ? bytes.Take(3).ToArray() : bytes.Skip(1).Take(3).ToArray();
        }

        /// <summary>
        /// Converts an integer to a byte array given the needed endian type.
        /// </summary>
        /// <param name="integer">This integer.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian.</param>
        /// <returns>The byte array filled if success, null otherwise.</returns>
        public static byte[] ToBytes(this int integer, bool isLittleEndian = true)
        {
            var bytes = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian != isLittleEndian) bytes = bytes.Reverse().ToArray();

            return bytes;
        }

        /// <summary>
        /// Converts an integer to an hex representation of the integer.
        /// </summary>
        /// <param name="integer">The integer to convert.</param>
        /// <param name="hexLength">The length of the string representation if needed.</param>
        /// <returns>The hex representation of the integer.</returns>
        public static string ToHex(this int integer, int? hexLength = null)
        {
            return hexLength.HasValue ? integer.ToString($"X{hexLength.Value}") : integer.ToString("X");
        }

        #endregion Methods (Conversion)
    }
}