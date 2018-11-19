using System.Windows;

namespace Emulators
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            pageView.Navigate(new MainPage());
        }
    }
}
