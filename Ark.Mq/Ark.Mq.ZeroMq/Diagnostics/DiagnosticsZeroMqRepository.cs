using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Logging;

namespace Ark.Net.ZeroMq.Diagnostics;

/// <summary>
/// Repository used to perform diagnostics operations on ZeroMQ.
/// </summary>
public class DiagnosticsZeroMqRepository : ZeroMqRepositoryBase
{
    public DiagnosticsZeroMqRepository(ZeroMqConnectionPool pool, ILogger<DiagnosticsZeroMqRepository> logger)
        : base(pool, logger)
    {
    }

    /// <summary>
    /// Placeholder method to read messages from a topic.
    /// </summary>
    /// <param name="queue">The queue name.</param>
    /// <param name="maxMessagesNumber">Number of messages to read.</param>
    /// <returns>
    /// Success : The execution has succeeded and the list of messages has been returned.
    /// NotFound : No message was found in the queue.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    public Task<Result<List<string>>> GetQueueXFirstMessages(string queue, int maxMessagesNumber)
        => Task.FromResult(Result<List<string>>.Unexpected);
}
