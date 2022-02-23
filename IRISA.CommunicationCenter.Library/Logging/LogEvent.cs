using IRISA.CommunicationCenter.Library.Models;
using System;

namespace IRISA.CommunicationCenter.Library.Logging
{
    public class LogEvent
    {
        public int Id { get; set; }
        public EventType Type { get; set; }
        public string PersianType => Type.ToPersian();
        public DateTime Time { get; set; }
        public string PersianTime => Time.ToPersianDateTime();
        public string Text { get; set; }
    }
}
