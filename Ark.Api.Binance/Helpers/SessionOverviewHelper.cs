using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Binance.Net.Enums;

#nullable enable

namespace Ark.Api.Binance
{
    /// <summary>
    /// Produces consolidated information for a <see cref="BinanceSession"/>.
    /// <para>+ Aggregates orders, balances and incomes in one DTO.</para>
    /// <para>- Serializes API responses; large sessions may allocate heavily.</para>
    /// <para>Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/"/></para>
    /// </summary>
    public static class SessionOverviewHelper
    {
        #region Methods (Public)
        /// <summary>
        /// Creates a <see cref="SessionOverviewDto"/> from the in-memory session data.
        /// <para>+ Filters entities by a <see cref="TimeWindow"/> and computes available balance.</para>
        /// <para>- Invokes multiple network calls; call sparingly to avoid rate limits.</para>
        /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.text.json"/></para>
        /// </summary>
        /// <param name="session">The Binance session.</param>
        /// <param name="window">Time window used to filter data.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>
        /// Aggregated overview of the session.
        /// <code language="json">
        /// {
        ///   "orders": [],
        ///   "positions": []
        /// }
        /// </code>
        /// </returns>
        public static async Task<SessionOverviewDto> ToOverviewAsync(this BinanceSession session, TimeWindow window, CancellationToken token = default)
        {
            var orders = session.Orders.Values
                .Where(r => r.IsSuccess && r.Data != null)
                .Select(r => r.Data!)
                .Where(o => o.Timestamp >= window.StartUtc && o.Timestamp <= window.EndUtc)
                .ToList();

            var positions = session.Positions.Values
                .Where(p => p.Timestamp >= window.StartUtc && p.Timestamp <= window.EndUtc)
                .ToList();

            var balances = await GetFuturesBalancesAsync(session, token);
            var incomes = await GetIncomesAsync(session, window, token);

            // Compute total available stablecoin balance (USDT + USDC)
            decimal tradingAvailable = 0m;
            if (session.FuturesBalances.TryGetValue("USDT", out var usdt))
                tradingAvailable += usdt.Available;
            if (session.FuturesBalances.TryGetValue("USDC", out var usdc))
                tradingAvailable += usdc.Available;

            return new SessionOverviewDto
            {
                Orders = orders,
                Positions = positions,
                Tickers = new Dictionary<string, List<TickerDto>>(),
                Balances = balances,
                IncomeSummaries = incomes,
                FuturesTradingAvailable = tradingAvailable,
                Environment = session.Options.Environment
            };
        }

        #endregion Methods (Public)

        #region Methods (Private)

        private static async Task<List<FuturesBalanceDto>> GetFuturesBalancesAsync(BinanceSession session, CancellationToken token)
        {
            var result = await session.Client.GetBalancesAsync(token);
            if (result.IsNotSuccess || result.Data == null)
                return session.FuturesBalances.Values.ToList();

            var json = JsonSerializer.Serialize(result.Data);
            using var doc = JsonDocument.Parse(json);
            var list = new List<FuturesBalanceDto>();
            foreach (var element in doc.RootElement.EnumerateArray())
            {
                var dto = ToBalanceDto(element);
                if (dto != null)
                    list.Add(dto);
            }

            return list.Count > 0 ? list : session.FuturesBalances.Values.ToList();
        }

        private static async Task<List<IncomeSummaryDto>> GetIncomesAsync(BinanceSession session, TimeWindow window, CancellationToken token)
        {
            var result = await session.Client.GetIncomeHistoryAsync(null, token);
            if (result.IsNotSuccess || result.Data == null)
                return new List<IncomeSummaryDto>();

            var json = JsonSerializer.Serialize(result.Data);
            using var doc = JsonDocument.Parse(json);
            var list = new List<IncomeSummaryDto>();
            foreach (var element in doc.RootElement.EnumerateArray())
            {
                var dto = ToIncomeDto(element);
                if (dto != null && dto.Time >= window.StartUtc && dto.Time <= window.EndUtc)
                    list.Add(dto);
            }

            return list;
        }

        private static FuturesBalanceDto? ToBalanceDto(JsonElement element)
        {
            try
            {
                var dto = new FuturesBalanceDto
                {
                    Asset = element.TryGetProperty("asset", out var a) ? a.GetString() ?? string.Empty : string.Empty,
                    Available = element.TryGetProperty("availableBalance", out var av) ? GetDecimal(av) :
                               element.TryGetProperty("balance", out var b) ? GetDecimal(b) : 0m,
                    MaxTransfer = element.TryGetProperty("maxWithdrawAmount", out var mw) ? GetDecimal(mw) : 0m,
                    TransfersRemaining = element.TryGetProperty("withdrawAvailable", out var wa) && wa.ValueKind == JsonValueKind.Number ? wa.GetInt32() : 0,
                    Timestamp = DateTime.UtcNow
                };
                return dto;
            }
            catch
            {
                return null;
            }
        }

        private static IncomeSummaryDto? ToIncomeDto(JsonElement element)
        {
            try
            {
                var dto = new IncomeSummaryDto
                {
                    Symbol = element.TryGetProperty("symbol", out var s) ? s.GetString() ?? string.Empty : string.Empty,
                    Time = element.TryGetProperty("time", out var t) && t.ValueKind == JsonValueKind.Number
                        ? DateTimeOffset.FromUnixTimeMilliseconds(t.GetInt64()).UtcDateTime
                        : DateTime.UtcNow,
                    IncomeType = element.TryGetProperty("incomeType", out var it) && Enum.TryParse<IncomeType>(it.GetString(), true, out var tmpIt)
                        ? tmpIt
                        : IncomeType.InternalTransfer,
                    Amount = element.TryGetProperty("income", out var inc) ? GetDecimal(inc) : 0m,
                    Fee = element.TryGetProperty("fee", out var fee) ? GetDecimal(fee) : 0m,
                    NetIncome = element.TryGetProperty("netIncome", out var net) ? GetDecimal(net) : (element.TryGetProperty("income", out var inc2) ? GetDecimal(inc2) : 0m),
                    Status = element.TryGetProperty("status", out var st) ? st.GetString() ?? string.Empty : string.Empty
                };

                return dto;
            }
            catch
            {
                return null;
            }
        }

        private static decimal GetDecimal(JsonElement element)
            => element.ValueKind == JsonValueKind.Number ? element.GetDecimal() : decimal.TryParse(element.GetString(), out var v) ? v : 0m;

        #endregion Methods (Private)
    }

}
