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
using Logic.DirectoryHelpers;
using Path = System.IO.Path;

namespace WebsiteReleaseHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sourceDirectory = ConfigurationManager.AppSettings["primary_instance_source_dir"];
            var targetDirectory = ConfigurationManager.AppSettings["primary_instance_target_dir"];

            targetDirectory = Path.Combine(targetDirectory, ConfigurationManager.AppSettings["website_dir"]);

            var files = new FilePathsProvider(sourceDirectory, targetDirectory).GetAllFilesPaths();
            files = files;
        }
    }
}
