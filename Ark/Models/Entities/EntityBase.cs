using System.Diagnostics;
using System.Runtime.Serialization;
#nullable enable

namespace Ark
{
    /// <summary>
    /// + Supplies common identity and timestamp management for entities.
    /// - Assumes integer primary keys.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("Id: {" + nameof(Id) + "}")]
    public abstract class EntityBase : BeingDirtyBase, IEntity
    {
#if DEBUG_MODEL
            public Guid InstanceId = Guid.NewGuid();
#endif

        private bool _hasIdentity;
        private int _id;
        private System.Guid _key;
        private DateTime _createDate;
        private DateTime _updateDate;

        /// <inheritdoc />
        [DataMember]
        public int Id
        {
            get => _id;
            set
            {
                SetPropertyValueAndDetectChanges(value, ref _id, nameof(Id));
                _hasIdentity = value != 0;
            }
        }

        /// <inheritdoc />
        [DataMember]
        public System.Guid Key
        {
            get
            {
                // if an entity does NOT have a key yet, assign one now
                if (_key == System.Guid.Empty)
                {
                    _key = System.Guid.NewGuid();
                }

                return _key;
            }
            set => SetPropertyValueAndDetectChanges(value, ref _key, nameof(Key));
        }

        /// <inheritdoc />
        [DataMember]
        public DateTime CreateDate
        {
            get => _createDate;
            set => SetPropertyValueAndDetectChanges(value, ref _createDate, nameof(CreateDate));
        }

        /// <inheritdoc />
        [DataMember]
        public DateTime UpdateDate
        {
            get => _updateDate;
            set => SetPropertyValueAndDetectChanges(value, ref _updateDate, nameof(UpdateDate));
        }

        /// <inheritdoc />
        [DataMember]
        public DateTime? DeleteDate { get; set; } // no change tracking - not persisted

        /// <inheritdoc />
        [DataMember]
        public virtual bool HasIdentity => _hasIdentity;

        /// <summary>
        ///     Resets the entity identity.
        /// </summary>
        public virtual void ResetIdentity()
        {
            _id = default;
            _key = System.Guid.Empty;
            _hasIdentity = false;
        }

        /// <summary>
        /// + Determines equality based on identity semantics.
        /// - Returns false when <paramref name="other"/> lacks an identity.
        /// </summary>
        /// <param name="other">Entity to compare with the current instance.</param>
        /// <returns>True if both share the same identity.</returns>
        public virtual bool Equals(EntityBase? other) =>
            other != null && (ReferenceEquals(this, other) || SameIdentityAs(other));

        /// <summary>
        /// + Overrides standard equality to use entity identity.
        /// - Performs a cast which may return null for non-entity objects.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>True if the object is an <see cref="EntityBase"/> with matching identity.</returns>
        public override bool Equals(object? obj) =>
            obj != null && (ReferenceEquals(this, obj) || SameIdentityAs(obj as EntityBase));

        /// <summary>
        /// + Generates a hash code based on identity fields.
        /// - Hash code changes when the entity gains identity.
        /// </summary>
        /// <returns>Hash code for dictionary lookups.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = HasIdentity.GetHashCode();
                hashCode = (hashCode * 397) ^ Id;
                hashCode = (hashCode * 397) ^ GetType().GetHashCode();
                return hashCode;
            }
        }

        private bool SameIdentityAs(EntityBase? other)
        {
            if (other == null)
            {
                return false;
            }

            // same identity if
            // - same object (reference equals)
            // - or same CLR type, both have identities, and they are identical
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return GetType() == other.GetType() && HasIdentity && other.HasIdentity && Id == other.Id;
        }

        /// <summary>
        /// + Creates a deep copy of the entity including cloneable references.
        /// - Expensive for large object graphs.
        /// </summary>
        /// <returns>A new instance with copied values.</returns>
        public object DeepClone()
        {
            // memberwise-clone (ie shallow clone) the entity
            System.Guid unused = Key; // ensure that 'this' has a key, before cloning
            var clone = (EntityBase)MemberwiseClone();

#if DEBUG_MODEL
                clone.InstanceId = Guid.NewGuid();
#endif

            // disable change tracking while we deep clone IDeepCloneable properties
            clone.DisableChangeTracking();

            // deep clone ref properties that are IDeepCloneable
            DeepCloneHelper.DeepCloneRefProperties(this, clone);

            PerformDeepClone(clone);

            // clear changes (ensures the clone has its own dictionaries)
            clone.ResetDirtyProperties(false);

            // re-enable change tracking
            clone.EnableChangeTracking();

            return clone;
        }

        /// <summary>
        ///     Used by inheritors to modify the DeepCloning logic
        /// </summary>
        /// <param name="clone"></param>
        protected virtual void PerformDeepClone(object clone)
        {
        }
    }
}
