namespace Ark
{
    /// <summary>
    /// This helper class extends the CancellationTokenSource object.
    /// </summary>
    public static class CancellationTokenSourceExtensibility
    {
        /// <summary>
        /// Tries to cancel an asynchronous execution using a CancellationToken.
        /// This method is safe even if the CancellationTokenSource has been already disposed.
        /// </summary>
        /// <param name="cancellationTokenSource">The CancellationTokenSource used to cancel the asynchronous Task.</param>
        public static void TryCancel(this CancellationTokenSource cancellationTokenSource)
        {
            try { cancellationTokenSource.Cancel(); }
            catch (ObjectDisposedException) { /* Do nothing as it can happen */ }
        }

        /// <summary>
        /// Tries to cancel an asynchronous execution using a CancellationToken.
        /// This method is safe even if the CancellationTokenSource has been already disposed.
        /// </summary>
        /// <param name="cancellationTokenSource">The CancellationTokenSource used to cancel the asynchronous Task.</param>
        /// <param name="throwOnFirstException">Whether the exceptions should immediately propagate and prevent the remaining callbacks to be executed.</param>
        public static void TryCancel(this CancellationTokenSource cancellationTokenSource, bool throwOnFirstException)
        {
            try { cancellationTokenSource.Cancel(throwOnFirstException); }
            catch (ObjectDisposedException) { /* Do nothing as it can happen */ }
        }

        /// <summary>
        /// Tries to cancel an asynchronous execution using a CancellationToken after a delay.
        /// This method is safe even if the CancellationTokenSource has been already disposed.
        /// </summary>
        /// <param name="cancellationTokenSource">The CancellationTokenSource used to cancel the asynchronous Task.</param>
        /// <param name="millisecondsDelay">The delay in ms.</param>
        public static void TryCancelAfter(this CancellationTokenSource cancellationTokenSource, int millisecondsDelay)
        {
            try { cancellationTokenSource.CancelAfter(millisecondsDelay); }
            catch (ObjectDisposedException) { /* Do nothing as it can happen */ }
        }

        /// <summary>
        /// Tries to cancel an asynchronous execution using a CancellationToken after a delay.
        /// This method is safe even if the CancellationTokenSource has been already disposed.
        /// </summary>
        /// <param name="cancellationTokenSource">The CancellationTokenSource used to cancel the asynchronous Task.</param>
        /// <param name="timeSpan">The delay as a TimeSpan.</param>
        public static void TryCancelAfter(this CancellationTokenSource cancellationTokenSource, TimeSpan timeSpan)
        {
            try { cancellationTokenSource.CancelAfter(timeSpan); }
            catch (ObjectDisposedException) { /* Do nothing as it can happen */ }
        }
    }
}