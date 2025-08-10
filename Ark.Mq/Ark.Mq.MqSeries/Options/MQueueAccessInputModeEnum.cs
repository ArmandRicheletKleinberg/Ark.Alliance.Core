namespace Ark.Net.MqSeries
{
    /// <summary>
    /// The input access of the queue if any.
    /// </summary>
    public enum MQueueAccessInputModeEnum
    {
        /// <summary>
        /// No input access for this queue.
        /// </summary>
        None = 0,

        /// <summary>
        /// The input access is taken the default mode configured for the queue on the server.
        /// </summary>
        QueueDefault = 1,

        /// <summary>
        /// The input access is shared with others that can get simultaneously the messages.
        /// </summary>
        Shared = 2,

        /// <summary>
        /// The input access is exclusive to this calling method.
        /// </summary>
        Exclusive = 3
    }
}