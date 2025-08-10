using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ark
{
    /// <summary>
    /// This JSON serialization contract resolver allow to serialize an object except some specified properties.
    /// </summary>
    public class IgnorePropertyResolver : DefaultContractResolver
    {
        #region Fields

        /// <summary>
        /// The target type.
        /// </summary>
        private readonly Type _targetType;

        /// <summary>
        /// The target property to ignore names.
        /// </summary>
        private readonly string[] _targetPropertyNames;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates an <see cref="IgnorePropertyResolver"/> instance.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="propertyNames">The target property to ignore names.</param>
        public IgnorePropertyResolver(Type targetType, params string[] propertyNames)
        {
            _targetType = targetType;
            _targetPropertyNames = propertyNames;
        }

        #endregion Constructors

        #region Methods (Override)

        /// <summary>
        /// Creates the properties to serialize.
        /// </summary>
        /// <param name="type">The type of the object to serialize.</param>
        /// <param name="memberSerialization">The member serialization info.</param>
        /// <returns>The list of JSON property for the serialization.</returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            return _targetType == type
                ? properties.Where(p => !_targetPropertyNames.Contains(p.PropertyName)).ToList()
                : properties;
        }

        #endregion Methods (Override)
    }
}