using IRISA.CommunicationCenter.Library.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public class Logger : ILogger
    {
        public IEnumerable<ILogAppender> LogAppenders { get; }

        public Logger(IEnumerable<ILogAppender> logAppenders)
        {
            if (!logAppenders.Any())
                throw new ArgumentException("Log Appenders must be passed to logger.");

            LogAppenders = logAppenders;
        }

        public void LogDebug(string testText, params object[] parameters)
        {
            Log(testText, LogLevel.Debug, null, parameters);
        }

        public void LogInformation(string infoText, params object[] parameters)
        {
            Log(infoText, LogLevel.Information, parameters);
        }

        public void LogWarning(string warningText, params object[] parameters)
        {
            Log(warningText, LogLevel.Warning, parameters);
        }

        public void LogError(string errorText, params object[] parameters)
        {
            Log(errorText, LogLevel.Error, parameters);
        }

        public void LogException(Exception exception, string message)
        {
            string text =
                $"{message}\r\n" +
                $"{exception.InnerExceptionsMessage()}\r\n" +
                $"StackTrace :{exception.StackTrace}\r\n";

            Log(text, LogLevel.Exception, exception.StackTrace);
        }

        private void Log(string eventText, LogLevel logLevel, params object[] parameters)
        {
            try
            {
                eventText = string.Format(eventText, parameters);
                LogAppenders
                    .ToList()
                    .ForEach(x => x.Log(eventText, logLevel));
            }
            catch
            {
            }
        }

        public List<LogEvent> GetLogs(LogSearchModel searchModel, int pageSize, out int resultsCount)
        {
            return LogAppenders.First().GetLogs(searchModel, pageSize, out resultsCount);
        }
    }
}
