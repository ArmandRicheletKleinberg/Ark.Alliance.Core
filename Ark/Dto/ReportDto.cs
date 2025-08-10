namespace Ark
{
    /// <summary>
    /// Represents minimal metadata about a diagnostic report produced by the Ark system.
    /// <para>+ Provides a lightweight wrapper for logging or UI binding.</para>
    /// <para>- Does not include the full report payload.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/logging"/></para>
    /// </summary>
    public class ReportDto
    {
        #region Properties (Public)

        /// <summary>
        /// The unique key of the report, typically the originating method name.
        /// <para>+ Enables correlation with the source generating the report.</para>
        /// <para>- Collisions may occur if method names are not unique.</para>
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Human-readable description of the method.
        /// <para>+ Helps consumers quickly understand report intent.</para>
        /// <para>- Long descriptions increase payload size.</para>
        /// </summary>
        public string Description { get; set; }

        #endregion Properties (Public)
    }
}

