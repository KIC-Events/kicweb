using Microsoft.AspNetCore.Http;

namespace KiCData.Services
{
    public interface IKiCLogger
    {
        void Log(Exception exception);

        void LogSquareEx(Square.Exceptions.ApiException exception);
    }
}
