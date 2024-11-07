using KiCData.Models;
using KiCData.Models.WebModels;
using MimeKit;

namespace KiCData.Services
{
    public interface IEmailService
    {
        public void SendEmail(FormMessage message);
        public FormMessage FormSubmissionEmailFactory(string rep);
    }
}
