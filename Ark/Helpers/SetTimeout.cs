namespace Ark
{
    /// <summary>
    /// Provides a convenient way to run an action after a given delay with the ability to cancel.
    /// </summary>
    public static class SetTimeout
    {
        /// <summary>
        /// Executes the specified <paramref name="method"/> after <paramref name="delayInMilliseconds"/>.
        /// </summary>
        /// <param name="method">Action to invoke once the delay has elapsed.</param>
        /// <param name="delayInMilliseconds">Delay before executing the action.</param>
        /// <returns>A <see cref="CancellationTokenSource"/> allowing the caller to cancel the action.</returns>
        public static CancellationTokenSource Create(Action method, int delayInMilliseconds)
        {
            var cts = new CancellationTokenSource();
            // ReSharper disable once MethodSupportsCancellation
            System.Threading.Tasks.Task.Run(() => ExecuteAsync(method, cts, delayInMilliseconds));
            return cts;
        }

        /// <summary>
        /// Internal worker that waits for the delay and invokes the action if not cancelled.
        /// </summary>
        /// <param name="method">Action to invoke.</param>
        /// <param name="cts">Token source used for cancellation.</param>
        /// <param name="delayInMilliseconds">Delay before execution.</param>
        private static async System.Threading.Tasks.Task ExecuteAsync(Action method, CancellationTokenSource cts, int delayInMilliseconds)
        {
            try
            {
                await System.Threading.Tasks.Task.Delay(delayInMilliseconds, cts.Token);

                if (!cts.IsCancellationRequested)
                    method();
            }
            catch (OperationCanceledException)
            {
                // Swallow the cancellation exception as this is expected.
            }
            finally
            {
                cts.Dispose();
            }
        }
    }
}