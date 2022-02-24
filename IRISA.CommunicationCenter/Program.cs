using IRISA.CommunicationCenter.Core;
using IRISA.CommunicationCenter.Forms;
using IRISA.CommunicationCenter.Library.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }

        [STAThread]
        private static void Main()
        {
            _ = new Mutex(true, "Irisa.CommunicationCenter", out bool firstInstanceOfApp);
            if (!firstInstanceOfApp)
            {
                MessageForm.ShowErrorMessage("نسخه دیگری از نرم افزار مرکز ارتباطات ایریسا در حال اجرا می باشد");
                return;
            }

            InitializeApplication();

            ConfigureServices();

            Application.Run(new MainForm());
        }

        private static void InitializeApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        static void ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IIccQueue, IccQueueInMemory>();
            services.AddSingleton<ILogger, LoggerInMemory>();
            ServiceProvider = services.BuildServiceProvider();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ServiceProvider.GetService<ILogger>().LogException((Exception)e.ExceptionObject, "بروز خطای کنترل نشده.");
        }
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ServiceProvider.GetService<ILogger>().LogException(e.Exception, "بروز خطای کنترل نشده.");

        }
    }
}
