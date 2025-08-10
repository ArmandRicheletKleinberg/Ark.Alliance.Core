using System;

namespace Ark.Infrastructure.Info;

/// <summary>
/// Lightweight summary of a Windows service.
/// + Includes status and account information for quick diagnostics.
/// - Omits resource metrics to minimize overhead.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.serviceprocess.servicecontroller"/>
/// </summary>
public class ServiceInfoDto
{
    #region Properties

    /// <summary>
    /// Service name.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Account under which the service runs.
    /// </summary>
    public string Account { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the service.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Service status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Service start type.
    /// </summary>
    public string StartType { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the service started.
    /// </summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>
    /// Duration the service has been running.
    /// </summary>
    public TimeSpan Uptime { get; set; }

    /// <summary>
    /// Path of the service executable.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the service can be stopped.
    /// </summary>
    public bool IsCanStop { get; set; }

    /// <summary>
    /// Indicates whether the service can be started.
    /// </summary>
    public bool IsCanStarted { get; set; }

    #endregion Properties
}

