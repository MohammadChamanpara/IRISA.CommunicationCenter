using IRISA.CommunicationCenter.Library.Extensions;
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
        public string BodyString => GetBodyAsString(',');
        public string GetBodyAsString(char bodySeparator)
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
        public void SetBodyFromString(string bodyString, char separator)
        {
            Body = bodyString.HasValue()
                ? new List<string>(bodyString.Split(separator))
                : new List<string>();
        }
        public string PersianSendTime => SendTime.ToPersianDateTime();
        public string PersianReceiveTime => ReceiveTime?.ToPersianDateTime();
        public bool IsReadyToSend => Sent == false && Dropped == false;

        public void SetAsReadyToSend()
        {
            Sent = false;
            Dropped = false;
            DropReason = null;
        }

        public void SetAsDropped(string dropReason)
        {
            Sent = false;
            Dropped = true;
            DropReason = dropReason;
        }

        public void SetAsSent()
        {
            Dropped = false;
            DropReason = null;
            Sent = true;
            ReceiveTime = DateTime.Now;
        }
    }
}
