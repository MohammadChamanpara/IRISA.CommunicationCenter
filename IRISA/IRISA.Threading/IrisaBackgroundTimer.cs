using IRISA.Loggers;
using System;
using System.ComponentModel;
using System.Threading;
namespace IRISA.Threading
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
		private IrisaBackgroundTimer.ProcessStatus processStatus = IrisaBackgroundTimer.ProcessStatus.Created;
		private DateTime startTime = default(DateTime);
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
			this.AliveTime = 0;
			this.processStatus = IrisaBackgroundTimer.ProcessStatus.Created;
			base.WorkerSupportsCancellation = true;
			this.Interval = 500;
		}
		protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
		{
			base.OnRunWorkerCompleted(e);
			if (this.EventLogger != null && this.processStatus == IrisaBackgroundTimer.ProcessStatus.ExceptionOccured)
			{
				this.EventLogger.LogWarning("اجرای {0} خاتمه یافت.", new object[]
				{
					this.PersianDescription
				});
			}
			this.processStatus = IrisaBackgroundTimer.ProcessStatus.JobCompleted;
		}
		protected override void OnDoWork(DoWorkEventArgs e)
		{
			while (!base.CancellationPending)
			{
				try
				{
					if (this.AliveTime > 0 && (DateTime.Now - this.startTime).TotalMilliseconds >= (double)this.AliveTime)
					{
						this.Stop();
					}
					base.OnDoWork(e);
					this.intervalManualReset.WaitOne(this.Interval);
				}
				catch (Exception ex)
				{
					if (this.EventLogger != null)
					{
						this.EventLogger.LogException(ex);
					}
					this.processStatus = IrisaBackgroundTimer.ProcessStatus.ExceptionOccured;
					this.Stop();
					if (this.EventLogger == null)
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
			base.CancelAsync();
			if (this.intervalManualReset != null)
			{
				this.intervalManualReset.Set();
			}
			this.Dispose(true);
		}
		public virtual void Start()
		{
			this.startTime = DateTime.Now;
			this.processStatus = IrisaBackgroundTimer.ProcessStatus.Running;
			if (!base.IsBusy)
			{
				this.intervalManualReset = new ManualResetEvent(false);
				base.RunWorkerAsync();
			}
		}
		public virtual void Awake()
		{
			if (!base.IsBusy && this.EventLogger != null)
			{
				this.EventLogger.LogSuccess("اجرای {0} توسط پروسه فعال ساز فعال شد.", new object[]
				{
					this.PersianDescription
				});
			}
			this.Start();
		}
	}
}
