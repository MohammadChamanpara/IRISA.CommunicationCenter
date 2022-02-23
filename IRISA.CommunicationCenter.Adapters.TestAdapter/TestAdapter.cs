using IRISA.CommunicationCenter.Library.Models;
using IRISA.Loggers;
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
                    this.dllSettings.SaveSetting("DelayInSend", value);
                }
            }

            public override string Type => "TestAdapter";
            public bool Running = false;
            public override bool Connected => Running;

            private object sendLocker = new object();
            protected override void SendTelegram(IccTelegram iccTelegram)
            {
                lock (sendLocker)
                {
                    Thread.Sleep(DelayInSend);
                    File.AppendAllText($@"c:\icc\{Name}.txt", iccTelegram.TelegramId.ToString() + "\r\n");
                }
            }

            public override void Start(ILogger eventLogger)
            {
                base.Start(eventLogger);
                Running = true;
                Task.Run(() => KeepReceiving());
            }

            public override void Stop()
            {
                base.Stop();
                Running = false;
            }
            private async Task KeepReceiving()
            {
                int id = Name == "Behnam" ? 1000 : 2000;
                while (Running)
                {
                    try
                    {
                        this.OnReceive
                        (
                            new ReceiveEventArgs
                            (
                                new IccTelegram()
                                {
                                    Source = Name,
                                    //Destination = Name == "Behnam" ? "Mamad" : "Behnam",
                                    Destination = "Behnam",
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
                        eventLogger.LogException(exception);
                    }
                    finally
                    {
                        await Task.Delay(1000);
                    }
                }
            }

        }
    }
}
