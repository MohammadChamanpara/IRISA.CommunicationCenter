using IRISA.Log;
using System;
namespace IRISA.CommunicationCenter.Adapters
{
	public interface IIccAdapter
	{
		event ReceiveEventHandler Receive;
		event ConnectionChangedEventHandler ConnectionChanged;
		string Name
		{
			get;
		}
		string PersianDescription
		{
			get;
		}
		bool Connected
		{
			get;
		}
		void Send(IccTelegram iccTelegram);
		void Start(IrisaEventLogger EventLogger);
		void Stop();
		void AwakeTimers();
	}
}
