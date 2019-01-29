﻿using System;
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
        public List<ButtonData> AllButtons = new List<ButtonData>();

        public GameSelection()
        {
            InitializeComponent();
            ButtonKeys = new Dictionary<string, string>();

            PlaceButtons();
            UpdateButtonsLive();

            PopulateSortComboBoxItems();
            PopulateConsoleComboBoxItems();
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
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "GameBoy", DataContext = ConsoleEnum.GameBoy });
        }

        public void PlaceButtons()
        {
            var gamesFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
            if (!Directory.Exists(gamesFolder))
            {
                Directory.CreateDirectory(gamesFolder);
            }

            if (!Directory.EnumerateFileSystemEntries(gamesFolder).Any())
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    NoFiles.Visibility = Visibility.Visible;
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    NoFiles.Visibility = Visibility.Collapsed;
                });
            }

            var folder = Directory.GetFiles($"{gamesFolder}", "*.*", SearchOption.AllDirectories);
            foreach (string file in folder)
            {
                MakeButton($"{Path.GetFullPath(file)}");
            }

            //AllButtons:

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach (Button button in ButtonHolder.Children)
                {
                    var file = ButtonKeys[button.Name];
                    AllButtons.Add(new ButtonData(button.Name, button, button.Content.ToString(), File.GetLastWriteTime(file), Path.GetExtension(file)));
                }
            });
            
        }

        

        public void MakeButton(string path)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Button newBtn = new Button();
                newBtn.SetResourceReference(StyleProperty, "SelectButton");
                newBtn.Name = Path.GetFileNameWithoutExtension(path).RemoveInvalidChars();
                newBtn.Content = Path.GetFileNameWithoutExtension(path);
                newBtn.Click += ClickButton;

                newBtn.ToolTip = Path.GetFileName(path);

                if (!ButtonHolder.Children.Contains(newBtn) && !Path.GetExtension(path).Equals(".sav"))
                {
                    ButtonHolder.Children.Add(newBtn);
                    ButtonKeys.Add(Path.GetFileNameWithoutExtension(path).RemoveInvalidChars(), path);
                }
            });
        }

        #region LiveUpdateButtons
        public void UpdateButtonsLive()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            var folderName = $"{AppDomain.CurrentDomain.BaseDirectory}Games";

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = folderName,

                /* Watch for changes in LastAccess and LastWrite times, and
                   the renaming of files or directories. */
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

        }

        //When button is changed, created, or deleted
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("File: " + e.Name + " " + e.ChangeType);

            var buttonName = "Button_" + Path.GetFileNameWithoutExtension(e.FullPath).RemoveInvalidChars();

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                ButtonHolder.Children.Clear();
            });

            ButtonKeys.Clear();
            AllButtons.Clear();
            PlaceButtons();
            ConsoleSort();
            SortButtons();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("File: {0} renamed to {1}", e.OldName, e.Name);

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                ButtonHolder.Children.Clear();
            });

            ButtonKeys.Clear();
            AllButtons.Clear();
            PlaceButtons();
            ConsoleSort();
            SortButtons();
        }
        #endregion

        public void ClickButton(object sender, RoutedEventArgs e)
        {
            var buttonName = (Button)sender;
            var file = ButtonKeys[buttonName.Name].ToString().AsFileParameter();

            ButtonData buttonClickedInListToConsole = AllButtons.First(x => x.Name == buttonName.Name);

            var console = buttonClickedInListToConsole.Console;

            var emulatorsFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Emulators";
            var n64 = $"{emulatorsFolder}/Project64/Project64.exe";
            var wii = $"{emulatorsFolder}/Dolphin/Dolphin.exe";
            var gamecube = $"{emulatorsFolder}/Dolphin/Dolphin.exe";
            var snes = $"{emulatorsFolder}/Snes9X/snes9x.exe";
            var nes = $"{emulatorsFolder}/Nestopia/nestopia.exe";
            var gameboy = $"{emulatorsFolder}/VisualBoyAdvance/VisualBoyAdvance.exe";

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
                case ConsoleEnum.GameBoy:
                    Process.Start(gameboy, $"{file}");
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
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                ComboBoxItem consoleItem = (ComboBoxItem)ConsoleFilter.SelectedItem;
                var selectedConsole = (ConsoleEnum)consoleItem.DataContext;

                List<ButtonData> final = new List<ButtonData>();

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
            });
        }

        public void SortButtons()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                ComboBoxItem sortItem = (ComboBoxItem)SortBy.SelectedItem;
                var selectedSort = (SortEnum)sortItem.DataContext;

                List<ButtonData> buttons = new List<ButtonData>();
                foreach (Button button in ButtonHolder.Children)
                {
                    var file = ButtonKeys[button.Name];
                    buttons.Add(new ButtonData(button.Name, button, button.Content.ToString(), File.GetLastWriteTime(file)));
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
            });
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

        private void ButtonHolder_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var gamesFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (!Directory.Exists(gamesFolder))
                {
                    Directory.CreateDirectory(gamesFolder);
                }

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName($"{file}");

                    var buttonContent = Path.GetFileNameWithoutExtension(fileName);
                    var buttonName = "Button_" + Path.GetFileNameWithoutExtension(fileName).RemoveInvalidChars();

                    File.Move(file, $"{gamesFolder}\\{fileName}");
                }
            }
        }
    }

    public class ButtonData
    {
        public string Name { get; set; }
        public Button Button { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public string Extension { get; set; }
        public ConsoleEnum Console { get; set; }

        public ButtonData(string name, Button button, string content, DateTime time)
        {
            Name = name;
            Button = button;
            Content = content;
            Time = time;
            Extension = string.Empty;
        }

        public ButtonData(string name, Button button, string content, DateTime time, string extension) : this(name, button, content, time)
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
                case ".gb":
                case ".gba":
                    return ConsoleEnum.GameBoy;
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
        GameCube,
        GameBoy
    }

    public enum SortEnum
    {
        Recent,
        Az,
        Za
    }
    //public class ButtonData
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

    //public class ButtonDataWithExtension : ButtonData
    //{
    //    public string Extension { get; set; }

    //    public ButtonDataWithExtension(string name, Button button, string content, DateTime time, string extension) : base(name, button, content, time)
    //    {
    //        Extension = extension;
    //    }
    //}
}
