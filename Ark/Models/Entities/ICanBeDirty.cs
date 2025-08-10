using System.ComponentModel;

namespace Ark
{
    /// <summary>
    /// + Interface for change-tracking entities.
    /// - Requires manual invocation of notification methods.
    /// </summary>
    public interface ICanBeDirty
    {
        /// <summary>
        /// + Raised when a tracked property changes.
        /// - May fire frequently during bulk updates.
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Determines whether the current entity is dirty.
        /// </summary>
        bool IsDirty();

        /// <summary>
        ///     Determines whether a specific property is dirty.
        /// </summary>
        bool IsPropertyDirty(string propName);

        /// <summary>
        ///     Gets properties that are dirty.
        /// </summary>
        IEnumerable<string> GetDirtyProperties();

        /// <summary>
        ///     Resets dirty properties.
        /// </summary>
        void ResetDirtyProperties();

        /// <summary>
        ///     Disables change tracking.
        /// </summary>
        void DisableChangeTracking();

        /// <summary>
        ///     Enables change tracking.
        /// </summary>
        void EnableChangeTracking();
    }
}
