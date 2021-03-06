using IRISA.CommunicationCenter.Library.Models;
using System;

namespace IRISA.CommunicationCenter.Library.Adapters
{
    public class SendCompletedEventArgs : EventArgs
    {
        public SendCompletedEventArgs(IccTelegram iccTelegram, Boolean successful, Exception failException = null)
        {
            IccTelegram = iccTelegram;
            Successful = successful;
            FailException = failException;
        }
        public IccTelegram IccTelegram { get; set; }
        public Boolean Successful { get; set; }
        public Exception FailException { get; set; }
    }
}
