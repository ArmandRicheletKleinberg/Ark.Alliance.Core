namespace Ark
{
    /// <summary>
    /// Extensibility method for int and UInt32.
    /// </summary>
    public static class UInt32Extensibility
    {
        /// <summary>
        /// Converts an integer to a byte array given the needed endian type.
        /// </summary>
        /// <param name="integer">This integer.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian.</param>
        /// <returns>The byte array filled if success, null otherwise.</returns>
        public static byte[] ToBytes(this uint integer, bool isLittleEndian = true)
        {
            var bytes = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian != isLittleEndian) bytes = bytes.Reverse().ToArray();

            return bytes;
        }
    }
}