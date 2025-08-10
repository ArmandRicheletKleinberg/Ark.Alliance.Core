using System.Collections;
using System.Collections.ObjectModel;

namespace Ark
{
    /// <summary>
    /// This helper class extends the IEnumerable object.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtensibility
    {
        /// <summary>
        /// Keeps only the not null values.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Where(i => i != null);
        }

        /// <summary>
        /// Executes an action for each item in the enumerable.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to execute.</param>
        public static void ForEach(this IEnumerable enumerable, Action<object> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        /// <summary>
        /// Executes an action for each item in the enumerable asynchronously.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The asynchronous action to execute.</param>
        public static async Task ForEachAsync(this IEnumerable enumerable, Func<object, Task> action)
        {
            foreach (var item in enumerable)
                await action(item);
        }

        /// <summary>
        /// Executes an action for each item in the enumerable.
        /// This differs from for each because that the index is passed in parameter.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to execute.</param>
        public static void Each(this IEnumerable enumerable, Action<object, int> action)
        {
            var i = 0;
            foreach (var item in enumerable)
                action(item, i++);
        }

        /// <summary>
        /// Executes an action for each item in the enumerable asynchronously.
        /// This differs from for each because that the index is passed in parameter.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The asynchronous action to execute.</param>
        public static async Task EachAsync(this IEnumerable enumerable, Func<object, int, Task> action)
        {
            var i = 0;
            foreach (var item in enumerable)
                await action(item, i++);
        }

        /// <summary>
        /// Executes an action for each item in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to execute.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        /// <summary>
        /// Executes an action for each item in the enumerable asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The asynchronous action to execute.</param>
        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            foreach (var item in enumerable)
                await action(item);
        }

