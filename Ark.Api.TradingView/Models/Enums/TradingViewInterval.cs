namespace Ark.Core.Api.TradingView.Models
{
    /// <summary>
    /// Represents the candle interval used when retrieving historical data.
    /// + Maps to TradingView resolution codes like <c>1</c> or <c>15S</c>.
    /// - Not every interval is supported for all symbols.
    /// Ref: <see href="https://www.tradingview.com/support/solutions/43000529350-history/"/>
    /// </summary>
    public enum TradingViewInterval
    {
        /// <summary>Fifteen seconds interval (<c>15S</c>).</summary>
        Second15,
        /// <summary>Thirty seconds interval (<c>30S</c>).</summary>
        Second30,
        /// <summary>One minute interval (<c>1</c>).</summary>
        Minute1,
        /// <summary>Five minutes interval (<c>5</c>).</summary>
        Minute5,
        /// <summary>Fifteen minutes interval (<c>15</c>).</summary>
        Minute15,
        /// <summary>Hourly interval (<c>60</c>).</summary>
        Hour1,
        /// <summary>Four hours interval (<c>240</c>).</summary>
        Hour4,
        /// <summary>Daily interval (<c>D</c>).</summary>
        Day1,
        /// <summary>Weekly interval (<c>W</c>).</summary>
        Week1,
        /// <summary>Monthly interval (<c>M</c>).</summary>
        Month1
    }
}
