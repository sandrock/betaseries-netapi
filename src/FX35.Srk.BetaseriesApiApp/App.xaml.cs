
namespace Srk.BetaseriesApiApp
{
    using System;
    using System.Configuration;
    using System.Windows;
    using System.Windows.Threading;
    using Srk.BetaseriesApi;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var apiKey = ConfigurationManager.AppSettings["ApiKey"];
            BetaseriesClientFactory.Default = new BetaseriesClientFactory(apiKey, AppVersion.ApplicationUserAgent, true);

            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString(), "Application crash", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Application crash", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
