using System;
using System.Windows;
using System.Windows.Controls;

namespace Battleship
{

    public partial class Setup : UserControl
    {
        public event EventHandler play;
        public string name;

        public Setup()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            name = txtboxName.Text;
            if (name == "")
            {
                MessageBox.Show("You must enter a name", "Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
            else
            {
                play(this,e);
            }
        }
    }
}
