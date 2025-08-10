using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ark.Data.EFCore;
using Microsoft.EntityFrameworkCore;
using EfIndex = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Represents a trade history entry.
    /// </summary>
    [Table("Trade")]
    [EfIndex(nameof(SessionId))]
    public class TradeDbEntity : DbEntity<BinanceDbContext>
    {
        /// <summary>
        /// Unique trade identifier.
        /// </summary>
        [Key]
        [Column("Id")]
        public long Id { get; set; }

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
        /// Symbol traded.
        /// </summary>
        [Column("Symbol", TypeName = "varchar"), Required, MaxLength(20)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Direction of the trade.
        /// </summary>
        [Column("Side", TypeName = "varchar"), Required, MaxLength(10)]
        public string Side { get; set; } = string.Empty;

        /// <summary>
        /// Executed quantity.
        /// </summary>
        [Column("Quantity", TypeName = "decimal(20,10)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Executed price.
        /// </summary>
        [Column("Price", TypeName = "decimal(20,10)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Commission paid.
        /// </summary>
        [Column("Fee", TypeName = "decimal(20,10)")]
        public decimal Fee { get; set; }

        /// <summary>
        /// Profit or loss realized.
        /// </summary>
        [Column("RealizedPnl", TypeName = "decimal(20,10)")]
        public decimal RealizedPnl { get; set; }

        /// <summary>
        /// Leverage used for the trade.
        /// </summary>
        [Column("Leverage", TypeName = "decimal(20,10)")]
        public decimal Leverage { get; set; }

        /// <summary>
        /// Indicates how the trade was closed.
        /// </summary>
        [Column("CloseType", TypeName = "varchar"), MaxLength(20)]
        public string CloseType { get; set; } = string.Empty;

        /// <summary>
        /// Trade timestamp.
        /// </summary>
        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Status returned by Binance.
        /// </summary>
        [Column("Status", TypeName = "varchar"), MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        /// <summary>Session associated with this trade.</summary>
        [ForeignKey(nameof(SessionId))]
        public virtual BinanceSessionDbEntity Session { get; set; } = null!;
    }
}
