using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Core
{
    public interface IIccQueue
    {
        List<IccTelegram> GetTelegramsToSend();
        void Edit(IccTelegram iccTelegram);
        void Add(IccTelegram iccTelegram);
        List<IccTelegram> GetTelegrams(IccTelegramSearchModel searchModel, int pageSize, out int resultCount);
        string Type { get; }
        bool Connected { get; }
    }
}
