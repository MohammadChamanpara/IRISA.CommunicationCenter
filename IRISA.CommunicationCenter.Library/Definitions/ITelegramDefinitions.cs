using IRISA.CommunicationCenter.Library.Models;

namespace IRISA.CommunicationCenter.Library.Definitions
{
    public interface ITelegramDefinitions
    {
        ITelegramDefinition Find(IccTelegram iccTelegram);
    }
}
