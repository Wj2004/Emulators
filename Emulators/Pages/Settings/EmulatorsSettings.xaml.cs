using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Data;
using System.Collections.Generic;

namespace Emulators.Pages.Settings
{
    public partial class EmulatorsSettings : Page
    {
        public EmulatorsSettings()
        {
            InitializeComponent();

            MakeDesign();
            MakeGridList();
        }

        void MakeDesign()
        {
            foreach (SettingsProperty s in Properties.Settings.Default.Properties)
            {
                if (s.Name.Contains("Emu"))
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    HolderGrid.RowDefinitions.Add(rowDefinition);
                }
            }
        }

        public List<SettingPair> SettingsList = new List<SettingPair>();

        private void MakeGridList()
        {
            Properties.Settings.Default.Reload();
            foreach (SettingsProperty s in Properties.Settings.Default.Properties)
            {
                if (s.Name.Contains("Emu"))
                {
                    SettingsList.Add(new SettingPair(LabelMaker(s.Name.Replace("Emu", ":")), TextBoxMaker(s.DefaultValue.ToString(), s.Name)));
                    SettingsList.Sort((x, y) => String.Compare(x.EmuLabel.Content.ToString(), y.EmuLabel.Content.ToString()));
                }
            }
            int i = 0;
            foreach (SettingPair setting in SettingsList)
            {
                Label label = setting.EmuLabel;
                TextBox textbox = setting.Path;
                Grid.SetRow(label, i);
                Grid.SetRow(textbox, i);

                HolderGrid.Children.Add(setting.EmuLabel);
                HolderGrid.Children.Add(setting.Path);
                i++;
            }
        }

        private Label LabelMaker(string content)
        {
            Label l = new Label()
            {
                Content = content
            };
            Grid.SetColumn(l, 0);
            return l;
        }

        private TextBox TextBoxMaker(string name, string setting)
        {
            TextBox t = new TextBox()
            {
                Name = name,
                IsReadOnly = true,
            };

            Binding myBinding = new Binding
            {
                Source = Properties.Settings.Default,
                Path = new PropertyPath(setting),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            BindingOperations.SetBinding(t, TextBox.TextProperty, myBinding);
            BindingOperations.SetBinding(t, TextBox.ToolTipProperty, myBinding);

            t.PreviewMouseUp += PreviewMouseUp;
            Grid.SetColumn(t, 1);
            return t;
        }


        private new void PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

    public class SettingPair
    {
        public Label EmuLabel { get; set; }
        public TextBox Path { get; set; }

        public SettingPair(Label label, TextBox textBox)
        {
            EmuLabel = label;
            Path = textBox;
        }
    }

}
