using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Logic;
using Logic.Settings;
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
            SetCallbacks();

            _globalInfo = new WebsiteReleaseGlobalInfo();

            // Create the startup window
            var wnd = new MainWindow(_globalInfo, GetWebsiteNodes());
            wnd.Show();
        }

        private IReadOnlyCollection<SiteInstance> GetWebsiteNodes()
        {
            // Ожидаем файл в той же директории, где и иполняемый файл программы.
            // В настройках сборки прописано, что должен копироваться в ту же директорию
            var filecontent = File.ReadAllText("WebsiteNodesInfo.json");

            var nodes = new SettingsParser(filecontent, _globalInfo).GetSettings();

            return nodes.Select(x => new SiteInstance(x, _globalInfo)).ToArray();
        }

        #region Обработчики ошибок

        private void SetCallbacks()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
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
            MessageBox.Show(exception.Message, caption: "Ошибка в программе");
        }

        #endregion
    }
}
