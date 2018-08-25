using System;
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
using Logic.SiteInstances;
using WebsiteReleaseHelper.FormsControlWrappers;

namespace WebsiteReleaseHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PrimaryInstanceHelper _primaryInstanceHelper;

        private readonly ProgressBarWrapper _primaryInstanceProgressBar;

        public MainWindow(IGlobalInfo globalInfo)
        {
            InitializeComponent();

            _primaryInstanceProgressBar = new ProgressBarWrapper(ProgressBar_Common, Dispatcher);

            _primaryInstanceHelper = new PrimaryInstanceHelper(globalInfo);
        }

        /// <summary>
        /// Архивирование первичной ноды
        /// </summary>
        private void Button_ArhivePrimaryInstance_OnClick(object sender, RoutedEventArgs e)
        {
            var archiveFolderPostfix = TextBox_ArchiveFolderPostfix.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(archiveFolderPostfix))
                archiveFolderPostfix = null;

            StartPrimaryInstanceAsyncOperations("Начинаю архивировать первичную ноду");

            _primaryInstanceHelper.Archive(archiveFolderPostfix, archiveFinishedCallback: (archiveResult) =>
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
            StartPrimaryInstanceAsyncOperations("Начинаю копирование файлов деплоя первичной ноды");

            _primaryInstanceHelper.CopyFilesFromDeployDirectory((copyResult) =>
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
            _primaryInstanceHelper.OpenInWebBrowser();
        }

        /// <summary>
        /// Выполнение какой-то операции, связанной с UI, в основном потоке.
        /// Это на случай, если нужно обновить контрол на форме из вторичных потоков
        /// </summary>
        private void UpdateInUiThread(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
