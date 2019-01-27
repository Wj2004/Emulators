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
        public List<ButtonCopy> AllButtons = new List<ButtonCopy>();

        public GameSelection()
        {
            InitializeComponent();
            ButtonKeys = new Dictionary<string, string>();

            PlaceButtons();

            PopulateSortComboBoxItems();
            PopulateConsoleComboBoxItems();

            //AllButtons:
            foreach (Button button in ButtonHolder.Children)
            {
                var file = ButtonKeys[button.Name];
                AllButtons.Add(new ButtonCopy(button.Name, button, button.Content.ToString(), File.GetLastWriteTime(file), Path.GetExtension(file)));
            }
        }

        private void PopulateSortComboBoxItems()
        {
            SortBy.Items.Add(new ComboBoxItem { Content = "Recent", DataContext = SortEnum.Recent });
            SortBy.Items.Add(new ComboBoxItem { Content = "A-Z", DataContext = SortEnum.Az });
            SortBy.Items.Add(new ComboBoxItem { Content = "Z-A", DataContext = SortEnum.Za });
        }
        private void PopulateConsoleComboBoxItems()
        {
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "All", DataContext = ConsoleEnum.All });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "Wii", DataContext = ConsoleEnum.Wii });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "N64", DataContext = ConsoleEnum.Nintendo64 });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "SNES", DataContext = ConsoleEnum.SuperNES });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "NES", DataContext = ConsoleEnum.NES });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "GameCube", DataContext = ConsoleEnum.GameCube });
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
            Button newBtn = new Button();
            newBtn.SetResourceReference(StyleProperty, "SelectButton");
            newBtn.Name = name;
            newBtn.Content = content;
            newBtn.Click += ClickButton;

            ButtonHolder.Children.Add(newBtn);

            ButtonKeys.Add(name, path);
        }

        public void ClickButton(object sender, RoutedEventArgs e)
        {
            var buttonName = (Button)sender;
            var file = ButtonKeys[buttonName.Name].ToString().AsFileParameter();

            ButtonCopy buttonClickedInListToConsole = AllButtons.First(x => x.Name == buttonName.Name);

            var console = buttonClickedInListToConsole.Console;

            var emulatorsFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Emulators";
            var n64 = $"{emulatorsFolder}/Project64/Project64.exe";
            var wii = $"{emulatorsFolder}/Dolphin/Dolphin.exe";
            var gamecube = $"{emulatorsFolder}/Dolphin/Dolphin.exe";
            var snes = $"{emulatorsFolder}/Snes9X/snes9x.exe";
            var nes = $"{emulatorsFolder}/Nestopia/nestopia.exe";

            switch (console)
            {
                case ConsoleEnum.Nintendo64:
                    Process.Start(n64, $"{file}");
                    break;
                case ConsoleEnum.Wii:
                    Process.Start(wii, $"{file}");
                    break;
                case ConsoleEnum.GameCube:
                    Process.Start(gamecube, $"{file}");
                    break;
                case ConsoleEnum.SuperNES:
                    Process.Start(snes, $"{file}");
                    break;
                case ConsoleEnum.NES:
                    Process.Start(nes, $"{file}");
                    break;
                default:
                    Process.Start($"{file}");
                    break;
            }
        }

        #region sorting
        private void SortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortButtons();
        }
        private void Console_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConsoleSort();
            SortButtons();
        }


        public void ConsoleSort()
        {
            ComboBoxItem consoleItem = (ComboBoxItem)ConsoleFilter.SelectedItem;
            var selectedConsole = (ConsoleEnum)consoleItem.DataContext;

            List<ButtonCopy> final = new List<ButtonCopy>();

            if (selectedConsole == ConsoleEnum.All)
            {
                final = AllButtons;
            }
            else
            {
                final = AllButtons.Where(x => x.Console == selectedConsole).ToList();
            }

            foreach (Button button in AllButtons.Select(x => x.Button))
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    ButtonHolder.Children.Remove(button);
                });
            }
            foreach (Button button in final.Select(x => x.Button))
            {
                ButtonHolder.Children.Add(button);
            }
        }

        public void SortButtons()
        {
            ComboBoxItem sortItem = (ComboBoxItem)SortBy.SelectedItem;
            var selectedSort = (SortEnum)sortItem.DataContext;

            List<ButtonCopy> buttons = new List<ButtonCopy>();
            foreach (Button button in ButtonHolder.Children)
            {
                var file = ButtonKeys[button.Name];
                buttons.Add(new ButtonCopy(button.Name, button, button.Content.ToString(), File.GetLastWriteTime(file)));
            }

            switch (selectedSort)
            {
                case SortEnum.Recent:
                    buttons.Sort((x, y) => DateTime.Compare(y.Time, x.Time));
                    break;
                case SortEnum.Az:
                    buttons.Sort((x, y) => String.Compare(x.Content, y.Content));
                    break;
                case SortEnum.Za:
                    buttons.Sort((x, y) => String.Compare(y.Content, x.Content));
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

        Settings.SettingsWindow settings;
        public void SettingsClick(object sender, RoutedEventArgs e)
        {
            if (settings == null)
            {
                settings = new Settings.SettingsWindow();
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
                //Wii Dolphin
                case ".wbfs":
                case ".cISO":
                case ".wdf":
                case ".wdf1":
                case ".wdf2":
                    return ConsoleEnum.Wii;
                //Gamecube Dolphin
                case ".gcm":
                    return ConsoleEnum.GameCube;
                //Nintendo 64
                case ".z64":
                case ".v64":
                case ".n64":
                    return ConsoleEnum.Nintendo64;
                //Super Nintendo Entertainment System
                case ".smc":
                case ".sfc":
                    return ConsoleEnum.SuperNES;
                //Nintendo Entertainment System
                case ".nes":
                    return ConsoleEnum.NES;
                default:
                    return ConsoleEnum.Unknown;
            }
        }
    }

    public enum ConsoleEnum
    {
        Unknown,
        All,
        Wii,
        Nintendo64,
        SuperNES,
        NES,
        GameCube
    }

    public enum SortEnum
    {
        Recent,
        Az,
        Za
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
