using System;
using System.Threading.Tasks;
using Ark.Net.MqSeries.Ftp;

namespace Ark.Net.MqSeries
{
    /// <inheritdoc />
    /// <summary>
    /// Ce repository est utilisé pour communiquer avec un serveur MQ Series.
    /// </summary>
    internal class FtpMqSeriesRepository : MqSeriesRepositoryBase
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="FtpMqSeriesRepository" /> instance.
        /// </summary>
        public FtpMqSeriesRepository(string connectionPoolName)
        : base(connectionPoolName)
        { }

        #endregion Constructors

        #region Methods (Public)

        /// <inheritdoc>
        /// <cref>MqSeriesRepositoryBase.BrowseFirstRawStringMessageAndRemoveOnActionSuccess</cref>
        /// </inheritdoc>
        public Task<Result<MainFrameFileMfEntity>> FtpBrowseFirstMainFrameObjectAndRemoveOnActionSuccess(string queueKey, Func<MainFrameFileMfEntity, Task<Result>> action)
            => BrowseFirstMainFrameObjectAndRemoveOnActionSuccess<MainFrameFileMfEntity>(queueKey, action);


        #endregion Methods (Public)
    }
}


