namespace Ark
{
    /// <summary>
    /// Extesibility method for double and Double.
    /// </summary>
    public static class DoubleExtensibility
    {
        /// <summary>
        /// Converts a double to Int32.
        /// </summary>
        /// <param name="value">The double to convert.</param>
        /// <returns>The converted Int32.</returns>
        public static int ToInt32(this double value)
        {
            return Convert.ToInt32(value);
        }
    }
}