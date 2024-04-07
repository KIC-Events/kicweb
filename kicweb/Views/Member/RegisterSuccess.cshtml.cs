using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace kicweb.Views.Member
{
    public class RegisterSuccess : PageModel
    {
        private readonly ILogger<RegisterSuccess> _logger;

        public RegisterSuccess(ILogger<RegisterSuccess> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}