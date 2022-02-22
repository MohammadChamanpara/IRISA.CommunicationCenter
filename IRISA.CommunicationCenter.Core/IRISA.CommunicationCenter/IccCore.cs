using IRISA.CommunicationCenter.Adapters;
using IRISA.Loggers;
using IRISA.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IRISA.CommunicationCenter.Core
{
    public class IccCore
    {
        public delegate void IccCoreTelegramEventHandler(IccCoreTelegramEventArgs e);
        public List<IIccAdapter> connectedClients;
        private IrisaBackgroundTimer sendTimer;
        private IrisaBackgroundTimer activatorTimer;
        private DLLSettings<IccCore> dllSettings;
        public event IccCoreTelegramEventHandler TelegramQueued;
        public event IccCoreTelegramEventHandler TelegramSent;
        public event IccCoreTelegramEventHandler TelegramDropped;
        private readonly object sendLocker = new object();
        private readonly object receiveLocker = new object();

        public readonly ILogger Logger;
        private readonly IInProcessTelegrams InProcessTelegrams;
        public readonly IIccQueue IccQueue;

        public IccCore(IInProcessTelegrams inProcessTelegrams, ILogger logger, IIccQueue iccQueue)
        {
            InProcessTelegrams = inProcessTelegrams;
            Logger = logger;
            IccQueue = iccQueue;
        }

        [DisplayName("شرح فارسی هسته مرکزی سیستم ارتباط")]
        public string PersianDescription
        {
            get
            {
                return dllSettings.FindStringValue("PersianDescription", "هسته مرکزی سیستم ارتباط");
            }
            set
            {
                dllSettings.SaveSetting("PersianDescription", value);
            }
        }
        [DisplayName("شرح فارسی پروسه فعال ساز")]
        public string ActivatorTimerPersianDescription
        {
            get
            {
                return dllSettings.FindStringValue("ActivatorTimerPersianDescription", "پروسه فعال ساز");
            }
            set
            {
                dllSettings.SaveSetting("ActivatorTimerPersianDescription", value);
            }
        }
        [DisplayName("شرح فارسی پروسه ارسال تلگرام")]
        public string SendTimerPersianDescription
        {
            get
            {
                return dllSettings.FindStringValue("SendTimerPersianDescription", "پروسه ارسال تلگرام");
            }
            set
            {
                dllSettings.SaveSetting("SendTimerPersianDescription", value);
            }
        }

        [DisplayName("کاراکتر جدا کننده بین مقصد های تلگرام")]
        public char DestinationSeparator
        {
            get
            {
                return dllSettings.FindCharacterValue("DestinationSeparator", ',');
            }
            set
            {
                dllSettings.SaveSetting("DestinationSeparator", value);
            }
        }
        [DisplayName("دوره زمانی ارسال تلگرام بر حسب میلی ثانیه")]
        public int SendTimerInterval
        {
            get
            {
                return dllSettings.FindIntValue("sendTimerInterval", 2000);
            }
            set
            {
                dllSettings.SaveSetting("sendTimerInterval", value);
            }
        }
        [DisplayName("دوره زمانی فعال نمودن پروسه های متوقف شده بر حسب میلی ثانیه")]
        public int ActivatorTimerInterval
        {
            get
            {
                return dllSettings.FindIntValue("activatorTimerInterval", 20000);
            }
            set
            {
                dllSettings.SaveSetting("activatorTimerInterval", value);
            }
        }
        [Category("Information"), DisplayName("وضعیت اجرای پروسه")]
        public bool Started
        {
            get;
            private set;
        }
        [Category("Information"), DisplayName("نوع فایل")]
        public string FileAssembly
        {
            get
            {
                return dllSettings.Assembly.AssemblyName();
            }
        }
        [Category("Information"), DisplayName("ورژن برنامه")]
        public string FileAssemblyVersion
        {
            get
            {
                return dllSettings.Assembly.AssemblyVersion();
            }
        }
        [Category("Information"), DisplayName("آدرس فایل")]
        public string FileAddress
        {
            get
            {
                return dllSettings.Assembly.Location;
            }
        }
        private void sendTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (sendLocker)
            {
                List<IccTelegram> telegrams = IccQueue.GetTelegramsToSend();

                telegrams = InProcessTelegrams.RemoveFrom(telegrams);

                InProcessTelegrams.AddRange(telegrams);

                SendTelegrams(telegrams, connectedClients);
            }
        }


        public void SendTelegrams(List<IccTelegram> telegrams, List<IIccAdapter> adapters)
        {
            if (telegrams.Count == 0)
                return;

            telegrams = telegrams.OrderBy(x => x.SendTime).ToList();

            foreach (IccTelegram telegram in telegrams)
            {
                try
                {
                    IIccAdapter iccAdapter = GetDestinationAdapter(adapters, telegram.Destination);

                    if (!iccAdapter.Connected)
                    {
                        InProcessTelegrams.Remove(telegram);
                        continue;
                    }

                    ValidateTelegram(telegram);

                    iccAdapter.Send(telegram);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        private void UpdateSentTelegram(IccTelegram iccTelegram)
        {
            InProcessTelegrams.Remove(iccTelegram);
            iccTelegram.Dropped = false;
            iccTelegram.DropReason = null;
            iccTelegram.ReceiveTime = new DateTime?(DateTime.Now);
            iccTelegram.Sent = true;

            IccQueue.Edit(iccTelegram);

            Logger.LogSuccess("تلگرام با شناسه رکورد {0} موفقیت آمیز به مقصد ارسال شد.", new object[]
            {
                    iccTelegram.TransferId
            });

            TelegramSent?.Invoke(new IccCoreTelegramEventArgs(iccTelegram));
        }

        public IIccAdapter GetDestinationAdapter(List<IIccAdapter> adapters, string destination)
        {
            return
                adapters
                    .Where(x => x.Name == destination)
                    .Single();
        }

        private void activatorTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                sendTimer.Awake();
                if (connectedClients != null)
                {
                    foreach (IIccAdapter current in connectedClients)
                    {
                        current.AwakeTimers();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogWarning("بروز خطا هنگام فعال سازی پروسه ها", new object[0]);
                Logger.LogException(exception);
            }
        }
        private void client_OnReceive(ReceiveEventArgs e)
        {
            lock (receiveLocker)
            {
                try
                {
                    if (!e.Successful)
                    {
                        DropTelegram(e.IccTelegram, e.FailException, false);
                    }
                    else
                    {
                        foreach (IccTelegram current in DuplicateTelegramByDestination(e.IccTelegram))
                        {
                            QueueTelegram(current);
                        }
                    }
                }
                catch (Exception dropException)
                {
                    DropTelegram(e.IccTelegram, dropException, false);
                }
            }
        }
        public void Start()
        {
            try
            {
                connectedClients = new List<IIccAdapter>();
                dllSettings = new DLLSettings<IccCore>();
                Logger.LogInfo("اجرای {0} آغاز شد.", new object[]
                {
                        PersianDescription
                });
                LoadClients();
                if (sendTimer != null)
                {
                    sendTimer.Stop();
                }
                sendTimer = new IrisaBackgroundTimer();
                sendTimer.Interval = SendTimerInterval;
                sendTimer.DoWork += new DoWorkEventHandler(sendTimer_DoWork);
                sendTimer.PersianDescription = SendTimerPersianDescription + " در " + PersianDescription;
                sendTimer.EventLogger = Logger;
                sendTimer.Start();
                if (activatorTimer != null)
                {
                    activatorTimer.Stop();
                }
                activatorTimer = new IrisaBackgroundTimer();
                activatorTimer.Interval = ActivatorTimerInterval;
                activatorTimer.DoWork += new DoWorkEventHandler(activatorTimer_DoWork);
                activatorTimer.PersianDescription = ActivatorTimerPersianDescription + " در " + PersianDescription;
                activatorTimer.EventLogger = Logger;
                activatorTimer.Start();
                Started = true;
            }
            catch (Exception exception)
            {
                Started = false;
                Logger.LogException(exception);
            }
        }
        public void Stop()
        {
            if (sendTimer != null)
            {
                sendTimer.Stop();
            }
            if (activatorTimer != null)
            {
                activatorTimer.Stop();
            }
            foreach (IIccAdapter current in connectedClients)
            {
                current.Stop();
            }
            if (Started)
            {
                Logger.LogInfo("اجرای {0} خاتمه یافت.", new object[]
                {
                    PersianDescription
                });
            }
            Started = false;
        }

        private void LoadClients()
        {
            connectedClients = HelperMethods.LoadPlugins<IIccAdapter>(@"C:\Projects\ICC\IRISA.CommunicationCenter.Adapters.TestAdapter\bin\Debug");

            if (connectedClients.Count == 0)
            {
                Logger.LogWarning("کلاینتی برای اتصال یافت نشد.", new object[0]);
            }
            foreach (IIccAdapter adapter in connectedClients)
            {
                try
                {
                    adapter.Receive += new ReceiveEventHandler(client_OnReceive);
                    adapter.SendCompleted += Adapter_SendCompleted;
                    adapter.Start(Logger);
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception);
                }
            }
        }

        private void Adapter_SendCompleted(object sender, SendCompletedEventArgs e)
        {
            if (e.Successful)
                UpdateSentTelegram(e.IccTelegram);
            else
                DropTelegram(e.IccTelegram, e.FailException, true);
        }

        private void ValidateTelegram(IccTelegram iccTelegram)
        {
            if (!iccTelegram.Destination.HasValue())
            {
                throw HelperMethods.CreateException("مقصد تلگرام مشخص نشده است.", new object[0]);
            }

            var destination = connectedClients
                    .Where(c => c.Name == iccTelegram.Destination);

            if (destination.Count() == 0)
            {
                throw HelperMethods.CreateException("مقصد مشخص شده وجود ندارد.", new object[0]);
            }

            if (destination.Count() > 1)
            {
                throw HelperMethods.CreateException("چند مقصد با نام داده شده وجود دارد.", new object[0]);
            }
        }
        private void DropTelegram(IccTelegram iccTelegram, Exception dropException, bool existingRecord)
        {
            try
            {
                if (!(dropException is IrisaException))
                {
                    Logger.LogException(dropException);
                }
                iccTelegram.Sent = false;
                iccTelegram.Dropped = true;
                iccTelegram.DropReason = dropException.InnerExceptionsMessage();

                if (existingRecord)
                {
                    IccQueue.Edit(iccTelegram);
                }
                else
                {
                    IccQueue.Add(iccTelegram);
                }

                Logger.LogInfo("تلگرام با شناسه {0} حذف شد.", new object[]
                {
                        iccTelegram.TransferId
                });

                if (TelegramDropped != null)
                {
                    TelegramDropped(new IccCoreTelegramEventArgs(iccTelegram));
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
            finally
            {
                InProcessTelegrams.Remove(iccTelegram);
            }
        }
        private void QueueTelegram(IccTelegram iccTelegram)
        {
            bool flag = false;
            try
            {
                iccTelegram.Dropped = false;
                iccTelegram.DropReason = null;
                iccTelegram.Sent = false;

                IccQueue.Add(iccTelegram);
                Logger.LogSuccess("تلگرام با شناسه رکورد {0} در صف ارسال قرار گرفت.", new object[]
                {
                    iccTelegram.TransferId
                });
                TelegramQueued?.Invoke(new IccCoreTelegramEventArgs(iccTelegram));
            }
            catch (Exception ex)
            {
                if (!flag)
                {
                    DropTelegram(iccTelegram, ex, false);
                }
                else
                {
                    Logger.LogException(ex);
                }
            }
        }
        private List<IccTelegram> DuplicateTelegramByDestination(IccTelegram iccTelegram)
        {
            List<IccTelegram> list = new List<IccTelegram>();
            string[] array = iccTelegram.Destination.Split(new char[]
            {
                DestinationSeparator
            });
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                if (!(text.Trim() == ""))
                {
                    IccTelegram item = new IccTelegram
                    {
                        Destination = text,
                        TelegramId = iccTelegram.TelegramId,
                        SendTime = iccTelegram.SendTime,
                        Source = iccTelegram.Source,
                        Body = new List<string>(iccTelegram.Body)
                    };
                    list.Add(item);
                }
            }
            return list;
        }
    }
}
