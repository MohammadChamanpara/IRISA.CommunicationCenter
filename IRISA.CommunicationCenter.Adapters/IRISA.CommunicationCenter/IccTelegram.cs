using System;
using System.Collections.Generic;
namespace IRISA.CommunicationCenter
{
	public class IccTelegram
	{
		public long TransferId
		{
			get;
			set;
		}
		public int TelegramId
		{
			get;
			set;
		}
		public string Source
		{
			get;
			set;
		}
		public string Destination
		{
			get;
			set;
		}
		public List<string> Body
		{
			get;
			set;
		}
		public DateTime SendTime
		{
			get;
			set;
		}
		public IccTelegram()
		{
			this.Body = new List<string>();
		}
		public string GetBodyString(char bodySeparator)
		{
			string text = "";
			foreach (string current in this.Body)
			{
				text = text + current + bodySeparator;
			}
			if (text.Length > 0)
			{
				text = text.Remove(text.Length - 1, 1);
			}
			return text;
		}
		public void SetBodyString(string bodyString, char separator)
		{
			if (bodyString == null)
			{
				this.Body = new List<string>();
			}
			else
			{
				this.Body = new List<string>(bodyString.Split(new char[]
				{
					separator
				}));
			}
		}
	}
}
