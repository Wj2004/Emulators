using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Emulators.Pages
{
    public partial class GameSelection : Page
    {
        public List<ButtonData> AllButtons = new List<ButtonData>();

        public GameSelection()
        {
            InitializeComponent();

            LoadButtons();
            CommandBindings.Add(new CommandBinding(Command.OpenFileLocation, OpenLocation, Allow));
            Focus();
        }

        public void LoadButtons()
        {
            PlaceButtons();
            UpdateButtonsLive();

            PopulateSortComboBoxItems();
            PopulateConsoleComboBoxItems();
        }

        private void Reload()
        {
            this.Reload();
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
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "Wii U", DataContext = ConsoleEnum.WiiU });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "N64", DataContext = ConsoleEnum.Nintendo64 });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "SNES", DataContext = ConsoleEnum.SuperNES });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "NES", DataContext = ConsoleEnum.NES });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "GameCube", DataContext = ConsoleEnum.GameCube });
            ConsoleFilter.Items.Add(new ComboBoxItem { Content = "GameBoy", DataContext = ConsoleEnum.GameBoy });
        }

        private void PlaceButtons()
        {
            var gamesFolder = $"{Properties.Settings.Default.GameFolder}";

            var listOfFiles = Directory.GetFiles($"{gamesFolder}", "*.*", SearchOption.AllDirectories);
            var listOfDirs = Directory.GetDirectories($"{gamesFolder}", "*.*", SearchOption.TopDirectoryOnly);


            if (!gamesFolder.Equals("") && Directory.Exists(gamesFolder))
            {
                //foreach (string folder in listOfDirs)
                //{
                //    if (Directory.Exists(folder) && Shortcuts.WiiUChecker.CheckForWiiUFolder(folder) != null)
                //    {
                //        MakeButton($"{Shortcuts.WiiUChecker.CheckForWiiUFolder(folder)}");
                //    }
                //}

                foreach (string file in listOfFiles)
                {
                    MakeButton($"{Path.GetFullPath(file)}");
                }
            }
            else
            {
                if (MessageBox.Show($"You need to set a folder to store your games, before you can start. Will you do it now?", "Setup", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    OpenSettings.Open("Games");
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void OpenLocation(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Opened");
        }
        private void Allow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MakeButton(string path)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Button newBtn = new Button();
                newBtn.SetResourceReference(StyleProperty, "SelectButton");
                newBtn.Name = Path.GetFileNameWithoutExtension(path).RemoveInvalidChars();
                newBtn.Content = Path.GetFileNameWithoutExtension(path);
                newBtn.Click += ClickButton;
                newBtn.DataContext = path;

                newBtn.ToolTip = Path.GetFileName(path);

                if (!ButtonHolder.Children.Contains(newBtn) && !Path.GetExtension(path).Equals(".sav"))
                {
                    ButtonHolder.Children.Add(newBtn);
                    AllButtons.Add(new ButtonData(newBtn.Name, newBtn, newBtn.Content.ToString(), File.GetLastWriteTime(path), Path.GetExtension(path)));
                }
            });
        }

        private void DeleteButton(string path)
        {
            var ButtonName = Path.GetFileNameWithoutExtension(path).RemoveInvalidChars();

            Application.Current.Dispatcher.Invoke(delegate
            {
                if (ButtonHolder.Children.Contains(AllButtons.Select(x => x.Button).First()))
                {
                    var child = ButtonHolder.Children.OfType<Control>().Where(x => x.Name == ButtonName).First();
                    ButtonHolder.Children.Remove(child);
                }
            });

            AllButtons.RemoveAll(x => x.Name == ButtonName);
        }

        #region LiveUpdateButtons
        public void UpdateButtonsLive()
        {
            string[] args = Environment.GetCommandLineArgs();
            var folderName = $"{AppDomain.CurrentDomain.BaseDirectory}Games";

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = folderName,
                IncludeSubdirectories = true,

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

            var ButtonName = Path.GetFileNameWithoutExtension(e.FullPath).RemoveInvalidChars();

            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                MakeButton(e.FullPath);
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                DeleteButton(e.FullPath);
            }

            ConsoleSort();
            SortButtons();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            var oldName = Path.GetFileNameWithoutExtension(e.OldName).RemoveInvalidChars();

            Console.WriteLine("File: {0} renamed to {1}", e.OldName, e.Name);

            DeleteButton(e.OldFullPath);
            MakeButton(e.FullPath);

            ConsoleSort();
            SortButtons();
        }
        #endregion

        private void ClickButton(object sender, RoutedEventArgs e)
        {
            var buttonName = (Button)sender;
            var file = buttonName.DataContext.ToString().AsFileParameter();

            ButtonData buttonClickedInListToConsole = AllButtons.First(x => x.Name == buttonName.Name);

            var console = buttonClickedInListToConsole.Console;

            var n64 = $"{Properties.Settings.Default.N64Emu}";
            var wii = $"{Properties.Settings.Default.WiiEmu}";
            var wiiu = $"{Properties.Settings.Default.WiiUEmu}";
            var gamecube = $"{Properties.Settings.Default.GameCubeEmu}";
            var snes = $"{Properties.Settings.Default.SnesEmu}";
            var nes = $"{Properties.Settings.Default.NesEmu}";
            var gameboy = $"{Properties.Settings.Default.GameboyEmu}";

            switch (console)
            {
                case ConsoleEnum.Nintendo64:
                    StartFile(n64, file, "Nintendo 64");
                    break;
                case ConsoleEnum.Wii:
                    StartFile(wii, file, "Wii");
                    break;
                case ConsoleEnum.GameCube:
                    StartFile(gamecube, file, "GameCube");
                    break;
                case ConsoleEnum.SuperNES:
                    StartFile(snes, file, "Super Nintendo Entertainment System");
                    break;
                case ConsoleEnum.NES:
                    StartFile(nes, file, "Nintendo Entertainment System");
                    break;
                case ConsoleEnum.GameBoy:
                    StartFile(gameboy, file, "Gameboy");
                    break;
                case ConsoleEnum.WiiU:
                    StartFile(wiiu, $"-g {file}", "Wii U");
                    break;
                default:
                    Process.Start($"{file}");
                    break;
            }
        }

        private void StartFile(string emulator, string file, string emulatorName)
        {
            if (File.Exists(emulator))
            {
                Process.Start(emulator, $"{file}");
            }
            else
            {
                if (MessageBox.Show($"You're missing an emulator for: {emulatorName}. Would you like to set a location for that emulator?", "Missing emulator", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    OpenSettings.Open("Emulators");
                }
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


        private void ConsoleSort()
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

        private void SortButtons()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                ComboBoxItem sortItem = (ComboBoxItem)SortBy.SelectedItem;
                var selectedSort = (SortEnum)sortItem.DataContext;

                List<ButtonData> buttons = new List<ButtonData>();
                foreach (Button button in ButtonHolder.Children)
                {
                    var file = button.DataContext.ToString();
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

        #region search functions
        private void Searchbar_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConsoleFilter.SelectedIndex = 0;
            Search();
            SortButtons();
        }

        private void Search()
        {
            List<ButtonData> final = new List<ButtonData>();

            final = AllButtons.Where(x => x.Content.ToString().ToLower().Contains(Searchbar.Text.ToLower())).ToList();

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

        #endregion

        private void DoubleClickWrap(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var GameFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
                Process.Start(GameFolder);
            }
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            OpenSettings.Open();
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
                //Wii
                case ".wbfs":
                case ".cISO":
                case ".wdf":
                case ".wdf1":
                case ".wdf2":
                    return ConsoleEnum.Wii;
                //Gamecube
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
                //Wii U
                case ".rpx":
                case ".wud":
                case ".wux":
                case ".wad":
                case ".elf":
                    return ConsoleEnum.WiiU;
                default:
                    return ConsoleEnum.Unknown;
            }
        }
    }

    public static class Command
    {

        public static readonly RoutedUICommand OpenFileLocation = new RoutedUICommand("Open file location", "OpenLocation", typeof(GameSelection));

    }

    public class ButtonDataPath : ButtonData
    {
        public string Path { get; set; }

        public ButtonDataPath(string name, Button button, string content, DateTime time, string path) : base(name, button, content, time)
        {
            Path = path;
        }
    }

    public enum ConsoleEnum : int
    {
        Unknown,
        All,
        Wii,
        WiiU,
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
}
