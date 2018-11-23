using System;
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

            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri("Resources/DarkTheme.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Add(dict);

            base.OnStartup(e);

            MainWindow mw = new MainWindow();
            mw.Show();
        }
    }
}
