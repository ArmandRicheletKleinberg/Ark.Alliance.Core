using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ark.Data.EFCore;
using EfIndex = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Stores fee configuration for a Binance futures symbol.
    /// + Enables quick lookup of maker/taker rates by VIP level.
    /// - Values may become outdated without periodic refresh.
    /// Ref: <see href="https://www.binance.com/en/fee/futures"/>
    /// </summary>
    [Table("FeeRules")]
    [EfIndex(nameof(Symbol), IsUnique = true)]
    public class FeeRulesDbEntity : DbEntity<BinanceDbContext>
    {
        #region Properties

        /// <summary>
        /// Surrogate identifier.
        /// + Stable primary key for ORM usage.
        /// - Not exposed by Binance APIs.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Futures symbol the rules apply to.
        /// + Acts as natural key.
        /// - Case-sensitive according to Binance conventions.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Maker fee for VIP0 level expressed as a decimal rate.
        /// + Allows precise storage up to 8 decimals.
        /// - Must be synchronized with Binance fee schedule.
        /// </summary>
        [Column(TypeName = "decimal(10,8)")]
        public decimal MakerFeeVip0 { get; set; }

        /// <summary>
        /// Taker fee for VIP0 level expressed as a decimal rate.
        /// + Supports high precision values.
        /// - May not reflect temporary promotions.
        /// </summary>
        [Column(TypeName = "decimal(10,8)")]
        public decimal TakerFeeVip0 { get; set; }

        /// <summary>
        /// Serialized VIP fee rates in JSON format.
        /// + Flexibly stores per-level maker/taker fees.
        /// - Requires JSON parsing on usage.
        /// Example:
        /// <code>{"vip1":{"maker":0.00016,"taker":0.0004}}</code>
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string VipRatesJson { get; set; } = "{}";

        /// <summary>
        /// Liquidation fee rate applied by the exchange.
        /// + Useful for risk calculations.
        /// - Subject to exchange policy changes.
        /// </summary>
        [Column(TypeName = "decimal(10,8)")]
        public decimal LiquidationFeeRate { get; set; }

        /// <summary>
        /// Current funding rate estimate.
        /// + Helps project holding costs.
        /// - Only an estimate; actual rate may vary.
        /// </summary>
        [Column(TypeName = "decimal(10,8)")]
        public decimal CurrentFundingRate { get; set; }

        /// <summary>
        /// Next scheduled funding time in UTC.
        /// + Indicates when funding rate will apply.
        /// - Value becomes stale after funding occurs.
        /// </summary>
        public DateTime NextFundingTime { get; set; }

        /// <summary>
        /// Indicates if BNB discount is applied.
        /// + Allows fee reduction using BNB holdings.
        /// - Requires sufficient BNB balance to take effect.
        /// </summary>
        public bool BnbDiscountEnabled { get; set; } = true;

        /// <summary>
        /// Timestamp of the last update in UTC.
        /// + Aids auditing of fee changes.
        /// - Must be kept in sync with actual update operations.
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User or system that updated the record.
        /// + Helps trace configuration changes.
        /// - Free-form string; consider standardizing values.
        /// </summary>
        [MaxLength(128)]
        public string UpdatedBy { get; set; } = "System";

        /// <summary>
        /// Additional advanced rules in JSON format.
        /// + Extensible without schema changes.
        /// - Consumers must validate structure.
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string AdvancedRulesJson { get; set; } = "{}";

        /// <summary>
        /// Flag indicating whether the rule set is active.
        /// + Inactive rows can remain for auditing.
        /// - Consumers must filter inactive entries.
        /// </summary>
        public bool IsActive { get; set; } = true;

        #endregion Properties
    }
}
