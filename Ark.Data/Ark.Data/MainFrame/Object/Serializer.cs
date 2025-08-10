using System;
using System.Collections.Generic;
using System.Linq;

namespace Ark.Data
{
    /// <summary>
    /// This class is used to serialize/deserialize a mainframe object.
    /// It contains all the properties to be serialized/deserialized.
    /// Base class without generic to use it freely.
    /// </summary>
    internal abstract class MainFrameObjectSerializer
    {
        #region Methods (Public)

        /// <summary>
        /// Serializes a mainframe object to a string.
        /// It serializes all the properties in the code order.
        /// </summary>
        /// <param name="mainFrameObject">The main frame object to serialize.</param>
        /// <returns>The serialized properties concatenated string.</returns>
        internal abstract string Serialize(object mainFrameObject);

        /// <summary>
        /// Deserializes a string message coming from the mainframe.
        /// </summary>
        /// <param name="message">The string message to deserialize.</param>
        /// <returns>The newly created mainframe object with the properties set from the string message data.</returns>
        internal abstract object Deserialize(string message);

        /// <summary>
        /// Gets the length of the data message string when serialized.
        /// BEWARE ! This method can be called recursively in case of nested mainframe objects.
        /// </summary>
        /// <returns>The characters length of the mainframe object serialized message.</returns>
        internal abstract int GetObjectTotalStringDataLength();

        #endregion Methods (Public)
    }

    /// <summary>
    /// This class is used to serialize/deserialize a mainframe object.
    /// It contains all the properties to be serialized/deserialized.
    /// </summary>
    /// <typeparam name="TMfo">The type of the mainframe object to serialize/deserialize.</typeparam>
    internal class MainFrameObjectSerializer<TMfo> : MainFrameObjectSerializer
        where TMfo : class, new()
    {
        #region Fields

        /// <summary>
        /// The properties of the mainframe object are created in the constructor to speedup performance.
        /// </summary>
        private readonly List<MainFramePropertySerializer<TMfo>> _properties;

        /// <summary>
        /// The total length of the serialized string for this mainframe object.
        /// It is computed from the length of the inner mainframe properties and kept it for performance purpose.
        /// </summary>
        private int? _totalStringDataLength;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="MainFrameObjectSerializer{TMfo}"/> instance.
        /// It creates the properties of the mainframe object.
        /// </summary>
        internal MainFrameObjectSerializer()
        {
            _properties = typeof(TMfo).GetProperties().Where(p => Attribute.IsDefined(p, typeof(MainFramePropertyAttribute))).Select(MainFramePropertySerializerFactory.Create<TMfo>).OrderBy(ps => ps.Order).ToList();
        }

        #endregion Constructors

        #region Methods (Public)

        /// <inheritdoc />
        internal override string Serialize(object mainFrameObject)
            => SerializeType((TMfo)mainFrameObject);

        /// <summary>
        /// Serializes a mainframe object to a string.
        /// It serializes all the properties in the code order.
        /// </summary>
        /// <param name="mainFrameObject">The main frame object to serialize.</param>
        /// <returns>The serialized properties concatenated string.</returns>
        internal string SerializeType(TMfo mainFrameObject)
        {
            mainFrameObject = mainFrameObject ?? new TMfo();
            return string.Concat(_properties.Select(p => p.Serialize(mainFrameObject)));
        }

        /// <inheritdoc />
        internal override object Deserialize(string message)
            => DeserializeToType(message);

        /// <summary>
        /// Deserializes a string message coming from the mainframe.
        /// </summary>
        /// <param name="message">The string message to deserialize.</param>
        /// <returns>The newly created mainframe object with the properties set from the string message data.</returns>
        // ReSharper disable once UnusedTypeParameter
        internal TMfo DeserializeToType(string message)
        {
            var mainFrameObject = new TMfo();

            var totalStringLength = GetObjectTotalStringDataLength();
            if (message.Length < totalStringLength)
                throw new Exception($"Unable to deserialize because the message length {message.Length} is lower than the object to create {totalStringLength}");

            var index = 0;
            _properties.ForEach(p => index += p.Deserialize(mainFrameObject, message.Substring(index, p.TotalStringDataLength)));
            return mainFrameObject;
        }

        /// <inheritdoc />
        internal override int GetObjectTotalStringDataLength()
            => (_totalStringDataLength = _totalStringDataLength ?? _properties.Sum(p => p.TotalStringDataLength)).Value;

        #endregion Methods (Public)
    }
}