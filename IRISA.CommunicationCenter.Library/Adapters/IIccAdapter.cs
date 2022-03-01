using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using System;
namespace IRISA.CommunicationCenter.Library.Adapters
{
    public interface IIccAdapter
    {
        event Action<TelegramReceivedEventArgs> TelegramReceived;
        event Action<IIccAdapter> ConnectionChanged;
        event Action<SendCompletedEventArgs> SendCompleted;

        string Name { get; }
        string PersianDescription { get; }
        bool Connected { get; }
        void Send(IccTelegram iccTelegram);
        void Start(ILogger EventLogger, ITelegramDefinitions telegramDefinitions);
        void Stop();
    }
}
