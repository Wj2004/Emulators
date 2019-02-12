using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Emulators
{
    class OpenSettings
    {
        static Pages.Settings.SettingsWindow settings;
        public static void Open(string category = "")
        {
            if (settings == null)
            {
                settings = new Pages.Settings.SettingsWindow();
            }
            settings.Closed += delegate { settings = null; };
            settings.ShowDialog();

            if (!category.Equals(""))
            {
                foreach (TreeViewItem item in settings.Category.Items)
                {
                    if (item.Header.Equals(category))
                    {
                        item.Focus();
                    }
                    else
                    {
                        Debug.WriteLine("No item with that name");
                    }
                }
            }
        }
    }
}
