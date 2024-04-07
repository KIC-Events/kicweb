using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace kicweb.Views.Home
{
    public class Volunteers : PageModel
    {
        private readonly ILogger<Volunteers> _logger;

        public Volunteers(ILogger<Volunteers> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}