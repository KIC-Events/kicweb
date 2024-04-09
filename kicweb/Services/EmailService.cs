using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;

namespace kicweb.Services
{
    public class EmailService : IEmailService
    {
        private IConfigurationRoot _config;
        public EmailService(IConfigurationRoot config)
        {
            this._config = config;
        }

        public MimeMessage FormSubmissionEmailFactory(string rep, string address)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("KICWeb Automation", "technology@kicevents.com"));
            message.To.Add(new MailboxAddress(rep, address));
            message.Cc.Add(new MailboxAddress("KIC Admin", _config["AppSettings:Email Addresses:Admin"]));
            message.Subject = "Web Form Submission";
            
            return message;
        }

        public void SendEmail(MimeMessage message)
        {
            using(SmtpClient smtpClient = new SmtpClient())
            {
                try
                {
                    smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.SslOnConnect);
                    smtpClient.Authenticate(_config.GetValue<string>("AppSettings:Credentials:smtp-username"), _config.GetValue<string>("AppSettings:Credentials:smtp-password"));
                    smtpClient.Send(message);
                }
                catch (Exception ex)
                {
                    //Handle exception and log here
                }
            }
        }
    }
}
