using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Windows.Data;
using System.Collections.Generic;
using System;

namespace Emulators.Pages.Settings
{
    public partial class ThemeSettings : Page
    {
        public ThemeSettings()
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
                if (s.Name.Contains("Theme"))
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    HolderGrid.RowDefinitions.Add(rowDefinition);

                    Label l = new Label()
                    {
                        Content = s.Name + ":"
                    };
                    Grid.SetRow(l, i);
                    Grid.SetColumn(l, 0);
                    HolderGrid.Children.Add(l);

                    ComboBox t = new ComboBox()
                    {
                        Name = s.DefaultValue.ToString(),
                        IsReadOnly = false,

                    };
                    t.SelectionChanged += OnChanged;

                    t.Items.Add(new ComboBoxItem() { Content = "Dark" });
                    t.Items.Add(new ComboBoxItem() { Content = "Light" });

                    t.Text = Properties.Settings.Default.Theme;

                    Grid.SetRow(t, i);
                    Grid.SetColumn(t, 1);
                    HolderGrid.Children.Add(t);
                    i++;
                }
            }
        }

        private void OnChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            ComboBoxItem selectedCar = (ComboBoxItem)box.SelectedItem;

            Properties.Settings.Default.Theme = selectedCar.Content.ToString();
            Properties.Settings.Default.Save();

            App.Load();
        }
    }
}
