using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Loggers;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
namespace IRISA.CommunicationCenter.Library.Adapters
{
    public abstract class TcpIpBaseAdapter<DllT> : BaseAdapter<DllT>
    {
        #region Properties
        private TcpListener tcpListener;
        private Socket socket;
        private IrisaBackgroundTimer clientDetectTimer;
        private IrisaBackgroundTimer receiveTimer;
        protected List<byte> receivedBuffer = new List<byte>();
        private DateTime lastConnectionTime;

        [Category("Information")]
        public override string Type
        {
            get
            {
                return "Tcp/Ip";
            }
        }
        
        [Category("Operation")]
        [DisplayName("کاراکتر آغاز کننده تلگرام")]
        public char StartCharacter
        {
            get
            {
                return this.dllSettings.FindCharacterValue("StartCharacter", '$');
            }
            set
            {
                this.dllSettings.SaveSetting("StartCharacter", value);
            }
        }

        [Category("Operation")]
        [DisplayName("کاراکتر خاتمه دهنده تلگرام")]
        public char EndCharacter
        {
            get
            {
                return this.dllSettings.FindCharacterValue("EndCharacter", '#');
            }
            set
            {
                this.dllSettings.SaveSetting("EndCharacter", value);
            }
        }
        
        [Category("Operation")]
        [DisplayName("سایز هدر تلگرام بر حسب بایت")]
        public int HeaderSize
        {
            get
            {
                return this.dllSettings.FindIntValue("HeaderSize", 27);
            }
            set
            {
                this.dllSettings.SaveSetting("HeaderSize", value);
            }
        }

        [Category("Operation")]
        [DisplayName("شناسه کلاینت")]
        public int Id
        {
            get
            {
                return this.dllSettings.FindIntValue("Id", 0);
            }
            set
            {
                this.dllSettings.SaveSetting("Id", value);
            }
        }

        [Category("Operation")]
        [DisplayName("آدرس کلاینت")]
        public string Ip
        {
            get
            {
                return this.dllSettings.FindStringValue("Ip", "127.0.0.1");
            }
            set
            {
                this.dllSettings.SaveSetting("Ip", value);
            }
        }

        [Category("Operation")]
        [DisplayName("شماره پورت ارتباطی")]
        public int Port
        {
            get
            {
                return this.dllSettings.FindIntValue("port", 6000);
            }
            set
            {
                this.dllSettings.SaveSetting("port", value);
            }
        }

        [Category("Operation")]
        [DisplayName("شرح فارسی پروسه تشخیص کلاینت")]
        public string ClientDetectTimerPersianDescription
        {
            get
            {
                return this.dllSettings.FindStringValue("ClientDetectTimerPersianDescription", "پروسه تشخیص کلاینت");
            }
            set
            {
                this.dllSettings.SaveSetting("ClientDetectTimerPersianDescription", value);
            }
        }

        [Category("Operation")]
        [DisplayName("دوره زمانی بررسی حضور کلاینت بر حسب میلی ثانیه")]
        public int ClientDetectInterval
        {
            get
            {
                return this.dllSettings.FindIntValue("ClientDetectInterval", 2000);
            }
            set
            {
                this.dllSettings.SaveSetting("ClientDetectInterval", value);
            }
        }

        [Category("Operation")]
        [DisplayName("شرح فارسی پروسه دریافت پکت")]
        public string ReceiveTimerPersianDescription
        {
            get
            {
                return this.dllSettings.FindStringValue("ReceiveTimerPersianDescription", "پروسه دریافت پکت");
            }
            set
            {
                this.dllSettings.SaveSetting("ReceiveTimerPersianDescription", value);
            }
        }

        [DisplayName("دوره زمانی بررسی دریافت پکت بر حسب میلی ثانیه")]
        public int ReceiveInterval
        {
            get
            {
                return this.dllSettings.FindIntValue("ReceiveInterval", 2000);
            }
            set
            {
                this.dllSettings.SaveSetting("ReceiveInterval", value);
            }
        }

        [DisplayName("حداکثر زمان معتبر بودن اتصال کلاینت بر حسب میلی ثانیه")]
        public int TcpIpConnectExpireTime
        {
            get
            {
                return this.dllSettings.FindIntValue("TcpIpConnectExpireTime", 20000);
            }
            set
            {
                this.dllSettings.SaveSetting("TcpIpConnectExpireTime", value);
            }
        }

