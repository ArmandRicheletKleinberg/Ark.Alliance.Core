using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ark.Data.EFCore;
using Binance.Net.Enums;
using Microsoft.EntityFrameworkCore;
using EfIndex = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Represents an open position.
    /// </summary>
    [Table("Position")]
    [EfIndex(nameof(SessionId))]
    public class PositionDbEntity : DbEntity<BinanceDbContext>
    {
        /// <summary>
        /// Primary key of the position.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Identifier of the related session.
        /// </summary>
        [Column("SessionId")]
        public System.Guid SessionId { get; set; }

        /// <summary>
        /// Identifier of the Binance account owner.
        /// </summary>
        [Column("OwnerId", TypeName = "varchar"), Required, MaxLength(128)]
        public string OwnerId { get; set; } = string.Empty;

        /// <summary>
        /// Trading symbol.
        /// </summary>
        [Column("Symbol", TypeName = "varchar"), Required, MaxLength(20)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Position side.
        /// </summary>
        [Column("Side")]
        public PositionSide Side { get; set; }

        /// <summary>
        /// Quantity of the position.
        /// </summary>
        [Column("Quantity", TypeName = "decimal(20,10)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Entry price for the position.
        /// </summary>
        [Column("EntryPrice", TypeName = "decimal(20,10)")]
        public decimal EntryPrice { get; set; }

        /// <summary>
        /// Current mark price.
        /// </summary>
        [Column("MarkPrice", TypeName = "decimal(20,10)")]
        public decimal MarkPrice { get; set; }

        /// <summary>
        /// Unrealized profit or loss.
        /// </summary>
        [Column("UnrealizedPnl", TypeName = "decimal(20,10)")]
        public decimal UnrealizedPnl { get; set; }

        /// <summary>
        /// Leverage used.
        /// </summary>
        [Column("Leverage", TypeName = "decimal(20,10)")]
        public decimal Leverage { get; set; }

        /// <summary>
        /// Timestamp of the data.
        /// </summary>
        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>Owning session.</summary>
        [ForeignKey(nameof(SessionId))]
        public virtual BinanceSessionDbEntity Session { get; set; } = null!;
    }
}
