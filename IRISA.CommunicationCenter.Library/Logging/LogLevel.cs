namespace IRISA.CommunicationCenter.Library.Logging
{
    public enum LogLevel
    {
        Debug,
        Information,
        Warning,
        Error
    }

    public static class LogLevelExtensions
    {
        public static string ToPersian(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return "رفع عیب";
                case LogLevel.Information:
                    return "اطلاعات";
                case LogLevel.Warning:
                    return "هشدار";
                case LogLevel.Error:
                    return "خطا";
                default:
                    return "نا مشخص";
            }
        }
    }
}
