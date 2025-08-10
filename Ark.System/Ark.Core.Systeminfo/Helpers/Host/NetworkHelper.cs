using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;


namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides methods to retrieve network information such as IP address, occupied ports, and domain name of the machine.
    /// </summary>
    /// <remarks>
    /// The <see cref="NetworkInfoDto"/> class includes methods to:
    /// <list type="bullet">
    /// <item>
    /// <description>Get the local IP address of the machine (<see cref="GetLocalIPAddress"/>).</description>
    /// </item>
    /// <item>
    /// <description>Get the list of currently occupied ports along with their assemblies (<see cref="GetOccupiedPorts"/>).</description>
    /// </item>
    /// <item>
    /// <description>Get the fully qualified domain name (FQDN) of the machine (<see cref="GetFullyQualifiedDomainName"/>).</description>
    /// </item>
    /// <item>
    /// <description>Get detailed network information including IP address and occupied ports (<see cref="GetNetworkInfo"/>).</description>
    /// </item>
    /// </list>
    /// </remarks>
public static class NetworkHelper
{
#region Methods
        /// <summary>
        /// Retrieves the local IP address.
        /// + Scans DNS entries and filters IPv4 and IPv6 addresses.
        /// - Returns only the first matching address.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.dns.gethostentry"/>
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing the IP address of the current machine as a string or an error message.</returns>
        public static Result<string> GetLocalIPAddress()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily.IsOneOf(AddressFamily.InterNetwork, AddressFamily.InterNetworkV6))
                    {
                        return Result<string>.Success.WithData(ip.ToString());
                    }
                }
                return Result<string>.Failure.WithReason("Local IP Address Not Found!");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure.AddReason($"An error occurred while retrieving the local IP address: {ex.Message}");
            }
        }

        /// <summary>
        /// Enumerates ports currently in use and their owning assemblies.
        /// + Merges TCP and UDP listeners for a unified view.
        /// - Requires `netstat` and may need elevated privileges.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.ipglobalproperties.getactivetcplisteners"/>
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing a list of <see cref="PortInfoDto"/> representing the occupied ports and their assemblies or an error message.</returns>
        public static Result<List<PortInfoDto>> GetOccupiedPorts()
        {
            try
            {
                IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
                IPEndPoint[] udpEndPoints = properties.GetActiveUdpListeners();

                List<PortInfoDto> occupiedPorts = [];

                foreach (IPEndPoint? endpoint in tcpEndPoints.Concat(udpEndPoints))
                {
                    PortInfoDto portInfo = new PortInfoDto
                    {
                        Port = endpoint.Port
                    };

                    Result<AssemblyInfoDto> assemblyInfoResult = GetAssemblyInfoUsingPort(endpoint.Port);
                    if (assemblyInfoResult.IsSuccess)
                    {
                        AssemblyInfoDto assemblyInfo = assemblyInfoResult.Data;
                        portInfo.AssemblyPath = assemblyInfo.AssemblyPath;
                        portInfo.ProductName = assemblyInfo.ProductName;
                        portInfo.Version = assemblyInfo.Version;
                        portInfo.Author = assemblyInfo.Author;
                        portInfo.Company = assemblyInfo.Company;
                        portInfo.Signature = assemblyInfo.Signature;
                    }

                    occupiedPorts.Add(portInfo);
                }

                return Result<List<PortInfoDto>>.Success.WithData(occupiedPorts.Distinct().ToList());
            }
            catch (Exception ex)
            {
                return Result<List<PortInfoDto>>.Failure.AddReason($"An error occurred while retrieving the occupied ports: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the process information using a specific port and retrieves its assembly information.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <returns>A <see cref="Result{T}"/> containing an <see cref="AssemblyInfoDto"/> with the assembly details or an error message.</returns>
        private static Result<AssemblyInfoDto> GetAssemblyInfoUsingPort(int port)
        {
            try
            {

                var netstatOutput = ExecuteNetstatCommand(port);

                if (netstatOutput.IsNullOrEmpty())
                {
                    return Result<AssemblyInfoDto>.Failure.WithReason("No process found using the specified port.");
                }

                // Extract the Process ID from the netstat output
                var processId = ExtractProcessIdFromNetstatOutput(netstatOutput);
                if (processId == -1)
                {
                    return Result<AssemblyInfoDto>.Failure.WithReason($"Failed to find a valid process ID for port {port}.");
                }

                try
                {
                    var process = Process.GetProcessById(processId);
                    var mainModule = process.MainModule;
                    var fileName = mainModule?.FileName;

                    var assemblyInfo = new AssemblyInfoDto
                    {
                        ProcessId = processId,
                        AssemblyPath = fileName ?? string.Empty,
                        ProductName = GetAssemblyAttribute<AssemblyProductAttribute>(mainModule)?.Product ?? string.Empty,
                          Version = mainModule?.FileVersionInfo.FileVersion ?? string.Empty,
                        Author = GetAssemblyAttribute<AssemblyCompanyAttribute>(mainModule)?.Company ?? string.Empty,
                        Company = GetAssemblyAttribute<AssemblyCompanyAttribute>(mainModule)?.Company ?? string.Empty,
                          Signature = mainModule?.FileName is { } file ? GetAssemblySignature(file) ?? string.Empty : string.Empty
                      };


                    return Result<AssemblyInfoDto>.Success.WithData(assemblyInfo);
                }
                catch (Exception ex)
                {
                    return Result<AssemblyInfoDto>.Failure.AddReason($"An error occurred while retrieving the assembly info for port {port}: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return Result<AssemblyInfoDto>.Failure.AddReason($"An error occurred while searching for processes using port {port}: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a specific attribute from an assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute to retrieve.</typeparam>
        /// <param name="module">The module to get the attribute from.</param>
        /// <returns>The attribute of type <typeparamref name="T"/>, if found; otherwise, null.</returns>
        private static T? GetAssemblyAttribute<T>(ProcessModule? module) where T : Attribute
        {
            try
            {
                if (module == null) return null;
                Assembly assembly = Assembly.LoadFile(module.FileName);
                return (T?)assembly.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the signature of an assembly file.
        /// </summary>
        /// <param name="filePath">The path to the assembly file.</param>
        /// <returns>The signature of the assembly file, if found; otherwise, null.</returns>
        private static string GetAssemblySignature(string filePath)
        {
            try
            {
                X509Certificate2 cert = X509CertificateLoader.LoadCertificateFromFile(filePath);
                return cert.Subject;
            }
            catch
            {
                return "No Certificate Found";
            }
        }



        /// <summary>
        /// Bundles local IP and active ports into a single structure.
        /// + Reuses existing helpers for consolidated output.
        /// - Fails fast if either query fails.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.ipglobalproperties"/>
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing a <see cref="NetworkInfoDto"/> with the IP address and a list of occupied ports or an error message.</returns>
        public static Result<NetworkInfoDto> GetNetworkInfo()
        {
            Result<string> ipAddressResult = GetLocalIPAddress();
            if (!ipAddressResult.IsSuccess)
            {
                return Result<NetworkInfoDto>.Failure.AddReason(ipAddressResult.Reason);
            }

            Result<List<PortInfoDto>> occupiedPortsResult = GetOccupiedPorts();
            if (!occupiedPortsResult.IsSuccess)
            {
                return Result<NetworkInfoDto>.Failure.AddReason(occupiedPortsResult.Reason);
            }

            NetworkInfoDto networkInfo = new NetworkInfoDto
            {
                IPAddress = ipAddressResult.Data,
                OccupiedPorts = occupiedPortsResult.Data
            };

            return Result<NetworkInfoDto>.Success.WithData(networkInfo);
        }

        /// <summary>
        /// Resolves the machine's fully qualified domain name (FQDN).
        /// + Uses <see cref="IPGlobalProperties"/> to append the domain when available.
        /// - Falls back to the host name if no domain is configured.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.ipglobalproperties.domainname"/>
        /// </summary>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the fully qualified domain name of the current machine or an error message.
        /// If the machine is not part of a domain, the method returns only the machine name.
        /// </returns>
        public static Result<string> GetFullyQualifiedDomainName()
        {
            try
            {
                string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
                string machineName = Dns.GetHostName();

                if (!domainName.IsNullOrEmpty())
                    return Result<string>.Success.WithData($"{machineName}.{domainName}");

                return Result<string>.Success.WithData(machineName);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure.AddReason($"An error occurred while retrieving the fully qualified domain name: {ex.Message}");
            }
        }


        /// <summary>
        /// Extracts the process ID from the netstat output.
        /// </summary>
        /// <param name="netstatOutput">The netstat command output.</param>
        /// <returns>The Process ID if found, or -1 if not found.</returns>
        private static int ExtractProcessIdFromNetstatOutput(string netstatOutput)
        {
            // Regex to find the process ID at the end of the netstat line
            var regex = new Regex(@"\s+(\d+)$");
            var match = regex.Match(netstatOutput);

            if (match.Success && int.TryParse(match.Groups[1].Value, out int processId))
            {
                return processId;
            }

            return -1;
        }

        /// <summary>
        /// Executes the netstat command to retrieve information about the process using the specified port.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <returns>The output of the netstat command.</returns>
        private static string ExecuteNetstatCommand(int port)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "netstat";
                    process.StartInfo.Arguments = $"-a -n -o | findstr :{port}";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    return output;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
#endregion Methods
    }
}



