using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Ark.App.Secrets.Stores; 
namespace Ark.App.Secrets.Typed
{
    /// <summary>
    /// Dependency injection helpers for the <see cref="TypedSecretsFactory"/>.
    /// + Loads declarative schemas from embedded resources.
    /// - Requires an <see cref="ISecretStore"/> to resolve actual values.
    /// </summary>
    public static class TypedSecretsRegistrationExtensions
    {
        #region Methods
        /// <summary>
        /// Adds <see cref="TypedSecretsFactory"/> loaded from the embedded 'typed-secrets.json' schema.
        /// + Allows services to request strongly typed secrets.
        /// - Throws if the embedded resource is missing.
        /// </summary>
        /// <param name="services">Service collection to modify.</param>
        /// <param name="resourceName">Resource name containing the schema JSON.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddTypedSecretsFactory(this IServiceCollection services, string resourceName = "Ark.App.Secrets.Resources.typed-secrets.json")
        {
            services.AddSingleton(sp =>
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream!);
                var json = reader.ReadToEnd();
                var schema = TypedSecretsSchema.FromJson(json);
                var store = sp.GetRequiredService<ISecretStore>();
                return new TypedSecretsFactory(store, schema);
            });
            return services;
        }
        #endregion Methods
    }
}
