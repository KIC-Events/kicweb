namespace KiCWeb.Services
{
    public interface IKiCLogger
    {
        void Log(Exception exception, HttpRequest context);
    }
}
