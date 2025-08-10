using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ark.Data.EFCore;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Represents Binance request rate limit thresholds for an endpoint category.
    /// + Enables proactive throttling based on exchange quotas.
    /// - Requires manual updates when Binance revises limits.
    /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#limits"/>
    /// </summary>
    [Table("RateLimitRules")]
    public class RateLimitRulesDbEntity : DbEntity<BinanceDbContext>
    {
        #region Properties

        /// <summary>
        /// Surrogate identifier.
        /// + Simplifies database joins.
        /// - Not part of Binance specifications.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Endpoint category these limits apply to.
        /// + Acts as natural key.
        /// - Free-form value; ensure consistency with client categories.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string EndpointCategory { get; set; } = string.Empty;

        /// <summary>
        /// Weight limit allowed per minute.
        /// + Used to control general request weight.
        /// - May vary across Binance deployments.
        /// </summary>
        public int WeightLimitPerMinute { get; set; } = 2400;

        /// <summary>
        /// Order count limit per minute.
        /// + Prevents order bursts.
        /// - Assumes standard account tier.
        /// </summary>
        public int OrderLimitPerMinute { get; set; } = 1200;

        /// <summary>
        /// Order count limit for any 10 second window.
        /// + Shields from short spikes.
        /// - May be more restrictive during volatility.
        /// </summary>
        public int OrderLimitPer10Seconds { get; set; } = 300;

        /// <summary>
        /// Request weight of the endpoint.
        /// + Multiplies against global weight limits.
        /// - Zero or negative values produce invalid calculations.
        /// </summary>
        public int RequestWeight { get; set; } = 1;

        /// <summary>
        /// Usage ratio at which alerts should be raised.
        /// + Allows early warning before limits are hit.
        /// - Too low values may generate noisy alerts.
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal AlertThreshold { get; set; }

        /// <summary>
        /// Ratio under which the system considers usage recovered.
        /// + Enables automatic resumption of requests.
        /// - Misconfiguration may resume too early.
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal RecoveryThreshold { get; set; }

        /// <summary>
        /// Creation timestamp in UTC.
        /// + Useful for auditing rule changes.
        /// - Not automatically updated on modifications.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last update timestamp in UTC.
        /// + Reflects the most recent configuration change.
        /// - Must be set by the caller when updating.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indicates if the rule set is currently enforced.
        /// + Allows temporary deactivation without deleting.
        /// - Consumers must check this flag before use.
        /// </summary>
        public bool IsActive { get; set; } = true;

        #endregion Properties
    }
}
