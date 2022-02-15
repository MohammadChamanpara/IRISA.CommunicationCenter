using IRISA.CommunicationCenter.Model;
using IRISA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace IRISA.CommunicationCenter
{
	public class WebServiceAdapter
	{
		public WebServiceEventLogger EventLogger = new WebServiceEventLogger();
		private EntityBusiness<WebServiceEntities, IccClientTelegram> ClientTelegrams
		{
			get
			{
				return new EntityBusiness<WebServiceEntities, IccClientTelegram>();
			}
		}
		private void CheckDatabaseConnection()
		{
			if (!this.ClientTelegrams.Connected)
			{
				throw HelperMethods.CreateException("برنامه قادر به اتصال به پایگاه داده {0} نمی باشد. ", new object[]
				{
					Settings.Default.PersianDescription
				});
			}
		}
		private void ConvertWebServiceDataToClientTelegram(string webServiceTelegram, ref IccClientTelegram iccClientTelegram)
		{
			char headerSeparator = Settings.Default.HeaderSeparator;
			char headerAndBodySeparator = Settings.Default.HeaderAndBodySeparator;
			if (!webServiceTelegram.Contains(headerAndBodySeparator))
			{
				throw HelperMethods.CreateException("تلگرام حاوی کاراکتر جدا کننده {0} نمی باشد", new object[]
				{
					headerAndBodySeparator
				});
			}
			string[] array = webServiceTelegram.Split(new char[]
			{
				headerAndBodySeparator
			});
			if (array.Length > 2)
			{
				throw HelperMethods.CreateException("تلگرام حاوی چند کاراکتر جدا کننده {0} می باشد", new object[]
				{
					headerAndBodySeparator
				});
			}
			string text = array[0];
			iccClientTelegram.BODY = array[1];
			string[] array2 = text.Split(new char[]
			{
				headerSeparator
			});
			if (array2.Length < 4)
			{
				throw HelperMethods.CreateException("تعداد قسمت های هدر {0} ارسال شده است در صورتی که تعداد مورد انتظار حداقل 4 می باشد.", new object[]
				{
					array2.Length
				});
			}
			int tELEGRAM_ID;
			if (!int.TryParse(array2[0], out tELEGRAM_ID))
			{
				throw HelperMethods.CreateException("مقدار {0} به عنوان شناسه تلگرام معتبر نمی باشد", new object[]
				{
					array2[0]
				});
			}
			iccClientTelegram.TELEGRAM_ID = tELEGRAM_ID;
			iccClientTelegram.DESTINATION = array2[1];
			iccClientTelegram.SOURCE = array2[2];
			iccClientTelegram.READY_FOR_CLIENT = false;
			DateTime minValue = DateTime.MinValue;
			if (!DateTime.TryParse(array2[3], out minValue))
			{
				throw HelperMethods.CreateException("مقدار {0} به عنوان تاریخ معتبر نمی باشد.", new object[]
				{
					array2[3]
				});
			}
			iccClientTelegram.SEND_TIME = minValue;
		}
		private string ConvertClientTelegramToWebServiceData(IccClientTelegram clientTelegram)
		{
			string text = "";
			char headerSeparator = Settings.Default.HeaderSeparator;
			text = text + clientTelegram.TELEGRAM_ID.ToString() + headerSeparator;
			text = text + clientTelegram.DESTINATION.ToString() + headerSeparator;
			text = text + clientTelegram.SOURCE.ToString() + headerSeparator;
			text += clientTelegram.SEND_TIME.ToString(Settings.Default.DateFormat);
			text += Settings.Default.HeaderAndBodySeparator;
			return text + clientTelegram.BODY;
		}
		private IQueryable<IccClientTelegram> AvailableData()
		{
			return 
				from q in this.ClientTelegrams.GetAll()
				where q.PROCESSED == false && q.READY_FOR_CLIENT == true && q.DESTINATION == Settings.Default.ClientName
				select q;
		}
		public bool ReceiveDataFromWebService(string webServiceTelegram)
		{
			IccClientTelegram entity = new IccClientTelegram
			{
				SOURCE = Settings.Default.ClientName
			};
			this.ConvertWebServiceDataToClientTelegram(webServiceTelegram, ref entity);
			this.ClientTelegrams.Create(entity);
			return true;
		}
		public string SendDataToWebService()
		{
			this.CheckDatabaseConnection();
			IQueryable<IccClientTelegram> source = this.AvailableData();
			string result;
			if (source.Count<IccClientTelegram>() == 0)
			{
				result = null;
			}
			else
			{
				IccClientTelegram iccClientTelegram = (
					from q in source
					orderby q.ID
					select q).First<IccClientTelegram>();
				string text = this.ConvertClientTelegramToWebServiceData(iccClientTelegram);
				iccClientTelegram.PROCESSED = true;
				this.ClientTelegrams.Edit(iccClientTelegram);
				result = text;
			}
			return result;
		}
		public bool DataIsAvailableForWebService()
		{
			return this.AvailableData().Count<IccClientTelegram>() > 0;
		}
		public void CheckPassword(string password)
		{
			if (password == Settings.Default.WebServicePassword)
			{
				return;
			}
			throw HelperMethods.CreateException("کلمه عبور برای دسترسی به وب سرویس {0} صحیح نمی باشد", new object[]
			{
				Settings.Default.PersianDescription
			});
		}
		public IccWebService.TelegramsWithinRangeObject[] GetHistory(DateTime startDate, DateTime endDate, int? telegramId, bool sortOrder)
		{
			this.CheckDatabaseConnection();
			IQueryable<IccClientTelegram> queryable = 
				from p in this.ClientTelegrams.GetAll()
				where p.DESTINATION == Settings.Default.ClientName && p.SEND_TIME >= startDate && p.SEND_TIME <= endDate
				select p;
			if (telegramId.HasValue)
			{
				queryable = 
					from p in queryable
					where (int?)p.TELEGRAM_ID == telegramId
					select p;
			}
			if (sortOrder)
			{
				queryable = 
					from p in queryable
					orderby p.SEND_TIME
					select p;
			}
			else
			{
				queryable = 
					from p in queryable
					orderby p.SEND_TIME descending
					select p;
			}
			List<IccWebService.TelegramsWithinRangeObject> list = new List<IccWebService.TelegramsWithinRangeObject>();
			foreach (IccClientTelegram current in queryable)
			{
				list.Add(new IccWebService.TelegramsWithinRangeObject
				{
					isTransferred = current.PROCESSED == true,
					PCS_Standard_Telegram_String_Form = current.BODY,
					TelegramArrivalTime = current.SEND_TIME,
					TelegramID = current.TELEGRAM_ID.ToString(),
					TelegramSenderSendTime = current.SEND_TIME,
					TelegramTransferTime = current.SEND_TIME
				});
			}
			return list.ToArray();
		}
	}
}
