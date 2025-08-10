using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable PossibleNullReferenceException

namespace Ark.Data
{
    /// <summary>
    /// Executes PowerShell scripts located on disk.
    /// + Wraps <see href="https://learn.microsoft.com/powershell/scripting/overview"/> to simplify script invocation.
    /// - Depends on the availability of <c>powershell.exe</c> on the host.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.process"/>
    /// </summary>
    public class PowershellFileRepository
    {
        #region Methods (Public)

        /// <summary>
        /// Executes a PowerShell script with optional parameters.
        /// + Uses <see cref="ProcessStartInfo"/> to spawn a new process.
        /// - Arguments are passed verbatim and may require manual quoting.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.processstartinfo"/>
        /// </summary>
        /// <param name="filePath">Absolute script file path (will be surrounded by ").</param>
        /// <param name="parameters">Parameters forwarded to the script; surround with " if they contain spaces.</param>
        /// <returns>
        /// Success : The process exit code when execution completes.
        /// NotFound : Returned when the script file is missing.
        /// Unexpected : An unexpected error occurs.
        /// Example JSON: { "isSuccess": true, "value": 0 }
        /// </returns>
        public virtual Task<Result<int>> Execute(string filePath, params string[] parameters)
            => Task.Run(() => Result<int>.SafeExecute(() =>
            {
                if (!File.Exists(filePath))
                    return Result<int>.NotFound.WithReason($"The file {filePath} does not exist.");

                var args = $"-ExecutionPolicy Bypass -File \"{filePath}\" {string.Join(" ", parameters)}";
                var processInfo = new ProcessStartInfo("powershell.exe", args)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    return new Result<int>(process.ExitCode);
                }
            }));

        #endregion Methods (Public)
    }
}