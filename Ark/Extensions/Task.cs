namespace Ark
{
    /// <summary>
    /// This class extends the <see cref="Task"/> class.
    /// </summary>
    public static class TaskExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Waits for the given task allowing it to be cancelled via the provided token.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="task">The task to await.</param>
        /// <param name="cancellationToken">Token used to cancel waiting.</param>
        /// <returns>The result of the completed task.</returns>
        /// <exception cref="OperationCanceledException">The token was cancelled before the task completed.</exception>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            return await task.WaitAsync(cancellationToken);
        }


        /// <summary>
        /// This method is used to remove warning 4014 regarding a task that is not await in a async method.
        /// </summary>
        /// <param name="task">The task that should not wait.</param>
        public static void DoNotAwait(this Task task) { }



        #endregion Methods (Public)
    }
}
