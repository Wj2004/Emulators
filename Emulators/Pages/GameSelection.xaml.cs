using System.Windows;
using System.Windows.Controls;

namespace Emulators.Pages
{
    public partial class GameSelection : Page
    {
        public GameSelection()
        {
            InitializeComponent();
        }

        private void Button_Back(object sender, RoutedEventArgs e)
        {
            var main = new MainPage();
            this.NavigationService.Navigate(main);
        }
    }
}
