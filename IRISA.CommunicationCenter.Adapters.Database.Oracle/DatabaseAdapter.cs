using IRISA.CommunicationCenter.Adapters.Database.Oracle.Model;
using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Oracle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IRISA.CommunicationCenter.Adapters.Database.Oracle
{
    public class DatabaseAdapter : BaseAdapter<DatabaseAdapter>
    {
        #region Properties

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

        #endregion

        protected override bool CheckConnection()
        {
            return ClientTelegrams.Connected;
        }

        protected override void SendTelegram(IccTelegram iccTelegram)
        {
            var clientTelegram = ToClientTelegram(iccTelegram);

            using (EntityBusiness<Entities, IccClientTelegram> clientTelegrams = ClientTelegrams)
            {
                clientTelegrams.Create(clientTelegram);
            }
        }

        private IccClientTelegram ToClientTelegram(IccTelegram iccTelegram)
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

        private IccTelegram ToIccTelegram(IccClientTelegram clientTelegram)
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

        protected override void ReceiveTimer_DoWork()
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
                        iccTelegram = ToIccTelegram(clientTelegram);
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
