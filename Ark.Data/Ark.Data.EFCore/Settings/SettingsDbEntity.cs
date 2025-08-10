using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Represents a key/value pair persisted in the settings table.
    /// + Simplifies configuration storage across services.
    /// - Values are stored as plain text without type enforcement.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/modeling/entity-types"/>.
    /// </summary>
    [Table("Settings")]
    public class SettingsDbEntity<TContext> : DbEntity<TContext>
        where TContext : DbContextEx, new()
    {
        #region Properties (Columns)

        /// <summary>
        /// Key used to find the stored value.
        /// + Must be unique per setting.
        /// - Case sensitivity depends on database collation.
        /// </summary>
        [Key]
        [Required]
        [MaxLength(254)]
        public string Key { get; set; }

        /// <summary>
        /// Stored value as text.
        /// + Allows flexible configuration formats.
        /// - Caller must handle conversion and validation.
        /// </summary>
        public string Value { get; set; }

        #endregion Properties (Columns)
    }
}