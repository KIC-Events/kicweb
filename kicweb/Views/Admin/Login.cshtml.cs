using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KiCData.Models.WebModels;
using KiCData.Models;

namespace KiCWeb.Views.Admin
{
    
    public class LoginModel : PageModel
    {
        private KiCdbContext _context;
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
            if(!ModelState.IsValid)
            {
                return;
            }
            User user = _context.User.Where(b => b.Username.Equals(login.UserName) && b.Password.Equals(login.Password)).FirstOrDefault();
            if (user == null)
            {
                ModelState.AddModelError("Login", "Invalid login attempt.");
                return;
            }
            return;

        }
    }
}