        [DisplayName("حداکثر زمان انتظار برای ارسال پکت بر حسب میلی ثانیه")]
        public int SendTimeout
        {
            get
            {
                return this.dllSettings.FindIntValue("SendTimeout", 5000);
            }
            set
            {
                this.dllSettings.SaveSetting("SendTimeout", value);
            }
        }
        [Category("Information"), DisplayName("کلاینت متصل شده - آدرس")]
        public string ClientServerIp
        {
            get
            {
                string result;
                try
                {
                    if (this.socket != null)
                    {
                        result = (this.socket.RemoteEndPoint as IPEndPoint).Address.ToString();
                    }
                    else
                    {
                        result = "";
                    }
                }
                catch
                {
                    result = "";
                }
                return result;
            }
        }
        [Category("Information"), DisplayName("کلاینت متصل شده - نام")]
        public string ClientServerName
        {
            get
            {
                string result;
                try
                {
                    if (this.socket == null)
                    {
                        result = "";
                    }
                    else
                    {
                        result = Dns.GetHostEntry((this.socket.RemoteEndPoint as IPEndPoint).Address).HostName;
                    }
                }
                catch
                {
                    result = "";
                }
                return result;
            }
        }

        [Category("Information")]
        public override bool Connected
        {
            get
            {
                return base.Started && this.socket != null && this.socket.Connected;
            }
        }
        #endregion

        #region Methods

