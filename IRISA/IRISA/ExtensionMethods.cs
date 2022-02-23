using System;
using System.Globalization;
using System.IO;
using System.Reflection;
namespace IRISA
{
    public static class ExtensionMethods
    {
        public static string FormatWith(this string instance, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, instance, args);
        }
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
        public static string InnerExceptionsMessage(this Exception exception)
        {
            string text = exception.Message;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                text = text + "\r\nInner Exception: " + exception.Message;
            }
            return text;
        }
        public static Exception MostInnerException(this Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            return exception;
        }
        public static string ToPersianDateTime(this DateTime dateTime, string separator = "/")
        {
            return new PersianDateTime(dateTime)
            {
                DateSparator = separator
            }.PersianDateTimeString;
        }
        public static string ToPersianDate(this DateTime dateTime, string separator = "/")
        {
            return new PersianDateTime(dateTime)
            {
                DateSparator = separator
            }.PersianDateString;
        }
        public static DateTime ToEnglishDate(this string persianDate)
        {
            int year;
            int month;
            int day;
            PersianCalendar persianCalendar = new PersianCalendar();
            try
            {
                year = int.Parse(persianDate.Substring(0, 4));
                month = int.Parse(persianDate.Substring(5, 2));
                day = int.Parse(persianDate.Substring(8, 2));
            }
            catch (Exception)
            {
                throw new Exception("تاریخ به اشتباه وارد شده است");
            }
            if (!(year < 1404 && year > 1385))
            {
                string error = "سال باید بین 1385 تا 1404 باشد";
                throw new Exception(error);
            }

            if (!(month >= 1 && month <= 12))
            {
                string error = "ماه باید بین 1 تا 12 باشد";
                throw new Exception(error);
            }

            if (!(day >= 1 && day <= 31))
            {
                string error = "روز باید بین 1 تا 31 باشد";
                throw new Exception(error);
            }

            return persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);

        }

        public static string DigitGrouping(this int number)
        {
            string text = number.ToString();
            if (text.Length > 3)
            {
                text = text.Insert(text.Length - 3, ",");
            }
            if (text.Length > 7)
            {
                text = text.Insert(text.Length - 7, ",");
            }
            if (text.Length > 11)
            {
                text = text.Insert(text.Length - 11, ",");
            }
            return text;
        }
        public static string AsssemblyFileName(this Assembly assembly)
        {
            return Path.GetFileNameWithoutExtension(assembly.Location);
        }
        public static string AssemblyTitle(this Assembly assembly)
        {
            object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            string result;
            if (customAttributes.Length > 0)
            {
                AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute)customAttributes[0];
                if (assemblyTitleAttribute.Title != "")
                {
                    result = assemblyTitleAttribute.Title;
                    return result;
                }
            }
            result = Path.GetFileNameWithoutExtension(assembly.CodeBase);
            return result;
        }
        public static string AssemblyVersion(this Assembly assembly)
        {
            return assembly.GetName().Version.ToString();
        }
        public static string AssemblyName(this Assembly assembly)
        {
            return assembly.GetName().Name;
        }
        public static string AssemblyDescription(this Assembly assembly)
        {
            object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            string result;
            if (customAttributes.Length == 0)
            {
                result = "";
            }
            else
            {
                result = ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
            }
            return result;
        }
        public static string AssemblyProduct(this Assembly assembly)
        {
            object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            string result;
            if (customAttributes.Length == 0)
            {
                result = "";
            }
            else
            {
                result = ((AssemblyProductAttribute)customAttributes[0]).Product;
            }
            return result;
        }
        public static string AssemblyCopyright(this Assembly assembly)
        {
            object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            string result;
            if (customAttributes.Length == 0)
            {
                result = "";
            }
            else
            {
                result = ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
            }
            return result;
        }
        public static string AssemblyCompany(this Assembly assembly)
        {
            object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            string result;
            if (customAttributes.Length == 0)
            {
                result = "";
            }
            else
            {
                result = ((AssemblyCompanyAttribute)customAttributes[0]).Company;
            }
            return result;
        }
    }
}
