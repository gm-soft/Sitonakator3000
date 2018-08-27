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

        /// <summary>
        /// Архивирование первичной ноды
        /// </summary>
        private void Button_ArhivePrimaryInstance_OnClick(object sender, RoutedEventArgs e)
        {
            ThrowErrorIfNoSelectedInstance();

            var archiveFolderPostfix = TextBox_ArchiveFolderPostfix.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(archiveFolderPostfix))
                archiveFolderPostfix = null;

            StartPrimaryInstanceAsyncOperations("Начат процесс архивации {}");

            _currentSiteInstance.Archive(archiveFolderPostfix, archiveFinishedCallback: (archiveResult) =>
            {
                UpdateInUiThread(() =>
                {
                    var messageToShow = archiveResult.Result
                        ? "Архив сделан успешно"
                        : $"Произошла ошибка при архивировании: {archiveResult.Exception.Message}";

                    StopPrimaryInstanceAsyncOperations(messageToShow);
                });
            });
        }

        private void Button_CopyPrimaryNewFiles_OnClick(object sender, RoutedEventArgs e)
        {
            ThrowErrorIfNoSelectedInstance();

            StartPrimaryInstanceAsyncOperations("Начинаю копирование файлов деплоя первичной ноды");

            _currentSiteInstance.CopyFilesFromDeployDirectory((copyResult) =>
            {
                UpdateInUiThread(() =>
                {
                    var messageToShow = copyResult.Result
                        ? "Копирование завершено"
                        : $"Произошла ошибка при копировании: {copyResult.Exception.Message}";

                    StopPrimaryInstanceAsyncOperations(messageToShow);
                });
            });
        }

        private void StartPrimaryInstanceAsyncOperations(string messageToShow)
        {
            _primaryInstanceProgressBar.Start();

            Label_StatusBarInfo.Content = messageToShow;

            Button_ArhivePrimaryInstance.IsEnabled = false;
            Button_CopyPrimaryNewFiles.IsEnabled = false;
        }

        private void StopPrimaryInstanceAsyncOperations(string messageToShow)
        {
            _primaryInstanceProgressBar.Stop();

            Label_StatusBarInfo.Content = messageToShow;

            Button_ArhivePrimaryInstance.IsEnabled = true;
            Button_CopyPrimaryNewFiles.IsEnabled = true;
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
        }
    }
}
