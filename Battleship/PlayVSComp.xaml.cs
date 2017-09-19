using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.SqlClient;

namespace Battleship
{

    public partial class PlayVSComp : UserControl
    {
        public event EventHandler replay;

        public string playerName;
        public Grid[] playerGrid;
        public Grid[] compGrid;
        public List<int> hitList;
        int turnCount = 0;
        public Random random = new Random();
        public bool gameActivity;

        int pCarrierCount = 5, cCarrierCount = 5;
        int pBattleshipCount = 4, cBattleshipCount = 4;
        int pSubmarineCount = 3, cSubmarineCount = 3;
        int pCruiserCount = 3, cCruiserCount = 3;
        int pDestroyerCount = 2, cDestroyerCount = 2;

        string conn = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=BattleShip2;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public PlayVSComp(Grid[] playerGrid, string playerName)
        {
            InitializeComponent();
            this.playerName = playerName;
            gameActivity = true;
            initiateSetup(playerGrid);
            hitList = new List<int>();
        }

        private void initiateSetup(Grid[] userGrid)
        {
            compGrid = new Grid[100];
            CompGrid.Children.CopyTo(compGrid, 0);
            for (int i = 0; i < 100; i++)
            {
                compGrid[i].Tag = "water";
            }
            setupCompGrid();
            playerGrid = new Grid[100];
            PlayerGrid.Children.CopyTo(playerGrid, 0);

            for (int i = 0; i < 100; i++)
            {
                playerGrid[i].Background = userGrid[i].Background;
                playerGrid[i].Tag = userGrid[i].Tag;
            }
        }

        private void setupCompGrid()
        {
            Random random = new Random();
            int[] shipSizes = new int[] { 2, 3, 3, 4, 5 };
            string[] ships = new string[] { "destroyer", "cruiser", "submarine", "battleship", "carrier" };
            int size, index;
            string ship;
            Orientation orientation;
            bool unavailableIndex = true;

            for (int i = 0; i < shipSizes.Length; i++)
            {
                size = shipSizes[i];
                ship = ships[i];
                unavailableIndex = true;

                if (random.Next(0, 2) == 0)
                    orientation = Orientation.Horizontal;
                else
                    orientation = Orientation.Vertical;

                if (orientation.Equals(Orientation.Horizontal))
                {
                    index = random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while ((index + size - 1) % 10 < size - 1)
                        {
                            index = random.Next(0, 100);
                        }

                        for (int j = 0; j < size; j++)
                        {
                            if (index + j > 99 || !compGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < size; j++)
                    {
                        compGrid[index + j].Tag = ship;
                    }
                }
                else
                {
                    index = random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while (index / 10 + size * 10 > 100)
                        {
                            index = random.Next(0, 100);
                        }

                        for (int j = 0; j < size * 10; j += 10)
                        {
                            if (index + j > 99 || !compGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < size * 10; j += 10)
                    {
                        compGrid[index + j].Tag = ship;
                    }
                }

            }
        }

        private void gridMouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid square = (Grid)sender;

            if (turnCount % 2 != 0)
            {
                return;
            }

            switch (square.Tag.ToString())
            {
                case "water":
                    square.Tag = "miss";
                    square.Background = new SolidColorBrush(Colors.LightGray);
                    turnCount++;
                    compTurn();
                    return;
                case "miss":
                case "hit":
                    Console.WriteLine("User hit a miss/hit");
                    return;
                case "destroyer":
                    cDestroyerCount--;
                    break;
                case "cruiser":
                    cCruiserCount--;
                    break;
                case "submarine":
                    cSubmarineCount--;
                    break;
                case "battleship":
                    cBattleshipCount--;
                    break;
                case "carrier":
                    cCarrierCount--;
                    break;
            }
            square.Tag = "hit";
            square.Background = new SolidColorBrush(Colors.Red);
            turnCount++;
            checkPlayerWin();
            compTurn();
        }

        private void compTurn()
        {
            hunterMode();

            turnCount++;
            checkComputerWin();
        }

        private void checkPlayerWin()
        {
            if (cCarrierCount == 0)
            {
                cCarrierCount = -1;
                MessageBox.Show("You sunk my Aircraft Carrier!");
            }
            if (cCruiserCount == 0)
            {
                cCruiserCount = -1;
                MessageBox.Show("You sunk my Cruiser!");
            }
            if (cDestroyerCount == 0)
            {
                cDestroyerCount = -1;
                MessageBox.Show("You sunk my Destroyer!");
            }
            if (cBattleshipCount == 0)
            {
                cBattleshipCount = -1;
                MessageBox.Show("You sunk my Battleship!");
            }
            if (cSubmarineCount == 0)
            {
                cSubmarineCount = -1;
                MessageBox.Show("You sunk my Submarine!");
            }

            if (cCarrierCount == -1 && cBattleshipCount == -1 && cSubmarineCount == -1 &&
                cCruiserCount == -1 && cDestroyerCount == -1)
            {
                string sqlExp = String.Format("INSERT INTO Player (Name, StatusId) VALUES ('{0}',{1})", this.playerName.ToUpper(), 1);
                if (gameActivity == true)
                    using (SqlConnection connection = new SqlConnection(conn))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExp, connection);
                        command.ExecuteNonQuery();
                    }
                gameActivity = false;
                MessageBox.Show("You win!");
                disableGrids();
            }
        }

