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
            ResourceDictionary LightDicti = new ResourceDictionary();
            LightDicti.Source = new Uri("Resources/LightTheme.xaml", UriKind.Relative);

            ResourceDictionary DarkDicti = new ResourceDictionary();
            DarkDicti.Source = new Uri("Resources/DarkTheme.xaml", UriKind.Relative);

            if (Emulators.Properties.Settings.Default.LightTheme == true)
            {
                Application.Current.Resources.MergedDictionaries.Add(LightDicti);
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(DarkDicti);
            }




            

            base.OnStartup(e);

            MainWindow mw = new MainWindow();
            mw.Show();
        }
    }
}
