using Emulators.Pages;
using System;
using System.Windows;
using System.Windows.Navigation;

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
            pageView.Navigate(new GameSelection());
        }
    }
}
