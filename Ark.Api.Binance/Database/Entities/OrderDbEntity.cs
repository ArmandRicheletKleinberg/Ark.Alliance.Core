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
    /// Represents an order recorded from Binance.
    /// </summary>
    [Table("Order")]
    [EfIndex(nameof(SessionId))]
    public class OrderDbEntity : DbEntity<BinanceDbContext>
    {
        /// <summary>
        /// Identifier assigned by Binance.
        /// </summary>
        [Key]
        [Column("OrderId")]
        public long OrderId { get; set; }

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
        /// Order side (Buy/Sell).
        /// </summary>
        [Column("Side")]
        public OrderSide Side { get; set; }

        /// <summary>
        /// Order type (Market, Limit...).
        /// </summary>
        [Column("Type")]
        public FuturesOrderType Type { get; set; }

        /// <summary>
        /// Quantity traded.
        /// </summary>
        [Column("Quantity", TypeName = "decimal(20,10)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Price when applicable.
        /// </summary>
        [Column("Price", TypeName = "decimal(20,10)")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Stop price when applicable.
        /// </summary>
        [Column("StopPrice", TypeName = "decimal(20,10)")]
        public decimal? StopPrice { get; set; }

        /// <summary>
        /// Time in force policy.
        /// </summary>
        [Column("TimeInForce")]
        public TimeInForce TimeInForce { get; set; }

        /// <summary>
        /// Indicates if the order reduces a position.
        /// </summary>
        [Column("ReduceOnly")]
        public bool ReduceOnly { get; set; }

        /// <summary>
        /// Position side of the order.
        /// </summary>
        [Column("PositionSide")]
        public PositionSide PositionSide { get; set; }

        /// <summary>
        /// Optional client order identifier.
        /// </summary>
        [Column("ClientOrderId", TypeName = "varchar"), MaxLength(64)]
        public string ClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// Order status returned by Binance.
        /// </summary>
        [Column("Status")]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Timestamp provided by Binance.
        /// </summary>
        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>Associated session.</summary>
        [ForeignKey(nameof(SessionId))]
        public virtual BinanceSessionDbEntity Session { get; set; } = null!;
    }
}
