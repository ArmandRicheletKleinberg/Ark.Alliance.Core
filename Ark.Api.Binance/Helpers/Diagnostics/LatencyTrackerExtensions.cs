using System.Threading.Tasks;
using Ark.App.Diagnostics;
using Ark.Api.Binance.Services; 

namespace Ark.Api.Binance
{
    /// <summary>
    /// Extends <see cref="LatencyTracker"/> with an asynchronous error completion wrapper.
    /// + Provides an awaitable wrapper around <see cref="LatencyTracker.CompleteWithError(string)"/>
    /// - Does not include additional latency data beyond the synchronous call
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task"/>
    /// </summary>
    public static class LatencyTrackerExtensions
    {
        /// <summary>
        /// Completes the latency tracker with the specified error message asynchronously.
        /// + Enables awaiting of error completions without relying on internal APIs
        /// - Returns a completed task without capturing exception context
        /// </summary>
        /// <param name="tracker">The tracker to complete.</param>
        /// <param name="reason">Human‑readable error description.</param>
        /// <returns>A completed <see cref="Task"/> representing the operation.</returns>
        public static Task CompleteWithErrorAsync(this LatencyTracker tracker, string reason)
        {
            tracker.CompleteWithError(reason);
            return Task.CompletedTask;
        }
    }
}
