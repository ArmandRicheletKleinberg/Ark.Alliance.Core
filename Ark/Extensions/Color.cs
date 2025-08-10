using System.Drawing;

namespace Ark
{
    /// <summary>
    /// The extensions method for a <see cref="System.Drawing.Color"/>.
    /// </summary>
    public static class ColorExtensions
    {
        #region Methods (Convert)

        /// <summary>
        /// Converts a color to the CSS hexadecimal string.
        /// Either #RRGGBB or #AARRGGBB.
        /// </summary>
        /// <param name="color">The color to convert to CSS string.</param>
        /// <returns>The converted CSS string.</returns>
        public static string ToCssString(this Color color)
            => color.A != byte.MaxValue
                ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        #endregion Methods (Convert)
    }
}