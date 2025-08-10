namespace Ark;

#nullable enable

/// <summary>
/// Represents a case-insensitive composite dictionary key composed of two strings.
/// <para>+ Simplifies lookups using combined identifiers.</para>
/// <para>- Neither component may be <c>null</c>.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.string"/></para>
/// </summary>
/// <example>
    /// <code language="csharp">
    /// var key = new CompositeStringStringKey("eth", "usdt");
    /// var dict = new Dictionary&lt;CompositeStringStringKey, int&gt;();
    /// dict[key] = 1;
    /// </code>
/// </example>
public struct CompositeStringStringKey : IEquatable<CompositeStringStringKey>
{
    #region Fields
    private readonly string _key1;
    private readonly string _key2;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeStringStringKey"/> struct.
    /// <para>+ Normalizes components for case-insensitive comparisons.</para>
    /// <para>- Throws when any component is <c>null</c>.</para>
    /// </summary>
    /// <param name="key1">First string component. Cannot be <c>null</c>.</param>
    /// <param name="key2">Second string component. Cannot be <c>null</c>.</param>
    public CompositeStringStringKey(string? key1, string? key2)
    {
        _key1 = key1?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(key1));
        _key2 = key2?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(key2));
    }
    #endregion Constructors

    #region Operators
    /// <summary>
    /// Determines whether two keys are equal.
    /// </summary>
    /// <param name="left">First key to compare.</param>
    /// <param name="right">Second key to compare.</param>
    /// <returns><c>true</c> if both keys are equal; otherwise <c>false</c>.</returns>
    public static bool operator ==(CompositeStringStringKey left, CompositeStringStringKey right)
        => left._key2 == right._key2 && left._key1 == right._key1;

    /// <summary>
    /// Determines whether two keys are not equal.
    /// </summary>
    /// <param name="left">First key to compare.</param>
    /// <param name="right">Second key to compare.</param>
    /// <returns><c>true</c> if the keys differ; otherwise <c>false</c>.</returns>
    public static bool operator !=(CompositeStringStringKey left, CompositeStringStringKey right)
        => left._key2 != right._key2 || left._key1 != right._key1;
    #endregion Operators

    #region Equality
    /// <summary>
    /// Indicates whether the current key is equal to another key.
    /// </summary>
    /// <param name="other">The key to compare with.</param>
    /// <returns><c>true</c> if the keys are equal; otherwise <c>false</c>.</returns>
    public bool Equals(CompositeStringStringKey other)
        => _key2 == other._key2 && _key1 == other._key1;

    /// <summary>
    /// Determines whether the specified object is equal to the current key.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is <see cref="CompositeStringStringKey"/> and equals this instance; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj)
        => obj is CompositeStringStringKey other && _key2 == other._key2 && _key1 == other._key1;

    /// <summary>
    /// Returns a hash code for the current key.
    /// </summary>
    /// <returns>Combined hash code of its components.</returns>
    public override int GetHashCode()
        => (_key2.GetHashCode() * 31) + _key1.GetHashCode();
    #endregion Equality
}
