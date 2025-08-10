// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System;

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Configuration settings controlling database connections and migrations.
    /// + Centralizes Entity Framework Core options for Ark services.
    /// - Misconfiguration can prevent context initialization.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/dbcontext-configuration"/>
    /// </summary>
    public class DatabaseOptions
    {
        #region Properties (Public)

        /// <summary>
        /// Gets or sets the provider-specific connection string.
        /// + Supports standard ADO.NET syntax for cross-provider compatibility.
        /// - Embedding secrets in configuration may expose credentials.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/miscellaneous/connection-strings"/>
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Maximum command execution time in seconds.
        /// + Prevents long-running queries from monopolizing resources.
        /// - Values that are too low may cancel legitimate workloads.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/miscellaneous/configuring-dbcontext#command-timeout"/>
        /// </summary>
        public int? CommandTimeout { get; set; }

        /// <summary>
        /// Indicates whether pending migrations should be applied on startup.
        /// + Ensures schema compatibility for new deployments.
        /// - Automatic migration may disrupt running systems if changes are destructive.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/managing-schemas/migrations/applying"/>
        /// </summary>
        public bool Migrate { get; set; }

        /// <summary>
        /// Assembly name that contains migration classes.
        /// + Allows reusing migrations across multiple services.
        /// - Mistyped names lead to runtime resolution errors.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/managing-schemas/migrations/projects"/>
        /// </summary>
        public string MigrationsAssembly { get; set; }

        /// <summary>
        /// Table name used to record applied migrations.
        /// + Custom names help isolate history per context.
        /// - Renaming requires manual migration of existing history.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/managing-schemas/migrations/history-table"/>
        /// </summary>
        public string MigrationsHistoryTable { get; set; } = "__EFMigrationsHistory";

        /// <summary>
        /// Whether to include the built-in key/value Settings table.
        /// + Provides lightweight configuration storage.
        /// - Unused tables increase schema size.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/modeling/entity-types"/>
        /// </summary>
        public bool UseSettingsTable { get; set; }

        /// <summary>
        /// Name of the Settings table when <see cref="UseSettingsTable"/> is enabled.
        /// + Allows multi-tenancy by customizing configuration table names.
        /// - Changing requires migration scripts to preserve data.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/managing-schemas/migrations/"/>
        /// </summary>
        public string SettingsTableName { get; set; } = "Settings";

        /// <summary>
        /// Specifies how <see cref="DateTime"/> values are stored.
        /// + Using <see cref="DateTimeKind.Utc"/> avoids time zone ambiguity.
        /// - Local time storage can cause daylight saving issues.
        /// Ref: <see href="https://learn.microsoft.com/ef/core/modeling/value-conversions#datetimes-and-timezones"/>
        /// </summary>
        public DateTimeKind GlobalDateTimeGlobalKind { get; set; } = DateTimeKind.Utc;

        #endregion Properties (Public)
    }
}