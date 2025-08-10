namespace Ark
{
    /// <summary>
    ///     Used to mark properties that should be ignored when performing a deep clone.
    ///     This is typically applied to reference-type properties whose value is derived
    ///     from another field and should not be copied.
    /// </summary>
    public class DoNotCloneAttribute : Attribute
    {
    }
}