        /// <summary>
        /// Executes an action for each item in the enumerable.
        /// This differs from for each because that the index is passed in parameter.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to execute.</param>
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in enumerable)
                action(item, i++);
        }

        /// <summary>
        /// Executes an action for each item in the enumerable asynchronously.
        /// This differs from for each because that the index is passed in parameter.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The asynchronous action to execute.</param>
        public static async Task EachAsync<T>(this IEnumerable<T> enumerable, Func<T, int, Task> action)
        {
            var i = 0;
            foreach (var item in enumerable)
                await action(item, i++);
        }

        /// <summary>
        /// Gets the minimum value of a sequence or default value if no items.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="sequence">The sequence to return the minimum.</param>
        /// <param name="defaultValue">The default value to return if no element in sequence.</param>
        /// <returns>The minimum value if at least one item, default otherwise.</returns>
        public static T MinOrDefault<T>(this IEnumerable<T> sequence, T defaultValue = default)
        {
            var enumerable = sequence as T[] ?? sequence.ToArray();
            return enumerable.Any() ? enumerable.Min() : defaultValue;
        }

        /// <summary>
        /// Gets the maximum value of a sequence or default value if no items.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="sequence">The sequence to return the maximum.</param>
        /// <returns>The maximum value if at least one item, default otherwise.</returns>
        public static T MaxOrDefault<T>(this IEnumerable<T> sequence)
        {
            var enumerable = sequence as T[] ?? sequence.ToArray();
            return enumerable.Any() ? enumerable.Max() : default;
        }

        /// <summary>
        /// Converts the enumeration to an array with a fixed sized array.
        /// It crops or adds some default elements.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to convert to array.</param>
        /// <param name="arrayLength">The length of the array.</param>
        /// <param name="defaultValue">The default value to fill the missing items if any.</param>
        /// <returns>The maximum value if at least one item, default otherwise.</returns>
        public static T[] ToFixedSizeArray<T>(this IEnumerable<T> enumerable, int arrayLength, T defaultValue = default)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();

            var fixedSizeArray = Enumerable.Repeat(defaultValue, arrayLength).ToArray();
            Array.Copy(array, 0, fixedSizeArray, 0, Math.Min(array.Length, arrayLength));

            return fixedSizeArray;
        }

        /// <summary>
        /// Flattens an hierarchy enumerable who owns children of the same type.
        /// </summary>
        /// <typeparam name="T">The type of the elements and children.</typeparam>
        /// <param name="enumerable"></param>
        /// <param name="getChildrenFunc">The function to get the element children.</param>
        /// <returns>The array of elements/children flattened.</returns>
        public static T[] Flatten<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();
            return array.SelectMany(e => getChildrenFunc(e)?.Flatten(getChildrenFunc) ?? new T[0]).Concat(array).ToArray();
        }

        /// <summary>
        /// Whether this enumerable has no elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable to check for elements.</param>
        /// <returns>true if no elements, false otherwise.</returns>
        public static bool None<T>(this IEnumerable<T> enumerable)
            => !enumerable.Any();

        /// <summary>
        /// Whether this enumerable does not exist or has no elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable to check for elements.</param>
        /// <returns>true if null or no elements, false otherwise.</returns>
        public static bool HasNoElements<T>(this IEnumerable<T> enumerable)
            => !(enumerable?.Any() ?? false);

        /// <summary>
        /// Whether this enumerable exists (not null) and has at least one element.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable to check for elements.</param>
        /// <returns>true if not null and elements number greater than 0, false otherwise.</returns>
        public static bool HasAnElement<T>(this IEnumerable<T> enumerable)
            => enumerable?.Any() ?? false;

        /// <summary>
        /// Whether this enumerable exists (not null) and has a single element.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable to check for elements.</param>
        /// <returns>true if not null and elements number is 1, false otherwise.</returns>
        public static bool HasSingleElement<T>(this IEnumerable<T> enumerable)
            => enumerable?.Count() == 1;

        /// <summary>
        /// Returns the first occurence or a specified value if not found.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable element.</typeparam>
        /// <param name="enumerable">The enumerable to check for a first element.</param>
        /// <param name="value">The value to return if not exists.</param>
        /// <returns>The first element if found or specified value if not.</returns>
        public static T FirstOrValue<T>(this IEnumerable<T> enumerable, T value)
        {
            var firstOrDefaultValue = enumerable.FirstOrDefault();
            return Equals(firstOrDefaultValue, default(T)) ? value : firstOrDefaultValue;
        }

        /// <summary>
        /// Distinct element by a property
        /// </summary>
        /// <typeparam name="T">The type of the elements</typeparam>
        /// <typeparam name="TKey">The key of the property.</typeparam>
        /// <param name="enumerable">The enumerable></param>
        /// <param name="keySelector"></param>
        /// <returns>Return list with distinct property</returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in enumerable)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Converts an enumeration to an observable collection.
        /// If the enumeration is null then the returned collection will be null as well.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration items.</typeparam>
        /// <param name="enumerable">The source enumeration.</param>
        /// <returns>The observable collection with the wrapped enumeration opr null if enumeration is null.</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
            => enumerable != null
                ? new ObservableCollection<T>(enumerable)
                : null;

        /// <summary>
        /// Orders ascending an enumerable dynamically given a property name.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to order</param>
        /// <param name="propertyName">The name of the property to order.</param>
        /// <returns>The ascending ordered enumerable.</returns>
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string propertyName)
            => enumerable.OrderBy(i => i.GetPropertyValue<object>(propertyName));

        /// <summary>
        /// Orders descending an enumerable dynamically given a property name.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to order</param>
        /// <param name="propertyName">The name of the property to order.</param>
        /// <returns>The descending ordered enumerable.</returns>
        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> enumerable, string propertyName)
            => enumerable.OrderByDescending(i => i.GetPropertyValue<object>(propertyName));

        /// <summary>
        /// Orders ascending or descending an enumerable dynamically given a property name.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to order</param>
        /// <param name="propertyName">The name of the property to order.</param>
        /// <param name="descendingOrder">Whether to sort descending or ascending otherwise.</param>
        /// <returns>The ascending or descending ordered enumerable.</returns>
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string propertyName, bool descendingOrder)
            => descendingOrder
                ? enumerable.OrderByDescending(i => i.GetPropertyValue<object>(propertyName))
                : enumerable.OrderBy(i => i.GetPropertyValue<object>(propertyName));
    }
}