        private void receiverTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.socket != null)
            {
                if ((DateTime.Now - this.lastConnectionTime).TotalMilliseconds > (double)this.TcpIpConnectExpireTime)
                {
                    this.eventLogger.LogWarning("کلاینت {0} به دلیل منقضی شدن زمان اتصال متوقف شد.", new object[]
					{
						base.PersianDescription
					});
                    this.socket = null;
                    this.OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
                }
                else
                {
                    if (this.socket != null)
                    {
                        if (this.socket.Available > 0)
                        {
                            this.lastConnectionTime = DateTime.Now;
                            byte[] array = new byte[this.socket.Available];
                            int num = this.socket.Receive(array);
                            this.receivedBuffer.AddRange(array);
                            bool flag = false;
                            IccTelegram iccTelegram = new IccTelegram
                            {
                                Source = base.Name,
                                SendTime = DateTime.Now
                            };
                            try
                            {
                                while (this.receivedBuffer.Count > 0 && !flag)
                                {
                                    iccTelegram = new IccTelegram
                                    {
                                        Source = base.Name,
                                        SendTime = DateTime.Now
                                    };
                                    byte[] completeTelegram;
                                    if (this.retrieveCompleteTelegram(out completeTelegram, ref iccTelegram))
                                    {
                                        this.ConvertClientTelegramToStandardTelegram(completeTelegram, ref iccTelegram);
                                        this.OnReceive(new ReceiveEventArgs(iccTelegram, true, null));
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                            }
                            catch (Exception failException)
                            {
                                this.receivedBuffer.Clear();
                                this.OnReceive(new ReceiveEventArgs(iccTelegram, false, failException));
                            }
                        }
                    }
                }
            }
        }
        private void clientDetectTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.tcpListener.Pending())
            {
                this.lastConnectionTime = DateTime.Now;
                bool flag = false;
                if (this.socket == null)
                {
                    this.eventLogger.LogInfo(string.Format("کلاینت {0} متصل شد.", base.PersianDescription), new object[0]);
                    flag = true;
                }
                else
                {
                    this.eventLogger.LogInfo(string.Format("کلاینت {0} مجددا متصل شد.", base.PersianDescription), new object[0]);
                }
                this.socket = this.tcpListener.AcceptSocket();
                this.socket.SendTimeout = this.SendTimeout;
                if (flag)
                {
                    this.OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
                }
            }
        }
        protected DateTime StringToDateTime(string stringDate, string format)
        {
            DateTime result;
            try
            {
                result = DateTime.ParseExact(stringDate, format, null);
            }
            catch
            {
                this.eventLogger.LogWarning("زمان ارسال تلگرام از کلاینت {0} برابر با {1} می باشد و قابل تبدیل به فرمت {2} نیست.", new object[]
				{
					base.PersianDescription,
					stringDate,
					format
				});
                result = DateTime.Now;
            }
            return result;
        }
        protected override void SendTelegram(IccTelegram iccTelegram)
        {
            byte[] buffer = this.ConvertStandardTelegramToClientTelegram(iccTelegram);
            if (!this.Connected)
            {
                throw IrisaException.Create("مقصد تلگرام متصل نمی باشد", new object[0]);
            }
            try
            {
                this.socket.Send(buffer);
                this.lastConnectionTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw IrisaException.Create("پاسخی از سرور {0} دریافت نشد. متن خطا : {1}", new object[]
				{
					base.PersianDescription,
					ex.Message
				});
            }
        }
        public override void Start(ILogger eventLogger)
        {
            base.Start(eventLogger);
            this.lastConnectionTime = DateTime.Now;
            IPAddress localaddr = IPAddress.Parse(this.Ip);
            if (this.tcpListener != null)
            {
                this.tcpListener.Stop();
            }
            this.tcpListener = new TcpListener(localaddr, this.Port);
            try
            {
                this.tcpListener.Start();
            }
            catch
            {
                throw IrisaException.Create("برنامه دیگری در حال استفاده از پورت {0} مورد استفاده در آداپتور {1} می باشد", new object[]
				{
					this.Port,
					base.PersianDescription
				});
            }
            if (this.clientDetectTimer != null)
            {
                this.clientDetectTimer.Stop();
            }
            this.clientDetectTimer = new IrisaBackgroundTimer();
            this.clientDetectTimer.Interval = this.ClientDetectInterval;
            this.clientDetectTimer.DoWork += new DoWorkEventHandler(this.clientDetectTimer_DoWork);
            this.clientDetectTimer.PersianDescription = this.ClientDetectTimerPersianDescription + " در آداپتور " + base.PersianDescription;
            this.clientDetectTimer.EventLogger = this.eventLogger;
            this.clientDetectTimer.Start();
            if (this.receiveTimer != null)
            {
                this.receiveTimer.Stop();
            }
            this.receiveTimer = new IrisaBackgroundTimer();
            this.receiveTimer.Interval = this.ReceiveInterval;
            this.receiveTimer.DoWork += new DoWorkEventHandler(this.receiverTimer_DoWork);
            this.receiveTimer.PersianDescription = this.ReceiveTimerPersianDescription + " در آداپتور " + base.PersianDescription;
            this.receiveTimer.EventLogger = this.eventLogger;
            this.receiveTimer.Start();
        }
        public override void Stop()
        {
            base.Stop();
            if (this.clientDetectTimer != null)
            {
                this.clientDetectTimer.Stop();
            }
            if (this.receiveTimer != null)
            {
                this.receiveTimer.Stop();
            }
            if (this.tcpListener != null)
            {
                this.tcpListener.Stop();
            }
            if (this.socket != null)
            {
                this.socket.Close();
            }
        }
        public override void AwakeTimers()
        {
            base.AwakeTimers();
            if (this.receiveTimer != null)
            {
                this.receiveTimer.Awake();
            }
            if (this.clientDetectTimer != null)
            {
                this.clientDetectTimer.Awake();
            }
        }
        protected virtual bool retrieveCompleteTelegram(out byte[] completeTelegram, ref IccTelegram iccTelegram)
        {
            bool result;
            if (this.receivedBuffer.Count == 0)
            {
                completeTelegram = null;
                result = false;
            }
            else
            {
                char startCharacter = this.StartCharacter;
                byte b = BitConverter.GetBytes(startCharacter).First<byte>();
                char endCharacter = this.EndCharacter;
                byte b2 = BitConverter.GetBytes(endCharacter).First<byte>();
                if (b != 0 && this.receivedBuffer.First<byte>() != b)
                {
                    throw IrisaException.Create("پکت دریافتی  با کاراکتر {0} آغاز نشده است.", new object[]
					{
						startCharacter
					});
                }
                if (this.receivedBuffer.Count < this.HeaderSize)
                {
                    throw IrisaException.Create("طول پکت دریافتی {0} بایت و حداقل طول مجاز {1} بایت می باشد.", new object[]
					{
						this.receivedBuffer.Count,
						this.HeaderSize
					});
                }
                iccTelegram.TelegramId = this.GetTelegramId(this.receivedBuffer.ToArray());
                int telegramSize = this.GetTelegramSize(this.receivedBuffer.ToArray());
                if (telegramSize < 0)
                {
                    throw IrisaException.Create("اندازه تلگرام بر حسب بایت {0} اعلام شده است.", new object[]
					{
						telegramSize
					});
                }
                completeTelegram = new byte[telegramSize];
                if (this.receivedBuffer.Count < telegramSize)
                {
                    if (this.receivedBuffer.Count < 1000 && this.receivedBuffer.Last<byte>() == b2)
                    {
                        throw IrisaException.Create("طول پکت دریافتی {0} بایت و طول اعلام شده توسط پکت {1} بایت می باشد.", new object[]
						{
							this.receivedBuffer.Count,
							telegramSize
						});
                    }
                    this.eventLogger.LogInfo("دریافت تلگرام چند قسمتی از {0} آغاز شد.", new object[]
					{
						base.Name
					});
                    result = false;
                }
                else
                {
                    if (this.receivedBuffer.Count > telegramSize)
                    {
                        this.receivedBuffer.CopyTo(0, completeTelegram, 0, telegramSize);
                        this.receivedBuffer = this.receivedBuffer.Skip(telegramSize).ToList<byte>();
                    }
                    else
                    {
                        completeTelegram = this.receivedBuffer.ToArray();
                        this.receivedBuffer.Clear();
                    }
                    if (b2 != 0 && completeTelegram.Last<byte>() != b2)
                    {
                        throw IrisaException.Create("تلگرام دریافتی  با کاراکتر {0} خاتمه نیافته است.", new object[]
						{
							endCharacter
						});
                    }
                    result = true;
                }
            }
            return result;
        }
        protected virtual void ConvertClientTelegramToStandardTelegram(byte[] completeTelegram, ref IccTelegram iccTelegram)
        {
            iccTelegram.TelegramId = this.GetTelegramId(completeTelegram);
            int telegramBodySize = this.GetTelegramBodySize(completeTelegram);
            if (telegramBodySize < 0)
            {
                throw IrisaException.Create("اندازه محتوات تلگرام بر حسب بایت {0} اعلام شده است.", new object[]
				{
					telegramBodySize
				});
            }
            iccTelegram.SendTime = this.GetTelegramSendTime(completeTelegram);
            iccTelegram.Source = this.GetTelegramSource(completeTelegram);
            TelegramDefinition telegramDefinition = this.telegramDefinitions.Find(iccTelegram);
            iccTelegram.Destination = telegramDefinition.Destination;
            byte[] array = completeTelegram.Skip(this.HeaderSize).ToArray<byte>();
            array = array.Take(telegramBodySize).ToArray<byte>();
            byte[] bodyBytes = array;
            List<FieldDefinition> fields = telegramDefinition.Fields;
            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                FieldDefinition fieldDefinition = fields[i];
                if (array.Length < fieldDefinition.Size)
                {
                    throw IrisaException.Create("تعداد فیلد های ارسال شده {0} و تعداد فیلد های تعریف شده {1} می باشد.", new object[]
					{
						i,
						count
					});
                }
                string value = fieldDefinition.GetValue(array);
                array = array.Skip(fieldDefinition.Size).ToArray<byte>();
                iccTelegram.Body.Add(value);
            }
            if (array.Length > 0)
            {
                throw IrisaException.Create("طول تلگرام ارسال شده {0} بایت بزرگتر از تلگرام تعریف شده در سیستم است.", new object[]
				{
					array.Length
				});
            }
            this.ExtraValidationsOnReceive(completeTelegram, bodyBytes, iccTelegram);
        }
        protected virtual void ExtraValidationsOnReceive(byte[] completeTelegram, byte[] bodyBytes, IccTelegram iccTelegram)
        {
        }
        protected virtual byte[] ConvertStandardTelegramToClientTelegram(IccTelegram iccTelegram)
        {
            TelegramDefinition telegramDefinition = this.telegramDefinitions.Find(iccTelegram);
            MemoryStream memoryStream = new MemoryStream();
            List<FieldDefinition> fields = telegramDefinition.Fields;
            if (fields.Count != iccTelegram.Body.Count)
            {
                throw IrisaException.Create("تعداد فیلد های تلگرام ارسالی {0} و تعداد فیلد های تلگرام تعریف شده {1} می باشد.", new object[]
				{
					iccTelegram.Body.Count,
					fields.Count
				});
            }
            int num = 0;
            foreach (FieldDefinition current in fields)
            {
                byte[] bytes = current.GetBytes(iccTelegram.Body[num++]);
                memoryStream.Write(bytes, 0, bytes.Length);
            }
            byte[] body = memoryStream.ToArray();
            return this.CreateClientBytes(iccTelegram, body);
        }
        protected virtual int GetTelegramSize(byte[] telegramBytes)
        {
            int num = (this.EndCharacter == '\0') ? 0 : 1;
            return this.HeaderSize + this.GetTelegramBodySize(telegramBytes) + num;
        }
        protected abstract byte[] CreateClientBytes(IccTelegram iccTelegram, byte[] body);
        protected abstract int GetTelegramBodySize(byte[] telegramBytes);
        protected abstract int GetTelegramId(byte[] telegramBytes);
        protected abstract string GetTelegramSource(byte[] telegramBytes);
        protected abstract DateTime GetTelegramSendTime(byte[] telegramBytes);
        #endregion
    }
}
