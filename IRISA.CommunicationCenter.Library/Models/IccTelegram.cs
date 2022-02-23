using System;
using System.Collections.Generic;
using System.Globalization;

namespace IRISA.CommunicationCenter.Library.Models
{
    public class IccTelegram
    {
        public long TransferId { get; set; }
        public int TelegramId { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }

        public List<string> Body = new List<string>();
        public DateTime SendTime { get; set; }
        public bool Sent { get; set; }
        public bool Dropped { get; set; }
        public string DropReason { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public string BodyString => GetBodyString(',');
        public string GetBodyString(char bodySeparator)
        {
            string text = "";
            foreach (string current in Body)
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
                Body = new List<string>();
            }
            else
            {
                Body = new List<string>(bodyString.Split(new char[]
                {
                    separator
                }));
            }
        }
        public string PersianSendTime
        {
            get
            {
                var date = SendTime;
                string pDate = "";
                if (date != null)
                {
                    var Pc = new PersianCalendar();
                    int year = Pc.GetYear(date);
                    int month = Pc.GetMonth(date);
                    int day = Pc.GetDayOfMonth(date);
                    pDate = string.Format("{0:0000}/{1:00}/{2:00}-{3:00}:{4:00}:{5:00}", year, month, day, date.Hour, date.Minute, date.Second);
                }
                return pDate;
            }
        }
        public string PersianReceiveTime
        {
            get
            {
                var date = ReceiveTime;
                string pDate = "";
                if (date != null)
                {
                    var Pc = new PersianCalendar();
                    int year = Pc.GetYear(date.Value);
                    int month = Pc.GetMonth(date.Value);
                    int day = Pc.GetDayOfMonth(date.Value);
                    pDate = string.Format("{0:0000}/{1:00}/{2:00}-{3:00}:{4:00}:{5:00}", year, month, day, date.Value.Hour, date.Value.Minute, date.Value.Second);
                }
                return pDate;
            }
        }
        public bool IsReadyToSend => Sent == false && Dropped == false;
    }
}
