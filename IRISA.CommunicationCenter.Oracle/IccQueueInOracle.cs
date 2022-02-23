using IRISA.CommunicationCenter.Core;
using IRISA.CommunicationCenter.Core.Model;
using IRISA.CommunicationCenter.Library.Loggers;
using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IRISA.CommunicationCenter.Oracle
{
    public class IccQueueInOracle : IIccQueue
    {
        private readonly DLLSettings<IccQueueInOracle> dllSettings = new DLLSettings<IccQueueInOracle>();

        [Browsable(false)]
        public EntityBusiness<Entities, IccTransfer> IccTransfers
        {
            get
            {
                return new EntityBusiness<Entities, IccTransfer>(new Entities(ConnectionString));
            }
        }

        public void Add(IccTelegram iccTelegram)
        {
            IccTransfers.Create(iccTelegram.ToIccTransfer());

            var id =
                IccTransfers
                .GetAll()
                .Max(x => x.ID);

            iccTelegram.TransferId = id;
        }

        public void Edit(IccTelegram iccTelegram)
        {
            IccTransfers.Edit(iccTelegram.ToIccTransfer());
        }

        public List<IccTelegram> GetTelegramsToSend()
        {
            if (!Connected)
                throw IrisaException.Create("برنامه قادر به دسترسی به صف تلگرام ها نمی باشد");

            return IccTransfers
                .GetAll()
                .Where(x => x.DROPPED == false && x.SENT == false)
                .Select(x => x.ToIccTelegram())
                .ToList();
        }

        public List<IccTelegram> GetTelegrams(int pageSize = 50)
        {
            return
                IccTransfers
                .GetAll()
                   .OrderByDescending(x => x.ID)
                   .Take(pageSize)
                   .Select(x => x.ToIccTelegram())
                   .ToList();
        }

        public List<IccTelegram> GetTelegrams(IccTelegramSearchModel searchModel, int pageSize, out int resultCount)
        {
            var transfers = IccTransfers.GetAll();

            /*.................... Transfer Id ....................*/
            transfers = searchModel.TransferId.HasValue
                ? transfers.Where(x => x.ID == searchModel.TransferId)
                : transfers;

            /*.................... Telegram Id ....................*/

            transfers = searchModel.TelegramId.HasValue
                 ? transfers.Where(x => x.TELEGRAM_ID == searchModel.TelegramId)
                 : transfers;

            /*.................... Source ....................*/

            transfers = searchModel.Source.HasValue()
                ? transfers.Where(x => x.SOURCE.ToLower().Contains(searchModel.Source.ToLower()))
                : transfers;

            /*.................... Destination ....................*/

            transfers = searchModel.Destination.HasValue()
                ? transfers.Where(x => x.DESTINATION.ToLower().Contains(searchModel.Destination.ToLower()))
                : transfers;

            /*.................... Sent ....................*/

            transfers = searchModel.Sent.HasValue
                ? transfers.Where(x => x.SENT == searchModel.Sent)
                : transfers;

            /*.................... Dropped....................*/

            transfers = searchModel.Dropped.HasValue
                ? transfers.Where(x => x.DROPPED == searchModel.Dropped)
                : transfers;

            /* ................Drop Reason...................*/

            transfers = searchModel.DropReason.HasValue()
                ? transfers.Where(x => x.DROP_REASON.Contains(searchModel.DropReason))
                : transfers;

            /*....................Send Time....................*/

            if (searchModel.SendTime.HasValue)
            {
                var sendTime = searchModel.SendTime.Value;
                transfers = transfers.Where
                (
                    x =>
                    x.SEND_TIME.Year == sendTime.Year &&
                    x.SEND_TIME.Month == sendTime.Month &&
                    x.SEND_TIME.Day == sendTime.Day
                );

                if (sendTime.Hour > 0)
                    transfers = transfers.Where(x => x.SEND_TIME.Hour == sendTime.Hour);
                if (sendTime.Minute > 0)
                    transfers = transfers.Where(x => x.SEND_TIME.Minute == sendTime.Minute);
                if (sendTime.Second > 0)
                    transfers = transfers.Where(x => x.SEND_TIME.Second == sendTime.Second);
            }

            /*....................Receive Time....................*/

            if (searchModel.ReceiveTime.HasValue)
            {
                var receiveTime = searchModel.ReceiveTime.Value;
                transfers = transfers.Where
                (
                    x =>
                    x.RECEIVE_TIME.Value.Year == receiveTime.Year &&
                    x.RECEIVE_TIME.Value.Month == receiveTime.Month &&
                    x.RECEIVE_TIME.Value.Day == receiveTime.Day
                );

                if (receiveTime.Hour > 0)
                    transfers = transfers.Where(x => x.RECEIVE_TIME.Value.Hour == receiveTime.Hour);
                if (receiveTime.Minute > 0)
                    transfers = transfers.Where(x => x.RECEIVE_TIME.Value.Minute == receiveTime.Minute);
                if (receiveTime.Second > 0)
                    transfers = transfers.Where(x => x.RECEIVE_TIME.Value.Second == receiveTime.Second);
            }


            resultCount = transfers.Count();

            pageSize = Math.Min(pageSize, resultCount);

            return transfers
                .OrderByDescending(t => t.ID)
                .Take(pageSize)
                .ToList()
                .Select(x => x.ToIccTelegram())
                .ToList();
        }

        [Category("Operation")]
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

        [DisplayName("نوع صف")]
        [Category("Information")]
        public string Type => nameof(IccQueueInOracle);

        [Category("Information")]
        [DisplayName("وضعیت اتصال")]
        public bool Connected
        {
            get
            {
                try
                {
                    return IccTransfers.Connected;
                }
                catch
                {
                    return false;
                }
            }
        }

        [Category("Information")]
        [DisplayName("تعداد تلگرام ها")]
        public int Count => IccTransfers.GetAll().Count();
    }
}
