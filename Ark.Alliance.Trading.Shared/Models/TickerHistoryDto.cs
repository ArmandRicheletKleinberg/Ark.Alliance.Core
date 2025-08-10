namespace Ark.Alliance.Trading.Shared.Models;

using Ark.Api.Binance;
using System.Collections.Generic;

/// <summary>
/// Snapshot of collected ticker data for multiple symbols.
/// + Enables correlation of historical quotes across providers.
/// - Requires external pruning to avoid unbounded memory growth.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2" />
/// </summary>
public class TickerHistoryDto
{
    #region Properties

    /// <summary>
    /// Historical ticks keyed by trading symbol.
    /// + Provides ordered <see cref="TickerDto" /> sequences per symbol.
    /// - Symbols with heavy activity may cause high memory usage.
    /// </summary>
    /// <example>
    /// {
    ///   "BTCUSDT": [
    ///     { "Symbol": "BTCUSDT", "Price": 30000.0, "Timestamp": "2024-01-01T00:00:00Z" }
    ///   ]
    /// }
    /// </example>
    public Dictionary<string, TickerDto[]> History { get; set; } = new();

    #endregion Properties
}
