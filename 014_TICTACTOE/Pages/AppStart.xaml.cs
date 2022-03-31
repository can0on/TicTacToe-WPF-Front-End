using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logika interakcji dla klasy AppStart.xaml
    /// </summary>
    public partial class AppStart : Page
    {
        internal static string? UserName { get; set; }
        internal static int cellUserValue = 0;

        public AppStart()
        {
            InitializeComponent();
        }

        private void CreateRoomClicked(object sender, RoutedEventArgs e)
        {
            UserName = UserNick.Text;
            cellUserValue = 1;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/creategame.php?username=" + UserName);
            myRequest.Method = "POST";
            var resp = (HttpWebResponse)myRequest.GetResponse();

            MainWindow.MainFrame.NavigationService.Navigate(new AppGame());
        }

        private void JoinGameClicked(object sender, RoutedEventArgs e)
        {
            UserName = UserNick.Text;
            cellUserValue = 2;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/joingame.php?username=" + UserName + "&room=" + RoomCode.Text);
            myRequest.Method = "POST";
            var resp = (HttpWebResponse)myRequest.GetResponse();

            MainWindow.MainFrame.NavigationService.Navigate(new AppGame());
        }
    }
}
