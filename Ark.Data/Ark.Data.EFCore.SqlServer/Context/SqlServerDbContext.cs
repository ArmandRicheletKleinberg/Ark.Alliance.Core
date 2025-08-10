using Microsoft.EntityFrameworkCore;

namespace Ark.Data.EFCore.SqlServer
{
    /// <summary>
    /// The SQLite database context is used to connect a SQLite database.
    /// </summary>
    public abstract class SqlServerDbContext : DbContextEx
    {
        #region Methods (Override)

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (optionsBuilder.IsConfigured)
                return;

            optionsBuilder.UseSqlServer(Options?.ConnectionString ?? "NoConnectionStringDefined", options =>
            {
                options.MigrationsAssembly(Options?.MigrationsAssembly ?? GetType().Assembly.GetName().Name);
                options.MigrationsHistoryTable(Options?.MigrationsHistoryTable ?? "__EFMigrationsHistory");
                options.CommandTimeout(Options?.CommandTimeout ?? 30);
            });
        }

        #endregion Methods (Override)
    }
}