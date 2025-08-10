using System.Collections.Concurrent;
using System.ComponentModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

#nullable enable

namespace Ark
{
    /// <summary>
    /// This class extends the object class.
    /// </summary>
    public static class ObjectExtensibility
    {
        /// <summary>
        /// Wait for an event occurs.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event handler arguments.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="attachEvent">The action to attach the event.</param>
        /// <param name="detachEvent">The action to detach the event.</param>
        /// <param name="timeout">The timeout laps in ms.</param>
        /// <returns>Success if event occurs, timeout if timeout and unexpected if an error arise.</returns>
        public static async Task<Result<TEventArgs>> WaitFor<TEventArgs>(this object source, Action<EventHandler<TEventArgs>> attachEvent, Action<EventHandler<TEventArgs>> detachEvent, int timeout)
        {
            EventHandler<TEventArgs>? eventHandler = null;
            try
            {
                // Creates the TaskCompletionSource used to wait for the condition
                var tcs = new TaskCompletionSource<TEventArgs>();

                // Hooks the event and wait for a successful condition check
                eventHandler = (sender, args) => tcs.SetResult(args);
                attachEvent(eventHandler);

                // In case of timeout creates another timeout task to complete after a laps and return timeout result if before the event occurs
                if (timeout > 0)
                {
                    var task = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
                    if (task != tcs.Task) return Result<TEventArgs>.Timeout;
                    return new Result<TEventArgs>(tcs.Task.Result);
                }

                // Otherwise, just wait for the event
                await tcs.Task;
                return new Result<TEventArgs>(tcs.Task.Result);
            }
            catch (Exception) { return Result<TEventArgs>.Unexpected; }
            finally { if (eventHandler != null) detachEvent(eventHandler); } // Unhook gracefully the event in any case
        }

        /// <summary>
        /// Wait for a condition expression given an event to hook and a timeout if needed.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event handler arguments.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="conditionToWaitForFunc">The function with the condition to wait check.</param>
        /// <param name="attachEvent">The action to attach the event.</param>
        /// <param name="detachEvent">The action to detach the event.</param>
        /// <param name="timeout">The timeout laps in ms.</param>
        /// <returns>Success if condition fulfilled, timeout if timeout and unexpected if an error arise.</returns>
        public static async Task<Result> WaitFor<TEventArgs>(this object source, Func<bool> conditionToWaitForFunc, Action<EventHandler<TEventArgs>> attachEvent, Action<EventHandler<TEventArgs>> detachEvent, int timeout)
        {
            EventHandler<TEventArgs>? eventHandler = null;
            try
            {
                // If the condition is already fulfilled then success.
                if (conditionToWaitForFunc()) return Result.Success;

                // Creates the TaskCompletionSource used to wait for the condition
                var tcs = new TaskCompletionSource<bool>();

                // Hooks the event and wait for a successful condition check
                eventHandler = (sender, args) =>
                {
                    try { if (conditionToWaitForFunc()) tcs.SetResult(true); }
                    catch (Exception) { tcs.SetResult(false); }
                };
                attachEvent(eventHandler);

                // In case of timeout creates another timeout task to complete after a laps and return timeout result if before the condition check success
                if (timeout > 0)
                {
                    var task = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
                    if (task != tcs.Task) return Result.Timeout;
                    return tcs.Task.Result ? Result.Success : Result.Unexpected;
                }

                // Otherwise, just wait for the condition check success
                var result = await tcs.Task;
                return result ? Result.Success : Result.Unexpected;
            }
            catch (Exception) { return Result.Unexpected; }
            finally { if (eventHandler != null) detachEvent(eventHandler); } // Unhook gracefully the event in any case
        }

