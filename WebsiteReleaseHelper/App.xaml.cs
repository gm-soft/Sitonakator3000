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
            _globalInfo = new WebsiteReleaseGlobalInfo();

            IReadOnlyCollection<SiteInstance> nodes = TryGetWebsiteNodes();

            if (nodes != null)
            {
                SetCallbacks();

                // Create the startup window
                var wnd = new MainWindow(_globalInfo, nodes);
                wnd.Show();
            }
            else
            {
                Shutdown();
            }
        }

        private IReadOnlyCollection<SiteInstance> TryGetWebsiteNodes()
        {
            IReadOnlyCollection<SiteInstance> result = null;
            try
            {
                // Ожидаем файл в той же директории, где и иполняемый файл программы.
                // В настройках сборки прописано, что должен копироваться в ту же директорию
                var filecontent = File.ReadAllText("WebsiteNodesInfo.json");

                var nodes = new SettingsParser(filecontent, _globalInfo).GetSettings();

                result = nodes.Select(x => new SiteInstance(x, _globalInfo)).ToArray();
            }
            catch (Exception ex)
            {
                var message = $"Произошла ошибка при чтении файла настроек.\r\n{ex.Message}";
                if (ex.InnerException != null)
                    message += "\r\n" + ex.InnerException.Message;

                HandleException(message);
            }

            return result;
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
            HandleException(exception.Message);
        }

        private static void HandleException(string errorMessage)
        {
            MessageBox.Show(errorMessage, caption: "Ошибка в программе");
        }

        #endregion
    }
}
