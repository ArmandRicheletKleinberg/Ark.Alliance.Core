namespace Ark;

/// <summary>
/// Defines how list items are duplicated during deep cloning operations.
/// <para>+ Provides control over performance versus isolation.</para>
/// <para>- Incorrect selection may lead to unintended shared references.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone"/></para>
/// </summary>
public enum ListCloneBehavior
{
    /// <summary>
    /// Clones each element only the first time, then reuses references for subsequent clones.
    /// <para>+ Reduces allocations after the initial clone.</para>
    /// <para>- Items added later are not cloned automatically.</para>
    /// </summary>
    CloneOnce,

    /// <summary>
    /// No items are cloned; the resulting list shares all references with the original.
    /// <para>+ Fastest option with minimal memory overhead.</para>
    /// <para>- Mutating items affects both source and copy.</para>
    /// </summary>
    None,

    /// <summary>
    /// Always clones every element, ensuring full separation from the source list.
    /// <para>+ Guarantees isolation between cloned lists.</para>
    /// <para>- Highest allocation and CPU cost.</para>
    /// </summary>
    Always,
}
