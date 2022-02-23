using IRISA.CommunicationCenter.Library.Loggers;
using System;
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
            Log(testText, EventType.Debug, null, parameters);
        }

        public void LogInformation(string infoText, params object[] parameters)
        {
            Log(infoText, EventType.Information, parameters);
        }

        public void LogWarning(string warningText, params object[] parameters)
        {
            Log(warningText, EventType.Warning, parameters);
        }

        public void LogError(string errorText, params object[] parameters)
        {
            Log(errorText, EventType.Error, parameters);
        }

        public void LogException(Exception exception, string message)
        {
            string text = 
                $"{message}\r\n" +
                $"{exception.InnerExceptionsMessage()}\r\n" +
                $"StackTrace :{exception.StackTrace}\r\n";

            Log(text, EventType.Exception, exception.StackTrace);
        }
        
        private void Log(string eventText, EventType eventType, params object[] parameters)
        {
            try
            {
                eventText = string.Format(eventText, parameters);
                Log(eventText, eventType);
                OnEventLogged();
            }
            catch
            {
            }
        }

        protected abstract void Log(string eventText, EventType eventType);

        public abstract IQueryable<LogEvent> GetLogs();
    }
}
