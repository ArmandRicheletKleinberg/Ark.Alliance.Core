using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Requests an index on the decorated column.
    /// + Improves lookup performance for frequently filtered fields.
    /// - Excessive indexing slows down write operations.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/modeling/indexes"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexAttribute : Attribute
    { }
}