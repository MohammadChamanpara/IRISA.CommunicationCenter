using IRISA.CommunicationCenter.Adapters.Model;
using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Loggers;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Threading;
using IRISA.CommunicationCenter.Oracle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace IRISA.CommunicationCenter.Adapters
{
    public class DatabaseAdapter : BaseAdapter<DatabaseAdapter>
    {
        private IrisaBackgroundTimer receiveTimer;
        private IrisaBackgroundTimer activatorTimer;
        private bool databaseIsConnected = false;
        [Browsable(false)]
        public EntityBusiness<Entities, IccClientTelegram> ClientTelegrams
        {
            get
            {
                return new EntityBusiness<Entities, IccClientTelegram>(new Entities(ConnectionString));
            }
        }
        public override string Type
        {
            get
            {
                return "پایگاه داده";
            }
        }
        [Category("Information"), DisplayName("وضعیت اتصال کلاینت")]
        public override bool Connected
        {
            get
            {
                if (!databaseIsConnected)
                {
                    try
                    {
                        CheckDatabaseConnection();
                    }
                    catch (Exception exception)
                    {
                        Logger.LogException(exception);
                    }
                }
                return base.Started && databaseIsConnected;
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
        [DisplayName("دوره زمانی بررسی دریافت تلگرام بر حسب میلی ثانیه")]
        public int ReceiveTimerInterval
        {
            get
            {
                return dllSettings.FindIntValue("ReceiveTimerInterval", 2000);
            }
            set
            {
                dllSettings.SaveSetting("ReceiveTimerInterval", value);
            }
        }

        [DisplayName("دوره زمانی فعال نمودن پروسه های دریافت تلگرام متوقف شده بر حسب میلی ثانیه")]
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

        [DisplayName("شرح فارسی پروسه دریافت تلگرام")]
        public string ReceiveTimerPersianDescription
        {
            get
            {
                return dllSettings.FindStringValue("ReceiveTimerPersianDescription", "پروسه دریافت تلگرام");
            }
            set
            {
                dllSettings.SaveSetting("ReceiveTimerPersianDescription", value);
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
        [DisplayName("تعیین مقصد تلگرام توسط فرستنده")]
        public bool GetDestinationFromSender
        {
            get
            {
                return dllSettings.FindBooleanValue("GetDestinationFromSender", false);
            }
            set
            {
                dllSettings.SaveSetting("GetDestinationFromSender", value);
            }
        }
        [DisplayName("انجام عملیات اعتبار سنجی محتوای تلگرام")]
        public bool PerformBodyValidation
        {
            get
            {
                return dllSettings.FindBooleanValue("PerformBodyValidation", true);
            }
            set
            {
                dllSettings.SaveSetting("PerformBodyValidation", value);
            }
        }
        public override void Start(ILogger eventLogger)
        {
            base.Start(eventLogger);
            receiveTimer = new IrisaBackgroundTimer
            {
                Interval = ReceiveTimerInterval,
                PersianDescription = ReceiveTimerPersianDescription + " در آداپتور " + base.PersianDescription,
                EventLogger = eventLogger
            };
            receiveTimer.DoWork += new DoWorkEventHandler(ReceiveTimer_DoWork);
            receiveTimer.Start();
            if (Connected)
            {
                OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
            }
            if (activatorTimer != null)
            {
                activatorTimer.Stop();
            }
            activatorTimer = new IrisaBackgroundTimer
            {
                Interval = ActivatorTimerInterval,
                PersianDescription = ActivatorTimerPersianDescription + " در " + PersianDescription,
                EventLogger = eventLogger
            };
            activatorTimer.DoWork += new DoWorkEventHandler(ActivatorTimer_DoWork);
            activatorTimer.Start();
        }
        private void ActivatorTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (receiveTimer != null)
                    receiveTimer.Awake();

            }
            catch (Exception exception)
            {
                Logger.LogWarning("بروز خطا هنگام فعال سازی پروسه ها", new object[0]);
                Logger.LogException(exception);
            }
        }
        public override void Stop()
        {
            base.Stop();
            if (receiveTimer != null)
            {
                receiveTimer.Stop();
            }
        }
        protected override void SendTelegram(IccTelegram iccTelegram)
        {
            IccClientTelegram entity = new IccClientTelegram();
            ConvertStandardTelegramToClientTelegram(iccTelegram, ref entity);
            using (EntityBusiness<Entities, IccClientTelegram> clientTelegrams = ClientTelegrams)
            {
                clientTelegrams.Create(entity);
            }
        }
        public virtual void ConvertStandardTelegramToClientTelegram(IccTelegram iccTelegram, ref IccClientTelegram clientTelegram)
        {
            telegramDefinitions.Find(iccTelegram);
            clientTelegram = new IccClientTelegram
            {
                BODY = iccTelegram.GetBodyString(BodySeparator),
                DESTINATION = iccTelegram.Destination,
                TELEGRAM_ID = iccTelegram.TelegramId,
                PROCESSED = false,
                SEND_TIME = iccTelegram.SendTime,
                SOURCE = iccTelegram.Source,
                READY_FOR_CLIENT = true,
                TRANSFER_ID = iccTelegram.TransferId
            };
        }
        public virtual void ConvertClientTelegramToStandardTelegram(IccClientTelegram clientTelegram, ref IccTelegram iccTelegram)
        {
            iccTelegram.Source = clientTelegram.SOURCE;
            iccTelegram.SendTime = clientTelegram.SEND_TIME;
            iccTelegram.TelegramId = clientTelegram.TELEGRAM_ID;
            if (GetDestinationFromSender)
            {
                iccTelegram.Destination = clientTelegram.DESTINATION;
            }
            else
            {
                TelegramDefinition telegramDefinition = telegramDefinitions.Find(iccTelegram);
                iccTelegram.Destination = telegramDefinition.Destination;
            }
            if (clientTelegram.BODY.HasValue())
            {
                iccTelegram.Body = clientTelegram.BODY.Split(new char[]
				{
					BodySeparator
				}).ToList<string>();
            }
            else
            {
                iccTelegram.Body = new List<string>();
            }
        }
        public override void AwakeTimers()
        {
            base.AwakeTimers();
            if (receiveTimer != null)
            {
                receiveTimer.Awake();
            }
        }

        private void CheckDatabaseConnection()
        {
            using (EntityBusiness<Entities, IccClientTelegram> clientTelegrams = ClientTelegrams)
            {
                bool flag = databaseIsConnected;
                if (!clientTelegrams.Connected)
                {
                    databaseIsConnected = false;
                    if (flag)
                    {
                        OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
                    }
                    throw IrisaException.Create("برنامه قادر به اتصال به پایگاه داده {0} نمی باشد. ", new object[]
					{
						base.PersianDescription
					});
                }
                databaseIsConnected = true;
                if (!flag)
                {
                    OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
                }
            }
        }
        private void ReceiveTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            using (EntityBusiness<Entities, IccClientTelegram> clientTelegrams = ClientTelegrams)
            {
                CheckDatabaseConnection();
                List<IccClientTelegram> list = (
                    from t in clientTelegrams.GetAll()
                    where t.PROCESSED == false && t.SOURCE.ToLower() == Name.ToLower() && t.READY_FOR_CLIENT == false
                    select t).Take(100).ToList<IccClientTelegram>();
                foreach (IccClientTelegram current in list)
                {
                    IccTelegram iccTelegram = new IccTelegram
                    {
                        Source = base.Name
                    };
                    bool successful = false;
                    Exception failException = null;
                    try
                    {
                        ConvertClientTelegramToStandardTelegram(current, ref iccTelegram);
                        successful = true;
                    }
                    catch (Exception ex)
                    {
                        failException = ex;
                        successful = false;
                    }
                    finally
                    {
                        current.PROCESSED = true;
                        clientTelegrams.Edit(current);
                        OnReceive(new ReceiveEventArgs(iccTelegram, successful, failException));
                    }
                }
            }
        }
    }
}
