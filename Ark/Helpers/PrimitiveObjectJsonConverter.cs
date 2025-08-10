using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ark
{
    /// <inheritdoc />
    /// <summary>
    /// This allows to serialize/deserialize a single primitive property depending on the JSON type.
    /// This works only on <see cref="object"/> type and only for primitive (string, number, bool, datetime) types, otherwise sets null.
    /// </summary>
    public class PrimitiveObjectJsonConverter : JsonConverter<object>
    {
        #region Methods (Override)

        /// <inheritdoc />
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String: return reader.TryGetDateTime(out var dateTime) ? dateTime : (object)reader.GetString();
                case JsonTokenType.Number: return reader.GetDecimal();
                case JsonTokenType.True: return true;
                case JsonTokenType.False: return false;
                case JsonTokenType.Null: return null;
                default: return null;
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null: writer.WriteNullValue(); break;
                case string stringValue: writer.WriteStringValue(stringValue); break;
                case bool boolValue: writer.WriteBooleanValue(boolValue); break;
                case DateTime dateTime: writer.WriteStringValue(dateTime.ToString("O")); break;
                default:
                    if (value.GetType().IsNumeric())
                        writer.WriteNumberValue(Convert.ToDecimal(value));
                    else
                        writer.WriteNullValue();
                    break;
            }
        }

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
            => true;

        #endregion Methods (Override)
    }
}