using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Settings;
using IRISA.CommunicationCenter.Library.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IRISA.CommunicationCenter.Library.Adapters
{
    public abstract class BaseAdapter<DLLT> : IIccAdapter
    {
        #region Variables
        protected ITelegramDefinitions _telegramDefinitions;
        protected DLLSettings<DLLT> _dllSettings;
        protected ILogger _logger;
        private BackgroundTimer _receiveTimer;
        private BackgroundTimer _sendTimer;
        private List<long> _sentTelegrams = new List<long>();

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
                return _dllSettings.FindIntValue("TelegramReceiveInterval", 1000);
            }
            set
            {
                _dllSettings.SaveSetting("TelegramReceiveInterval", value);
            }
        }

        [DisplayName("دوره زمانی بررسی ارسال تلگرام بر حسب میلی ثانیه")]
        public int TelegramSendInterval
        {
            get
            {
                return _dllSettings.FindIntValue("TelegramSendInterval", 1000);
            }
            set
            {
                _dllSettings.SaveSetting("TelegramSendInterval", value);
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
                return _dllSettings.Assembly.AsssemblyFileName();
            }
        }

        [Category("Information"), DisplayName("ورژن برنامه")]
        public string FileAssemblyVersion
        {
            get
            {
                return _dllSettings.Assembly.AssemblyVersion();
            }
        }

        [Category("Information"), DisplayName("آدرس فایل آداپتور")]
        public string FileAddress
        {
            get
            {
                return _dllSettings.Assembly.Location;
            }
        }

        [Category("Information"), DisplayName("نوع فایل آداپتور")]
        public string FileAssembly
        {
            get
            {
                return _dllSettings.Assembly.AssemblyName();
            }
        }

        [DisplayName("نام کلاینت")]
        public string Name
        {
            get
            {
                return _dllSettings.FindStringValue("Name", FileName);
            }
            set
            {
                _dllSettings.SaveSetting("Name", value);
            }
        }

        [DisplayName("نام فارسی کلاینت")]
        public string PersianDescription
        {
            get
            {
                return _dllSettings.FindStringValue("PersianDescription", "کلاینت");
            }
            set
            {
                _dllSettings.SaveSetting("PersianDescription", value);
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

        public virtual void Start(ILogger eventLogger, ITelegramDefinitions telegramDefinitions)
        {
            _logger = eventLogger;
            _dllSettings = new DLLSettings<DLLT>();
            _telegramDefinitions = telegramDefinitions;
            _sentTelegrams = new List<long>();

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
            _sentTelegrams.Clear();
        }

        protected abstract bool CheckConnection();

        public void Send(IccTelegram iccTelegram)
        {
            sendQueue.Enqueue(iccTelegram);
            _logger.LogDebug($"تلگرام با شناسه {iccTelegram.TransferId} جهت ارسال در صف آداپتور {PersianDescription} قرار گرفت.");
        }

        private void SendTimer_DoWork()
        {
            while (sendQueue.Any())
            {
                if (!Connected)
                    return;

                var iccTelegram = sendQueue.Dequeue();
                try
                {
                    if (iccTelegram.TransferId != 0)
                    {
                        if (_sentTelegrams.Contains(iccTelegram.TransferId))
                        {
                            _logger.LogError($"تلاش برای ارسال مجدد تلگرام ارسال شده با شناسه {iccTelegram.TransferId}.");
                            continue;
                        }
                        _sentTelegrams.Add(iccTelegram.TransferId);
                    }

                    _logger.LogDebug($"ارسال تلگرام با شناسه {iccTelegram.TransferId} در آداپتور {PersianDescription} آغاز شد.");
                    SendTelegram(iccTelegram);
                    SendCompleted?.Invoke(new SendCompletedEventArgs(iccTelegram, true, null));
                }
                catch (Exception exception)
                {
                    SendCompleted?.Invoke(new SendCompletedEventArgs(iccTelegram, false, exception));
                }
            }

            if (_sentTelegrams.Count > 2000)
            {
                _sentTelegrams = _sentTelegrams
                    .OrderByDescending(x => x)
                    .Take(1000)
                    .ToList();
                _logger.LogDebug($"SentTelegrams list in {Name} truncated. Count: {_sentTelegrams.Count}. First: {_sentTelegrams.First()}. Last: {_sentTelegrams.Last()}.");
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
