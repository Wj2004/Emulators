using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Data;

namespace Emulators.Pages.Settings
{
    public partial class EmulatorsSettings : Page
    {
        public EmulatorsSettings()
        {
            InitializeComponent();

            MakeGridList();
        }

        private void MakeGridList()
        {
            int i = 0;
            Properties.Settings.Default.Reload();

            foreach (SettingsProperty s in Properties.Settings.Default.Properties)
            {
                if (s.Name.Contains("Emu"))
                {
                    Label l = new Label()
                    {
                        Content = s.Name
                    };
                    Grid.SetRow(l, i);
                    Grid.SetColumn(l, 0);
                    LayoutRoot.Children.Add(l);

                    TextBox t = new TextBox()
                    {
                        Name = s.DefaultValue.ToString(),
                        Text = s.Provider.ToString(),
                        ToolTip = "",
                        IsReadOnly = true,
                    };

                    Binding myBinding = new Binding
                    {
                        Source = Properties.Settings.Default,
                        Path = new PropertyPath(s.Name),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };

                    BindingOperations.SetBinding(t, TextBox.TextProperty, myBinding);

                    t.PreviewMouseUp += PreviewMouseUp;
                    Grid.SetRow(t, i);
                    Grid.SetColumn(t, 1);
                    LayoutRoot.Children.Add(t);
                    i++;
                }
            }
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
            }
        }
    }
}
