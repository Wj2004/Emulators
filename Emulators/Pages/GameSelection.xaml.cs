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
        public List<ButtonData> AllButtons = new List<ButtonData>();

        public GameSelection()
        {
            InitializeComponent();

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

        private void PlaceButtons()
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
            var oldName = "Button_" + Path.GetFileNameWithoutExtension(e.OldName).RemoveInvalidChars();

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

            var emulatorsFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Emulators";
            var n64 = $"{emulatorsFolder}/Project64/Project64.exe";
            var wii = $"{emulatorsFolder}/Dolphin/Dolphin.exe";
            var gamecube = $"{emulatorsFolder}/Dolphin/Dolphin.exe";
            var snes = $"{emulatorsFolder}/Snes9X/snes9x.exe";
            var nes = $"{emulatorsFolder}/Nestopia/nestopia.exe";
            var gameboy = $"{emulatorsFolder}/VisualBoyAdvance/VisualBoyAdvance.exe";

            if (!File.Exists(n64))
            {
                Debug.Write("n64 emulator" + " dosen't exist");
            }

            switch (console)
            {
                case ConsoleEnum.Nintendo64:
                    StartFile(n64, file);
                    break;
                case ConsoleEnum.Wii:
                    Process.Start(wii, $"{file}");
                    break;
                case ConsoleEnum.GameCube:
                    StartFile(gamecube, file);
                    break;
                case ConsoleEnum.SuperNES:
                    StartFile(snes, file);
                    break;
                case ConsoleEnum.NES:
                    StartFile(nes, file);
                    break;
                case ConsoleEnum.GameBoy:
                    StartFile(gameboy, file);
                    break;
                default:
                    Process.Start($"{file}");
                    break;
            }
        }

        private void StartFile(string emulator, string file)
        {
            if (File.Exists(emulator))
            {
                Process.Start(emulator, $"{file}");
            }
            else
            {
                Debug.WriteLine("Emulator dosen't exist");
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

        Settings.SettingsWindow settings;
        private void SettingsClick(object sender, RoutedEventArgs e)
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
    public class ButtonDataPath : ButtonData
    {
        public string Path { get; set; }

        public ButtonDataPath(string name, Button button, string content, DateTime time, string path) : base(name, button, content, time)
        {
            Path = path;
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
}
