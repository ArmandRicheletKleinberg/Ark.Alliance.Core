namespace Ark
{
    /// <summary>
    /// + Describes a persistence-aware domain entity.
    /// - Imposes identity and lifecycle properties on implementers.
    /// </summary>
    public interface IEntity : IDeepCloneable
    {
        /// <summary>
        ///     Gets or sets the integer identifier of the entity.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        ///     Gets or sets the Guid unique identifier of the entity.
        /// </summary>
        System.Guid Key { get; set; }

        /// <summary>
        ///     Gets or sets the creation date.
        /// </summary>
        DateTime CreateDate { get; set; }

        /// <summary>
        ///     Gets or sets the last update date.
        /// </summary>
        DateTime UpdateDate { get; set; }

        /// <summary>
        ///     Gets or sets the delete date.
        /// </summary>
        /// <remarks>
        ///     <para>The delete date is null when the entity has not been deleted.</para>
        ///     <para>
        ///         The delete date has a value when the entity instance has been deleted, but this value
        ///         is transient and not persisted in database (since the entity does not exist anymore).
        ///     </para>
        /// </remarks>
        DateTime? DeleteDate { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the entity has an identity.
        /// </summary>
        bool HasIdentity { get; }

        /// <summary>
        /// + Clears the identifier and key, marking the entity as transient.
        /// - Should only be used on entities not yet persisted.
        /// </summary>
        void ResetIdentity();
    }
}
