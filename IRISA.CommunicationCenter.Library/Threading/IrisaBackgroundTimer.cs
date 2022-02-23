using IRISA.CommunicationCenter.Library.Logging;
using System;
using System.ComponentModel;
using System.Threading;
namespace IRISA.CommunicationCenter.Library.Threading
{
    public class IrisaBackgroundTimer : BackgroundWorker
    {
        private enum ProcessStatus
        {
            Created,
            Running,
            JobCompleted,
            ExceptionOccured
        }
        private ManualResetEvent intervalManualReset;
        private ProcessStatus processStatus = ProcessStatus.Created;
        private DateTime startTime = default;
        public ILogger EventLogger
        {
            get;
            set;
        }
        public int Interval
        {
            get;
            set;
        }
        public int AliveTime
        {
            get;
            set;
        }
        public string PersianDescription
        {
            get;
            set;
        }
        public IrisaBackgroundTimer()
        {
            AliveTime = 0;
            processStatus = ProcessStatus.Created;
            WorkerSupportsCancellation = true;
            Interval = 500;
        }
        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            base.OnRunWorkerCompleted(e);
            if (EventLogger != null && processStatus == ProcessStatus.ExceptionOccured)
            {
                EventLogger.LogWarning("اجرای {0} خاتمه یافت.", new object[]
                {
                    PersianDescription
                });
            }
            processStatus = ProcessStatus.JobCompleted;
        }
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            while (!CancellationPending)
            {
                try
                {
                    if (AliveTime > 0 && (DateTime.Now - startTime).TotalMilliseconds >= AliveTime)
                    {
                        Stop();
                    }
                    base.OnDoWork(e);
                    intervalManualReset.WaitOne(Interval);
                }
                catch (Exception ex)
                {
                    if (EventLogger != null)
                    {
                        EventLogger.LogException(ex);
                    }
                    processStatus = ProcessStatus.ExceptionOccured;
                    Stop();
                    if (EventLogger == null)
                    {
                        throw ex;
                    }
                }
            }
            if (e != null)
            {
                e.Cancel = true;
            }
        }
        public virtual void Stop()
        {
            CancelAsync();
            if (intervalManualReset != null)
            {
                intervalManualReset.Set();
            }
            Dispose(true);
        }
        public virtual void Start()
        {
            startTime = DateTime.Now;
            processStatus = ProcessStatus.Running;
            if (!IsBusy)
            {
                intervalManualReset = new ManualResetEvent(false);
                RunWorkerAsync();
            }
        }
        public virtual void Awake()
        {
            if (!IsBusy && EventLogger != null)
            {
                EventLogger.LogSuccess("اجرای {0} توسط پروسه فعال ساز فعال شد.", new object[]
                {
                    PersianDescription
                });
            }
            Start();
        }
    }
}
