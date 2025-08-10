// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark
{
    /// <summary>
    /// This class is used to retry an action several times until success or maximum number of attempts.
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// Retries many times an asynchronous function given a success condition.
        /// </summary>
        /// <typeparam name="T">The type of the function to return.</typeparam>
        /// <param name="function">The function to execute asynchronously.</param>
        /// <param name="successConditionFunction">The condition to the result to check for success.</param>
        /// <param name="failureValueFunction">The function to specify the return value in case of error. Default, the last return value.</param>
        /// <param name="retryCount">The number of times to retry.</param>
        /// <param name="retryIntervalLaps">The laps to wait before each tries.</param>
        /// <returns>The function return result of the last successful try or default value.</returns>
        public static async Task<T> Do<T>(Func<Task<T>> function, Func<T, bool> successConditionFunction = null, Func<T, T> failureValueFunction = null, int retryCount = 3, TimeSpan retryIntervalLaps = default)
        {
            var result = default(T);
            for (var retry = 0; retry < retryCount; retry++)
            {
                var success = false;
                try
                {
                    // Executes the function and checks for success
                    result = await function();
                    success = successConditionFunction?.Invoke(result) ?? true;
                    if (!success) continue;

                    // If success then return the result
                    return result;
                }
                catch (Exception) { /* Do nothing */ }
                finally { if (retryIntervalLaps != default && !success) await System.Threading.Tasks.Task.Delay(retryIntervalLaps); } // Wait the interval laps
            }

            // Return the failure value
            failureValueFunction ??= t => t;
            return failureValueFunction.Invoke(result);
        }
    }
}
