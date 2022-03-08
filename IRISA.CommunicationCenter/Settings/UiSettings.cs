using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Settings;
using System.ComponentModel;

namespace IRISA.CommunicationCenter.Settings
{
    public class UiSettings
    {
        private readonly DLLSettings<UiSettings> dllSettings = new DLLSettings<UiSettings>();

        [Category("عملیات")]
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

        [Category("عملیات")]
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

        [Category("عملیات")]
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

        [Category("عملیات")]
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

        [Category("عملیات")]
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

        [Category("اطلاعات")]
        [DisplayName("نوع نرم افزار واسط کاربر")]
        public string UIApplicationType
        {
            get
            {
                return "Microsoft Visual C# .Net Framework 4.8, Windows Forms Application. ";
            }
        }
        [Category("اطلاعات")]
        [DisplayName("نوع فایل")]
        public string FileAssembly
        {
            get
            {
                return dllSettings.Assembly.AssemblyName();
            }
        }
        [Category("اطلاعات")]
        [DisplayName("ورژن برنامه")]
        public string FileAssemblyVersion
        {
            get
            {
                return dllSettings.Assembly.AssemblyVersion();
            }
        }
        [Category("اطلاعات")]
        [DisplayName("آدرس فایل")]
        public string FileAddress
        {
            get
            {
                return dllSettings.Assembly.Location;
            }
        }
    }
}
