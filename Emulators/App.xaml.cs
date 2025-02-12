﻿using System;
using System.Windows;

namespace Emulators
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Load();

            base.OnStartup(e);

            MainWindow mw = new MainWindow();
            mw.Show();
        }

        public static void Load()
        {
            ResourceDictionary LightDicti = new ResourceDictionary();
            LightDicti.Source = new Uri("Resources/LightTheme.xaml", UriKind.Relative);

            ResourceDictionary DarkDicti = new ResourceDictionary();
            DarkDicti.Source = new Uri("Resources/DarkTheme.xaml", UriKind.Relative);

            if (Emulators.Properties.Settings.Default.Theme == "Light")
            {
                Application.Current.Resources.MergedDictionaries.Add(LightDicti);
            }
            else if (Emulators.Properties.Settings.Default.Theme == "Dark")
            {
                Application.Current.Resources.MergedDictionaries.Add(DarkDicti);
            }
        }
    }
}
