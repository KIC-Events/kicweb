using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IO;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto.Engines;

namespace KiCData.Services
{
    public class KiCLogger : IKiCLogger
    {
        private readonly string path;
        private string fileName;

        public KiCLogger()
        {
            path = Environment.CurrentDirectory.ToString() + @"/Logs/";
            InitLogFile();
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
