using System;
namespace IRISA.CommunicationCenter.Library.Loggers
{
    public class IrisaException : Exception
    {
        public IrisaException(string message) : base(message)
        {
        }
        public static IrisaException Create(string messageFormat, params object[] parameters)
        {
            return new IrisaException(string.Format(messageFormat, parameters));
        }
    }
}
