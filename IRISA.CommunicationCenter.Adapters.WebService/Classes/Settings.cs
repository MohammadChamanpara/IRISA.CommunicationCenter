using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace IRISA.CommunicationCenter
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0"), CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("IRISA2009"), DebuggerNonUserCode]
		public string WebServicePassword
		{
			get
			{
				return (string)this["WebServicePassword"];
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("PCS"), DebuggerNonUserCode]
		public string ClientName
		{
			get
			{
				return (string)this["ClientName"];
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("yyyy/MM/dd HH:mm:ss"), DebuggerNonUserCode]
		public string DateFormat
		{
			get
			{
				return (string)this["DateFormat"];
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("#"), DebuggerNonUserCode]
		public char HeaderAndBodySeparator
		{
			get
			{
				return (char)this["HeaderAndBodySeparator"];
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("$"), DebuggerNonUserCode]
		public char HeaderSeparator
		{
			get
			{
				return (char)this["HeaderSeparator"];
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("اتوماسیون ایریسا"), DebuggerNonUserCode]
		public string PersianDescription
		{
			get
			{
				return (string)this["PersianDescription"];
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("True"), DebuggerNonUserCode]
		public bool ThrowExceptionToCaller
		{
			get
			{
				return (bool)this["ThrowExceptionToCaller"];
			}
		}
	}
}
