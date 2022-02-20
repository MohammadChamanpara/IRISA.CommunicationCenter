using IRISA.CommunicationCenter.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Core
{
    public interface IInProcessTelegrams
    {
        List<IccTransfer> RemoveFrom(List<IccTransfer> telegrams);
        void AddRange(IEnumerable<IccTransfer> telegrams);
        void RemoveRange(IEnumerable<IccTransfer> completedTelegrams);
        List<long> GetAllIds();
        void Remove(IccTransfer telegram);
    }

    public class InProcessTelegrams : IInProcessTelegrams
    {
        private HashSet<long> InProcessIds = new HashSet<long>();

        public InProcessTelegrams(params long[] ids)
        {
            InProcessIds.UnionWith(ids);
        }

        public void AddRange(IEnumerable<IccTransfer> telegrams)
        {
            InProcessIds.UnionWith(telegrams.Select(x => x.ID));
        }

        public List<IccTransfer> RemoveFrom(List<IccTransfer> telegrams)
        {
            return telegrams
               .Where(x => !InProcessIds.Contains(x.ID))
               .ToList();
        }

        public void RemoveRange(IEnumerable<IccTransfer> completedTelegrams)
        {
            var completedIds = completedTelegrams.Select(telegram => telegram.ID);

            InProcessIds.RemoveWhere
            (
                inProcess => completedIds.Contains(inProcess)
            );
        }

        public List<long> GetAllIds()
        {
            return InProcessIds.ToList();
        }

        public void Remove(IccTransfer telegram)
        {
            InProcessIds.Remove(telegram.ID);
        }
    }
}
