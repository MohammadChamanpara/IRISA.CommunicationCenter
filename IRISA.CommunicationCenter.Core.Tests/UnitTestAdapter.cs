using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Core.Tests
{
    internal class UnitTestAdapter : IIccAdapter
    {
        readonly string _name;
        public UnitTestAdapter(string name)
        {
            _name = name;
        }

        public string Name => _name;

        public string PersianDescription => Name;

        public bool Connected => true;

        public event Action<ReceiveCompletedEventArgs> TelegramReceiveCompleted;
        public event Action<SendCompletedEventArgs> TelegramSendCompleted;

        public List<IccTelegram> SentTelegrams = new List<IccTelegram>();
        public void Send(IccTelegram iccTelegram)
        {
            SentTelegrams.Add(iccTelegram);
            TelegramSendCompleted?.Invoke(new SendCompletedEventArgs(iccTelegram:iccTelegram, successful: true));
        }

        public void Start(ILogger EventLogger, ITelegramDefinitions telegramDefinitions) { }

        public void Stop() { }

        public void SendTelegramToIcc(IccTelegram iccTelegram)
        {
            TelegramReceiveCompleted?.Invoke(
                new ReceiveCompletedEventArgs(iccTelegram: iccTelegram, successful: true, failException: null));
        }
    }
}
