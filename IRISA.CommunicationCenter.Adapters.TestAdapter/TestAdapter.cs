using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IRISA.CommunicationCenter.Adapters.TestAdapter
{
    namespace IRISA.CommunicationCenter.Adapters
    {
        public class TestAdapter : BaseAdapter<TestAdapter>
        {
            [DisplayName("تاخیر در ارسال - میلی ثانیه")]
            public int DelayInSend
            {
                get
                {
                    return _dllSettings.FindIntValue("DelayInSend", 0);
                }
                set
                {
                    _dllSettings.SaveSetting("DelayInSend", value);
                }
            }

            public override string Type => nameof(TestAdapter);

            protected override void SendTelegram(IccTelegram iccTelegram)
            {
                Thread.Sleep(DelayInSend);
            }

            protected override void ReceiveTimer_DoWork()
            {
                int id = Name == "Behnam" ? 1000 : 2000;

                var iccTelegram = new IccTelegram()
                {
                    Source = Name,
                    Body = new List<string>() { "A", "B" },
                    TelegramId = Name == "Behnam" ? 1 : 2,
                    TransferId = id++,
                    SendTime = DateTime.Now.AddMinutes(-10),
                };
                try
                {
                    iccTelegram.Destination = _telegramDefinitions.Find(iccTelegram).Destination;
                    OnTelegramReceived(new TelegramReceivedEventArgs(iccTelegram, true, null));
                }
                catch (Exception exception)
                {
                    OnTelegramReceived(new TelegramReceivedEventArgs(iccTelegram, false, exception));
                }

                var iccTelegram2 = new IccTelegram()
                {
                    Source = Name,
                    TelegramId = 3,
                    TransferId = id++,
                    SendTime = DateTime.Now.AddMinutes(-10)
                };
                try
                {
                    iccTelegram2.Destination = _telegramDefinitions.Find(iccTelegram2).Destination;
                    OnTelegramReceived(new TelegramReceivedEventArgs(iccTelegram2, true, null));
                }
                catch (Exception exception)
                {
                    OnTelegramReceived(new TelegramReceivedEventArgs(iccTelegram2, false, exception));
                }
            }

            protected override bool CheckConnection()
            {
                return DateTime.Now.Second % 20 != 0;
            }
        }
    }
}
