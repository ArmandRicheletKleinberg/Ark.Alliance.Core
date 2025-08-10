namespace Ark;

#nullable enable

/// <summary>
/// Represents a case-insensitive composite dictionary key composed of an integer and a string.
/// <para>+ Enables fast lookups with minimal allocations.</para>
/// <para>- Integer part must be non-negative.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2"/></para>
/// </summary>
/// <example>
    /// <code language="csharp">
    /// var key = new CompositeIntStringKey(1, "btc");
    /// var dict = new Dictionary&lt;CompositeIntStringKey, int&gt;();
    /// dict[key] = 42;
    /// </code>
/// </example>
public struct CompositeIntStringKey : IEquatable<CompositeIntStringKey>
{
    #region Fields
    private readonly int _key1;
    private readonly string _key2;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeIntStringKey"/> struct.
    /// <para>+ Normalizes null components to sentinel values.</para>
    /// <para>- Throws when <paramref name="key1"/> is negative.</para>
    /// </summary>
    /// <param name="key1">Non-negative integer component. Use <c>null</c> for a sentinel value of -1.</param>
    /// <param name="key2">String component compared case-insensitively. <c>null</c> becomes "NULL".</param>
    public CompositeIntStringKey(int? key1, string? key2)
    {
        if (key1 < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key1));
        }

        _key1 = key1 ?? -1;
        _key2 = key2?.ToLowerInvariant() ?? "NULL";
    }
    #endregion Constructors

    #region Operators
    /// <summary>
    /// Determines whether two keys are equal.
    /// </summary>
    /// <param name="left">First key to compare.</param>
    /// <param name="right">Second key to compare.</param>
    /// <returns><c>true</c> if both keys are equal; otherwise <c>false</c>.</returns>
    public static bool operator ==(CompositeIntStringKey left, CompositeIntStringKey right)
        => left._key2 == right._key2 && left._key1 == right._key1;

    /// <summary>
    /// Determines whether two keys are not equal.
    /// </summary>
    /// <param name="left">First key to compare.</param>
    /// <param name="right">Second key to compare.</param>
    /// <returns><c>true</c> if the keys differ; otherwise <c>false</c>.</returns>
    public static bool operator !=(CompositeIntStringKey left, CompositeIntStringKey right)
        => left._key2 != right._key2 || left._key1 != right._key1;
    #endregion Operators

    #region Equality
    /// <summary>
    /// Indicates whether the current key is equal to another key.
    /// </summary>
    /// <param name="other">The key to compare with.</param>
    /// <returns><c>true</c> if the keys are equal; otherwise <c>false</c>.</returns>
    public bool Equals(CompositeIntStringKey other)
        => _key2 == other._key2 && _key1 == other._key1;

    /// <summary>
    /// Determines whether the specified object is equal to the current key.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is <see cref="CompositeIntStringKey"/> and equals this instance; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj)
        => obj is CompositeIntStringKey other && _key2 == other._key2 && _key1 == other._key1;

    /// <summary>
    /// Returns a hash code for the current key.
    /// </summary>
    /// <returns>Combined hash code of its components.</returns>
    public override int GetHashCode()
        => (_key2.GetHashCode() * 31) + _key1;
    #endregion Equality
}
