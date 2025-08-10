using System.Linq.Expressions;
using System.Reflection;

namespace Ark
{
    /// <summary>
    /// This helper class extends the IEnumerable object.
    /// </summary>
    public static class TypeExtensions
    {
        #region Methods (New)

        /// <summary>
        /// Creates an instance of that type using the constructors matching the parameters.
        /// </summary>
        /// <param name="type">The type of the instance to be created.</param>
        /// <returns></returns>
        public static object New(this Type type)
        {
            return New(type, new object[0]);
        }

        /// <summary>
        /// Creates an instance of that type using the constructors matching the parameters.
        /// </summary>
        /// <param name="type">The type of the instance to be created.</param>
        /// <param name="parameters">The parameters of the constructors to be called.</param>
        /// <returns></returns>
        public static object New(this Type type, params object[] parameters)
        {
            var constructors = type.GetTypeInfo().DeclaredConstructors.Where(ctor => !ctor.IsStatic);
            if (parameters == null || parameters.Length == 0)
            {
                var emptyConstructor = constructors.First(c => c.GetParameters().Length == 0);
                return Expression.Lambda<Func<object>>(Expression.New(emptyConstructor)).Compile()();
            }

            var types = parameters.Select(p => p.GetType()).ToArray();
            var constructor = constructors.First(c => c.GetParameters().Select(p => p.ParameterType).ToArray().SequenceEqual(types));

            var parameterExpressions = parameters.Select(p => Expression.Parameter(p.GetType())).ToArray();
            var newExpression = Expression.New(constructor, parameterExpressions.Cast<Expression>());
            var lambda = Expression.Lambda(newExpression, parameterExpressions);
            var callConstructorMethod = lambda.Compile();

            return callConstructorMethod.DynamicInvoke(parameters);
        }

        /// <summary>
        /// Creates an instance of that type using the constructors matching the parameters.
        /// </summary>
        /// <typeparam name="T">The generic type of the instance to be created.</typeparam>
        /// <param name="type">The type of the instance to be created.</param>
        /// <returns></returns>
        public static T New<T>(this Type type)
            where T : class
        {
            return New<T>(type, new object[0]);
        }

        /// <summary>
        /// Creates an instance of that type using the constructors matching the parameters.
        /// </summary>
        /// <typeparam name="T">The generic type of the instance to be created.</typeparam>
        /// <param name="type">The type of the instance to be created.</param>
        /// <param name="parameters">The parameters of the constructors to be called.</param>
        /// <returns></returns>
        public static T New<T>(this Type type, params object[] parameters)
            where T : class
        {
            return New(type, parameters) as T;
        }

        #endregion Methods (New)

        #region Methods (Is)

        /// <summary>
        /// Checks whether a type is an another type or is inheriting from it.
        /// </summary>
        /// <typeparam name="TOther">The another type to check for equality or inheritance.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is assignable, false otherwise.</returns>
        public static bool Is<TOther>(this Type type)
            => typeof(TOther).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

        /// <summary>
        /// Checks whether a type is a nullable type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a nullable type, false otherwise.</returns>
        public static bool IsNullable(this Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);


        /// <summary>
        /// The numeric types defined as a hashset for performance checks.
        /// </summary>
        private static readonly HashSet<Type> NumericTypes = new()
        {
            typeof(int), typeof(double), typeof(decimal), typeof(long), typeof(short), typeof(sbyte), typeof(byte), typeof(ulong), typeof(ushort), typeof(uint), typeof(float) };

        /// <summary>
        /// Determines if a type is numeric.
        /// Nullable numeric types are also considered numeric.
        /// </summary>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        /// <param name="type">The type to check for numeric.</param>
        /// <returns>True if numeric, false otherwise.</returns>
        public static bool IsNumeric(this Type type)
            => NumericTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);

        /// <summary>
        /// Determines if a type is a date time.
        /// Nullable date time types are considered date time.
        /// </summary>
        /// <param name="type">The type to check for date time.</param>
        /// <returns>True if date time, false otherwise.</returns>
        public static bool IsDateTime(this Type type)
        {
            if (type == null)
                return false;

            if (Type.GetTypeCode(type) == TypeCode.DateTime)
                return true;

            return type.IsNullable() && IsDateTime(Nullable.GetUnderlyingType(type));
        }

        /// <summary>
        /// Whether this type is a simple type that is without nested fields or properties.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Whether the type is a simple type.</returns>
        public static bool IsSimple(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                // ReSharper disable once TailRecursiveCall
                return IsSimple(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
                   || typeInfo.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal);
        }

        /// <summary>
        /// Whether this type is a complex type that is with nested fields or properties.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Whether the type is a complex type.</returns>
        public static bool IsComplex(this Type type)
            => !type.IsSimple();

        #endregion Methods (Is)

        #region Methods (GetDefaultValue)

        /// <summary>
        /// Gets the default value for a type.
        /// For reference type returns null and for value type creates an instance.
        /// </summary>
        /// <param name="type">The type to get the default value.</param>
        /// <returns>The default value for the type.</returns>
        public static object GetDefaultValue(this Type type)
            => type.IsValueType && Nullable.GetUnderlyingType(type) == null ? Activator.CreateInstance(type) : null;

        #endregion Methods (GetDefaultValue)

        #region Methods (GetPrivateField)

        /// <summary>
        /// Gets a private field of this type or from a inherited type.
        /// </summary>
        /// <param name="type">The type to search for a private field.</param>
        /// <param name="name">The name of the private field to search.</param>
        /// <returns>The field info with the field if found, null otherwise</returns>
        public static FieldInfo GetPrivateField(this Type type, string name)
        {
            var fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            while (fieldInfo == null && type.BaseType != null)
            {
                type = type.BaseType;
                fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            }

            return fieldInfo;
        }

        #endregion Methods (GetPrivateField)
    }
}