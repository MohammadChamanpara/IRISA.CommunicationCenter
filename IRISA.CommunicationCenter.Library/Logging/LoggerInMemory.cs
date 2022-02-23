using System;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public class LoggerInMemory : BaseLogger
    {
        private static List<LogEvent> logs = new List<LogEvent>();
        private static int id = 1;
        protected override void Log(string eventText, EventType eventType)
        {
            logs.Add(new LogEvent()
            {
                Id = id++,
                Time = DateTime.Now,
                Text = eventText,
                Type = eventType
            });

            if (logs.Count > 2000)
                logs = logs.OrderByDescending(x => x.Id).Take(1000).ToList();
        }
        public override IQueryable<LogEvent> GetLogs()
        {
            return logs.AsQueryable();
        }
    }
}
