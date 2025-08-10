using System.Buffers;
using System.Text.Json;

namespace Ark
{
    /// <summary>
    /// This extensions class enhances the <see cref="JsonElement"/> class.
    /// </summary>
    public static class JsonElementExtensions
    {
        /// <summary>
        /// Converts the JsonElement to a strongly-typed object.
        /// </summary>
        /// <typeparam name="TObj">The type of strongly typed object to deserialize the JsonElement into.</typeparam>
        /// <param name="element">The JSON element to deserialize.</param>
        /// <param name="options">The options to deserialize properly.</param>
        /// <returns>The deserialized strongly typed object.</returns>
        public static TObj ToObject<TObj>(this JsonElement element, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
                element.WriteTo(writer);
            return JsonSerializer.Deserialize<TObj>(bufferWriter.WrittenSpan, options);
        }
    }
}