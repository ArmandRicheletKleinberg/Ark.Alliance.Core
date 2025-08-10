using Microsoft.EntityFrameworkCore.Migrations;

namespace Ark.Data.EFCore
{
    /// <summary>
    /// This class extends the <see cref="MigrationBuilder"/> class.
    /// </summary>
    public static class MigrationBuilderExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Creates the generic key/value settings table to be used by the SettingsDbServices.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder used to add the Settings table creation.</param>
        /// <param name="tableName">The name of the table to create. Default to "Settings".</param>
        public static void CreateSettingsTable(this MigrationBuilder migrationBuilder, string tableName = "Settings")
        {
            migrationBuilder.CreateTable(tableName, table => new
            {
                Key = table.Column<string>(nullable: false, maxLength: 254),
                Value = table.Column<string>(nullable: false)
            },
                constraints: table =>
                {
                    table.PrimaryKey($"PK_{tableName}", c => c.Key);
                });
        }

        /// <summary>
        /// Drops the generic key/value settings table.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder used to add the Settings table creation.</param>
        /// <param name="tableName">The name of the table to create. Default to "Settings".</param>
        public static void DropSettingsTable(this MigrationBuilder migrationBuilder, string tableName = "Settings")
        {
            migrationBuilder.DropTable(tableName);
        }

        #endregion Methods (Public)

    }
}