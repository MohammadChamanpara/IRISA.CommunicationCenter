using IRISA.CommunicationCenter.Core;
using IRISA.CommunicationCenter.Forms;
using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Core;
using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Oracle;
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
            try
            {
                var mutex = new Mutex(true, "Irisa.CommunicationCenter", out bool firstInstanceOfApp);
                if (!firstInstanceOfApp)
                {
                    MessageForm.ShowErrorMessage("نسخه دیگری از نرم افزار مرکز ارتباطات ایریسا در حال اجرا می باشد. ");
                    return;
                }
                GC.KeepAlive(mutex);

                InitializeApplication();

                ConfigureServices();

                Run();
            }
            catch (Exception exception)
            {
                string message = "بروز خطا هنگام اجرای اولیه برنامه";
                ServiceProvider.GetService<ILogger>().LogException(exception, message);
                MessageForm.ShowErrorMessage($"{message} \r\n{exception.Message}");
            }
        }

        static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<ILogAppender, LogAppenderInMemory>()
                .AddSingleton<ILogAppender, LogAppenderInFile>()
                .AddSingleton<IIccCore, IccCore>()
                .AddSingleton<ITelegramDefinitions, TelegramDefinitions>()
                .AddSingleton<IAdapterRepository, AdapterRepositoryInFile>()
                .AddSingleton<ITransferHistory, TransferHistoryInOracle>()
                .AddDebugServices();

            ServiceProvider = services.BuildServiceProvider();
        }

        private static IServiceCollection AddDebugServices(this IServiceCollection services)
        {
#if DEBUG

            services
                .AddSingleton<ITransferHistory, TransferHistoryInMemory>()
                .AddSingleton<IAdapterRepository, AdapterRepositoryInFileForDebug>();
#endif
            return services;
        }

        private static void Run()
        {
            Application.Run
            (
                new MainForm
                (
                    ServiceProvider.GetService<ILogger>(),
                    ServiceProvider.GetService<IIccCore>(),
                    ServiceProvider.GetService<ITelegramDefinitions>()
                )
            );
        }

        private static void InitializeApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ServiceProvider.GetService<ILogger>().LogException((Exception)e.ExceptionObject, "بروز خطای کنترل نشده.");
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ServiceProvider.GetService<ILogger>().LogException(e.Exception, "بروز خطای کنترل نشده. ");

        }
    }
}
