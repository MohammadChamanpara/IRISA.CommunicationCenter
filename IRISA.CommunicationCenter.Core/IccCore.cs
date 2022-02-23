using IRISA.CommunicationCenter.Library.Adapters;
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

        public static List<T> LoadPlugins<T>()
        {
            return LoadPlugins<T>(AppDomain.CurrentDomain.BaseDirectory + "\\Plugins");
        }
        public static List<T> LoadPlugins<T>(string directory)
        {
            List<T> result;
            try
            {
                List<T> list = new List<T>();
                if (!Directory.Exists(directory))
                {
                    throw IrisaException.Create("مسیر ذخیره پلاگین ها {0} موجود نیست.", new object[]
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
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex is ReflectionTypeLoadException)
                {
                    ReflectionTypeLoadException ex2 = ex as ReflectionTypeLoadException;
                    if (ex2.LoaderExceptions != null && ex2.LoaderExceptions.Count<Exception>() > 0)
                    {
                        message = ex2.LoaderExceptions.First<Exception>().Message;
                    }
                }
                throw IrisaException.Create("خطا هنگام لود کردن پلاگین ها. متن خطا : " + message, new object[0]);
            }
            return result;
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
        private void SendTimer_DoWork(object sender, DoWorkEventArgs e)
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
                catch (Exception exception)
                {
                    DropTelegram(telegram, exception, true);
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

        private void ActivatorTimer_DoWork(object sender, DoWorkEventArgs e)
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
                Logger.LogException(exception, "بروز خطا هنگام فعال سازی پروسه ها");
            }
        }
        private void Client_OnReceive(ReceiveEventArgs e)
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
                Started = false;
                Logger.LogException(exception, $"بروز خطا هنگام شروع به کار {PersianDescription}.");
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
            connectedClients = LoadPlugins<IIccAdapter>(@"C:\Projects\ICC\IRISA.CommunicationCenter.Adapters.TestAdapter\bin\Debug");

            if (connectedClients.Count == 0)
            {
                Logger.LogWarning("کلاینتی برای اتصال یافت نشد.", new object[0]);
            }
            foreach (IIccAdapter adapter in connectedClients)
            {
                try
                {
                    adapter.Receive += new ReceiveEventHandler(Client_OnReceive);
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

        private void ValidateTelegram(IccTelegram iccTelegram)
        {
            if (!iccTelegram.Destination.HasValue())
            {
                throw IrisaException.Create("مقصد تلگرام مشخص نشده است.", new object[0]);
            }

            var destination = connectedClients
                    .Where(c => c.Name == iccTelegram.Destination);

            if (destination.Count() == 0)
            {
                throw IrisaException.Create("مقصد مشخص شده وجود ندارد.", new object[0]);
            }

            if (destination.Count() > 1)
            {
                throw IrisaException.Create("چند مقصد با نام داده شده وجود دارد.", new object[0]);
            }
        }
        private void DropTelegram(IccTelegram iccTelegram, Exception dropException, bool existingRecord)
        {
            try
            {
                if (!(dropException is IrisaException))
                {
                    Logger.LogException(dropException, "بروز خطای پیشبینی نشده.");
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

                TelegramDropped?.Invoke(new IccCoreTelegramEventArgs(iccTelegram));
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام ثبت تلگرام حذف شده.");
            }
            finally
            {
                InProcessTelegrams.Remove(iccTelegram);
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
                Logger.LogSuccess("تلگرام با شناسه رکورد {0} در صف ارسال قرار گرفت.", new object[]
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
