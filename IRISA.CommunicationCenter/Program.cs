using IRISA.CommunicationCenter.Forms;
using IRISA.CommunicationCenter.Library.Logging;
using System;
using System.Threading;
using System.Windows.Forms;
namespace IRISA.CommunicationCenter
{
    internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
            Mutex mutex = new Mutex(true, "Irisa.CommunicationCenter", out bool firstInstanceOfApp);
            if (!firstInstanceOfApp)
			{
				MessageForm.ShowErrorMessage("نسخه دیگری از نرم افزار مرکز ارتباطات ایریسا ( {0} ) در حال اجرا می باشد", new object[]
				{
					"ICC"
				});
			}
			else
			{
				Application.Run(new MainForm());
			}
		}
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception exception = (Exception)e.ExceptionObject;
			new LoggerInMemory().LogException(exception, "بروز خطای کنترل نشده.");
		}
		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			new LoggerInMemory().LogException(e.Exception, "بروز خطای کنترل نشده.");
		}
	}
}
