using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

        private void Button_GoBack(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Visible;
        }

        private void ThemeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)ThemeBox.SelectedItem;
            string value = typeItem.Content.ToString();

            Emulators.Properties.Settings.Default.Theme = value;

            string theme = Emulators.Properties.Settings.Default.Theme;

            ResourceDictionary dir = new ResourceDictionary();
            dir.Source = new Uri($"Resources/{theme}Theme.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Add(dir);

        }

        private void ButtonPopup_Save(object sender, RoutedEventArgs e)
        {
            Emulators.Properties.Settings.Default.Save();

            var main = new MainPage();
            this.NavigationService.Navigate(main);
        }

        private void ButtonPopup_Cancel(object sender, RoutedEventArgs e)
        {
            var main = new MainPage();
            this.NavigationService.Navigate(main);
        }
    }
}
