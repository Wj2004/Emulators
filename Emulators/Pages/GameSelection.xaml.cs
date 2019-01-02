using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Emulators.Pages
{
    public partial class GameSelection : Page
    {
        private static Dictionary<string, string> ButtonKeys;

        public GameSelection()
        {
            InitializeComponent();
            ButtonKeys = new Dictionary<string, string>();

            PlaceButtons();
            SortButtons();
            //Console();
        }

        public void PlaceButtons()
        {
            var baseFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }

            var folder = Directory.GetFiles($"{baseFolder}", "*.*", SearchOption.AllDirectories);
            foreach (string file in folder)
            {
                MakeButton($"{Path.GetFileNameWithoutExtension(file).RemoveInvalidChars()}", $"{Path.GetFileNameWithoutExtension(file)}", $"{Path.GetFullPath(file)}");
            }
        }

        public void MakeButton(string name, string content, string path)
        {
            Button newBtn = new System.Windows.Controls.Button();
            newBtn.SetResourceReference(StyleProperty, "SelectButton");
            newBtn.Name = name;
            newBtn.Content = content;
            newBtn.Click += ClickButton;

            ButtonHolder.Children.Add(newBtn);

            ButtonKeys.Add(name, path);
        }

        public void ClickButton(object sender, RoutedEventArgs e)
        {
            var buttonName = (System.Windows.Controls.Button)sender;
            var file = ButtonKeys[buttonName.Name];

            Process.Start($"{file}");
        }

        public void Console()
        {
            ComboBoxItem consoleItem = (ComboBoxItem)SortBy.SelectedItem;
            string console = consoleItem.Content.ToString();
        }

        #region sorting
        private void SortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortButtons();
        }

        public void SortButtons()
        {
            ComboBoxItem sortItem = (ComboBoxItem)SortBy.SelectedItem;
            string sortby = sortItem.Content.ToString();

            IEnumerable<Button> orderedButtons = ButtonHolder.Children.OfType<System.Windows.Controls.Button>();
            List<ButtonCopy> lastAccessTimes = new List<ButtonCopy>();

            foreach (Button button in orderedButtons)
            {
                var file = ButtonKeys[button.Name];
                lastAccessTimes.Add(new ButtonCopy(button.Name, button, File.GetLastWriteTime(file)));
            }

            switch (sortby)
            {
                case "Recent":
                    lastAccessTimes.Sort((x, y) => DateTime.Compare(y.Time, x.Time));
                    orderedButtons = lastAccessTimes.Select(x => x.Button);
                    break;
                case "A-Z":
                    orderedButtons = ButtonHolder.Children.OfType<Button>().OrderBy(button => button.Content);
                    break;
                case "Z-A":
                    orderedButtons = ButtonHolder.Children.OfType<Button>().OrderByDescending(button => button.Content);
                    break;
                default:
                    orderedButtons = ButtonHolder.Children.OfType<Button>().OrderBy(button => button.Content);
                    break;
            }

            foreach (System.Windows.Controls.Button button in orderedButtons)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    ButtonHolder.Children.Remove(button);
                });
                ButtonHolder.Children.Add(button);
            }
        }
        #endregion

        public void DoubleClickWrap(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var GameFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
                Process.Start(GameFolder);
            }
        }
    }

    public class ButtonCopy
    {
        public string Name { get; set; }
        public Button Button { get; set; }
        //public string PathToFile { get; set; }
        public DateTime Time { get; set; }

        public ButtonCopy(string name, Button button, DateTime time)
        {
            Name = name;
            Button = button;
            Time = time;
        }
    }
}
