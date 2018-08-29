using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Logic;
using Logic.Server;
using Logic.Settings;
using WebsiteReleaseHelper.FormsControlWrappers;

namespace WebsiteReleaseHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGlobalInfo _globalInfo;

        private SiteInstance _currentSiteInstance;

        private readonly ProgressBarWrapper _primaryInstanceProgressBar;

        public MainWindow(IGlobalInfo globalInfo, IReadOnlyCollection<SiteInstance> websiteInstances)
        {
            InitializeComponent();

            _globalInfo = globalInfo;

            _primaryInstanceProgressBar = new ProgressBarWrapper(ProgressBar_Common, Dispatcher);

            FillListbox(websiteInstances);
        }

        private void FillListbox(IReadOnlyCollection<SiteInstance> websiteInstances)
        {
            ListBox_WebsiteNodes.Items.Clear();

            foreach (SiteInstance node in websiteInstances)
                ListBox_WebsiteNodes.Items.Add(node);
        }

        private void FillInfoForCurrentWebsite()
        {
            TextBlock_DisplayableName.Text = _currentSiteInstance.DisplayableName;
            TextBlock_DeployDirectoryPath.Text = _currentSiteInstance.DeployDirectoryPath;
            TextBlock_WebsiteRootPath.Text = _currentSiteInstance.WebsiteRootPath;
            TextBlock_WebsiteUrls.Text = _currentSiteInstance.WebsiteUrlsAsString;
            TextBlock_ServerMachineName.Text = _currentSiteInstance.ServerMachineName;
        }

        /// <summary>
        /// Архивирование первичной ноды
        /// </summary>
        private void Button_ArhivePrimaryInstance_OnClick(object sender, RoutedEventArgs e)
        {
            ThrowErrorIfNoSelectedInstance();

            var archiveFolderPostfix = TextBox_ArchiveFolderPostfix.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(archiveFolderPostfix))
                archiveFolderPostfix = null;

            StartAsyncOperations("Начинаю процесс архивации");

            _currentSiteInstance.Archive(archiveFolderPostfix, archiveFinishedCallback: (archiveResult) =>
            {
                UpdateInUiThread(() =>
                {
                    var messageToShow = archiveResult.Result
                        ? $"Архив сделан успешно"
                        : $"Ошибка при архивировании: {archiveResult.Exception.Message}";

                    StopAsyncOperations(messageToShow);
                });
            });
        }

        private void Button_CopyPrimaryNewFiles_OnClick(object sender, RoutedEventArgs e)
        {
            ThrowErrorIfNoSelectedInstance();

            StartAsyncOperations("Начинаю копирование файлов деплоя");

            _currentSiteInstance.CopyFilesFromDeployDirectory((copyResult) =>
            {
                UpdateInUiThread(() =>
                {
                    var messageToShow = copyResult.Result
                        ? "Копирование завершено"
                        : $"Произошла ошибка при копировании: {copyResult.Exception.Message}";

                    StopAsyncOperations(messageToShow);
                });
            });
        }

        private void StartAsyncOperations(string messageToShow)
        {
            _primaryInstanceProgressBar.Start();

            Label_StatusBarInfo.Content = messageToShow;

            Button_ArhivePrimaryInstance.IsEnabled = false;
            Button_CopyPrimaryNewFiles.IsEnabled = false;
            Button_OpenPage.IsEnabled = false;
            ListBox_WebsiteNodes.IsEnabled = false;
        }

        private void StopAsyncOperations(string messageToShow)
        {
            _primaryInstanceProgressBar.Stop();

            Label_StatusBarInfo.Content = messageToShow;

            Button_ArhivePrimaryInstance.IsEnabled = true;
            Button_CopyPrimaryNewFiles.IsEnabled = true;
            Button_OpenPage.IsEnabled = true;
            ListBox_WebsiteNodes.IsEnabled = true;
        }

        private void Button_OpenPrimaryInstancePage_OnClick(object sender, RoutedEventArgs e)
        {
            ThrowErrorIfNoSelectedInstance();

            _currentSiteInstance.OpenInWebBrowser();
        }

        /// <summary>
        /// Выполнение какой-то операции, связанной с UI, в основном потоке.
        /// Это на случай, если нужно обновить контрол на форме из вторичных потоков
        /// </summary>
        private void UpdateInUiThread(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private void ThrowErrorIfNoSelectedInstance()
        {
            if (_currentSiteInstance == null)
                throw new InvalidOperationException(@"Нужно выбрать ноду в списке");
        }

        private void ListBox_WebsiteNodes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentSiteInstance = (SiteInstance)ListBox_WebsiteNodes.SelectedItem;

            FillInfoForCurrentWebsite();
        }

        private void Button_OpenDeployDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            ThrowErrorIfNoSelectedInstance();

            _currentSiteInstance.OpenDeployDirectory();
        }

        private void Button_OpenWebsiteDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            ThrowErrorIfNoSelectedInstance();

            _currentSiteInstance.OpenWebsiteRootDirectory();
        }

        private void Button_StartIIS_OnClick(object sender, RoutedEventArgs e)
        {
            ThrowErrorIfNoSelectedInstance();

            StartAsyncOperations("Запускаю IIS");

            _currentSiteInstance.StartServerAsync(result =>
            {
                UpdateInUiThread(() =>
                {
                    var messageToShow = result.Result
                        ? $"IIS запущен"
                        : $"Ошибка при старте IIS: {result.Exception.Message}";

                    StopAsyncOperations(messageToShow);
                });
            });
        }

        private void Button_StopIIS_OnClick(object sender, RoutedEventArgs e)
        {
            ThrowErrorIfNoSelectedInstance();

            StartAsyncOperations("Останавливаю IIS");

            _currentSiteInstance.StopServerAsync(result =>
            {
                UpdateInUiThread(() =>
                {
                    var messageToShow = result.Result
                        ? $"IIS остановлен"
                        : $"Ошибка при остановке IIS: {result.Exception.Message}";

                    StopAsyncOperations(messageToShow);
                });
            });
        }
    }
}
