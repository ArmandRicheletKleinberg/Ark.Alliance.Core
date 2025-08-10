using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Runtime.Versioning;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Maps raw service information to <see cref="DetailedServiceInfoDto"/> objects.
    /// </summary>
    public static class ServiceInfoMapper
    {
        #region Methods (Public)

        /// <summary>
        /// Builds a <see cref="DetailedServiceInfoDto"/> from service metrics.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static DetailedServiceInfoDto Map(ServiceController svc, string account,
            DateTime? lastStart, DateTime? lastStop,
            double cpu, double cpuProp, double mem, double memProp,
            List<EventLogDto> events,
            string version, string signature, string publisher, DateTime? exeDate)
            => new()
            {
                ServiceName = svc.ServiceName,
                DisplayName = svc.DisplayName,
                Status = svc.Status.ToString(),
                StartType = svc.StartType.ToString(),
                Account = account,
                LastStartTime = lastStart,
                LastStopTime = lastStop,
                CpuUsage = cpu,
                CpuProportion = cpuProp,
                MemoryUsage = mem,
                MemoryProportion = memProp,
                DiskUsage = 0,
                DiskProportion = 0,
                NetworkUsage = 0,
                NetworkProportion = 0,
                Events = events,
                Version = version,
                Signature = signature,
                Publisher = publisher,
                ExecutableDate = exeDate
            };

        #endregion Methods (Public)
    }
}
