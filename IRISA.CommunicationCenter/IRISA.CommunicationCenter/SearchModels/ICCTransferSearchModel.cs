using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRISA.CommunicationCenter.SearchModels
{
    public class ICCTransferSearchModel
    {
        public Nullable<int> Id { get; set; }
        public string Source{ get; set; }

        public string Destination { get; set; }

        public Nullable<int> Telegram_Id { get; set; }

        public  Nullable<short> Sent { get; set; }

        public  Nullable<short> Dropped { get; set; }

        public DateTime? Send_Time { get; set; }

        public DateTime? ReciveTime { get; set; }

        public string DropReason { get; set; }
    }
}
