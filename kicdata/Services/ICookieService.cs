using Microsoft.AspNetCore.Http;

namespace KiCData.Services
{
    public interface ICookieService
    {
        /// <summary>
        /// Using an HttpRequest, checks if the provided request contains a cookie that indicates the user has passed the age gate.
        /// </summary>
        /// <param name="context">The default HttpContext for the current action.</param>
        /// <returns>bool</returns>
        public bool AgeGateCookieAccepted(HttpRequest context);

        public bool AuthTokenCookie(HttpRequest context);

        /// <summary>
        /// Builds a CookieOptions configured to store the user's acceptance of the age gate disclaimer.
        /// </summary>
        /// <returns>CookieOptions</returns>
        public CookieOptions NewCookieFactory();

        public void DeleteCookie(HttpRequest context, string key);
    }
}
