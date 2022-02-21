using IRISA.CommunicationCenter.Core.Model;
using IRISA.Loggers;
using IRISA.Model;
using System;
using System.Linq;

namespace IRISA.CommunicationCenter.Core.Loggers
{
    public class OracleDBLogger : BaseLogger
    {
        private DLLSettings<OracleDBLogger> dllSettings = new DLLSettings<OracleDBLogger>();

        private string ConnectionString
        {
            get
            {
                return this.dllSettings.FindConnectionString();
            }
        }
        public EntityBusiness<Entities, IccEvent> IccEvents
        {
            get
            {
                return new EntityBusiness<Entities, IccEvent>(new Entities(this.ConnectionString));
            }
        }
        protected override void Log(string eventText, EventType eventType)
        {
            if (IccEvents.Connected)
            {
                IccEvents.Create(new IccEvent
                {
                    TEXT = eventText,
                    TIME = DateTime.Now,
                    TYPE = eventType.ToPersian()
                });
            }
        }

        public override IQueryable<LogEvent> GetLogs()
        {
            return IccEvents.GetAll().Select(x=>new LogEvent() { });
        }
    }
}
