using IRISA.CommunicationCenter.Adapters;
using IRISA.CommunicationCenter.Model;
using IRISA.Log;
using IRISA.Model;
using IRISA.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
namespace IRISA.CommunicationCenter
{
	public class IccCore
	{
		public delegate void IccCoreTelegramEventHandler(IccCoreTelegramEventArgs e);
		public List<IIccAdapter> connectedClients;
		private IrisaBackgroundTimer sendTimer;
		private IrisaBackgroundTimer activatorTimer;
		private DLLSettings<IccCore> dllSettings;
		public IccEventLogger eventLogger = new IccEventLogger();
		public event IccCore.IccCoreTelegramEventHandler TelegramQueued;
		public event IccCore.IccCoreTelegramEventHandler TelegramSent;
		public event IccCore.IccCoreTelegramEventHandler TelegramDropped;
		[Browsable(false)]
		public EntityBusiness<Entities, IccEvent> Events
		{
			get
			{
				return new EntityBusiness<Entities, IccEvent>(new Entities(this.ConnectionString));
			}
		}
		[Browsable(false)]
		public EntityBusiness<Entities, IccTransfer> Transfers
		{
			get
			{
				return new EntityBusiness<Entities, IccTransfer>(new Entities(this.ConnectionString));
			}
		}
		[DisplayName("شرح فارسی هسته مرکزی سیستم ارتباط")]
		public string PersianDescription
		{
			get
			{
				return this.dllSettings.FindStringValue("PersianDescription", "هسته مرکزی سیستم ارتباط");
			}
			set
			{
				this.dllSettings.SaveSetting("PersianDescription", value);
			}
		}
		[DisplayName("شرح فارسی پروسه فعال ساز")]
		public string ActivatorTimerPersianDescription
		{
			get
			{
				return this.dllSettings.FindStringValue("ActivatorTimerPersianDescription", "پروسه فعال ساز");
			}
			set
			{
				this.dllSettings.SaveSetting("ActivatorTimerPersianDescription", value);
			}
		}
		[DisplayName("شرح فارسی پروسه ارسال تلگرام")]
		public string SendTimerPersianDescription
		{
			get
			{
				return this.dllSettings.FindStringValue("SendTimerPersianDescription", "پروسه ارسال تلگرام");
			}
			set
			{
				this.dllSettings.SaveSetting("SendTimerPersianDescription", value);
			}
		}
		[DisplayName("کاراکتر جدا کننده فیلد های تلگرام")]
		public char BodySeparator
		{
			get
			{
				return this.dllSettings.FindCharacterValue("BodySeparator", '$');
			}
			set
			{
				this.dllSettings.SaveSetting("BodySeparator", value);
			}
		}
		[DisplayName("کاراکتر جدا کننده بین مقصد های تلگرام")]
		public char DestinationSeparator
		{
			get
			{
				return this.dllSettings.FindCharacterValue("DestinationSeparator", ',');
			}
			set
			{
				this.dllSettings.SaveSetting("DestinationSeparator", value);
			}
		}
		[DisplayName("دوره زمانی ارسال تلگرام بر حسب میلی ثانیه")]
		public int SendTimerInterval
		{
			get
			{
				return this.dllSettings.FindIntValue("sendTimerInterval", 2000);
			}
			set
			{
				this.dllSettings.SaveSetting("sendTimerInterval", value);
			}
		}
		[DisplayName("دوره زمانی فعال نمودن پروسه های متوقف شده بر حسب میلی ثانیه")]
		public int ActivatorTimerInterval
		{
			get
			{
				return this.dllSettings.FindIntValue("activatorTimerInterval", 20000);
			}
			set
			{
				this.dllSettings.SaveSetting("activatorTimerInterval", value);
			}
		}
		[DisplayName("رشته اتصال به پایگاه داده")]
		public string ConnectionString
		{
			get
			{
				return this.dllSettings.FindConnectionString();
			}
			set
			{
				this.dllSettings.SaveConnectionString(value);
			}
		}
		[Category("ReadOnly"), DisplayName("وضعیت اجرای پروسه")]
		public bool Started
		{
			get;
			private set;
		}
		[Category("ReadOnly"), DisplayName("نوع فایل")]
		public string FileAssembly
		{
			get
			{
				return this.dllSettings.Assembly.AssemblyName();
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
		[Category("ReadOnly"), DisplayName("آدرس فایل")]
		public string FileAddress
		{
			get
			{
				return this.dllSettings.Assembly.Location;
			}
		}
		private void sendTimer_DoWork(object sender, DoWorkEventArgs e)
		{
			this.CheckDataBaseConnection();
			List<IccTransfer> list = (
				from t in this.Transfers.GetAll()
				where t.SENT==false  && t.DROPPED==false
				select t).OrderBy(x=>x.ID).Take(100).ToList<IccTransfer>();
			foreach (IccTransfer transfer in list)
			{
				try
				{
					IccTelegram iccTelegram = this.IccTransferToIccTelegram(transfer);
					this.ValidationForSend(iccTelegram);
					IIccAdapter iccAdapter = (
						from c in this.connectedClients
						where c.Name == transfer.DESTINATION
						select c).Single<IIccAdapter>();
					if (iccAdapter.Connected)
					{
						iccAdapter.Send(iccTelegram);
						transfer.DROPPED =false;
						transfer.DROP_REASON = null;
						transfer.RECEIVE_TIME = new DateTime?(DateTime.Now);
						transfer.SENT = true;
						this.Transfers.Edit(transfer);
						this.eventLogger.LogSuccess("تلگرام با شناسه رکورد {0} موفقیت آمیز به مقصد ارسال شد.", new object[]
						{
							transfer.ID
						});
						if (this.TelegramSent != null)
						{
							this.TelegramSent(new IccCoreTelegramEventArgs(iccTelegram));
						}
					}
				}
				catch (Exception ex)
				{
					if (transfer.SENT ==false)
					{
						this.DropTelegram(transfer, ex, true);
					}
					else
					{
						this.eventLogger.LogException(ex);
					}
				}
			}
		}
		private void activatorTimer_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				this.sendTimer.Awake();
				if (this.connectedClients != null)
				{
					foreach (IIccAdapter current in this.connectedClients)
					{
						current.AwakeTimers();
					}
				}
			}
			catch (Exception exception)
			{
				this.eventLogger.LogWarning("بروز خطا هنگام فعال سازی پروسه ها", new object[0]);
				this.eventLogger.LogException(exception);
			}
		}
		private void client_OnReceive(ReceiveEventArgs e)
		{
			bool flag = false;
			try
			{
				Monitor.Enter(this, ref flag);
				try
				{
					if (!e.Successful)
					{
						this.DropTelegram(e.IccTelegram, e.FailException, false);
					}
					else
					{
						foreach (IccTelegram current in this.DuplicateTelegramByDestination(e.IccTelegram))
						{
							this.QueueTelegram(current);
						}
					}
				}
				catch (Exception dropException)
				{
					this.DropTelegram(e.IccTelegram, dropException, false);
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this);
				}
			}
		}
		public void Start()
		{
			try
			{
				this.connectedClients = new List<IIccAdapter>();
				this.dllSettings = new DLLSettings<IccCore>();
				this.eventLogger.DbLoggerEnabled = true;
				if (!this.Transfers.Connected)
				{
					this.eventLogger.DbLoggerEnabled = false;
				}
				else
				{
					this.eventLogger.LogInfo("اجرای {0} آغاز شد.", new object[]
					{
						this.PersianDescription
					});
					this.LoadClients();
					if (this.sendTimer != null)
					{
						this.sendTimer.Stop();
					}
					this.sendTimer = new IrisaBackgroundTimer();
					this.sendTimer.Interval = this.SendTimerInterval;
					this.sendTimer.DoWork += new DoWorkEventHandler(this.sendTimer_DoWork);
					this.sendTimer.PersianDescription = this.SendTimerPersianDescription + " در " + this.PersianDescription;
					this.sendTimer.EventLogger = this.eventLogger;
					this.sendTimer.Start();
					if (this.activatorTimer != null)
					{
						this.activatorTimer.Stop();
					}
					this.activatorTimer = new IrisaBackgroundTimer();
					this.activatorTimer.Interval = this.ActivatorTimerInterval;
					this.activatorTimer.DoWork += new DoWorkEventHandler(this.activatorTimer_DoWork);
					this.activatorTimer.PersianDescription = this.ActivatorTimerPersianDescription + " در " + this.PersianDescription;
					this.activatorTimer.EventLogger = this.eventLogger;
					this.activatorTimer.Start();
					this.Started = true;
				}
			}
			catch (Exception exception)
			{
				this.Started = false;
				this.eventLogger.LogException(exception);
			}
		}
		public void Stop()
		{
			if (this.sendTimer != null)
			{
				this.sendTimer.Stop();
			}
			if (this.activatorTimer != null)
			{
				this.activatorTimer.Stop();
			}
			foreach (IIccAdapter current in this.connectedClients)
			{
				current.Stop();
			}
			if (this.Started)
			{
				this.eventLogger.LogInfo("اجرای {0} خاتمه یافت.", new object[]
				{
					this.PersianDescription
				});
			}
			this.Started = false;
		}
		private void CheckDataBaseConnection()
		{
			if (!this.Transfers.Connected)
			{
				throw HelperMethods.CreateException("برنامه قادر به دسترسی به پایگاه داده {0} نمی باشد", new object[]
				{
					this.PersianDescription
				});
			}
		}
		private void LoadClients()
		{
			this.connectedClients = HelperMethods.LoadPlugins<IIccAdapter>();
			if (this.connectedClients.Count == 0)
			{
				this.eventLogger.LogWarning("کلاینتی برای اتصال یافت نشد.", new object[0]);
			}
			foreach (IIccAdapter current in this.connectedClients)
			{
				try
				{
					current.Receive += new ReceiveEventHandler(this.client_OnReceive);
					current.Start(this.eventLogger);
				}
				catch (Exception exception)
				{
					this.eventLogger.LogException(exception);
				}
			}
		}
		private void ValidationForSend(IccTelegram iccTelegram)
		{
			if (!iccTelegram.Destination.HasValue())
			{
				throw HelperMethods.CreateException("مقصد تلگرام مشخص نشده است.", new object[0]);
			}
			IEnumerable<IIccAdapter> source = 
				from c in this.connectedClients
				where c.Name == iccTelegram.Destination
				select c;
			if (source.Count<IIccAdapter>() == 0)
			{
				throw HelperMethods.CreateException("مقصد مشخص شده وجود ندارد.", new object[0]);
			}
			if (source.Count<IIccAdapter>() > 1)
			{
				throw HelperMethods.CreateException("چند مقصد با نام داده شده وجود دارد.", new object[0]);
			}
		}
		private void DropTelegram(IccTelegram iccTelegram, Exception dropException, bool existingRecord)
		{
			this.DropTelegram(this.IccTelegramToIccTransfer(iccTelegram), dropException, existingRecord);
		}
		private void DropTelegram(IccTransfer iccTransfer, Exception dropException, bool existingRecord)
		{
			try
			{
				bool flag = false;
				try
				{
					Monitor.Enter(this, ref flag);
					if (!(dropException is IrisaException))
					{
						this.eventLogger.LogException(dropException);
					}
					iccTransfer.SENT = false;
					iccTransfer.DROPPED = true;
					iccTransfer.DROP_REASON = dropException.MostInnerException().Message;
					if (existingRecord)
					{
						this.Transfers.Edit(iccTransfer);
					}
					else
					{
						this.Transfers.Create(iccTransfer);
						iccTransfer = (
							from t in this.Transfers.GetAll()
							orderby t.ID descending
							select t).First<IccTransfer>();
					}
					this.eventLogger.LogInfo("تلگرام با شناسه {0} حذف شد.", new object[]
					{
						iccTransfer.ID
					});
					if (this.TelegramDropped != null)
					{
						this.TelegramDropped(new IccCoreTelegramEventArgs(this.IccTransferToIccTelegram(iccTransfer)));
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(this);
					}
				}
			}
			catch (Exception exception)
			{
				this.eventLogger.LogException(exception);
			}
		}
		private void QueueTelegram(IccTelegram iccTelegram)
		{
			bool flag = false;
			try
			{
				IccTransfer iccTransfer = this.IccTelegramToIccTransfer(iccTelegram);
				iccTransfer.DROPPED = false;
				iccTransfer.DROP_REASON = null;
				iccTransfer.SENT = false;
				this.Transfers.Create(iccTransfer);
				flag = true;
				iccTransfer = (
					from t in this.Transfers.GetAll()
					orderby t.ID descending
					select t).First<IccTransfer>();
				this.eventLogger.LogSuccess("تلگرام با شناسه رکورد {0} در صف ارسال قرار گرفت.", new object[]
				{
					iccTransfer.ID
				});
				if (this.TelegramQueued != null)
				{
					this.TelegramQueued(new IccCoreTelegramEventArgs(iccTelegram));
				}
			}
			catch (Exception ex)
			{
				if (!flag)
				{
					this.DropTelegram(iccTelegram, ex, false);
				}
				else
				{
					this.eventLogger.LogException(ex);
				}
			}
		}
		public IccTelegram IccTransferToIccTelegram(IccTransfer iccTransfer)
		{
			IccTelegram iccTelegram = new IccTelegram();
			iccTelegram.Destination = iccTransfer.DESTINATION;
			iccTelegram.TelegramId = (iccTransfer.TELEGRAM_ID.HasValue ? iccTransfer.TELEGRAM_ID.Value : 0);
			iccTelegram.TransferId = iccTransfer.ID;
			iccTelegram.SendTime = iccTransfer.SEND_TIME;
			iccTelegram.Source = iccTransfer.SOURCE;
			iccTelegram.SetBodyString(iccTransfer.BODY, this.BodySeparator);
			return iccTelegram;
		}
		public IccTransfer IccTelegramToIccTransfer(IccTelegram iccTelegram)
		{
			return new IccTransfer
			{
				TELEGRAM_ID = new int?(iccTelegram.TelegramId),
				SOURCE = iccTelegram.Source,
				DESTINATION = iccTelegram.Destination,
				SEND_TIME = iccTelegram.SendTime,
				BODY = iccTelegram.GetBodyString(this.BodySeparator)
			};
		}
		private List<IccTelegram> DuplicateTelegramByDestination(IccTelegram iccTelegram)
		{
			List<IccTelegram> list = new List<IccTelegram>();
			string[] array = iccTelegram.Destination.Split(new char[]
			{
				this.DestinationSeparator
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
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
					list.Add(item);
				}
			}
			return list;
		}
	}
}
