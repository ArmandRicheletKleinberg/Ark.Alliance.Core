using System.Collections.Generic;

namespace Ark.Net.Models
{
    /// <summary>
    /// The email message contains all the information to send an email message.
    /// That is the recipients, title, body and attachments.
    /// </summary>
    public class EmailMessageDto
    {
        #region Properties (Public)

        /// <summary>
        /// The sender of the email message.
        /// Optional. If not set the default recipient from the email service profile will be set.
        /// </summary>
        public EmailRecipientDto From { get; set; }

        /// <summary>
        /// The direct recipients of the email message (To).
        /// Optional. At least one recipient should be chosen.
        /// </summary>
        public List<EmailRecipientDto> Tos { get; set; }

        /// <summary>
        /// The indirect recipients of the email message (Cc).
        /// Optional. At least one recipient should be chosen.
        /// </summary>
        public List<EmailRecipientDto> Ccs { get; set; }

        /// <summary>
        /// The hidden indirect recipients of the email message (Bcc).
        /// Optional. At least one recipient should be chosen.
        /// </summary>
        public List<EmailRecipientDto> Bccs { get; set; }

        /// <summary>
        /// The subject of the email message (title).
        /// Optional but recommended.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The body of the email message in HTML.
        /// Optional. At least one body should be chosen.
        /// </summary>
        public string BodyHtml { get; set; }

        /// <summary>
        /// The body of the email message in raw text.
        /// Optional. At least one body should be chosen.
        /// </summary>
        public string BodyText { get; set; }

        /// <summary>
        /// The attachments file to attach to this email message.
        /// Optional.
        /// </summary>
        public List<FileDto> Attachments { get; set; }

        #endregion Properties (Public)
    }
}
