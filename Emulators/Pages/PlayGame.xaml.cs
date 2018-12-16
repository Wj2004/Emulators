using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Emulators
{
    public partial class PlayGame : Page
    {
        private static Dictionary<string, string> Buttons;

        public PlayGame()
        {
            InitializeComponent();
            Buttons = new Dictionary<string, string>();

            AddButtons();
            CheckDir();
        }

        //Add buttons to grid at start
        private void AddButtons()
        {
            var baseFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }

            if (IsDirectoryEmpty(baseFolder))
            {
                NoFiles.Visibility = Visibility.Visible;
            }
            else
            {
                var allFiles = Directory.GetFileSystemEntries($"{baseFolder}");

                foreach (string fileName in allFiles)
                {
                    var buttonContent = Path.GetFileNameWithoutExtension(fileName);
                    var buttonName = "Button_" + Path.GetFileNameWithoutExtension(fileName).RemoveInvalidChars();
                    
                    CreateButton(buttonContent, buttonName);
                    Buttons.Add(buttonName, fileName);
                }
            }
        }

        #region checkDirForChanges
        //Drop file into window
        private void WrapPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var folderName = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName($"{file}");

                    var buttonContent = Path.GetFileNameWithoutExtension(fileName);
                    var buttonName = "Button_" + Path.GetFileNameWithoutExtension(fileName).RemoveInvalidChars();

                    File.Move(file, $"{folderName}\\{fileName}");
                }
            }
        } 

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void CheckDir()
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

            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                var buttonContent = Path.GetFileNameWithoutExtension(e.FullPath);

                CreateButton(buttonContent, buttonName);
                Buttons.Add(buttonName, e.FullPath);

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    var baseFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
                    if (!IsDirectoryEmpty(baseFolder))
                    {
                        NoFiles.Visibility = Visibility.Collapsed;
                    }
                });
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    var child = buttonHolder.Children.OfType<Control>().Where(x => x.Name == buttonName).First();
                    buttonHolder.Children.Remove(child);
                });
                Buttons.Remove(buttonName);

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    var baseFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
                    if (IsDirectoryEmpty(baseFolder))
                    {
                        NoFiles.Visibility = Visibility.Visible;
                    }
                });
            }



        }

        //When file is renamed
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            var oldName = "Button_" + Path.GetFileNameWithoutExtension(e.OldName).RemoveInvalidChars();

            var buttonContent = Path.GetFileNameWithoutExtension(e.Name);
            var buttonName = "Button_" + Path.GetFileNameWithoutExtension(e.Name).RemoveInvalidChars();

            Console.WriteLine("File: {0} renamed to {1}", e.OldName, e.Name);

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                var child = buttonHolder.Children.OfType<Control>().Where(x => x.Name == oldName).First();
                buttonHolder.Children.Remove(child);
            });

            Buttons.Remove(oldName);

            CreateButton(buttonContent, buttonName);
            Buttons.Add(buttonName, e.FullPath);
        }
        #endregion



        //Open a file from button
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var emuFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Emulators";
            
            var button = (Button)sender;
            var file = Buttons[button.Name];

            var extension = Path.GetExtension(file);
            
            if (Directory.Exists(file))
            {
                var rpx = Directory.GetFiles(file, "*.rpx", SearchOption.AllDirectories).Length > 0;
                var wud = Directory.GetFiles(file, "*.wud", SearchOption.AllDirectories).Length > 0;
                var wux = Directory.GetFiles(file, "*.wux", SearchOption.AllDirectories).Length > 0;

                var openRpx = Directory.GetFiles(file, "*.rpx", SearchOption.AllDirectories);
                var openWud = Directory.GetFiles(file, "*.wud", SearchOption.AllDirectories);
                var openWux = Directory.GetFiles(file, "*.wux", SearchOption.AllDirectories);

                if (rpx)
                {
                    Process.Start($"{emuFolder}/Cemu/Cemu.exe", openRpx[0].AsFileParameter());
                }
                else if (wud)
                {
                    Process.Start($"{emuFolder}/Cemu/Cemu.exe", openWud[0].AsFileParameter());
                }
                else if (wux)
                {
                    Process.Start($"{emuFolder}/Cemu/Cemu.exe", openWux[0].AsFileParameter());
                }
                else
                {
                    Console.WriteLine("There's no Cemu game in this folder!");
                }


                Console.WriteLine("Works");
            }
            else if (File.Exists(file))
            {
                switch (extension)
                {
                    case ".txt":
                        Process.Start($"{file}");
                        Console.WriteLine("Opened a text file!");
                        break;
                    case ".wbfs":
                    case ".gcz":
                    case ".gcm":
                    case ".iso":
                    case ".ciso":
                        Process.Start($"{emuFolder}/Dolphin/Dolphin.exe", file.AsFileParameter());
                        Console.WriteLine("A Wii game is opening!");
                        break;
                    case ".smc":
                    case ".sfc":
                        Process.Start($"{emuFolder}/SnesGT/snesgt.exe", file.AsFileParameter());
                        Console.WriteLine("A Snes game is opening!");
                        break;
                    case ".n64":
                    case ".jsf":
                    case ".mpk":
                    case ".pj":
                    case ".pj1":
                    case ".z64":
                    case ".rdb":
                        Process.Start($"{emuFolder}/Project64/Project64.exe", file.AsFileParameter());
                        Console.WriteLine("A Nintendo 64 game is opening!");
                        break;
                    default:
                        Console.WriteLine("That filetype can't be opened!");
                        break;
                }
            }
            else
            {
                Console.WriteLine("File dosen't exist!");
            }
        }
        

        private void Mouseclick(object sender, RoutedEventArgs e)
        {
            var GameFolder = $"{AppDomain.CurrentDomain.BaseDirectory}Games";
            Process.Start(GameFolder);
        }


        //Create a button
        private void CreateButton(string buttonContent, string buttonName)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (FindName("NoFileLabel") != null)
                {
                    var child = buttonHolder.Children.OfType<Control>().Where(x => x.Name == "NoFileLabel").First();
                    buttonHolder.Children.Remove(child);
                }

                Button newBtn = new Button
                {
                    Content = buttonContent,
                    Name = buttonName.RemoveInvalidChars(),
                    BorderThickness = new Thickness(1),
                    Width = 100,
                    Height = 100
                };

                newBtn.Click += OpenFile;

                buttonHolder.Children.Add(newBtn);
            });
        }

        //If folder is empty
        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }


        private void Button_Back(object sender, RoutedEventArgs e)
        {
            var main = new MainPage();
            this.NavigationService.Navigate(main);
        }
    }
}
