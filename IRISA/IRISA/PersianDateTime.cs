using System;
using System.Globalization;
using System.Linq;
namespace IRISA
{
	public class PersianDateTime
	{
		private DateTime dateTime = default(DateTime);
		public string DateSparator
		{
			get;
			set;
		}
		public string PersianDateTimeString
		{
			get
			{
				string result;
				try
				{
					PersianCalendar persianCalendar = new PersianCalendar();
					result = string.Format("{0}\u00a0{1:00}:{2:00}:{3:00}", new object[]
					{
						this.PersianDateString,
						persianCalendar.GetHour(this.dateTime),
						persianCalendar.GetMinute(this.dateTime),
						persianCalendar.GetSecond(this.dateTime)
					});
				}
				catch
				{
					result = "";
				}
				return result;
			}
		}
		public string PersianDateString
		{
			get
			{
				string result;
				try
				{
					PersianCalendar persianCalendar = new PersianCalendar();
					result = string.Format("{0:0000}{3}{1:00}{3}{2:00}", new object[]
					{
						persianCalendar.GetYear(this.dateTime),
						persianCalendar.GetMonth(this.dateTime),
						persianCalendar.GetDayOfMonth(this.dateTime),
						this.DateSparator
					});
				}
				catch
				{
					result = "";
				}
				return result;
			}
		}
		public PersianDateTime(DateTime dateTime)
		{
			this.dateTime = dateTime;
		}
		public PersianDateTime(string persianDate)
		{
			this.DateSparator = "/";
			if (!persianDate.Contains(' ') && persianDate.Length == 10)
			{
				PersianCalendar persianCalendar = new PersianCalendar();
				int year = int.Parse(persianDate.Substring(0, 4));
				int month = int.Parse(persianDate.Substring(5, 2));
				int day = int.Parse(persianDate.Substring(8, 2));
				this.dateTime = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
			}
		}
		public DateTime ToDateTime()
		{
			return this.dateTime;
		}
	}
}
