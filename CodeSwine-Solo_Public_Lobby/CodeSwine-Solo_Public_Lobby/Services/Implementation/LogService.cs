using System;
using System.IO;

namespace GTA5_Private_Public_Lobby.Services.Implementation
{
    public class LogService : ILogService
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