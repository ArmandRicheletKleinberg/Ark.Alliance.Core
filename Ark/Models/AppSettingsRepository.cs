using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace Ark

{
    /// <summary>
    /// Provides access to application settings via <see cref="IConfiguration"/>.
    /// + Caches section instances to reduce repeated deserialization.
    /// - Requires <see cref="Configuration"/> to be configured before first use.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/configuration"/>
    /// </summary>
    public class AppSettingsRepository
    {
        #region Static

        /// <summary>
        /// Cache of previously retrieved sections for performance.
        /// </summary>
        private static readonly Dictionary<Type, AppSettingsSectionBase> Sections = new();

        /// <summary>
        /// Application configuration root.
        /// + Must be assigned during application startup.
        /// - Not thread-safe to modify after initialization.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/configuration"/>
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        #endregion Static

        #region Methods (Public)

        /// <summary>
        /// Retrieves an application settings section of type <typeparamref name="TSection"/>.
        /// + Returns a cached instance after the first call to avoid repeated parsing.
        /// - Throws <see cref="Exception"/> if the configuration or section path is missing.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/configuration"/>
        /// </summary>
        /// <typeparam name="TSection">Type deriving from <see cref="AppSettingsSectionBase"/> representing a configuration section.</typeparam>
        /// <returns>
        /// A populated <typeparamref name="TSection"/> instance from the JSON configuration.
        /// Example:
        /// <code language="json">
        /// {
        ///   "MySection": {
        ///     "SomeProperty": "value"
        ///   }
        /// }
        /// </code>
        /// </returns>
        public virtual TSection GetSection<TSection>()
            where TSection : AppSettingsSectionBase
        {
            // First search in the section cache for performance purpose
            var sectionType = typeof(TSection);
            if (Sections.GetValue(sectionType) is TSection section)
                return section;

            if (Configuration == null)
                throw new Exception("This repository needs to be initialized using Startup.Services.AddAppSettings(IHostingEnvironment hostingEnvironment)");

            section = typeof(TSection).New<TSection>();
            if (section.SectionPath.IsNullOrEmpty())
                throw new Exception($"The section path must be defined for all sections. {sectionType}");
            section = section.Deserialize(Configuration) as TSection;
            if (section == null)
                throw new Exception($"The section could not be deserialized correctly, check your deserialization code. {sectionType}");

            Sections.Add(sectionType, section);
            return section;
        }

        /// <summary>
        /// Builds an <see cref="ExpandoObject"/> representing the entire configuration hierarchy.
        /// + Combines configuration sources and environment variables into a single object.
        /// - May allocate significant memory for large configurations.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/configuration"/>
        /// </summary>
        /// <returns>
        /// An <see cref="ExpandoObject"/> whose JSON representation mirrors the application configuration.
        /// Example:
        /// <code language="json">
        /// {
        ///   "Logging": { "LogLevel": { "Default": "Information" } }
        /// }
        /// </code>
        /// </returns>
        public virtual ExpandoObject GetFullConfigurationJsonObject()
        {
            var obj = new ExpandoObject();
            AddConfigurationSection(Configuration, obj);
            return obj;
        }

        #endregion Methods (Public)

        #region Methods (Helpers)

        /// <summary>
        /// Recursively copies <see cref="IConfiguration"/> data into a dynamic object.
        /// + Preserves nested structure by creating child <see cref="ExpandoObject"/> instances.
        /// - Recursive traversal can impact performance on deep hierarchies.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/configuration"/>
        /// </summary>
        /// <param name="configuration">The configuration section or root used to build the JSON object.</param>
        /// <param name="parent">The dynamic parent to add property or object children to.</param>
        private static void AddConfigurationSection(IConfiguration configuration, ExpandoObject parent)
        {
            configuration?.GetChildren().ForEach(section =>
            {
                if (section.Value != null)
                {
                    parent.AddOrUpdate(section.Key, section.Value);
                    return;
                }

                var child = new ExpandoObject();
                parent.AddOrUpdate(section.Key, child);
                AddConfigurationSection(section, child);
            });
        }

        #endregion Methods (Helpers)
    }
}