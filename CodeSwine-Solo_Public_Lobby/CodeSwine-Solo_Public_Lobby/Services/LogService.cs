using System;
using System.IO;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public class LogService
    {
        private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");

        public void LogException(Exception ex)
        {
            using (var sw = OpenLog())
            {
                sw.WriteLine(ex.Message);
            }
        }

        private StreamWriter OpenLog()
        {
            if (!File.Exists(_path))
            {
                var sw = File.CreateText(_path);
                sw.WriteLine("Error log generated at " + DateTime.Now.ToShortDateString());
                return sw;
            }

            return File.AppendText(_path);
        }
    }
}