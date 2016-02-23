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
using System.Windows.Shapes;

namespace Scrabble
{
    /// <summary>
    /// Interaction logic for PlayMode.xaml
    /// </summary>
    public partial class PlayMode : Window
    {

        public GameType type { get; set; }
        public PlayMode()
        {
            InitializeComponent();
        }

        private void goGame()
        {
            this.Close();
        }

        private void clickEasy(object sender, RoutedEventArgs e)
        {
            this.type = new GameType("Easy", 5);
            goGame();
        }

        private void clickIntermediate(object sender, RoutedEventArgs e)
        {
            this.type = new GameType("Indermediate", 8);
            goGame();
        }

        private void clickExpert(object sender, RoutedEventArgs e)
        {
            this.type = new GameType("Expert", -1);
            goGame();
        }

        private void clickLearning(object sender, RoutedEventArgs e)
        {
            this.type = new GameType("Learning", 999);
            goGame();
        }
    }
}
