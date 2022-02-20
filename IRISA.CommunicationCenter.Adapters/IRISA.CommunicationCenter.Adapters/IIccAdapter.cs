using IRISA.Log;
using System;
namespace IRISA.CommunicationCenter.Adapters
{
	public interface IIccAdapter
	{
		event ReceiveEventHandler Receive;
		event EventHandler<AdapterConnectionChangedEventArgs> ConnectionChanged;
		event EventHandler<SendCompletedEventArgs> SendCompleted;

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
		void SendTelegram(IccTelegram iccTelegram);
		void Start(IrisaEventLogger EventLogger);
		void Stop();
		void AwakeTimers();
	}
}
