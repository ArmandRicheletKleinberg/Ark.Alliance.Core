namespace Ark.Net.Models
{
    /// <summary>
    /// A recipient used to send email to.
    /// </summary>
    public class EmailRecipientDto
    {
        #region Properties (Public)

        /// <summary>
        /// The recipient name that will be displayed instead of the email address in the mail client.
        /// Maybe null.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The email address to send email to.
        /// </summary>
        public string Address { get; set; }

        #endregion Properties (Public)
    }
}
