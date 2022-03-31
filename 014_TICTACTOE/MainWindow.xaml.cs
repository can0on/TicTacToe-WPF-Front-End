using _014_TICTACTOE.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _014_TICTACTOE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static Frame? MainFrame;
        bool isActionMenuVisible = false;
        public MainWindow()
        {
            InitializeComponent();
            AppFrame.NavigationService.Navigate(new AppStart());
            MainFrame = AppFrame;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        
        private void ChangeActionMenuState(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (!isActionMenuVisible)
                {
                    ActionMenu.Visibility = Visibility.Visible;
                    isActionMenuVisible = true;
                }
                else
                {
                    ActionMenu.Visibility = Visibility.Collapsed;
                    isActionMenuVisible = false;
                }
            }
        }

        private void NavigateToAppStart(object sender, RoutedEventArgs e)
        {
            AppFrame.NavigationService.Navigate(new AppStart());
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
