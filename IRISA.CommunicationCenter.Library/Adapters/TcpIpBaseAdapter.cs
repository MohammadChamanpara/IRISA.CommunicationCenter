using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Loggers;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Tasks;
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
        private BackgroundTimer _clientDetectTimer;

        protected List<byte> receivedBuffer = new List<byte>();
        private DateTime lastConnectionTime;

        public override string Type
        {
            get
            {
                return "Tcp/Ip";
            }
        }
        [Category("اطلاعات")]
        [DisplayName("کلاینت متصل شده - آدرس")]
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

        [Category("اطلاعات")]
        [DisplayName("کلاینت متصل شده - نام")]
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

        [Category("ساختار تلگرام")]
        [DisplayName("کاراکتر آغاز کننده تلگرام")]
        public char StartCharacter
        {
            get
            {
                return _dllSettings.FindCharacterValue("StartCharacter", '$');
            }
            set
            {
                _dllSettings.SaveSetting("StartCharacter", value);
            }
        }

        [Category("ساختار تلگرام")]
        [DisplayName("کاراکتر خاتمه دهنده تلگرام")]
        public char EndCharacter
        {
            get
            {
                return _dllSettings.FindCharacterValue("EndCharacter", '#');
            }
            set
            {
                _dllSettings.SaveSetting("EndCharacter", value);
            }
        }

        [Category("ساختار تلگرام")]
        [DisplayName("سایز هدر تلگرام بر حسب بایت")]
        public int HeaderSize
        {
            get
            {
                return _dllSettings.FindIntValue("HeaderSize", 27);
            }
            set
            {
                _dllSettings.SaveSetting("HeaderSize", value);
            }
        }

        [Category("عملیات")]
        [DisplayName("شناسه کلاینت")]
        public int Id
        {
            get
            {
                return _dllSettings.FindIntValue("Id", 0);
            }
            set
            {
                _dllSettings.SaveSetting("Id", value);
            }
        }

        [Category("اتصال")]
        [DisplayName("آدرس کلاینت")]
        public string Ip
        {
            get
            {
                return _dllSettings.FindStringValue("Ip", "127.0.0.1");
            }
            set
            {
                _dllSettings.SaveSetting("Ip", value);
            }
        }

        [Category("اتصال")]
        [DisplayName("شماره پورت ارتباطی")]
        public int Port
        {
            get
            {
                return _dllSettings.FindIntValue("port", 6000);
            }
            set
            {
                _dllSettings.SaveSetting("port", value);
            }
        }

        [Category("عملیات")]
        [DisplayName("دوره زمانی بررسی حضور کلاینت بر حسب میلی ثانیه")]
        public int ClientDetectInterval
        {
            get
            {
                return _dllSettings.FindIntValue("ClientDetectInterval", 2000);
            }
            set
            {
                _dllSettings.SaveSetting("ClientDetectInterval", value);
            }
        }

        [Category("عملیات")]
        [DisplayName("حداکثر زمان معتبر بودن اتصال کلاینت بر حسب میلی ثانیه")]
        public int TcpIpConnectExpireTime
        {
            get
            {
                return _dllSettings.FindIntValue("TcpIpConnectExpireTime", 20000);
            }
            set
            {
                _dllSettings.SaveSetting("TcpIpConnectExpireTime", value);
            }
        }

        [Category("عملیات")]
        [DisplayName("حداکثر زمان انتظار برای ارسال پکت بر حسب میلی ثانیه")]
        public int SendTimeout
        {
            get
            {
                return _dllSettings.FindIntValue("SendTimeout", 5000);
            }
            set
            {
                _dllSettings.SaveSetting("SendTimeout", value);
            }
        }

        protected override bool CheckConnection()
        {
            return socket != null && socket.Connected;
        }
        #endregion

        #region Methods

        protected override void ReceiveTimer_DoWork()
        {
            if (socket != null)
            {
                if ((DateTime.Now - lastConnectionTime).TotalMilliseconds > (double)TcpIpConnectExpireTime)
                {
                    _logger.LogWarning("کلاینت {0} به دلیل منقضی شدن زمان اتصال متوقف شد.", new object[]
                    {
                        base.PersianDescription
                    });
                    socket = null;
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
                                        ToIccTelegram(completeTelegram, ref iccTelegram);
                                        OnTelegramReceived(new TelegramReceivedEventArgs(iccTelegram, true, null));
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
                                OnTelegramReceived(new TelegramReceivedEventArgs(iccTelegram, false, failException));
                            }
                        }
                    }
                }
            }
        }
        private void ClientDetectTimer_DoWork()
        {
            if (tcpListener.Pending())
            {
                lastConnectionTime = DateTime.Now;
                if (socket == null)
                {
                    _logger.LogInformation($"کلاینت {PersianDescription} متصل شد.");
                }
                else
                {
                    _logger.LogInformation($"کلاینت {PersianDescription} مجددا متصل شد.");
                }
                socket = tcpListener.AcceptSocket();
                socket.SendTimeout = SendTimeout;
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
                _logger.LogWarning("زمان ارسال تلگرام از کلاینت {0} برابر با {1} می باشد و قابل تبدیل به فرمت {2} نیست.", new object[]
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
            byte[] buffer = ToClientTelegram(iccTelegram);
            if (!Connected)
            {
                throw IrisaException.Create("مقصد تلگرام متصل نمی باشد. ");
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
        public override void Start(ILogger eventLogger, ITelegramDefinitions telegramDefinitions)
        {
            base.Start(eventLogger, telegramDefinitions);
            lastConnectionTime = DateTime.Now;
            IPAddress localaddr = IPAddress.Parse(Ip);
            InitializeTcpListener(localaddr);
            InitializeClientDetectTimer(eventLogger);
        }

        private void InitializeClientDetectTimer(ILogger eventLogger)
        {
            if (_clientDetectTimer != null)
            {
                _clientDetectTimer.Stop();
            }
            _clientDetectTimer = new BackgroundTimer(eventLogger)
            {
                Interval = ClientDetectInterval,
                PersianDescription = $"پروسه تشخیص کلاینت در آداپتور {PersianDescription}"
            };
            _clientDetectTimer.DoWork += ClientDetectTimer_DoWork;
            _clientDetectTimer.Start();
        }

        private void InitializeTcpListener(IPAddress localaddr)
        {
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
        }

        public override void Stop()
        {
            base.Stop();
            _clientDetectTimer?.Stop();
            tcpListener?.Stop();
            socket?.Close();
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
                    _logger.LogInformation("دریافت تلگرام چند قسمتی از {0} آغاز شد.", new object[]
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
        protected virtual void ToIccTelegram(byte[] completeTelegram, ref IccTelegram iccTelegram)
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
            iccTelegram.SendTime = DateTime.Now;
            iccTelegram.Source = GetTelegramSource(completeTelegram);
            var telegramDefinition = _telegramDefinitions.Find(iccTelegram);
            iccTelegram.Destination = telegramDefinition.Destination;
            byte[] array = completeTelegram.Skip(HeaderSize).ToArray<byte>();
            array = array.Take(telegramBodySize).ToArray<byte>();
            byte[] bodyBytes = array;

            foreach (var fieldDefinition in telegramDefinition.Fields)
            {
                if (array.Length < fieldDefinition.Size)
                {
                    throw IrisaException.Create($"تعداد فیلد های ارسال شده {array.Length} و تعداد فیلد های تعریف شده {fieldDefinition.Size} می باشد.");
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
        protected virtual byte[] ToClientTelegram(IccTelegram iccTelegram)
        {
            var telegramDefinition = _telegramDefinitions.Find(iccTelegram);
            MemoryStream memoryStream = new MemoryStream();
            var fields = telegramDefinition.Fields;
            if (fields.Count() != iccTelegram.Body.Count)
            {
                throw IrisaException.Create("تعداد فیلد های تلگرام ارسالی {0} و تعداد فیلد های تلگرام تعریف شده {1} می باشد.", new object[]
                {
                    iccTelegram.Body.Count,
                    fields.Count()
                });
            }
            int num = 0;
            foreach (IFieldDefinition current in fields)
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
