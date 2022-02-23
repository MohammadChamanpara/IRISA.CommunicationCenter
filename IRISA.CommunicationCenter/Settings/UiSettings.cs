using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Settings;
using System.ComponentModel;

namespace IRISA.CommunicationCenter.Settings
{
    public class UiSettings
    {
        private DLLSettings<UiSettings> dllSettings = new DLLSettings<UiSettings>();
        [DisplayName("زمان نمایش رویداد بر حسب میلی ثانیه")]
        public int NotifyIconShowTime
        {
            get
            {
                return dllSettings.FindIntValue("NotifyIconShowTime", 2000);
            }
            set
            {
                dllSettings.SaveSetting("NotifyIconShowTime", value);
            }
        }
        [DisplayName("فارسی بودن زبان نمایش رویداد")]
        public bool NotifyIconPersianLanguage
        {
            get
            {
                return dllSettings.FindBooleanValue("NotifyIconPersianLanguage", false);
            }
            set
            {
                dllSettings.SaveSetting("NotifyIconPersianLanguage", value);
            }
        }
        [DisplayName("نمایش رویداد حذف تلگرام")]
        public bool NotifyIconShowDrop
        {
            get
            {
                return dllSettings.FindBooleanValue("NotifyIconShowDrop", true);
            }
            set
            {
                dllSettings.SaveSetting("NotifyIconShowDrop", value);
            }
        }
        [DisplayName("نمایش رویداد در صف قرار گرفتن تلگرام")]
        public bool NotifyIconShowQueued
        {
            get
            {
                return dllSettings.FindBooleanValue("NotifyIconShowQueued", true);
            }
            set
            {
                dllSettings.SaveSetting("NotifyIconShowQueued", value);
            }
        }
        [DisplayName("نمایش رویداد ارسال تلگرام")]
        public bool NotifyIconShowSent
        {
            get
            {
                return dllSettings.FindBooleanValue("NotifyIconShowSent", true);
            }
            set
            {
                dllSettings.SaveSetting("NotifyIconShowSent", value);
            }
        }
        [DisplayName("نمایش رویداد متصل شدن کلاینت")]
        public bool NotifyIconShowClientConnected
        {
            get
            {
                return dllSettings.FindBooleanValue("NotifyIconShowClientConnected", true);
            }
            set
            {
                dllSettings.SaveSetting("NotifyIconShowClientConnected", value);
            }
        }
        [DisplayName("عنوان نمایش رویداد ")]
        public string NotifyIconTitle
        {
            get
            {
                return dllSettings.FindStringValue("NotifyIconTitle", "Irisa Communication Center");
            }
            set
            {
                dllSettings.SaveSetting("NotifyIconTitle", value);
            }
        }
        [DisplayName("عنوان نرم افزار ")]
        public string ProgramTitle
        {
            get
            {
                return dllSettings.FindStringValue("ProgramTitle", "نرم افزار ارتباطی ایریسا");
            }
            set
            {
                dllSettings.SaveSetting("ProgramTitle", value);
            }
        }
        [DisplayName("شرح فارسی واسط کاربر")]
        public string UiInterfacePersianDescription
        {
            get
            {
                return dllSettings.FindStringValue("UiInterfacePersianDescription", "واسط کاربر");
            }
            set
            {
                dllSettings.SaveSetting("UiInterfacePersianDescription", value);
            }
        }
        [DisplayName("تعداد رکورد هایی که در لود اولیه نمایش داده می شوند")]
        public int RecordsLoadCount
        {
            get
            {
                return dllSettings.FindIntValue("RecordsLoadCount", 10);
            }
            set
            {
                dllSettings.SaveSetting("RecordsLoadCount", value);
            }
        }
        [DisplayName("تعداد رکورد هایی که به لیست در حال نمایش اضافه می شوند")]
        public int RecordsIncrementCount
        {
            get
            {
                return dllSettings.FindIntValue("RecordsIncrementCount", 20);
            }
            set
            {
                dllSettings.SaveSetting("RecordsIncrementCount", value);
            }
        }
        [DisplayName("آدرس فایل تعریف ساختار تلگرام ها")]
        public string TelegramDefinitionFile
        {
            get
            {
                return dllSettings.FindStringValue("TelegramDefinitionFile", "TelegramDefinitions.xml");
            }
            set
            {
                dllSettings.SaveSetting("TelegramDefinitionFile", value);
            }
        }
        [Category("Information"), DisplayName("نوع نرم افزار واسط کاربر")]
        public string UIApplicationType
        {
            get
            {
                return "Microsoft Visual C# 2010 .Net Framework 4.0, Windows Forms Application. ";
            }
        }
        [Category("Information"), DisplayName("نوع فایل")]
        public string FileAssembly
        {
            get
            {
                return dllSettings.Assembly.AssemblyName();
            }
        }
        [Category("Information"), DisplayName("ورژن برنامه")]
        public string FileAssemblyVersion
        {
            get
            {
                return dllSettings.Assembly.AssemblyVersion();
            }
        }
        [Category("Information"), DisplayName("آدرس فایل")]
        public string FileAddress
        {
            get
            {
                return dllSettings.Assembly.Location;
            }
        }
    }
}
