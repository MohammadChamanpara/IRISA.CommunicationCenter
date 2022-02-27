using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Core;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Loggers;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Settings;
using IRISA.CommunicationCenter.Library.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IRISA.CommunicationCenter.Core
{
    public class IccCore : IIccCore
    {
        public delegate void IccCoreTelegramEventHandler(IccCoreTelegramEventArgs e);
        public List<IIccAdapter> ConnectedAdapters { get; private set; }
        public IIccQueue IccQueue { get; set; }

        private IrisaBackgroundTimer sendTimer;
        private IrisaBackgroundTimer activatorTimer;
        private DLLSettings<IccCore> dllSettings;
        private readonly HashSet<long> inProcessTelegrams = new HashSet<long>();
        private readonly object sendLocker = new object();
        private readonly object receiveLocker = new object();
        private readonly ILogger Logger;

        public event IccCoreTelegramEventHandler TelegramQueued;
        public event IccCoreTelegramEventHandler TelegramSent;
        public event IccCoreTelegramEventHandler TelegramDropped;

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

        public IccCore(ILogger logger, IIccQueue iccQueue)
        {
            Logger = logger;
            IccQueue = iccQueue;
        }

        public static List<T> LoadAdapters<T>()
        {
            return LoadAdapters<T>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Adapters"));
        }

        public static List<T> LoadAdapters<T>(string directory)
        {
            List<T> result;
            try
            {
                List<T> list = new List<T>();
                if (!Directory.Exists(directory))
                {
                    throw IrisaException.Create("مسیر ذخیره آداپتور ها {0} موجود نیست.", new object[]
                    {
                        directory
                    });
                }
                Type typeFromHandle = typeof(T);
                string[] files = Directory.GetFiles(directory, "*.dll");
                for (int i = 0; i < files.Length; i++)
                {
                    string path = files[i];
                    Type[] types = Assembly.LoadFile(path).GetTypes();
                    for (int j = 0; j < types.Length; j++)
                    {
                        Type type = types[j];
                        if (typeFromHandle.IsAssignableFrom(type) && typeFromHandle != type && !type.ContainsGenericParameters)
                        {
                            T item = (T)((object)Activator.CreateInstance(type));
                            list.Add(item);
                        }
                    }
                }
                result = list;
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                if (exception is ReflectionTypeLoadException)
                {
                    ReflectionTypeLoadException typeLoadException = exception as ReflectionTypeLoadException;
                    if (typeLoadException?.LoaderExceptions.Count() > 0)
                    {
                        message = typeLoadException.LoaderExceptions.First<Exception>().Message;
                    }
                }
                throw IrisaException.Create("خطا هنگام لود کردن آداپتور ها. متن خطا : " + message);
            }
            return result;
        }

        private void SendTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (sendLocker)
            {
                SendTelegrams(IccQueue.GetTelegramsToSend(), ConnectedAdapters);
            }
        }

        public void SendTelegrams(List<IccTelegram> iccTelegrams, List<IIccAdapter> adapters)
        {
            iccTelegrams = SortSendingTelegrams(iccTelegrams);
            foreach (IccTelegram iccTelegram in iccTelegrams)
            {
                try
                {
                    if (inProcessTelegrams.Contains(iccTelegram.TransferId))
                        continue;

                    IIccAdapter destination = GetDestinationAdapter(adapters, iccTelegram.Destination);

                    if (!destination.Connected)
                        continue;

                    inProcessTelegrams.Add(iccTelegram.TransferId);

                    Logger.LogDebug($"تلگرام با شناسه {iccTelegram.TransferId} جهت ارسال به آداپتور {destination.PersianDescription} تحویل داده شد.");

                    destination.Send(iccTelegram);
                }
                catch (Exception exception)
                {
                    DropTelegram(iccTelegram, exception, true);
                }
            }
        }

        private List<IccTelegram> SortSendingTelegrams(List<IccTelegram> iccTelegrams)
        {
            return iccTelegrams
                .OrderBy(x => x.TransferId)
                .ToList();
        }

        public IIccAdapter GetDestinationAdapter(List<IIccAdapter> adapters, string destinationName)
        {
            if (!destinationName.HasValue())
            {
                throw IrisaException.Create("مقصد تلگرام مشخص نشده است.");
            }

            var destinationAdapter = adapters.Where(x => x.Name == destinationName);

            if (!destinationAdapter.Any())
            {
                throw IrisaException.Create($"مقصدی با نام {destinationName} وجود ندارد.");
            }

            if (destinationAdapter.Count() > 1)
            {
                throw IrisaException.Create($"چند مقصد با نام {destinationName} وجود دارد.");
            }

            return destinationAdapter.Single();
        }

        private void ActivatorTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                sendTimer.Awake();
                if (ConnectedAdapters != null)
                {
                    foreach (IIccAdapter current in ConnectedAdapters)
                    {
                        current.AwakeTimers();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام فعال سازی پروسه ها");
            }
        }

        private void Adapter_OnReceive(ReceiveEventArgs e)
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
                ConnectedAdapters = new List<IIccAdapter>();
                dllSettings = new DLLSettings<IccCore>();
                Logger.LogInformation("اجرای {0} آغاز شد.", new object[]
                {
                        PersianDescription
                });
                LoadAdapters();
                if (sendTimer != null)
                {
                    sendTimer.Stop();
                }
                sendTimer = new IrisaBackgroundTimer
                {
                    Interval = SendTimerInterval,
                    PersianDescription = SendTimerPersianDescription + " در " + PersianDescription,
                    EventLogger = Logger
                };
                sendTimer.DoWork += new DoWorkEventHandler(SendTimer_DoWork);
                sendTimer.Start();
                if (activatorTimer != null)
                {
                    activatorTimer.Stop();
                }
                activatorTimer = new IrisaBackgroundTimer
                {
                    Interval = ActivatorTimerInterval,
                    PersianDescription = ActivatorTimerPersianDescription + " در " + PersianDescription,
                    EventLogger = Logger
                };
                activatorTimer.DoWork += new DoWorkEventHandler(ActivatorTimer_DoWork);
                activatorTimer.Start();
                Started = true;
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, $"بروز خطا هنگام شروع به کار {PersianDescription}.");
                this.Stop();
            }
        }

        public void Stop()
        {
            sendTimer?.Stop();
            activatorTimer?.Stop();
            foreach (IIccAdapter adapter in ConnectedAdapters)
            {
                adapter.Stop();
            }

            if (Started)
                Logger.LogInformation($"اجرای {PersianDescription} خاتمه یافت.");

            Started = false;
        }

        private void LoadAdapters()
        {
            ConnectedAdapters = LoadAdapters<IIccAdapter>(@"C:\Projects\ICC\IRISA.CommunicationCenter.Adapters.TestAdapter\bin\Debug");
            ConnectedAdapters.AddRange(LoadAdapters<IIccAdapter>(@"C:\Projects\ICC\IRISA.CommunicationCenter.Adapters.TcpIp.Wasco\bin\Debug"));
            ConnectedAdapters.AddRange(LoadAdapters<IIccAdapter>(@"C:\Projects\ICC\IRISA.CommunicationCenter.Adapters.Database.Oracle\bin\Debug"));

            //ConnectedAdapters = LoadAdapters<IIccAdapter>();

            if (!ConnectedAdapters.Any())
                Logger.LogWarning("کلاینتی برای اتصال یافت نشد.");

            foreach (IIccAdapter adapter in ConnectedAdapters)
            {
                try
                {
                    adapter.TelegramReceived += new ReceiveEventHandler(Adapter_OnReceive);
                    adapter.SendCompleted += Adapter_SendCompleted;
                    adapter.Start(Logger);
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, "بروز خطا هنگام لود کلاینت ها.");
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

        private void UpdateSentTelegram(IccTelegram iccTelegram)
        {
            try
            {
                iccTelegram.Dropped = false;
                iccTelegram.DropReason = null;
                iccTelegram.ReceiveTime = new DateTime?(DateTime.Now);
                iccTelegram.Sent = true;

                IccQueue.Edit(iccTelegram);

                Logger.LogInformation("تلگرام با شناسه رکورد {0} موفقیت آمیز به مقصد ارسال شد.", new object[]
                {
                    iccTelegram.TransferId
                });

                TelegramSent?.Invoke(new IccCoreTelegramEventArgs(iccTelegram));
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام ثبت تلگرام ارسال شده.");
            }
            finally
            {
                inProcessTelegrams.Remove(iccTelegram.TransferId);
            }
        }

        private void DropTelegram(IccTelegram iccTelegram, Exception dropException, bool existingRecord)
        {
            try
            {
                if (!(dropException is IrisaException))
                {
                    Logger.LogException(dropException, "بروز خطا هنگام ارسال تلگرام.");
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

                Logger.LogInformation("تلگرام با شناسه {0} حذف شد.", new object[]
                {
                        iccTelegram.TransferId
                });

                TelegramDropped?.Invoke(new IccCoreTelegramEventArgs(iccTelegram));
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام ثبت تلگرام حذف شده.");
            }
            finally
            {
                inProcessTelegrams.Remove(iccTelegram.TransferId);
            }
        }

        private void QueueTelegram(IccTelegram iccTelegram)
        {
            try
            {
                iccTelegram.Dropped = false;
                iccTelegram.DropReason = null;
                iccTelegram.Sent = false;

                IccQueue.Add(iccTelegram);
                Logger.LogInformation("تلگرام با شناسه رکورد {0} در صف ارسال قرار گرفت.", new object[]
                {
                    iccTelegram.TransferId
                });

                TelegramQueued?.Invoke(new IccCoreTelegramEventArgs(iccTelegram));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "بروز خطا هنگام ثبت تلگرام در صف.");
            }
        }

        private List<IccTelegram> DuplicateTelegramByDestination(IccTelegram iccTelegram)
        {
            List<IccTelegram> iccTelegrams = new List<IccTelegram>();

            string[] destinations = iccTelegram.Destination.Split(DestinationSeparator);

            for (int i = 0; i < destinations.Length; i++)
            {
                string text = destinations[i];
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
                    iccTelegrams.Add(item);
                }
            }
            
            if (destinations.Length > 1)
                Logger.LogInformation($"تلگرام دریافت شده از {iccTelegram.Source} از نوع {iccTelegram.TelegramId} به {destinations.Count()} تلگرام تقسیم شد.");

            return iccTelegrams;
        }
    }
}
