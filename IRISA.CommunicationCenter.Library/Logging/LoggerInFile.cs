using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public class LoggerInFile : BaseLogger
    {
        private string LogFileAddress => $"Logs\\Logs {DateTime.Now.ToPersianDate("-")}.txt";

        protected override void Log(string eventText, LogLevel logLevel)
        {
            string logText = string.Concat(new string[]
            {
                " English Time : ",
                DateTime.Now.ToString(),
                "\r\n Persian Time : ",
                DateTime.Now.ToPersianDateTime("/"),
                "\r\n Type : ",
                logLevel.ToPersian(),
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

        public override List<LogEvent> GetLogs(LogSearchModel searchModel, int pageSize, out int resultsCount)
        {
            throw new NotSupportedException($"Getting logs is not supported in {nameof(LoggerInFile)}");
        }
    }
}
