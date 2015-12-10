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
using System.Text.RegularExpressions;

namespace Scrabble
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int played_count;
        Queue<BoardTile> selectionQueue;
        TilePool tile_sack;
        List<GameTile> game_tray;
        List<BoardTile> current_word_form;
        
        public MainWindow()
        {
            this.current_word_form = new List<BoardTile>();
            this.tile_sack = new TilePool();
            InitializeComponent();
            initStartUp();
        }
        private void initStartUp()
        {

            for (int i = 0; i < 225; i++)
            {
                BoardTile blank = new BoardTile();

                blank.Tag = new GameTile();
                blank.Height = 40;
                blank.Width = 40;
                if (i == 112)
                {
                    blank.Content = "X";
                }
                blank.Name = "GRID_" + (i + 1);
                blank.id = i;
                blank.Click += boardTileListener;
                GameBoard.Children.Add(blank);
            }

            for (int i = 0; i < 8; i++)
            {
                BoardTile placer = new BoardTile();
                placer.Height = 60;
                placer.Width = 60;
                placer.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
                placer.tag = tile_sack.getRandomLetter();
                placer.Content = placer.ToString();

                placer.Click += tileClickListener;
                PlayerTray.Children.Add(placer);
            }
        }

        private void resetBoardTiles()
        {
            foreach (BoardTile b in GameBoard.Children)
            {
                b.BorderBrush = new SolidColorBrush(Colors.Black);
                b.BorderThickness = new Thickness(1);
            }
        }

        private void tileClickListener(object sender, EventArgs e)
        {
            if (selectionQueue == null)
                selectionQueue = new Queue<BoardTile>();

            selectionQueue.Enqueue((BoardTile)sender);
            PlayerTray.Children.Remove((BoardTile)sender);

            if(played_count == 0)
            {
                foreach (BoardTile b in GameBoard.Children)
                {
                    if(b.Content == "X")
                    {
                        b.BorderBrush = new SolidColorBrush(Colors.LimeGreen);
                        b.BorderThickness = new Thickness(3);
                    }
                }
            }
            else
            {
                Stack<int> sequence = new Stack<int>();
                int x =0;
                foreach (BoardTile b in GameBoard.Children)
                {
                    if (b.Content != null)
                    {
                        sequence.Push(x + 16);
                        sequence.Push(x+2);
                        sequence.Push(x);
                        sequence.Push(x - 14);
                    }
                    x++;
                }
                foreach (BoardTile b in GameBoard.Children)
                {
                    String resultString = Regex.Match(b.Name.ToString(), @"\d+").Value;
                    if(Convert.ToInt32(resultString) == sequence.Peek())
                    {
                        b.BorderBrush = new SolidColorBrush(Colors.LimeGreen);
                        b.BorderThickness = new Thickness(3);
                        sequence.Pop();
                    }
                    if(sequence.Count == 0)
                    {
                        break;
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
            try
            {
                String s = ((BoardTile)sender).Name;
                foreach (BoardTile b in GameBoard.Children)
                {
                    if (b.Name.Equals(s))
                    {
                        if (b.occupied == true)
                        {
                            GameQTrack.Content = "Already occupied!";
                        }
                        else
                        {
                            BoardTile let = selectionQueue.Dequeue();
                            let.id = b.id;
                            current_word_form.Add(let);
                            b.Content = let.tag.letter_alpha;
                            b.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
                            played_count++;
                            b.occupied = true;
                            resetBoardTiles();
                        }
                    }
                }
            }
            catch(Exception)
            {
                GameQTrack.Content = "No Tiles Selected";
            }
        }

        private void removeBoardTile()
        {
        }

        private void clickSubmitWord(object sender, RoutedEventArgs e)
        {
            List<BoardTile> SortedList = current_word_form.OrderBy(o => o.id).ToList();

            WordValidation wv = new WordValidation();

            String s ="";
            foreach(BoardTile b in SortedList)
            {
                s += b.tag.letter_alpha;
            }

            bool result = wv.isWord(s);

            if(result == true)
            {
                int score = wv.getScore(SortedList);
            }
            else
            {
                GameQTrack.Content = "Not a valid word!";
                resetGameSelections();
            }
            
            int y=0;
        }

        private void resetGameSelections()
        {
            foreach(BoardTile b in current_word_form)
            {
                if(b.accepted_placement==false)
                {
                    b.Height = 60;
                    b.Width = 60;
                    PlayerTray.Children.Add(b);
                    BoardTile temp = new BoardTile();
                    temp.id = b.id;
                    b.tag = null;
                    played_count--;
                }
                
            }
        }
    }
}
