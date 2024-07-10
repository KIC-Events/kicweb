using System.Text;
using System.Text.Json;

namespace KiCData.Models.WebModels
{
    public class FormMessage
    {
        public List<string>? To { get; set; }

        public List<string>? Cc { get; set; }

        public List<string>? Bcc { get; set; }

        public string From { get; set; }

        public string? Subject { get; set; }

        public string? Html { get; set; }

        public string? Text { get; set; }

        public StringBuilder? HtmlBuilder { get; set; }

        public string Sender { get; set; }

        public FormMessage()
        {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            HtmlBuilder = new StringBuilder();
            From = "mailer-daemon@kicevents.com";
            Sender = "mailer-daemon@kicevents.com";
            Cc.Add("technology@kicevents.com");
        }

        public void BuildHtml()
        {
            if(HtmlBuilder is null)
            {
                throw new Exception("No text has been added.");
            }
            else if(Html is not null)
            {
                throw new Exception("Body text is not null, this will cause overwrite.");
            }

            Html = HtmlBuilder.ToString();
            Text = HtmlBuilder.ToString();
        }

        public string MessageFactory()
        {
            if(To is null || Subject is null || Html is null)
            {
                throw new Exception("Missing message details.");
            }

            string message = JsonSerializer.Serialize(this);

            return message;
        }
    }
}
