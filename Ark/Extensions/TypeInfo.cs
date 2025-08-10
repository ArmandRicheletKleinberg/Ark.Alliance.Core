using System.Reflection;

namespace Ark
{
    /// <summary>
    /// This helper class extends the TypeInfo object.
    /// </summary>
    public static class TypeInfoExtensibility
    {
        #region Methods

        /// <summary>
        /// Get all constructors with inherited also.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo instance/</param>
        /// <returns>All constructors with inherited also.</returns>
        public static IEnumerable<ConstructorInfo> GetAllConstructors(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredConstructors);

        /// <summary>
        /// Get all events with inherited also.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo instance/</param>
        /// <returns>All events with inherited also.</returns>
        public static IEnumerable<EventInfo> GetAllEvents(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredEvents);

        /// <summary>
        /// Get all fields with inherited also.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo instance/</param>
        /// <returns>All fields with inherited also.</returns>
        public static IEnumerable<FieldInfo> GetAllFields(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredFields);

        /// <summary>
        /// Get all members with inherited also.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo instance/</param>
        /// <returns>All members with inherited also.</returns>
        public static IEnumerable<MemberInfo> GetAllMembers(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredMembers);

        /// <summary>
        /// Get all methods with inherited also.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo instance/</param>
        /// <returns>All methods with inherited also.</returns>
        public static IEnumerable<MethodInfo> GetAllMethods(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredMethods);

        /// <summary>
        /// Get all nested types with inherited also.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo instance/</param>
        /// <returns>All nested types with inherited also.</returns>
        public static IEnumerable<TypeInfo> GetAllNestedTypes(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredNestedTypes);

        /// <summary>
        /// Get all properties with inherited also.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo instance/</param>
        /// <returns>All properties with inherited also.</returns>
        public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredProperties);

        /// <summary>
        /// Get alls type of code in a type including inherited types.
        /// </summary>
        /// <typeparam name="T">The type of the code.</typeparam>
        /// <param name="typeInfo">The type.</param>
        /// <param name="accessor">A function to get the code types/</param>
        /// <returns>The list of the code types including inherited.</returns>
        private static IEnumerable<T> GetAll<T>(TypeInfo typeInfo, Func<TypeInfo, IEnumerable<T>> accessor)
        {
            while (typeInfo != null)
            {
                foreach (var t in accessor(typeInfo))
                    yield return t;

                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }
        }

        #endregion Methods
    }
}