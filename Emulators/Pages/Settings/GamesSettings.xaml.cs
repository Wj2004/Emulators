using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using Binding = System.Windows.Data.Binding;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;

namespace Emulators.Pages.Settings
{
    /// <summary>
    /// Interaction logic for GamesSettings.xaml
    /// </summary>
    public partial class GamesSettings : Page
    {
        public GamesSettings()
        {
            InitializeComponent();

            MakeDesign();
            MakeGridList();
        }

        void MakeDesign()
        {
            foreach (SettingsProperty s in Properties.Settings.Default.Properties)
            {
                if (s.Name.Contains("GameFolder"))
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
                if (s.Name.Contains("GameFolder"))
                {
                    SettingsList.Add(new SettingPair(LabelMaker(s.Name.Replace("GameFolder", "Gamefolder:")), TextBoxMaker(s.DefaultValue.ToString(), s.Name)));
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
            FolderBrowserDialog selectFolder = new FolderBrowserDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            DialogResult folder = selectFolder.ShowDialog();

            if (folder.ToString().Equals("OK"))
            {
                var t = (TextBox)sender;

                t.Text = $"{selectFolder.SelectedPath}";
            }
        }
    }
}
