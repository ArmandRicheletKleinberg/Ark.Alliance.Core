using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ark.Data.EFCore;
using EfIndex = Microsoft.EntityFrameworkCore.IndexAttribute;

#nullable enable

namespace Ark.Api.Binance
{
    /// <summary>
    /// Records latency measurements for Binance API calls.
    /// + Helps analyse network and processing delays.
    /// - Can grow quickly; purge old data regularly.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/modeling/indexes"/>
    /// </summary>
    [Table("LatencyMeasurements")]
    [EfIndex(nameof(Endpoint), nameof(MeasuredAt))]
    public class LatencyMeasurementDbEntity : DbEntity<BinanceDbContext>
    {
        #region Properties

        /// <summary>
        /// Surrogate identifier for the measurement.
        /// + Supports efficient lookups.
        /// - Not meaningful outside the database.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Endpoint being measured (e.g., REST path).
        /// + Enables grouping metrics by operation.
        /// - Free-form; inconsistent naming hampers aggregation.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// HTTP method or request type.
        /// + Useful to differentiate same endpoint by verb.
        /// - Optional; empty value reduces context.
        /// </summary>
        [MaxLength(20)]
        public string RequestType { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when request was sent.
        /// + Allows calculation of total latency.
        /// - Caller must supply a UTC value.
        /// </summary>
        public DateTime RequestStartTime { get; set; }

        /// <summary>
        /// Timestamp when response was received.
        /// + Used with <see cref="RequestStartTime"/> to compute total latency.
        /// - Caller must ensure consistent clock.
        /// </summary>
        public DateTime ResponseReceivedTime { get; set; }

        /// <summary>
        /// Server timestamp provided by Binance, if available.
        /// + Helps detect clock drift.
        /// - May be null when not returned.
        /// </summary>
        public DateTime? BinanceTimestamp { get; set; }

        /// <summary>
        /// Total latency in milliseconds.
        /// + Sum of network and processing latencies.
        /// - Negative values are invalid.
        /// </summary>
        [Column(TypeName = "decimal(10,3)")]
        public decimal TotalLatencyMs { get; set; }

        /// <summary>
        /// Network portion of latency in milliseconds.
        /// + Useful for connectivity diagnostics.
        /// - Requires client-side measurement logic.
        /// </summary>
        [Column(TypeName = "decimal(10,3)")]
        public decimal NetworkLatencyMs { get; set; }

        /// <summary>
        /// Processing time on the server in milliseconds.
        /// + Helps pinpoint backend bottlenecks.
        /// - Estimated value, not exact.
        /// </summary>
        [Column(TypeName = "decimal(10,3)")]
        public decimal ProcessingLatencyMs { get; set; }

        /// <summary>
        /// Indicates whether the call succeeded.
        /// + Allows filtering successful versus failed calls.
        /// - False does not imply retry behaviour.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Optional error code returned by Binance.
        /// + Enables quick failure categorization.
        /// - Empty when call succeeds.
        /// </summary>
        [MaxLength(50)]
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Timestamp when the measurement was recorded in UTC.
        /// + Useful for time-based queries.
        /// - Automatically set to current time by default.
        /// </summary>
        public DateTime MeasuredAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Additional context serialized as JSON.
        /// + Allows storing request metadata.
        /// - Consumers must validate JSON content.
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string AdditionalDataJson { get; set; } = "{}";

        #endregion Properties
    }
}
