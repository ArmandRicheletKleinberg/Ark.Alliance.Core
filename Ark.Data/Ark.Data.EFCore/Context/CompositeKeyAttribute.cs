using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Marks a property as part of a composite primary key.
    /// + Enables multi-column key mapping without fluent configuration.
    /// - Misuse can cause Entity Framework Core to create unexpected indexes.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/modeling/keys?tabs=data-annotations#composite-keys"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CompositeKeyAttribute : Attribute
    { }
}