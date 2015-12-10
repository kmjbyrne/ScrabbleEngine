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

namespace Scrabble
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int played_count;
        Queue<Button> selectionQueue;
        TilePool tile_sack;
        List<GameTile> game_tray;
        List<GameTile> current_word_form;

        public MainWindow()
        {
            this.current_word_form = new List<GameTile>();
            this.tile_sack = new TilePool();
            InitializeComponent();
            initStartUp();
 
        }
        private void initStartUp()
        {

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Button blank = new Button();

                    blank.Tag = new BoardTile();
                    blank.Height = 40;
                    blank.Width = 40;
                    if (i == 8 && j == 8)
                    {
                        blank.Content = "X";
                    }
                    blank.Name = "GRID_" + i + "_" + j;
                    blank.Click += boardTileListener;
                    GameBoard.Children.Add(blank);
                }
            }

            for (int i = 0; i < 8; i++)
            {
                GameTile tile = new GameTile();
                tile.tile_chip.Height = 60;
                tile.tile_chip.Width = 60;
                tile.tile_chip.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
                tile.setTileState(tile_sack.getRandomLetter());

                Button placer = new Button();
                placer = tile.tile_chip;
                placer.Tag = tile;

                tile.tile_chip.Click += tileClickListener;
                PlayerTray.Children.Add(placer);
            }
        }

        private void tileClickListener(object sender, EventArgs e)
        {
            if (selectionQueue == null) 
                selectionQueue = new Queue<Button>();

            selectionQueue.Enqueue((Button)sender);
            PlayerTray.Children.Remove((Button)sender);

            if(played_count == 0)
            {
                foreach(Button b in GameBoard.Children)
                {
                    if(b.Name == "GRID_8_8")
                    {
                        b.BorderBrush = new SolidColorBrush(Colors.LimeGreen);
                        b.BorderThickness = new Thickness(3);
                    }
                }
            }
        }

        private void returnCharactersToTray(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    PlayerTray.Children.Add(selectionQueue.Dequeue());
                }
            }
            catch (Exception)
            {
            }
        }

        private void clickResetTray(object sender, RoutedEventArgs e)
        {
            int count = selectionQueue.Count;
            for (int i = 0; i < count; i++)
                PlayerTray.Children.Add(selectionQueue.Dequeue());
        }

        private void releaseTileAction(object sender, MouseButtonEventArgs e)
        {
            int x = 0;
            int y = 0;

            try
            { 
                if(selectionQueue.Count == null || selectionQueue.Count == 0){}   
            }
            catch(Exception)
            {
                        GameQTrack.Content = "Select a tile before placing a tile";
            }
        }
        private void boardTileListener(object sender, EventArgs e)
        {
            String s = ((Button)sender).Name;
            Button let = (Button)selectionQueue.Dequeue();
            GameTile inner_object = (GameTile)let.Tag;

            GameTile tile = new GameTile();
            tile.tile_chip.Height = 40;
            tile.tile_chip.Width = 40;
            tile.tile_chip.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
            tile.setTileState(inner_object.tag);

            int x = 0;
            foreach (Button b in GameBoard.Children)
            {
                x++;
                if (b.Name.Equals(s))
                {
                    current_word_form.Add(tile);
                    b.Content = tile.tag.letter_alpha;
                    b.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
                }
            }
        }
    }
}
