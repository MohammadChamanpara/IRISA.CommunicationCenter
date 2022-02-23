using IRISA.CommunicationCenter.Library.Models;

namespace IRISA.CommunicationCenter.Core
{
    public class IccCoreTelegramEventArgs
	{
		public IccTelegram IccTelegram;
		public IccCoreTelegramEventArgs(IccTelegram iccTelegram)
		{
			IccTelegram = iccTelegram;
		}
	}
}
