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
        WordBuffer game_logic;
        TilePool tile_sack;
        GameStatistics stats;
        AIEngine AI;
        
        public MainWindow()
        {
            this.AI = new AIEngine();
            this.stats = new GameStatistics();
            this.game_logic = new WordBuffer();
            this.tile_sack = new TilePool();
            InitializeComponent();
            initStartUp();
        }
        private void drawBegin()
        {
            for (int i = 0; i < 225; i++)
            {
                BoardTile blank = new BoardTile();
                blank.Tag = new BoardTile();
                blank.Height = 40;
                blank.Width = 40;

                if (i == 112)
                {
                    blank.Content = "X";
                }
                blank.Name = "GRID_" + (i + 1);
                blank.id = i;
                blank.Content = i.ToString();
                blank.Click += boardTileListener;
                GameBoard.Children.Add(blank);
            }
        }
        private void drawRefresh()
        {
            for (int i = 0; i < 225; i++)
            {
                BoardTile blank = new BoardTile();

                blank.Tag = new BoardTile();
                blank.Height = 40;
                blank.Width = 40;

                if (i == 112)
                {
                    blank.Content = "X";
                }
                blank.Name = "GRID_" + (i + 1);
                blank.id = i;
                blank.Content = i.ToString();
                blank.Click += boardTileListener;
                GameBoard.Children.Add(blank);
            } 
        }
        private void drawTray()
        {
            int counter = PlayerTray.Children.Count;
            for (int i = 0; i < 8 - counter; i++)
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

        private void initStartUp()
        {

            drawBegin();
            drawTray();
            beginAISequence();
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

            BoardTile placeholder = (BoardTile)sender;

            selectionQueue.Enqueue((BoardTile)sender);
            SelectedSequence.Content += "" + placeholder.tag.letter_alpha + " ";
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
            resetTray();
        }

        private void resetTray()
        {
            try
            {
                int count = selectionQueue.Count;
                BoardTile[] storage = new BoardTile[GameBoard.Children.Count];
                GameBoard.Children.CopyTo(storage, 0);

                foreach (BoardTile t in storage)
                {
                    if (t.accepted_placement == false && t.occupied == true)
                    {
                        this.generateTrayFromBoard(t);
                        this.resetBoardTile(t);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void resetBoardTile(BoardTile board_tile)
        {
            BoardTile blank = new BoardTile();

            blank.Tag = new BoardTile();
            blank.Height = 40;
            blank.Width = 40;

            if (board_tile.id == 112)
            {
                blank.Content = "X";
            }
            blank.Name = board_tile.Name;
            blank.id = board_tile.id;
            blank.Click += boardTileListener;
            GameBoard.Children.RemoveAt(board_tile.id);
            GameBoard.Children.Insert(board_tile.id, blank);
        }
        private void generateTrayFromBoard(BoardTile board_tile)
        {
            BoardTile placer = new BoardTile();
            placer.Height = 60;
            placer.Width = 60;
            placer.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
            placer.tag = board_tile.tag;
            placer.Content = placer.ToString();

            placer.Click += tileClickListener;
            PlayerTray.Children.Add(placer);
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
                            SelectedSequence.Content = SelectedSequence.Content.ToString().Substring(0, SelectedSequence.Content.ToString().Length - 2);
                            let.id = b.id;
                            this.game_logic.addToSequence(let);
                            b.Content = let.tag.letter_alpha;
                            b.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
                            played_count++;
                            b.occupied = true;
                            b.tag = let.tag;
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
            foreach(BoardTile t in GameBoard.Children)
            {
                if (t.occupied == true && t.accepted_placement == false)
                {
                    int x =0;
                    int id = t.id;
                    while (x < 99)
                    {
                        if (this.hasAdjacentSouth(id) != 0)
                        {
                                BoardTile placeholder = new BoardTile();
                                BoardTile game_piece = (BoardTile)GameBoard.Children[t.id + this.hasAdjacentSouth(t.id)];
                                placeholder.id = game_piece.id;
                                placeholder.tag = game_piece.tag;

                                try
                                {
                                    this.game_logic.addToSequence(placeholder);
                                    break;
                                }
                                catch (Exception)
                                {

                                }
                        }
                        else
                            break;
                    }

                    for (int i = 0; i < x; i++)
                    {
                        
                    }
                }
            }
            this.game_logic.sortInputList();
            
            if (game_logic.checkCurrentEntry() == false)
            {
                GameQTrack.Content = "Not a valid word!";
                resetTray();
            }
            else
            {
                foreach(BoardTile t in GameBoard.Children)
                {
                    if(t.occupied)
                    {
                        t.accepted_placement = true;
                    }
                }
                game_logic.calculateScore();
                this.stats.total_score_player += game_logic.score;
                this.stats.total_moves++;
                this.PlayedWords.Items.Add(game_logic.current + " - " + game_logic.score);
                drawTray();
                this.game_logic.clearBuffer();
                this.ScoreLabel.Content = this.stats.total_score_player;
                this.beginAISequence();
            }
        }
        private int hasAdjacentSouth(int location)
        {
            //If there exists a tile below this tile
            //It is included in the current input String
            BoardTile placeholderS = (BoardTile)GameBoard.Children[location + 15];
            BoardTile placeholderN = (BoardTile)GameBoard.Children[location - 15];
            BoardTile placeholderE = (BoardTile)GameBoard.Children[location + 1];
            BoardTile placeholderW = (BoardTile)GameBoard.Children[location -1];

            if (placeholderS.accepted_placement == true)
            {
                return 15;
            }
            else if(placeholderN.accepted_placement == true)
            {
                return -15;
            }
            else if (placeholderE.accepted_placement == true)
            {
                return 1;
            }
            else if (placeholderW.accepted_placement == true)
            {
                return -1;
            }
            return 0;
        }

        private void resetGameSelections()
        {
            foreach(BoardTile b in GameBoard.Children)
            {
                if(b.accepted_placement==false)
                {
                    BoardTile temp = b;
                    temp.Height = 60;
                    temp.Width = 60;

                    //PlayerTray.Children.Add(b);

                    b.Background = null;
                    b.Height = 40;
                    b.Width = 40;
                    this.played_count--;
                }
                
            }
        }

        private void beginAISequence()
        {
            AIStatusReadout.Text = "Current AI Word Distribution: \r\n";
            int counter = AI.current_list.Count;
            for(int i =0; i < 8 - counter; i++)
            {
                BoardTile fresh_tile = new BoardTile();
                fresh_tile.tag = tile_sack.getRandomLetter();
                AI.current_list.Add(fresh_tile);
                AIStatusReadout.Text += fresh_tile.tag.ToString() + "  ";
            }
        }

        private void clickRequestNewTray(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you wish to request new tiles? This will surrender the current turn.", "Info", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (BoardTile t in PlayerTray.Children)
                {
                    tile_sack.callLetterIncrement(t);
                }
                PlayerTray.Children.Clear();
                drawTray();
            }
            else
            {
                //Do nothing
            }
        }
    }
}
