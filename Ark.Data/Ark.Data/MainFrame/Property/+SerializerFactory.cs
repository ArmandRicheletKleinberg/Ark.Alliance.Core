using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ark.Data
{
    /// <summary>
    /// This factory is used to create some property serializer given the property information.
    /// </summary>
    internal static class MainFramePropertySerializerFactory
    {
        /// <summary>
        /// Creates a property serializer given the property info.
        /// </summary>
        /// <typeparam name="TMfo">The type of the main frame object to serialize.</typeparam>
        /// <param name="propertyInfo">The info about the the property.</param>
        /// <returns>The serializer needed to serialize a property of the main frame object.</returns>
        internal static MainFramePropertySerializer<TMfo> Create<TMfo>(PropertyInfo propertyInfo)
            where TMfo : class, new()
        {
            MainFramePropertySerializer<TMfo> propertySerializer;

            var underlyingType = propertyInfo.PropertyType;
            var iEnumerableType = underlyingType.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (iEnumerableType != null && underlyingType != typeof(string))
                underlyingType = iEnumerableType.GetGenericArguments()[0];

            var isNullable = underlyingType.IsNullable();
            if (isNullable)
                underlyingType = Nullable.GetUnderlyingType(underlyingType) ?? typeof(object);

            switch (underlyingType.Name)
            {
                case nameof(String): propertySerializer = new MainFramePropertyStringSerializer<TMfo>(); break;
                case nameof(Int32):
                    propertySerializer = isNullable
                        ? (MainFramePropertySerializer<TMfo>)new MainFramePropertyInt32NullableSerializer<TMfo>()
                        : new MainFramePropertyInt32Serializer<TMfo>(); break;
                case nameof(Decimal):
                    propertySerializer = isNullable
                        ? (MainFramePropertySerializer<TMfo>)new MainFramePropertyDecimalNullableSerializer<TMfo>()
                        : new MainFramePropertyDecimalSerializer<TMfo>(); break;
                case nameof(DateTime):
                    propertySerializer = isNullable
                        ? (MainFramePropertySerializer<TMfo>)new MainFramePropertyDateTimeNullableSerializer<TMfo>()
                        : new MainFramePropertyDateTimeSerializer<TMfo>(); break;
                case nameof(Boolean):
                    propertySerializer = isNullable
                        ? (MainFramePropertySerializer<TMfo>)new MainFramePropertyBooleanNullableSerializer<TMfo>()
                        : new MainFramePropertyBooleanSerializer<TMfo>(); break;
                default:
                    if (underlyingType.GetCustomAttribute<MainFrameObjectAttribute>() != null)
                    {
                        var tObject = typeof(MainFramePropertyMainFrameObjectSerializer<,>).MakeGenericType(typeof(TMfo), underlyingType);
                        propertySerializer = (MainFramePropertySerializer<TMfo>)Activator.CreateInstance(tObject);
                        break;
                    }

                    throw new Exception($"The type {underlyingType} of the property {propertyInfo.Name} of the object {typeof(TMfo).Name} is not managed.");
            }
            propertySerializer.Init(propertyInfo);

            return propertySerializer;
        }
    }
}