        /// <summary>
        /// Gets an object private field value given its name.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to get.</typeparam>
        /// <param name="obj">The object to get its private field.</param>
        /// <param name="privateFieldName">The name of the private field to get.</param>
        /// <returns>The private field value if found, null otherwise.</returns>
        public static TValue? GetPrivateFieldValue<TValue>(this object obj, string privateFieldName)
            => (TValue?)obj?.GetType().GetField(privateFieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

        #region Methods (GetPropertyValue)

        /// <summary>
        /// This dictionary is used to cache the expression used by the GetPropertyValue for better performance.
        /// </summary>
        private static ConcurrentDictionary<string, Func<object, object?>?>? _getPropertyValueCache;

        /// <summary>
        /// Gets a property value.
        /// This method is ultra optimized and use Expression and cache system for better performance.
        /// </summary>
        /// <returns>The property value found.</returns>
        public static TProp? GetPropertyValue<TProp>(this object obj, string propertyName)
        {
            // First gets or lazy creates the get function from expression cache given an unique key
            _getPropertyValueCache ??= new ConcurrentDictionary<string, Func<object, object?>?>();
            var key = $"{obj.GetType()}#{propertyName}";
            var getFunc = _getPropertyValueCache.GetValue(key);
            if (getFunc == null)
            {
                var parameterObjExpression = Expression.Parameter(typeof(object), "obj");
                var convertObjExpression = Expression.Convert(parameterObjExpression, obj.GetType());
                var propertyInfo = obj.GetType().GetLowestProperty(propertyName)
                    ?? throw new ArgumentException($"The property {propertyName} has not been found on Type {obj.GetType().FullName}", nameof(propertyName));
                var propertyExpression = Expression.Property(convertObjExpression, propertyInfo);
                var convertPropertyExpression = Expression.Convert(propertyExpression, typeof(object));
                var lambda = Expression.Lambda<Func<object, object?>>(convertPropertyExpression, parameterObjExpression);
                _getPropertyValueCache.AddOrUpdate(key, getFunc = lambda.Compile());
            }

            // Uses the function and returns the cast value
            var value = getFunc!(obj);
            return (TProp?)value;
        }

     
        /// <summary>
        /// Gets a property value.
        /// This method is ultra optimized and use Expression and cache system for better performance.
        /// Also works with ExpandoObject.
        /// </summary>
        /// <returns>The property value found.</returns>
        public static TProp? GetPropertyValueWithExpandoSupport<TProp>(this object obj, string propertyName)
            => obj is ExpandoObject expandoObject
                ? (TProp?)expandoObject.GetValue(propertyName)
                : GetPropertyValue<TProp>(obj, propertyName);

        #endregion Methods (GetPropertyValue)

        #region Methods (SetPropertyValue)

        /// <summary>
        /// This dictionary is used to cache the expression used by the SetPropertyValue for better performance.
        /// </summary>
        private static readonly Dictionary<string, object?> SetPropertyValueCache = new();

        /// <summary>
        /// Sets a property value.
        /// This method is ultra optimized and use Expression and cache system for better performance.
        /// </summary>
        /// <param name="obj">The object to set the value of a property.</param>
        /// <param name="propertyName">The name of the property to set a value.</param>
        /// <param name="value">The value to set to the property.</param>
        /// <exception cref="NullReferenceException">If the object is null</exception>
        /// <exception cref="ArgumentException">If the property does not exist on the object.</exception>
        public static void SetPropertyValue<TObj, TValue>(this TObj obj, string propertyName, TValue value)
        {
            // If the object is null then returns a NullReferenceException 
            var typeObj = obj?.GetType();
            if (typeObj == null)
                throw new NullReferenceException();

            var key = $"{typeObj}#{propertyName}";
            var setFunc = (Action<TObj, TValue>?)SetPropertyValueCache.GetValue(key);
            if (setFunc == null)
            {
                var propertyInfo = typeObj.GetProperty(propertyName);
                if (propertyInfo == null)
                    throw new ArgumentException($@"The property {propertyName} has not been found on Type {typeObj.FullName}", nameof(propertyName));

                var delegateType = typeof(Action<,>).MakeGenericType(typeObj, propertyInfo.PropertyType);
                var setMethod = propertyInfo.GetSetMethod() ?? throw new ArgumentException($"The property {propertyName} has not been found on Type {typeObj.FullName}", nameof(propertyName));
                var delegateSet = Delegate.CreateDelegate(delegateType, null, setMethod);
                setFunc = (o, v) => delegateSet.DynamicInvoke(o, v);
                SetPropertyValueCache.Add(key, setFunc);
            }

            setFunc!(obj, value);
        }

        #endregion Methods (SetPropertyValue)

        /// <summary>
        /// Gets the lowest property in inheritance given a property name in case of "new" Property override.
        /// </summary>
        /// <param name="type">The type to search for property.</param>
        /// <param name="name">The property name to search.</param>
        /// <returns>The info of the lowest inheritance property if found, null otherwise.</returns>
        public static PropertyInfo? GetLowestProperty(this Type type, string name)
        {
            Type? current = type;
            while (current != null)
            {
                var property = current.GetProperty(name, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                    return property;
                current = current.BaseType;
            }
            return null;
        }

        #region Methods (General)

        /// <summary>
        /// Wraps the instance in an enumerable containing a single element.
        /// </summary>
        public static IEnumerable<T> AsEnumerableOfOne<T>(this T input) => Enumerable.Repeat(input, 1);

        /// <summary>
        /// Dispose the object if it implements <see cref="IDisposable"/>.
        /// </summary>
        public static void DisposeIfDisposable(this object input)
        {
            if (input is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // ignore if it is already disposed
                }
            }
        }

        /// <summary>
        /// Safely casts the object to the specified type.
        /// </summary>
        public static T? SafeCast<T>(this object input)
        {
            if (input == null)
                return default;
            return input is T variable ? variable : default;
        }

        /// <summary>
        /// Converts the value to a <see cref="System.Guid"/> or returns <see cref="System.Guid.Empty"/>.
        /// </summary>
        public static System.Guid AsGuid(this object value) => value is System.Guid guid ? guid : System.Guid.Empty;

        /// <summary>
        /// Convert the value to an XML safe string representation for the specified type.
        /// </summary>
        public static string ToXmlString(this object value, Type type)
        {
            if (value == null)
                return string.Empty;

            if (type == typeof(string))
                return value.ToString() ?? string.Empty;

            if (type == typeof(bool))
                return XmlConvert.ToString((bool)value);
            if (type == typeof(byte))
                return XmlConvert.ToString((byte)value);
            if (type == typeof(char))
                return XmlConvert.ToString((char)value);
            if (type == typeof(DateTime))
                return XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Unspecified);
            if (type == typeof(decimal))
                return XmlConvert.ToString((decimal)value);
            if (type == typeof(double))
                return XmlConvert.ToString((double)value);
            if (type == typeof(float))
                return XmlConvert.ToString((float)value);
            if (type == typeof(System.Guid))
                return XmlConvert.ToString((System.Guid)value);
            if (type == typeof(int))
                return XmlConvert.ToString((int)value);
            if (type == typeof(long))
                return XmlConvert.ToString((long)value);
            if (type == typeof(sbyte))
                return XmlConvert.ToString((sbyte)value);
            if (type == typeof(short))
                return XmlConvert.ToString((short)value);
            if (type == typeof(TimeSpan))
                return XmlConvert.ToString((TimeSpan)value);
            if (type == typeof(uint))
                return XmlConvert.ToString((uint)value);
            if (type == typeof(ulong))
                return XmlConvert.ToString((ulong)value);
            if (type == typeof(ushort))
                return XmlConvert.ToString((ushort)value);

            throw new NotSupportedException($"Cannot convert type {type.FullName} to a string using XmlConvert");
        }

        /// <summary>
        /// Generic overload of <see cref="ToXmlString(object,Type)"/>.
        /// </summary>
        public static string ToXmlString<T>(this object value) => value.ToXmlString(typeof(T));

        /// <summary>
        /// Convert an object into a dictionary of property names and values.
        /// </summary>
        public static IDictionary<string, TVal> ToDictionary<TVal>(this object o, params string[] ignoreProperties)
        {
            if (o == null)
                return new Dictionary<string, TVal>();

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(o);
            var d = new Dictionary<string, TVal>();
            foreach (PropertyDescriptor prop in props.Cast<PropertyDescriptor>().Where(x => !ignoreProperties.Contains(x.Name)))
            {
                var val = prop.GetValue(o);
                if (val != null)
                {
                    d.Add(prop.Name, (TVal)val);
                }
            }

            return d;
        }

        #endregion Methods (General)
    }
}