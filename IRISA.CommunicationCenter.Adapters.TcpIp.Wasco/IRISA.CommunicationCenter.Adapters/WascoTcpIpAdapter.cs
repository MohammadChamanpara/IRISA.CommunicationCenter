using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Loggers;
using IRISA.CommunicationCenter.Library.Models;
using System;
using System.IO;
using System.Text;
namespace IRISA.CommunicationCenter.Adapters
{
    public class WascoTcpIpAdapter : TcpIpBaseAdapter<WascoTcpIpAdapter>
    {
        /*----------------------Start Wasco-------------------*/
        protected byte CalculateCrc(byte[] body)
        {
            byte crc = this.CrcDivisor;

            foreach (byte b in body)
                crc = (byte)(crc ^ b);

            return crc;
        }
        protected byte GetTelegramCrc(byte[] completeTelegram)
        {
            return completeTelegram[this.HeaderSize - 1];
        }
        /*------------------------End Wasco----------------------------------*/
        /*------------------------Wasco--------------------------------------*/

        public Byte CrcDivisor
        {
            get { return dllSettings.FindByteValue("CrcDivisor", (byte)0xF); }
            set { dllSettings.SaveSetting("CrcDivisor", value); }
        }

        public Boolean CheckCrc
        {
            get { return dllSettings.FindBooleanValue("CheckCrc", true); }
            set { dllSettings.SaveSetting("CheckCrc", value); }
        }

        protected override void ExtraValidationsOnReceive(byte[] completeTelegram, byte[] bodyBytes, IccTelegram iccTelegram)
        {
            if (!this.CheckCrc)
                return;

            byte destinationCrc = CalculateCrc(bodyBytes);
            byte sourceCrc = GetTelegramCrc(completeTelegram);

            if (destinationCrc != sourceCrc)
                throw IrisaException.Create("بایت {0} محاسبه شده در مقصد {1} و بایت ارسال شده از مبدا {2} می باشد."
                    , "CRC", destinationCrc, sourceCrc);
        }
        /*------------------------End Wasco--------------------------------------*/
        private string dateFormat = "yyyyMMddHHmmss";
        protected override int GetTelegramId(byte[] telegramBytes)
        {
            return BitConverter.ToInt16(telegramBytes, 1);
        }
        protected override int GetTelegramBodySize(byte[] telegramBytes)
        {
            var size= (int)BitConverter.ToInt16(telegramBytes, 22);
            return size;
        }
        protected override DateTime GetTelegramSendTime(byte[] telegramBytes)
        {
            string @string = Encoding.ASCII.GetString(telegramBytes, 8, this.dateFormat.Length);
            return base.StringToDateTime(@string, this.dateFormat);
        }
        protected override string GetTelegramSource(byte[] telegramBytes)
        {
            string source = Encoding.ASCII.GetString(telegramBytes, 5, 3);
            if (source != this.Name)
                throw IrisaException.Create("نام کلاینت ارسال کننده {0} و نام کلاینت مورد انتظار {1} می باشد.", source, this.Name);
            return this.Name;
        }
        protected override byte[] CreateClientBytes(IccTelegram iccTelegram, byte[] body)
        {
            MemoryStream memoryStream = new MemoryStream();

            /* Start Character */
            memoryStream.Write(BitConverter.GetBytes(this.StartCharacter), 0, 1);

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
            memoryStream.Write(Encoding.ASCII.GetBytes(iccTelegram.SendTime.ToString(dateFormat)), 0, dateFormat.Length);

            /* Body Size */
            memoryStream.Write(BitConverter.GetBytes(body.Length), 0, 2);

            /* CRC */
            memoryStream.Write(new byte[] { CalculateCrc(body) }, 0, 1);

            /* Body */
            memoryStream.Write(body, 0, body.Length);

            /* End Character */
            memoryStream.Write(BitConverter.GetBytes(this.EndCharacter), 0, 1);

            return memoryStream.ToArray();
        }


    }
}
