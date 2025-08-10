using System;

namespace Ark;

/// <summary>
/// + Utility extensions for working with <see cref="System.Guid"/> values.
/// - Conversion routines operate on the raw byte representation only.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.guid"/>
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    /// Converts the first four bytes of the <paramref name="guid"/> to a 32-bit integer.
    /// </summary>
    /// <param name="guid">The source identifier.</param>
    /// <returns>A 32-bit integer composed from the first four bytes.</returns>
    public static int ToInt(this Guid guid)
    {
        var bytes = guid.ToByteArray();
        return BitConverter.ToInt32(bytes, 0);
    }
}

