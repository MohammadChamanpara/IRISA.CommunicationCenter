using IRISA.CommunicationCenter.Adapters;
using IRISA.CommunicationCenter.Model;
using IRISA.Log;
using IRISA.Model;
using IRISA.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IRISA.CommunicationCenter
{
    public class IccCore
    {
        public delegate void IccCoreTelegramEventHandler(IccCoreTelegramEventArgs e);
        private static HashSet<long> ProcessingTelegrams = new HashSet<long>();
        public List<IIccAdapter> connectedClients;
        private IrisaBackgroundTimer sendTimer;
        private IrisaBackgroundTimer activatorTimer;
        private DLLSettings<IccCore> dllSettings;
        public IccEventLogger eventLogger = new IccEventLogger();
        public event IccCoreTelegramEventHandler TelegramQueued;
        public event IccCoreTelegramEventHandler TelegramSent;
        public event IccCoreTelegramEventHandler TelegramDropped;
        [Browsable(false)]
        public EntityBusiness<Entities, IccEvent> Events
        {
            get
            {
                return new EntityBusiness<Entities, IccEvent>(new Entities(ConnectionString));
            }
        }
        [Browsable(false)]
        public EntityBusiness<Entities, IccTransfer> Transfers
        {
            get
            {
                return new EntityBusiness<Entities, IccTransfer>(new Entities(ConnectionString));
            }
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
        [DisplayName("کاراکتر جدا کننده فیلد های تلگرام")]
        public char BodySeparator
        {
            get
            {
                return dllSettings.FindCharacterValue("BodySeparator", '$');
            }
            set
            {
                dllSettings.SaveSetting("BodySeparator", value);
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
        [DisplayName("رشته اتصال به پایگاه داده")]
        public string ConnectionString
        {
            get
            {
                return dllSettings.FindConnectionString();
            }
            set
            {
                dllSettings.SaveConnectionString(value);
            }
        }
        [Category("ReadOnly"), DisplayName("وضعیت اجرای پروسه")]
        public bool Started
        {
            get;
            private set;
        }
        [Category("ReadOnly"), DisplayName("نوع فایل")]
        public string FileAssembly
        {
            get
            {
                return dllSettings.Assembly.AssemblyName();
            }
        }
        [Category("ReadOnly"), DisplayName("ورژن برنامه")]
        public string FileAssemblyVersion
        {
            get
            {
                return dllSettings.Assembly.AssemblyVersion();
            }
        }
        [Category("ReadOnly"), DisplayName("آدرس فایل")]
        public string FileAddress
        {
            get
            {
                return dllSettings.Assembly.Location;
            }
        }
        private void sendTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckDataBaseConnection();

            var telegrams = GetTelegramsFromDataBase(Transfers);

            telegrams = RemoveInProcessTelegrams(telegrams, ProcessingTelegrams);

            ProcessingTelegrams = AddTelegramsToProcessingList(telegrams, ProcessingTelegrams);

            var telegramGroups = GroupTelegramsByDestination(telegrams);

            foreach (var list in telegramGroups)
                Task.Run(() => SendTelegramsToADestination(list, connectedClients));
        }

        public HashSet<long> AddTelegramsToProcessingList(List<IccTransfer> telegrams, HashSet<long> processingTelegrams)
        {
            processingTelegrams.UnionWith(telegrams.Select(x => x.ID));
            return processingTelegrams;
        }

        public List<IccTransfer> GetTelegramsFromDataBase(EntityBusiness<Entities, IccTransfer> transfers)
        {
            return
                transfers
                   .GetAll()
                   .Where(x => x.SENT == false && x.DROPPED == false)
                   .ToList();
        }

        public List<IccTransfer> RemoveInProcessTelegrams(List<IccTransfer> telegrams, HashSet<long> processingTelegrams)
        {
            return telegrams
                .Where(x => !processingTelegrams.Contains(x.ID))
                .ToList();
        }

        public List<List<IccTransfer>> GroupTelegramsByDestination(List<IccTransfer> telegrams)
        {
            return telegrams
                .GroupBy(x => x.DESTINATION)
                .Select(x => x.ToList())
                .ToList();
        }

        public void SendTelegramsToADestination(List<IccTransfer> telegrams, List<IIccAdapter> adapters)
        {
            if (telegrams.Count == 0)
                return;

            IIccAdapter iccAdapter = GetDestinationAdapter(adapters, telegrams.First().DESTINATION);

            if (!iccAdapter.Connected)
                return;

            foreach (IccTransfer transfer in telegrams)
            {
                try
                {
                    IccTelegram iccTelegram = IccTransferToIccTelegram(transfer);

                    ValidateTelegram(iccTelegram);

                    iccAdapter.Send(iccTelegram);

                    transfer.DROPPED = false;
                    transfer.DROP_REASON = null;
                    transfer.RECEIVE_TIME = new DateTime?(DateTime.Now);
                    transfer.SENT = true;
                    Transfers.Edit(transfer);

                    eventLogger.LogSuccess("تلگرام با شناسه رکورد {0} موفقیت آمیز به مقصد ارسال شد.", new object[]
                    {
                            transfer.ID
                    });

                    if (TelegramSent != null)
                    {
                        TelegramSent(new IccCoreTelegramEventArgs(iccTelegram));
                    }
                }
                catch (Exception ex)
                {
                    if (transfer.SENT == false)
                    {
                        DropTelegram(transfer, ex, true);
                    }
                    else
                    {
                        eventLogger.LogException(ex);
                    }
                }
            }
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
                eventLogger.LogWarning("بروز خطا هنگام فعال سازی پروسه ها", new object[0]);
                eventLogger.LogException(exception);
            }
        }
        private void client_OnReceive(ReceiveEventArgs e)
        {
            bool flag = false;
            try
            {
                Monitor.Enter(this, ref flag);
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
            finally
            {
                if (flag)
                {
                    Monitor.Exit(this);
                }
            }
        }
        public void Start()
        {
            try
            {
                connectedClients = new List<IIccAdapter>();
                dllSettings = new DLLSettings<IccCore>();
                eventLogger.DbLoggerEnabled = true;
                if (!Transfers.Connected)
                {
                    eventLogger.DbLoggerEnabled = false;
                }
                else
                {
                    eventLogger.LogInfo("اجرای {0} آغاز شد.", new object[]
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
                    sendTimer.EventLogger = eventLogger;
                    sendTimer.Start();
                    if (activatorTimer != null)
                    {
                        activatorTimer.Stop();
                    }
                    activatorTimer = new IrisaBackgroundTimer();
                    activatorTimer.Interval = ActivatorTimerInterval;
                    activatorTimer.DoWork += new DoWorkEventHandler(activatorTimer_DoWork);
                    activatorTimer.PersianDescription = ActivatorTimerPersianDescription + " در " + PersianDescription;
                    activatorTimer.EventLogger = eventLogger;
                    activatorTimer.Start();
                    Started = true;
                }
            }
            catch (Exception exception)
            {
                Started = false;
                eventLogger.LogException(exception);
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
                eventLogger.LogInfo("اجرای {0} خاتمه یافت.", new object[]
                {
                    PersianDescription
                });
            }
            Started = false;
        }
        private void CheckDataBaseConnection()
        {
            if (!Transfers.Connected)
            {
                throw HelperMethods.CreateException("برنامه قادر به دسترسی به پایگاه داده {0} نمی باشد", new object[]
                {
                    PersianDescription
                });
            }
        }
        private void LoadClients()
        {
            connectedClients = HelperMethods.LoadPlugins<IIccAdapter>();
            if (connectedClients.Count == 0)
            {
                eventLogger.LogWarning("کلاینتی برای اتصال یافت نشد.", new object[0]);
            }
            foreach (IIccAdapter current in connectedClients)
            {
                try
                {
                    current.Receive += new ReceiveEventHandler(client_OnReceive);
                    current.Start(eventLogger);
                }
                catch (Exception exception)
                {
                    eventLogger.LogException(exception);
                }
            }
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
            DropTelegram(IccTelegramToIccTransfer(iccTelegram), dropException, existingRecord);
        }
        private void DropTelegram(IccTransfer iccTransfer, Exception dropException, bool existingRecord)
        {
            try
            {
                bool flag = false;
                try
                {
                    Monitor.Enter(this, ref flag);
                    if (!(dropException is IrisaException))
                    {
                        eventLogger.LogException(dropException);
                    }
                    iccTransfer.SENT = false;
                    iccTransfer.DROPPED = true;
                    iccTransfer.DROP_REASON = dropException.MostInnerException().Message;
                    if (existingRecord)
                    {
                        Transfers.Edit(iccTransfer);
                    }
                    else
                    {
                        Transfers.Create(iccTransfer);
                        iccTransfer = (
                            from t in Transfers.GetAll()
                            orderby t.ID descending
                            select t).First<IccTransfer>();
                    }
                    eventLogger.LogInfo("تلگرام با شناسه {0} حذف شد.", new object[]
                    {
                        iccTransfer.ID
                    });
                    if (TelegramDropped != null)
                    {
                        TelegramDropped(new IccCoreTelegramEventArgs(IccTransferToIccTelegram(iccTransfer)));
                    }
                }
                finally
                {
                    if (flag)
                    {
                        Monitor.Exit(this);
                    }
                }
            }
            catch (Exception exception)
            {
                eventLogger.LogException(exception);
            }
        }
        private void QueueTelegram(IccTelegram iccTelegram)
        {
            bool flag = false;
            try
            {
                IccTransfer iccTransfer = IccTelegramToIccTransfer(iccTelegram);
                iccTransfer.DROPPED = false;
                iccTransfer.DROP_REASON = null;
                iccTransfer.SENT = false;
                Transfers.Create(iccTransfer);
                flag = true;
                iccTransfer = (
                    from t in Transfers.GetAll()
                    orderby t.ID descending
                    select t).First<IccTransfer>();
                eventLogger.LogSuccess("تلگرام با شناسه رکورد {0} در صف ارسال قرار گرفت.", new object[]
                {
                    iccTransfer.ID
                });
                if (TelegramQueued != null)
                {
                    TelegramQueued(new IccCoreTelegramEventArgs(iccTelegram));
                }
            }
            catch (Exception ex)
            {
                if (!flag)
                {
                    DropTelegram(iccTelegram, ex, false);
                }
                else
                {
                    eventLogger.LogException(ex);
                }
            }
        }
        public IccTelegram IccTransferToIccTelegram(IccTransfer iccTransfer)
        {
            IccTelegram iccTelegram = new IccTelegram();
            iccTelegram.Destination = iccTransfer.DESTINATION;
            iccTelegram.TelegramId = (iccTransfer.TELEGRAM_ID.HasValue ? iccTransfer.TELEGRAM_ID.Value : 0);
            iccTelegram.TransferId = iccTransfer.ID;
            iccTelegram.SendTime = iccTransfer.SEND_TIME;
            iccTelegram.Source = iccTransfer.SOURCE;
            iccTelegram.SetBodyString(iccTransfer.BODY, BodySeparator);
            return iccTelegram;
        }
        public IccTransfer IccTelegramToIccTransfer(IccTelegram iccTelegram)
        {
            return new IccTransfer
            {
                TELEGRAM_ID = new int?(iccTelegram.TelegramId),
                SOURCE = iccTelegram.Source,
                DESTINATION = iccTelegram.Destination,
                SEND_TIME = iccTelegram.SendTime,
                BODY = iccTelegram.GetBodyString(BodySeparator)
            };
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
