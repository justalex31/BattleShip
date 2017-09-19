using System;
using System.Windows;
using System.Windows.Controls;

namespace Battleship
{

    public partial class MainWindow : Window
    {
        Grid grid = new Grid();

        private Setup setup;
        private ShipPlacement shipPlacement;
        private PlayVSComp playVSComp;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }
      
        private void InitializeGame()
        {
            Content = grid;

            this.MinHeight = 300;
            this.MinWidth = 330;
            this.Height = 300;
            this.Width = 330;

            setup = new Setup();
            grid.Children.Add(setup);

            setup.play += new EventHandler(shipSetup);
        }

        private void shipSetup(object sender, EventArgs e)
        {
            grid.Children.Clear();

            this.MinWidth = 460;
            this.MinHeight = 530;
            this.Width = 460;
            this.Height = 530;

            shipPlacement = new ShipPlacement();

            grid.Children.Add(shipPlacement);

            shipPlacement.play += new EventHandler(playGame);
        }

        private void playGame(object sender, EventArgs e)
        {
            grid.Children.Clear();

            this.MinWidth = 953.286;
            this.MinHeight = 480;
            this.Width = 953.286;
            this.Height = 480;

            playVSComp = new PlayVSComp(shipPlacement.playerGrid, setup.name);

            grid.Children.Add(playVSComp);
            playVSComp.replay += new EventHandler(replayGame);

        }
        
        private void replayGame(object sender, EventArgs e)
        {
            grid.Children.Clear();
            InitializeGame();
        }     
    }
}