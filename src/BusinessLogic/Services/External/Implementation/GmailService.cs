using BusinessLogic.Models.Mails;
using BusinessLogic.Services.External.Base;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Options.Models;
using System.Text;

namespace BusinessLogic.Services.External.Implementation
{
    internal class GmailService : IMailService
    {
        private readonly MailOptions _mailOptions;

        public GmailService(MailOptions mailOptions)
        {
            _mailOptions = mailOptions;
        }

        public async Task<MailContentModel> BuildRegisterConfirmMailContentAsync(
            string templatePath,
            string to,
            string subject,
            string confirmationLink)
        {
            var mailTemplate = await File.ReadAllTextAsync(path: templatePath);

            // Build content for the mail.
            var mailBodyBuilder = new StringBuilder(value: mailTemplate);

            // Replace the placeholder with specified values.
            mailBodyBuilder.Replace(
                oldValue: MailOptions.LinkPlaceHolder,
                newValue: confirmationLink);

            mailBodyBuilder.Replace(
                oldValue: MailOptions.WebUrlHolder,
                newValue: _mailOptions.WebUrl);

            mailBodyBuilder.Replace(
                oldValue: MailOptions.LogoUrlHolder,
                newValue: Path.Combine(_mailOptions.WebUrl, "logo", "ecomore_logo.jpg"));

            var mailContent = new MailContentModel
            {
                To = to,
                Subject = subject,
                Body = mailBodyBuilder.ToString(),
            };

            return mailContent;
        }

        public async Task<bool> SendMailAsync(MailContentModel mailContent)
        {
            //Init an email for sending.
            var email = new MimeMessage()
            {
                Sender = new MailboxAddress(
                    name: _mailOptions.DisplayName,
                    address: _mailOptions.Address)
            };

            // Add the "from" section.
            email.From.Add(
                address: new MailboxAddress(
                    name: _mailOptions.DisplayName,
                    address: _mailOptions.Address)
                );

            //Add the "to" section.
            email.To.Add(
                address: MailboxAddress.Parse(text: mailContent.To));

            //Add the "subject" section.
            email.Subject = mailContent.Subject;

            //Add the "body" section.
            var bodyBuilder = new BodyBuilder()
            {
                HtmlBody = mailContent.Body
            };

            email.Body = bodyBuilder.ToMessageBody();

            using SmtpClient smtp = new();

            try
            {
                await smtp.ConnectAsync(
                    host: _mailOptions.Host,
                    port: _mailOptions.Port,
                    options: SecureSocketOptions.StartTlsWhenAvailable);

                await smtp.AuthenticateAsync(
                    userName: _mailOptions.Address,
                    password: _mailOptions.Password);

                await smtp.SendAsync(message: email);

                await smtp.DisconnectAsync(quit: true);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
