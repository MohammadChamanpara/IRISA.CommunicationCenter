using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Settings;
using System.ComponentModel;

namespace IRISA.CommunicationCenter.Settings
{
    public class UiSettings
    {
        private readonly DLLSettings<UiSettings> dllSettings = new DLLSettings<UiSettings>();
        
        [DisplayName("طول نمایش رویداد در ویندوز بر حسب میلی ثانیه")]
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

        [DisplayName("فارسی بودن زبان نمایش رویداد در ویندوز")]
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
    
        [DisplayName("نمایش رویداد متصل شدن کلاینت در ویندوز")]
        public bool NotifyIconShowAdapterConnected
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

        [DisplayName("عنوان نمایش رویداد در ویندوز")]
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

        [DisplayName("دوره زمانی بازیابی رکورد ها بر حسب میلی ثانیه")]
        public int RecordsRefreshInterval
        {
            get
            {
                return dllSettings.FindIntValue("RecordsRefreshInterval", 500);
            }
            set
            {
                dllSettings.SaveSetting("RecordsRefreshInterval", value);
            }
        }

        [DisplayName("طول زمان فعال بودن پروسه بازیابی رکورد ها بر حسب ثانیه")]
        public int RecordsRefreshAliveTime
        {
            get
            {
                return dllSettings.FindIntValue("RecordsRefreshAliveTime", 120);
            }
            set
            {
                dllSettings.SaveSetting("RecordsRefreshAliveTime", value);
            }
        }

        [DisplayName("تعداد رکورد هایی که در لود اولیه نمایش داده می شوند")]
        public int RecordsLoadCount
        {
            get
            {
                return dllSettings.FindIntValue("RecordsLoadCount", 50);
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
                return dllSettings.FindIntValue("RecordsIncrementCount", 200);
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
                return "Microsoft Visual C# .Net Framework 4.8, Windows Forms Application. ";
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
