using System;

namespace Ark.Data
{
    /// <summary>
    /// This is the base class to serialize/deserialize main frame flat data.
    /// </summary>
    public class MainFrameSerializer
    {
        #region Methods (Public)

        /// <summary>
        /// Serializes a main frame object into a flat string.
        /// </summary>
        /// <typeparam name="TMfo">The type of the main frame object to serialize.</typeparam>
        /// <param name="mainFrameObject">The main frame object to serialize.</param>
        /// <returns>The serialized main frame object into a flat string.</returns>
        public virtual string Serialize<TMfo>(TMfo mainFrameObject)
            where TMfo : class, new()
        {
            var serializer = MainFrameObjectSerializerFactory.Create<TMfo>();
            var message = serializer.Serialize(mainFrameObject);
            return message;
        }

        /// <summary>
        /// Deserializes a flat string message into a main frame object.
        /// </summary>
        /// <typeparam name="TMfo">The type of the main frame object into which deserialize.</typeparam>
        /// <param name="message">The flat string message to deserialize.</param>
        /// <returns>The deserialized main frame object.</returns>
        public virtual TMfo Deserialize<TMfo>(string message)
            where TMfo : class, new()
        {
            var serializer = MainFrameObjectSerializerFactory.Create<TMfo>();
            var mainFrameObject = ((MainFrameObjectSerializer<TMfo>)serializer).DeserializeToType(message);
            return mainFrameObject;
        }

        /// <summary>
        /// Deserializes a flat string message into a main frame object.
        /// </summary>
        /// <param name="message">The flat string message to deserialize.</param>
        /// <param name="mainFrameObjectType">The type of the main frame object into which deserialize.</param>
        /// <returns>The deserialized main frame object.</returns>
        public virtual object Deserialize(string message, Type mainFrameObjectType)
        {
            var serializer = MainFrameObjectSerializerFactory.Create(mainFrameObjectType);
            var mainFrameObject = serializer.Deserialize(message);
            return mainFrameObject;
        }

        #endregion Methods (Public)
    }
}