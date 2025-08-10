using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ark;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Provides access to application settings stored in the database.
    /// + Enables centralized configuration management.
    /// - Requires database availability for reads and writes.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/querying/"/>.
    /// </summary>
    public class SettingsDbServices<TContext, TEnum> : DbServices<TContext, SettingsDbEntity<TContext>>
        where TContext : DbContextEx, new()
        where TEnum : struct, IComparable
    {
        #region Methods (Get)

        /// <summary>
        /// Finds a setting value given its key.
        /// + Returns <see cref="ResultStatus.NotFound"/> when the key is missing.
        /// - Database failures propagate as <see cref="ResultStatus.Unexpected"/>.
        /// </summary>
        /// <param name="settingKey">The setting key to find the stored value.</param>
        /// <returns>
        /// Success : The found value.
        /// NotFound : No setting has been found for this key.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public async Task<Result<string>> GetValue(TEnum settingKey)
        {
            var result = await FindByPrimaryKey(settingKey.ToString());
            return new Result<string>(result).WithData(result.Data.Value);
        }

        /// <summary>
        /// Finds a setting value given its key and returns the default value if the key is not found or the database is not reachable.
        /// It also converts the string found to the type of data to return.
        /// + Avoids exceptions when the key is absent.
        /// - Conversion errors silently return the default value.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/csharp/programming-guide/types/casting-and-type-conversions"/>.
        /// </summary>
        /// <typeparam name="TData">The type of data to return.</typeparam>
        /// <param name="settingKey">The setting key to find the stored value.</param>
        /// <param name="defaultValue">The default value to return if not found or if something has gone wrong.</param>
        /// <returns>The converted data found or default value if not found or if something has gone wrong.</returns>
        public async Task<TData> GetValueOrDefault<TData>(TEnum settingKey, TData defaultValue = default)
        {
            var result = await FindByPrimaryKey(settingKey.ToString());
            if (result.IsNotSuccess)
                return defaultValue;

            var value = result.Data.Value.ToObject(defaultValue);
            return value;
        }

        /// <summary>
        /// Gets many settings values given some keys.
        /// + Bundles multiple lookups into a single query.
        /// - Large key sets may impact performance.
        /// </summary>
        /// <param name="settingKeys">The keys of the setting to get.</param>
        /// <returns>
        /// Success : The dictionary with the setting key/values found.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public async Task<Result<Dictionary<TEnum, string>>> GetRange(params TEnum[] settingKeys)
        {
            var result = await GetWhere(setting => settingKeys.Distinct().Select(s => s.ToString()).Contains(setting.Key));
            if (result.IsNotSuccess)
                return new Result<Dictionary<TEnum, string>>(result);

            var dictionary = result.Data.ToDictionary(kvp => kvp.Key.ToEnum(default(TEnum)), kvp => kvp.Value);
            return new Result<Dictionary<TEnum, string>>(result).WithData(dictionary);
        }

        #endregion Methods (Get)

        #region Methods (Set)

        /// <summary>
        /// Sets the value of a setting given its key.
        /// It upserts the value in database.
        /// + Inserts or updates automatically using Upsert.
        /// - Values are stored as strings without validation.
        /// Ref: <see href="https://github.com/FlexLabs/EFCore.Upsert"/>.
        /// </summary>
        /// <param name="settingKey">The keys of the setting to set its value.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>
        /// Success : The dictionary with the setting key/values found.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public async Task<Result> SetValue(TEnum settingKey, object value)
            => await base.CreateOrUpdate(new SettingsDbEntity<TContext> { Key = settingKey.ToString(), Value = value?.ToString() });

        /// <summary>
        /// Sets a bunch of setting values in one upsert operation.
        /// + Efficient for batch updates.
        /// - Lacks transactional boundaries across different tables.
        /// </summary>
        /// <param name="settings">The settings to upsert in database.</param>
        /// <returns>
        /// Success : The settings have been upserted successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public async Task<Result> SetRange(Dictionary<TEnum, string> settings)
            => await base.CreateOrUpdateRange(settings.Select(kvp => new SettingsDbEntity<TContext>
            {
                Key = kvp.Key.ToString(),
                Value = kvp.Value
            }).ToArray());

        #endregion Methods (Set)
    }
}