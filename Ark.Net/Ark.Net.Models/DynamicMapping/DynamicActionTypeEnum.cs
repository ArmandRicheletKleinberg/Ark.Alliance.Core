namespace Ark.Net.Models
{
    /// <summary>
    /// This enumeration lists all the possible actions to execute when a condition is verified.
    /// </summary>
    public enum DynamicActionTypeEnum
    {
        /// <summary>
        /// No action defined.
        /// </summary>
        None = 0,

        /// <summary>
        /// Set the value of a property of the object.
        /// </summary>
        SetValue = 1,

        /// <summary>
        /// Executes another mapping.
        /// </summary>
        ExecuteMapping = 2
    }
}