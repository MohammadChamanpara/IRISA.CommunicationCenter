using System;
namespace IRISA.CommunicationCenter.Library.Loggers
{
    public class IrisaException : Exception
    {
        public IrisaException(string message) : base(message)
        {
        }

        public IrisaException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static IrisaException Create(string messageFormat, params object[] parameters)
        {
            return new IrisaException(string.Format(messageFormat, parameters));
        }

        public static IrisaException Create(Exception innerException, string message)
        {
            return new IrisaException(message, innerException);
        }
    }

    public static class ExceptionExtensions
    {
        public static string InnerExceptionsMessage(this Exception exception)
        {
            string text = exception.Message;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                text = text + "\r\nInner Exception: " + exception.Message;
            }
            return text;
        }
    }
}
