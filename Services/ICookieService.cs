namespace kicweb.Services
{
    public interface ICookieService
    {
        public bool AgeGateCookieAccepted(HttpRequest context);

        public CookieOptions AgeGateCookieFactory();
    }
}
