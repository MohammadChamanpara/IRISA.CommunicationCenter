using IRISA.CommunicationCenter.Library.Models;
using System;
using System.IO;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public class LoggerInFile : BaseLogger
    {
        private string LogFileAddress => $"Events\\Events {DateTime.Now.ToPersianDate("-")}.txt";

        protected override void Log(string eventText, EventType eventType)
        {
            string logText = string.Concat(new string[]
            {
                " English Time : ",
                DateTime.Now.ToString(),
                "\r\n Persian Time : ",
                DateTime.Now.ToPersianDateTime("/"),
                "\r\n Type : ",
                eventType.ToPersian(),
                "\r\n Event : ",
                eventText,
                "\r\n"
            });

            logText += "_______________________________________________________________________________________\r\n\r\n";
            string fileAddress = LogFileAddress;
            string path = Path.GetFullPath(fileAddress).Replace(Path.GetFileName(fileAddress), "");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.AppendAllText(fileAddress, logText);
        }

        public override IQueryable<LogEvent> GetLogs()
        {
            throw new NotSupportedException($"Getting logs is not supported in {nameof(LoggerInFile)}");
        }
    }
}
