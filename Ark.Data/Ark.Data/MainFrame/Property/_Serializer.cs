using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ark.Data
{
    /// <summary>
    /// The serializer used for a single property.
    /// It is the base class inherited by all the property serializer depending on their types.
    /// </summary>
    /// <typeparam name="TMfo">The type of the mainframe object to serialize.</typeparam>
    internal abstract class MainFramePropertySerializer<TMfo>
        where TMfo : class, new()
    {
        #region Fields

        /// <summary>
        /// The name of the property to serialize.
        /// </summary>
        protected string PropertyName;

        /// <summary>
        /// The attribute of the mainframe property with the data needed to serialize/deserialize properly.
        /// </summary>
        protected MainFramePropertyAttribute Attribute;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="MainFramePropertySerializer{TMfo}"/> instance.
        /// </summary>
        /// <param name="propertyInfo">The info of a .NET class property. It must have a MainFrameProperty attribute.</param>
        internal virtual void Init(PropertyInfo propertyInfo)
        {
            PropertyName = propertyInfo.Name;
            Attribute = propertyInfo.GetCustomAttribute<MainFramePropertyAttribute>();
            StringDataLength = GetStringDataLength();
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The length of the serialized string data for a single occurence of the property.
        /// </summary>
        internal int StringDataLength { get; set; }

        /// <summary>
        /// The total length of the serialized string data including arrays total length.
        /// </summary>
        internal int TotalStringDataLength => ArrayOccurrencesNumber * StringDataLength;

        #endregion Properties (Public)

        #region Properties (Computed)

        /// <summary>
        /// The order of the property in the mainframe object.
        /// It is auto generated in the constructor.
        /// It is really important as the order of the properties are used in the serialization process.
        /// </summary>
        internal int Order => Attribute.Order;

        /// <summary>
        /// The order of the property in the mainframe object.
        /// It is auto generated in the constructor.
        /// It is really important as the order of the properties are used in the serialization process.
        /// </summary>
        internal int Length => Attribute.Length;

        /// <summary>
        /// Only for array.
        /// The number of occurrences of the data in an array.
        /// </summary>
        internal int ArrayOccurrencesNumber => Attribute.ArrayOccurrencesNumber;

        #endregion Properties (Computed)

        #region Methods (Abstract)

        /// <summary>
        /// Serializes a mainframe property into a flat text string that can be sent to the mainframe.
        /// It is called by the mainframe object to serialize all the mainframe object inner properties.
        /// </summary>
        /// <param name="mainFrameObject">The mainframe object to serialize a single property.</param>
        /// <returns>The flat text string that can be sent to the mainframe.</returns>
        internal abstract string Serialize(TMfo mainFrameObject);

        /// <summary>
        /// Deserializes a mainframe property from a flat text string.
        /// </summary>
        /// <param name="mainFrameObject">The mainframe object to set the deserialized property into.</param>
        /// <param name="data">The flat string text data to deserialize into a .NET C# property value.</param>
        /// <returns>The number of characters used for the deserialization.</returns>
        internal abstract int Deserialize(TMfo mainFrameObject, string data);

        /// <summary>
        /// Gets the length of the string serialized from the mainframe property.
        /// </summary>
        /// <returns>The length of the string serialized from the mainframe property.</returns>
        internal abstract int GetStringDataLength();

        #endregion Methods (Abstract)
    }

    internal abstract class MainFramePropertySerializer<TMfo, TProperty> : MainFramePropertySerializer<TMfo>
        where TMfo : class, new()
    {
        #region Fields

        /// <summary>
        /// The function used to get the value from a main frame object property.
        /// Keep in field for performance purpose.
        /// </summary>
        protected Func<TMfo, TProperty> GetValueFunction;

        /// <summary>
        /// The action used to set the value to a main frame object property.
        /// Keep in field for performance purpose.
        /// </summary>
        protected Action<TMfo, TProperty> SetValueAction;

        /// <summary>
        /// The function used to get the values from a main frame object enumerable property.
        /// Keep in field for performance purpose.
        /// </summary>
        protected Func<TMfo, IEnumerable<TProperty>> GetArrayValueFunction;

        /// <summary>
        /// The function used to set the values to a main frame object enumerable property.
        /// Keep in field for performance purpose.
        /// </summary>
        protected Action<TMfo, TProperty[]> SetArrayValueAction;

        #endregion Fields

        #region Methods (Init)

        /// <inheritdoc />
        /// <summary>
        /// Initializes the property serializer by creating the needed setters/getters to gain performances.
        /// </summary>
        /// <param name="propertyInfo">The info of the property to link.</param>
        internal override void Init(PropertyInfo propertyInfo)
        {
            base.Init(propertyInfo);

            if (propertyInfo.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
            {
                GetArrayValueFunction = (Func<TMfo, IEnumerable<TProperty>>)Delegate.CreateDelegate(typeof(Func<TMfo, IEnumerable<TProperty>>), propertyInfo.GetGetMethod());
                // TODO with IEnumerable
                SetArrayValueAction = (Action<TMfo, TProperty[]>)Delegate.CreateDelegate(typeof(Action<TMfo, TProperty[]>), propertyInfo.GetSetMethod());
            }
            else
            {
                GetValueFunction = (Func<TMfo, TProperty>)Delegate.CreateDelegate(typeof(Func<TMfo, TProperty>), propertyInfo.GetGetMethod());
                SetValueAction = (Action<TMfo, TProperty>)Delegate.CreateDelegate(typeof(Action<TMfo, TProperty>), propertyInfo.GetSetMethod());
            }
        }

        #endregion Methods (Internal)

        #region Methods (Override)

        /// <inheritdoc />
        /// <summary>
        /// Serializes a main frame object property.
        /// If the property is an enumerable then executes it for each occurence.
        /// It also checks if auto computed column should be computed prior to be serialized.
        /// </summary>
        /// <param name="mainFrameObject">The main frame object to serialize its property.</param>
        /// <returns>The string serialized property.</returns>
        internal sealed override string Serialize(TMfo mainFrameObject)
        {
            try
            {
                if (GetArrayValueFunction != null)
                {
                    var values = GetArrayValueFunction(mainFrameObject)?.Take(ArrayOccurrencesNumber).ToList() ?? new List<TProperty>();
                    if (values.Count < ArrayOccurrencesNumber)
                        values.AddRange(new TProperty[ArrayOccurrencesNumber - values.Count]);

                    var serializedConcatenatedString = string.Concat(values.Select(ConvertValueToString));
                    return serializedConcatenatedString;
                }

                var value = GetValueFunction(mainFrameObject);
                var serializedString = ConvertValueToString(value);
                return serializedString;
            }
            catch (Exception exception)
            {
                throw new Exception($"Unexpected error when serializing the property {PropertyName} from the object {typeof(TMfo).Name}", exception);
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// Deserializes a main frame from some flat string data.
        /// </summary>
        /// <param name="mainFrameObject">The empty instance of main frame object to fill.</param>
        /// <param name="data">The data to deserialize into a mainframe object.</param>
        /// <returns>The total length of the deserialized main frame object.</returns>
        internal sealed override int Deserialize(TMfo mainFrameObject, string data)
        {
            try
            {
                if (data.Length < TotalStringDataLength)
                    throw new Exception($"The data used to create the property {PropertyName} from the object {typeof(TMfo).Name} must be at least {TotalStringDataLength} characters long");

                if (SetArrayValueAction != null)
                {
                    var items = data.SplitByLength(StringDataLength).Select(ConvertStringToValue).ToArray();
                    SetArrayValueAction(mainFrameObject, items);
                    return TotalStringDataLength;
                }

                var value = ConvertStringToValue(data);
                SetValueAction(mainFrameObject, value);

                return TotalStringDataLength;
            }
            catch (Exception exception)
            {
                throw new Exception($"Unexpected error when deserializing the property {PropertyName} from the object {typeof(TMfo).Name} with data : {data}", exception);
            }
        }

        #endregion Methods (Override)

        #region Methods (Abstract)

        /// <summary>
        /// Converts a value in the property type into a string.
        /// Abstract as it must be defined in the inheriting classes.
        /// </summary>
        /// <param name="value">The value in the property type.</param>
        /// <returns>The converted string.</returns>
        internal abstract string ConvertValueToString(TProperty value);

        /// <summary>
        /// Converts a string into the property type value.
        /// Abstract as it must be defined in the inheriting classes.
        /// </summary>
        /// <param name="data">The string data to convert in the property type.</param>
        /// <returns>The converted property type value.</returns>
        internal abstract TProperty ConvertStringToValue(string data);

        #endregion Methods (Abstract)
    }
}