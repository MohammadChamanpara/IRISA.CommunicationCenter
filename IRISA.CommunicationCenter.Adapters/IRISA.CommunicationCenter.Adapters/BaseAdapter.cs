using IRISA.Log;
using System;
using System.ComponentModel;
namespace IRISA.CommunicationCenter.Adapters
{
	public abstract class BaseAdapter<DLLT> : IIccAdapter
	{
		protected TelegramDefinitions telegramDefinitions;
		protected DLLSettings<DLLT> dllSettings;
		protected IrisaEventLogger eventLogger;
		public event ReceiveEventHandler Receive;
		public event ConnectionChangedEventHandler ConnectionChanged;
		[Category("ReadOnly"), DisplayName("درحال اجرا")]
		public bool Started
		{
			get;
			private set;
		}
		[Category("ReadOnly"), DisplayName("نوع کلاینت")]
		public abstract string Type
		{
			get;
		}
		[Category("ReadOnly"), DisplayName("نام فایل پلاگین")]
		public string FileName
		{
			get
			{
				return this.dllSettings.Assembly.AsssemblyFileName();
			}
		}
		[Category("ReadOnly"), DisplayName("ورژن برنامه")]
		public string FileAssemblyVersion
		{
			get
			{
				return this.dllSettings.Assembly.AssemblyVersion();
			}
		}
		[Category("ReadOnly"), DisplayName("آدرس فایل پلاگین")]
		public string FileAddress
		{
			get
			{
				return this.dllSettings.Assembly.Location;
			}
		}
		[Category("ReadOnly"), DisplayName("نوع فایل پلاگین")]
		public string FileAssembly
		{
			get
			{
				return this.dllSettings.Assembly.AssemblyName();
			}
		}
		[DisplayName("نام کلاینت")]
		public string Name
		{
			get
			{
				return this.dllSettings.FindStringValue("Name", this.FileName);
			}
			set
			{
				this.dllSettings.SaveSetting("Name", value);
			}
		}
		[DisplayName("نام فارسی کلاینت")]
		public string PersianDescription
		{
			get
			{
				return this.dllSettings.FindStringValue("PersianDescription", "کلاینت");
			}
			set
			{
				this.dllSettings.SaveSetting("PersianDescription", value);
			}
		}
		[DisplayName("آدرس فایل تعریف ساختار تلگرام ها")]
		public string TelegramDefinitionFile
		{
			get
			{
				return this.dllSettings.FindStringValue("TelegramDefinitionFile", "TelegramDefinitions.xml");
			}
			set
			{
				this.dllSettings.SaveSetting("TelegramDefinitionFile", value);
			}
		}
		[Category("ReadOnly"), DisplayName("وضعیت اتصال کلاینت")]
		public abstract bool Connected
		{
			get;
		}
		public abstract void Send(IccTelegram iccTelegram);
		public virtual void Start(IrisaEventLogger eventLogger)
		{
			this.eventLogger = eventLogger;
			this.dllSettings = new DLLSettings<DLLT>();
			this.telegramDefinitions = new TelegramDefinitions(this.TelegramDefinitionFile);
			this.Started = true;
		}
		public virtual void Stop()
		{
			bool connected = this.Connected;
			this.Started = false;
			if (connected)
			{
				this.OnConnectionChanged(new IccCoreClientConnectionChangedEventArgs(this));
			}
		}
		public virtual void AwakeTimers()
		{
		}
		public virtual void OnReceive(ReceiveEventArgs e)
		{
			if (this.Receive != null)
			{
				this.Receive(e);
			}
		}
		public virtual void OnConnectionChanged(IccCoreClientConnectionChangedEventArgs e)
		{
			if (this.ConnectionChanged != null)
			{
				this.ConnectionChanged(e);
			}
		}
	}
}
