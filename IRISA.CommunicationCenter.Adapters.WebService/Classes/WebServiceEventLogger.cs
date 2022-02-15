using IRISA.Log;
using System;
using System.Web;
namespace IRISA.CommunicationCenter
{
	public class WebServiceEventLogger : IrisaEventLogger
	{
		protected override bool SingleFilePerDay
		{
			get
			{
				return base.SingleFilePerDay;
			}
		}
		protected override string LogFileAddress
		{
			get
			{
				return HttpContext.Current.Request.PhysicalApplicationPath.ToString() + "\\Events\\Events.txt";
			}
		}
		protected override void LogEventInDB(string eventText, string eventType, string stackTrace)
		{
		}
	}
}
