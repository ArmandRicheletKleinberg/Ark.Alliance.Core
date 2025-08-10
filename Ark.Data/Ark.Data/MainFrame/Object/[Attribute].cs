using System;

namespace Ark.Data
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute must be set on a POCO class that contains properties decorated with MainFrameProperty attribute.
    /// It is mandatory for a class to be considered as serializable main frame object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MainFrameObjectAttribute : Attribute
    { }
}