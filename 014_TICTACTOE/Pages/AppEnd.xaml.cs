using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using _014_TICTACTOE.Pages;

namespace _014_TICTACTOE.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy AppEnd.xaml
    /// </summary>
    public partial class AppEnd : Page
    {
        public AppEnd(string text)
        {
            InitializeComponent();
            if (text.Length > 7)
            {
                GameResult.FontSize = 120;
                GameResult.LineHeight = 110;
            }
            else
            {
                GameResult.FontSize = 180;
                GameResult.LineHeight = 150;
            }
            GameResult.Text = text;
        }

        private void CreateRoomClicked(object sender, RoutedEventArgs e)
        {
            AppStart.cellUserValue = 1;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/creategame.php?username=" + AppStart.UserName);
            myRequest.Method = "POST";
            var resp = (HttpWebResponse)myRequest.GetResponse();

            MainWindow.MainFrame.NavigationService.Navigate(new AppGame());
        }

        private void MenuClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrame.NavigationService.Navigate(new AppStart());
        }

        private void RevengeClicked(object sender, RoutedEventArgs e)
        {

            MainWindow.MainFrame.NavigationService.Navigate(new AppGame());
        }
    }
}
