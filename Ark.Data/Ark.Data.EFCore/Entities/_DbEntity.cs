// ReSharper disable UnusedTypeParameter

using System.Collections.Generic;

namespace Ark.Data.EFCore
{
    /// <summary>
    /// The base class for all EF Core database entities.
    /// The entity is always linked to a database context.
    /// </summary>
    public abstract class DbEntity
    {
    }

    /// <summary>
    /// The base class for all EF Core database entities.
    /// The entity is always linked to a database context.
    /// </summary>
    public abstract class DbEntity<TDbContext> : DbEntity
        where TDbContext : DbContextEx, new()
    {
    }
}