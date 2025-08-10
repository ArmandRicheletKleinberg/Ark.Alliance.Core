namespace Ark
{
    /// <summary>
    /// This helper class extends the Stream object.
    /// </summary>
    public static class StreamExtensibility
    {
        #region Methods (Helpers)

        /// <summary>
        /// Reads all bytes from a stream.
        /// </summary>
        /// <param name="stream">The stream to read all bytes from.</param>
        /// <returns>All the bytes from the string or null if failure.</returns>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var bytes = new byte[stream.Length];
            stream.ReadExactly(bytes, 0, Convert.ToInt32(stream.Length));

            return bytes;
        }

        /// <summary>
        /// Reads all bytes from a stream asynchronously.
        /// </summary>
        /// <param name="stream">The stream to read all bytes from.</param>
        /// <returns>All the bytes from the string or null if failure.</returns>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var bytes = new byte[stream.Length];
            await stream.ReadExactlyAsync(bytes.AsMemory(0, Convert.ToInt32(stream.Length)));

            return bytes;
        }


        /// <summary>
        /// Reads all byte from a stream.
        /// Some streams can't use the seek method. 
        /// </summary>
        /// <param name="stream">The stream to read all bytes from.</param>
        /// <returns>All the bytes from the string</returns>
        public static byte[] ReadFully(this Stream stream)
        {
            var buffer = new byte[16 * 1024];
            using var ms = new MemoryStream();
            int read;

            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
        #endregion Methods (Helpers)
    }
}