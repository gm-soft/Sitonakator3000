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

        private readonly ProgressBarWrapper _primaryInstanceProgressBar;

        public MainWindow(IGlobalInfo globalInfo)
        {
            InitializeComponent();

            _globalInfo = globalInfo;

            _primaryInstanceProgressBar = new ProgressBarWrapper(ProgressBar_PrimaryInstance, Dispatcher);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sourceDirectory = _globalInfo.PrimaryInstanceDeployDirectory();
            var targetDirectory = _globalInfo.GetConfig("primary_instance_target_dir");

            targetDirectory = Path.Combine(targetDirectory, _globalInfo.SiteFolderName());

            var files = new FilesToCopyProvider(sourceDirectory, targetDirectory, _globalInfo.SpecificContentFolderNames()).GetAll();
            files = files;
        }

        private void Button_ArhivePrimaryInstance_OnClick(object sender, RoutedEventArgs e)
        {
            var archiveFolderPostfix = TextBox_ArchiveFolderPostfix.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(archiveFolderPostfix))
                archiveFolderPostfix = null;

            var websiteDir = new WebsiteArchivator(
                directoryRootPath: _globalInfo.GetConfig("primary_instance_target_dir"),
                globalInfo: _globalInfo,
                archiveFinishedCallback: OnArchiveFinished);

            websiteDir.Arhive(archiveFolderPostfix);

            _primaryInstanceProgressBar.Start();

            Label_StatusBarInfo.Content = "Начинаю архивировать первичную ноду";
        }

        private void OnArchiveFinished(ArchiveResult result)
        {
            UpdateInUiThread(() =>
            {
                _primaryInstanceProgressBar.Stop();

                var messageToShow = result.Result 
                    ? "Архив сделан успешно" 
                    : $"Произошла ошибка при архивировании: {result.Exception.Message}";

                Label_StatusBarInfo.Content = messageToShow;
            });
        }

        private void UpdateInUiThread(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private void Button_CopyPrimaryNewFiles_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_OpenPrimaryInstancePage_OnClick(object sender, RoutedEventArgs e)
        {
            var websiteUrl = _globalInfo.GetConfig("primary_instance_url");
            Process.Start(websiteUrl);
        }
    }
}
