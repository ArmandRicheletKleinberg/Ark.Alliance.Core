namespace Ark.Api.Binance
{
    /// <summary>
    /// Specifies the Binance environment to use.
    /// + <see cref="Testnet"/> enables safe experimentation.
    /// - <see cref="Production"/> actions affect real funds.
    /// </summary>
    public enum BinanceEnvironment
    {
        /// <summary>Connects to the Binance test network.</summary>
        Testnet,
        /// <summary>Connects to the live Binance environment.</summary>
        Production
    }
}
