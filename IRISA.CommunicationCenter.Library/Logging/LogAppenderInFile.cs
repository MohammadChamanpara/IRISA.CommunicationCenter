using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public class LogAppenderInFile : ILogAppender
    {
        private string LogFileAddress => $"Logs\\Logs {DateTime.Now.ToPersianDate("-")}.txt";

        public void Log(string eventText, LogLevel logLevel)
        {
            string logText = 
                $" English Time : {DateTime.Now}\r\n" +
                $" Persian Time : {DateTime.Now.ToPersianDateTime()}\r\n" +
                $" Level : {logLevel.ToPersian()}\r\n" +
                $" Text : {eventText}\r\n" +
                $"_______________________________________________________________________________________\r\n\r\n";

            string fileAddress = LogFileAddress;

            var directory = Path.GetDirectoryName(fileAddress);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.AppendAllText(fileAddress, logText);
        }

        public List<LogEvent> GetLogs(LogSearchModel searchModel, int pageSize, out int resultsCount)
        {
            throw new NotSupportedException($"Getting logs is not supported in {nameof(LogAppenderInFile)}");
        }
    }
}
