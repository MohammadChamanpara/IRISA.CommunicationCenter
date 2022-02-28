using System;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public interface ILogger
    {
        void LogDebug(string testText, params object[] parameters);
        void LogInformation(string successText, params object[] parameters);
        void LogWarning(string warningText, params object[] parameters);
        void LogError(string errorText, params object[] parameters);
        void LogException(Exception exception, string message);
        void SetMinumumLevel(LogLevel minimumLevel);
        List<LogEvent> GetLogs(LogSearchModel searchModel, int pageSize, out int resultsCount);
    }
}
