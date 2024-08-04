using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KiCData.Models.WebModels;
using KiCData.Models;

namespace KiCWeb.Views.Admin
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginViewModel Login { get; set; }
        //private readonly ILogger<LoginViewModel> _logger;

        //public LoginViewModel<ILogger<LoginViewModel>> logger
        //{
        //    _logger = logger
        //}
        public void OnGet()
        {
        }

        public void OnPost(LoginViewModel login)
        {
            KiCdbContext context = new KiCdbContext();
            User user = context.User.Where(b => b.Username.Equals(login.UserName)).FirstOrDefault();
            if (login.UserName == login.UserName)
            {

            }
        }
    }
}
