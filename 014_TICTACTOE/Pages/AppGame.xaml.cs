using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace _014_TICTACTOE.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy AppGame.xaml
    /// </summary>
    public partial class AppGame : Page
    {
        List<int> board;
        int cellUserValue;
        string room;
        Image imageX;
        Image imageCircle;
        Timer checkWinTimer;
        Timer clearBoardTimer;
        Timer twoPlayersInTheRoom;
        int playerWinner = 0;
        public AppGame()
        {
            InitializeComponent();
            getLastUserRoom();
            board = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            cellUserValue = AppStart.cellUserValue;
            GameResultInfo.Text = getResult(room);

            twoPlayersInTheRoom = new();
            twoPlayersInTheRoom.Interval = 500;
            twoPlayersInTheRoom.Elapsed += TwoPlayersInTheRoom_Elapsed;
            twoPlayersInTheRoom.Start();

            Timer turnRequestTimer = new();
            turnRequestTimer.Interval = 500;
            turnRequestTimer.Elapsed += TurnRequestTimer_Elapsed;
            turnRequestTimer.Start();

            checkWinTimer = new();
            checkWinTimer.Interval = 1000;
            checkWinTimer.Elapsed += CheckWinTimer_Elapsed;
            checkWinTimer.Start();
        }

        private void TwoPlayersInTheRoom_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                Player1Info.Text = getPlayerName(1);
                Player2Info.Text = getPlayerName(2);
                twoPlayersInTheRoom.Interval = 1000;
                if (board.Any(x => x == 2))
                    twoPlayersInTheRoom.Stop();
            }));
        }

        private void CheckWinTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            checkWin();
        }

        private void TurnRequestTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (isYourTurn())
            {
                getBoard();
                updateBoard();
            }
        }
        private void getBoard()
        {
            HttpWebRequest boardRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/getboard.php?room=" + room);
            boardRequest.Method = "GET";
            var boardResp = (HttpWebResponse)boardRequest.GetResponse();
            var boardJson = new StreamReader(boardResp.GetResponseStream()).ReadToEnd();
            board = new List<int>(JsonConvert.DeserializeObject<List<int>>(boardJson));
        }
        private void updateBoard()
        {
            List<Button> boardButtons = new List<Button> { b00, b01, b02, b10, b11, b12, b20, b21, b22};
            for (int i = 0; i < boardButtons.Count; i++)
            {
                if (board[i] == 1)
                { 
                    Dispatcher.Invoke(new Action(() => {
                        imageX = new();
                        imageX.Width = 80;
                        imageX.Source = new BitmapImage(new Uri(@"/Images/White/x_sign.png", UriKind.RelativeOrAbsolute));
                        boardButtons[i].Content = imageX;
                        }));
                }
                else if (board[i] == 2)
                {
                    Dispatcher.Invoke(new Action(() => {
                        imageCircle = new();
                        imageCircle.Width = 80;
                        imageCircle.Source = new BitmapImage(new Uri(@"/Images/White/circle.png", UriKind.RelativeOrAbsolute));
                        boardButtons[i].Content = imageCircle;
                        }));
                }
            }
        }
        private void CellClicked(object sender, RoutedEventArgs e)
        { 
            if (isYourTurn())
            {
                UserTurn(sender, e);
            }
        }
        private void UserTurn(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var buttonColumn = Grid.GetColumn(button);
            var buttonRow = Grid.GetRow(button);
            var buttonIndex = buttonColumn + (buttonRow * 3);

            var dbCellNames = new List<string> { "c00", "c01", "c02", "c10", "c11", "c12", "c20", "c21", "c22" };

            if (board[buttonIndex] == 0)
            {
                board[buttonIndex] = cellUserValue;

                imageX = new();
                imageX.Width = 80;
                imageX.Source = new BitmapImage(new Uri(@"/Images/White/x_sign.png", UriKind.RelativeOrAbsolute));

                imageCircle = new();
                imageCircle.Width = 80;
                imageCircle.Source = new BitmapImage(new Uri(@"/Images/White/circle.png", UriKind.RelativeOrAbsolute));

                addPlayerTurn(dbCellNames[buttonIndex]);

                if (cellUserValue == 1)
                    button.Content = imageX;
                if (cellUserValue == 2)
                    button.Content = imageCircle;

                TurnInfo.Text = "";
            }
        }
        private void addPlayerTurn(string cell)
        {
            HttpWebRequest playerTurnRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/addplayerturn.php?room=" + room + "&cell=" + cell);
            playerTurnRequest.Method = "POST";
            var playerTurnResp = (HttpWebResponse)playerTurnRequest.GetResponse();
        }

        private bool isYourTurn()
        {
            return getWhoseTurn();
        }
        private void getLastUserRoom()
        {
            HttpWebRequest lastUserRoomRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/lastuserroom.php?username=" + AppStart.UserName);
            lastUserRoomRequest.Method = "GET";
            var roomResp = (HttpWebResponse)lastUserRoomRequest.GetResponse();
            room = new StreamReader(roomResp.GetResponseStream()).ReadToEnd();
            Dispatcher.Invoke(new Action (() => RoomInfoTextBlock.Text = "Kod pokoju: " + room));
        }
        private bool getWhoseTurn()
        {
            HttpWebRequest whoseTurn = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/whoseturn.php?room=" + room);
            whoseTurn.Method = "GET";
            var turnResp = (HttpWebResponse)whoseTurn.GetResponse();
            var turn = new StreamReader(turnResp.GetResponseStream()).ReadToEnd();

            if (AppStart.UserName == turn)
            {
                Dispatcher.Invoke(new Action(() => TurnInfo.Text = "YOUR TURN"));
                return true;
            }
            else
                return false;
        }
        private void checkWin()
        {
            if (board[0] == board[1] && board[1] == board[2])
            {
                if (board[0] == cellUserValue)
                {
                    endScreen("YOU WIN");
                    playerWinner = 1;
                    clearBoard();
                }
                else if (board[0] != cellUserValue && board[0] != 0)
                {
                    endScreen("YOU LOSE");
                    playerWinner = 2;
                    clearBoard();
                }
            }
            else if (board[0] == board[3] && board[3] == board[6])
            {
                if (board[0] == cellUserValue)
                {
                    endScreen("YOU WIN");
                    playerWinner = 1;
                    clearBoard();
                }
                else if (board[0] != cellUserValue && board[0] != 0)
                {
                    endScreen("YOU LOSE");
                    playerWinner = 2;
                    clearBoard();
                }
            }
            else if (board[0] == board[4] && board[4] == board[8])
            {
                if (board[0] == cellUserValue)
                {
                    endScreen("YOU WIN");
                    playerWinner = 1;
                    clearBoard();
                }
                else if (board[0] != cellUserValue && board[0] != 0)
                {
                    endScreen("YOU LOSE");
                    playerWinner = 2;
                    clearBoard();
                }
            }
            // [2]
            else if (board[2] == board[5] && board[5] == board[8])
            {
                if (board[2] == cellUserValue)
                {
                    endScreen("YOU WIN");
                    playerWinner = 1;
                    clearBoard();
                }
                else if (board[2] != cellUserValue && board[2] != 0)
                {
                    endScreen("YOU LOSE");
                    playerWinner = 2;
                    clearBoard();
                }
            }
            else if (board[2] == board[4] && board[4] == board[6])
            {
                if (board[2] == cellUserValue)
                {
                    endScreen("YOU WIN");
                    playerWinner = 1;
                    clearBoard();
                }
                else if (board[2] != cellUserValue && board[2] != 0)
                {
                    endScreen("YOU LOSE");
                    playerWinner = 2;
                    clearBoard();
                }
            }
            // [4]
            else if (board[4] == board[3] && board[3] == board[5])
            {
                if (board[4] == cellUserValue)
                {
                    endScreen("YOU WIN");
                    playerWinner = 1;
                    clearBoard();
                }
                else if (board[4] != cellUserValue && board[4] != 0)
                {
                    endScreen("YOU LOSE");
                    playerWinner = 2;
                    clearBoard();
                }
            }
            else if (board[4] == board[1] && board[1] == board[7])
            {
                if (board[4] == cellUserValue)
                {
                    endScreen("YOU WIN");
                    playerWinner = 1;
                    clearBoard();
                }
                else if (board[4] != cellUserValue && board[4] != 0)
                {
                    endScreen("YOU LOSE");
                    playerWinner = 2;
                    clearBoard();
                }
            }
            // [6]
            else if (board[6] == board[7] && board[7] == board[8])
            {
                if (board[6] == cellUserValue)
                {
                    endScreen("YOU WIN");
                    playerWinner = 1;
                    clearBoard();
                }
                else if (board[6] != cellUserValue && board[6] != 0)
                {
                    endScreen("YOU LOSE");
                    playerWinner = 2;
                    clearBoard();
                }
            }
            else if (board.All(e => e != 0))
            {
                endScreen("NOBODY WON");
                clearBoard();
            }
            addWin();
        }
        private void clearBoard()
        {
            clearBoardTimer = new();
            clearBoardTimer.Interval = 1000;
            clearBoardTimer.Elapsed += ClearBoardTimer_Elapsed;
            clearBoardTimer.Start();
        }

        private void ClearBoardTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            board.Clear();
            HttpWebRequest clearBoardRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/clearboard.php?room=" + room);
            clearBoardRequest.Method = "POST";
            var clearBoardResp = (HttpWebResponse)clearBoardRequest.GetResponse();
            clearBoardTimer.Stop();
        }

        private void endScreen(string text)
        {
            checkWinTimer.Stop();

            Dispatcher.Invoke(new Action(() => MainWindow.MainFrame.NavigationService.Navigate(new AppEnd(text))));
        }
        private void addWin()
        {
            if (playerWinner != 0 && cellUserValue == 1)
            {
                HttpWebRequest addWinRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/addwin.php?room=" + room + "&player=" + playerWinner);
                addWinRequest.Method = "UPDATE";
                var addWinResp = (HttpWebResponse)addWinRequest.GetResponse();
            }
        }
        private string getPlayerName(int player)
        {
            HttpWebRequest getOpponentNameRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/getplayername.php?room=" + room + "&player=" + player);
            getOpponentNameRequest.Method = "GET";
            var getOpponentResp = (HttpWebResponse)getOpponentNameRequest.GetResponse();
            return new StreamReader(getOpponentResp.GetResponseStream()).ReadToEnd();
        }
        private string getResult(string room)
        {
            HttpWebRequest getResultRequest = (HttpWebRequest)WebRequest.Create("https://wiktorstabach.pl/TICTACTOE/getresult.php?room=" + room);
            getResultRequest.Method = "GET";
            var getResultResp = (HttpWebResponse)getResultRequest.GetResponse();
            return new StreamReader(getResultResp.GetResponseStream()).ReadToEnd();
        }
        
    }
}
