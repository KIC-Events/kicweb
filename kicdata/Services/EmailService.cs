using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using KiCData.Models;
using KiCData.Models.WebModels;
using System.Net.Http.Headers;
using KiCData.Services;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace KiCData.Services
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

        /// <summary>
        /// Creates a form message with the necessary details to be serialized into an email.
        /// </summary>
        /// <param name="rep">The member of staff to whom the email should be sent.</param>
        /// <returns>FormMessage</returns>
        public FormMessage FormSubmissionEmailFactory(string rep)
        {
            FormMessage message = new FormMessage();

            message.To.Add(_config["Email Addresses:" + rep]);
            message.Cc.Add(_config["Email Addresses:Admin"]);
            message.Subject = "Web Form Submission | " + rep + " | " + DateTime.Now.ToString();
            
            return message;
        }

        /// <summary>
        /// Sends the given message as an email.
        /// </summary>
        /// <param name="message">The FormMessage to be sent.</param>
        /// <returns>Task</returns>
        /// <exception cref="Exception"></exception>
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
                _logger.LogText("Sending email.");

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
