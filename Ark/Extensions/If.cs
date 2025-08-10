namespace Ark
{
#nullable enable
    /// <summary>
    ///     Extension methods providing fluent <c>If</c>-style helpers.
    ///     These utilities simplify null and boolean checks.
    ///
    ///     <para>Example using the extensions:</para>
    ///     <code language="csharp">
    ///     string? name = GetName();
    ///     name.IfNotNull(n => Console.WriteLine(n));
    ///     bool canRun = true;
    ///     canRun.IfTrue(() => Start());
    ///     </code>
    ///     <para>Equivalent code without the extensions:</para>
    ///     <code language="csharp">
    ///     string? name = GetName();
    ///     if (name != null) Console.WriteLine(name);
    ///     bool canRun = true;
    ///     if (canRun) Start();
    ///     </code>
    ///     <para>Advantages:</para>
    ///     <list type="bullet">
    ///         <item>Reduces repetitive <c>if</c> statements.</item>
    ///         <item>Allows functional chaining of operations.</item>
    ///     </list>
    ///     <para>Disadvantages:</para>
    ///     <list type="bullet">
    ///         <item>May be less explicit for developers unfamiliar with the helpers.</item>
    ///     </list>
    ///     <para>Performance:</para>
    ///     <para>These helpers simply check a condition and invoke a delegate, so the
    ///     complexity is O(1) and equivalent to manual <c>if</c> statements with no
    ///     additional allocations.</para>
    ///     <para>Use them when readability improves through fluent chaining; otherwise a
    ///     regular <c>if</c> statement is perfectly adequate.</para>
    /// </summary>
    public static class IfExtensions
    {
        /// <summary>
        /// Executes <paramref name="action"/> only when <paramref name="item"/> is not <c>null</c>.
        /// </summary>
        /// <typeparam name="TItem">Type of the item.</typeparam>
        /// <param name="item">Object to test for null.</param>
        /// <param name="action">Action invoked when the item is non-null.</param>
        /// <remarks>
        /// Complexity: O(1). Equivalent to an <c>if</c> statement.
        /// Best used for chaining multiple null checks in a fluent style.
        /// </remarks>
        public static void IfNotNull<TItem>(this TItem item, Action<TItem> action)
            where TItem : class
        {
            if (item != null)
            {
                action(item);
            }
        }

        /// <summary>
        /// Executes <paramref name="action"/> when <paramref name="predicate"/> evaluates to <c>true</c>.
        /// </summary>
        /// <param name="predicate">Condition to evaluate.</param>
        /// <param name="action">Action executed if the condition is <c>true</c>.</param>
        /// <remarks>
        /// Complexity: O(1). Offers no performance gain over a direct <c>if</c> statement,
        /// but can improve readability when chaining multiple checks.
        /// </remarks>
        public static void IfTrue(this bool predicate, Action action)
        {
            if (predicate)
            {
                action();
            }
        }

        /// <summary>
        ///     Evaluates <paramref name="action"/> when <paramref name="item"/> is not null and
        ///     returns the resulting value or <paramref name="defaultValue"/> otherwise.
        /// </summary>
        /// <typeparam name="TResult">Type of the returned value.</typeparam>
        /// <typeparam name="TItem">Type of the item.</typeparam>
        /// <param name="item">Object to test for null.</param>
        /// <param name="action">Function applied when the item is non-null.</param>
        /// <param name="defaultValue">Value returned when the item is null.</param>
        /// <returns>Result of <paramref name="action"/> or <paramref name="defaultValue"/>.</returns>
        /// <remarks>
        /// Complexity: O(1). This is syntactic sugar for an <c>if</c> statement returning a value.
        /// </remarks>
        public static TResult? IfNotNull<TResult, TItem>(this TItem? item, Func<TItem, TResult> action, TResult? defaultValue = default)
            where TItem : class
            => item != null ? action(item) : defaultValue;

        /// <summary>
        ///     Returns <paramref name="item"/> if it is not null; otherwise executes
        ///     <paramref name="action"/> to create a fallback value.
        /// </summary>
        /// <typeparam name="TItem">Type of the value.</typeparam>
        /// <param name="item">Value that may be null.</param>
        /// <param name="action">Factory used when <paramref name="item"/> is null.</param>
        /// <returns>Either the original value or the result of <paramref name="action"/>.</returns>
        /// <remarks>
        /// Complexity: O(1). This helper simply checks for null before invoking the factory.
        /// </remarks>
        public static TItem IfNull<TItem>(this TItem? item, Func<TItem, TItem> action)
            where TItem : class
            => item ?? action(item!);
    }
}