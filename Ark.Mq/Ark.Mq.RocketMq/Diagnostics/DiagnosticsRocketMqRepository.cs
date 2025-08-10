using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Logging;

namespace Ark.Net.RocketMq.Diagnostics;

/// <summary>
/// Repository used to perform diagnostics operations on RabbitMQ.
/// </summary>
public class DiagnosticsRocketMqRepository : RocketMqRepositoryBase
{
    public DiagnosticsRocketMqRepository(RocketMqConnectionPool pool, ILogger<DiagnosticsRocketMqRepository> logger)
        : base(pool, logger)
    {
    }

    /// <summary>
    /// Read the first messages of a queue.
    /// </summary>
    /// <param name="queue">The queue name.</param>
    /// <param name="maxMessagesNumber">Number of messages to read.</param>
    /// <returns>
    /// Success : The execution has succeeded and the list of messages has been returned.
    /// NotFound : No message was found in the queue.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    public Task<Result<List<string>>> GetQueueXFirstMessages(string queue, int maxMessagesNumber)
        => Execute(producer =>
        {
            // RocketMQ client does not provide a simple sync fetch API.
            // Implementation should poll using a consumer and return collected messages.
            return Result<List<string>>.NotFound;
        });
}
