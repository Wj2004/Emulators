using System.Windows;
using System.Windows.Controls;

namespace Emulators.Pages
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
            Category.Items.Add(new TreeViewItem { Header = "Emulators", DataContext = Setting.Emulators });
            Category.Items.Add(new TreeViewItem { Header = "Theme", DataContext = Setting.Theme });
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {

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
        Theme
    }
}
