using System;

namespace IRISA.CommunicationCenter
{
    public class IccTelegramSearchModel
    {
        public long? TransferId { get; set; }
        public int? TelegramId { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public bool? Sent { get; set; }
        public bool? Dropped { get; set; }
        public string DropReason { get; set; }
    }
}
