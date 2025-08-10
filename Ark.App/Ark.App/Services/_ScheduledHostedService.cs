using System;
using System.Threading;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskCompletionSource = System.Threading.Tasks.TaskCompletionSource;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Ark.App
{
    /// <inheritdoc />
    /// <summary>
    /// Extension of <see cref="HostedService"/> that executes on a defined
    /// schedule. The timing information is read from configuration so services
    /// can run periodically without manual timers.
    ///
    /// <para>Example with this class:</para>
    /// <code>
    /// public class ReportService : ScheduledHostedService
    /// {
    ///     protected override Task Execute(CancellationToken token) => DoWork();
    /// }
    /// </code>
    /// The schedule is defined in <c>appsettings.json</c>.
    ///
    /// <para>Without this class</para> you would need custom timer logic and
    /// manual configuration parsing.
    ///
    /// <para>Advantages:</para>
    /// <list type="bullet">
    /// <item>Centralised scheduling mechanism.</item>
    /// <item>Consistent logging and lifecycle management.</item>
    /// </list>
    /// <para>Disadvantages:</para>
    /// <list type="bullet">
    /// <item>Relies on reflection to instantiate and configure services.</item>
    /// </list>
    ///
    /// <para>Performance:</para>
    /// Startup overhead is comparable to <see cref="HostedService"/> with the
    /// additional cost of scheduling timers. For applications with only one or
    /// two background tasks, writing custom timers may be slightly lighter, but
    /// using <c>ScheduledHostedService</c> greatly simplifies maintenance.
    /// </summary>
    /// <example>
    /// Into the app.settings, this section is needed or an exception will be thrown.
    /// { "services" : {
    ///     "[SERVICE_NAME]" = {
    ///         "isEnabled" : true,
    ///         "scheduledStartUtcTime" : "23:00:00",
    ///         "scheduledLaps" : "1.00:00:00",
    ///         "executeWhenStart" : true,
    ///         "data" : {
    ///             "data1" : "..."
    ///             }
    ///         }
    ///     }
    /// }
    /// </example>
    public abstract class ScheduledHostedService : HostedService
    {
        #region Fields

        /// <summary>
        /// A cancellation token source to cancel the wait time of the scheduled service.
        /// </summary>
        protected CancellationTokenSource WaitCts;

        /// <summary>
        /// The schedule to apply for this service execution.
        /// </summary>
        protected ISchedule ScheduleToApply;

        /// <summary>
        /// The scheduled action to execute periodically.
        /// </summary>
        protected ScheduledAction ScheduledAction;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="ScheduledHostedService"/> instance.
        /// </summary>
        /// <param name="configuration">The application configuration. Injected.</param>
        /// <param name="logger">The logger used to log the service executions and errors.</param>
        protected ScheduledHostedService(IConfiguration configuration, ILogger logger = null)
            : base(configuration, logger)
        { }

        #endregion Constructors

        #region Properties (Override)

        /// <summary>
        /// The type of the settings to get.
        /// </summary>
        protected override Type SettingsType => typeof(ScheduledHostedServiceSettings);

        #endregion Properties (Override)

        #region Properties (Protected Helpers)

        /// <summary>
        /// The settings given to this hosted service.
        /// </summary>
        protected new ScheduledHostedServiceSettings Settings =>
            (ScheduledHostedServiceSettings)base.Settings;

        #endregion Properties (Protected Helpers)

        #region Methods (Abstract & Virtual)

        /// <summary>
        /// Gets the schedule to apply for this service to run it.
        /// </summary>
        /// <returns>The schedule to apply.</returns>
        public virtual ISchedule GetSchedule()
            => Schedule.Every(
                Settings.ScheduledStartLocalTime.SpecifyKind(DateTimeKind.Local)
                ?? Settings.ScheduledStartUtcTime.SpecifyKind(DateTimeKind.Utc)
                ?? DateTime.UtcNow, Settings.ScheduledLaps);

        #endregion Methods (Abstract & Virtual)

        #region Methods (Public)

        /// <summary>
        /// Forces an immediate execution of the code.
        /// </summary>
        public override void ForceImmediateExecution()
            => WaitCts?.TryCancel();

        #endregion Methods (Public)

        #region IHostedService

        /// <inheritdoc />
        /// <summary>
        /// Starts the hosted service by executing some code at scheduled times.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the start of the service (not used).</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!Settings.IsEnabled)
                return;

            State.LastExecutionTime = DateTime.UtcNow;
            State.StartTime ??= DateTime.UtcNow;
            LifecycleStatus = HostedServiceLifecycleStatusEnum.Starting;
            ScheduledAction ??= new ScheduledAction(DoExecute, GetSchedule(), Settings.ExecuteWhenStart);
            ExecuteCts = new CancellationTokenSource();
            ExecuteTcs = new TaskCompletionSource();
            var linkCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ExecuteCts.Token);
            await ScheduledAction.Start(linkCts);

            LifecycleStatus = HostedServiceLifecycleStatusEnum.Running;
        }

        /// <inheritdoc />
        /// <summary>
        /// Stops the scheduled hosted service.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the stop of the service (not used).</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            LifecycleStatus = HostedServiceLifecycleStatusEnum.Stopping;
            ExecuteCts.TryCancel();
            await ScheduledAction.Stop(cancellationToken);
            LifecycleStatus = HostedServiceLifecycleStatusEnum.Stopped;
        }

        #endregion IHostedService

        #region Methods (Helpers)

        /// <summary>
        /// Executes the code of the hosted service.
        /// It is called at startup or on-demand.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token used to cancel the execution (coming from StartAsync or None).</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        private async Task DoExecute(CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(ExecuteCts.Token, cancellationToken);
            try
            {
                await Execute(cts.Token);
            }
            catch (TaskCanceledException) { /* Do Nothing */ }
            catch (Exception exception)
            {
                Logger?.LogError($"Unexpected error while executing hosted service {Name} : {exception.Message}{Environment.NewLine}{exception.StackTrace}");
            }
        }

        #endregion Methods (Helpers)
    }

    /// <inheritdoc />
    /// <summary>This overriden generic class allows to use some specific settings data in the service.</summary>
    public abstract class ScheduledHostedService<TData> : ScheduledHostedService
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="ScheduledHostedService"/> instance.
        /// </summary>
        /// <param name="configuration">The application configuration. Injected.</param>
        /// <param name="logger">The logger used to log the service executions and errors.</param>
        protected ScheduledHostedService(IConfiguration configuration, ILogger logger = null)
            : base(configuration, logger)
        { }

        #endregion Constructors

        #region Properties (Override)

        /// <summary>
        /// The type of the settings to get.
        /// </summary>
        protected override Type SettingsType => typeof(ScheduledHostedServiceSettings<TData>);

        #endregion Properties (Override)

        #region Properties (Protected helpers)

        /// <summary>
        /// The settings given to this hosted service.
        /// </summary>
        protected new ScheduledHostedServiceSettings<TData> Settings =>
            (ScheduledHostedServiceSettings<TData>)base.Settings;

        /// <summary>
        /// The data useful for this service.
        /// </summary>
        protected TData Data
            => Settings.Data;

        #endregion Methods (Protected helpers)
    }
}