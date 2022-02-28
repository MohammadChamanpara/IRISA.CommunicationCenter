using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Settings;
using IRISA.CommunicationCenter.Library.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IRISA.CommunicationCenter.Library.Adapters
{
    public abstract class BaseAdapter<DLLT> : IIccAdapter
    {
        #region Variables
        protected TelegramDefinitions telegramDefinitions;
        protected DLLSettings<DLLT> dllSettings;
        protected ILogger _logger;
        private BackgroundTimer _receiveTimer;
        private BackgroundTimer _sendTimer;

        public event Action<TelegramReceivedEventArgs> TelegramReceived;
        public event Action<IIccAdapter> ConnectionChanged;
        public event Action<SendCompletedEventArgs> SendCompleted;


        private readonly Queue<IccTelegram> sendQueue = new Queue<IccTelegram>();
        #endregion

        #region Properties

        [DisplayName("دوره زمانی بررسی دریافت تلگرام بر حسب میلی ثانیه")]
        public int TelegramReceiveInterval
        {
            get
            {
                return dllSettings.FindIntValue("TelegramReceiveInterval", 1000);
            }
            set
            {
                dllSettings.SaveSetting("TelegramReceiveInterval", value);
            }
        }

        [DisplayName("دوره زمانی بررسی ارسال تلگرام بر حسب میلی ثانیه")]
        public int TelegramSendInterval
        {
            get
            {
                return dllSettings.FindIntValue("TelegramSendInterval", 1000);
            }
            set
            {
                dllSettings.SaveSetting("TelegramSendInterval", value);
            }
        }

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

        [Category("Information"), DisplayName("نام فایل آداپتور")]
        public string FileName
        {
            get
            {
                return dllSettings.Assembly.AsssemblyFileName();
            }
        }

        [Category("Information"), DisplayName("ورژن برنامه")]
        public string FileAssemblyVersion
        {
            get
            {
                return dllSettings.Assembly.AssemblyVersion();
            }
        }

        [Category("Information"), DisplayName("آدرس فایل آداپتور")]
        public string FileAddress
        {
            get
            {
                return dllSettings.Assembly.Location;
            }
        }

        [Category("Information"), DisplayName("نوع فایل آداپتور")]
        public string FileAssembly
        {
            get
            {
                return dllSettings.Assembly.AssemblyName();
            }
        }

        [DisplayName("نام کلاینت")]
        public string Name
        {
            get
            {
                return dllSettings.FindStringValue("Name", FileName);
            }
            set
            {
                dllSettings.SaveSetting("Name", value);
            }
        }

        [DisplayName("نام فارسی کلاینت")]
        public string PersianDescription
        {
            get
            {
                return dllSettings.FindStringValue("PersianDescription", "کلاینت");
            }
            set
            {
                dllSettings.SaveSetting("PersianDescription", value);
            }
        }

        [DisplayName("آدرس فایل تعریف ساختار تلگرام ها")]
        public string TelegramDefinitionFile
        {
            get
            {
                return dllSettings.FindStringValue("TelegramDefinitionFile", "TelegramDefinitions.xml");
            }
            set
            {
                dllSettings.SaveSetting("TelegramDefinitionFile", value);
            }
        }

        private bool _connected = false;
        [Category("Information")]
        [DisplayName("وضعیت اتصال کلاینت")]
        public bool Connected
        {
            get
            {
                if (!Started)
                    return false;

                try
                {
                    Connected = CheckConnection();
                }
                catch (Exception exception)
                {
                    _logger.LogException(exception, "بروز خطا هنگام بررسی اتصال کلاینت");
                    Connected = false;
                }

                return _connected;
            }
            set
            {
                if (_connected == value)
                    return;
                _connected = value;
                ConnectionChanged?.Invoke(this);
            }
        }

        #endregion

        public virtual void Start(ILogger eventLogger)
        {
            _logger = eventLogger;
            dllSettings = new DLLSettings<DLLT>();
            telegramDefinitions = new TelegramDefinitions(TelegramDefinitionFile);

            InitializeSendTimer();
            InitializeReceiveTimer();

            Started = true;
        }

        private void InitializeSendTimer()
        {
            _sendTimer = new BackgroundTimer(_logger)
            {
                Interval = TelegramSendInterval,
                PersianDescription = $"پروسه ارسال تلگرام در {PersianDescription}"
            };
            _sendTimer.DoWork += SendTimer_DoWork;
            _sendTimer.Start();
        }

        private void InitializeReceiveTimer()
        {
            _receiveTimer = new BackgroundTimer(_logger)
            {
                Interval = TelegramReceiveInterval,
                PersianDescription = $"پروسه دریافت تلگرام در {PersianDescription}"
            };
            _receiveTimer.DoWork += ReceiveTimer_DoWork;
            _receiveTimer.Start();
        }

        public virtual void Stop()
        {
            Started = false;
            Connected = false;
            _sendTimer?.Stop();
            _receiveTimer?.Stop();
        }

        protected abstract bool CheckConnection();

        public void Send(IccTelegram iccTelegram)
        {
            sendQueue.Enqueue(iccTelegram);
            _logger.LogDebug($"تلگرام با شناسه {iccTelegram.TransferId} جهت ارسال در صف آداپتور {PersianDescription} قرار گرفت.");
        }

        private void SendTimer_DoWork()
        {
            while (sendQueue.Count > 0)
            {
                var iccTelegram = sendQueue.Dequeue();
                try
                {
                    _logger.LogDebug($"ارسال تلگرام با شناسه {iccTelegram.TransferId} در آداپتور {PersianDescription} آغاز شد.");
                    SendTelegram(iccTelegram);
                    SendCompleted?.Invoke(new SendCompletedEventArgs(iccTelegram, true, null));
                }
                catch (Exception exception)
                {
                    SendCompleted?.Invoke(new SendCompletedEventArgs(iccTelegram, false, exception));
                }
            }
        }
        protected abstract void ReceiveTimer_DoWork();

        protected abstract void SendTelegram(IccTelegram iccTelegram);

        public virtual void OnTelegramReceived(TelegramReceivedEventArgs e)
        {
            TelegramReceived?.Invoke(e);
        }
    }
}
