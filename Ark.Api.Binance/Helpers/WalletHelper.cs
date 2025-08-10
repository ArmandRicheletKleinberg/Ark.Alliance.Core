using System.Threading;
using System.Threading.Tasks;
using Ark;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Extension methods for wallet operations on a <see cref="BinanceSession"/>.
    /// </summary>
    public static class WalletHelper
    {
        /// <summary>
        /// Transfers funds from the futures wallet to the funding wallet.
        /// </summary>
        public static async Task<Result> TransferFuturesToFundingAsync(this BinanceSession session, string asset, decimal quantity, CancellationToken token = default)
        {
            var result = await session.Client.TransferFuturesToFundingAsync(asset, quantity, token);
            return result;
        }

        /// <summary>
        /// Transfers funds from the funding wallet to the futures wallet.
        /// </summary>
        public static async Task<Result> TransferFundingToFuturesAsync(this BinanceSession session, string asset, decimal quantity, CancellationToken token = default)
        {
            var result = await session.Client.TransferFundingToFuturesAsync(asset, quantity, token);
            return result;
        }
    }
}
