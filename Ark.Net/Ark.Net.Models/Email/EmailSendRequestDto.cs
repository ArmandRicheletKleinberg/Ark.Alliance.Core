namespace Ark.Net.Models
{
    /// <summary>
    /// Contains all the information needed to send an email message.
    /// </summary>
    public class EmailSendRequestDto
    {
        #region Properties (Public)

        /// <summary>
        /// The email message to send.
        /// </summary>
        public EmailMessageDto Message { get; set; }

        /// <summary>
        /// The email service profile used if more than one.
        /// Optional. Default to the first profile defined in the CrossCutting application settings.
        /// </summary>
        public string ProfileToUse { get; set; }

        #endregion Properties (Public)
    }
}