using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using System;
namespace IRISA.CommunicationCenter.Library.Adapters
{
    public interface IIccAdapter
    {
        event Action<ReceiveCompletedEventArgs> TelegramReceiveCompleted;
        event Action<SendCompletedEventArgs> TelegramSendCompleted;

        string Name { get; }
        string PersianDescription { get; }
        bool Connected { get; }
        void Send(IccTelegram iccTelegram);
        void Start(ILogger EventLogger, ITelegramDefinitions telegramDefinitions);
        void Stop();
    }
}
