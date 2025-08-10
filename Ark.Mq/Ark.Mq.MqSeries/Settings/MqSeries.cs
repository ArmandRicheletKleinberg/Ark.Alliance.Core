namespace Ark.Net.MqSeries
{
    /// <summary>
    /// This is a static helper used to Initialize the MQ series repositories by settings the connection pools information.
    /// </summary>
    public static class MqSeries
    {
        /// <summary>
        /// Adds the MQ Series support to the app.
        /// It mainly initializes the connection pools used by the MQSeriesRepositoryBase repositories.
        /// </summary>
        /// <param name="connectionPoolsSettings">The connection pools to use by the MQ Series repositories.</param>
        public static void Initialize(MQueueConnectionPoolSettings[] connectionPoolsSettings)
        {
            MqSeriesRepositoryBase.Initialize(connectionPoolsSettings);
        }
    }
}