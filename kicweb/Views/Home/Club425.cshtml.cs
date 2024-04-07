using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace kicweb.Views.Home
{
    public class Club425 : PageModel
    {
        private readonly ILogger<Club425> _logger;

        public Club425(ILogger<Club425> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}