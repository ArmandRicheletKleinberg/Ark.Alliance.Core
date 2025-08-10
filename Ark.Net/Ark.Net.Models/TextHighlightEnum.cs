namespace Ark.Net.Models
{
    /// <summary>
    /// This enumeration of how to highlight the text displayed.
    /// </summary>
    public enum TextHighlightEnum
    {
        /// <summary>
        /// The text by default is not highlighted.
        /// </summary>
        None = 0,

        /// <summary>
        /// The text is not important and the style should reflect this.
        /// </summary>
        NotImportant = 1,

        /// <summary>
        /// The text should highlight the value as a success.
        /// </summary>
        Success = 2,

        /// <summary>
        /// The text should highlight the value as a warning.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// The text should highlight the value as an error.
        /// </summary>
        Error = 4
    }
}