using System;

namespace IRISA.Loggers
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
