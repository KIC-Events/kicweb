using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using KiCWeb.Models;
using System.Net.Http.Headers;
using KiCWeb.Services;
using System.Net;
using Microsoft.AspNetCore.SignalR;

namespace kicweb.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfigurationRoot _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private IKiCLogger _logger;
        public EmailService(IConfigurationRoot config, IHttpClientFactory clientFactory, IKiCLogger logger)
        {
            this._config = config;
            this._httpClientFactory = clientFactory;
            this._logger = logger;
        }

        public FormMessage FormSubmissionEmailFactory(string rep)
        {
            FormMessage message = new FormMessage();

            message.To.Add(_config["Email Addresses:" + rep]);
            message.Cc.Add(_config["Email Addresses:Admin"]);
            message.Subject = "Web Form Submission | " + rep + " | " + DateTime.Now.ToString();
            
            return message;
        }

        public async Task SendEmail(FormMessage message)
        {
            if(message.Html is null && message.HtmlBuilder is null)
            {
                throw new Exception("Empty FormMessage");
            }

            if (message.Html is null)
            {
                message.BuildHtml();
            }

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("KIC Automated Mailer", "mailer-daemon@kicevents.com"));
            foreach(string s in message.To)
            {
                mimeMessage.To.Add(new MailboxAddress("", s));
            }
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Html };

            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect("smtp.forwardemail.net", 465, true);
                    client.Authenticate(_config["Credentials:Mailbot:Username"], _config["Credentials:Mailbot:Password"]);
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
