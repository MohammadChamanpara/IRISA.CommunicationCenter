using System;
using System.Collections.Generic;

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
        public string PersianSendTime => SendTime.ToPersianDateTime();
        public string PersianReceiveTime => ReceiveTime?.ToPersianDateTime();
        public bool IsReadyToSend => Sent == false && Dropped == false;
    }
}
