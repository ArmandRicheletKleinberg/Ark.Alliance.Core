using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

#nullable enable

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ark
{
    /// <summary>
    /// This class is used to extend <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        #region Static

        /// <summary>
        /// The conversion table is used by the <see cref="ToObject{TData}"/> method.
        /// Lazy created.
        /// </summary>
    private static Dictionary<Type, Func<string, object?, object?>?>? _conversionTable;
        /// <summary>
        /// The conversion table is used by the <see cref="ToObject{TData}"/> method.
        /// Lazy created.
        /// </summary>
    private static Dictionary<Type, Func<string, object?, object?>?> ConversionTable => _conversionTable ??= new Dictionary<Type, Func<string, object?, object?>?>
        {
              [typeof(string)] = (str, def) => str,
              [typeof(byte)] = (str, def) => byte.TryParse(str, out var value) ? value : def,
            [typeof(sbyte)] = (str, def) => sbyte.TryParse(str, out var value) ? value : def,
            [typeof(short)] = (str, def) => short.TryParse(str, out var value) ? value : def,
            [typeof(ushort)] = (str, def) => ushort.TryParse(str, out var value) ? value : def,
            [typeof(int)] = (str, def) => int.TryParse(str, out var value) ? value : def,
            [typeof(uint)] = (str, def) => uint.TryParse(str, out var value) ? value : def,
            [typeof(long)] = (str, def) => long.TryParse(str, out var value) ? value : def,
            [typeof(ulong)] = (str, def) => ulong.TryParse(str, out var value) ? value : def,
            [typeof(float)] = (str, def) => float.TryParse(str, out var value) ? value : def,
            [typeof(double)] = (str, def) => double.TryParse(str, out var value) ? value : def,
            [typeof(decimal)] = (str, def) => decimal.TryParse(str, out var value) ? value : def,
            [typeof(bool)] = (str, def) => bool.TryParse(str, out var value) ? value : def,
            [typeof(DateTime)] = (str, def) => DateTime.TryParse(str, out var value) ? value : def,
            [typeof(byte?)] = (str, def) => str != null ? byte.TryParse(str, out var value) ? value : def : null,
            [typeof(sbyte?)] = (str, def) => str != null ? sbyte.TryParse(str, out var value) ? value : def : null,
            [typeof(short?)] = (str, def) => str != null ? short.TryParse(str, out var value) ? value : def : null,
            [typeof(ushort?)] = (str, def) => str != null ? ushort.TryParse(str, out var value) ? value : def : null,
            [typeof(int?)] = (str, def) => str != null ? int.TryParse(str, out var value) ? value : def : null,
            [typeof(uint?)] = (str, def) => str != null ? uint.TryParse(str, out var value) ? value : def : null,
            [typeof(long?)] = (str, def) => str != null ? long.TryParse(str, out var value) ? value : def : null,
            [typeof(ulong?)] = (str, def) => str != null ? ulong.TryParse(str, out var value) ? value : def : null,
            [typeof(float?)] = (str, def) => str != null ? float.TryParse(str, out var value) ? value : def : null,
            [typeof(double?)] = (str, def) => str != null ? double.TryParse(str, out var value) ? value : def : null,
            [typeof(decimal?)] = (str, def) => str != null ? decimal.TryParse(str, out var value) ? value : def : null,
            [typeof(bool?)] = (str, def) => str != null ? bool.TryParse(str, out var value) ? value : def : null,
              [typeof(DateTime?)] = (str, def) => str != null ? DateTime.TryParse(str, out var value) ? value : def : null
        };

        #endregion Static

        #region Methods (Public)

        /// <summary>
        /// Tries to parse this string to a nullable boolean.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <returns>The string parsed to boolean if success, null if failure.</returns>
        public static bool ToBoolean(this string stringToParse, bool defaultValue = false)
            => bool.TryParse(stringToParse, out var returnBoolean) ? returnBoolean : defaultValue;


        /// <summary>
        /// Tries to parse this string to a nullable boolean.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <returns>The string parsed to boolean if success, null if failure.</returns>
        public static bool? ToNullableBoolean(this string stringToParse, bool? defaultValue = null)
            => bool.TryParse(stringToParse, out var returnBoolean) ? returnBoolean : defaultValue;

        /// <summary>
        /// Tries to parse this string to an integer.
        /// Returns a default value if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <param name="numberStyle">The number styles indication.</param>
        /// <param name="formatProvider">The culture format provider if any.</param>
        /// <returns>The string parsed to integer if success, default value if failure.</returns>
        public static int ToInt32(this string stringToParse, int defaultValue = 0, NumberStyles numberStyle = NumberStyles.None, IFormatProvider? formatProvider = null)
            => int.TryParse(stringToParse, numberStyle, formatProvider, out var returnInteger) ? returnInteger : defaultValue;

        /// <summary>
        /// Tries to parse this string to a nullable integer.
        /// Returns a default value if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <param name="numberStyle">The number styles indication.</param>
        /// <param name="formatProvider">The culture format provider if any.</param>
        /// <returns>The string parsed to integer if success, default value if failure.</returns>
        public static int? ToNullableInt32(this string stringToParse, int? defaultValue = null, NumberStyles numberStyle = NumberStyles.None, IFormatProvider? formatProvider = null)
            => int.TryParse(stringToParse, numberStyle, formatProvider, out var returnInteger) ? returnInteger : defaultValue;

        /// <summary>
        /// Tries to parse this string to a long.
        /// Returns a default value if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <param name="numberStyle">The number styles indication.</param>
        /// <param name="formatProvider">The culture format provider if any.</param>
        /// <returns>The string parsed to integer if success, default value if failure.</returns>
        public static long ToInt64(this string stringToParse, long defaultValue = 0, NumberStyles numberStyle = NumberStyles.None, IFormatProvider? formatProvider = null)
            => long.TryParse(stringToParse, numberStyle, formatProvider, out var returnLong) ? returnLong : defaultValue;

        /// <summary>
        /// Tries to parse this string to a nullable long.
        /// Returns a default value if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <param name="numberStyle">The number styles indication.</param>
        /// <param name="formatProvider">The culture format provider if any.</param>
        /// <returns>The string parsed to integer if success, default value if failure.</returns>
        public static long? ToNullableInt64(this string stringToParse, long? defaultValue = null, NumberStyles numberStyle = NumberStyles.None, IFormatProvider? formatProvider = null)
            => long.TryParse(stringToParse, numberStyle, formatProvider, out var returnLong) ? returnLong : defaultValue;

        /// <summary>
        /// Tries to parse this string to a ulong.
        /// Returns a default value if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <param name="numberStyle">The number styles indication.</param>
        /// <param name="formatProvider">The culture format provider if any.</param>
        /// <returns>The string parsed to integer if success, default value if failure.</returns>
        public static ulong ToUInt64(this string stringToParse, ulong defaultValue = 0, NumberStyles numberStyle = NumberStyles.None, IFormatProvider? formatProvider = null)
            => ulong.TryParse(stringToParse, numberStyle, formatProvider, out var returnLong) ? returnLong : defaultValue;

        /// <summary>
        /// Tries to parse this string to a decimal.
        /// Returns a default value if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <param name="numberStyle">The number styles indication.</param>
        /// <param name="formatProvider">The culture format provider if any.</param>
        /// <returns>The string parsed to decimal if success, default value if failure.</returns>
        public static decimal ToDecimal(this string stringToParse, decimal defaultValue = 0, NumberStyles numberStyle = NumberStyles.None, IFormatProvider? formatProvider = null)
            => decimal.TryParse(stringToParse, numberStyle, formatProvider, out var returnDecimal) ? returnDecimal : defaultValue;

        /// <summary>
        /// Tries to parse this string to a nullable ulong.
        /// Returns a default value if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <param name="numberStyle">The number styles indication.</param>
        /// <param name="formatProvider">The culture format provider if any.</param>
        /// <returns>The string parsed to integer if success, default value if failure.</returns>
        public static ulong? ToNullableUInt64(this string stringToParse, ulong? defaultValue = null, NumberStyles numberStyle = NumberStyles.None, IFormatProvider? formatProvider = null)
            => ulong.TryParse(stringToParse, numberStyle, formatProvider, out var returnLong) ? returnLong : defaultValue;

        /// <summary>
        /// Tries to parse this string to a nullable decimal.
        /// Returns a default value if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <param name="numberStyle">The number styles indication.</param>
        /// <param name="formatProvider">The culture format provider if any.</param>
        /// <returns>The string parsed to nullable decimal if success, default value if failure.</returns>
        public static decimal? ToNullableDecimal(this string stringToParse, decimal? defaultValue = null, NumberStyles numberStyle = NumberStyles.None, IFormatProvider? formatProvider = null)
            => decimal.TryParse(stringToParse, numberStyle, formatProvider, out var returnDecimal) ? returnDecimal : defaultValue;

        /// <summary>
        /// Tries to parse this string to a datetime given an exact format.
        /// Returns a default value if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse.</param>
        /// <param name="format">The format to use for the parsing.</param>
        /// <param name="defaultValue">The default value to apply if parse fails.</param>
        /// <returns>The string parsed to datetime if success, default value if failure.</returns>
        public static DateTime ToDateTimeExact(this string stringToParse, string format, DateTime defaultValue = default)
            => DateTime.TryParseExact(stringToParse, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var returnDateTime) ? returnDateTime : defaultValue;

        /// <summary>
        /// Tries to parse this string to an enum given the name.
        /// Returns a default enum if not able to parse.
        /// </summary>
        /// <param name="stringToParse">The string to parse to name.</param>
        /// <param name="defaultEnum">The default enum to apply if parse fails.</param>
        /// <returns>The string parsed to enum if success, default enum if failure.</returns>
        public static T ToEnum<T>(this string stringToParse, T defaultEnum)
            where T : struct
            => Enum.TryParse<T>(stringToParse, out var returnEnum) ? returnEnum : defaultEnum;

        /// <summary>
        /// Converts a BASE64 string to a byte array or null if failure.
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <returns>The byte array if converted, null otherwise.</returns>
        public static byte[]? FromBase64ToBytes(this string stringToConvert)
        {
            try { return Convert.FromBase64String(stringToConvert); }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Converts a string to a byte array given a encoding charset (default UTF8).
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <param name="encoding">The encoding charset if any.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToBytes(this string stringToConvert, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetBytes(stringToConvert);
        }

        /// <summary>
        /// Converts a string to any primitive types.
        /// If the conversion can not be done then the default value is returned.
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the conversion fails.</param>
        /// <returns>The converted object if success or default value if conversion failed.</returns>
        public static TData? ToObject<TData>(this string stringToConvert, TData? defaultValue = default)
        {
            var convertFunction = ConversionTable.GetValue(typeof(TData));
            if (convertFunction == null)
                return defaultValue;

            var value = convertFunction!(stringToConvert, defaultValue);
            return (TData?)value;
        }

        /// <summary>
        /// Converts a string to a BASE26 string (char from a to z only).
        /// It removes all any other character and lower-ize all characters.
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <returns>The BASE26 string.</returns>
        public static string ToBase26(this string stringToConvert)
            => new(stringToConvert.RemoveDiacritics()!.ToLower().Where(char.IsLetter).ToArray());

        /// <summary>
        /// Converts a string to a BASE36 string (char from a to z only and digit 0 to 9).
        /// It removes all any other character and lower-ize all characters.
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <returns>The BASE36 string.</returns>
        public static string ToBase36(this string stringToConvert)
            => new(stringToConvert.RemoveDiacritics()!.ToLower().Where(char.IsLetterOrDigit).ToArray());

        /// <summary>
        /// Converts a title string into title behavior case that is upper for first of each word and lower for rest.
        /// </summary>
        /// <param name="title">The title to convert.</param>
        /// <returns>The title case converted title.</returns>
        public static string ToTitleCase(this string title)
            => title
                .Split(' ')
                .Where(w => w.Length > 0)
                .Select(w => char.ToUpper(w[0]) + w.Remove(0, 1).ToLower())
                .Aggregate((w1, w2) => $"{w1} {w2}");

        /// <summary>
        /// This converts a pascal case string to a CSS class name. The first upper letter is lowered and then the upper letters are lowered and prefixed by -.
        /// </summary>
        /// <example>OrderReferenceNumber becomes order-reference-number.</example>
        /// <param name="text">The text to convert.</param>
        /// <returns>The CSS class name converted text.</returns>
        public static string? ToCssClassName(this string? text)
            => text != null
                ? string.Join("-", Regex.Split(FirstLetterToLower(text)!, @"(?<!^)(?=[A-Z])").Select(p => p.ToLower()))
                : null;

        /// <summary>
        /// Converts an hex string to an integer.
        /// </summary>
        /// <param name="hexString">The hex string to convert.</param>
        /// <param name="defaultValue">The default value to return if not able to parse.</param>
        /// <returns>The correct integer if parsed, default value if not.</returns>
        public static int FromHexToInt32(this string hexString, int defaultValue = -1)
            => int.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var integer) ? integer : defaultValue;

        /// <summary>
        /// Split a string into fixed size string chunk.
        /// </summary>
        /// <param name="str">The string to split by fixed size.</param>
        /// <param name="maxLength">The maximum length to split the string.</param>
        /// <returns>An enumeration with the string split.</returns>
        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            for (var index = 0; index < str.Length; index += maxLength)
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
        }

        #region File and Json

        /// <summary>
        /// Removes the extension from a file name if any.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The file name without its extension.</returns>
        public static string StripFileExtension(this string fileName)
        {
            if (fileName.Contains('\n') || fileName.Contains('\r'))
                return fileName;

            var lastIndex = fileName.LastIndexOf('.');
            if (lastIndex > 0)
            {
                var ext = fileName.Substring(lastIndex);
                if (!ext.Contains(' '))
                    return fileName.Substring(0, lastIndex);
            }

            return fileName;
        }

        /// <summary>
        /// Determines the extension part of a path or URL.
        /// </summary>
        /// <param name="file">The file path or URL.</param>
        /// <returns>The extension including the leading dot if found; otherwise an empty string.</returns>
        public static string GetFileExtension(this string file)
        {
            const string pattern = @"(?<extension>\.[^\.\?]+)(\?.*|$)";
            var match = Regex.Match(file, pattern);
            return match.Success ? match.Groups["extension"].Value : string.Empty;
        }

        /// <summary>
        /// Detects whether a string looks like JSON without parsing it.
        /// </summary>
        /// <param name="input">The text to inspect.</param>
        /// <returns>True if the string seems to be JSON.</returns>
        public static bool DetectIsJson(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();
            return (input[0] == '[' && input[^1] == ']') || (input[0] == '{' && input[^1] == '}');
        }

        /// <summary>
        /// Detects whether the provided JSON string is empty ("[]" or "{}").
        /// </summary>
        /// <param name="input">The JSON text.</param>
        /// <returns>True if the JSON is empty.</returns>
        public static bool DetectIsEmptyJson(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var cleaned = Regex.Replace(input, @"\s", string.Empty);
            return cleaned == "[]" || cleaned == "{}";
        }

        /// <summary>
        /// Replaces non alphanumeric characters by the provided replacement character.
        /// </summary>
        /// <param name="input">The text to sanitize.</param>
        /// <param name="replacement">The replacement character.</param>
        /// <returns>The sanitized string.</returns>
        public static string? ReplaceNonAlphanumericChars(this string? input, char replacement)
        {
            if (input == null)
                return null;

            var chars = input.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
                if (!char.IsLetterOrDigit(chars[i]))
                    chars[i] = replacement;

            return new string(chars);
        }

        #endregion File and Json

        /// <summary>
        /// Checks whether a string contains another string
        /// </summary>
        /// <param name="stringFull">The full string to search string inside.</param>
        /// <param name="stringToSearch">The string to search inside the full string.</param>
        /// <param name="stringComparison">the string comparison way.</param>
        /// <returns>True if contains, false otherwise.</returns>
        public static bool Contains(this string stringFull, string stringToSearch, StringComparison stringComparison)
            => stringFull.IndexOf(stringToSearch, stringComparison) >= 0;

        /// <summary>
        /// Gets a substring from the given character to the end of the string or empty string if not found.
        /// </summary>
        /// <param name="str">The string to substring.</param>
        /// <param name="character">The character from which takes the substring.</param>
        /// <returns>The substring from the given character to the end of the string or null if not found.</returns>
        public static string? SubstringFrom(this string str, char character)
        {
            var index = str.IndexOf(character);
            if (index < 0)
                return null;

            return str.Substring(index + 1);
        }

        /// <summary>
        /// Gets a substring from the last occur of a given character to the end of the string or empty string if not found.
        /// </summary>
        /// <param name="str">The string to substring.</param>
        /// <param name="character">The character from which takes the substring.</param>
        /// <returns>The substring from the last occur of a given character to the end of the string or null if not found.</returns>
        public static string? SubstringFromLast(this string str, char character)
        {
            var index = str.LastIndexOf(character);
            if (index < 0)
                return null;

            return str.Substring(index + 1);
        }

        /// <summary>
        /// Gets a substring from the given string to the end of the string or empty string if not found.
        /// </summary>
        /// <param name="str">The string to substring.</param>
        /// <param name="strToCompare">The string to compare from which takes the substring.</param>
        /// <returns>The substring from the given character to the end of the string or null if not found.</returns>
        public static string? SubstringFrom(this string str, string strToCompare)
        {
            var index = str.IndexOf(strToCompare, StringComparison.Ordinal);
            if (index < 0)
                return null;

            return str.Substring(index + strToCompare.Length);
        }

        /// <summary>
        /// Gets a substring from the last occur of a given string to the end of the string or empty string if not found.
        /// </summary>
        /// <param name="str">The string to substring.</param>
        /// <param name="strToCompare">The string to compare from which takes the substring.</param>
        /// <returns>The substring from the last occur of a given string to the end of the string or null if not found.</returns>
        public static string? SubstringFromLast(this string str, string strToCompare)
        {
            var index = str.LastIndexOf(strToCompare, StringComparison.Ordinal);
            if (index < 0)
                return null;

            return str.Substring(index + strToCompare.Length);
        }

        /// <summary>
        /// Gets a substring from the beginning of the string until a given character or the full string if not found.
        /// </summary>
        /// <param name="str">The string to substring.</param>
        /// <param name="character">The character until which takes the substring.</param>
        /// <returns>The substring from the beginning of the string until a given character or the full string if not found.</returns>
        public static string SubstringUntil(this string str, char character)
        {
            var index = str.IndexOf(character);
            if (index < 0)
                return str;

            return str.Substring(0, index);
        }

        /// <summary>
        /// Gets a substring from the beginning of the string until the last occur of a given character or the full string if not found.
        /// </summary>
        /// <param name="str">The string to substring.</param>
        /// <param name="character">The character until which takes the substring.</param>
        /// <returns>The substring from the beginning of the string until the last occur of a given character or the full string if not found.</returns>
        public static string SubstringUntilLast(this string str, char character)
        {
            var index = str.LastIndexOf(character);
            if (index < 0)
                return str;

            return str.Substring(0, index);
        }

        /// <summary>
        /// Gets a substring from the beginning of the string until a given character or the full string if not found.
        /// </summary>
        /// <param name="str">The string to substring.</param>
        /// <param name="strToCompare">The string to compare until which takes the substring.</param>
        /// <returns>The substring from the beginning of the string until a given character or the full string if not found.</returns>
        public static string SubstringUntil(this string str, string strToCompare)
        {
            var index = str.IndexOf(strToCompare, StringComparison.Ordinal);
            if (index < 0)
                return str;

            return str.Substring(0, index);
        }

        /// <summary>
        /// Gets a substring from the beginning of the string until the last occur of a substring or the full string if not found.
        /// </summary>
        /// <param name="str">The string to substring.</param>
        /// <param name="strToCompare">The string to compare until which takes the substring.</param>
        /// <returns>The substring from the beginning of the string until the last occur of a substring or the full string if not found.</returns>
        public static string SubstringUntilLast(this string str, string strToCompare)
        {
            var index = str.LastIndexOf(strToCompare, StringComparison.Ordinal);
            if (index < 0)
                return str;

            return str.Substring(0, index);
        }

        /// <summary>
        /// Gets a substring between the first and last given character of the string or the full string if not found.
        /// </summary>
        /// <param name="str">The string to substring.</param>
        /// <param name="character">The character between which takes the substring</param>
        /// <returns></returns>
        public static string SubstringBetween(this string str, char character)
        {
            var firstIndex = str.IndexOf(character);
            var secondIndex = str.LastIndexOf(character);

            if (firstIndex == secondIndex || firstIndex < 0)
                return str;

            return str.Substring(0, secondIndex).Substring(firstIndex + 1);
        }

        /// <summary>
        /// Encode a string to base64 
        /// </summary>
        /// <param name="plainText">the string to encode</param>
        /// <returns></returns>
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Decode a base64 string
        /// </summary>
        /// <param name="base64EncodedData">the base64 string to decode</param>
        /// <returns></returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Removes all the diacritics from a string and replace them with the non-diacritic letter.
        /// </summary>
        /// <param name="str">The string to remove the diacritics from.</param>
        /// <returns>The string where the diacritics have been replaced by normal a-z letter.</returns>
        public static string? RemoveDiacritics(this string? str)
        {
            // Only if the string is not null
            if (str == null)
                return null;

            // Normalize each unicode character to replace the diacritics with the corresponding letter
            var chars = str.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            var cleanString = new string(chars.ToArray()).Normalize(NormalizationForm.FormC);

            return cleanString;
        }

        /// <summary>
        /// Checks whether a string has any occur in a string array.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="strings">The strings array.</param>
        /// <returns>True if any are equals, false otherwise.</returns>
        public static bool EqualsAny(this string stringToCheck, params string[] strings)
            => strings.Contains(stringToCheck);

        /// <summary>
        /// Checks whether a string contains any sub text in a string array.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="strings">The strings array.</param>
        /// <returns>True if any are equals, false otherwise.</returns>
        public static bool ContainsAny(this string stringToCheck, params string[] strings)
            => strings.Any(stringToCheck.Contains);

        /// <summary>
        /// Truncates a string if the length is more than the max length.
        /// </summary>
        /// <param name="value">The value to truncate.</param>
        /// <param name="maxLength">The max length.</param>
        /// <returns>The truncated string.</returns>
        public static string? Truncate(this string? value, int maxLength)
        {
            if (value == null) return null;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Returns a fixed string length truncated if too big or filled with padding characters if too small.
        /// If the string is null then the returned string will be completely filled with padding characters.
        /// </summary>
        /// <param name="str">The string to convert to fixed string.</param>
        /// <param name="length">The fixed length of the output string.</param>
        /// <param name="paddingCharacter">The padding character if the size is too small.</param>
        /// <returns>The fixed length string.</returns>
        public static string ToFixedSize(this string str, int length, char paddingCharacter = ' ')
        {
            if (str == null)
                return new string(paddingCharacter, length);

            if (str.Length < length)
                return str.PadRight(length, paddingCharacter);

            if (str.Length > length)
                return str.Substring(0, length);

            return str;
        }

        /// <summary>
        /// Gets the first section content text if any.
        /// ex.: This is a {{ Sample }} test =>  " Sample ".
        /// </summary>
        /// <param name="text">The text to search in.</param>
        /// <param name="sectionStartDelimiter">The section start string delimiter.</param>
        /// <param name="sectionEndDelimiter">The section end string delimiter.</param>
        /// <returns>The section content if found, null otherwise.</returns>
        public static string? GetSectionContent(this string text, string sectionStartDelimiter, string sectionEndDelimiter)
        {
            // First tries to find the start and end index of the section delimiters to delimit a valid section if any
            var startIndex = text.IndexOf(sectionStartDelimiter, 0, StringComparison.Ordinal);
            if (startIndex < 0)
                return null;
            var endIndex = text.IndexOf(sectionEndDelimiter, startIndex, StringComparison.Ordinal);
            if (endIndex < 0)
                return null;

            return text.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        /// <summary>
        /// Gets all the section content text if any.
        /// ex.: This is a {{ Sample }} test {{isn't it}} =>  [" Sample ", "isn't it"].
        /// </summary>
        /// <param name="text">The text to search in.</param>
        /// <param name="sectionStartDelimiter">The section start string delimiter.</param>
        /// <param name="sectionEndDelimiter">The section end string delimiter.</param>
        /// <returns>All the section content if one found, empty array otherwise.</returns>
        public static string[] GetAllSectionContent(this string text, string sectionStartDelimiter, string sectionEndDelimiter)
        {
            var currentIndex = 0;
            var content = new List<string>();
            while (true)
            {
                var startIndex = text.IndexOf(sectionStartDelimiter, currentIndex, StringComparison.Ordinal);
                if (startIndex < 0)
                    break;
                var endIndex = text.IndexOf(sectionEndDelimiter, startIndex, StringComparison.Ordinal);
                if (endIndex < 0)
                    break;

                var startSectionIndex = startIndex + sectionStartDelimiter.Length;
                var sectionText = text.Substring(startSectionIndex, endIndex - startSectionIndex);
                content.Add(sectionText);

                currentIndex = endIndex + sectionEndDelimiter.Length;
            }

            return content.ToArray();
        }

        /// <summary>
        /// Replaces in a text all sections defined by start/end delimiters by a computed text given the section content.
        /// ex.: This is a {{ Sample }} test => This is a SAMPLE test.
        /// </summary>
        /// <param name="text">The text to replace the formatted sections if any.</param>
        /// <param name="sectionStartDelimiter">The section start string delimiter.</param>
        /// <param name="sectionEndDelimiter">The section end string delimiter.</param>
        /// <param name="sectionTextChangeFunc">The function to change the section inner text. ex: str => str.ToUpper().</param>
        /// <returns>The text with the sections replaced if any, or the text itself if no section found.</returns>
        public static string ReplaceSections(this string text, string sectionStartDelimiter, string sectionEndDelimiter, Func<string, string> sectionTextChangeFunc)
        {
            var currentIndex = 0;
            var outText = new StringBuilder();
            while (true)
            {
                // First tries to find the start and end index of the section delimiters to delimit a valid section if any
                var startIndex = text.IndexOf(sectionStartDelimiter, currentIndex, StringComparison.Ordinal);
                if (startIndex < 0)
                    break;
                var endIndex = text.IndexOf(sectionEndDelimiter, startIndex, StringComparison.Ordinal);
                if (endIndex < 0)
                    break;

                // Appends the previous text to the out text
                outText.Append(text, currentIndex, startIndex - currentIndex);

                // Gets the section text, applies the change function to it and appends it to the out text
                var startSectionIndex = startIndex + sectionStartDelimiter.Length;
                var sectionText = text.Substring(startSectionIndex, endIndex - startSectionIndex);
                var replacedText = sectionTextChangeFunc(sectionText);
                outText.Append(replacedText);

                currentIndex = endIndex + sectionEndDelimiter.Length;
            }

            // If no section has been found then return the text itself.
            if (currentIndex == 0)
                return text;

            // Otherwise appends the remaining of the text and returns
            outText.Append(text, currentIndex, text.Length - currentIndex);
            return outText.ToString();
        }

        /// <summary>
        /// Emphasizes a part of the text by replacing it with a formatted text including this same text part without changing the case.
        /// </summary>
        /// <example>"This is a Great text", "great", "<em>{0}</em>" >> "This is a <em>Great</em> text"</example>
        /// <param name="text">The text to emphasize.</param>
        /// <param name="part">The part of the text to emphasize.</param>
        /// <param name="formatEmphasize">The format used to emphasize the text part with {0} parameter.</param>
        /// <returns>The emphasized text or the text itself if the text part is not found.</returns>
        public static string Emphasize(this string text, string part, string formatEmphasize = "<em>{0}</em>")
        {
            if (text.IsNullOrEmpty() || part.IsNullOrEmpty())
                return text;

            var startIndex = text.IndexOf(part, StringComparison.InvariantCultureIgnoreCase);
            if (startIndex < 0)
                return text;

            return $"{text.Substring(0, startIndex)}{string.Format(formatEmphasize, text.Substring(startIndex, part.Length))}{text.Substring(startIndex + part.Length)}";
        }

        /// <summary>
        /// Makes a dynamic string interpolation using a single parameter.
        /// </summary>
        /// <param name="stringInterpolation">The string to be interpolated using the parameter.</param>
        /// <param name="parameter">The parameter used in string interpolation.</param>
        /// <param name="parameterName">The name of the parameter used.</param>
        /// <returns>The interpolated string.</returns>
        public static string Interpolate(this string stringInterpolation, object parameter, string parameterName)
        {
            return Regex.Replace(stringInterpolation, @"{(?<exp>[^}]+)}", match =>
            {
                var result = EvaluateSimpleExpression(parameter, match.Groups["exp"].Value);
                return result?.ToString() ?? string.Empty;
            });
        }

        /// <summary>
        /// Makes a dynamic string interpolation using a single parameter.
        /// </summary>
        /// <param name="stringInterpolation">The string to be interpolated using the parameter.</param>
        /// <param name="parameterExpression">An expression that returns a parameter.</param>
        /// <returns>The interpolated string.</returns>
        public static string Interpolate(this string stringInterpolation, Expression<Func<object>> parameterExpression)
            => Interpolate(stringInterpolation, parameterExpression.Compile()(), parameterExpression.Body.GetFirstMemberOrMethodName());

        /// <summary>
        /// Evaluates a simple expression on the given parameter using reflection.
        /// Supports nested property paths and parameterless method calls.
        /// </summary>
        /// <param name="parameter">The parameter used as expression root.</param>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The evaluated object or null.</returns>
        private static object? EvaluateSimpleExpression(object parameter, string expression)
        {
            if (parameter == null || string.IsNullOrWhiteSpace(expression))
                return null;

            object? current = parameter;
            foreach (var part in expression.Split('.', StringSplitOptions.RemoveEmptyEntries))
            {
                if (current == null)
                    return null;

                var member = part.Trim();
                if (member.EndsWith("()", StringComparison.Ordinal))
                {
                    var methodName = member.Substring(0, member.Length - 2);
                    var methodInfo = current.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase, Type.DefaultBinder, Type.EmptyTypes, null);
                    current = methodInfo?.Invoke(current, null);
                }
                else
                {
                    current = current.GetPropertyValueWithExpandoSupport<object>(member);
                }
            }

            return current;
        }

        /// <summary>
        /// Splits a string by indexes (yield IEnumerable).
        /// </summary>
        /// <param name="string">The string to split.</param>
        /// <param name="indexes">The indexes where to split the string.</param>
        /// <returns>The split string as enumerable (yield).</returns>
        public static IEnumerable<string> SplitByIndex(this string @string, params int[] indexes)
        {
            var previousIndex = 0;
            foreach (var index in indexes.OrderBy(i => i))
            {
                yield return @string[previousIndex..index];
                previousIndex = index;
            }

            yield return @string.Substring(previousIndex);
        }

        /// <summary>
        /// Whether this string is null or empty.
        /// Extension method allows the string to be null.
        /// </summary>
        /// <param name="str">The string to challenge.</param>
        /// <returns>True if null or empty, false otherwise.</returns>
        public static bool IsNullOrEmpty(this string str)
            => string.IsNullOrEmpty(str);

        /// <summary>
        /// Whether this string is null or empty or another specific value.
        /// Extension method allows the string to be null.
        /// </summary>
        /// <param name="str">The string to challenge.</param>
        /// <param name="value">The string to compare if not null or empty.</param>
        /// <returns>True if null or empty, false otherwise.</returns>
        public static bool IsNullOrEmptyOr(this string str, string value)
            => string.IsNullOrEmpty(str) || str == value;

        /// <summary>
        /// Whether this string is null or empty or white space.
        /// Extension method allows the string to be null.
        /// </summary>
        /// <param name="str">The string to challenge.</param>
        /// <returns>True if null or empty or white space, false otherwise.</returns>
        public static bool IsNullOrWhiteSpace(this string str)
            => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Whether this string is not null or empty.
        /// Extension method allows the string to be null.
        /// </summary>
        /// <param name="str">The string to challenge.</param>
        /// <returns>True if not null or empty, false otherwise.</returns>
        public static bool IsNotNullOrEmpty(this string str)
            => !string.IsNullOrEmpty(str);

        /// <summary>
        /// Whether this string is not null or empty or white space.
        /// Extension method allows the string to be null.
        /// </summary>
        /// <param name="str">The string to challenge.</param>
        /// <returns>True if not null or empty or white space, false otherwise.</returns>
        public static bool IsNotNullOrWhiteSpace(this string str)
            => !string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Appends a string to this string.
        /// If this string is null then it returns only the appended string.
        /// </summary>
        /// <param name="str">The string that will be appended.</param>
        /// <param name="stringToAppend">The string to append.</param>
        /// <returns>The newly appended string.</returns>
        public static string Append(this string str, string stringToAppend)
        {
            if (string.IsNullOrEmpty(stringToAppend))
                return str;

            return str == null ? stringToAppend : $"{str}{stringToAppend}";
        }

        /// <summary>
        /// Appends a string to this string with a new line if text already exists.
        /// If this string is null then it returns only the appended string.
        /// </summary>
        /// <param name="str">The string that will be appended.</param>
        /// <param name="stringToAppend">The string to append.</param>
        /// <returns>The newly appended string.</returns>
        public static string AppendToNewLine(this string str, string stringToAppend)
        {
            if (string.IsNullOrEmpty(stringToAppend))
                return str;

            return str == null ? stringToAppend : $"{str}{Environment.NewLine}{stringToAppend}";
        }


        /// <summary>
        /// Convert first letter of the String to upper case.
        /// </summary>
        /// <param name="str">The string that will be changed.</param>
        /// <returns>The String with the first char upper.</returns>
        public static string? FirstLetterToUpper(this string? str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        /// <summary>
        /// Convert first letter of the String to lower case.
        /// </summary>
        /// <param name="str">The string that will be changed.</param>
        /// <returns>The String with the first char upper.</returns>
        public static string? FirstLetterToLower(this string? str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToLower(str[0]) + str.Substring(1);

            return str.ToLower();
        }

        /// <summary>
        /// Checks whether a string matches a wildcard pattern.
        /// By default with * as wildcard character.
        /// </summary>
        /// <example>
        /// John matches *o* pattern.
        /// John does not match *o pattern.
        /// John does match John pattern.
        /// John does match *John* pattern.
        /// </example>
        /// <param name="str">The string to check.</param>
        /// <param name="pattern">The wildcard pattern to use for the check.</param>
        /// <param name="wildcardCharacter">The wildcard character, by default * but could be also ? for example.</param>
        /// <returns>True if the string matches the wildcard pattern, false otherwise.</returns>
        public static bool IsMatchingWildCardPattern(this string str, string pattern, char wildcardCharacter = '*')
        {
            var regex = "^" + Regex.Escape(pattern).Replace($"\\{wildcardCharacter}", $".{wildcardCharacter}") + "$";
            return Regex.IsMatch(str, regex);
        }

        #endregion Methods (Helpers)

        #region Methods (MultiLanguage)

        /// <summary>
        /// Extracts a text from a JSON multi language string.
        /// Returns the culture related text if available (fr-BE) or the language text if available (fr) or the first one.
        /// </summary>
        /// <param name="jsonMlText">The JSON formatted culture to extract the more relevant text.</param>
        /// <param name="culture">The culture to extract the more relevant text in ISO (fr-BE).</param>
        /// <returns>The more relevant text returned or null if empty or not valid.</returns>
        public static string? GetFromMultiLanguageJsonText(this string jsonMlText, string culture)
        {
            try
            {
                if (jsonMlText.IsNullOrWhiteSpace() || culture.Length < 2)
                    return null;

                var texts = JsonConvert.DeserializeObject<Dictionary<string, string?>>(jsonMlText);
                if (texts == null)
                    return null;
                culture = culture.ToLower();
                return texts.GetValue(culture) ?? texts.GetValue(culture.Substring(2)) ?? texts.Values.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts an array of items from a JSON array string.
        /// </summary>
        /// <typeparam name="TItem">The type of the item array.</typeparam>
        /// <param name="jsonArray">The JSON array string.</param>
        /// <returns>The item array extracted or empty array if deserialization failed.</returns>
        public static TItem[] GetArrayFromJsonArray<TItem>(this string jsonArray)
        {
            try
            {
                return JsonConvert.DeserializeObject<TItem[]>(jsonArray) ?? Array.Empty<TItem>();
            }
            catch (Exception)
            {
                return new TItem[0];
            }
        }

        #endregion Methods (MultiLanguage)

        #region Methods (Additional)

        /// <summary>
        /// Checks whether a string is a valid email address.
        /// </summary>
        /// <param name="email">The string to check.</param>
        /// <returns>True if the string is a valid email address.</returns>
        public static bool IsEmail(this string? email)
            => !string.IsNullOrWhiteSpace(email) && new EmailAddressAttribute().IsValid(email);




        /// <summary>
        /// Removes HTML tags from the given string.
        /// </summary>
        /// <param name="text">String that can contain HTML tags.</param>
        /// <returns>The string without HTML tags.</returns>
        public static string StripHtml(this string text)
            => Regex.Replace(text, "<(.|\n)*?>", string.Empty, RegexOptions.Compiled);

        /// <summary>
        /// Escapes characters with special meaning in regular expressions.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <returns>Escaped text safe for regular expressions.</returns>
        public static string EscapeRegexSpecialCharacters(this string text)
            => Regex.Escape(text);

        /// <summary>
        /// Trims the specified substring from both ends of this string.
        /// </summary>
        /// <param name="value">The string to trim.</param>
        /// <param name="forRemoving">Substring to remove.</param>
        /// <returns>String without the substring at both ends.</returns>
        public static string Trim(this string value, string forRemoving)
            => value.TrimEnd(forRemoving).TrimStart(forRemoving);

        /// <summary>
        /// Trims the specified substring from the start of this string.
        /// </summary>
        /// <param name="value">The string to trim.</param>
        /// <param name="forRemoving">Substring to remove.</param>
        /// <returns>String without the substring at the start.</returns>
        public static string TrimStart(this string value, string forRemoving)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(forRemoving))
                return value;

            while (value.StartsWith(forRemoving, StringComparison.InvariantCultureIgnoreCase))
                value = value.Substring(forRemoving.Length);

            return value;
        }

        /// <summary>
        /// Trims the specified substring from the end of this string.
        /// </summary>
        /// <param name="value">The string to trim.</param>
        /// <param name="forRemoving">Substring to remove.</param>
        /// <returns>String without the substring at the end.</returns>
        public static string TrimEnd(this string value, string forRemoving)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(forRemoving))
                return value;

            while (value.EndsWith(forRemoving, StringComparison.InvariantCultureIgnoreCase))
                value = value.Substring(0, value.LastIndexOf(forRemoving, StringComparison.InvariantCultureIgnoreCase));

            return value;
        }

        /// <summary>
        /// Counts the occurrences of a substring inside this string.
        /// </summary>
        /// <param name="haystack">String to search.</param>
        /// <param name="needle">Substring to find.</param>
        /// <returns>The number of occurrences.</returns>
        public static int CountOccurrences(this string haystack, string needle)
            => haystack.Length - haystack.Replace(needle, string.Empty).Length;

        /// <summary>
        /// Normalizes a culture code into its invariant casing.
        /// </summary>
        /// <param name="culture">Culture code.</param>
        /// <returns>Culture code in standard casing or null.</returns>
        public static string? EnsureCultureCode(this string? culture)
        {
            if (string.IsNullOrEmpty(culture) || culture == "*")
                return culture;

            return new CultureInfo(culture).Name;
        }

        /// <summary>
        ///     Compares two strings using invariant culture and case-insensitive rules.
        /// </summary>
        /// <param name="compare">The string instance to compare.</param>
        /// <param name="compareTo">The other string to compare with.</param>
        /// <returns>
        ///     <c>true</c> if both strings are equal ignoring case using invariant culture;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool InvariantEquals(this string? compare, string? compareTo) =>
            string.Equals(compare, compareTo, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Appends query string fragments to a base URL.
        /// </summary>
        /// <param name="url">Base URL.</param>
        /// <param name="queryStrings">Query fragments.</param>
        /// <returns>The combined URL with query strings.</returns>
        public static string AppendQueryStringToUrl(this string url, params string[] queryStrings)
        {
            var nonEmpty = queryStrings.Where(q => !string.IsNullOrWhiteSpace(q))
                                       .Select(q => q.Trim('&', '?'));
            if (!nonEmpty.Any())
                return url;

            var separator = url.Contains('?') ? '&' : '?';
            return url + separator + string.Join("&", nonEmpty);
        }

        #endregion Methods (Additional)
    }
}