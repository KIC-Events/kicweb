using MimeKit;

namespace kicweb.Services
{
    public interface IEmailService
    {
        public void SendEmail(MimeMessage message, HttpRequest context);
        public MimeMessage FormSubmissionEmailFactory(string rep, string address);
    }
}
