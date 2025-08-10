using System.Text.Json;

namespace Ark
{
    /// <summary>
    /// This extensions class enhances the <see cref="JsonDocument"/> class.
    /// </summary>
    public static class JsonDocumentExtensions
    {
        /// <summary>
        /// Converts the JsonDocument to a strongly-typed object.
        /// </summary>
        /// <typeparam name="TObj">The type of strongly typed object to deserialize the JsonDocument into.</typeparam>
        /// <param name="document">The JSON document to deserialize.</param>
        /// <param name="options">The options to deserialize properly.</param>
        /// <returns>The deserialized strongly typed object.</returns>
        public static TObj ToObject<TObj>(this JsonDocument document, JsonSerializerOptions options = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            return document.RootElement.ToObject<TObj>(options);
        }
    }
}