using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Loggers;
using IRISA.CommunicationCenter.Library.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace IRISA.CommunicationCenter.Adapters.TcpIp.Wasco
{
    public class WascoTcpIpAdapter : TcpIpBaseAdapter<WascoTcpIpAdapter>
    {
        protected byte CalculateCrc(byte[] body)
        {
            byte crc = CrcDivisor;

            foreach (byte b in body)
                crc = (byte)(crc ^ b);

            return crc;
        }
        protected byte GetTelegramCrc(byte[] completeTelegram)
        {
            return completeTelegram[HeaderSize - 1];
        }

        [Category("اعتبار سنجی")]
        public Byte CrcDivisor
        {
            get { return _dllSettings.FindByteValue("CrcDivisor", (byte)0xF); }
            set { _dllSettings.SaveSetting("CrcDivisor", value); }
        }

        [Category("اعتبار سنجی")]
        public Boolean CheckCrc
        {
            get { return _dllSettings.FindBooleanValue("CheckCrc", true); }
            set { _dllSettings.SaveSetting("CheckCrc", value); }
        }

        protected override void ExtraValidationsOnReceive(byte[] completeTelegram, byte[] bodyBytes, IccTelegram iccTelegram)
        {
            if (!CheckCrc)
                return;

            byte destinationCrc = CalculateCrc(bodyBytes);
            byte sourceCrc = GetTelegramCrc(completeTelegram);

            if (destinationCrc != sourceCrc)
                throw IrisaException.Create("بایت {0} محاسبه شده در مقصد {1} و بایت ارسال شده از مبدا {2} می باشد. "
                    , "CRC", destinationCrc, sourceCrc);
        }
        private const string DateFormat = "yyyyMMddHHmmss";
        protected override int GetTelegramId(byte[] telegramBytes)
        {
            return BitConverter.ToInt16(telegramBytes, 1);
        }
        protected override int GetTelegramBodySize(byte[] telegramBytes)
        {
            var size = (int)BitConverter.ToInt16(telegramBytes, 22);
            return size;
        }
        protected override DateTime GetTelegramSendTime(byte[] telegramBytes)
        {
            string @string = Encoding.ASCII.GetString(telegramBytes, 8, DateFormat.Length);
            return base.StringToDateTime(@string, DateFormat);
        }
        protected override string GetTelegramSource(byte[] telegramBytes)
        {
            string source = Encoding.ASCII.GetString(telegramBytes, 5, 3);
            if (source != Name)
                throw IrisaException.Create("نام کلاینت ارسال کننده {0} و نام کلاینت مورد انتظار {1} می باشد. ", source, Name);
            return Name;
        }
        protected override byte[] CreateClientBytes(IccTelegram iccTelegram, byte[] body)
        {
            MemoryStream memoryStream = new MemoryStream();

            /* Start Character */
            memoryStream.Write(BitConverter.GetBytes(StartCharacter), 0, 1);

            /* Telegram Id */
            memoryStream.Write(BitConverter.GetBytes(iccTelegram.TelegramId), 0, 4);

            /* Source */
            string source = iccTelegram.Source;
            if (source == null)
                source = "";
            if (source.Length > 3)
                source = source.Substring(0, 3);
            while (source.Length < 3)
                source += " ";
            memoryStream.Write(Encoding.ASCII.GetBytes(source), 0, 3);

            /* Send Time */
            memoryStream.Write(Encoding.ASCII.GetBytes(iccTelegram.SendTime.ToString(DateFormat)), 0, DateFormat.Length);

            /* Body Size */
            memoryStream.Write(BitConverter.GetBytes(body.Length), 0, 2);

            /* CRC */
            memoryStream.Write(new byte[] { CalculateCrc(body) }, 0, 1);

            /* Body */
            memoryStream.Write(body, 0, body.Length);

            /* End Character */
            memoryStream.Write(BitConverter.GetBytes(EndCharacter), 0, 1);

            return memoryStream.ToArray();
        }


    }
}
