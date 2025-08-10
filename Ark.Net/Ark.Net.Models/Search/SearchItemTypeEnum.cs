using Ark;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Ark.Net.Models
{
    /// <summary>
    /// Represents a search item type loaded from a JSON data set.
    /// </summary>
    public class SearchItemTypeEnum
    {
        private class ItemData
        {
            public string Code { get; set; }
            public string Label { get; set; }
        }

        private static readonly object LockObj = new();
        private static Func<string> _jsonLoader = LoadFromEmbeddedResource;
        private static Dictionary<string, SearchItemTypeEnum> _items;

        ///// <summary>
        ///// Configures the enumeration to load the dataset from a database table.
        ///// </summary>
        ///// <typeparam name="TContext">The database context type.</typeparam>
        ///// <param name="contextFactory">Factory used to create the context.</param>
        ///// <param name="tableName">The table containing the JSON dataset.</param>
        ///// <param name="fieldName">The column containing the JSON dataset.</param>
        //public static void ConfigureFromDatabase<TContext>(Func<TContext> contextFactory, string tableName, string fieldName)
        //    where TContext : DbContext
        //{
        //    _jsonLoader = () =>
        //    {
        //        using var db = contextFactory();
        //        using var connection = db.Database.GetDbConnection();
        //        connection.Open();
        //        using var command = connection.CreateCommand();
        //        command.CommandText = $"SELECT {fieldName} FROM {tableName}";
        //        var result = command.ExecuteScalar();
        //        return result?.ToString() ?? "[]";
        //    };
        //    _items = null;
        //}

        /// <summary>
        /// Configures the enumeration to load the dataset from a local JSON file.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        public static void ConfigureFromFile(string filePath)
        {
            _jsonLoader = () => File.ReadAllText(filePath);
            _items = null;
        }

        private static string LoadFromEmbeddedResource()
        {
            var assembly = typeof(SearchItemTypeEnum).Assembly;
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith("SearchItemTypes.json"));
            using var stream = assembly.GetManifestResourceStream(resourceName ?? string.Empty);
            using var reader = new StreamReader(stream ?? throw new FileNotFoundException(resourceName));
            return reader.ReadToEnd();
        }

        private static void EnsureLoaded()
        {
            if (_items != null)
                return;

            lock (LockObj)
            {
                if (_items != null)
                    return;

                var json = _jsonLoader();
                var data = json.GetArrayFromJsonArray<ItemData>();
                _items = data.ToDictionary(
                    d => d.Code,
                    d => new SearchItemTypeEnum
                    {
                        Code = d.Code,
                        GetLabelFunc = () => d.Label.GetFromMultiLanguageJsonText(CultureInfo.CurrentCulture.Name ?? "en") ?? d.Code
                    });
            }
        }

        /// <summary>
        /// Gets all available search item types.
        /// </summary>
        public static IEnumerable<SearchItemTypeEnum> All
        {
            get
            {
                EnsureLoaded();
                return _items.Values;
            }
        }

        /// <summary>
        /// Gets a search item type by its code.
        /// </summary>
        /// <param name="code">The code to search.</param>
        /// <returns>The enumeration item or <c>null</c>.</returns>
        public static SearchItemTypeEnum FromCode(string code)
        {
            EnsureLoaded();
            _items.TryGetValue(code, out var item);
            return item;
        }

        /// <summary>
        /// The code identifying the search item type.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// The function used to get the label for the current culture.
        /// </summary>
        public Func<string> GetLabelFunc { get; private set; }
    }
}
