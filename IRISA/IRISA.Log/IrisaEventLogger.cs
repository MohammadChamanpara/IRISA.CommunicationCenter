using System;
using System.IO;
namespace IRISA.Log
{
	public abstract class IrisaEventLogger
	{
		protected enum EventType
		{
			Success,
			Info,
			Warning,
			Error,
			Exception,
			Debug
		}
		public delegate void EventLoggerEventHandler();
		private object ThisCode = 0;
		public event IrisaEventLogger.EventLoggerEventHandler EventLogged;
		protected virtual bool SingleFilePerDay
		{
			get
			{
				return true;
			}
		}
		protected virtual string LogFileAddress
		{
			get
			{
				return "Events\\Events.txt";
			}
		}
		public bool DbLoggerEnabled
		{
			get;
			set;
		}
		public IrisaEventLogger()
		{
			this.DbLoggerEnabled = true;
		}
		public void LogSuccess(string successText, params object[] parameters)
		{
			this.CallLogFunctions(successText, IrisaEventLogger.EventType.Success, null, parameters);
		}
		public void LogInfo(string infoText, params object[] parameters)
		{
			this.CallLogFunctions(infoText, IrisaEventLogger.EventType.Info, null, parameters);
		}
		public void LogWarning(string warningText, params object[] parameters)
		{
			this.CallLogFunctions(warningText, IrisaEventLogger.EventType.Warning, null, parameters);
		}
		public void LogError(string errorText, params object[] parameters)
		{
			this.CallLogFunctions(errorText, IrisaEventLogger.EventType.Error, null, parameters);
		}
		public void LogException(Exception exception)
		{
			this.CallLogFunctions(exception.InnerExceptionsMessage(), IrisaEventLogger.EventType.Exception, exception.StackTrace, new object[0]);
		}
		public void LogTest(string testText, params object[] parameters)
		{
			this.CallLogFunctions(testText, IrisaEventLogger.EventType.Debug, null, parameters);
		}
		private void CallLogFunctions(string eventText, IrisaEventLogger.EventType eventType, string stackTrace, params object[] parameters)
		{
			try
			{
				string eventTypeString = this.GetEventTypeString(eventType);
				eventText = string.Format(eventText, parameters);
				try
				{
					this.LogEventInFile(eventText, eventTypeString, stackTrace);
				}
				catch
				{
				}
				if (this.DbLoggerEnabled)
				{
					this.LogEventInDB(eventText, eventTypeString, stackTrace);
				}
				this.OnEventLogged();
			}
			catch
			{
			}
		}
		private string GetEventTypeString(IrisaEventLogger.EventType eventType)
		{
			string result;
			switch (eventType)
			{
			case IrisaEventLogger.EventType.Success:
				result = "موفقیت";
				break;
			case IrisaEventLogger.EventType.Info:
				result = "اطلاعات";
				break;
			case IrisaEventLogger.EventType.Warning:
				result = "هشدار";
				break;
			case IrisaEventLogger.EventType.Error:
				result = "اشکال";
				break;
			case IrisaEventLogger.EventType.Exception:
				result = "خطا";
				break;
			case IrisaEventLogger.EventType.Debug:
				result = "رفع عیب";
				break;
			default:
				result = "نا مشخص";
				break;
			}
			return result;
		}
		protected virtual void LogEventInFile(string eventText, string eventType, string stackTrace)
		{
			string text = string.Concat(new string[]
			{
				" English Time : ",
				DateTime.Now.ToString(),
				"\r\n Persian Time : ",
				DateTime.Now.ToPersianDateTime("/"),
				"\r\n Type : ",
				eventType,
				"\r\n Event : ",
				eventText,
				"\r\n"
			});
			if (stackTrace != null)
			{
				text = text + " StackTrace : " + stackTrace + "\r\n";
			}
			text += "_______________________________________________________________________________________\r\n\r\n";
			string text2 = this.LogFileAddress;
			if (this.SingleFilePerDay)
			{
				text2 = this.AddDayToLogFileName(text2);
			}
			string path = Path.GetFullPath(text2).Replace(Path.GetFileName(text2), "");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			File.AppendAllText(text2, text);
		}
		protected abstract void LogEventInDB(string eventText, string eventType, string stackTrace);
		protected virtual void OnEventLogged()
		{
			if (this.EventLogged != null)
			{
				this.EventLogged();
			}
		}
		protected string AddDayToLogFileName(string fileName)
		{
			return string.Format("{0}\\{1} {2}{3}", new object[]
			{
				Path.GetDirectoryName(fileName),
				Path.GetFileNameWithoutExtension(fileName),
				DateTime.Now.ToPersianDate("-"),
				Path.GetExtension(fileName)
			});
		}
	}
}
