using System;
namespace IRISA.CommunicationCenter.Adapters
{
	public class IccCoreClientConnectionChangedEventArgs
	{
		public IIccAdapter Client
		{
			get;
			set;
		}
		public IccCoreClientConnectionChangedEventArgs(IIccAdapter client)
		{
			this.Client = client;
		}
	}
}
