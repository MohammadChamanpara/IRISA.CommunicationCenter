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
                return dllSettings.FindCharacterValue("StartCharacter", '$');
            }
            set
            {
                dllSettings.SaveSetting("StartCharacter", value);
            }
        }

        [Category("Operation")]
        [DisplayName("کاراکتر خاتمه دهنده تلگرام")]
        public char EndCharacter
        {
            get
            {
                return dllSettings.FindCharacterValue("EndCharacter", '#');
            }
            set
            {
                dllSettings.SaveSetting("EndCharacter", value);
            }
        }
        
        [Category("Operation")]
        [DisplayName("سایز هدر تلگرام بر حسب بایت")]
        public int HeaderSize
        {
            get
            {
                return dllSettings.FindIntValue("HeaderSize", 27);
            }
            set
            {
                dllSettings.SaveSetting("HeaderSize", value);
            }
        }

        [Category("Operation")]
        [DisplayName("شناسه کلاینت")]
        public int Id
        {
            get
            {
                return dllSettings.FindIntValue("Id", 0);
            }
            set
            {
                dllSettings.SaveSetting("Id", value);
            }
        }

        [Category("Operation")]
        [DisplayName("آدرس کلاینت")]
        public string Ip
        {
            get
            {
                return dllSettings.FindStringValue("Ip", "127.0.0.1");
            }
            set
            {
                dllSettings.SaveSetting("Ip", value);
            }
        }

        [Category("Operation")]
        [DisplayName("شماره پورت ارتباطی")]
        public int Port
        {
            get
            {
                return dllSettings.FindIntValue("port", 6000);
            }
            set
            {
                dllSettings.SaveSetting("port", value);
            }
        }

        [Category("Operation")]
        [DisplayName("شرح فارسی پروسه تشخیص کلاینت")]
        public string ClientDetectTimerPersianDescription
        {
            get
            {
                return dllSettings.FindStringValue("ClientDetectTimerPersianDescription", "پروسه تشخیص کلاینت");
            }
            set
            {
                dllSettings.SaveSetting("ClientDetectTimerPersianDescription", value);
            }
        }

        [Category("Operation")]
        [DisplayName("دوره زمانی بررسی حضور کلاینت بر حسب میلی ثانیه")]
        public int ClientDetectInterval
        {
            get
            {
                return dllSettings.FindIntValue("ClientDetectInterval", 2000);
            }
            set
            {
                dllSettings.SaveSetting("ClientDetectInterval", value);
            }
        }

        [Category("Operation")]
        [DisplayName("شرح فارسی پروسه دریافت پکت")]
        public string ReceiveTimerPersianDescription
        {
            get
            {
                return dllSettings.FindStringValue("ReceiveTimerPersianDescription", "پروسه دریافت پکت");
            }
            set
            {
                dllSettings.SaveSetting("ReceiveTimerPersianDescription", value);
            }
        }

        [DisplayName("دوره زمانی بررسی دریافت پکت بر حسب میلی ثانیه")]
        public int ReceiveInterval
        {
            get
            {
                return dllSettings.FindIntValue("ReceiveInterval", 2000);
            }
            set
            {
                dllSettings.SaveSetting("ReceiveInterval", value);
            }
        }

        [DisplayName("حداکثر زمان معتبر بودن اتصال کلاینت بر حسب میلی ثانیه")]
        public int TcpIpConnectExpireTime
        {
            get
            {
                return dllSettings.FindIntValue("TcpIpConnectExpireTime", 20000);
            }
            set
            {
                dllSettings.SaveSetting("TcpIpConnectExpireTime", value);
            }
        }

        [DisplayName("حداکثر زمان انتظار برای ارسال پکت بر حسب میلی ثانیه")]
        public int SendTimeout
        {
            get
            {
                return dllSettings.FindIntValue("SendTimeout", 5000);
            }
            set
            {
                dllSettings.SaveSetting("SendTimeout", value);
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
                    if (socket != null)
                    {
                        result = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();
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
                    if (socket == null)
                    {
                        result = "";
                    }
                    else
                    {
                        result = Dns.GetHostEntry((socket.RemoteEndPoint as IPEndPoint).Address).HostName;
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
                return base.Started && socket != null && socket.Connected;
            }
        }
        #endregion

        #region Methods

        private void ReceiverTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (socket != null)
            {
                if ((DateTime.Now - lastConnectionTime).TotalMilliseconds > (double)TcpIpConnectExpireTime)
                {
                    Logger.LogWarning("کلاینت {0} به دلیل منقضی شدن زمان اتصال متوقف شد.", new object[]
					{
						base.PersianDescription
					});
                    socket = null;
                    OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
                }
                else
                {
                    if (socket != null)
                    {
                        if (socket.Available > 0)
                        {
                            lastConnectionTime = DateTime.Now;
                            byte[] array = new byte[socket.Available];
                            socket.Receive(array);
                            receivedBuffer.AddRange(array);
                            bool flag = false;
                            IccTelegram iccTelegram = new IccTelegram
                            {
                                Source = base.Name,
                                SendTime = DateTime.Now
                            };
                            try
                            {
                                while (receivedBuffer.Count > 0 && !flag)
                                {
                                    iccTelegram = new IccTelegram
                                    {
                                        Source = base.Name,
                                        SendTime = DateTime.Now
                                    };
                                    if (RetrieveCompleteTelegram(out byte[] completeTelegram, ref iccTelegram))
                                    {
                                        ConvertClientTelegramToStandardTelegram(completeTelegram, ref iccTelegram);
                                        OnReceive(new ReceiveEventArgs(iccTelegram, true, null));
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                            }
                            catch (Exception failException)
                            {
                                receivedBuffer.Clear();
                                OnReceive(new ReceiveEventArgs(iccTelegram, false, failException));
                            }
                        }
                    }
                }
            }
        }
        private void ClientDetectTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (tcpListener.Pending())
            {
                lastConnectionTime = DateTime.Now;
                bool flag = false;
                if (socket == null)
                {
                    Logger.LogInfo(string.Format("کلاینت {0} متصل شد.", base.PersianDescription), new object[0]);
                    flag = true;
                }
                else
                {
                    Logger.LogInfo(string.Format("کلاینت {0} مجددا متصل شد.", base.PersianDescription), new object[0]);
                }
                socket = tcpListener.AcceptSocket();
                socket.SendTimeout = SendTimeout;
                if (flag)
                {
                    OnConnectionChanged(new AdapterConnectionChangedEventArgs(this));
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
                Logger.LogWarning("زمان ارسال تلگرام از کلاینت {0} برابر با {1} می باشد و قابل تبدیل به فرمت {2} نیست.", new object[]
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
            byte[] buffer = ConvertStandardTelegramToClientTelegram(iccTelegram);
            if (!Connected)
            {
                throw IrisaException.Create("مقصد تلگرام متصل نمی باشد", new object[0]);
            }
            try
            {
                socket.Send(buffer);
                lastConnectionTime = DateTime.Now;
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
            lastConnectionTime = DateTime.Now;
            IPAddress localaddr = IPAddress.Parse(Ip);
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }
            tcpListener = new TcpListener(localaddr, Port);
            try
            {
                tcpListener.Start();
            }
            catch
            {
                throw IrisaException.Create("برنامه دیگری در حال استفاده از پورت {0} مورد استفاده در آداپتور {1} می باشد", new object[]
				{
					Port,
					base.PersianDescription
				});
            }
            if (clientDetectTimer != null)
            {
                clientDetectTimer.Stop();
            }
            clientDetectTimer = new IrisaBackgroundTimer
            {
                Interval = ClientDetectInterval
            };
            clientDetectTimer.DoWork += new DoWorkEventHandler(ClientDetectTimer_DoWork);
            clientDetectTimer.PersianDescription = ClientDetectTimerPersianDescription + " در آداپتور " + base.PersianDescription;
            clientDetectTimer.EventLogger = eventLogger;
            clientDetectTimer.Start();
            if (receiveTimer != null)
            {
                receiveTimer.Stop();
            }
            receiveTimer = new IrisaBackgroundTimer
            {
                Interval = ReceiveInterval
            };
            receiveTimer.DoWork += new DoWorkEventHandler(ReceiverTimer_DoWork);
            receiveTimer.PersianDescription = ReceiveTimerPersianDescription + " در آداپتور " + base.PersianDescription;
            receiveTimer.EventLogger = eventLogger;
            receiveTimer.Start();
        }
        public override void Stop()
        {
            base.Stop();
            if (clientDetectTimer != null)
            {
                clientDetectTimer.Stop();
            }
            if (receiveTimer != null)
            {
                receiveTimer.Stop();
            }
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }
            if (socket != null)
            {
                socket.Close();
            }
        }
        public override void AwakeTimers()
        {
            base.AwakeTimers();
            if (receiveTimer != null)
            {
                receiveTimer.Awake();
            }
            if (clientDetectTimer != null)
            {
                clientDetectTimer.Awake();
            }
        }
        protected virtual bool RetrieveCompleteTelegram(out byte[] completeTelegram, ref IccTelegram iccTelegram)
        {
            bool result;
            if (receivedBuffer.Count == 0)
            {
                completeTelegram = null;
                result = false;
            }
            else
            {
                char startCharacter = StartCharacter;
                byte b = BitConverter.GetBytes(startCharacter).First<byte>();
                char endCharacter = EndCharacter;
                byte b2 = BitConverter.GetBytes(endCharacter).First<byte>();
                if (b != 0 && receivedBuffer.First<byte>() != b)
                {
                    throw IrisaException.Create("پکت دریافتی  با کاراکتر {0} آغاز نشده است.", new object[]
					{
						startCharacter
					});
                }
                if (receivedBuffer.Count < HeaderSize)
                {
                    throw IrisaException.Create("طول پکت دریافتی {0} بایت و حداقل طول مجاز {1} بایت می باشد.", new object[]
					{
						receivedBuffer.Count,
						HeaderSize
					});
                }
                iccTelegram.TelegramId = GetTelegramId(receivedBuffer.ToArray());
                int telegramSize = GetTelegramSize(receivedBuffer.ToArray());
                if (telegramSize < 0)
                {
                    throw IrisaException.Create("اندازه تلگرام بر حسب بایت {0} اعلام شده است.", new object[]
					{
						telegramSize
					});
                }
                completeTelegram = new byte[telegramSize];
                if (receivedBuffer.Count < telegramSize)
                {
                    if (receivedBuffer.Count < 1000 && receivedBuffer.Last<byte>() == b2)
                    {
                        throw IrisaException.Create("طول پکت دریافتی {0} بایت و طول اعلام شده توسط پکت {1} بایت می باشد.", new object[]
						{
							receivedBuffer.Count,
							telegramSize
						});
                    }
                    Logger.LogInfo("دریافت تلگرام چند قسمتی از {0} آغاز شد.", new object[]
					{
						base.Name
					});
                    result = false;
                }
                else
                {
                    if (receivedBuffer.Count > telegramSize)
                    {
                        receivedBuffer.CopyTo(0, completeTelegram, 0, telegramSize);
                        receivedBuffer = receivedBuffer.Skip(telegramSize).ToList<byte>();
                    }
                    else
                    {
                        completeTelegram = receivedBuffer.ToArray();
                        receivedBuffer.Clear();
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
            iccTelegram.TelegramId = GetTelegramId(completeTelegram);
            int telegramBodySize = GetTelegramBodySize(completeTelegram);
            if (telegramBodySize < 0)
            {
                throw IrisaException.Create("اندازه محتوات تلگرام بر حسب بایت {0} اعلام شده است.", new object[]
				{
					telegramBodySize
				});
            }
            iccTelegram.SendTime = GetTelegramSendTime(completeTelegram);
            iccTelegram.Source = GetTelegramSource(completeTelegram);
            TelegramDefinition telegramDefinition = telegramDefinitions.Find(iccTelegram);
            iccTelegram.Destination = telegramDefinition.Destination;
            byte[] array = completeTelegram.Skip(HeaderSize).ToArray<byte>();
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
            ExtraValidationsOnReceive(completeTelegram, bodyBytes, iccTelegram);
        }
        protected virtual void ExtraValidationsOnReceive(byte[] completeTelegram, byte[] bodyBytes, IccTelegram iccTelegram)
        {
        }
        protected virtual byte[] ConvertStandardTelegramToClientTelegram(IccTelegram iccTelegram)
        {
            TelegramDefinition telegramDefinition = telegramDefinitions.Find(iccTelegram);
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
            return CreateClientBytes(iccTelegram, body);
        }
        protected virtual int GetTelegramSize(byte[] telegramBytes)
        {
            int num = (EndCharacter == '\0') ? 0 : 1;
            return HeaderSize + GetTelegramBodySize(telegramBytes) + num;
        }
        protected abstract byte[] CreateClientBytes(IccTelegram iccTelegram, byte[] body);
        protected abstract int GetTelegramBodySize(byte[] telegramBytes);
        protected abstract int GetTelegramId(byte[] telegramBytes);
        protected abstract string GetTelegramSource(byte[] telegramBytes);
        protected abstract DateTime GetTelegramSendTime(byte[] telegramBytes);
        #endregion
    }
}
