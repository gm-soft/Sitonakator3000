using System.Windows;
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

            // Create the startup window
            var wnd = new MainWindow(_globalInfo);
            wnd.Show();
        }
    }
}