        private void checkComputerWin()
        {
            if (pCarrierCount == 0)
            {
                pCarrierCount = -1;
                MessageBox.Show("Your Aircraft Carrier got destroyed!");
            }
            if (pCruiserCount == 0)
            {
                pCruiserCount = -1;
                MessageBox.Show("Your Cruiser got destroyed!");
            }
            if (pDestroyerCount == 0)
            {
                pDestroyerCount = -1;
                MessageBox.Show("Your Destroyer got destroyed!");
            }
            if (pBattleshipCount == 0)
            {
                pBattleshipCount = -1;
                MessageBox.Show("Your Battleship got destroyed!");
            }
            if (pSubmarineCount == 0)
            {
                pSubmarineCount = -1;
                MessageBox.Show("Your Submarine got destroyed!");
            }

            if (pCarrierCount == -1 && pBattleshipCount == -1 && pSubmarineCount == -1 &&
                pCruiserCount == -1 && pDestroyerCount == -1)
            {
                string sqlExp = String.Format("INSERT INTO Player (Name, StatusId) VALUES ('{0}',{1})", this.playerName.ToUpper(), 2);
                if (gameActivity == true)
                    using (SqlConnection connection = new SqlConnection(conn))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExp, connection);
                        command.ExecuteNonQuery();
                    }
                gameActivity = false;
                MessageBox.Show("You lose!");
                disableGrids();
            }
        }

        private void disableGrids()
        {
            foreach (var element in compGrid)
            {
                if (element.Tag.Equals("water"))
                {
                    element.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (element.Tag.Equals("carrier") || element.Tag.Equals("cruiser") ||
                  element.Tag.Equals("destroyer") || element.Tag.Equals("battleship") || element.Tag.Equals("submarine"))
                {
                    element.Background = new SolidColorBrush(Colors.LightGreen);
                }
                element.IsEnabled = false;
            }
            foreach (var element in playerGrid)
            {
                if (element.Tag.Equals("water"))
                {
                    element.Background = new SolidColorBrush(Colors.LightGray);
                }
                element.IsEnabled = false;
            }
        }

        private void btnStartOver_Click(object sender, RoutedEventArgs e)
        {
            string sqlExp = String.Format("INSERT INTO Player (Name, StatusId) VALUES ('{0}',{1})", this.playerName.ToUpper(), 3);
            if (gameActivity == true)
                using(SqlConnection connection = new SqlConnection(conn))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExp, connection);
                    command.ExecuteNonQuery();
                }
            replay(this, e);
        }

        private void hunterMode()
        {
            int position;
            do
            {
                position = random.Next(100);
            } while ((playerGrid[position].Tag.Equals("miss")) || (playerGrid[position].Tag.Equals("hit")));
            simpleMode(position);
        }

        private void simpleMode(int position)
        {
            if (!(playerGrid[position].Tag.Equals("water")))
            {
                switch (playerGrid[position].Tag.ToString())
                {
                    case "destroyer":
                        pDestroyerCount--;
                        break;
                    case "cruiser":
                        pCruiserCount--;
                        break;
                    case "submarine":
                        pSubmarineCount--;
                        break;
                    case "battleship":
                        pBattleshipCount--;
                        break;
                    case "carrier":
                        pCarrierCount--;
                        break;
                }
                playerGrid[position].Tag = "hit";
                playerGrid[position].Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                playerGrid[position].Tag = "miss";
                playerGrid[position].Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        private int binarySearch(string[] players, string value)
        {

            int low = 0;
            int high = players.Length - 1;

            while (high >= low)
            {
                int middle = (low + high) / 2;

                if (players[middle].CompareTo(value) == 0)
                {
                    return middle;
                }
                if (players[middle].CompareTo(value) < 0)
                {
                    low = middle + 1;
                }
                if (players[middle].CompareTo(value) > 0)
                {
                    high = middle - 1;
                }
            }
            return -(low + 1);
        }
    }
}