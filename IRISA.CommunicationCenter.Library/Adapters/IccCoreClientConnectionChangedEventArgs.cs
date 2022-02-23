namespace IRISA.CommunicationCenter.Library.Adapters
{
    public class AdapterConnectionChangedEventArgs
    {
        public IIccAdapter Adapter
        {
            get;
            set;
        }
        public AdapterConnectionChangedEventArgs(IIccAdapter adapter)
        {
            this.Adapter = adapter;
        }
    }
}
