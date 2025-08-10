using Ark.Net.Models;
using FluentValidation;

namespace Ark.Net.CrossCutting.Validation
{
    /// <inheritdoc />
    /// <summary>
    /// This validator is used to validate an email message.
    /// It checks that the addresses are valid and that there is at least one recipient and one subject. The body can be empty.
    /// </summary>
    public class EmailMessageValidator : AbstractValidator<EmailMessageDto>
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="EmailMessageValidator"/> instance.
        /// </summary>
        public EmailMessageValidator()
        {
            var recipientValidator = new EmailRecipientValidator();
            RuleFor(message => message.Tos).NotEmpty().When(message => message.Ccs.HasNoElements() && message.Bccs.HasNoElements());
            RuleFor(message => message.Ccs).NotEmpty().When(message => message.Tos.HasNoElements() && message.Bccs.HasNoElements());
            RuleFor(message => message.Bccs).NotEmpty().When(message => message.Tos.HasNoElements() && message.Ccs.HasNoElements());
            RuleForEach(message => message.Tos).SetValidator(recipient => recipientValidator);
            RuleForEach(message => message.Ccs).SetValidator(recipient => recipientValidator);
            RuleForEach(message => message.Bccs).SetValidator(recipient => recipientValidator);
        }

        #endregion Constructors
    }
}