using IRISA.CommunicationCenter.Library.Models;
using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Library.Core
{
    public interface ITransferHistory
    {
        List<IccTelegram> GetTelegramsToSend();
        void Save(IccTelegram iccTelegram);
        List<IccTelegram> GetTelegrams(IccTelegramSearchModel searchModel, int pageSize, out int resultCount);
        string Type { get; }
        bool Connected { get; }
    }
}
