using BusinessLogic.Models.Mails;

namespace BusinessLogic.Services.External.Base
{
    public interface IMailService
    {
        /// <summary>
        ///     Build a new <see cref="MailContentModel"/> instance for
        ///     the register-confirmation purpose.
        /// </summary>
        /// <param name="templatePath">
        ///     The path where the mail template is located.
        /// </param>
        /// <param name="to">
        ///     The to-address this mail will be sent.
        /// </param>
        /// <param name="subject">
        ///     The subject of the mail.
        /// </param>
        /// <param name="confirmationLink">
        ///     The link that leads user to confirm their registration.
        /// </param>
        /// <returns>
        ///     The <see cref="MailContentModel"/> instance contains required
        ///     information to help user confirm the registration.
        /// </returns>
        Task<MailContentModel> BuildRegisterConfirmMailContentAsync(
            string templatePath,
            string to,
            string subject,
            string confirmationLink);

        Task<bool> SendMailAsync(MailContentModel mailContent);
    }
}
