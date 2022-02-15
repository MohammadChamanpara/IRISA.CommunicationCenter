using System;
namespace IRISA.CommunicationCenter.Adapters
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
			this.IccTelegram = iccTelegram;
			this.Successful = successful;
			this.FailException = failException;
		}
	}
}
