using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace Ark
{
    /// <summary>
    /// Provides helper methods for enumeration types such as flag manipulation,
    /// description retrieval and dynamic loading of additional values.
    /// </summary>
    /// <remarks>
    /// The <see cref="LoadFromJson{TEnum}(TEnum,JsonDocument,out Result{List{EnumItem}})"/> method
    /// allows extending an enumeration at runtime using values defined in a JSON
    /// document. Existing codes are replaced and invalid items are reported
    /// through the returned <see cref="Result"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = JsonDocument.Parse("[ { \"Code\": 1, \"Label\": \"Open\" } ]");
    /// OrderStatus.None.LoadFromJson(json, out var loadResult);
    /// foreach (var item in loadResult.Value)
    ///     Console.WriteLine($"Added {item.Code} => {item.Label}");
    /// if (loadResult.Status != ResultStatus.Success)
    ///     Console.WriteLine(loadResult.Reason);
    /// </code>
    /// </example>
    public static class EnumExtensibility
    {
        #region Nested Classes

        /// <summary>
        /// Representation of an enumeration item as defined in a JSON file.
        /// </summary>
        public class EnumItem
        {
            /// <summary>
            /// Code of the enumeration value. Must be convertible to the
            /// underlying numeric type.
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// Display label of the enumeration value.
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// Optional description of the enumeration value.
            /// </summary>
            public string Description { get; set; } = string.Empty;

            /// <summary>
            /// Optional attribute name expected on the enumeration value.
            /// </summary>
            public string Atribute { get; set; } = string.Empty;

        }
        #endregion Nested Classes
        #region Static (Fields)

        /// <summary>
        /// The already found enum descriptions to avoid multiple reflections.
        /// </summary>
        private static ConcurrentDictionary<Type, Dictionary<int, string>> _enumDescriptions;

        /// <summary>
        /// The enumeration items loaded from JSON files keyed by type.
        /// </summary>
        private static ConcurrentDictionary<Type, Dictionary<int, EnumItem>> _enumItems;

        #endregion Static (Fields)

        #region Methods

        /// <summary>
        /// Adds a flag to a flagged enumeration and returns the modified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration to add the flag.</typeparam>
        /// <param name="enumeration">The current flagged enumeration.</param>
        /// <param name="flag">The flag to add.</param>
        /// <returns>The enumeration with the flag added.</returns>
        public static T AddFlag<T>(this Enum enumeration, T flag)
            where T : struct
        {
            var intEnum = Convert.ToInt64(enumeration);
            intEnum |= Convert.ToInt64(flag);
            return (T)Enum.ToObject(typeof(T), intEnum);
        }

        /// <summary>
        /// Removes a flag from a flagged enumeration and returns the modified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration to remove the flag.</typeparam>
        /// <param name="enumeration">The current flagged enumeration.</param>
        /// <param name="flag">The flag to remove.</param>
        /// <returns>The enumeration with the flag removed.</returns>
        public static T RemoveFlag<T>(this Enum enumeration, T flag)
            where T : struct
        {
            var intEnum = Convert.ToInt64(enumeration);
            intEnum &= ~Convert.ToInt64(flag);
            return (T)Enum.ToObject(typeof(T), intEnum);
        }

        /// <summary>
        /// Get the description attribute value for an enumeration if any.
        /// </summary>
        /// <param name="enumeration">The enumeration to find the description from.</param>
        /// <returns>The list item attribute from an enumeration if any.</returns>
        public static string GetDescription(this Enum enumeration)
        {
            _enumDescriptions ??= new ConcurrentDictionary<Type, Dictionary<int, string>>();

            var enumerationType = enumeration.GetType();
            if (!_enumDescriptions.TryGetValue(enumerationType, out var descriptions))
            {
                var names = Enum.GetNames(enumerationType);
                var members = enumerationType.GetTypeInfo().DeclaredMembers.Where(m => names.Contains(m.Name));
                descriptions = members.ToDictionary(m => (int)Enum.Parse(enumerationType, m.Name), m => m.GetCustomAttribute<DescriptionAttribute>()?.Description);
                _enumDescriptions.AddOrUpdate(enumerationType, descriptions);
            }

            return descriptions[(int)(object)enumeration];
        }

        /// <summary>
        /// Whether this enumeration is the same enumeration given in parameter.
        /// Syntactic sugar.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="enumeration">The enumeration to check.</param>
        /// <param name="enumerationToCheck">The other enumeration to check.</param>
        /// <returns>True if this enumeration is the same enumeration given in parameter.</returns>
        public static bool Is<TEnum>(this TEnum enumeration, TEnum enumerationToCheck)
            where TEnum : struct, IComparable
            => enumeration.CompareTo(enumerationToCheck) == 0;

        /// <summary>
        /// Whether this enumeration is not the same enumeration given in parameter.
        /// Syntactic sugar.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="enumeration">The enumeration to check.</param>
        /// <param name="enumerationToCheck">The other enumeration to check.</param>
        /// <returns>True if this enumeration is not the same enumeration given in parameter.</returns>
        public static bool IsNot<TEnum>(this TEnum enumeration, TEnum enumerationToCheck)
            where TEnum : struct, IComparable
            => enumeration.CompareTo(enumerationToCheck) != 0;

        /// <summary>
        /// Whether this enumeration is one of the following enumerations.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="enumeration">The enumeration to check.</param>
        /// <param name="enumerationsToCheck">The array of enumerations to check against the enumeration.</param>
        /// <returns>True if the enumeration is one of the enumerations to check.</returns>
        public static bool IsOneOf<TEnum>(this TEnum enumeration, params TEnum[] enumerationsToCheck)
            where TEnum : struct, IComparable
            => enumerationsToCheck.Contains(enumeration);

        /// <summary>
        /// Convenience overload that calls
        /// <see cref="LoadFromJson{TEnum}(TEnum,JsonDocument,out Result{List{EnumItem}})"/>
        /// and ignores the returned <see cref="Result"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="enumeration">The enumeration to load.</param>
        /// <param name="json">The JSON document containing the enumeration values.</param>
        /// <returns>The enumeration so the call can be chained.</returns>
        public static TEnum LoadFromJson<TEnum>(this TEnum enumeration, JsonDocument json)
            => enumeration.LoadFromJson(json, out _);

        /// <summary>
        /// Loads additional enumeration values from a JSON document.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="enumeration">The enumeration to load.</param>
        /// <param name="json">The JSON document containing the enumeration values.</param>
        /// <param name="result">
        ///     Output result populated with warnings and the list of items that were
        ///     added or replaced during the load. The <c>Status</c> is
        ///     <see cref="ResultStatus.Success"/> when all items are valid or
        ///     <see cref="ResultStatus.BadParameters"/> if one or more entries could
        ///     not be parsed.
        /// </param>
        /// <returns>The enumeration so the call can be chained.</returns>
        /// <example>
        /// <code>
        /// var json = JsonDocument.Parse("[ { \"Code\": 1, \"Label\": \"New\" } ]");
        /// MyEnum.None.LoadFromJson(json, out var loadResult);
        /// foreach (var added in loadResult.Value)
        ///     Console.WriteLine(added.Label);
        /// if (loadResult.Status != ResultStatus.Success)
        ///     Console.WriteLine(loadResult.Reason);
        /// </code>
        /// </example>
        public static TEnum LoadFromJson<TEnum>(this TEnum enumeration, JsonDocument json, out Result<List<EnumItem>> result)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            var resultItems = new List<EnumItem>();
            result = new Result<List<EnumItem>>(resultItems);

            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentException($"{enumType.FullName} is not an enumeration.");

            if (json.RootElement.ValueKind != JsonValueKind.Array)
                throw new FormatException("Json document must contain an array of items.");

            EnsureCaches(enumType, out var items, out var descriptions);
            var underlyingType = Enum.GetUnderlyingType(enumType);

            var index = 0;
            foreach (var element in json.RootElement.EnumerateArray())
            {
                index++;
                if (!TryParseEnumItem(element, enumType, underlyingType, index, result, out var key, out var item))
                    continue;

                AddOrReplaceItem(items, descriptions, key, item, result, index, resultItems);
            }

            return enumeration;
        }

        #region Methods (Private Helpers)

        /// <summary>
        /// Initializes the internal caches for the specified enumeration type.
        /// </summary>
        /// <param name="enumType">The enumeration type to initialize caches for.</param>
        /// <param name="items">The dictionary of loaded items.</param>
        /// <param name="descriptions">The dictionary of loaded descriptions.</param>
        private static void EnsureCaches(Type enumType, out Dictionary<int, EnumItem> items, out Dictionary<int, string> descriptions)
        {
            _enumDescriptions ??= new ConcurrentDictionary<Type, Dictionary<int, string>>();
            _enumItems ??= new ConcurrentDictionary<Type, Dictionary<int, EnumItem>>();

            if (!_enumItems.TryGetValue(enumType, out items))
            {
                items = new Dictionary<int, EnumItem>();
                _enumItems.TryAdd(enumType, items);
            }

            if (!_enumDescriptions.TryGetValue(enumType, out descriptions))
            {
                descriptions = new Dictionary<int, string>();
                _enumDescriptions.TryAdd(enumType, descriptions);
            }
        }

        /// <summary>
        /// Parses a JSON element into an <see cref="EnumItem"/> instance and
        /// validates its data.
        /// </summary>
        /// <param name="element">The JSON element representing the item.</param>
        /// <param name="enumType">The enumeration type being loaded.</param>
        /// <param name="underlyingType">The numeric type of the enumeration.</param>
        /// <param name="index">The one-based index of the item in the JSON array.</param>
        /// <param name="result">Result object used to report warnings.</param>
        /// <param name="key">Resulting numeric key of the enumeration value.</param>
        /// <param name="item">Parsed <see cref="EnumItem"/> if successful.</param>
        /// <returns>True when parsing succeeded.</returns>
        private static bool TryParseEnumItem(JsonElement element, Type enumType, Type underlyingType, int index, Result<List<EnumItem>> result, out int key, out EnumItem item)
        {
            key = default;
            item = null;

            try
            {
                item = element.ToObject<EnumItem>();
            }
            catch (Exception ex)
            {
                result.WithStatus(ResultStatus.BadParameters).AddReason($"Invalid item at line {index} : {ex.Message}");
                return false;
            }

            if (string.IsNullOrWhiteSpace(item?.Code) || string.IsNullOrWhiteSpace(item.Label))
            {
                result.WithStatus(ResultStatus.BadParameters).AddReason($"Invalid item at line {index}: code and label are required.");
                return false;
            }

            object value;
            try
            {
                value = Convert.ChangeType(item.Code, underlyingType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                result.WithStatus(ResultStatus.BadParameters).AddReason($"Unable to convert code '{item.Code}' at line {index} : {ex.Message}");
                return false;
            }

            key = Convert.ToInt32(value, CultureInfo.InvariantCulture);

            var attributeName = item.Atribute;
            if (!string.IsNullOrWhiteSpace(attributeName) &&
                !enumType.GetCustomAttributes().Any(a => a.GetType().Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase)))
                result.AddReason($"Line {index}: attribute '{attributeName}' not supported for code {item.Code} ({item.Label}).");

            return true;
        }

        /// <summary>
        /// Adds the parsed item to the caches or replaces an existing entry.
        /// </summary>
        /// <param name="items">Dictionary of enumeration items for the type.</param>
        /// <param name="descriptions">Dictionary of descriptions for the type.</param>
        /// <param name="key">Numeric key of the enumeration value.</param>
        /// <param name="item">The item to insert or replace.</param>
        /// <param name="result">Result object used to append warnings.</param>
        /// <param name="index">Index of the item in the JSON array.</param>
        /// <param name="resultItems">List of items added or replaced.</param>
        private static void AddOrReplaceItem(Dictionary<int, EnumItem> items, Dictionary<int, string> descriptions, int key, EnumItem item, Result<List<EnumItem>> result, int index, List<EnumItem> resultItems)
        {
            switch (items.TryGetValue(key, out var existing))
            {
                case true when existing.Label == item.Label && existing.Description == item.Description && existing.Atribute == item.Atribute:
                    result.AddReason($"Line {index}: item with code {item.Code} already exists.");
                    break;
                case true:
                    result.AddReason($"Line {index}: item with code {item.Code} replaced.");
                    goto default;
                default:
                    resultItems.Add(item);
                    break;
            }

            items[key] = item;
            descriptions[key] = item.Label;
        }

        #endregion Methods (Private Helpers)


        #endregion Methods
    }
}