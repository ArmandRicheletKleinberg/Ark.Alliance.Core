// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ark
{
    /// <summary>
    /// This class is an helper to make atomic operations.
    /// It is like a simple transaction system.
    /// The principle is dead simple, the operations are executed in order and if one fail or have not the good result then 
    /// this operation and the previous rollbacks functions are executed.
    /// </summary>
    public class Atomic
    {
        #region Nested Classes

        /// <summary>
        /// The operations execution result.
        /// </summary>
        public enum ExecutionResult
        {
            /// <summary>
            /// All the operations have been successfully executed.
            /// </summary>
            Success,

            /// <summary>
            /// An error occured and the operations were rolled back successfully.
            /// </summary>
            Rollback,

            /// <summary>
            /// An error occured but the operations have not been able to be rolled back (exception during rollback).
            /// Possible non consistent state.
            /// </summary>
            FailureWhenRollback
        }

        /// <summary>
        /// This is the base operation class for either OperationFunction and OperationAction
        /// </summary>
        private abstract class OperationBase
        {
            #region Methods (Abstract)

            /// <summary>
            /// Executes the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public abstract Task<bool> Execute();

            /// <summary>
            /// Rollbacks the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public abstract Task<bool> Rollback();

            #endregion Methods (Abstract)
        }

        /// <summary>
        /// This is an operation to execute (action).
        /// </summary>
        private class OperationAction : OperationBase
        {
            #region Fields

            /// <summary>
            /// The action to execute the operation.
            /// </summary>
            private readonly Action _executeAction;

            /// <summary>
            /// The rollback action to execute if failure.
            /// </summary>
            private readonly Action _rollbackAction;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Creates a <see cref="OperationAction"/> instance.
            /// </summary>
            /// <param name="executeAction">The action to execute the operation.</param>
            /// <param name="rollbackAction">The rollback action to execute if failure.</param>
            public OperationAction(Action executeAction, Action rollbackAction)
            {
                _executeAction = executeAction;
                _rollbackAction = rollbackAction;
            }

            #endregion Constructors

            #region Methods (Public)

            /// <summary>
            /// Executes the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public override async Task<bool> Execute()
            {
                await System.Threading.Tasks.Task.CompletedTask;

                try { _executeAction(); return true; }
                catch (Exception) { return false; }
            }

            /// <summary>
            /// Rollbacks the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public override async Task<bool> Rollback()
            {
                await System.Threading.Tasks.Task.CompletedTask;

                try { _rollbackAction(); return true; }
                catch (Exception) { return false; }
            }

            #endregion Methods (Public)
        }

        /// <summary>
        /// This is an operation to execute (async action).
        /// </summary>
        private class OperationActionAsync : OperationBase
        {
            #region Fields

            /// <summary>
            /// The action to execute the operation.
            /// </summary>
            private readonly Func<System.Threading.Tasks.Task> _executeAction;

            /// <summary>
            /// The rollback action to execute if failure.
            /// </summary>
            private readonly Func<System.Threading.Tasks.Task> _rollbackAction;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Creates a <see cref="OperationAction"/> instance.
            /// </summary>
            /// <param name="executeAction">The action to execute the operation.</param>
            /// <param name="rollbackAction">The rollback action to execute if failure.</param>
            public OperationActionAsync(Func<System.Threading.Tasks.Task> executeAction, Func<System.Threading.Tasks.Task> rollbackAction)
            {
                _executeAction = executeAction;
                _rollbackAction = rollbackAction;
            }

            #endregion Constructors

            #region Methods (Public)

            /// <summary>
            /// Executes the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public override async Task<bool> Execute()
            {
                try { await _executeAction(); return true; }
                catch (Exception) { return false; }
            }

            /// <summary>
            /// Rollbacks the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public override async Task<bool> Rollback()
            {
                try { await _rollbackAction(); return true; }
                catch (Exception) { return false; }
            }

            #endregion Methods (Public)
        }

        /// <summary>
        /// This is an operation to execute (function).
        /// </summary>
        private class OperationFunction : OperationBase
        {
            #region Fields

            /// <summary>
            /// The function to execute the operation.
            /// </summary>
            private readonly Func<object> _executeFunction;

            /// <summary>
            /// The function to check for operation success.
            /// </summary>
            private readonly Func<object, bool> _successCheckFunction;

            /// <summary>
            /// The rollback action to execute if failure.
            /// </summary>
            private readonly Action<object> _rollbackAction;

            /// <summary>
            /// The result of the execution when executed.
            /// </summary>
            private object _executionResult;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Creates a <see cref="OperationFunction"/> instance.
            /// </summary>
            /// <param name="executeFunction">The function to execute the operation.</param>
            /// <param name="successCheckFunction">The function to check for operation success.</param>
            /// <param name="rollbackAction">The rollback action to execute if failure.</param>
            public OperationFunction(Func<object> executeFunction, Func<object, bool> successCheckFunction, Action<object> rollbackAction)
            {
                _executeFunction = executeFunction;
                _successCheckFunction = successCheckFunction;
                _rollbackAction = rollbackAction;
            }

            #endregion Constructors

            #region Methods (Public)

            /// <summary>
            /// Executes the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public override async Task<bool> Execute()
            {
                await System.Threading.Tasks.Task.CompletedTask;

                try
                {
                    _executionResult = _executeFunction();
                    return _successCheckFunction(_executionResult);
                }
                catch (Exception) { return false; }
            }

            /// <summary>
            /// Rollbacks the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public override async Task<bool> Rollback()
            {
                await System.Threading.Tasks.Task.CompletedTask;

                try { _rollbackAction(_executionResult); return true; }
                catch (Exception) { return false; }
            }

            #endregion Methods (Public)
        }

        /// <summary>
        /// This is an operation to execute (async function).
        /// </summary>
        private class OperationFunctionAsync : OperationBase
        {
            #region Fields

            /// <summary>
            /// The function to execute the operation.
            /// </summary>
            private readonly Func<Task<object>> _executeFunction;

            /// <summary>
            /// The function to check for operation success.
            /// </summary>
            private readonly Func<object, bool> _successCheckFunction;

            /// <summary>
            /// The rollback action to execute if failure.
            /// </summary>
            private readonly Func<object, System.Threading.Tasks.Task> _rollbackAction;

            /// <summary>
            /// The result of the execution when executed.
            /// </summary>
            private object _executionResult;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Creates a <see cref="OperationFunction"/> instance.
            /// </summary>
            /// <param name="executeFunction">The function to execute the operation.</param>
            /// <param name="successCheckFunction">The function to check for operation success.</param>
            /// <param name="rollbackAction">The rollback action to execute if failure.</param>
            public OperationFunctionAsync(Func<Task<object>> executeFunction, Func<object, bool> successCheckFunction, Func<object, System.Threading.Tasks.Task> rollbackAction)
            {
                _executeFunction = executeFunction;
                _successCheckFunction = successCheckFunction;
                _rollbackAction = rollbackAction;
            }

            #endregion Constructors

            #region Methods (Public)

            /// <summary>
            /// Executes the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public override async Task<bool> Execute()
            {
                try
                {
                    _executionResult = await _executeFunction();
                    return _successCheckFunction(_executionResult);
                }
                catch (Exception) { return false; }
            }

            /// <summary>
            /// Rollbacks the operation.
            /// </summary>
            /// <returns>True if success, false if exception or wrong result.</returns>
            public override async Task<bool> Rollback()
            {
                try { await _rollbackAction.Invoke(_executionResult); return true; }
                catch (Exception) { return false; }
            }

            #endregion Methods (Public)
        }

        #endregion Nested Classes

        #region Methods (Static)

        /// <summary>
        /// Executes a list of operations in order and rollbacks in case of error.
        /// </summary>
        /// <param name="executeAction">The action to execute the operation.</param>
        /// <param name="rollbackAction">The rollback action to execute if failure.</param>
        public static Atomic Do(Action executeAction, Action rollbackAction)
        {
            var atomic = new Atomic();
            atomic.Then(executeAction, rollbackAction);

            return atomic;
        }

        /// <summary>
        /// Executes a list of operations in order and rollbacks in case of error.
        /// </summary>
        /// <param name="executeAction">The action to execute the operation.</param>
        /// <param name="rollbackAction">The rollback action to execute if failure.</param>
        public static Atomic DoAsync(Func<System.Threading.Tasks.Task> executeAction, Func<System.Threading.Tasks.Task> rollbackAction)
        {
            var atomic = new Atomic();
            atomic.ThenAsync(executeAction, rollbackAction);

            return atomic;
        }

        /// <summary>
        /// Executes a list of operations in order and rollbacks in case of error.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="executeFunction">The function to execute the operation.</param>
        /// <param name="successCheckFunction">The function to check for operation success.</param>
        /// <param name="rollbackAction">The rollback action to execute if failure.</param>
        public static Atomic Do<T>(Func<T> executeFunction, Func<T, bool> successCheckFunction, Action<T> rollbackAction)
        {
            var atomic = new Atomic();
            atomic.Then(executeFunction, successCheckFunction, rollbackAction);

            return atomic;
        }

        /// <summary>
        /// Executes a list of operations in order and rollbacks in case of error.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="executeFunction">The function to execute the operation.</param>
        /// <param name="successCheckFunction">The function to check for operation success.</param>
        /// <param name="rollbackAction">The rollback action to execute if failure.</param>
        public static Atomic DoAsync<T>(Func<Task<T>> executeFunction, Func<T, bool> successCheckFunction, Func<T, System.Threading.Tasks.Task> rollbackAction = null)
        {
            var atomic = new Atomic();
            atomic.ThenAsync(executeFunction, successCheckFunction, rollbackAction);

            return atomic;
        }

        #endregion Methods (Static)

        #region Fields

        /// <summary>
        /// The list of operations to execute.
        /// </summary>
        private readonly List<OperationBase> _operations = new();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="Atomic"/> instance.
        /// this class i auto instantiated.
        /// </summary>
        private Atomic()
        { }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Adds an operation to the atomic execution.
        /// </summary>
        /// <param name="executeAction">The action to execute the operation.</param>
        /// <param name="rollbackAction">The rollback action to execute if failure.</param>
        public Atomic Then(Action executeAction, Action rollbackAction = null)
        {
            _operations.Add(new OperationAction(() => executeAction?.Invoke(), () => rollbackAction?.Invoke()));

            return this;
        }

        /// <summary>
        /// Adds an operation to the atomic execution (async).
        /// </summary>
        /// <param name="executeAction">The action to execute the operation.</param>
        /// <param name="rollbackAction">The rollback action to execute if failure.</param>
        public Atomic ThenAsync(Func<System.Threading.Tasks.Task> executeAction, Func<System.Threading.Tasks.Task> rollbackAction = null)
        {
            _operations.Add(new OperationActionAsync(
                async () => await executeAction(),
                async () =>
                {
                    if (rollbackAction == null) return;
                    await rollbackAction();
                }));

            return this;
        }

        /// <summary>
        /// Adds an operation to the atomic execution.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="executeFunction">The function to execute the operation.</param>
        /// <param name="successCheckFunction">The function to check for operation success.</param>
        /// <param name="rollbackAction">The rollback action to execute if failure.</param>
        /// <returns>This atomic instance.</returns>
        public Atomic Then<T>(Func<T> executeFunction, Func<T, bool> successCheckFunction = null, Action<T> rollbackAction = null)
        {
            _operations.Add(new OperationFunction(() => executeFunction(), obj => successCheckFunction?.Invoke((T)obj) ?? true, obj => rollbackAction?.Invoke((T)obj)));

            return this;
        }

        /// <summary>
        /// Adds an operation to the atomic execution (async).
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="executeFunction">The function to execute the operation.</param>
        /// <param name="successCheckFunction">The function to check for operation success.</param>
        /// <param name="rollbackAction">The rollback action to execute if failure.</param>
        public Atomic ThenAsync<T>(Func<Task<T>> executeFunction, Func<T, bool> successCheckFunction, Func<T, System.Threading.Tasks.Task> rollbackAction = null)
        {
            _operations.Add(new OperationFunctionAsync(
                async () => await executeFunction(),
                obj => successCheckFunction?.Invoke((T)obj) ?? true,
                async obj =>
                {
                    if (rollbackAction == null) return;
                    await rollbackAction.Invoke((T)obj);
                }));

            return this;
        }

        /// <summary>
        /// Execute all the operations and rolled back if an error occured.
        /// </summary>
        /// <returns>The result of the execution/</returns>
        public async Task<ExecutionResult> ExecuteAsync()
        {
            var executedOperations = new List<OperationBase>();
            foreach (var operation in _operations)
            {
                // Inserts the operation at the begin of the list (rollback execution should be reverse).
                executedOperations.Insert(0, operation);

                // Executes the operation
                if (await operation.Execute()) continue;

                // Rollbacks if error
                var rollbackSuccess = true;
                await executedOperations.ForEachAsync(async o => { rollbackSuccess &= await o.Rollback(); });
                return rollbackSuccess ? ExecutionResult.Rollback : ExecutionResult.FailureWhenRollback;
            }

            return ExecutionResult.Success;
        }

        #endregion Methods (Public)
    }
}