using System.Threading.Tasks;
using Ark;
using Ark.App.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Ark.Net.Mqtt.Iot.Emqx5.Diagnostics;

/// <summary>
/// Base class used to access diagnostics reports for MQTT.
/// </summary>
public abstract class Emqx5BkrMttqReportsBase : ReportsBase
{
    #region Methods (Protected)
    /// <summary>
    /// Helper method to ping a topic using the provided repository.
    /// </summary>
    /// <param name="repository">Diagnostics repository.</param>
    /// <param name="topic">Topic to test.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    protected Task<Result> PingTopic(DiagnosticsEmqx5BkrMttqRepository repository, string topic)
        => repository.PingAsync(topic);
    #endregion
}
