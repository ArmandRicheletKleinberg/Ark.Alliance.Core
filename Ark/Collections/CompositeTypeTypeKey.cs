namespace Ark;

#nullable enable

/// <summary>
/// Represents a composite dictionary key composed of two <see cref="Type"/> instances.
/// <para>+ Avoids tuple allocations for type-based lookups.</para>
/// <para>- Components must not be <c>null</c>.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.type"/></para>
/// </summary>
/// <example>
/// <code language="csharp">
/// var key = new CompositeTypeTypeKey(typeof(string), typeof(int));
/// </code>
/// </example>
public struct CompositeTypeTypeKey : IEquatable<CompositeTypeTypeKey>
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeTypeTypeKey"/> struct.
    /// </summary>
    /// <param name="type1">First type component.</param>
    /// <param name="type2">Second type component.</param>
    public CompositeTypeTypeKey(Type type1, Type type2)
        : this()
    {
        Type1 = type1;
        Type2 = type2;
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// Gets the first type component.
    /// </summary>
    public Type Type1 { get; }

    /// <summary>
    /// Gets the second type component.
    /// </summary>
    public Type Type2 { get; }
    #endregion Properties

    #region Operators
    /// <summary>
    /// Determines whether two keys are equal.
    /// </summary>
    /// <param name="left">First key to compare.</param>
    /// <param name="right">Second key to compare.</param>
    /// <returns><c>true</c> if both keys are equal; otherwise <c>false</c>.</returns>
    public static bool operator ==(CompositeTypeTypeKey left, CompositeTypeTypeKey right)
        => left.Type1 == right.Type1 && left.Type2 == right.Type2;

    /// <summary>
    /// Determines whether two keys are not equal.
    /// </summary>
    /// <param name="left">First key to compare.</param>
    /// <param name="right">Second key to compare.</param>
    /// <returns><c>true</c> if the keys differ; otherwise <c>false</c>.</returns>
    public static bool operator !=(CompositeTypeTypeKey left, CompositeTypeTypeKey right)
        => left.Type1 != right.Type1 || left.Type2 != right.Type2;
    #endregion Operators

    #region Equality
    /// <inheritdoc />
    public bool Equals(CompositeTypeTypeKey other) => Type1 == other.Type1 && Type2 == other.Type2;

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        var other = obj is CompositeTypeTypeKey key ? key : default;
        return Type1 == other.Type1 && Type2 == other.Type2;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Type1.GetHashCode() * 397) ^ Type2.GetHashCode();
        }
    }
    #endregion Equality
}
