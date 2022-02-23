using System;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public interface ILogger
    {
        event Action EventLogged;
        void LogSuccess(string successText, params object[] parameters);
        void LogInfo(string infoText, params object[] parameters);
        void LogWarning(string warningText, params object[] parameters);
        void LogError(string errorText, params object[] parameters);
        void LogException(Exception exception);
        void LogTest(string testText, params object[] parameters);
        IQueryable<LogEvent> GetLogs();
    }
}
