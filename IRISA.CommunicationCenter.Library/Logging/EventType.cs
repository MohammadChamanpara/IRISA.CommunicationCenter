namespace IRISA.CommunicationCenter.Library.Logging
{
    public enum EventType
    {
        Debug,
        Information,
        Warning,
        Error,
        Exception
    }

    public static class EventTypeExtensions
    {
        public static string ToPersian(this EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Debug:
                    return "رفع عیب";
                case EventType.Information:
                    return "موفقیت";
                case EventType.Warning:
                    return "هشدار";
                case EventType.Error:
                    return "اشکال";
                case EventType.Exception:
                    return "خطا";
                default:
                    return "نا مشخص";
            }
        }
    }
}
