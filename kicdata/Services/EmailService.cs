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
using System.Text;
using System.Text.Json;

namespace KiCData.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfigurationRoot _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private IKiCLogger _logger;
        private HttpClient _httpClient;

        public EmailService(IConfigurationRoot config, IHttpClientFactory clientFactory, IKiCLogger logger, HttpClient httpClient)
        {
            this._config = config;
            this._httpClientFactory = clientFactory;
            this._logger = logger;
            this._httpClient = httpClient;
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
            message.From = "mailer-daemon@kicevents.com";


            return message;
        }

        /// <summary>
        /// Sends the given message as an email.
        /// </summary>
        /// <param name="message">The FormMessage to be sent.</param>
        /// <returns>Task</returns>
        /// <exception cref="Exception"></exception>
        public void SendEmail(FormMessage message)
        {
            if(message.Html is null && message.HtmlBuilder is null)
            {
                throw new Exception("Empty FormMessage");
            }

            if (message.Html is null)
            {
                message.BuildHtml();
            }

            _logger.LogText("Sending email. ");

            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();

            string toValue = null;
            string ccValue = null;

            foreach (string s in message.To)
            {
                toValue = toValue + "," + s;
            }

            foreach (string s in message.Cc){
                ccValue = ccValue + "," + s;
            }

            keyValuePairs.Add(new KeyValuePair<string, string>("to", toValue));
            keyValuePairs.Add(new KeyValuePair<string, string>("cc", ccValue));
            keyValuePairs.Add(new KeyValuePair<string, string>("from", message.From));
            keyValuePairs.Add(new KeyValuePair<string, string>("body", message.Html));
            keyValuePairs.Add(new KeyValuePair<string, string>("html", message.Html));
            keyValuePairs.Add(new KeyValuePair<string, string>("subject", message.Subject));

            HttpRequestMessage? request = new HttpRequestMessage(HttpMethod.Post, "");

            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(keyValuePairs);

            request.Content = formUrlEncodedContent;

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(_config["Credentials:Mailbot:Token"] + ":")));

            HttpResponseMessage? result = _httpClient.Send(request);

            _logger.LogText("Message sent...");
        }
    }
}
