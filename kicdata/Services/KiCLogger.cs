using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IO;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto.Engines;
using Square;

namespace KiCData.Services
{
    public class KiCLogger : IKiCLogger
    {
        private readonly string path;
        private string fileName;

        public KiCLogger()
        {
            path = System.Environment.CurrentDirectory.ToString() + @"/Logs/";
            InitLogFile();
        }

        public void LogText(string text)
        {
            InitLogFile();

            StreamWriter sw = File.AppendText(path + fileName);
            sw.WriteLine(DateTime.Now.ToString());
            sw.WriteLine(text);
            sw.WriteLine();
            sw.Close();
        }

        public void Log(Exception exception)
        {
            InitLogFile();

            StreamWriter sw = File.AppendText(path + fileName);
            sw.WriteLine("An exception has occurred.");
            sw.WriteLine(exception.ToString());
            //sw.WriteLine(context.Headers.Host.ToString());
            //sw.WriteLine(context.Headers.UserAgent.ToString());
            //sw.WriteLine(context.Headers.Cookie.ToString());
            sw.WriteLine();
            sw.WriteLine();
            if(exception.InnerException != null)
            {
                sw.WriteLine("Inner Exception");
                sw.WriteLine(exception.InnerException.ToString());
                sw.WriteLine();
                sw.WriteLine();
            }
            

            sw.Close();
        }

        public void LogSquareEx(Square.Exceptions.ApiException exception)
        {
            InitLogFile();

            StreamWriter sw = File.AppendText(path + fileName);
            sw.WriteLine("An exception has occurred.");
            sw.WriteLine(exception.ToString());
            sw.WriteLine();
            sw.WriteLine();
            if (exception.HttpContext.Response.Body != null)
            {
                sw.WriteLine("Response Body");
                sw.WriteLine(exception.HttpContext.Response.Body);
                sw.WriteLine();
                sw.WriteLine();
            }


            sw.Close();
        }

        private void InitLogFile()
        {
            DateTime now = DateTime.Now.Date;
            fileName = now.Year.ToString() + "-" + now.Month.ToString() + "-" + now.Day.ToString() + ".log";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if(!File.Exists(path + fileName))
            {
                File.Create(path + fileName).Close();
            }
        }
    }
}
