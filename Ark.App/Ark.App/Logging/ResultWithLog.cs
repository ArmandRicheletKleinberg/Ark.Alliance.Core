using System;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.App
{
    /// <summary>
    /// Helper methods that wrap
    /// <see cref="Result.SafeExecute(System.Func{Ark.Result}, System.Action{System.Exception})"/>
    /// and log unexpected errors, removing repetitive
    /// <c>try/catch</c> blocks in application code.
    /// <para>Example usage:</para>
    /// <code>
    /// var result = ResultWithLog.SafeExecute(MyWork, _logger);
    /// </code>
    /// <para>+ Ensures that all failures are logged consistently.</para>
    /// <para>- Introduces minor overhead compared to inline error handling.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/logging">Logging in .NET</see></para>
    /// </summary>
    public class ResultWithLog
    {
        #region Static Methods (Safe Execute)

        /// <summary>
        /// Safely executes an action that returns a <see cref="Result"/>.
        /// <para>+ Converts thrown exceptions into <c>Unexpected</c> results and logs them.</para>
        /// <para>- Does not log successful executions.</para>
        /// </summary>
        /// <param name="actionToSafelyExecute">The action to safely execute.</param>
        /// <param name="logger">Logger used to record the result when it is not successful.</param>
        /// <returns>A <see cref="Result"/> instance. Format: <c>{"isSuccess":bool,"error":string}</c>.</returns>
        public static Result SafeExecute(Func<Result> actionToSafelyExecute, ILogger logger)
        {
            var result = Result.SafeExecute(actionToSafelyExecute);
            if (result.IsNotSuccess)
                logger?.LogResult(result);

            return result;
        }

        /// <summary>
        /// Safely executes an asynchronous action that returns a <see cref="Result"/>.
        /// <para>+ Awaits the operation and logs any non-success outcome.</para>
        /// <para>- Adds an extra allocation for the asynchronous state machine.</para>
        /// </summary>
        /// <param name="actionToSafelyExecute">The asynchronous action to execute.</param>
        /// <param name="logger">Logger used to record the result when it is not successful.</param>
        /// <returns>A <see cref="Result"/> instance. Format: <c>{"isSuccess":bool,"error":string}</c>.</returns>
        public static async Task<Result> SafeExecute(Func<Task<Result>> actionToSafelyExecute, ILogger logger)
        {
            var result = await Result.SafeExecute(actionToSafelyExecute);
            if (result.IsNotSuccess)
                logger?.LogResult(result);

            return result;
        }

        #endregion Static Methods (Safe Execute)
    }

    /// <summary>
    /// Extends <see cref="Result{T}"/> with logging support for typed results.
    /// <para>+ Preserves the payload while ensuring failures are logged.</para>
    /// <para>- Slightly increases allocations for generic logging scopes.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/logging">Logging in .NET</see></para>
    /// </summary>
    public class ResultWithLog<T>
    {
        #region Static Methods (Safe Execute)

        /// <summary>
        /// Safely executes an action that returns a <see cref="Result{T}"/>.
        /// <para>+ Converts thrown exceptions into <c>Unexpected</c> results and logs them.</para>
        /// <para>- Successful outcomes are not logged.</para>
        /// </summary>
        /// <param name="actionToSafelyExecute">The action to safely execute.</param>
        /// <param name="logger">Logger used to record the result when it is not successful.</param>
        /// <returns>A <see cref="Result{T}"/> instance. Format: <c>{"isSuccess":bool,"value":T,"error":string}</c>.</returns>
        public static Result<T> SafeExecute(Func<Result<T>> actionToSafelyExecute, ILogger logger)
        {
            var result = Result<T>.SafeExecute(actionToSafelyExecute);
            if (result.IsNotSuccess)
                logger?.LogResult(result);

            return result;
        }

        /// <summary>
        /// Safely executes an asynchronous action that returns a <see cref="Result{T}"/>.
        /// <para>+ Awaits the operation and logs any non-success outcome.</para>
        /// <para>- Adds an asynchronous state machine allocation.</para>
        /// </summary>
        /// <param name="actionToSafelyExecute">The asynchronous action to execute.</param>
        /// <param name="logger">Logger used to record the result when it is not successful.</param>
        /// <returns>A <see cref="Result{T}"/> instance. Format: <c>{"isSuccess":bool,"value":T,"error":string}</c>.</returns>
        public static async Task<Result<T>> SafeExecute(Func<Task<Result<T>>> actionToSafelyExecute, ILogger logger)
        {
            var result = await Result<T>.SafeExecute(actionToSafelyExecute);
            if (result.IsNotSuccess)
                logger?.LogResult(result);

            return result;
        }

        #endregion Static Methods (Safe Execute)
    }
}
