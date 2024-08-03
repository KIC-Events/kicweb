using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KiCData.Models.WebModels;

namespace KiCWeb.Views.Admin
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginViewModel Login { get; set; }
        public void OnGet()
        {
        }

        public void OnPost()
        {
        }
    }
}
