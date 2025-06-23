using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace KiCWeb.Views.Shared
{
    public class UnderConstruction : PageModel
    {
        private readonly ILogger<UnderConstruction> _logger;

        public UnderConstruction(ILogger<UnderConstruction> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}