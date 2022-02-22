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

        public List<IccTelegram> GetTelegrams(int pageSize = 50)
        {
            return items
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
