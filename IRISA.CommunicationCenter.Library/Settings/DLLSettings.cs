using IRISA.CommunicationCenter.Library.Loggers;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace IRISA.CommunicationCenter.Library.Settings
{
    public class DLLSettings<DLLT>
    {
        private Configuration configuration;
        public Assembly Assembly
        {
            get
            {
                return typeof(DLLT).Assembly;
            }
        }
        private void RefreshConfiguration()
        {
            configuration = ConfigurationManager.OpenExeConfiguration(Assembly.Location);
        }
        private bool SettingExists(string key)
        {
            return (
                from k in configuration.AppSettings.Settings.AllKeys
                select k.ToLower()).Contains(key.ToLower());
        }
        public char FindCharacterValue(string key, char defaultValue)
        {
            string text = FindStringValue(key, defaultValue.ToString());
            if (char.TryParse(text, out char result))
            {
                return result;
            }
            throw IrisaException.Create("مقدار تعیین شده برای {0} برابر با {1} می باشد که به عنوان یک کاراکتر معتبر نیست.", new object[]
            {
                key,
                text
            });
        }
        public int FindIntValue(string key, int defaultValue)
        {
            string text = FindStringValue(key, defaultValue.ToString());
            if (int.TryParse(text, out int result))
            {
                return result;
            }
            throw IrisaException.Create("مقدار تعیین شده برای {0} برابر با {1} می باشد که به عنوان یک عدد صحیح معتبر نیست.", new object[]
            {
                key,
                text
            });
        }
        public long FindLongValue(string key, long defaultValue)
        {
            string text = FindStringValue(key, defaultValue.ToString());
            if (long.TryParse(text, out long result))
            {
                return result;
            }
            throw IrisaException.Create("مقدار تعیین شده برای {0} برابر با {1} می باشد که به عنوان یک عدد صحیح بزرگ معتبر نیست.", new object[]
            {
                key,
                text
            });
        }
        public T FindEnumValue<T>(string key, T defaultValue) where T : struct
        {
            string text = FindStringValue(key, defaultValue.ToString());
            if (Enum.TryParse(text, out T result))
            {
                return result;
            }
            throw IrisaException.Create("مقدار تعیین شده برای {0} برابر با {1} می باشد که به عنوان مقدار شمارشی معتبر نیست.", new object[]
            {
                key,
                text
            });
        }
        public bool FindBooleanValue(string key, bool defaultValue)
        {
            string text = FindStringValue(key, defaultValue.ToString());
            return bool.TryParse(text, out bool result)
                ? result
                : throw IrisaException.Create("مقدار تعیین شده برای {0} برابر با {1} می باشد که به عنوان یک مقدار بولین معتبر نیست.", new object[]
            {
                key,
                text
            });
        }
        public byte FindByteValue(string key, byte defaultValue)
        {
            string text = FindStringValue(key, defaultValue.ToString());
            if (byte.TryParse(text, out byte result))
            {
                return result;
            }
            throw IrisaException.Create("مقدار تعیین شده برای {0} برابر با {1} می باشد که به عنوان یک مقدار بایت معتبر نیست.", new object[]
            {
                key,
                text
            });
        }
        public string FindStringValue(string key, string defaultValue)
        {
            string value;
            try
            {
                RefreshConfiguration();
                if (!SettingExists(key))
                {
                    SaveSetting(key, defaultValue);
                }
                value = configuration.AppSettings.Settings[key].Value;
            }
            catch (Exception ex)
            {
                throw IrisaException.Create("خطا در بازیابی تنظیمات برای {0} . متن خطا : {1}", new object[]
                {
                    key,
                    ex.Message
                });
            }
            return value;
        }
        public void SaveSetting(string key, object newValue)
        {
            try
            {
                KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
                if (!SettingExists(key))
                {
                    settings.Add(key, newValue.ToString());
                }
                else
                {
                    settings[key].Value = newValue.ToString();
                }
                configuration.Save();
            }
            catch (Exception ex)
            {
                throw IrisaException.Create("خطا در ذخیره تنظیمات برای {0} . متن خطا : {1}", new object[]
                {
                    key,
                    ex.Message
                });
            }
        }
        public string FindConnectionString()
        {
            return FindConnectionString("ConnectionString");
        }
        public string FindConnectionString(string key)
        {
            string connectionString;
            try
            {
                RefreshConfiguration();
                ConnectionStringSettingsCollection connectionStrings = configuration.ConnectionStrings.ConnectionStrings;
                for (int i = 0; i < connectionStrings.Count; i++)
                {
                    if (connectionStrings[i].Name.ToLower() == key.ToLower())
                    {
                        connectionString = connectionStrings[i].ConnectionString;
                        return connectionString;
                    }
                }
                SaveConnectionString(key, "metadata=res://*/Model.Model.csdl|res://*/Model.Model.ssdl|res://*/Model.Model.msl;provider=Oracle.DataAccess.Client;provider connection string=\"CONNECTION TIMEOUT=5;PERSIST SECURITY INFO=True;PASSWORD=password;USER ID=UserId;DATA SOURCE=TnsName\"");
                connectionString = connectionStrings[key].ConnectionString;
            }
            catch (Exception ex)
            {
                throw IrisaException.Create("خطا در بازیابی تنظیمات برای " + key + " متن خطا : " + ex.Message);
            }
            return connectionString;
        }
        public void SaveConnectionString(string connectionString)
        {
            SaveConnectionString("ConnectionString", connectionString);
        }
        public void SaveConnectionString(string key, string connectionString)
        {
            try
            {
                ConnectionStringSettingsCollection connectionStrings = configuration.ConnectionStrings.ConnectionStrings;
                for (int i = 0; i < connectionStrings.Count; i++)
                {
                    if (connectionStrings[i].Name.ToLower() == key.ToLower())
                    {
                        connectionStrings[i].ConnectionString = connectionString;
                        configuration.Save();
                        return;
                    }
                }
                connectionStrings.Add(new ConnectionStringSettings(key, connectionString, "System.Data.EntityClient"));
                configuration.Save();
            }
            catch (Exception ex)
            {
                throw IrisaException.Create("خطا در ذخیره تنظیمات برای " + key + " متن خطا : " + ex.Message);
            }
        }
    }
}
