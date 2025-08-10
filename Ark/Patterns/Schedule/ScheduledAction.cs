// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Ark
{
    /// <summary>
    /// This helper class allowed to manage some scheduled tasks (repetitive or one shot).
    /// </summary>
    public class ScheduledAction
    {
        #region Fields

        /// <summary>
        /// The action to execute with a <see cref="CancellationTokenSource"/> to cancel it.
        /// </summary>
        private readonly Func<CancellationToken, System.Threading.Tasks.Task> _action;

        /// <summary>
        /// The schedule used to execute periodically the action.
        /// </summary>
        private readonly ISchedule _schedule;

        /// <summary>
        /// Whether the action should be executed when starting the <see cref="ScheduledAction"/>.
        /// </summary>
        private readonly bool _executeWhenStarting;

        /// <summary>
        /// The cancellation token source used to cancel the action periodic execution.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// The cancellation token source used during wait in order to be able to force an immediate execution.
        /// </summary>
        private CancellationTokenSource _waitTcs;

        /// <summary>
        /// The Task completion source to wait until the end of the action execution.
        /// </summary>
        private TaskCompletionSource _taskCompletionSource;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="ScheduledAction"/> instance.
        /// </summary>
        /// <param name="action">The action to execute with a <see cref="CancellationTokenSource"/> to cancel it.</param>
        /// <param name="schedule">The schedule used to execute periodically the action.</param>
        /// <param name="executeWhenStarting">Whether the action should be executed when starting the <see cref="ScheduledAction"/>.</param>
        public ScheduledAction(Action<CancellationToken> action, ISchedule schedule, bool executeWhenStarting = false)
        {
            _action = cts => System.Threading.Tasks.Task.Run(() => action(cts));
            _schedule = schedule;
            _executeWhenStarting = executeWhenStarting;
        }

        /// <summary>
        /// Creates a <see cref="ScheduledAction"/> instance.
        /// </summary>
        /// <param name="action">The asynchronous action to execute with a <see cref="CancellationTokenSource"/> to cancel it.</param>
        /// <param name="schedule">The schedule used to execute periodically the action.</param>
        /// <param name="executeWhenStarting">Whether the action should be executed when starting the <see cref="ScheduledAction"/>.</param>
        public ScheduledAction(Func<CancellationToken, System.Threading.Tasks.Task> action, ISchedule schedule, bool executeWhenStarting = false)
        {
            _action = action;
            _schedule = schedule;
            _executeWhenStarting = executeWhenStarting;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// Whether the scheduled periodic action execution is started.
        /// </summary>
        public bool IsStarted
            => _cancellationTokenSource != null;

        #endregion Properties (Public)

        #region Methods (Start/Stop)

        /// <summary>
        /// Start the periodic execution of the action.
        /// It can be stopped either by cancelling the <see cref="CancellationTokenSource"/> passed by action arguments or by calling explicitly <see cref="Stop"/> method.
        /// Asynchronous because waits for the first action execution if needed.
        /// The other periodic actions are not awaited.
        /// </summary>
        /// <param name="cancellationTokenSource">The cancellation token source to cancel the action periodic execution.</param>
        public async System.Threading.Tasks.Task Start(CancellationTokenSource cancellationTokenSource = null)
        {
            if (_cancellationTokenSource != null)
                return;

            _cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
            _taskCompletionSource = new TaskCompletionSource();

            if (_executeWhenStarting && !_cancellationTokenSource.IsCancellationRequested)
                await _action(_cancellationTokenSource.Token);

            System.Threading.Tasks.Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        var laps = _schedule.GetNextScheduleLaps();
                        if (!laps.HasValue)
                            break;

                        _waitTcs = new CancellationTokenSource();
                        try
                        {
                            using var linkCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, _waitTcs.Token);
                            if (laps.Value.Ticks > 0)
                                await System.Threading.Tasks.Task.Delay(laps.Value, linkCts.Token);

                            if (_cancellationTokenSource.IsCancellationRequested)
                                break;

                            await _action(_cancellationTokenSource.Token);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }
                    catch (TaskCanceledException) { /* Do nothing */ }
                }

                _taskCompletionSource.TrySetResult();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

            }, _cancellationTokenSource.Token).DoNotAwait();
        }

        /// <summary>
        /// Stops the periodic execution of the action.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the wait until the action is executed and to returns directly.</param>
        public async System.Threading.Tasks.Task Stop(CancellationToken cancellationToken = default)
        {
            if (_cancellationTokenSource == null)
                return;

            _cancellationTokenSource.TryCancel();
            await System.Threading.Tasks.Task.Run(async () => await _taskCompletionSource.Task, cancellationToken);

            _cancellationTokenSource = null;
        }

        #endregion Methods (Start/Stop)

        #region Methods (ForceImmediateExecution)

        /// <summary>
        /// Forces an immediate execution by stopping the active wait between method execution.
        /// </summary>
        public void ForceImmediateExecution()
            => _waitTcs.TryCancel();

        #endregion Methods (ForceImmediateExecution)
    }
}