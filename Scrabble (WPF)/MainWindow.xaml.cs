using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public partial class MainGameWindow : Window
    {
        int played_count;
        Queue<BoardTile> selectionQueue;
        WordBuffer game_logic;
        TilePool tile_sack;
        GameStatistics stats;
        AIEngine AI;
        const int CENTRE_GRID = 112;
        
        public MainGameWindow()
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
                blank.Content = i.ToString();
                blank.id = i;
                blank.Click += boardTileListener;
                blank.MouseEnter += tileDebug;
                blank.FontWeight = FontWeights.Bold;
                GameBoard.Children.Add(blank);
            }
            generateGridMapping();
        }

        private void generateGridMapping()
        {
            int x = 0;
            foreach (BoardTile t in GameBoard.Children)
            {
                try
                {
                    t.right = (BoardTile)GameBoard.Children[x + 1];
                }
                catch (ArgumentOutOfRangeException)
                {
                        t.Content = "SE";
                        t.down = null;
                        t.right = null;
                        setBonusTripleWordScore(t);
                }
                try
                {
                    t.left = (BoardTile)GameBoard.Children[x - 1];
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                try
                {
                    t.up = (BoardTile)GameBoard.Children[x - 15];
                }
                catch(ArgumentOutOfRangeException)
                {
                    t.Content = "N";
                    t.up = null;
                }
                try
                {
                    t.down = (BoardTile)GameBoard.Children[x + 15];
                }
                catch (ArgumentOutOfRangeException)
                {
                    if (t.id != 224)
                    {
                        t.Content = "S";
                        t.up = null;
                    }
                }

                //North west boundary mapping
                if (t.id == 0)
                {
                    t.Content = "NW";
                    t.left = null;
                    t.up = null;
                    setBonusTripleWordScore(t);
                }

                //North East Boundary Mapping
                if (t.id < 15 && t.id != 0)
                {
                    t.Content = "N";
                    t.up = null;
                    if (t.id == 14)
                    {
                        t.Content = "NE";
                        t.up = null;
                        t.right = null;
                        setBonusTripleWordScore(t);
                    }
                }

                //Western boundary mappings
                if (t.id % 15 == 0 && t.down != null && t.id != 0)
                {
                    t.Content = "W";
                    t.left = null;
                }

                else if(t.id % 15 == 0 && t.down == null)
                {
                    t.Content = "SW";
                    setBonusTripleWordScore(t);
                }

                //Eastern boundary mappings
                if (t.id % 15 == 14 && t.id != 0)
                {
                    if (t.id == 14)
                    {
                        t.Content = "NE";
                        t.up = null;
                        t.right = null;
                        setBonusTripleWordScore(t);
                        
                    }
                    else if(t.id != 224)
                    {
                        t.Content = "E";
                        t.right = null;
                    }
                }

                x++;

                if(t.left != null && t.up != null && t.down != null & t.right != null)
                {
                    if(t.id % 16 == 0)
                        setBonusDoubleWordScore(t);
                    else if (t.id % 14 == 0)
                        setBonusDoubleWordScore(t);
                }
            }
        }

        private void setBonusTripleWordScore(BoardTile t)
        {
            t.Background = new SolidColorBrush(Colors.Red);
            t.Content = "3x WS";
            t.bonus_multiplier = new KeyValuePair<char, int>('W', 3);
        }
        private void setBonusDoubleWordScore(BoardTile t)
        {
            if(t.id == 112)
            {
                t.Content = "X";
                t.FontWeight = FontWeights.UltraBold;
                t.Background = new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                t.Background = new SolidColorBrush(Colors.Pink);
                t.Content = "2x WS";
                t.bonus_multiplier = new KeyValuePair<char, int>('W', 2);
            }
        }
        private void tileDebug(object sender, MouseEventArgs e)
        {
            BoardTile b = (BoardTile)(sender);
            String stringout = "";

            //stringout += b.left.ToString() + "\r\n";
            //stringout += b.right.ToString() + "\r\n";
            //stringout += b.up.ToString() + "\r\n";
            //stringout += b.down.ToString() + "\r\n";
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
                BoardTile placer = drawTile(tile_sack.getRandomLetter());
                placer.Click += tileClickListener;
                PlayerTray.Children.Add(placer);
            }
        }

        private BoardTile drawTile(Letter Tag)
        {
            BoardTile placer = new BoardTile();
            placer.Margin = new Thickness(2, 0, 0, 0);
            placer.Height = 60;
            placer.Width = 60;
            placer.tag = Tag;
            placer.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative))};
            placer.Content = placer.ToString();
            return placer;
        }

        private void initStartUp()
        {
            drawBegin();
            drawTray();
            beginAISequence();
        }

        private void clearPlacementSelectionBorder()
        {
            foreach(BoardTile t in GameBoard.Children)
            {
                t.BorderBrush = new SolidColorBrush(Colors.Gray);
                t.BorderThickness = new Thickness(1);
            }
        }

        private void tileClickListener(object sender, EventArgs e)
        {
            int sequence_up = -15;
            int sequence_down = 15;
            int sequence_left = -1;
            int sequence_right = 1;

            if (selectionQueue == null)
                selectionQueue = new Queue<BoardTile>();

            BoardTile placeholder = (BoardTile)sender;

            selectionQueue.Enqueue((BoardTile)sender);
            SelectedSequence.Content += "" + placeholder.tag.letter_alpha + " ";
            PlayerTray.Children.Remove((BoardTile)sender);

            int counter = selectionQueue.Count;

            List<int> sequence = new List<int>();

            #region Only applicable for turn 1
            if (played_count == 0)
            {
                if (counter == 1)
                {
                    sequence.Add(CENTRE_GRID);
                }
                else
                {
                    for (int i = 1; i < counter; i++)
                    {
                        sequence.Add(CENTRE_GRID + (sequence_up * i));
                        sequence.Add(CENTRE_GRID + (sequence_down * i));
                        sequence.Add(CENTRE_GRID + (sequence_right * i));
                        sequence.Add(CENTRE_GRID + (sequence_left * i));
                    }
                }
                sequence = sequence.OrderBy(o => o).ToList();

                foreach (int t in sequence)
                {
                    if (t > -1 && t < 225)
                    {
                        BoardTile holder = (BoardTile)GameBoard.Children[t];
                        holder.BorderBrush = new SolidColorBrush(Colors.LimeGreen);
                        holder.BorderThickness = new Thickness(3);
                        holder.placement_possible = true;
                    }
                }
            } 
            #endregion
            else if(played_count > 0)
            {
                List<int> placed_tile_id = new List<int>();
                foreach(BoardTile t in GameBoard.Children)
                {
                    if(t.accepted_placement == true)
                    {
                        placed_tile_id.Add(t.id);
                    }
                }
                foreach(int x in placed_tile_id)
                {
                    for (int i = 1; i < counter + 1; i++)
                    {
                        sequence.Add(x + (sequence_up * counter));
                        sequence.Add(x + (sequence_down * counter));
                        sequence.Add(x + (sequence_right * counter));
                        sequence.Add(x + (sequence_left * counter));
                    }
                    sequence = sequence.OrderBy(o => o).ToList();

                    foreach (int t in sequence)
                    {
                        if (t > 0 && t < 224)
                        {
                            BoardTile holder = (BoardTile)GameBoard.Children[t];
                            holder.BorderBrush = new SolidColorBrush(Colors.LimeGreen);
                            holder.BorderThickness = new Thickness(3);
                            holder.placement_possible = true;
                        }
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
                    SelectedSequence[(SelectedSequence.ToString().Length) - 1] = "";
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

        private void removeUnsetBoardTiles()
        {
           
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
                    if (t.accepted_placement == false && t.occupied==true)
                    {
                        this.generateTrayFromBoard(t);
                        this.resetBoardTile(t);
                        game_logic.selection.Clear();
                    }
                }
            }
            catch (Exception)
            {
            }

            this.flushSelectionQueue();
        }

        private void flushSelectionQueue()
        {
            BoardTile[] storage = new BoardTile[selectionQueue.Count];
            selectionQueue.CopyTo(storage, 0);

            foreach (BoardTile item in storage)
            {
                PlayerTray.Children.Add(selectionQueue.Dequeue());
            }

            SelectedSequence.Content = "";
        }
        private void resetBoardTile(BoardTile board_tile)
        {
            BoardTile blank = new BoardTile();

            blank.Tag = new BoardTile();
            blank.Height = 40;
            blank.Width = 40;
            blank.BorderBrush = new SolidColorBrush(Colors.Gray);

            if (board_tile.id == 112)
            {
                blank.Content = "X";
            }
            blank.Name = board_tile.Name;
            blank.id = board_tile.id;
            blank.Click += boardTileListener;
            blank.bonus_multiplier = board_tile.bonus_multiplier;
            blank.placement_possible = true;
            blank.Content = board_tile.id.ToString();

            GameBoard.Children.RemoveAt(board_tile.id);
            GameBoard.Children.Insert(board_tile.id, blank);

            foreach (BoardTile t in GameBoard.Children)
            {
                if(t.placement_possible == true && t.accepted_placement == false)
                {
                    t.BorderBrush = null;
                }
            }
        }
        private void generateTrayFromBoard(BoardTile board_tile)
        {
            BoardTile placer = new BoardTile();
            placer.Height = 60;
            placer.Width = 60;
            placer.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
            placer.tag = board_tile.tag;
            placer.Content = board_tile.ToString();

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
                            MessageBox.Show("Already occupied", "Alert", MessageBoxButton.OK);
                        }
                        else if(b.placement_possible == false)
                        {
                            MessageBox.Show("Placement is out of bounds", "Alert", MessageBoxButton.OK);
                        }
                        else
                        {
                            BoardTile let = selectionQueue.Dequeue();
                           
                            Sequence.Content = SelectedSequence.Content.ToString().Substring(0, SelectedSequence.Content.ToString().Length - 2);
                            let.id = b.id;
                            this.game_logic.addToSequence(let);
                            b.Content = let.tag.letter_alpha;
                            b.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
                            let.bonus_multiplier = b.bonus_multiplier;
                            b.FontWeight = FontWeights.UltraBold;
                            played_count++;
                            b.occupied = true;
                            b.tag = let.tag;
                            clearPlacementSelectionBorder();
                        }
                    }
                }
            }
            catch(Exception)
            {
                GameQTrack.Content = "No Tiles Selected";
            }
        }

        private void clickSubmitWord(object sender, RoutedEventArgs e)
        {
            foreach(BoardTile t in GameBoard.Children)
            {
                int x = 0;
                if (t.occupied == true && t.accepted_placement == true && x < selectionQueue.Count)
                {
                    
                    int id = t.id;

                    if (this.hasAdjacent(id) != 0)
                    {
                        BoardTile placeholder = new BoardTile();
                        BoardTile game_piece = (BoardTile)GameBoard.Children[t.id + this.hasAdjacent(t.id)];
                        placeholder.id = game_piece.id;
                        placeholder.tag = game_piece.tag;

                        try
                        {
                            this.game_logic.addToSequence(placeholder);
                             
                        }
                        catch (Exception)
                        {

                        }
                        break;
                    }
                    else
                    {
                        break;
                    }
                    x++;
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
                clearPlacementSelectionBorder();
                AIPerformTurn();
            }
            
        }
        private int hasAdjacent(int location)
        {
            //If there exists a tile below this tile
            //It is included in the current input String
            BoardTile placeholderS = (BoardTile)GameBoard.Children[location + 15];
            BoardTile placeholderN = (BoardTile)GameBoard.Children[location - 15];
            BoardTile placeholderE = (BoardTile)GameBoard.Children[location + 1];
            BoardTile placeholderW = (BoardTile)GameBoard.Children[location - 1];

            if (placeholderS.accepted_placement == true)
            {
                return 15;
            }
            else if (placeholderN.accepted_placement == true)
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
        private bool hasAdjacentEast(BoardTile t)
        {
            //If there exists a tile below this tile
            //It is included in the current input String

            if (t.right.tag != null)
            {
                return true;
            }
            return false;
        }
        private bool hasAdjacentWest(BoardTile t)
        {
            //If there exists a tile below this tile
            //It is included in the current input String

            if (t.left.tag != null)
            {
                return true;
            }
            return false;
        }
        private bool hasAdjacentNorth(BoardTile t)
        {
            //If there exists a tile below this tile
            //It is included in the current input String

            if (t.up.tag != null)
            {
                return true;
            }
            return false;
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
            AI.fillGameTree(game_logic.getAllWords());
        }

        private void AIPerformTurn()
        {
            
            game_logic.clearBuffer();
            int counter = AI.current_list.Count;
            AI.current_list.Clear();
            
            for (int i = 0; i < 8 - counter; i++)
            {
                BoardTile fresh_tile = new BoardTile();
                fresh_tile.tag = tile_sack.getRandomLetter();
                AI.current_list.Add(fresh_tile);
                AIStatusReadout.Text += fresh_tile.tag.ToString() + "  ";
            }


            List<BoardTile> joint_structure = new List<BoardTile>();

            List<BoardTile> candidate_entry_points = new List<BoardTile>();
            foreach (BoardTile bt in GameBoard.Children)
            {
                if (bt.accepted_placement == true)
                {
                    candidate_entry_points.Add(bt);
                    AI.current_list.Add(bt);
                }
            }

            AI.retrieveSuperSet();
            int y = 0;
            List<int> placed_char_index = new List<int>();
            List<String> ai_placement_buffer = new List<String>();
            Dictionary<KeyValuePair<int, int>, List<BoardTile>> play_candidates = new Dictionary<KeyValuePair<int, int>, List<BoardTile>>();
            int index = 0;
            int root = 0; //Will need later for loc recall

            foreach(BoardTile bt in candidate_entry_points)
            {
                //Consider word placement from here
                //Calculate score, determine strongest
                //Option to place, verify and place.
                foreach (String s in AI.possible_words)
                { 
                    if (s.Contains(bt.tag.letter_alpha))
                    {
                        root = s.IndexOf(bt.tag.letter_alpha);
                        //Mark buffer as possible word placement
                        game_logic.selection = game_logic.getTilesFromString(s, bt, root-1, candidate_entry_points);
                        game_logic.calculateScore();
                        game_logic.root_location = root;
                        KeyValuePair<int, int> fill = new KeyValuePair<int, int>(index, game_logic.score);
                        play_candidates.Add(fill, game_logic.selection);
                        index++;
                    }
                }
            }

            KeyValuePair<int, int> dict_index_score = new KeyValuePair<int, int>();
            foreach (KeyValuePair<int, int> kvp in play_candidates.Keys)
            {
                if (kvp.Value > dict_index_score.Value)
                {
                    dict_index_score = new KeyValuePair<int, int>(kvp.Key, kvp.Value);
                    game_logic.selection = play_candidates[kvp];
                    game_logic.calculateScore();
                }
            }

            int n = 0;
            String output_word = "";
            foreach(List<BoardTile> item in play_candidates.Values)
            { 
                if (n == dict_index_score.Key)
                {
                    int marker = 0;
                    //Attempt to play word selection
                    foreach (BoardTile t in item)
                    {
                        output_word += t.tag.letter_alpha;
                        List<BoardTile> outer_storage = new List<BoardTile>();
                        outer_storage.AddRange(item);
                        if (t.accepted_placement == true)
                        {
                            if (t.up.accepted_placement == false && t.down.accepted_placement == false)
                            {
                                int new_loc = t.id;
                                for (int i = marker; i >= 0; i--)
                                {
                                    try
                                    {
                                        mapAITileToBoard(outer_storage[i], new_loc);                                    
                                    }
                                    catch (Exception e)
                                    {
                                    }
                                    new_loc = new_loc - 15;
                                }
                                new_loc = t.id;
                                for (int i = marker; i < item.Count; i++)
                                {
                                    try
                                    {
                                        mapAITileToBoard(outer_storage[i], new_loc);
                                    }
                                    catch (Exception e)
                                    {
                                    }
                                    new_loc = new_loc + 15;
                                }
                            }
                            else if (t.right.accepted_placement == false && t.left.accepted_placement == false)
                            {
                                int new_loc = t.id;
                                for (int i = marker; i >= 0; i--)
                                {
                                    try
                                    {
                                        mapAITileToBoard(outer_storage[i], new_loc);
                                    }
                                    catch (Exception e)
                                    {
                                    }
                                    new_loc = new_loc - 1;
                                }

                                new_loc = t.id;
                                for (int i = marker; i < item.Count; i++)
                                {
                                    try
                                    {
                                        mapAITileToBoard(outer_storage[i], new_loc);
                                    }
                                    catch (Exception e)
                                    {
                                    }
                                    new_loc = new_loc + 1;
                                }
                            }
                        }
                        marker++;
                    }
                    this.AIPlayedWords.Items.Add(output_word + " - " + game_logic.score);
                }
                n++;
            }

            foreach(BoardTile t in GameBoard.Children)
            {
                if (t.accepted_placement == false)
                {
                    
                }
                else
                {
                    t.placement_possible = true;
                }
            }

            int l = 0;

            game_logic.clearBuffer();
        }

        private void mapAITileToBoard(BoardTile t, int loc)
        {
            try
            {
                BoardTile placer = drawTile(t.tag);
                placer.id = loc;
                placer.Content = t.Content;
                foreach(BoardTile inner_set in GameBoard.Children)
                {
                    if (inner_set.id == loc)
                    {
                        inner_set.tag = placer.tag;
                        inner_set.Content = inner_set.tag.letter_alpha;
                        inner_set.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative))};
                        inner_set.FontWeight = FontWeights.UltraBold;
                        inner_set.accepted_placement = true;
                        inner_set.placement_possible = false;
                    }
                }                       
            }
            catch (Exception e)
            {
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
