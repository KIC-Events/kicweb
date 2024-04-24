using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using KiCWeb.Services;

namespace kicweb.Services
{
    public class EmailService : IEmailService
    {
        private IConfigurationRoot _config;
        private IKiCLogger _logger;
        public EmailService(IConfigurationRoot config, IKiCLogger logger)
        {
            this._config = config;
            this._logger = logger;
        }

        public MimeMessage FormSubmissionEmailFactory(string rep, string address)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("KICWeb Automation", "technology@kicevents.com"));
            message.To.Add(new MailboxAddress(rep, address));
            message.Cc.Add(new MailboxAddress("KIC Admin", _config["Email Addresses:Admin"]));
            message.Subject = "Web Form Submission";
            
            return message;
        }

        public void SendEmail(MimeMessage message, HttpRequest context)
        {
            using(SmtpClient smtpClient = new SmtpClient())
            {
                try
                {
                    smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.SslOnConnect);
                    smtpClient.Authenticate(_config.GetValue<string>("Credentials:smtp-username"), _config.GetValue<string>("Credentials:smtp-password"));
                    smtpClient.Send(message);
                }
                catch (Exception ex)
                {
                    _logger.Log(ex, context);
                }
            }
        }
    }
}
