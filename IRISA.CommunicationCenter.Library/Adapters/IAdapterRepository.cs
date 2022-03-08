using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Library.Adapters
{
    public interface IAdapterRepository
    {
        IEnumerable<IIccAdapter> GetAll();
    }
}
