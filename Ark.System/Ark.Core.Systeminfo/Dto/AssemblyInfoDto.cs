namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// DTO that represents information about an assembly.
    /// + Useful for auditing binaries associated with network ports.
    /// - Does not verify digital signatures or file integrity.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.reflection.assemblyname"/>
    /// </summary>
    public class AssemblyInfoDto
    {
        #region Properties

        /// <summary>
        /// Process ID using the port.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Assembly path.
        /// </summary>
        public string AssemblyPath { get; set; } = string.Empty;

        /// <summary>
        /// Product name of the assembly.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Version of the assembly.
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
        /// Signature of the assembly.
        /// </summary>
        public string Signature { get; set; } = string.Empty;

        #endregion Properties
    }
}

