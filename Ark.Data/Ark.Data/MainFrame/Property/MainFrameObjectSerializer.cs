namespace Ark.Data
{
    /// <inheritdoc />
    internal class MainFramePropertyMainFrameObjectSerializer<TMfo, TChildMfo> : MainFramePropertySerializer<TMfo, TChildMfo>
        where TMfo : class, new()
        where TChildMfo : class, new()
    {
        /// <inheritdoc />
        internal override string ConvertValueToString(TChildMfo value)
        {
            var serializer = MainFrameObjectSerializerFactory.Create<TChildMfo>();
            return serializer.Serialize(value);
        }

        /// <inheritdoc />
        internal override TChildMfo ConvertStringToValue(string data)
        {
            var serializer = MainFrameObjectSerializerFactory.Create<TChildMfo>();
            return serializer.DeserializeToType(data);
        }

        /// <inheritdoc />
        internal override int GetStringDataLength()
        {
            var serializer = MainFrameObjectSerializerFactory.Create<TChildMfo>();
            return serializer.GetObjectTotalStringDataLength();
        }
    }
}