namespace IRISA.CommunicationCenter.Library.Logging
{
    public enum EventType
    {
        Success,
        Info,
        Warning,
        Error,
        Exception,
        Debug
    }

    public static class EventTypeExtensions
    {
        public static string ToPersian(this EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Success:
                    return "موفقیت";
                case EventType.Info:
                    return "اطلاعات";
                case EventType.Warning:
                    return "هشدار";
                case EventType.Error:
                    return "اشکال";
                case EventType.Exception:
                    return "خطا";
                case EventType.Debug:
                    return "رفع عیب";
                default:
                    return "نا مشخص";
            }
        }
    }
}
