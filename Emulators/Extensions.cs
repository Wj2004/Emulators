using System.Text.RegularExpressions;
using System.Windows;

namespace Emulators
{
    public static class Extensions
    {
        public static string AsFileParameter(this string fileName) => $"\"{fileName}\"";

        public static string RemoveInvalidChars(this string invalidString)
        {
            return Regex.Replace(invalidString, "[^a-zA-Z0-9_]+", string.Empty, RegexOptions.Compiled);
        }

        static Pages.Settings.SettingsWindow settings;
        public static void OpenSettings()
        {
            if (settings == null)
            {
                settings = new Pages.Settings.SettingsWindow();
            }
            settings.Closed += delegate { settings = null; };

            if (settings.WindowState == WindowState.Minimized)
            {
                settings.WindowState = WindowState.Normal;
            }
            settings.Show();
            settings.Activate();
        }
    }
}
