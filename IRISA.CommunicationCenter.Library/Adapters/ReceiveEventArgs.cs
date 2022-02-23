using IRISA.CommunicationCenter.Library.Models;
using System;
namespace IRISA.CommunicationCenter.Library.Adapters
{
    public class ReceiveEventArgs
	{
		public IccTelegram IccTelegram
		{
			get;
			set;
		}
		public bool Successful
		{
			get;
			set;
		}
		public Exception FailException
		{
			get;
			set;
		}
		public ReceiveEventArgs(IccTelegram iccTelegram, bool successful, Exception failException)
		{
			IccTelegram = iccTelegram;
			Successful = successful;
			FailException = failException;
		}
	}
}
