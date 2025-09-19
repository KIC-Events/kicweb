using Hangfire;
using KiCData.Models.WebModels;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiCWeb.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IConfigurationRoot _config;
        private readonly IEmailService _emailService;
        private readonly IBackgroundJobClient _jobClient;

        public ApiController(IConfigurationRoot config, IEmailService emailService, IBackgroundJobClient  backgroundJobClient)
        {
            _config = config;
            _emailService = emailService;
            _jobClient = backgroundJobClient;
        }

        [HttpGet]
        public List<string> ListImages()
        {
            List<string> images = new List<string>();

            string path = _config["Files:Images"];

            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("Path is empty or not found.");
            }

            foreach(string f in Directory.EnumerateFiles(path))
            {
                images.Add(f);
            }

            return images;
        }

        [HttpGet]
        public List<string> ListImages(string path)
        {
            List<string> images = new List<string>();

            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("Path is empty or not found.");
            }

            foreach (string f in Directory.EnumerateFiles(path))
            {
                images.Add(f);
            }

            return images;
        }

        [HttpGet]
        public FileResult GetImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
            {
                throw new Exception("Path is empty or not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, "img/png");
        }

        /*
        [Route("SendEmail")]
        [HttpPost]
        public string SendEmail(string to, string subject, string body)
        {
            var msg = new FormMessage
            {
                To = [to],
                From = _config["Email Addresses:From"],
                Sender = _config["Email Addresses:From"],
                Subject = subject,
                Html = body
            };
            _jobClient.Enqueue(() => _emailService.SendEmail(msg));
            return "OK";
        }
        */
    }
}
