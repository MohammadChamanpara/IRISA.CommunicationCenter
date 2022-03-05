using IRISA.CommunicationCenter.Library.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public class LogAppenderInMemory : ILogAppender
    {
        private static ConcurrentBag<LogEvent> logs = new ConcurrentBag<LogEvent>();
        private static int _id = 1;
        public void Log(string eventText, LogLevel logLevel)
        {
            logs.Add(new LogEvent()
            {
                Id = _id++,
                Time = DateTime.Now,
                Text = eventText,
                LogLevel = logLevel
            });

            int limit = 1000000;
            if (logs.Count > limit * 2)
                logs = new ConcurrentBag<LogEvent>
                (
                    logs
                        .OrderByDescending(x => x.Id)
                        .Take(limit)
                );
        }

        public List<LogEvent> GetLogs(LogSearchModel searchModel, int pageSize, out int resultsCount)
        {
            var events = logs.ToList();

            events = searchModel.SearchKeyword.HasValue()
                ? events
                    .Where
                    (
                        x =>
                            x.Id.ToString().Contains(searchModel.SearchKeyword) ||
                            x.LogLevel.ToString().Contains(searchModel.SearchKeyword) ||
                            x.PersianLogLevel.Contains(searchModel.SearchKeyword) ||
                            x.PersianTime.Contains(searchModel.SearchKeyword) ||
                            x.Text.Contains(searchModel.SearchKeyword)
                    )
                    .ToList()
                : events;

            events = searchModel.LogLevel.HasValue
                ? events.Where(x => x.LogLevel == searchModel.LogLevel).ToList()
                : events;

            resultsCount = events.Count();

            return events
                .OrderByDescending(x => x.Id)
                .Take(pageSize)
                .ToList();
        }
    }
}
