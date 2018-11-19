using System;
using System.Windows;
using System.Windows.Controls;

namespace Emulators
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_ChooseGame(object sender, RoutedEventArgs e)
        {
            var selGame = new PlayGame();
            this.NavigationService.Navigate(selGame);
        }

        private void Button_Settings(object sender, RoutedEventArgs e)
        {

        }

        private void Button_About(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Quit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
