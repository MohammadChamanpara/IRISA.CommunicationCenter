using System;
namespace IRISA.Loggers
{
    public class IrisaException : Exception
	{
		public IrisaException(string message) : base(message)
		{
		}
	}
}
