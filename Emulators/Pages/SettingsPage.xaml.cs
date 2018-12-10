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
        String oldTheme;

        public SettingsPage()
        {
            InitializeComponent();
            oldTheme = Emulators.Properties.Settings.Default.Theme;

            ThemeBox.Text = Emulators.Properties.Settings.Default.Theme;
        }

        private void Button_GoBack(object sender, RoutedEventArgs e)
        {
            if (oldTheme != Emulators.Properties.Settings.Default.Theme)
            {
                Overlay.Visibility = Visibility.Visible;
            }
            else
            {
                var main = new MainPage();
                this.NavigationService.Navigate(main);
            }
            
        }

        private void ThemeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)ThemeBox.SelectedItem;
            string value = typeItem.Content.ToString();

            Emulators.Properties.Settings.Default.Theme = value;

            ChangeTheme();
        }

        private void ButtonPopup_Save(object sender, RoutedEventArgs e)
        {
            Emulators.Properties.Settings.Default.Save();

            var main = new MainPage();
            this.NavigationService.Navigate(main);
        }

        private void ButtonPopup_Discard(object sender, RoutedEventArgs e)
        { 
            Emulators.Properties.Settings.Default.Theme = oldTheme;
            Emulators.Properties.Settings.Default.Save();

            ChangeTheme();

            var main = new MainPage();
            this.NavigationService.Navigate(main);
        }

        private void ButtonPopup_Cancel(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Collapsed;
        }


        void ChangeTheme()
        {
            string theme = Emulators.Properties.Settings.Default.Theme;

            ResourceDictionary dir = new ResourceDictionary();
            dir.Source = new Uri($"Resources/{theme}Theme.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Add(dir);
        }
    }
}
