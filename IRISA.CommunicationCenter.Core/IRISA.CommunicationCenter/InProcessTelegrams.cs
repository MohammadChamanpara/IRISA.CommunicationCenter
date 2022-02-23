using IRISA.CommunicationCenter.Core.Model;
using IRISA.CommunicationCenter.Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Core
{
    public interface IInProcessTelegrams
    {
        List<IccTelegram> RemoveFrom(List<IccTelegram> telegrams);
        void AddRange(IEnumerable<IccTelegram> telegrams);
        List<long> GetAllIds();
        void Remove(IccTelegram telegram);
    }

    public class InProcessTelegrams : IInProcessTelegrams
    {
        private HashSet<long> InProcessIds = new HashSet<long>();

        public InProcessTelegrams(params long[] ids)
        {
            InProcessIds.UnionWith(ids);
        }

        public void AddRange(IEnumerable<IccTelegram> telegrams)
        {
            InProcessIds.UnionWith(telegrams.Select(x => x.TransferId));
        }

        public List<IccTelegram> RemoveFrom(List<IccTelegram> telegrams)
        {
            return telegrams
               .Where(x => !InProcessIds.Contains(x.TransferId))
               .ToList();
        }

        public List<long> GetAllIds()
        {
            return InProcessIds.ToList();
        }

        public void Remove(IccTelegram telegram)
        {
            InProcessIds.Remove(telegram.TransferId);
        }
    }
}
