using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ark
{
    /// <inheritdoc />
    /// <summary>
    /// This allows to serialize/deserialize a generic dynamic object.
    /// </summary>
    public class ExpandoObjectJsonConverter : JsonConverter<ExpandoObject>
    {
        #region Methods (Override)

        /// <inheritdoc />
        public override ExpandoObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var test = ReadToExpandoObject(ref reader);
            return test;
        }

        private ExpandoObject ReadToExpandoObject(ref Utf8JsonReader reader)
        {
            var obj = new ExpandoObject();
            string propertyName = null;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartArray: SetValue(obj, propertyName, ReadArrayToExpandoObjectList(ref reader)); continue;
                    case JsonTokenType.StartObject: SetValue(obj, propertyName, ReadToExpandoObject(ref reader)); continue;
                    case JsonTokenType.EndObject: return obj;
                    case JsonTokenType.EndArray: return obj;
                    case JsonTokenType.PropertyName: propertyName = reader.GetString(); continue;
                    case JsonTokenType.String: SetValue(obj, propertyName, reader.TryGetDateTime(out var dateTime) ? dateTime : (object)reader.GetString()); continue;
                    case JsonTokenType.Number: SetValue(obj, propertyName, reader.GetDecimal()); continue;
                    case JsonTokenType.True: SetValue(obj, propertyName, true); continue;
                    case JsonTokenType.False: SetValue(obj, propertyName, false); continue;
                    case JsonTokenType.Null: SetValue(obj, propertyName, null); continue;
                }
            }

            throw new JsonException("The JSON object must end with }");
        }

        private List<ExpandoObject> ReadArrayToExpandoObjectList(ref Utf8JsonReader reader)
        {
            var list = new List<ExpandoObject>();
            do
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartArray: continue;
                    case JsonTokenType.EndArray: return list;
                    case JsonTokenType.StartObject: list.Add(ReadToExpandoObject(ref reader)); continue;
                    case JsonTokenType.EndObject: continue;
                }
            } while (reader.Read());

            throw new JsonException("The JSON array must end with ]");
        }

        private void SetValue(ExpandoObject obj, string propertyName, object value)
        {
            if (propertyName.IsNullOrEmpty())
                return;

            obj.AddOrUpdate(propertyName, value);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
        {
            // TODO
        }

        #endregion Methods (Override)
    }
}