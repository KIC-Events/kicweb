using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using KiCWeb.Models;
using System.Net.Http.Headers;

namespace kicweb.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfigurationRoot _config;
        private readonly IHttpClientFactory _httpClientFactory;
        public EmailService(IConfigurationRoot config, IHttpClientFactory clientFactory)
        {
            this._config = config;
            this._httpClientFactory = clientFactory;
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

            if(message.Html is null)
            {
                message.BuildHtml();
            }

            string serializedMessage = message.MessageFactory();

            HttpClient client = _httpClientFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.forwardemail.net/v1/emails");
            request.Headers.Add("Basic Authorization", _config["Credentials:Mailbot:Toekn"]);

            var response = await client.SendAsync(request);

            Console.WriteLine(response.Content);

            //ADD EXCEPTION HANDLING and LOGGING
        }
    }
}
