using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public class LoggerInMemory : BaseLogger
    {
        private static List<LogEvent> logs = new List<LogEvent>();
        private static int id = 1;
        protected override void Log(string eventText, LogLevel logLevel)
        {
            logs.Add(new LogEvent()
            {
                Id = id++,
                Time = DateTime.Now,
                Text = eventText,
                LogLevel = logLevel
            });

            if (logs.Count > 2000)
                logs = logs.OrderByDescending(x => x.Id).Take(1000).ToList();
        }

        public override List<LogEvent> GetLogs(LogSearchModel searchModel, int pageSize, out int resultsCount)
        {
            var events = logs.AsEnumerable();

            events = searchModel.PersianTime.HasValue()
                ? events.Where(x => x.PersianTime.Contains(searchModel.PersianTime))
                : events;

            events = searchModel.LogLevel.HasValue
                ? events.Where(x => x.LogLevel == searchModel.LogLevel)
                : events;

            resultsCount = events.Count();

            return events
                .OrderByDescending(x => x.Id)
                .Take(pageSize)
                .ToList();
        }
    }
}
