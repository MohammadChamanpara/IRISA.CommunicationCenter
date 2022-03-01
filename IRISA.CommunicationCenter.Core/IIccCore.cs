using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Core;
using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Core
{
    public interface IIccCore
    {
        bool Started { get; }
        ITransferHistory TransferHistory { get; }
        string PersianDescription { get; }
        List<IIccAdapter> ConnectedAdapters { get; }
        void Stop();
        void Start();
    }
}
