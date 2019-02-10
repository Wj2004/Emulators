using System.Windows;
using System.Windows.Controls;

namespace Emulators.Pages.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            PopulateTreeView();
        }

        private void PopulateTreeView()
        {
            TreeViewItem EmulatorItem = new TreeViewItem { Header = "Emulators", DataContext = Setting.Emulators };
            Category.Items.Add(EmulatorItem);
            Category.Items.Add(new TreeViewItem { Header = "Games", DataContext = Setting.Games });
            Category.Items.Add(new TreeViewItem { Header = "Theme", DataContext = Setting.Theme });

            EmulatorItem.Focus();
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void TreeView_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = (TreeViewItem)Category.SelectedItem;
            var selectedCategory = (Setting)item.DataContext;

            switch (selectedCategory)
            {
                case Setting.Emulators:
                    SettingView.Navigate(new EmulatorsSettings());
                    break;
                case Setting.Games:
                    SettingView.Navigate(new GamesSettings());
                    break;
                case Setting.Theme:
                    break;
                default:
                    SettingView.Navigate(new EmulatorsSettings());
                    break;
            }
        }
    }

    public enum Setting
    {
        Emulators,
        Games,
        Theme
    }
}
