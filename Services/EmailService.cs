using MailKit;
using MailKit.Net.Smtp;

namespace kicweb.Services
{
    public class EmailService : IEmailService
    {
        private SmtpClient smtpClient;


        public EmailService(IConfigurationRoot config)
        {

        }

        public Task FormSubmissionEmail(string address)
        {
            
        }
    }
}
