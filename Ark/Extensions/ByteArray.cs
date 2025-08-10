using System.Text;

namespace Ark
{
    /// <summary>
    /// + Provides endian-aware conversions for primitive types and strings.
    /// - Does not guard against incomplete data segments.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.bitconverter"/>
    /// </summary>
    public static class ByteArrayExtensibility
    {
        #region Methods (Convert)

        /// <summary>
        /// Converts a byte array to a short given the needed endian type.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="startOffset">The start offset to get only the needed bytes in the array.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian.</param>
        /// <returns>The value converted or default value if not valid.</returns>
        public static short ToInt16(this byte[] bytes, int startOffset = 0, bool isLittleEndian = true)
        {
            return BitConverter.ToInt16(GetBytes(bytes, startOffset, 2, isLittleEndian), 0);
        }

        /// <summary>
        /// Converts a byte array to an ushort given the needed endian type.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="startOffset">The start offset to get only the needed bytes in the array.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian.</param>
        /// <returns>The value converted or default value if not valid.</returns>
        public static ushort ToUInt16(this byte[] bytes, int startOffset = 0, bool isLittleEndian = true)
        {
            return BitConverter.ToUInt16(GetBytes(bytes, startOffset, 2, isLittleEndian), 0);
        }

        /// <summary>
        /// Converts a byte array to an int given the needed endian type.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="startOffset">The start offset to get only the needed bytes in the array.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian.</param>
        /// <returns>The value converted or default value if not valid.</returns>
        public static int ToInt32(this byte[] bytes, int startOffset = 0, bool isLittleEndian = true)
        {
            return BitConverter.ToInt32(GetBytes(bytes, startOffset, 4, isLittleEndian), 0);
        }

        /// <summary>
        /// Converts a byte array to an uint given the needed endian type.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="startOffset">The start offset to get only the needed bytes in the array.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian.</param>
        /// <returns>The value converted or default value if not valid.</returns>
        public static uint ToUInt32(this byte[] bytes, int startOffset = 0, bool isLittleEndian = true)
        {
            return BitConverter.ToUInt32(GetBytes(bytes, startOffset, 4, isLittleEndian), 0);
        }

        /// <summary>
        /// Converts a byte array to a long given the needed endian type.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="startOffset">The start offset to get only the needed bytes in the array.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian.</param>
        /// <returns>The value converted or default value if not valid.</returns>
        public static long ToInt64(this byte[] bytes, int startOffset = 0, bool isLittleEndian = true)
        {
            return BitConverter.ToInt64(GetBytes(bytes, startOffset, 8, isLittleEndian), 0);
        }

        /// <summary>
        /// Converts a byte array to an ulong given the needed endian type.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="startOffset">The start offset to get only the needed bytes in the array.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian.</param>
        /// <returns>The value converted or default value if not valid.</returns>
        public static ulong ToUInt64(this byte[] bytes, int startOffset = 0, bool isLittleEndian = true)
        {
            return BitConverter.ToUInt64(GetBytes(bytes, startOffset, 8, isLittleEndian), 0);
        }

        /// <summary>
        /// Converts a byte array to a UTF-8 encoded string.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <param name="startOffset">Optional offset in <paramref name="bytes"/>.</param>
        /// <param name="length">Optional length of the segment. Defaults to the remaining length.</param>
        /// <returns>Decoded string.</returns>
        public static string ToUtf8(this byte[] bytes, int startOffset = 0, int length = 0)
        {
            length = length == 0 ? bytes.Length - startOffset : length;
            return Encoding.UTF8.GetString(bytes, startOffset, length);
        }

        /// <summary>
        /// Converts a byte array to a UTF8 encoded string.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="startOffset">The start offset to get only the needed bytes in the array.</param>
        /// <param name="length">The length of the types to get.</param>
        /// <returns>The value converted or default value if not valid.</returns>
        public static string ToAscii(this byte[] bytes, int startOffset = 0, int length = 0)
        {
            length = length == 0 ? bytes.Length : length;

            return Encoding.ASCII.GetString(bytes, startOffset, length);
        }

        /// <summary>
        /// Converts a byte array to BASE64 string.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <returns>The value converted in BASE64.</returns>
        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts a byte array to an ushort given the needed endian type.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="startOffset">The start offset to get only the needed bytes in the array.</param>
        /// <param name="length">The length to take into account.</param>
        /// <param name="isLittleEndian">Whether integer should be converted into big or little endian or not taken into account.</param>
        /// <returns>The value converted or default value if not valid.</returns>
        private static byte[] GetBytes(byte[] bytes, int startOffset, int length, bool? isLittleEndian)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length < startOffset + length)
                throw new IndexOutOfRangeException("The bytes to convert are outside the byte array.");

            var target = new byte[length];
            Array.Copy(bytes, startOffset, target, 0, length);

            if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(target);

            return target;
        }


        #endregion Methods (Convert)
    }
}
