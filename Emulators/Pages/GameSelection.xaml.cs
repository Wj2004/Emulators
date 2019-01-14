using System;
using System.Collections;
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
        public List<ButtonCopy> AllButtons = new List<ButtonCopy>();

        public GameSelection()
        {
            InitializeComponent();
            ButtonKeys = new Dictionary<string, string>();

            PlaceButtons();
            SortButtons();
            //Console();


            //AllButtons:
            foreach (Button button in ButtonHolder.Children)
            {
                var file = ButtonKeys[button.Name];
                AllButtons.Add(new ButtonCopy(button.Name, button, button.Content.ToString(), File.GetLastWriteTime(file), Path.GetExtension(file)));
            }
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

        #region sorting
        private void SortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortButtons();
        }
        private void Console_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConsoleSort();
        }


        public void ConsoleSort()
        {
            ComboBoxItem consoleItem = (ComboBoxItem)ConsoleFilter.SelectedItem;
            string console = consoleItem.Content.ToString();

            List<ButtonCopy> final = new List<ButtonCopy>();
            final = AllButtons;


            //final.First().Console;


            switch (console)
            {
                case "All":
                    final = AllButtons;
                    break;
                case "N64":
                    string[] n64Extensions = { ".z64", ".v64", ".n64" };
                    final.RemoveAll(x => x.Extension != ".z64");
                    break;
                default:
                    final = AllButtons;
                    break;
            }

            foreach (ButtonCopy i in final)
            {
                Debug.WriteLine($"{i.Extension}");
            }

            foreach (ButtonCopy i in AllButtons)
            {
                IEnumerable children = LogicalTreeHelper.GetChildren(ButtonHolder);
                foreach (Button child in children)
                {
                    child.Visibility = Visibility.Collapsed;
                }
            }

            foreach (Button button in AllButtons.Select(x => x.Button))
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    ButtonHolder.Children.Remove(button);
                });
                ButtonHolder.Children.Add(button);
            }
        }

        public void SortButtons()
        {
            ComboBoxItem sortItem = (ComboBoxItem)SortBy.SelectedItem;
            string sortby = sortItem.Content.ToString();


            List<ButtonCopy> buttons = new List<ButtonCopy>();
            foreach (Button button in ButtonHolder.Children)
            {
                var file = ButtonKeys[button.Name];
                buttons.Add(new ButtonCopy(button.Name, button, button.Content.ToString(), File.GetLastWriteTime(file)));
            }

            switch (sortby)
            {
                case "Recent":
                    buttons.Sort((x, y) => DateTime.Compare(y.Time, x.Time));
                    break;
                case "A-Z":
                    buttons.Sort((x, y) => String.Compare(x.Content, y.Content));
                    break;
                case "Z-A":
                    buttons.Sort((x, y) => String.Compare(y.Content, x.Content));
                    break;
                default:
                    buttons.Sort((x, y) => DateTime.Compare(y.Time, x.Time));
                    break;
            }

            foreach (Button button in buttons.Select(x => x.Button))
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
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public string Extension { get; set; }
        public ConsoleEnum Console { get; set; }

        public ButtonCopy(string name, Button button, string content, DateTime time)
        {
            Name = name;
            Button = button;
            Content = content;
            Time = time;
            Extension = string.Empty;
        }

        public ButtonCopy(string name, Button button, string content, DateTime time, string extension) : this(name, button, content, time)
        {
            Extension = extension;
            Console = GetConsoleFromExtension(extension);
        }

        private ConsoleEnum GetConsoleFromExtension(string extension)
        {
            switch (extension)
            {
                //Wii
                case ".wbfs":
                case ".cISO":
                case ".wdf":
                case ".wdf1":
                case ".wdf2":
                case ".gcm":
                    return ConsoleEnum.Wii;
                //Nintendo 64
                case ".z64":
                case ".v64":
                case ".n64":
                    return ConsoleEnum.Nintendo64;
                default:
                    return ConsoleEnum.Unknown;
            }
        }
    }

    public enum ConsoleEnum
    {
        Unknown,
        Wii,
        Nintendo64
    }
    //public class ButtonCopy
    //{
    //    public string Name { get; set; }
    //    public Button Button { get; set; }
    //    public string Content { get; set; }
    //    public DateTime Time { get; set; }

    //    public ButtonCopy(string name, Button button, string content, DateTime time)
    //    {
    //        Name = name;
    //        Button = button;
    //        Content = content;
    //        Time = time;
    //    }
    //}

    //public class ButtonCopyWithExtension : ButtonCopy
    //{
    //    public string Extension { get; set; }

    //    public ButtonCopyWithExtension(string name, Button button, string content, DateTime time, string extension) : base(name, button, content, time)
    //    {
    //        Extension = extension;
    //    }
    //}
}
