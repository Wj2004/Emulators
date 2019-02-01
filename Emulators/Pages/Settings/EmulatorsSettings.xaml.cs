using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Diagnostics;

namespace Emulators.Pages.Settings
{
    public partial class EmulatorsSettings : Page
    {
        public EmulatorsSettings()
        {
            InitializeComponent();
        }

        private void PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog selectFile = new Microsoft.Win32.OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".exe",
                Filter = "EXE files (*.exe)|*.exe"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            bool? file = selectFile.ShowDialog();

            if (file.HasValue && file.Value)
            {
                string filename = selectFile.FileName;
                var t = (TextBox)sender;

                t.Text = $"{filename}";
                Properties.Settings.Default.GameCubeEmu = t.Text;
            }
        }

        private void TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
