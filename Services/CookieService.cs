using System.Net;

namespace kicweb.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CookieService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool AgeGateCookieAccepted(HttpRequest context)
        {
            var cookie = context.Cookies["age_Gate"];
            
            if (cookie == null) { return false; }
            else if (cookie == "true") { return true; }
            else { return false; }
        }

        public CookieOptions AgeGateCookieFactory()
        {
            CookieOptions cookie = new CookieOptions();
            cookie.Path = "/";
            cookie.Secure = true;
            cookie.Expires = DateTime.Now.AddDays(1);

            return cookie;
        }
    }
}
