using System.Collections;
using System.Reflection;
using System.Text;

namespace Ark
{
    /// <summary>
    /// This class extends the exception class to convert it to log string.
    /// </summary>
    public static class ExceptionExtensibility
    {
        /// <summary>
        /// This class handles the options to convert properly the exception to a detailed string.
        /// </summary>
        private class ExceptionOptions
        {
            #region Constructors

            /// <summary>
            /// Creates an ExceptionOptions instance.
            /// </summary>
            /// <param name="currentIndentLevel">The starting indent level.</param>
            /// <param name="indentSpaces">The number of spaces used for indent.</param>
            /// <param name="omitNullProperties">Whether to omit null properties.</param>
            public ExceptionOptions(int currentIndentLevel, int indentSpaces, bool omitNullProperties)
            {
                CurrentIndentLevel = currentIndentLevel;
                IndentSpaces = indentSpaces;
                OmitNullProperties = omitNullProperties;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// The starting indent level.
            /// </summary>
            public int CurrentIndentLevel { get; }

            /// <summary>
            /// The number of spaces used for indent.
            /// </summary>
            public int IndentSpaces { get; }

            /// <summary>
            /// Whether to omit null properties.
            /// </summary>
            public bool OmitNullProperties { get; }

            /// <summary>
            /// The indent in string filled with spaces.
            /// </summary>
            public string Indent => new(' ', IndentSpaces * CurrentIndentLevel);

            #endregion Properties
        }

        /// <summary>
        /// Gets the detailed exception string from an exception.
        /// </summary>
        /// <param name="exception">The exception to get the detailed string from.</param>
        /// <param name="currentIndentLevel">The starting indent level.</param>
        /// <param name="indentSpaces">The number of spaces used for indent.</param>
        /// <param name="omitNullProperties">Whether to omit null properties.</param>
        /// <returns>The detailed exception string.</returns>
        public static string ToDetailedString<T>(this T exception, int currentIndentLevel = 0, int indentSpaces = 4, bool omitNullProperties = true)
            where T : Exception
        {
            var stringBuilder = new StringBuilder();
            var options = new ExceptionOptions(currentIndentLevel, indentSpaces, omitNullProperties);

            AppendValue(stringBuilder, "Type", exception.GetType().FullName, options);

            var properties = typeof(T).GetTypeInfo().GetAllProperties()
                .OrderByDescending(x => string.Equals(x.Name, nameof(exception.Message), StringComparison.Ordinal))
                .ThenByDescending(x => string.Equals(x.Name, nameof(exception.Source), StringComparison.Ordinal))
                .ThenBy(x => string.Equals(x.Name, nameof(exception.InnerException), StringComparison.Ordinal))
                .ThenBy(x => string.Equals(x.Name, nameof(AggregateException.InnerExceptions), StringComparison.Ordinal))
                .ToArray();

            properties.ForEach(property =>
            {
                // WORKAROUND for an Mono problem where IsTransient Get method is missing
                try
                {
                    var value = property.GetValue(exception, null);
                    if (value == null && options.OmitNullProperties) return;
                    AppendValue(stringBuilder, property.Name, value ?? "", options);
                }
                catch (Exception) { /* do nothing */ }
            });

            return stringBuilder.ToString().TrimEnd('\r', '\n');
        }

        /// <summary>
        /// Appends a collection of either exception or objects to the string.
        /// </summary>
        /// <param name="stringBuilder">The string builder used to compose the detailed string.</param>
        /// <param name="propertyName">The name of the property that owns the collection.</param>
        /// <param name="collection">The collection of either Exception or objects.</param>
        /// <param name="options">The main options.</param>
        private static void AppendCollection(StringBuilder stringBuilder, string propertyName, IEnumerable collection, ExceptionOptions options)
        {
            stringBuilder.AppendLine($"{options.Indent}{propertyName} =");

            var innerOptions = new ExceptionOptions(options.CurrentIndentLevel + 1, options.IndentSpaces, options.OmitNullProperties);

            var counter = 0;
            collection.ForEach(item =>
            {
                var innerPropertyName = $"[{counter}]";

                var exception = item as Exception;
                if (exception != null) { AppendException(stringBuilder, innerPropertyName, exception, innerOptions); return; }

                AppendValue(stringBuilder, innerPropertyName, item, innerOptions);
                ++counter;
            });
        }

        /// <summary>
        /// Appends a new exception to the detailed string.
        /// </summary>
        /// <param name="stringBuilder">The string builder used to compose the detailed strind.</param>
        /// <param name="propertyName">The name of the property that owns the collection.</param>
        /// <param name="exception">The exception to append.</param>
        /// <param name="options">The main options.</param>
        private static void AppendException(StringBuilder stringBuilder, string propertyName, Exception exception, ExceptionOptions options)
        {
            var innerExceptionString = ToDetailedString(exception, options.CurrentIndentLevel + 1, options.IndentSpaces, options.OmitNullProperties);

            stringBuilder.AppendLine($"{options.Indent}{propertyName} =");
            stringBuilder.AppendLine(innerExceptionString);
        }

        /// <summary>
        /// Appends a single value objects to the detailed string.
        /// </summary>
        /// <param name="stringBuilder">The string builder used to compose the detailed strind.</param>
        /// <param name="propertyName">The name of the property that owns the collection.</param>
        /// <param name="value">The value object to append.</param>
        /// <param name="options">The main options.</param>
        private static void AppendValue(StringBuilder stringBuilder, string propertyName, object value, ExceptionOptions options)
        {
            if (value is DictionaryEntry) stringBuilder.AppendLine($"{options.Indent}{propertyName} = {((DictionaryEntry)value).Key} : {((DictionaryEntry)value).Value}");
            else if (value is Exception) AppendException(stringBuilder, propertyName, (Exception)value, options);
            else if (value is IEnumerable && !(value is string))
            {
                var collection = (IEnumerable)value;
                var enumerable = collection as object[] ?? collection.Cast<object>().ToArray();
                if (enumerable.GetEnumerator().MoveNext()) AppendCollection(stringBuilder, propertyName, enumerable, options);
            }
            else stringBuilder.AppendLine($"{options.Indent}{propertyName} = {value}");
        }
    }
}