using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public interface ILogAppender
    {
        void Log(string text, LogLevel level);
        List<LogEvent> GetLogs(LogSearchModel searchModel, int pageSize, out int resultsCount);
    }
}
