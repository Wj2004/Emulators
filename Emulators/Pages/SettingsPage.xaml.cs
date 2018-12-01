using System;
using System.Windows;
using System.Windows.Controls;

namespace Emulators
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {

        public SettingsPage()
        {
            InitializeComponent();
            ThemeBox.Text = Emulators.Properties.Settings.Default.Theme;
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            var main = new MainPage();

            Emulators.Properties.Settings.Default.Save();
            this.NavigationService.Navigate(main);
        }

        private void Button_GoBack(object sender, RoutedEventArgs e)
        {
            var main = new MainPage();

            this.NavigationService.Navigate(main);
        }

        private void ThemeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)ThemeBox.SelectedItem;
            string value = typeItem.Content.ToString();

            Emulators.Properties.Settings.Default.Theme = value;
            Emulators.Properties.Settings.Default.Save();

            string theme = Emulators.Properties.Settings.Default.Theme;

            ResourceDictionary dir = new ResourceDictionary();
            dir.Source = new Uri($"Resources/{theme}Theme.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Add(dir);

        }
    }
}
