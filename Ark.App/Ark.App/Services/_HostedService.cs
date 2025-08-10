using System;
using System.Threading;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Ark.App
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for implementing background services whose behaviour can be
    /// configured from <c>appsettings.json</c>. Any class derived from
    /// <see cref="HostedService"/> is discovered and started automatically by
    /// <c>UseHostedService</c> extension methods.
    ///
    /// <para>Example with this class:</para>
    /// <code>
    /// public class CleanupService : HostedService
    /// {
    ///     protected override Task Execute(CancellationToken token)
    ///     {
    ///         // cleanup logic
    ///         return Task.CompletedTask;
    ///     }
    /// }
    /// </code>
    /// The service executes according to the configured schedule.
    ///
    /// <para>Example without this class:</para>
    /// <code>
    /// IHostedService svc = new BackgroundServiceImplementation();
    /// // manual registration and scheduling required
    /// </code>
    ///
    /// <para>Advantages:</para>
    /// <list type="bullet">
    /// <item>Standardises logging and state management for all services.</item>
    /// <item>Configuration-driven behaviour simplifies deployment.</item>
    /// </list>
    /// <para>Disadvantages:</para>
    /// <list type="bullet">
    /// <item>Reflection is required to instantiate services automatically.</item>
    /// <item>Slightly higher memory footprint due to state tracking.</item>
    /// </list>
    ///
    /// <para>Performance:</para>
    /// Overhead is negligible for most applications. Startup time increases by a
    /// few milliseconds because the configuration must be parsed. Without using
    /// <see cref="HostedService"/>, developers must handle the scheduling logic
    /// themselves, which can be more error prone but avoids the reflection cost.
    /// Alternative: implement <see cref="IHostedService"/> directly and register
    /// it manually when precise control over startup time is required.
    /// </summary>
    /// <example>
    /// Into the app.settings, this section is needed or an exception will be thrown.
    /// { "services" : {
    ///     "[SERVICE_NAME]" = {
    ///         "isEnabled" : true,
    ///         "executeWhenStart" : true,
    ///         "data" : {
    ///             "data1" : "..."
    ///             }
    ///         }
    ///     }
    /// }
    /// </example>
    public abstract class HostedService : IHostedService
    {
        #region Fields

        /// <summary>
        /// A cancellation token source to cancel the execution of the scheduled service.
        /// </summary>
        protected CancellationTokenSource ExecuteCts;

        /// <summary>
        /// A task completion source to be able to wait until the execution finishes event if cancelled.
        /// </summary>
        protected TaskCompletionSource ExecuteTcs;

        /// <summary>
        /// A TaskCompletionSource to wait until the service lifecycle status changes.
        /// </summary>
        protected TaskCompletionSource<HostedServiceLifecycleStatusEnum> LifecycleStatusTcs;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="HostedService"/> instance.
        /// </summary>
        /// <param name="configuration">The application configuration. Injected.</param>
        /// <param name="logger">The logger used to log the service executions and errors.</param>
        protected HostedService(IConfiguration configuration, ILogger logger = null)
        {
            Logger ??= logger ?? new NullLogger<HostedService>();
            Name ??= GetType().Name.Replace("Service", "");

            Settings ??= configuration.GetSection($"Services:{Name}").Get(SettingsType) as HostedServiceSettings;
            if (Settings == null)
                throw new Exception($"Unable to start the hosted service {GetType()} because the app settings section Services.{Name} does not exist.");

            State = new HostedServiceState { Settings = Settings };
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The state information about this hosted service.
        /// </summary>
        public HostedServiceState State { get; }

        /// <summary>
        /// Whether the hosted service is running or not.
        /// </summary>
        public bool IsRunning
            => State.LifecycleStatus == HostedServiceLifecycleStatusEnum.Running;

        #endregion Properties (Public)

        #region Methods (Abstract & Virtual)

        /// <summary>
        /// Executes a Task.
        /// Either executed on application start or on-demand.
        /// </summary>
        /// <returns>Asynchronous so must return a Task.</returns>
        protected abstract Task Execute(CancellationToken cancellationToken);

        /// <summary>
        /// The logger used to log the hosted service execution process.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// The settings given to this hosted service.
        /// </summary>
        protected virtual HostedServiceSettings Settings { get; private set; }

        /// <summary>
        /// The name of the service to search for its configuration in the app.settings.
        /// If not overriden takes the Type name and removes the Service suffix.
        /// </summary>
        protected virtual string Name { get; }

        /// <summary>
        /// The type of the settings to get.
        /// </summary>
        protected virtual Type SettingsType => typeof(HostedServiceSettings);

        /// <summary>
        /// The service lifecycle status.
        /// </summary>
        protected virtual HostedServiceLifecycleStatusEnum LifecycleStatus
        {
            get => State.LifecycleStatus;
            set
            {
                if (State.LifecycleStatus != value)
                    Logger?.Log(LogLevel.Information, $"Service {GetType()} {value} : {DateTime.UtcNow}");

                State.LifecycleStatus = value;
                LifecycleStatusTcs?.TrySetResult(value);
            }
        }

        #endregion Methods (Abstract & Virtual)

        #region Methods (Public)

        /// <summary>
        /// Forces an immediate execution of the code.
        /// </summary>
        public virtual void ForceImmediateExecution()
        {
            Logger.Log(LogLevel.Information, $"The service {Name} has been forced to be executed.");
            DoExecute(CancellationToken.None).DoNotAwait();
        }

        /// <summary>
        /// Manually restarts a started service.
        /// It simply call the Stop and Start methods in sequence.
        /// </summary>
        /// <returns>Asynchronous so must return a Task.</returns>
        public async Task Restart()
        {
            Logger.Log(LogLevel.Information, $"The service {Name} is begin manually restarted.");
            await StopAsync(CancellationToken.None);
            await StartAsync(CancellationToken.None);
        }

        #endregion Methods (Public)

        #region Methods (IHostedService)

        /// <inheritdoc />
        public virtual Task StartAsync(CancellationToken cancellationToken)
            => Settings.ExecuteWhenStart ? DoExecute(cancellationToken) : Task.CompletedTask;

        /// <inheritdoc />
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            LifecycleStatus = HostedServiceLifecycleStatusEnum.Stopping;
            ExecuteCts?.TryCancel();
            if (ExecuteTcs != null)
                await ExecuteTcs.Task;

            LifecycleStatus = HostedServiceLifecycleStatusEnum.Stopped;
        }

        #endregion Methods (IHostedService)

        #region Methods (Helpers)

        /// <summary>
        /// Executes the code of the hosted service.
        /// It is called at startup or on-demand.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token used to cancel the execution (coming from StartAsync or None).</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        private async Task DoExecute(CancellationToken cancellationToken)
        {
            if (!Settings.IsEnabled)
                return;

            State.LastExecutionTime = DateTime.UtcNow;
            State.StartTime ??= DateTime.UtcNow;
            State.LifecycleStatus = HostedServiceLifecycleStatusEnum.Running;
            ExecuteCts = new CancellationTokenSource();
            ExecuteTcs = new TaskCompletionSource();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ExecuteCts.Token, cancellationToken);
            try
            {
                await Execute(cts.Token);
            }
            catch (TaskCanceledException) { /* Do Nothing */ }
            catch (Exception exception)
            {
                Logger?.Log(LogLevel.Error, $"Unexpected error while executing hosted service {Name} : {exception.Message}{Environment.NewLine}{exception.StackTrace}");
            }
            LifecycleStatus = HostedServiceLifecycleStatusEnum.Stopped;
            ExecuteTcs.TrySetResult();
        }

        #endregion Methods (Helpers)
    }

    /// <inheritdoc />
    /// <summary>This overriden generic class allows to use some specific settings data in the service.
    /// </summary>
    public abstract class HostedService<TData> : HostedService
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="HostedService"/> instance.
        /// </summary>
        /// <param name="configuration">The application configuration. Injected.</param>
        /// <param name="logger">The logger used to log the service executions and errors.</param>
        protected HostedService(IConfiguration configuration, ILogger logger = null)
            : base(configuration, logger)
        { }

        #endregion Constructors

        #region Properties (Override)

        /// <summary>
        /// The type of the settings to get.
        /// </summary>
        protected override Type SettingsType => typeof(HostedServiceSettings<TData>);

        #endregion Properties (Override)

        #region Properties (Protected helpers)

        /// <summary>
        /// The settings given to this hosted service.
        /// </summary>
        protected new HostedServiceSettings<TData> Settings =>
            (HostedServiceSettings<TData>)base.Settings;

        /// <summary>
        /// The data useful for this service.
        /// </summary>
        protected TData Data
            => Settings.Data;

        #endregion Methods (Protected helpers)
    }
}