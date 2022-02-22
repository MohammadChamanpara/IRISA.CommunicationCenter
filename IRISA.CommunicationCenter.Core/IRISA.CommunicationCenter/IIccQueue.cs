using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace IRISA.CommunicationCenter.Core
{
    public interface IIccQueue
    {
        List<IccTelegram> GetTelegramsToSend();
        void Edit(IccTelegram iccTelegram);
        void Add(IccTelegram iccTelegram);
        List<IccTelegram> GetTelegrams(int pagesize = 50);
        string Type { get; }
        bool Connected { get; }
    }
}
