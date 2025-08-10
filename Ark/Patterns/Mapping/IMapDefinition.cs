namespace Ark
{
    /// <summary>
    ///     Defines maps for <see cref="IArkMapper" />.
    /// </summary>
    public interface IMapDefinition
    {
        /// <summary>
        ///     Defines maps.
        /// </summary>
        void DefineMaps(IArkMapper mapper);
    }
}
