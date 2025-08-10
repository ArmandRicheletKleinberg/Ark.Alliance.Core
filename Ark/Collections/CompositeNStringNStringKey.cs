namespace Ark;

#nullable enable

/// <summary>
/// Represents a case-insensitive composite dictionary key composed of two optional strings.
/// <para>+ Supports <c>null</c> components without allocations.</para>
/// <para>- Uses the sentinel value "NULL" which may collide with legitimate values.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.string"/></para>
/// </summary>
/// <example>
/// <code language="csharp">
/// var key = new CompositeNStringNStringKey(null, "btc");
/// </code>
/// </example>
public struct CompositeNStringNStringKey : IEquatable<CompositeNStringNStringKey>
{
    #region Fields
    private readonly string _key1;
    private readonly string _key2;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeNStringNStringKey"/> struct.
    /// <para>+ Normalizes missing components to the sentinel value "NULL".</para>
    /// </summary>
    /// <param name="key1">First string component. <c>null</c> becomes "NULL".</param>
    /// <param name="key2">Second string component. <c>null</c> becomes "NULL".</param>
    public CompositeNStringNStringKey(string? key1, string? key2)
    {
        _key1 = key1?.ToLowerInvariant() ?? "NULL";
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
    public static bool operator ==(CompositeNStringNStringKey left, CompositeNStringNStringKey right)
        => left._key2 == right._key2 && left._key1 == right._key1;

    /// <summary>
    /// Determines whether two keys are not equal.
    /// </summary>
    /// <param name="left">First key to compare.</param>
    /// <param name="right">Second key to compare.</param>
    /// <returns><c>true</c> if the keys differ; otherwise <c>false</c>.</returns>
    public static bool operator !=(CompositeNStringNStringKey left, CompositeNStringNStringKey right)
        => left._key2 != right._key2 || left._key1 != right._key1;
    #endregion Operators

    #region Equality
    /// <summary>
    /// Indicates whether the current key is equal to another key.
    /// </summary>
    /// <param name="other">The key to compare with.</param>
    /// <returns><c>true</c> if the keys are equal; otherwise <c>false</c>.</returns>
    public bool Equals(CompositeNStringNStringKey other)
        => _key2 == other._key2 && _key1 == other._key1;

    /// <summary>
    /// Determines whether the specified object is equal to the current key.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is <see cref="CompositeNStringNStringKey"/> and equals this instance; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj)
        => obj is CompositeNStringNStringKey other && _key2 == other._key2 && _key1 == other._key1;

    /// <summary>
    /// Returns a hash code for the current key.
    /// </summary>
    /// <returns>Combined hash code of its components.</returns>
    public override int GetHashCode()
        => (_key2.GetHashCode() * 31) + _key1.GetHashCode();
    #endregion Equality
}
