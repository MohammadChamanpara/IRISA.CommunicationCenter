using System.Collections.Generic;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Adapters
{
    public class AdapterRepositoryInFileForDebug : AdapterRepositoryInFile
    {
        public override IEnumerable<IIccAdapter> GetAll()
        {
            var adapters = base.GetAll().ToList();
            adapters.AddRange(LoadAdaptersFromPath(@"C:\Projects\ICC\IRISA.CommunicationCenter.Adapters.TestAdapter\bin\Debug"));
            adapters.AddRange(LoadAdaptersFromPath(@"C:\Projects\ICC\IRISA.CommunicationCenter.Adapters.TcpIp.Wasco\bin\Debug"));
            adapters.AddRange(LoadAdaptersFromPath(@"C:\Projects\ICC\IRISA.CommunicationCenter.Adapters.Database.Oracle\bin\Debug"));
            return adapters;
        }
    }
}
