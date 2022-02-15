using System;
using System.IO;
using System.Text;
namespace IRISA.CommunicationCenter.Adapters
{
	public class HoscoTcpIpAdapter : TcpIpBaseAdapter<HoscoTcpIpAdapter>
	{
		private string dateFormat = "yyyyMMddHHmmss";
		protected override int GetTelegramId(byte[] telegramBytes)
		{
			return BitConverter.ToInt32(telegramBytes, 1);
		}
		protected override int GetTelegramBodySize(byte[] telegramBytes)
		{
			return BitConverter.ToInt32(telegramBytes, 5);
		}
		protected override DateTime GetTelegramSendTime(byte[] telegramBytes)
		{
			string @string = Encoding.ASCII.GetString(telegramBytes, 9, 14);
			return base.StringToDateTime(@string, this.dateFormat);
		}
		protected override string GetTelegramSource(byte[] telegramBytes)
		{
			int num = BitConverter.ToInt32(telegramBytes, 23);
			if (num != base.Id)
			{
				throw HelperMethods.CreateException("شناسه ارسال کننده صحیح نمی باشد. شناسه مورد انتظار {0} و شناسه ارسال شده {1} است.", new object[]
				{
					base.Id,
					num
				});
			}
			return base.Name;
		}
		protected override byte[] CreateClientBytes(IccTelegram iccTelegram, byte[] body)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(BitConverter.GetBytes(base.StartCharacter), 0, 1);
			memoryStream.Write(BitConverter.GetBytes(iccTelegram.TelegramId), 0, 4);
			memoryStream.Write(BitConverter.GetBytes(body.Length), 0, 4);
			memoryStream.Write(Encoding.ASCII.GetBytes(iccTelegram.SendTime.ToString(this.dateFormat)), 0, this.dateFormat.Length);
			memoryStream.Write(BitConverter.GetBytes(0), 0, 4);
			memoryStream.Write(body, 0, body.Length);
			memoryStream.Write(BitConverter.GetBytes(base.EndCharacter), 0, 1);
			return memoryStream.ToArray();
		}
	}
}
