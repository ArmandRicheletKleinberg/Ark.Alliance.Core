using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ark.App.Secrets.Stores;
namespace Ark.App.Secrets.Typed
{
    /// <summary>
    /// Builds typed secret objects from a declarative JSON schema.
    /// </summary>
    public sealed class TypedSecretsFactory
    {
        #region Fields

        private readonly ISecretStore _store;
        private readonly TypedSecretsSchema _schema;

        #endregion

        #region Ctors

        public TypedSecretsFactory(ISecretStore store, TypedSecretsSchema schema)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a typed object instance described by <paramref name="typeName"/> and <paramref name="environment"/>.
        /// </summary>
        /// <typeparam name="T">Target CLR type.</typeparam>
        public async Task<Result<T?>> CreateAsync<T>(string typeName, string environment, CancellationToken ct = default)
            where T : class
        {
            if (!_schema.Types.TryGetValue(typeName, out var def))
                return Result<T?>.Failure.WithReason($"Unknown typed-secret type '{typeName}'.");

            var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var field in def.Fields)
            {
                var key = field.ResolveKey(environment);
                var value = (await _store.GetSecretAsync(key, ct).ConfigureAwait(false)).Data;
                if (field.Required && string.IsNullOrEmpty(value))
                    return Result<T?>.Failure.WithReason($"Missing required secret '{key}'.");
                dict[field.Property] = value;
            }

            // Instantiate T via parameterless ctor then set properties by name
            var obj = Activator.CreateInstance(typeof(T)) as T;
            if (obj is null) return Result<T?>.Failure.WithReason($"Type '{typeof(T).FullName}' requires a parameterless constructor.");
            foreach (var kv in dict)
            {
                var prop = typeof(T).GetProperty(kv.Key);
                if (prop is null) continue;
                if (prop.PropertyType == typeof(string))
                    prop.SetValue(obj, kv.Value);
            }
            return new Result<T?>(obj);
        }

        #endregion
    }

    /// <summary>Schema root.</summary>
    public sealed class TypedSecretsSchema
    {
        public Dictionary<string, TypedTypeDefinition> Types { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public static TypedSecretsSchema FromJson(string json)
            => JsonSerializer.Deserialize<TypedSecretsSchema>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }

    /// <summary>Typed type definition.</summary>
    public sealed class TypedTypeDefinition
    {
        public List<TypedField> Fields { get; set; } = new();
    }

    /// <summary>One field/property mapping.</summary>
    public sealed class TypedField
    {
        public string Property { get; set; } = string.Empty;
        public string KeyTemplate { get; set; } = string.Empty;
        public bool Required { get; set; } = true;

        public string ResolveKey(string environment)
            => KeyTemplate.Replace("{env}", environment ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}
