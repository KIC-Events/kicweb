using MailKit;
using MailKit.Net.Smtp;

namespace kicweb.Services
{
    public class EmailService : IEmailService
    {
        public EmailService(IConfigurationRoot config)
        {
        }

        public async Task FormSubmissionEmail(string address)
        {
            
        }
    }
}
