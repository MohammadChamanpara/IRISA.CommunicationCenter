using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Core;
using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Loggers;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IRISA.CommunicationCenter.Core
{
    public class IccCore : IIccCore
    {
        [Browsable(false)]
        public IEnumerable<IIccAdapter> ConnectedAdapters { get; private set; }

        [Browsable(false)]
        public ITransferHistory TransferHistory { get; set; }

        private DLLSettings<IccCore> dllSettings;

        private readonly object receiveLocker = new object();

        private readonly ILogger _logger;
        private readonly ITelegramDefinitions _telegramDefinitions;
        private readonly IAdapterRepository _adapterRepository;


        [Category("عملیات")]
        [DisplayName("کمترین سطح ثبت رویداد")]
        public LogLevel LogMinimumLevel
        {
            get
            {
                return dllSettings.FindEnumValue("LogMinimumLevel", LogLevel.Debug);
            }
            set
            {
                dllSettings.SaveSetting("LogMinimumLevel", value);
                _logger.SetMinumumLevel(value);
            }
        }

        [Category("اطلاعات")]
        [DisplayName("وضعیت اجرای پروسه")]
        public bool Started
        {
            get;
            private set;
        }

        [Category("اطلاعات")]
        [DisplayName("نوع فایل")]
        public string FileAssembly
        {
            get
            {
                return dllSettings.Assembly.AssemblyName();
            }
        }

        [Category("اطلاعات")]
        [DisplayName("ورژن برنامه")]
        public string FileAssemblyVersion
        {
            get
            {
                return dllSettings.Assembly.AssemblyVersion();
            }
        }

        [Category("اطلاعات")]
        [DisplayName("آدرس فایل")]
        public string FileAddress
        {
            get
            {
                return dllSettings.Assembly.Location;
            }
        }

        public IccCore(ILogger logger, ITransferHistory transferHistory, ITelegramDefinitions telegramDefinitions, IAdapterRepository adapterRepository)
        {
            _logger = logger;
            TransferHistory = transferHistory;
            _telegramDefinitions = telegramDefinitions;
            _adapterRepository = adapterRepository;
        }

        public void Start()
        {
            ConnectedAdapters = new List<IIccAdapter>();
            dllSettings = new DLLSettings<IccCore>();
            InitializeLogger();
            var recoverdTelegrams = RecoverReadyTelegramsFromTransferHistory();
            LoadAdapters();
            SendRecoveredTelegrams(recoverdTelegrams);
            Started = true;
        }

        public void Stop()
        {
            foreach (IIccAdapter adapter in ConnectedAdapters)
            {
                adapter.Stop();
            }

            if (Started)
                _logger.LogInformation($"اجرای هسته مرکزی سیستم ارتباط خاتمه یافت.");

            Started = false;
        }

        private void InitializeLogger()
        {
            _logger.LogInformation($"اجرای هسته مرکزی سیستم ارتباط آغاز شد.");
            _logger.SetMinumumLevel(LogMinimumLevel);
        }

        private void LoadAdapters()
        {
            ConnectedAdapters = _adapterRepository.GetAll();
            
            if (!ConnectedAdapters.Any())
                _logger.LogWarning("کلاینتی برای اتصال یافت نشد.");

            lock (receiveLocker)
            {
                foreach (IIccAdapter adapter in ConnectedAdapters)
                {
                    try
                    {
                        adapter.TelegramReceiveCompleted += Adapter_TelegramReceived;
                        adapter.TelegramSendCompleted += Adapter_SendCompleted;
                        adapter.Start(_logger, _telegramDefinitions);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogException(exception, "بروز خطا هنگام لود کلاینت ها. ");
                    }
                }
            }
        }

        private void Adapter_TelegramReceived(TelegramReceivedEventArgs e)
        {
            lock (receiveLocker)
            {
                try
                {
                    e.IccTelegram.TransferId = 0;
                    if (!e.Successful)
                    {
                        UpdateDroppedTelegram(e.IccTelegram, e.FailException);
                    }
                    else
                    {
                        _logger.LogDebug($"تلگرام جدید از {e.IccTelegram.Source} دریافت شد.");
                        foreach (IccTelegram iccTelegram in DuplicateTelegramByDestination(e.IccTelegram))
                        {
                            QueueTelegram(iccTelegram);
                            SendTelegram(iccTelegram, ConnectedAdapters);
                        }
                    }
                }
                catch (Exception dropException)
                {
                    UpdateDroppedTelegram(e.IccTelegram, dropException);
                }
            }
        }

        private void QueueTelegram(IccTelegram iccTelegram)
        {
            try
            {
                iccTelegram.SetAsReadyToSend();
                TransferHistory.Save(iccTelegram);
                _logger.LogInformation($"تلگرام با شناسه {iccTelegram.TransferId} در صف ارسال قرار گرفت.");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "بروز خطا هنگام ثبت تلگرام در صف ارسال. ");
            }
        }

        private void SendTelegram(IccTelegram iccTelegram, IEnumerable<IIccAdapter> adapters)
        {
            try
            {
                IIccAdapter destinationAdapter = GetDestinationAdapter(adapters, iccTelegram.Destination);
                _logger.LogDebug($"تلگرام با شناسه {iccTelegram.TransferId} جهت ارسال به آداپتور {destinationAdapter.PersianDescription} تحویل داده شد.");
                destinationAdapter.Send(iccTelegram);
            }
            catch (Exception exception)
            {
                UpdateDroppedTelegram(iccTelegram, exception);
            }
        }

        private void Adapter_SendCompleted(SendCompletedEventArgs e)
        {
            if (e.Successful)
                UpdateSentTelegram(e.IccTelegram);
            else
                UpdateDroppedTelegram(e.IccTelegram, e.FailException);
        }

        private void UpdateSentTelegram(IccTelegram iccTelegram)
        {
            try
            {
                iccTelegram.SetAsSent();
                TransferHistory.Save(iccTelegram);
                _logger.LogInformation($"تلگرام با شناسه {iccTelegram.TransferId} موفقیت آمیز به مقصد ارسال شد.");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "بروز خطا هنگام ثبت تلگرام ارسال شده. ");
            }
        }

        private void UpdateDroppedTelegram(IccTelegram iccTelegram, Exception dropException)
        {
            try
            {
                if (!(dropException is IrisaException))
                {
                    _logger.LogException(dropException, "بروز خطا منجر به حذف تلگرام شد. ");
                }

                iccTelegram.SetAsDropped(dropException?.AllInnerExceptionsMessages());
                TransferHistory.Save(iccTelegram);

                _logger.LogInformation($"تلگرام با شناسه {iccTelegram.TransferId} حذف شد. ");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "بروز خطا هنگام ثبت تلگرام حذف شده. ");
            }
        }

        private IEnumerable<IccTelegram> RecoverReadyTelegramsFromTransferHistory()
        {
            try
            {
                var telegrams =
                    TransferHistory
                        .GetTelegramsToSend()
                        .OrderBy(x => x.TransferId);

                if (telegrams.Any())
                {
                    string verb = telegrams.Count() > 1 ? "شدند" : "شد";
                    _logger.LogInformation($"{telegrams.Count()} تلگرام آماده ارسال از لیست تاریخچه بازیابی {verb}.");
                }

                return telegrams;
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "بروز خطا هنگام بازیابی تلگرام های آماده ارسال از لیست تاریخچه. ");
                throw;
            }
        }

        private void SendRecoveredTelegrams(IEnumerable<IccTelegram> telegrams)
        {
            foreach (var telegram in telegrams)
                SendTelegram(telegram, ConnectedAdapters);

            _logger.LogInformation($"{telegrams.Count()} تلگرام بازیابی شده در صف ارسال آداپتور های مقصد قرار داده شد. ");
        }

        public IIccAdapter GetDestinationAdapter(IEnumerable<IIccAdapter> adapters, string destinationName)
        {
            if (!destinationName.HasValue())
            {
                throw IrisaException.Create("مقصد تلگرام مشخص نشده است. ");
            }

            var destinationAdapter = adapters.Where(x => x.Name == destinationName);

            if (!destinationAdapter.Any())
            {
                throw IrisaException.Create($"مقصدی با نام {destinationName} وجود ندارد.");
            }

            if (destinationAdapter.Count() > 1)
            {
                throw IrisaException.Create($"چند مقصد با نام {destinationName} وجود دارد.");
            }

            return destinationAdapter.Single();
        }

        private List<IccTelegram> DuplicateTelegramByDestination(IccTelegram iccTelegram)
        {
            List<IccTelegram> iccTelegrams = new List<IccTelegram>();

            string[] destinations = iccTelegram.Destination.Split(',');

            for (int i = 0; i < destinations.Length; i++)
            {
                string text = destinations[i];
                if (!(text.Trim() == ""))
                {
                    IccTelegram item = new IccTelegram
                    {
                        Destination = text,
                        TelegramId = iccTelegram.TelegramId,
                        SendTime = iccTelegram.SendTime,
                        Source = iccTelegram.Source,
                        Body = new List<string>(iccTelegram.Body)
                    };
                    iccTelegrams.Add(item);
                }
            }

            if (destinations.Length > 1)
                _logger.LogInformation($"تلگرام دریافت شده از {iccTelegram.Source} از نوع {iccTelegram.TelegramId} به {destinations.Count()} تلگرام تقسیم شد.");

            return iccTelegrams;
        }
    }
}
