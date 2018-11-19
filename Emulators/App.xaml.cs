using System.Windows;

namespace Emulators
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow mw = new MainWindow();
            mw.Show();
        }
    }
}
