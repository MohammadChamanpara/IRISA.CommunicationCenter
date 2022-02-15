using IRISA.Log;
using IRISA.Properties;
using IRISA.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace IRISA
{
	public static class HelperMethods
	{
		public static List<T> LoadPlugins<T>()
		{
			return HelperMethods.LoadPlugins<T>(AppDomain.CurrentDomain.BaseDirectory + "\\Plugins");
		}
		public static List<T> LoadPlugins<T>(string directory)
		{
			List<T> result;
			try
			{
				List<T> list = new List<T>();
				if (!Directory.Exists(directory))
				{
					throw HelperMethods.CreateException("مسیر ذخیره پلاگین ها {0} موجود نیست.", new object[]
					{
						directory
					});
				}
				Type typeFromHandle = typeof(T);
				string[] files = Directory.GetFiles(directory, "*.dll");
				for (int i = 0; i < files.Length; i++)
				{
					string path = files[i];
					Type[] types = Assembly.LoadFile(path).GetTypes();
					for (int j = 0; j < types.Length; j++)
					{
						Type type = types[j];
						if (typeFromHandle.IsAssignableFrom(type) && typeFromHandle != type && !type.ContainsGenericParameters)
						{
							T item = (T)((object)Activator.CreateInstance(type));
							list.Add(item);
						}
					}
				}
				result = list;
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				if (ex is ReflectionTypeLoadException)
				{
					ReflectionTypeLoadException ex2 = ex as ReflectionTypeLoadException;
					if (ex2.LoaderExceptions != null && ex2.LoaderExceptions.Count<Exception>() > 0)
					{
						message = ex2.LoaderExceptions.First<Exception>().Message;
					}
				}
				throw HelperMethods.CreateException("خطا هنگام لود کردن پلاگین ها. متن خطا : " + message, new object[0]);
			}
			return result;
		}
		public static IrisaException CreateException(string messageFormat, params object[] parameters)
		{
			return new IrisaException(string.Format(messageFormat, parameters));
		}
		public static void CallWithTimeout(Action action, int timeoutSeconds)
		{
			Thread threadToKill = null;
			Action action2 = delegate
			{
				threadToKill = Thread.CurrentThread;
				action();
			};
			IAsyncResult asyncResult = action2.BeginInvoke(null, null);
			if (asyncResult.AsyncWaitHandle.WaitOne(timeoutSeconds * 1000))
			{
				action2.EndInvoke(asyncResult);
				return;
			}
			threadToKill.Abort();
			throw new TimeoutException();
		}
		public static string ShowPasswordDialog(string caption)
		{
			PasswordDialogForm passwordDialogForm = new PasswordDialogForm();
			passwordDialogForm.Text = caption;
			passwordDialogForm.ShowDialog();
			return passwordDialogForm.passwordTextBox.Text;
		}
		public static void ShowErrorMessage(string messageFormat, params object[] messageArguments)
		{
			new MessageForm("خطا", messageFormat, messageArguments, Properties.Resources.error).ShowDialog();
		}
		public static void ShowWarningMessage(string messageFormat, params object[] messageArguments)
		{
            new MessageForm("هشدار", messageFormat, messageArguments, Properties.Resources.warning).ShowDialog();
		}
		public static void ShowInformationMessage(string messageFormat, params object[] messageArguments)
		{
            new MessageForm("اطلاعات", messageFormat, messageArguments, Properties.Resources.info).ShowDialog();
		}
	}
}
