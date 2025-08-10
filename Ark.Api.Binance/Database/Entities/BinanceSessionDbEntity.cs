using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ark.Data.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Database entity storing Binance session information.
    /// </summary>
    [Table("BinanceSession")]
    public class BinanceSessionDbEntity : DbEntity<BinanceDbContext>
    {
        /// <summary>
        /// Identifier of the session.
        /// </summary>
        [Key, Column("Id")]
        public System.Guid Id { get; set; }

        /// <summary>
        /// UTC creation time of the session.
        /// </summary>
        [Column("Created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// JSON serialized options used to create the session.
        /// </summary>
        [Column("OptionsJson"), Required]
        public string OptionsJson { get; set; } = string.Empty;

        /// <summary>
        /// Identifier of the Binance account owner.
        /// </summary>
        [Column("OwnerId", TypeName = "varchar"), Required, MaxLength(128)]
        public string OwnerId { get; set; } = string.Empty;

        #region Navigation Properties
        /// <summary>Orders associated with this session.</summary>
        public virtual ICollection<OrderDbEntity> Orders { get; set; } = new HashSet<OrderDbEntity>();

        /// <summary>Open positions tracked for this session.</summary>
        public virtual ICollection<PositionDbEntity> Positions { get; set; } = new HashSet<PositionDbEntity>();

        /// <summary>Ticker snapshots recorded during the session.</summary>
        public virtual ICollection<TickerDbEntity> Tickers { get; set; } = new HashSet<TickerDbEntity>();

        /// <summary>Trade history captured for the session.</summary>
        public virtual ICollection<TradeDbEntity> Trades { get; set; } = new HashSet<TradeDbEntity>();

        /// <summary>Income records linked to the session.</summary>
        public virtual ICollection<IncomeDbEntity> Incomes { get; set; } = new HashSet<IncomeDbEntity>();

        #endregion Navigation Properties
    }
}
