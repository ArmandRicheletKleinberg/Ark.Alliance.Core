// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Ark
{
    /// <inheritdoc />
    /// <summary>
    /// This class extends the SemaphoreSlim class with wait/release helpers.
    /// </summary>
    public class SemaphoreSlimEx : SemaphoreSlim
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="SemaphoreSlimEx"/> instance.
        /// </summary>
        /// <param name="initialCount">The initial count for the semaphore.</param>
        public SemaphoreSlimEx(int initialCount) : base(initialCount)
        { }

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="T:Jvd.SemaphoreSlimEx" /> instance.
        /// </summary>
        /// <param name="initialCount">The initial count for the semaphore.</param>
        /// <param name="maxCount">The maximum count for the semaphore.</param>
        public SemaphoreSlimEx(int initialCount, int maxCount) : base(initialCount, maxCount)
        { }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Waits for the semaphore to be available before executing an synchronous action.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="action">The synchronous action.</param>
        /// <returns>True if success, false if timeout.</returns>
        public void WaitAndRelease(Action action)
        {
            Wait();
            try
            {
                action();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an synchronous action.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="action">The synchronous action.</param>
        /// <param name="token">A cancellation token to cancel the wait.</param>
        /// <returns>True if success, false if timeout.</returns>
        public void WaitAndRelease(Action action, CancellationToken token)
        {
            Wait(token);
            try
            {
                action();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an synchronous action.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="action">The synchronous action.</param>
        /// <param name="timeout">The Wait timeout.</param>
        /// <returns>True if success, false if timeout.</returns>
        public bool WaitAndRelease(Action action, TimeSpan timeout)
        {
            if (!Wait(timeout)) return false;
            try
            {
                action();
                return true;
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an synchronous action.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="action">The synchronous action.</param>
        /// <param name="timeout">The Wait timeout.</param>
        /// <param name="token">A cancellation token to cancel the wait.</param>
        /// <returns>True if success, false if timeout.</returns>
        public bool WaitAndRelease(Action action, TimeSpan timeout, CancellationToken token)
        {
            if (!Wait(timeout, token)) return false;
            try
            {
                action();
                return true;
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an synchronous function.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="function">The synchronous function.</param>
        /// <returns>The return value of the function.</returns>
        public T WaitAndRelease<T>(Func<T> function)
        {
            Wait();
            try
            {
                return function();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an synchronous function.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="function">The synchronous function.</param>
        /// <param name="token">A cancellation token to cancel the wait.</param>
        /// <returns>The return value of the function.</returns>
        public T WaitAndRelease<T>(Func<T> function, CancellationToken token)
        {
            Wait(token);
            try
            {
                return function();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an synchronous function.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="function">The synchronous function.</param>
        /// <param name="timeout">The Wait timeout.</param>
        /// <param name="timeoutValue">The value to return when timeout.</param>
        /// <returns>The return value of the function.</returns>
        public T WaitAndRelease<T>(Func<T> function, TimeSpan timeout, T timeoutValue)
        {
            if (!Wait(timeout)) return timeoutValue;
            try
            {
                return function();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an synchronous function.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="function">The synchronous function.</param>
        /// <param name="timeout">The Wait timeout.</param>
        /// <param name="timeoutValue">The value to return when timeout.</param>
        /// <param name="token">A cancellation token to cancel the wait.</param>
        /// <returns>The return value of the function.</returns>
        public T WaitAndRelease<T>(Func<T> function, TimeSpan timeout, T timeoutValue, CancellationToken token)
        {
            if (!Wait(timeout, token)) return timeoutValue;
            try
            {
                return function();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an asynchronous Task.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="asyncTask">The asynchronous Task.</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        public async System.Threading.Tasks.Task WaitAndReleaseAsync(Func<System.Threading.Tasks.Task> asyncTask)
        {
            await WaitAsync();
            try
            {
                await asyncTask();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an asynchronous Task.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="asyncTask">The asynchronous Task.</param>
        /// <param name="token">A cancellation token to cancel the wait.</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        public async System.Threading.Tasks.Task WaitAndReleaseAsync(Func<System.Threading.Tasks.Task> asyncTask, CancellationToken token)
        {
            await WaitAsync(token);
            try
            {
                await asyncTask();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an asynchronous Task.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="asyncTask">The asynchronous Task.</param>
        /// <param name="timeout">The Wait timeout.</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        public async ValueTask<bool> WaitAndReleaseAsync(Func<System.Threading.Tasks.Task> asyncTask, TimeSpan timeout)
        {
            if (!await WaitAsync(timeout)) return false;
            try
            {
                await asyncTask();
                return true;
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an asynchronous Task.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="asyncTask">The asynchronous Task.</param>
        /// <param name="timeout">The Wait timeout.</param>
        /// <param name="token">A cancellation token to cancel the wait.</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        public async ValueTask<bool> WaitAndReleaseAsync(Func<System.Threading.Tasks.Task> asyncTask, TimeSpan timeout, CancellationToken token)
        {
            if (!await WaitAsync(timeout, token)) return false;
            try
            {
                await asyncTask();
                return true;
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an asynchronous Task with value.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="asyncTask">The asynchronous Task with value.</param>
        /// <returns>The value returned from the Task.</returns>
        public async ValueTask<T> WaitAndReleaseAsync<T>(Func<Task<T>> asyncTask)
        {
            await WaitAsync();
            try
            {
                return await asyncTask();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an asynchronous Task with value.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="asyncTask">The asynchronous Task with value.</param>
        /// <param name="token">A cancellation token to cancel the wait.</param>
        /// <returns>The value returned from the Task.</returns>
        public async ValueTask<T> WaitAndReleaseAsync<T>(Func<Task<T>> asyncTask, CancellationToken token)
        {
            await WaitAsync(token);
            try
            {
                return await asyncTask();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an asynchronous Task with value.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="asyncTask">The asynchronous Task with value.</param>
        /// <param name="timeout">The Wait timeout.</param>
        /// <param name="timeoutValue">The value to return when timeout.</param>
        /// <returns>The value returned from the Task.</returns>
        public async ValueTask<T> WaitAndReleaseAsync<T>(Func<Task<T>> asyncTask, TimeSpan timeout, T timeoutValue)
        {
            if (!await WaitAsync(timeout)) return timeoutValue;
            try
            {
                return await asyncTask();
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Waits for the semaphore to be available before executing an asynchronous Task with value.
        /// It releases automatically the semaphore count upon Task completion.
        /// </summary>
        /// <param name="asyncTask">The asynchronous Task with value.</param>
        /// <param name="timeout">The Wait timeout.</param>
        /// <param name="timeoutValue">The value to return when timeout.</param>
        /// <param name="token">A cancellation token to cancel the wait.</param>
        /// <returns>The value returned from the Task.</returns>
        public async ValueTask<T> WaitAndReleaseAsync<T>(Func<Task<T>> asyncTask, TimeSpan timeout, T timeoutValue, CancellationToken token)
        {
            if (!await WaitAsync(timeout, token)) return timeoutValue;
            try
            {
                return await asyncTask();
            }
            finally
            {
                Release();
            }
        }

        #endregion Methods (Public)
    }
}