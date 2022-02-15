using System;
using System.ComponentModel;
namespace IRISA.CommunicationCenter
{
	public class UiSettings
	{
		private DLLSettings<UiSettings> dllSettings = new DLLSettings<UiSettings>();
		[DisplayName("زمان نمایش رویداد بر حسب میلی ثانیه")]
		public int NotifyIconShowTime
		{
			get
			{
				return this.dllSettings.FindIntValue("NotifyIconShowTime", 2000);
			}
			set
			{
				this.dllSettings.SaveSetting("NotifyIconShowTime", value);
			}
		}
		[DisplayName("فارسی بودن زبان نمایش رویداد")]
		public bool NotifyIconPersianLanguage
		{
			get
			{
				return this.dllSettings.FindBooleanValue("NotifyIconPersianLanguage", false);
			}
			set
			{
				this.dllSettings.SaveSetting("NotifyIconPersianLanguage", value);
			}
		}
		[DisplayName("نمایش رویداد حذف تلگرام")]
		public bool NotifyIconShowDrop
		{
			get
			{
				return this.dllSettings.FindBooleanValue("NotifyIconShowDrop", true);
			}
			set
			{
				this.dllSettings.SaveSetting("NotifyIconShowDrop", value);
			}
		}
		[DisplayName("نمایش رویداد در صف قرار گرفتن تلگرام")]
		public bool NotifyIconShowQueued
		{
			get
			{
				return this.dllSettings.FindBooleanValue("NotifyIconShowQueued", true);
			}
			set
			{
				this.dllSettings.SaveSetting("NotifyIconShowQueued", value);
			}
		}
		[DisplayName("نمایش رویداد ارسال تلگرام")]
		public bool NotifyIconShowSent
		{
			get
			{
				return this.dllSettings.FindBooleanValue("NotifyIconShowSent", true);
			}
			set
			{
				this.dllSettings.SaveSetting("NotifyIconShowSent", value);
			}
		}
		[DisplayName("نمایش رویداد متصل شدن کلاینت")]
		public bool NotifyIconShowClientConnected
		{
			get
			{
				return this.dllSettings.FindBooleanValue("NotifyIconShowClientConnected", true);
			}
			set
			{
				this.dllSettings.SaveSetting("NotifyIconShowClientConnected", value);
			}
		}
		[DisplayName("عنوان نمایش رویداد ")]
		public string NotifyIconTitle
		{
			get
			{
				return this.dllSettings.FindStringValue("NotifyIconTitle", "Irisa Communication Center");
			}
			set
			{
				this.dllSettings.SaveSetting("NotifyIconTitle", value);
			}
		}
		[DisplayName("عنوان نرم افزار ")]
		public string ProgramTitle
		{
			get
			{
				return this.dllSettings.FindStringValue("ProgramTitle", "نرم افزار ارتباطی ایریسا");
			}
			set
			{
				this.dllSettings.SaveSetting("ProgramTitle", value);
			}
		}
		[DisplayName("شرح فارسی واسط کاربر")]
		public string UiInterfacePersianDescription
		{
			get
			{
				return this.dllSettings.FindStringValue("UiInterfacePersianDescription", "واسط کاربر");
			}
			set
			{
				this.dllSettings.SaveSetting("UiInterfacePersianDescription", value);
			}
		}
		[DisplayName("تعداد رکورد هایی که در لود اولیه نمایش داده می شوند")]
		public int RecordsLoadCount
		{
			get
			{
				return this.dllSettings.FindIntValue("RecordsLoadCount", 10);
			}
			set
			{
				this.dllSettings.SaveSetting("RecordsLoadCount", value);
			}
		}
		[DisplayName("تعداد رکورد هایی که به لیست در حال نمایش اضافه می شوند")]
		public int RecordsIncrementCount
		{
			get
			{
				return this.dllSettings.FindIntValue("RecordsIncrementCount", 20);
			}
			set
			{
				this.dllSettings.SaveSetting("RecordsIncrementCount", value);
			}
		}
		[DisplayName("آدرس فایل تعریف ساختار تلگرام ها")]
		public string TelegramDefinitionFile
		{
			get
			{
				return this.dllSettings.FindStringValue("TelegramDefinitionFile", "TelegramDefinitions.xml");
			}
			set
			{
				this.dllSettings.SaveSetting("TelegramDefinitionFile", value);
			}
		}
		[Category("ReadOnly"), DisplayName("نوع نرم افزار واسط کاربر")]
		public string UIApplicationType
		{
			get
			{
				return "Microsoft Visual C# 2010 .Net Framework 4.0, Windows Forms Application. ";
			}
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
	}
}
