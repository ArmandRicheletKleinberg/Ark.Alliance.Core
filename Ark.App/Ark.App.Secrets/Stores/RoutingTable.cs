using System;
using System.Collections.Generic;

namespace Ark.App.Secrets.Stores
{
    /// <summary>Routing configuration POCO.</summary>
    public sealed class RoutingTable
    {
        #region Properties

        /// <summary>
        /// Maps key prefixes to provider identifiers.
        /// </summary>
        public Dictionary<string, string> PrefixToProvider { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        #endregion
    }
}
