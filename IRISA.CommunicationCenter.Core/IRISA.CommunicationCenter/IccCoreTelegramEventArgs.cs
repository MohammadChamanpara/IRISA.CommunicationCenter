using System;
namespace IRISA.CommunicationCenter
{
	public class IccCoreTelegramEventArgs
	{
		public IccTelegram IccTelegram;
		public IccCoreTelegramEventArgs(IccTelegram iccTelegram)
		{
			this.IccTelegram = iccTelegram;
		}
	}
}
