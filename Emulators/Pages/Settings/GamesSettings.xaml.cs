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

            MakeList();
        }

        private void MakeList()
        {
            int i = 0;
            Properties.Settings.Default.Reload();

            foreach (SettingsProperty s in Properties.Settings.Default.Properties)
            {
                if (s.Name.Contains("GameFolder"))
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    LayoutRoot.RowDefinitions.Add(rowDefinition);

                    Label l = new Label()
                    {
                        Content = s.Name.Replace("GameFolder", "Gamefolder:")
                    };
                    Grid.SetRow(l, i);
                    Grid.SetColumn(l, 0);
                    LayoutRoot.Children.Add(l);

                    System.Windows.Controls.TextBox t = new TextBox()
                    {
                        Name = s.DefaultValue.ToString(),
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
                    BindingOperations.SetBinding(t, TextBox.ToolTipProperty, myBinding);

                    t.PreviewMouseUp += PreviewMouseUp;
                    Grid.SetRow(t, i);
                    Grid.SetColumn(t, 1);
                    LayoutRoot.Children.Add(t);
                    i++;
                }
            }
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
