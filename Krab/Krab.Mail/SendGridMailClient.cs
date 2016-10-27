using SendGrid;
using SendGrid.Helpers.Mail;

namespace Krab.Mail
{
    public class SendGridMailClient : IMailClient
    {
        private readonly SendGridAPIClient _sendGridClient;

        public SendGridMailClient(string apiKey)
        {
            _sendGridClient = new SendGridAPIClient(apiKey);
        }

        public void Send(string from, string to, string subject, string type, string value)
        {
            var fromEmail = new Email(from);
            var toEmail = new Email(to);
            var content = new Content(type, value);
            var mail = new SendGrid.Helpers.Mail.Mail(fromEmail, subject, toEmail, content);

            _sendGridClient.client.mail.send.post(requestBody: mail.Get());
        }
    }
}
