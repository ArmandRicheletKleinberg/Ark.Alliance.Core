using System;
using Ark.Net.Models;

namespace Ark.Net.CrossCutting
{
    /// <summary>
    /// These are the dynamic mapping data to cache that is the configuration and the compiled method.
    /// </summary>
    internal class DynamicMappingCache
    {
        #region Properties (Public)

        /// <summary>
        /// The configuration of the dynamic mapping.
        /// </summary>
        public DynamicMappingDto Config { get; set; }

        /// <summary>
        /// The compiled method.
        /// This is compiled on first execution of the dynamic mapping.
        /// </summary>
        public Action<object> CompiledMethod { get; set; }

        #endregion Properties (Public)
    }
}