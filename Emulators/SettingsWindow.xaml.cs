using System;
using System.Windows;
using System.Windows.Controls;

namespace Emulators
{
    public partial class SettingsWindow : Window
    {
        String oldTheme;

        public SettingsWindow()
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
                this.Close();
            }
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

            this.Close();
        }

        private void ButtonPopup_Discard(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonPopup_Cancel(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Collapsed;
        }
    }
}
