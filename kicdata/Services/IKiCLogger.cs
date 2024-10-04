using Microsoft.AspNetCore.Http;

namespace KiCData.Services
{
    public interface IKiCLogger
    {
        void Log(Exception exception);

        void LogText(string text);

        void LogSquareEx(Square.Exceptions.ApiException exception);
    }
}
