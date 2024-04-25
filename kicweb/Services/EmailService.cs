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
            message.Subject = "Web Form Submission";
            
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

            string serializedMessage;

            try
            {
                serializedMessage = message.MessageFactory();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            HttpClient client = _httpClientFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.forwardemail.net/v1/emails");
            var content = new System.Net.Http.StringContent(serializedMessage);
            request.Content = content;

            try
            {
                var tokenInBytes = System.Text.Encoding.UTF8.GetBytes(_config["Credentials:Mailbot:Token"]);
                var tokenBase64 = System.Convert.ToBase64String(tokenInBytes);
                AuthenticationHeaderValue authenticationHeaderValue = new AuthenticationHeaderValue("Basic", tokenBase64);
                request.Headers.Authorization = authenticationHeaderValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var response = await client.SendAsync(request);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Email sending failed.");
            }
        }
    }
}
