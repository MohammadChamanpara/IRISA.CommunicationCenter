using IRISA.CommunicationCenter.Core.Model;
using IRISA.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IRISA.CommunicationCenter.Core
{
    public class IccQueueInOracle : IIccQueue
    {
        private readonly DLLSettings<IccQueueInOracle> dllSettings = new DLLSettings<IccQueueInOracle>();

        [Browsable(false)]
        public EntityBusiness<Entities, IccTransfer> Transfers
        {
            get
            {
                return new EntityBusiness<Entities, IccTransfer>(new Entities(ConnectionString));
            }
        }

        public void Add(IccTelegram iccTelegram)
        {
            Transfers.Create(iccTelegram.ToIccTransfer());
        }

        public void Edit(IccTelegram iccTelegram)
        {
            Transfers.Edit(iccTelegram.ToIccTransfer());
        }

        public List<IccTelegram> GetTelegramsToSend()
        {
            return Transfers
                .GetAll()
                .Where(x => x.DROPPED == false && x.SENT == false)
                .Select(x => x.ToIccTelegram())
                .ToList();
        }

        public List<IccTelegram> GetTelegrams(int pageSize = 50)
        {
            return
                Transfers
                .GetAll()
                   .Take(pageSize)
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
                    return Transfers.Connected;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
