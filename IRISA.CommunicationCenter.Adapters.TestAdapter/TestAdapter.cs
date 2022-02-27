using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
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
                    return dllSettings.FindIntValue("DelayInSend", 0);
                }
                set
                {
                    dllSettings.SaveSetting("DelayInSend", value);
                }
            }

            public override string Type => "TestAdapter";

            private readonly object sendLocker = new object();
            protected override void SendTelegram(IccTelegram iccTelegram)
            {
                lock (sendLocker)
                {
                    Thread.Sleep(DelayInSend);
                    if (iccTelegram.TransferId % 3 == 0)
                        throw new Exception("error");
                    File.AppendAllText($@"c:\icc\{Name}.txt", iccTelegram.TelegramId.ToString() + "\r\n");
                }
            }

            public override void Start(ILogger eventLogger)
            {
                base.Start(eventLogger);
                Task.Run(() => KeepReceiving());
            }

            private async Task KeepReceiving()
            {
                int id = Name == "Behnam" ? 1000 : 2000;
                while (Started)
                {
                    try
                    {
                        if (DateTime.Now.Second % 20 == 0 && Name == "Behnam")
                        {
                            Connected = false;
                            throw new Exception("my disconnection------------");
                        }

                        OnReceive
                        (
                            new ReceiveEventArgs
                            (
                                new IccTelegram()
                                {
                                    Source = Name,
                                    Destination = Name == "Behnam" ? "Mamad" : "Behnam",
                                    Body = new List<string>() { "A", "B" },
                                    TelegramId = Name == "Behnam" ? 1 : 2,
                                    TransferId = id++,
                                    SendTime = DateTime.Now
                                }
                                , true
                                , null
                            )
                        );
                    }
                    catch (Exception exception)
                    {
                        Logger.LogException(exception, $"Receiving telegrams in test adapter {Name} failed");
                    }
                    finally
                    {
                        await Task.Delay(1000);
                    }
                }
            }

            protected override bool CheckConnection()
            {
                return DateTime.Now.Second % 20 == 0 ? false : true;
            }
        }
    }
}
