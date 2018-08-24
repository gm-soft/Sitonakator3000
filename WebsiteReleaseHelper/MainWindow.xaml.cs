using System;
using System.Collections.Generic;
using System.Configuration;
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
using Logic.DirectoryHelpers;
using Path = System.IO.Path;

namespace WebsiteReleaseHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGlobalInfo _globalInfo;

        public MainWindow(IGlobalInfo globalInfo)
        {
            InitializeComponent();

            _globalInfo = globalInfo;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sourceDirectory = _globalInfo.PrimaryInstanceDeployDirectory();
            var targetDirectory = _globalInfo.GetConfig("primary_instance_target_dir");

            targetDirectory = Path.Combine(targetDirectory, _globalInfo.WebsiteCommonDirectoryName());

            var files = new FilePathsProvider(sourceDirectory, targetDirectory).GetAllFilesPaths();
            files = files;
        }
    }
}
