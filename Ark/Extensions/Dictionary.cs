namespace Ark
{
    /// <summary>
    /// This helper class extends the Dictionary object.
    /// </summary>
    public static class DictionaryExtensibility
    {
        /// <summary>
        /// Tries to find the key in the dictionary or returns default if not found.
        /// </summary>
        /// <typeparam name="TK">The type of the key.</typeparam>
        /// <typeparam name="TV">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary into which search the key.</param>
        /// <param name="key">The key to search for in the dictionary.</param>
        /// <param name="defaultValue">The default value if not found.</param>
        /// <returns>The value of the key if exists, default otherwise.</returns>
        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV defaultValue = default)
        {
            if (dictionary == null)
                return defaultValue;

            return !dictionary.TryGetValue(key, out var value) ? defaultValue : value;
        }

        /// <summary>
        /// Tries to find the key in the dictionary and sets the value if exists.
        /// </summary>
        /// <typeparam name="TK">The type of the key.</typeparam>
        /// <typeparam name="TV">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary into which search the key.</param>
        /// <param name="key">The key to search for in the dictionary.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True if exists and set, false otherwise.</returns>
        public static bool SetValue<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV value)
        {
            if (!dictionary.ContainsKey(key)) return false;

            dictionary[key] = value;
            return true;
        }

        /// <summary>
        /// Adds a new item in the dictionary or update it if already exist.
        /// </summary>
        /// <typeparam name="TK">The type of the dictionary key.</typeparam>
        /// <typeparam name="TV">The type of the dictionary value.</typeparam>
        /// <param name="dictionary">The dictionary into which add or update the item.</param>
        /// <param name="key">The key to add/update in the dictionary.</param>
        /// <param name="value">The value to add/update in the dictionary.</param>
        public static void AddOrUpdate<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV value)
        {
            // the indexer handles both add and update for IDictionary and avoids race
            // conditions with concurrent dictionaries where checking ContainsKey then
            // adding can throw when another thread inserts the same key.
            dictionary[key] = value;
        }

        /// <summary>
        /// Compares a dictionary to another dictionary and returns whether the dictionary are equal or not.
        /// </summary>
        /// <typeparam name="TK">The type of the dictionary key.</typeparam>
        /// <typeparam name="TV">The type of the dictionary value.</typeparam>
        /// <param name="first">The first dictionary to compare.</param>
        /// <param name="second">The second dictionary to compare.</param>
        /// <returns>True if equals, false otherwise.</returns>
        public static bool CompareTo<TK, TV>(this IDictionary<TK, TV> first, IDictionary<TK, TV> second)
        {
            if (second == null) return false;
            return (first.Count == second.Count) && !first.Except(second).Any();
        }
    }
}