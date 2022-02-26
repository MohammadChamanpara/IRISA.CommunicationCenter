using IRISA.CommunicationCenter.Adapters.Database.Oracle.Model;
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

namespace IRISA.CommunicationCenter.Adapters.Database.Oracle
{
    public class DatabaseAdapter : BaseAdapter<DatabaseAdapter>
    {
        private IrisaBackgroundTimer receiveTimer;
        private IrisaBackgroundTimer activatorTimer;

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

        protected override bool CheckConnection()
        {
            return ClientTelegrams.Connected;
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
                Logger.LogException(exception, "بروز خطا هنگام فعال سازی پروسه ها");
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
            var clientTelegram = ConvertStandardTelegramToClientTelegram(iccTelegram);

            using (EntityBusiness<Entities, IccClientTelegram> clientTelegrams = ClientTelegrams)
            {
                clientTelegrams.Create(clientTelegram);
            }
        }

        private IccClientTelegram ConvertStandardTelegramToClientTelegram(IccTelegram iccTelegram)
        {
            telegramDefinitions.Find(iccTelegram);
            return new IccClientTelegram
            {
                BODY = iccTelegram.GetBodyAsString(BodySeparator),
                DESTINATION = iccTelegram.Destination,
                TELEGRAM_ID = iccTelegram.TelegramId,
                PROCESSED = false,
                SEND_TIME = iccTelegram.SendTime,
                SOURCE = iccTelegram.Source,
                READY_FOR_CLIENT = true,
                TRANSFER_ID = iccTelegram.TransferId
            };
        }

        private IccTelegram ConvertClientTelegramToStandardTelegram(IccClientTelegram clientTelegram)
        {
            var iccTelegram = new IccTelegram
            {
                Source = clientTelegram.SOURCE,
                SendTime = DateTime.Now,
                TelegramId = clientTelegram.TELEGRAM_ID
            };

            iccTelegram.Destination = GetDestinationFromSender
                ? clientTelegram.DESTINATION
                : telegramDefinitions.Find(iccTelegram).Destination;

            iccTelegram.SetBodyFromString(clientTelegram.BODY, BodySeparator);

            return iccTelegram;
        }

        public override void AwakeTimers()
        {
            base.AwakeTimers();
            if (receiveTimer != null)
            {
                receiveTimer.Awake();
            }
        }

        private void ReceiveTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var clientTelegramsTable = ClientTelegrams)
            {
                List<IccClientTelegram> clientTelegrams = clientTelegramsTable
                        .GetAll()
                        .Where
                        (
                            t =>
                            t.PROCESSED == false &&
                            t.SOURCE.ToLower() == Name.ToLower() &&
                            t.READY_FOR_CLIENT == false
                        )
                        .Take(100)
                        .ToList();

                foreach (IccClientTelegram clientTelegram in clientTelegrams)
                {
                    IccTelegram iccTelegram = new IccTelegram
                    {
                        Source = Name
                    };

                    bool successful = false;
                    Exception failException = null;

                    try
                    {
                        iccTelegram = ConvertClientTelegramToStandardTelegram(clientTelegram);
                        successful = true;
                    }
                    catch (Exception exception)
                    {
                        failException = exception;
                        successful = false;
                    }
                    finally
                    {
                        clientTelegram.PROCESSED = true;
                        clientTelegramsTable.Edit(clientTelegram);
                        OnReceive(new ReceiveEventArgs(iccTelegram, successful, failException));
                    }
                }
            }
        }
    }
}
