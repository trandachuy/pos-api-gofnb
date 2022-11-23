using System.Threading.Tasks;
using System.Collections.Generic;

namespace GoFoodBeverage.Email
{
    public interface IEmailSenderProvider
    {
        /// <summary>
        /// This method is used to send email to the user.
        /// </summary>
        /// <param name="subject">The subject string.</param>
        /// <param name="htmlContent">The email content.</param>
        /// <param name="receiver">The receiver's email address.</param>
        /// <returns>If the value is true, the email has been sent successfully.</returns>
        Task<bool> SendEmailAsync(
            string subject,
            string htmlContent,
            string receiver
        );

        /// <summary>
        /// This method is used to send emails to the user list.
        /// </summary>
        /// <param name="subject">The subject string.</param>
        /// <param name="htmlContent">The email content.</param>
        /// <param name="receiverList">List of email addresses.</param>
        /// <returns>If the value is true, the emails have been sent successfully.</returns>
        Task<bool> SendEmailAsync(
            string subject,
            string htmlContent,
            List<string> receiverList
        );
    }
}
