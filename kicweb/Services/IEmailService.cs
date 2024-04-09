using MimeKit;

namespace kicweb.Services
{
    public interface IEmailService
    {
        public void SendEmail(MimeMessage message);
        public MimeMessage FormSubmissionEmailFactory(string rep, string address);
    }
}
