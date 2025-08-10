using Ark.Net.Models;
using FluentValidation;

namespace Ark.Net.CrossCutting.Validation
{
    /// <inheritdoc />
    /// <summary>
    /// This validator is used to validate an email recipient.
    /// It checks that the email address is filled and is valid.
    /// </summary>
    public class EmailRecipientValidator : AbstractValidator<EmailRecipientDto>
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="EmailRecipientValidator"/> instance.
        /// </summary>
        public EmailRecipientValidator()
        {
            RuleFor(form => form.Address).NotEmpty().EmailAddress();
        }

        #endregion Constructors
    }
}