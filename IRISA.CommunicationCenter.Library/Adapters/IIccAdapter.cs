using IRISA.CommunicationCenter.Library.Models;
using IRISA.Loggers;
using System;
namespace IRISA.CommunicationCenter.Library.Adapters
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
		void Send(IccTelegram iccTelegram);
		void Start(ILogger EventLogger);
		void Stop();
		void AwakeTimers();
	}
}
