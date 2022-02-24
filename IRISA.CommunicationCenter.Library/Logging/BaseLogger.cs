using IRISA.CommunicationCenter.Library.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public abstract partial class BaseLogger : ILogger
    {
        public event Action EventLogged;

        protected void OnEventLogged()
        {
            EventLogged?.Invoke();
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
                Log(eventText, logLevel);
                OnEventLogged();
            }
            catch
            {
            }
        }

        protected abstract void Log(string eventText, LogLevel logLevel);

        public abstract List<LogEvent> GetLogs(LogSearchModel searchModel, int pageSize, out int resultsCount);
    }
}
