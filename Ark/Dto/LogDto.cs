using Microsoft.Extensions.Logging;

#nullable enable

namespace Ark;

/// <summary>
/// Represents a single log entry that can be processed by an <see cref="ILogger"/> pipeline.
/// <para>+ Provides structured data for diagnostics and storage.</para>
/// <para>- Excessive logging may impact application performance.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/logging"/></para>
/// </summary>
public class LogDto
{
    #region Properties (Public)

    /// <summary>
    /// The severity level of the log.
    /// <para>+ Enables filtering by <see cref="LogSeverityEnum"/>.</para>
    /// <para>- Misclassified levels can hide important issues.</para>
    /// </summary>
    public LogSeverityEnum Severity { get; set; }

    /// <summary>
    /// The time when the log was created.
    /// <para>Format: <c>yyyy-MM-ddTHH:mm:ssZ</c>.</para>
    /// <para>- Incorrect time zones can complicate analysis.</para>
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// The category of the log.
    /// <para>+ Groups entries by subsystem or feature.</para>
    /// <para>- Inconsistent categories hinder searchability.</para>
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The inner details of the logs.
    /// <para>+ Supplies context such as messages or stack traces.</para>
    /// <para>- May expose sensitive information if not sanitized.</para>
    /// </summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// Exception details associated with the log entry.
    /// <para>+ Facilitates troubleshooting with full stack traces.</para>
    /// <para>- May expose sensitive data; sanitize before persisting.</para>
    /// </summary>
    public string? Exception { get; set; }

    #endregion Properties (Public)
}

