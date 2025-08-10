namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// DTO that represents information about an occupied port.
    /// + Helps map services to listening sockets.
    /// - Does not reflect dynamic port changes after capture.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.ipglobalproperties"/>
    /// </summary>
    public class PortInfoDto
    {
        #region Properties

        /// <summary>
        /// Port number.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Assembly path using the port.
        /// </summary>
        public string AssemblyPath { get; set; } = string.Empty;

        /// <summary>
        /// Product name of the assembly.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Assembly version.
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Author of the assembly.
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Company of the assembly.
        /// </summary>
        public string Company { get; set; } = string.Empty;

        /// <summary>
        /// Digital signature subject if present.
        /// </summary>
        public string Signature { get; set; } = string.Empty;

        /// <summary>
        /// Internal name from assembly metadata.
        /// </summary>
        public string InternalName { get; set; } = string.Empty;

        /// <summary>
        /// Associated service name if available.
        /// </summary>
        public string ServiceName { get; set; } = string.Empty;

        /// <summary>
        /// Associated service display name.
        /// </summary>
        public string ServiceDisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Network protocol used by the port.
        /// </summary>
        public string Protocol { get; set; } = string.Empty;

        /// <summary>
        /// Roles retrieved from the assembly manifest if any.
        /// </summary>
        public string Roles { get; set; } = string.Empty;

        #endregion Properties
    }
}
