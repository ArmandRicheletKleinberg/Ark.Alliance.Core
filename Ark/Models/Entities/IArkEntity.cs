namespace Ark
{
    /// <summary>
    ///     Represents an entity that can be managed by the entity service.
    /// </summary>
    /// <remarks>
    ///     <para>An IArkEntity can be related to another via the IRelationService.</para>
    ///     <para>IArkEntities can be retrieved with the IEntityService.</para>
    ///     <para>An IArkEntity can participate in notifications.</para>
    /// </remarks>
    public interface IArkEntity : ITreeEntity
    {
    }
}
