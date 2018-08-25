using System;
using System.Windows;
using System.Windows.Threading;
using Logic;
using WebsiteReleaseHelper.Helpers;

namespace WebsiteReleaseHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IGlobalInfo _globalInfo;

        /// <summary>
        /// Метод, указанный как точка входа в приложение в файле App.xaml в поле Application.Startup
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _globalInfo = new WebsiteReleaseGlobalInfo();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;

            // Create the startup window
            var wnd = new MainWindow(_globalInfo);
            wnd.Show();
        }

        private void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // From the main UI dispatcher thread in your WPF application.
            HandleException(e.Exception);
            e.Handled = true;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // From all threads in the AppDomain
            HandleException((Exception)e.ExceptionObject);
        }

        private static void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}
