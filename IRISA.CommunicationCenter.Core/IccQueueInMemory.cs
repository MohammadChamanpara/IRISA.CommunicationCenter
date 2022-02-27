using IRISA.CommunicationCenter.Library.Core;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IRISA.CommunicationCenter.Core
{
    public class IccQueueInMemory : IIccQueue
    {
        private static List<IccTelegram> items = new List<IccTelegram>();
        private static long id = 1;

        public void Add(IccTelegram iccTelegram)
        {
            iccTelegram.TransferId = id++;
            items.Add(iccTelegram);

            int readytelegrams = items.Where(x => x.IsReadyToSend).Count();


            if (items.Count > 20000)
                items = items
                    .OrderByDescending(x => x.TransferId)
                    .Take(Math.Max(readytelegrams, 10000))
                    .ToList();
        }

        public void Edit(IccTelegram iccTelegram)
        {
        }

        public List<IccTelegram> GetTelegramsToSend()
        {
            return items
                .Where(x => x.IsReadyToSend)
                .ToList();
        }

        public List<IccTelegram> GetTelegrams(IccTelegramSearchModel searchModel, int pageSize, out int resultCount)
        {
            var transfers = items.AsEnumerable();

            /*.................... Transfer Id ....................*/
            transfers = searchModel.TransferId.HasValue
                ? transfers.Where(x => x.TransferId == searchModel.TransferId)
                : transfers;

            /*.................... Telegram Id ....................*/

            transfers = searchModel.TelegramId.HasValue
                 ? transfers.Where(x => x.TelegramId == searchModel.TelegramId)
                 : transfers;

            /*.................... Source ....................*/

            transfers = searchModel.Source.HasValue()
                ? transfers.Where(x => x.Source.ToLower().Contains(searchModel.Source.ToLower()))
                : transfers;

            /*.................... Destination ....................*/

            transfers = searchModel.Destination.HasValue()
                ? transfers.Where(x => x.Destination?.ToLower()?.Contains(searchModel.Destination.ToLower()) == true)
                : transfers;

            /*.................... Sent ....................*/

            transfers = searchModel.Sent.HasValue
                ? transfers.Where(x => x.Sent == searchModel.Sent)
                : transfers;

            /*.................... Dropped....................*/

            transfers = searchModel.Dropped.HasValue
                ? transfers.Where(x => x.Dropped == searchModel.Dropped)
                : transfers;

            /* ................Drop Reason...................*/

            transfers = searchModel.DropReason.HasValue()
                ? transfers.Where(x => x.DropReason?.Contains(searchModel.DropReason) == true)
                : transfers;

            /*....................Send Time....................*/

            if (searchModel.SendTime.HasValue)
            {
                var sendTime = searchModel.SendTime.Value;
                transfers = transfers.Where
                (
                    x =>
                    x.SendTime.Year == sendTime.Year &&
                    x.SendTime.Month == sendTime.Month &&
                    x.SendTime.Day == sendTime.Day
                );

                if (sendTime.Hour > 0)
                    transfers = transfers.Where(x => x.SendTime.Hour == sendTime.Hour);
                if (sendTime.Minute > 0)
                    transfers = transfers.Where(x => x.SendTime.Minute == sendTime.Minute);
                if (sendTime.Second > 0)
                    transfers = transfers.Where(x => x.SendTime.Second == sendTime.Second);
            }

            /*....................Receive Time....................*/

            if (searchModel.ReceiveTime.HasValue)
            {
                var receiveTime = searchModel.ReceiveTime.Value;
                transfers = transfers.Where
                (
                    x =>
                    x.ReceiveTime.Value.Year == receiveTime.Year &&
                    x.ReceiveTime.Value.Month == receiveTime.Month &&
                    x.ReceiveTime.Value.Day == receiveTime.Day
                );

                if (receiveTime.Hour > 0)
                    transfers = transfers.Where(x => x.ReceiveTime.Value.Hour == receiveTime.Hour);
                if (receiveTime.Minute > 0)
                    transfers = transfers.Where(x => x.ReceiveTime.Value.Minute == receiveTime.Minute);
                if (receiveTime.Second > 0)
                    transfers = transfers.Where(x => x.ReceiveTime.Value.Second == receiveTime.Second);
            }

            resultCount = transfers.Count();

            pageSize = Math.Min(pageSize, resultCount);

            return transfers
                .OrderByDescending(t => t.TransferId)
                .Take(pageSize)
                .ToList();
        }

        [DisplayName("نوع صف")]
        [Category("Information")]
        public string Type => nameof(IccQueueInMemory);

        [Category("Information")]
        [DisplayName("وضعیت اتصال")]
        public bool Connected => true;
    }
}
