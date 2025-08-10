using System.Collections.Generic;
using System.Threading.Tasks;
using Ark;
using Ark.App.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Ark.Net.ZeroMq.Diagnostics;

/// <summary>
/// Base class used to access diagnostics reports for ZeroMQ.
/// </summary>
public abstract class ZeroMqReportsBase : ReportsBase
{
    /// <summary>
    /// Get the messages from a topic.
    /// </summary>
    /// <param name="repository">The diagnostics repository.</param>
    /// <param name="queue">The topic name.</param>
    /// <param name="maxMessagesNumber">The maximum number of messages to return.</param>
    /// <returns>
    /// Success : The execution has succeeded and the data of raw string messages is returned.
    /// NotFound : No message was found in the queue.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    protected async Task<Result<List<string>>> GetReportQueueMessages(DiagnosticsZeroMqRepository repository, string queue, int maxMessagesNumber)
    {
        var resultGetMessages = await repository.GetQueueXFirstMessages(queue, maxMessagesNumber);

        if (resultGetMessages.IsNotSuccess)
            return new Result<List<string>>(resultGetMessages);

        return new Result<List<string>>(resultGetMessages.Data);
    }
}
