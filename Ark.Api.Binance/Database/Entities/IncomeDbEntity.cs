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
    /// Records income history from Binance.
    /// </summary>
    [Table("Income")]
    [EfIndex(nameof(SessionId))]
    public class IncomeDbEntity : DbEntity<BinanceDbContext>
    {
        /// <summary>
        /// Primary key of the income entry.
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
        /// Symbol this income relates to.
        /// </summary>
        [Column("Symbol", TypeName = "varchar"), Required, MaxLength(20)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Time of the income event.
        /// </summary>
        [Column("Time")]
        public DateTime Time { get; set; }

        /// <summary>
        /// Type of income.
        /// </summary>
        [Column("IncomeType")]
        public IncomeType IncomeType { get; set; }

        /// <summary>
        /// Raw amount received.
        /// </summary>
        [Column("Amount", TypeName = "decimal(20,10)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Fee deducted from the income.
        /// </summary>
        [Column("Fee", TypeName = "decimal(20,10)")]
        public decimal Fee { get; set; }

        /// <summary>
        /// Net income after fees.
        /// </summary>
        [Column("NetIncome", TypeName = "decimal(20,10)")]
        public decimal NetIncome { get; set; }

        /// <summary>
        /// Status returned by Binance.
        /// </summary>
        [Column("Status", TypeName = "varchar"), MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        /// <summary>Navigation to the related session.</summary>
        [ForeignKey(nameof(SessionId))]
        public virtual BinanceSessionDbEntity Session { get; set; } = null!;
    }
}
