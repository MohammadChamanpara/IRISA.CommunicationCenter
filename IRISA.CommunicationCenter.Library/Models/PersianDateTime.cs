using System;
using System.Globalization;
using System.Linq;
namespace IRISA.CommunicationCenter.Library.Models
{
    public class PersianDateTime
    {
        private DateTime dateTime = default;
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
                        PersianDateString,
                        persianCalendar.GetHour(dateTime),
                        persianCalendar.GetMinute(dateTime),
                        persianCalendar.GetSecond(dateTime)
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
                        persianCalendar.GetYear(dateTime),
                        persianCalendar.GetMonth(dateTime),
                        persianCalendar.GetDayOfMonth(dateTime),
                        DateSparator
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
            DateSparator = "/";
            if (!persianDate.Contains(' ') && persianDate.Length == 10)
            {
                PersianCalendar persianCalendar = new PersianCalendar();
                int year = int.Parse(persianDate.Substring(0, 4));
                int month = int.Parse(persianDate.Substring(5, 2));
                int day = int.Parse(persianDate.Substring(8, 2));
                dateTime = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            }
        }
        public DateTime ToDateTime()
        {
            return dateTime;
        }
    }

    public static class PersianDateTimeExtensions
    {
        public static string ToPersianDateTime(this DateTime dateTime, string separator = "/")
        {
            return new PersianDateTime(dateTime)
            {
                DateSparator = separator
            }.PersianDateTimeString;
        }
        public static string ToPersianDate(this DateTime dateTime, string separator = "/")
        {
            return new PersianDateTime(dateTime)
            {
                DateSparator = separator
            }.PersianDateString;
        }
        public static DateTime ToEnglishDate(this string persianDate)
        {
            int year;
            int month;
            int day;
            PersianCalendar persianCalendar = new PersianCalendar();
            try
            {
                year = int.Parse(persianDate.Substring(0, 4));
                month = int.Parse(persianDate.Substring(5, 2));
                day = int.Parse(persianDate.Substring(8, 2));
            }
            catch (Exception)
            {
                throw new Exception("تاریخ به اشتباه وارد شده است");
            }
            if (!(year < 1404 && year > 1385))
            {
                string error = "سال باید بین 1385 تا 1404 باشد";
                throw new Exception(error);
            }

            if (!(month >= 1 && month <= 12))
            {
                string error = "ماه باید بین 1 تا 12 باشد";
                throw new Exception(error);
            }

            if (!(day >= 1 && day <= 31))
            {
                string error = "روز باید بین 1 تا 31 باشد";
                throw new Exception(error);
            }

            return persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
    }
}
