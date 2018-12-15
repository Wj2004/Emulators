using System.Windows;
using System.Windows.Controls;

namespace Emulators.Pages
{
    public partial class ConsoleSelection : Page
    {
        public ConsoleSelection()
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
