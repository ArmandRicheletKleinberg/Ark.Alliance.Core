using System;

namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO contains the needed information to execute a dynamic mapping on an object.
    /// The object must be in simple flat object instance.
    /// </summary>
    public class DynamicMappingDto
    {
        #region Properties (Public)

        /// <summary>
        /// The identifier of the dynamic mapping.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name identifying uniquely the dynamic mapping.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A small description about the purpose of this dynamic mapping.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The last version of the dynamic mapping.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Creation date of the version.
        /// </summary>
        public DateTime LastChangeDate { get; set; }

        /// <summary>
        /// The user that has changed lastly the dynamic mapping.
        /// </summary>
        public string LastChangeUser { get; set; }

        /// <summary>
        /// What was changed in this version.
        /// </summary>
        public string LastChangeRemark { get; set; }

        /// <summary>
        /// The root condition that owns all the sub conditions of the mapping.
        /// </summary>
        public DynamicConditionDto RootCondition { get; set; }

        #endregion Properties (Public)
    }
}
