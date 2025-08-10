namespace Ark
{
    /// <summary>
    ///     Provides a mean to deep-clone an object.
    /// </summary>
    public interface IDeepCloneable
    {
        /// <summary>
        /// + Creates a new instance copying all nested data.
        /// - Implementations should handle reference cycles carefully.
        /// </summary>
        /// <returns>The cloned object.</returns>
        object DeepClone();
    }
}
