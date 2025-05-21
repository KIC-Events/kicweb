using Microsoft.AspNetCore.Mvc;

namespace KiCWeb.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IConfigurationRoot _config;

        public ApiController(IConfigurationRoot config)
        {
            _config = config;
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
    }
}
