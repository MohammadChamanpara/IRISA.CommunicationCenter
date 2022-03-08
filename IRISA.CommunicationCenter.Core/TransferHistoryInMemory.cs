using IRISA.CommunicationCenter.Library.Core;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IRISA.CommunicationCenter.Core
{
    public class TransferHistoryInMemory : ITransferHistory
    {
        private static ConcurrentBag<IccTelegram> _iccTelegrams = new ConcurrentBag<IccTelegram>();
        private static long _id = 1;


        public TransferHistoryInMemory()
        {
            _iccTelegrams.Add(
                new IccTelegram()
                {
                    TransferId = -2,
                    Source = "Mamad",
                    Destination = "Behnam",
                    TelegramId = 2,
                    SendTime = DateTime.Now.AddMinutes(-10)
                });
            _iccTelegrams.Add(
                new IccTelegram()
                {
                    TransferId = -1,
                    Source = "Mamad",
                    Destination = "Behnam",
                    TelegramId = 2,
                    SendTime = DateTime.Now.AddMinutes(-11)
                });
        }

        public void Save(IccTelegram iccTelegram)
        {
            if (iccTelegram.TransferId == 0)
                Add(iccTelegram);
        }

        private void Add(IccTelegram iccTelegram)
        {
            iccTelegram.TransferId = _id++;
            _iccTelegrams.Add(iccTelegram);

            int readytelegrams = _iccTelegrams.Where(x => x.IsReadyToSend).Count();

            int limit = 1000000;
            if (_iccTelegrams.Count > limit * 2)
                _iccTelegrams = new ConcurrentBag<IccTelegram>
                (
                    _iccTelegrams
                    .OrderByDescending(x => x.TransferId)
                    .Take(Math.Max(readytelegrams, limit))
                );
        }

        public List<IccTelegram> GetTelegramsToSend()
        {
            return _iccTelegrams
                .Where(x => x.IsReadyToSend)
                .ToList();
        }

        public List<IccTelegram> GetTelegrams(IccTelegramSearchModel searchModel, int pageSize, out int resultCount)
        {
            var telegrams = _iccTelegrams.AsEnumerable();

            /*.................... Transfer Id ....................*/
            telegrams = searchModel.TransferId.HasValue
                ? telegrams.Where(x => x.TransferId == searchModel.TransferId)
                : telegrams;

            /*.................... Telegram Id ....................*/

            telegrams = searchModel.TelegramId.HasValue
                 ? telegrams.Where(x => x.TelegramId == searchModel.TelegramId)
                 : telegrams;

            /*.................... Source ....................*/

            telegrams = searchModel.Source.HasValue()
                ? telegrams.Where(x => x.Source.ToLower().Contains(searchModel.Source.ToLower()))
                : telegrams;

            /*.................... Destination ....................*/

            telegrams = searchModel.Destination.HasValue()
                ? telegrams.Where(x => x.Destination?.ToLower()?.Contains(searchModel.Destination.ToLower()) == true)
                : telegrams;

            /*.................... Sent ....................*/

            telegrams = searchModel.Sent.HasValue
                ? telegrams.Where(x => x.Sent == searchModel.Sent)
                : telegrams;

            /*.................... Dropped....................*/

            telegrams = searchModel.Dropped.HasValue
                ? telegrams.Where(x => x.Dropped == searchModel.Dropped)
                : telegrams;

            /* ................Drop Reason...................*/

            telegrams = searchModel.DropReason.HasValue()
                ? telegrams.Where(x => x.DropReason?.Contains(searchModel.DropReason) == true)
                : telegrams;

            /*....................Send Time....................*/

            if (searchModel.SendTime.HasValue)
            {
                var sendTime = searchModel.SendTime.Value;
                telegrams = telegrams.Where
                (
                    x =>
                    x.SendTime.Year == sendTime.Year &&
                    x.SendTime.Month == sendTime.Month &&
                    x.SendTime.Day == sendTime.Day
                );

                if (sendTime.Hour > 0)
                    telegrams = telegrams.Where(x => x.SendTime.Hour == sendTime.Hour);
                if (sendTime.Minute > 0)
                    telegrams = telegrams.Where(x => x.SendTime.Minute == sendTime.Minute);
                if (sendTime.Second > 0)
                    telegrams = telegrams.Where(x => x.SendTime.Second == sendTime.Second);
            }

            /*....................Receive Time....................*/

            if (searchModel.ReceiveTime.HasValue)
            {
                var receiveTime = searchModel.ReceiveTime.Value;
                telegrams = telegrams.Where
                (
                    x =>
                    x.ReceiveTime.Value.Year == receiveTime.Year &&
                    x.ReceiveTime.Value.Month == receiveTime.Month &&
                    x.ReceiveTime.Value.Day == receiveTime.Day
                );

                if (receiveTime.Hour > 0)
                    telegrams = telegrams.Where(x => x.ReceiveTime.Value.Hour == receiveTime.Hour);
                if (receiveTime.Minute > 0)
                    telegrams = telegrams.Where(x => x.ReceiveTime.Value.Minute == receiveTime.Minute);
                if (receiveTime.Second > 0)
                    telegrams = telegrams.Where(x => x.ReceiveTime.Value.Second == receiveTime.Second);
            }

            resultCount = telegrams.Count();

            pageSize = Math.Min(pageSize, resultCount);

            return telegrams
                .OrderByDescending(t => t.TransferId)
                .Take(pageSize)
                .ToList();
        }

        [DisplayName("نوع صف")]
        [Category("اطلاعات")]
        public string Type => nameof(TransferHistoryInMemory);

        [Category("اطلاعات")]
        [DisplayName("وضعیت اتصال")]
        public bool Connected => true;
    }
}
