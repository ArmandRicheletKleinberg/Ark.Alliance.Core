using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Ark
{
    /// <summary>
    /// This custom converter is used to create some subclasses from a common base class in a JSON file given the property name.
    /// </summary>
    public class JsonConverterSubclass<TBaseClass> : JsonConverter
    {
        #region Static Fields

        /// <summary>
        /// The prefix to remove from the sub class name to check with the property.
        /// </summary>
        private readonly string _subClassNamePrefix;

        /// <summary>
        /// The test types are kept in memory to only fetch them once.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static Dictionary<Type, Dictionary<string, Type>> _types;

        #endregion Static Fields

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="T:SubclassJsonConverter`1" /> instance.
        /// </summary>
        public JsonConverterSubclass()
            : this(null)
        { }

        /// <summary>
        /// Creates a <see cref="JsonConverterSubclass{TBaseClass}"/> instance.
        /// </summary>
        /// <param name="subClassNamePrefix">The prefix to remove from the sub class name to check with the property.</param>
        public JsonConverterSubclass(string subClassNamePrefix = null)
        {
            _subClassNamePrefix = subClassNamePrefix ?? typeof(TBaseClass).Name.Replace("Base", "");
            _types ??= new Dictionary<Type, Dictionary<string, Type>>();
        }

        #endregion Constructors

        #region Methods (Override)

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException("Not implemented yet");


        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var typesByName = LazyInitializeTestTypes();

            if (typeof(System.Collections.IList).IsAssignableFrom(objectType))
            {
                return JArray.Load(reader)
                    .OfType<JObject>()
                    .Select(item => GetObjectFromFirstPropertyName(item, typesByName))
                    .OfType<TBaseClass>()
                    .ToArray();
            }

            var jObject = JObject.Load(reader);
            return GetObjectFromFirstPropertyName(jObject, typesByName);
        }

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => true;

        #endregion Methods (Override)

        #region Methods (Helpers)

        /// <summary>
        /// Gets the object from the from the first property name given a relationship between the class name and the property name.
        /// </summary>
        /// <param name="jObject">The JSON object to convert to the object type found given the property name.</param>
        /// <param name="typesByName">The dictionary with the property names/class types mappings.</param>
        /// <returns>The real object instanced with the correct type if found, null otherwise.</returns>
        private static object GetObjectFromFirstPropertyName(JObject jObject, IDictionary<string, Type> typesByName)
        {
            var property = jObject.Properties().FirstOrDefault(p => !p.Name.StartsWith("$"));
            if (property == null)
                return null;

            var subType = typesByName.GetValue(property.Name.ToLower());
            if (subType == null)
                return null;

            var testObject = property.Value as JObject;
            return testObject?.ToObject(subType);
        }


        /// <summary>
        /// Initializes the test types by searching them in all assemblies.
        /// </summary>
        private Dictionary<string, Type> LazyInitializeTestTypes()
        {
            // Only once
            var typesByName = _types.GetValue(typeof(TBaseClass));
            if (typesByName != null)
                return typesByName;

            // Gets this assembly and the executing one and gets all the types
            var executingAssembly = Assembly.GetExecutingAssembly();
            var thisAssembly = typeof(TBaseClass).Assembly;
            var types = new List<Type>(executingAssembly.GetTypes());
            if (executingAssembly != thisAssembly)
                types.AddRange(thisAssembly.GetTypes());

            // Returns only the type inherited from TestBase
            typesByName = types
                .Where(type => type.IsSubclassOf(typeof(TBaseClass)))
                .ToDictionary(type => type.Name.Substring(_subClassNamePrefix?.Length ?? 0).ToLower());
            _types.Add(typeof(TBaseClass), typesByName);
            return typesByName;
        }

        #endregion Methods (Helpers)
    }
}