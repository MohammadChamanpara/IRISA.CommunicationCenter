using IRISA.CommunicationCenter.Library.Loggers;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
namespace IRISA
{
	public class DLLSettings<DLLT>
	{
		private const string defaultConnectionString = "metadata=res://*/Model.Model.csdl|res://*/Model.Model.ssdl|res://*/Model.Model.msl;provider=Oracle.DataAccess.Client;provider connection string=\"CONNECTION TIMEOUT=5;PERSIST SECURITY INFO=True;PASSWORD=password;USER ID=UserId;DATA SOURCE=TnsName\"";
		private const string defaultProviderName = "System.Data.EntityClient";
		private const string defaultConnectionStringName = "ConnectionString";
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
			this.configuration = ConfigurationManager.OpenExeConfiguration(this.Assembly.Location);
		}
		private bool SettingExists(string key)
		{
			return (
				from k in this.configuration.AppSettings.Settings.AllKeys
				select k.ToLower()).Contains(key.ToLower());
		}
		public char FindCharacterValue(string key, char defaultValue)
		{
			string text = this.FindStringValue(key, defaultValue.ToString());
			char result;
			if (char.TryParse(text, out result))
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
			string text = this.FindStringValue(key, defaultValue.ToString());
			int result;
			if (int.TryParse(text, out result))
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
			string text = this.FindStringValue(key, defaultValue.ToString());
			long result;
			if (long.TryParse(text, out result))
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
			string text = this.FindStringValue(key, defaultValue.ToString());
			T result;
			if (Enum.TryParse<T>(text, out result))
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
			string text = this.FindStringValue(key, defaultValue.ToString());
			bool result;
			if (bool.TryParse(text, out result))
			{
				return result;
			}
			throw IrisaException.Create("مقدار تعیین شده برای {0} برابر با {1} می باشد که به عنوان یک مقدار بولین معتبر نیست.", new object[]
			{
				key,
				text
			});
		}
		public byte FindByteValue(string key, byte defaultValue)
		{
			string text = this.FindStringValue(key, defaultValue.ToString());
			byte result;
			if (byte.TryParse(text, out result))
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
				this.RefreshConfiguration();
				if (!this.SettingExists(key))
				{
					this.SaveSetting(key, defaultValue);
				}
				value = this.configuration.AppSettings.Settings[key].Value;
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
				KeyValueConfigurationCollection settings = this.configuration.AppSettings.Settings;
				if (!this.SettingExists(key))
				{
					settings.Add(key, newValue.ToString());
				}
				else
				{
					settings[key].Value = newValue.ToString();
				}
				this.configuration.Save();
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
			return this.FindConnectionString("ConnectionString");
		}
		public string FindConnectionString(string key)
		{
			string connectionString;
			try
			{
				this.RefreshConfiguration();
				ConnectionStringSettingsCollection connectionStrings = this.configuration.ConnectionStrings.ConnectionStrings;
				for (int i = 0; i < connectionStrings.Count; i++)
				{
					if (connectionStrings[i].Name.ToLower() == key.ToLower())
					{
						connectionString = connectionStrings[i].ConnectionString;
						return connectionString;
					}
				}
				this.SaveConnectionString(key, "metadata=res://*/Model.Model.csdl|res://*/Model.Model.ssdl|res://*/Model.Model.msl;provider=Oracle.DataAccess.Client;provider connection string=\"CONNECTION TIMEOUT=5;PERSIST SECURITY INFO=True;PASSWORD=password;USER ID=UserId;DATA SOURCE=TnsName\"");
				connectionString = connectionStrings[key].ConnectionString;
			}
			catch (Exception ex)
			{
				throw IrisaException.Create("خطا در بازیابی تنظیمات برای " + key + " متن خطا : " + ex.Message, new object[0]);
			}
			return connectionString;
		}
		public void SaveConnectionString(string connectionString)
		{
			this.SaveConnectionString("ConnectionString", connectionString);
		}
		public void SaveConnectionString(string key, string connectionString)
		{
			try
			{
				ConnectionStringSettingsCollection connectionStrings = this.configuration.ConnectionStrings.ConnectionStrings;
				for (int i = 0; i < connectionStrings.Count; i++)
				{
					if (connectionStrings[i].Name.ToLower() == key.ToLower())
					{
						connectionStrings[i].ConnectionString = connectionString;
						this.configuration.Save();
						return;
					}
				}
				connectionStrings.Add(new ConnectionStringSettings(key, connectionString, "System.Data.EntityClient"));
				this.configuration.Save();
			}
			catch (Exception ex)
			{
				throw IrisaException.Create("خطا در ذخیره تنظیمات برای " + key + " متن خطا : " + ex.Message, new object[0]);
			}
		}
	}
}
