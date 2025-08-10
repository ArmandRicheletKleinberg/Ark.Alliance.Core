using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Ark
{
    /// <inheritdoc />
    /// <summary>
    /// This converter allows to serialize/deserialize a .NET System.Exception which normally is not possible using NewtonSoft JSON.
    /// </summary>
    public class ExceptionJsonConverter : JsonConverter
    {
        #region Methods (Override)

        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            WriteExceptionObject(writer, (Exception)value, serializer);
        }

        /// <summary>
        /// Recursively writes the JSON of exception and inner exception.
        /// </summary>
        /// <param name="writer">The writer to write the JSON string.</param>
        /// <param name="exception">The exception to write, could be inner exception.</param>
        /// <param name="serializer">The serializer options to use to write the JSON.</param>
        private static void WriteExceptionObject(JsonWriter writer, Exception exception, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            var excludedProperties = new HashSet<string> { nameof(Exception.TargetSite), nameof(Exception.HResult) };
            typeof(Exception).GetProperties().Where(p => !excludedProperties.Contains(p.Name)).ForEach(property =>
            {
                if (!property.CanRead)
                    return;

                var memberValue = property.GetValue(exception, null);
                if (memberValue == null && serializer.NullValueHandling != NullValueHandling.Include)
                    return;

                writer.WritePropertyName(property.Name.FirstLetterToUpper());
                if (memberValue is Exception innerException)
                    WriteExceptionObject(writer, innerException, serializer);
                else
                    serializer.Serialize(writer, memberValue);
            });
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jObject = serializer.Deserialize(reader) as JObject;
            return CreateExceptionFromJObject(jObject);
        }

        /// <summary>
        /// Re Creates recursively an exception from a deserialized JObject.
        /// </summary>
        /// <param name="jObject">The deserialized JObject.</param>
        /// <returns>The re created Exception instance.</returns>
        private Exception CreateExceptionFromJObject(JObject jObject)
        {
            Exception exception;

            var message = jObject.GetValue(nameof(Exception.Message))?.Value<string>();
            if (jObject.GetValue(nameof(Exception.InnerException)) is JObject innerExceptionJObject)
                exception = new Exception(message, CreateExceptionFromJObject(innerExceptionJObject));
            else
                exception = new Exception(message);

            var dataJObject = jObject.GetValue(nameof(Exception.Data)) as JObject;
            dataJObject?.ToObject<Dictionary<string, string>>()?.ForEach(kvp => exception.Data.Add(kvp.Key, kvp.Value));

            var stackTraceField = exception.GetType().GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);
            stackTraceField?.SetValue(exception, jObject.GetValue(nameof(Exception.StackTrace))?.Value<string>());

            exception.HelpLink = jObject.GetValue(nameof(Exception.HelpLink))?.Value<string>();
            exception.Source = jObject.GetValue(nameof(Exception.Source))?.Value<string>();

            return exception;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
            => typeof(Exception).IsAssignableFrom(objectType);

        #endregion Methods (Override)
    }
}