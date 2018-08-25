using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Logic.Arhivator;
using Logic.DirectoryHelpers;
using WebsiteReleaseHelper.FormsControlWrappers;
using WebsiteReleaseHelper.SiteInstances;
using Path = System.IO.Path;
using WebsiteArchivator = Logic.Arhivator.WebsiteArchivator;

namespace WebsiteReleaseHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGlobalInfo _globalInfo;

        private readonly PrimaryInstanceHelper _primaryInstanceHelper;

        private readonly ProgressBarWrapper _primaryInstanceProgressBar;

        public MainWindow(IGlobalInfo globalInfo)
        {
            InitializeComponent();

            _globalInfo = globalInfo;

            _primaryInstanceProgressBar = new ProgressBarWrapper(ProgressBar_PrimaryInstance, Dispatcher);

            _primaryInstanceHelper = new PrimaryInstanceHelper(_globalInfo);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sourceDirectory = _globalInfo.GetConfig("primary_instance_deploy_dir");
            var targetDirectory = _globalInfo.GetConfig("primary_instance_target_dir");

            targetDirectory = Path.Combine(targetDirectory, _globalInfo.SiteFolderName());

            var filesProvider = new FilesToCopyProvider(sourceDirectory, targetDirectory)
            {
                DirecoryNamesToIgnore = _globalInfo.SpecificContentFolderNames()
            };

            var files = filesProvider.GetAll();
            files = files;
        }

        /// <summary>
        /// Архивирование первичной ноды
        /// </summary>
        private void Button_ArhivePrimaryInstance_OnClick(object sender, RoutedEventArgs e)
        {
            var archiveFolderPostfix = TextBox_ArchiveFolderPostfix.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(archiveFolderPostfix))
                archiveFolderPostfix = null;

            _primaryInstanceHelper.Archive(archiveFolderPostfix, archiveFinishedCallback: (archiveResult) =>
            {
                UpdateInUiThread(() =>
                {
                    _primaryInstanceProgressBar.Stop();

                    var messageToShow = archiveResult.Result
                        ? "Архив сделан успешно"
                        : $"Произошла ошибка при архивировании: {archiveResult.Exception.Message}";

                    Label_StatusBarInfo.Content = messageToShow;
                });
            });

            _primaryInstanceProgressBar.Start();

            Label_StatusBarInfo.Content = "Начинаю архивировать первичную ноду";
        }

        private void Button_CopyPrimaryNewFiles_OnClick(object sender, RoutedEventArgs e)
        {
            _primaryInstanceHelper.CopyFilesFromDeployDirectory();
        }

        private void Button_OpenPrimaryInstancePage_OnClick(object sender, RoutedEventArgs e)
        {
            _primaryInstanceHelper.OpenInWebBrowser();
        }

        /// <summary>
        /// Выполнение какой-то операции, связанной с UI, в основном потоке.
        /// Это на случай, если нужно обновить контрол на форме из вторичных потоков
        /// </summary>
        /// <param name="action"></param>
        private void UpdateInUiThread(Action action)
        {
            Dispatcher.Invoke(action);
        }
    }
}
