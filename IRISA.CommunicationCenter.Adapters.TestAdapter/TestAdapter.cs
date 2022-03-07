using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace IRISA.CommunicationCenter.Adapters.TestAdapter
{
    namespace IRISA.CommunicationCenter.Adapters
    {
        public class TestAdapter : BaseAdapter<TestAdapter>
        {
            [DisplayName("تاخیر در ارسال - میلی ثانیه")]
            [Category("Operation")]
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
                File.AppendAllText($"c:\\icc2\\{Name}.txt", $"{DateTime.Now}: {iccTelegram.TransferId}\r\n");
            }

            protected override void ReceiveTimer_DoWork()
            {
                var iccTelegram = new IccTelegram()
                {
                    Source = Name,
                    Body = new List<string>() { "A", "B" },
                    TelegramId = Name == "Behnam" ? 1 : 2,
                    SendTime = DateTime.Now
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

                if (Name == "Mamad")
                {
                    var iccTelegram2 = new IccTelegram()
                    {
                        Source = Name,
                        TelegramId = 3,
                        SendTime = DateTime.Now
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
            }

            protected override bool CheckConnection()
            {
                return DateTime.Now.Second % 20 != 0;
            }
        }
    }
}
