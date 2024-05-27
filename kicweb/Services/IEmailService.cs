using KiCWeb.Models;
using MimeKit;

namespace kicweb.Services
{
    public interface IEmailService
    {
        public Task SendEmail(FormMessage message);
        public FormMessage FormSubmissionEmailFactory(string rep);
    }
}
