using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// A transaction item is a manipulation on a single or multiple entities.
    /// </summary>
    internal class DbTransactionBuilderItem
    {
        #region Properties (Public)

        /// <summary>
        /// The type of manipulation to execute on the entities.
        /// </summary>
        public EntityState State { get; set; }

        /// <summary>
        /// For update only, the properties to update. The other properties are ignored and thus not updated.
        /// </summary>
        public string[] UpdateOnlyTheseProperties { get; set; }

        /// <summary>
        /// The entities to manipulate.
        /// If it is a single entity then the more fast method will be used.
        /// </summary>
        public object[] Entities { get; set; }

        #endregion Properties (Public)
    }
}