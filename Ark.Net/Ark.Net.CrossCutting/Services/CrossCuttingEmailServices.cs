using System.Threading.Tasks;
using Ark.Net.Models;

namespace Ark.Net.CrossCutting
{
    /// <summary>
    /// This class is used to access the cross cutting email services.
    /// Mainly used to send emails.
    /// </summary>
    public class CrossCuttingEmailServices
    {
        #region Fields

        /// <summary>
        /// The cross cutting HTTP repository is needed.
        /// </summary>
        internal CrossCuttingHttpRepository CrossCuttingHttpRepository = new CrossCuttingHttpRepository();

        #endregion Fields

        #region Properties (Public)

        /// <summary>
        /// Sends an email using the cross cutting services.
        /// The request data contains the email message data along with email profile to use.
        /// </summary>
        /// <param name="request">The request data with the email message to send.</param>
        /// <returns>
        /// Success : The email message has been successfully sent.
        /// BadPrerequisites : No root URL was defined.
        /// BadParameters : The email message data have not been validated.
        /// NoConnection : Unable to connect to the cross cutting services.
        /// Unauthorized : The application has not the right to access this cross cutting service.
        /// Failure : The response coming from the server has an unexpected model and the deserialization failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> SendEmailMessage(EmailSendRequestDto request)
            => CrossCuttingHttpRepository.PostEmailSendRequest(request);

        #endregion Properties (Public)
    }
}