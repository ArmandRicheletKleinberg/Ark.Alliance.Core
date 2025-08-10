using System.Globalization;

namespace Ark
{
    /// <summary>
    /// + Formatting helpers for <see cref="decimal"/> values.
    /// - Culture-specific; only implements French formatting.
    /// </summary>
    public static class Decimal
    {
        /// <summary>
        /// Converts a decimal to a french string.
        /// Ex : value = 16325.62m; => 16325,62
        /// </summary>
        /// <param name="value">The double to convert.</param>
        /// <param name="numberOfDecimal">Facultatif Nombre de décimale pour arrondir</param>
        /// <returns>The converted Int32.</returns>
        public static string ToFrenchString(this decimal value, int numberOfDecimal = 2)
        {
            String format = "F";
            value = decimal.Round(value, numberOfDecimal, MidpointRounding.AwayFromZero);
            format += numberOfDecimal.ToString();

            return value.ToString(format, CultureInfo.CreateSpecificCulture("fr-FR"));
        }

    }
}
