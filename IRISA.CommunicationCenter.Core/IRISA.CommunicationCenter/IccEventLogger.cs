using IRISA.CommunicationCenter.Model;
using IRISA.Log;
using IRISA.Model;
using System;
namespace IRISA.CommunicationCenter
{
	public class IccEventLogger : IrisaEventLogger
	{
		private DLLSettings<IccEventLogger> dllSettings = new DLLSettings<IccEventLogger>();
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
		protected override void LogEventInDB(string eventText, string eventType, string stackTrace)
		{
			if (this.IccEvents.Connected)
			{
				this.IccEvents.Create(new IccEvent
				{
					TEXT = eventText,
					TIME = DateTime.Now,
					TYPE = eventType,
					STACK_TRACE = stackTrace
				});
			}
		}
	}
}
