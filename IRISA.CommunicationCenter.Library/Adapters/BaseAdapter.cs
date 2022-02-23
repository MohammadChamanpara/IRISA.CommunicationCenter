using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IRISA.CommunicationCenter.Library.Adapters
{
    public abstract class BaseAdapter<DLLT> : IIccAdapter
    {
        #region Variables
        protected TelegramDefinitions telegramDefinitions;
        protected DLLSettings<DLLT> dllSettings;
        protected ILogger eventLogger;
        public event ReceiveEventHandler Receive;
        public event EventHandler<AdapterConnectionChangedEventArgs> ConnectionChanged;
        public event EventHandler<SendCompletedEventArgs> SendCompleted;

        private Queue<IccTelegram> sendQueue = new Queue<IccTelegram>();
        #endregion


        #region Properties
        [Category("Information"), DisplayName("درحال اجرا")]
        public bool Started
        {
            get;
            private set;
        }

        [Category("Information"), DisplayName("نوع کلاینت")]
        public abstract string Type
        {
            get;
        }

        [Category("Information"), DisplayName("نام فایل پلاگین")]
        public string FileName
        {
            get
            {
                return this.dllSettings.Assembly.AsssemblyFileName();
            }
        }
        [Category("Information"), DisplayName("ورژن برنامه")]
        public string FileAssemblyVersion
        {
            get
            {
                return this.dllSettings.Assembly.AssemblyVersion();
            }
        }
        [Category("Information"), DisplayName("آدرس فایل پلاگین")]
        public string FileAddress
        {
            get
            {
                return this.dllSettings.Assembly.Location;
            }
        }
        [Category("Information"), DisplayName("نوع فایل پلاگین")]
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
        [Category("Information"), DisplayName("وضعیت اتصال کلاینت")]
        public abstract bool Connected
        {
            get;
        }
        #endregion


        public void Send(IccTelegram iccTelegram)
        {
            this.sendQueue.Enqueue(iccTelegram);
        }

        private async Task KeepSending()
        {
            while (Started)
            {
                while (sendQueue.Count > 0)
                {
                    var iccTelegram = sendQueue.Dequeue();
                    try
                    {
                        SendTelegram(iccTelegram);
                        OnTelegramSendCompleted(new SendCompletedEventArgs(iccTelegram, true, null));
                    }
                    catch (Exception exception)
                    {
                        OnTelegramSendCompleted(new SendCompletedEventArgs(iccTelegram, false, exception));
                    }
                }
                await Task.Delay(500);
            }
        }
        protected void OnTelegramSendCompleted(SendCompletedEventArgs e)
        {
            SendCompleted?.Invoke(this, e);
        }

        protected abstract void SendTelegram(IccTelegram iccTelegram);
        public virtual void Start(ILogger eventLogger)
        {
            this.eventLogger = eventLogger;
            this.dllSettings = new DLLSettings<DLLT>();
            this.telegramDefinitions = new TelegramDefinitions(this.TelegramDefinitionFile);
            this.Started = true;
            Task.Run(() => KeepSending());
        }
        public virtual void Stop()
        {
            bool connected = this.Connected;
            this.Started = false;
            if (connected)
            {
                this.OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
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
        public virtual void OnConnectionChanged(AdapterConnectionChangedEventArgs e)
        {
            ConnectionChanged?.Invoke(this, e);
        }
    }
}
