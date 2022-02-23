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
                return new EntityBusiness<Entities, IccClientTelegram>(new Entities(this.ConnectionString));
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
                        this.CheckDatabaseConnection();
                    }
                    catch (Exception exception)
                    {
                        eventLogger.LogException(exception);
                    }
                }
                return base.Started && this.databaseIsConnected;
            }
        }
        [DisplayName("رشته اتصال به پایگاه داده")]
        public string ConnectionString
        {
            get
            {
                return this.dllSettings.FindConnectionString();
            }
            set
            {
                this.dllSettings.SaveConnectionString(value);
            }
        }
        [DisplayName("دوره زمانی بررسی دریافت تلگرام بر حسب میلی ثانیه")]
        public int ReceiveTimerInterval
        {
            get
            {
                return this.dllSettings.FindIntValue("ReceiveTimerInterval", 2000);
            }
            set
            {
                this.dllSettings.SaveSetting("ReceiveTimerInterval", value);
            }
        }

        [DisplayName("دوره زمانی فعال نمودن پروسه های دریافت تلگرام متوقف شده بر حسب میلی ثانیه")]
        public int ActivatorTimerInterval
        {
            get
            {
                return this.dllSettings.FindIntValue("activatorTimerInterval", 20000);
            }
            set
            {
                this.dllSettings.SaveSetting("activatorTimerInterval", value);
            }
        }

        [DisplayName("شرح فارسی پروسه دریافت تلگرام")]
        public string ReceiveTimerPersianDescription
        {
            get
            {
                return this.dllSettings.FindStringValue("ReceiveTimerPersianDescription", "پروسه دریافت تلگرام");
            }
            set
            {
                this.dllSettings.SaveSetting("ReceiveTimerPersianDescription", value);
            }
        }
        [DisplayName("شرح فارسی پروسه فعال ساز")]
        public string ActivatorTimerPersianDescription
        {
            get
            {
                return this.dllSettings.FindStringValue("ActivatorTimerPersianDescription", "پروسه فعال ساز");
            }
            set
            {
                this.dllSettings.SaveSetting("ActivatorTimerPersianDescription", value);
            }
        }
        [DisplayName("کاراکتر جدا کننده فیلد های تلگرام")]
        public char BodySeparator
        {
            get
            {
                return this.dllSettings.FindCharacterValue("BodySeparator", '$');
            }
            set
            {
                this.dllSettings.SaveSetting("BodySeparator", value);
            }
        }
        [DisplayName("تعیین مقصد تلگرام توسط فرستنده")]
        public bool GetDestinationFromSender
        {
            get
            {
                return this.dllSettings.FindBooleanValue("GetDestinationFromSender", false);
            }
            set
            {
                this.dllSettings.SaveSetting("GetDestinationFromSender", value);
            }
        }
        [DisplayName("انجام عملیات اعتبار سنجی محتوای تلگرام")]
        public bool PerformBodyValidation
        {
            get
            {
                return this.dllSettings.FindBooleanValue("PerformBodyValidation", true);
            }
            set
            {
                this.dllSettings.SaveSetting("PerformBodyValidation", value);
            }
        }
        public override void Start(ILogger eventLogger)
        {
            base.Start(eventLogger);
            this.receiveTimer = new IrisaBackgroundTimer();
            this.receiveTimer.Interval = this.ReceiveTimerInterval;
            this.receiveTimer.DoWork += new DoWorkEventHandler(this.receiveTimer_DoWork);
            this.receiveTimer.PersianDescription = this.ReceiveTimerPersianDescription + " در آداپتور " + base.PersianDescription;
            this.receiveTimer.EventLogger = this.eventLogger;
            this.receiveTimer.Start();
            if (this.Connected)
            {
                this.OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
            }
            if (this.activatorTimer != null)
            {
                this.activatorTimer.Stop();
            }
            this.activatorTimer = new IrisaBackgroundTimer();
            this.activatorTimer.Interval = this.ActivatorTimerInterval;
            this.activatorTimer.DoWork += new DoWorkEventHandler(this.activatorTimer_DoWork);
            this.activatorTimer.PersianDescription = this.ActivatorTimerPersianDescription + " در " + this.PersianDescription;
            this.activatorTimer.EventLogger = this.eventLogger;
            this.activatorTimer.Start();
        }
        private void activatorTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (this.receiveTimer != null)
                    this.receiveTimer.Awake();

            }
            catch (Exception exception)
            {
                this.eventLogger.LogWarning("بروز خطا هنگام فعال سازی پروسه ها", new object[0]);
                this.eventLogger.LogException(exception);
            }
        }
        public override void Stop()
        {
            base.Stop();
            if (this.receiveTimer != null)
            {
                this.receiveTimer.Stop();
            }
        }
        protected override void SendTelegram(IccTelegram iccTelegram)
        {
            IccClientTelegram entity = new IccClientTelegram();
            this.ConvertStandardTelegramToClientTelegram(iccTelegram, ref entity);
            using (EntityBusiness<Entities, IccClientTelegram> clientTelegrams = this.ClientTelegrams)
            {
                clientTelegrams.Create(entity);
            }
        }
        public virtual void ConvertStandardTelegramToClientTelegram(IccTelegram iccTelegram, ref IccClientTelegram clientTelegram)
        {
            this.telegramDefinitions.Find(iccTelegram);
            clientTelegram = new IccClientTelegram
            {
                BODY = iccTelegram.GetBodyString(this.BodySeparator),
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
            if (this.GetDestinationFromSender)
            {
                iccTelegram.Destination = clientTelegram.DESTINATION;
            }
            else
            {
                TelegramDefinition telegramDefinition = this.telegramDefinitions.Find(iccTelegram);
                iccTelegram.Destination = telegramDefinition.Destination;
            }
            if (clientTelegram.BODY.HasValue())
            {
                iccTelegram.Body = clientTelegram.BODY.Split(new char[]
				{
					this.BodySeparator
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
            if (this.receiveTimer != null)
            {
                this.receiveTimer.Awake();
            }
        }

        private void CheckDatabaseConnection()
        {
            using (EntityBusiness<Entities, IccClientTelegram> clientTelegrams = this.ClientTelegrams)
            {
                bool flag = this.databaseIsConnected;
                if (!clientTelegrams.Connected)
                {
                    this.databaseIsConnected = false;
                    if (flag)
                    {
                        this.OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
                    }
                    throw IrisaException.Create("برنامه قادر به اتصال به پایگاه داده {0} نمی باشد. ", new object[]
					{
						base.PersianDescription
					});
                }
                this.databaseIsConnected = true;
                if (!flag)
                {
                    this.OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
                }
            }
        }
        private void receiveTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            using (EntityBusiness<Entities, IccClientTelegram> clientTelegrams = this.ClientTelegrams)
            {
                this.CheckDatabaseConnection();
                List<IccClientTelegram> list = (
                    from t in clientTelegrams.GetAll()
                    where t.PROCESSED == false && t.SOURCE.ToLower() == this.Name.ToLower() && t.READY_FOR_CLIENT == false
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
                        this.ConvertClientTelegramToStandardTelegram(current, ref iccTelegram);
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
                        this.OnReceive(new ReceiveEventArgs(iccTelegram, successful, failException));
                    }
                }
            }
        }
    }
}
