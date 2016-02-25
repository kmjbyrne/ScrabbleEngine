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

        AIStatusForm readout;

        private System.Windows.Threading.DispatcherTimer timer;
        private System.Threading.Thread AIRunThread;

        GameKeeper player_tracker;
        GameKeeper cpu_tracker;

        GameType game_type;
        
        public MainGameWindow(GameType type)
        {
            this.game_type = type;
            this.player_tracker = new GameKeeper();
            this.cpu_tracker = new GameKeeper();
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

            int place = 0;

            BoardTile boundary = new BoardTile();
            boundary.id = -1;
            boundary.accepted_placement = false;
            boundary.placement_possible = false;
            boundary.occupied = false;

            foreach (BoardTile t in GameBoard.Children)
            {
                try
                {
                    t.right = (BoardTile)GameBoard.Children[x + 1];
                }
                catch (ArgumentOutOfRangeException)
                {
                        t.Content = place;
                        t.down = boundary;
                        t.right = boundary;
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
                    t.Content = place;
                    t.up = boundary;
                }
                try
                {
                    t.down = (BoardTile)GameBoard.Children[x + 15];
                }
                catch (ArgumentOutOfRangeException)
                {
                    if (t.id != 224)
                    {
                        t.Content = place;
                        t.up = boundary;
                    }
                }

                //North west boundary mapping
                if (t.id == 0)
                {
                    t.Content = place;
                    t.left = boundary;
                    t.up = boundary;
                    setBonusTripleWordScore(t);
                }

                //North East Boundary Mapping
                if (t.id < 15 && t.id != 0)
                {
                    t.Content = place;
                    t.up = boundary;
                    if (t.id == 14)
                    {
                        t.Content = place;
                        t.up = boundary;
                        t.right = boundary;
                        setBonusTripleWordScore(t);
                    }
                }

                //Western boundary mappings
                if (t.id % 15 == 0 && t.down != boundary && t.id != 0)
                {
                    t.Content = place;
                    t.left = boundary;
                }

                else if (t.id % 15 == 0 && t.down == boundary)
                {
                    t.Content = place;
                    setBonusTripleWordScore(t);
                }

                //Eastern boundary mappings
                if (t.id % 15 == 14 && t.id != 0)
                {
                    if (t.id == 14)
                    {
                        t.Content = place;
                        t.up = boundary;
                        t.right = boundary;
                        setBonusTripleWordScore(t);
                        
                    }
                    else if(t.id != 224)
                    {
                        t.Content = place;
                        t.right = boundary;
                    }
                }

                x++;

                if (t.left != boundary && t.up != boundary && t.down != boundary & t.right != boundary)
                {
                    if(t.id % 16 == 0)
                        setBonusDoubleWordScore(t);
                    else if (t.id % 14 == 0)
                        setBonusDoubleWordScore(t);
                }

                place++;
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
            try
            {
                for (int i = 0; i < 8 - counter; i++)
                {
                    BoardTile placer = drawTile(tile_sack.getRandomLetter());
                    placer.Click += tileClickListener;
                    PlayerTray.Children.Add(placer);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("No more tiles remaining in sack!", "Info", MessageBoxButton.OK);
            }
        }
        private void drawAITray()
        {
            int counter = AITray.Children.Count;
            try
            {
                for (int i = 0; i < 8 - counter; i++)
                {
                    BoardTile placer = drawTile(tile_sack.getRandomLetter());
                    placer.Click += tileClickListener;
                    AITray.Children.Add(placer);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("No more tiles remaining in sack!", "Info", MessageBoxButton.OK);
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
                    //SelectedSequence[(SelectedSequence.ToString().Length) - 1] = "";
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
                           
                            SelectedSequence.Content = SelectedSequence.Content.ToString().Substring(0, SelectedSequence.Content.ToString().Length - 2);
                            let.id = b.id;
                            this.game_logic.addToSequence(let);
                            b.Content = let.tag.letter_alpha;
                            b.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
                            let.bonus_multiplier = b.bonus_multiplier;
                            b.FontWeight = FontWeights.UltraBold;
                            played_count++;
                            b.occupied = true;
                            b.accepted_placement = false;
                            b.tag = let.tag;

                            if (hasAdjacent(let.id) == 15)
                            {
                                if (!(this.game_logic.selection.Contains((BoardTile)GameBoard.Children[let.id + 15])))
                                    this.game_logic.addToSequence((BoardTile)GameBoard.Children[let.id + 15]);
                            }
                            if(hasAdjacent(let.id) == -15)
                            {
                                if (!(this.game_logic.selection.Contains((BoardTile)GameBoard.Children[let.id - 15])))
                                    this.game_logic.addToSequence((BoardTile)GameBoard.Children[let.id - 15]);
                            }
                            if (hasAdjacent(let.id) == 1)
                            {
                                if (!(this.game_logic.selection.Contains((BoardTile)GameBoard.Children[let.id + 1])))
                                    this.game_logic.addToSequence((BoardTile)GameBoard.Children[let.id + 1]);
                            }
                            if (hasAdjacent(let.id) == -1)
                            {
                                if (!(this.game_logic.selection.Contains((BoardTile)GameBoard.Children[let.id - 1])))
                                    this.game_logic.addToSequence((BoardTile)GameBoard.Children[let.id - 1]);
                            }

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

        private void pushAdjacentsToSelection()
        {
            foreach(BoardTile tile in GameBoard.Children)
            {
                int id = tile.id;
                if (tile.accepted_placement == true)
                {
                    if (this.hasAdjacent(id) != 0)
                    {
                        BoardTile placeholder = new BoardTile();
                        BoardTile game_piece = (BoardTile)GameBoard.Children[id + this.hasAdjacent(id)];
                        placeholder.id = game_piece.id;
                        placeholder.tag = game_piece.tag;

                        try
                        {
                            this.game_logic.addToSequence(placeholder);

                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                    }
                }
            }
        }
        private void clickSubmitWord(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("User submitted word: " + game_logic.ToString());
            //pushAdjacentsToSelection();
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
                this.player_tracker.updateScore(game_logic.score);
                this.stats.total_moves++;
                this.PlayedWords.Items.Add(game_logic.current + " - " + game_logic.score);
                drawTray();
                this.game_logic.clearBuffer();
                this.player_tracker.round++;
                this.PlayerScore.Content = this.player_tracker.score;
                clearPlacementSelectionBorder();
                AIPerformTurn();
            }
            
        }

        void setTimer()
        {
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(timerTick);
        }
        void timerTick(object sender, EventArgs e)
        {
            this.readout.ReadoutList.ItemsSource = StaticUpdates.words;
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

        private void fillGameTree()
        {
            AI.fillGameTree(game_logic.getAllWords());
        }
        private void beginAISequence()
        {
            game_logic.clearBuffer();
            int counter = AITray.Children.Count;
            AI.current_list.Clear();
            fillGameTree();

            for (int i = 0; i < 8 - counter; i++)
            {
                BoardTile fresh_tile = new BoardTile();
                fresh_tile.tag = tile_sack.getRandomLetter();
                fresh_tile.tray_location = i;
                AI.current_list.Add(drawTile(fresh_tile.tag));
                AITray.Children.Add(drawTile(fresh_tile.tag));
                //AIStatusReadout.Text += fresh_tile.tag.ToString() + "  ";
            }

            this.MainWindowView.Title = "Tiles Remaining [" + this.tile_sack.getRemainingCount() + "]";
        }


        private bool placementPossible(Dictionary<KeyValuePair<int, int>, List<BoardTile>> play_candidates)
        {
            foreach(List<BoardTile> candidate_list in play_candidates.Values)
            {
                int count = candidate_list.Count;
                foreach(BoardTile tile in candidate_list)
                {
                    int id = tile.id;
                    int location_in_word = candidate_list.IndexOf(tile);
                }
            }

            return true;
        }


        private void mapPlacement(List<BoardTile> placement)
        {
            foreach(BoardTile fart in placement)
            {
                mapAITileToBoard(fart, fart.id);
            }
        }

        private KeyValuePair<bool, String> validatePlacement(List<BoardTile> candidate)
        {
            String output_word = "";
            int marker = 0;
            //Attempt to play word selection

            BoardTile boundary = new BoardTile();
            boundary.id = -1;
            boundary.accepted_placement = false;
            boundary.placement_possible = false;
            boundary.occupied = false;

            List<BoardTile> acceptable_roots = new List<BoardTile>();

            foreach(BoardTile possible_root in GameBoard.Children)
            {
                if(possible_root.accepted_placement == true)
                {
                    try
                    {
                        if (possible_root.up.id == -1)
                        {
                            if (possible_root.down.tag == null)
                            {
                                acceptable_roots.Add(possible_root);
                            }
                            else if (possible_root.left == null && possible_root.right == null)
                            {
                                acceptable_roots.Add(possible_root);
                            }
                            else
                            {
                                //Discard root possibility
                            }
                        }
                        else if (possible_root.down.id == -1)
                        {
                            if (possible_root.down.tag == null)
                            {
                                acceptable_roots.Add(possible_root);
                            }
                            else if (possible_root.left == null && possible_root.right == null)
                            {
                                acceptable_roots.Add(possible_root);
                            }
                            else
                            {
                                //Discard root possibility
                            }
                        }
                        else
                        {
                            if (possible_root.up.tag == null && possible_root.down.tag == null)
                            {
                                acceptable_roots.Add(possible_root);
                            }

                            else if (possible_root.left.tag == null && possible_root.right.tag == null)
                            {
                                acceptable_roots.Add(possible_root);
                            }
                            else
                            {
                                //Discard root possibility
                            }
                        }
                    }
                    catch(Exception)
                    {

                    }
                }
                //Take all possible root points
                //Find it's place within the candidate selection
                //Map the tile to the candidate, then check it's
                //Placement validity and return true or false
                //With the String as the Value return for debug
            }

            int place_marker = 0;
            bool exit = false;

            foreach(BoardTile t in candidate)
            {
                output_word += t.tag.letter_alpha;
                foreach (BoardTile root in acceptable_roots)
                {
                    if (root.tag == t.tag)
                    {
                        exit = true;
                        break;
                    }
                }
                if (exit)
                {
                    break;
                }
                else
                    place_marker++;
            }
            

            foreach (BoardTile t in candidate)
            { 
                int marker_id = 0;

                List<BoardTile> outer_storage = new List<BoardTile>();
                outer_storage.AddRange(candidate);
                if (t.accepted_placement == true)
                {
                    int counter = candidate.Count;
                    if (t.up.accepted_placement == false && t.down.accepted_placement == false)
                    {
                        //This states that the north and south paths are open so far
                        int new_loc = t.id;
                        String x = "U";
                        for (int i = place_marker-1; i >= 0; i--)
                        {
                            if (checkMapping(outer_storage[i], new_loc, i, x) == false)
                            {
                                return new KeyValuePair<bool,string>(false, output_word);
                            }
                            else
                            {

                            }
                            x = "U";
                            new_loc = new_loc - 15;
                        }
                        new_loc = t.id;

                        int root_difference = place_marker;
                        x = "ROOT";
                        for (int i = place_marker; i < candidate.Count; i++)
                        {
                            if (checkMapping(outer_storage[i], new_loc, i, x) == false)
                            {
                                return new KeyValuePair<bool, string>(false, output_word);
                            }
                            x = "D";
                            new_loc = new_loc + 15;
                        }
                    }
                    else if (t.right.accepted_placement == false && t.left.accepted_placement == false)
                    {
                        String x = "L";
                        int new_loc = t.id;
                        for (int i = place_marker-1; i >= 0; i--)
                        {
                            if (checkMapping(outer_storage[i], new_loc, i, x) == false)
                            {
                                return new KeyValuePair<bool,string>(false, output_word);
                            }

                            x = "L";
                            new_loc = new_loc - 1;
                        }

                        x = "ROOT";
                        new_loc = t.id;
                        for (int i = place_marker; i < candidate.Count; i++)
                        {
                            int remaining = candidate.Count - i;
                            if (checkMapping(outer_storage[i], new_loc, remaining, x) == false)
                            {
                                return new KeyValuePair<bool,string>(false, output_word);
                            }

                            x = "R";
                            new_loc = new_loc + 1;
                        }
                    }
                }
            }
            //this.AIPlayedWords.Items.Add(output_word + " - " + game_logic.score);

            return new KeyValuePair<bool, string>(true, output_word);
        }


        private void newAITray()
        {
            if (MessageBox.Show("AI Found no combinations from set. Making new tray!", "Info", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (BoardTile t in AITray.Children)
                {
                    tile_sack.callLetterIncrement(t);
                }
                AITray.Children.Clear();
                drawAITray();
            }
            else
            {
                //Do nothing
            }

            beginAISequence();
        }

        private void AIPerformTurn()
        {      
            List<BoardTile> joint_structure = new List<BoardTile>();

            List<BoardTile> candidate_entry_points = new List<BoardTile>();
            foreach (BoardTile bt in GameBoard.Children)
            {
                try
                {
                    if (bt.accepted_placement == true && bt.up.accepted_placement == false && bt.down.accepted_placement == false)
                    {
                        candidate_entry_points.Add(bt);
                        AI.current_list.Add(bt);
                    }
                    else if (bt.accepted_placement == true && bt.left.accepted_placement == false && bt.right.accepted_placement == false)
                    {
                        candidate_entry_points.Add(bt);
                        AI.current_list.Add(bt);
                    }
                }
                catch(Exception e)
                {

                }
            }

            List<int> placed_char_index = new List<int>();
            List<String> ai_placement_buffer = new List<String>();
            Dictionary<KeyValuePair<int, int>, List<BoardTile>> play_candidates = new Dictionary<KeyValuePair<int, int>, List<BoardTile>>();
            int index = 0;
            int root = 0; //Will need later for loc recall

            List<ComparableSelection> list = new List<ComparableSelection>();

            List<BoardTile> reserve = new List<BoardTile>();

            int measure = 0;

            if(game_type.name.Equals("Learning"))
            {
                int average = player_tracker.score / player_tracker.round;
                measure = average;
            }
            else
            {
                measure = game_type.formula;
            }

            int marker = 0;
            foreach(BoardTile bt in candidate_entry_points)
            {
                AI.current_list.Clear();
                foreach (BoardTile tray in AITray.Children)
                {
                    AI.current_list.Add(tray);
                }

                
                //AI.current_list = reserve;
                AI.current_list.Add(bt);
                AI.retrieveSuperSet();

                list.Add(new ComparableSelection(0));
                //Consider word placement from here
                //Calculate score, determine strongest
                //Option to place, verify and place.
                foreach (String s in AI.possible_words)
                {
                    if (s.Contains(bt.tag.letter_alpha))
                    {
                        //root = s.IndexOf(bt.tag.letter_alpha);
                        //Mark buffer as possible word placement
                        //game_logic.selection = game_logic.getTilesFromString(s, bt, root - 1, candidate_entry_points, bt.tray_location);
                        game_logic.selection = game_logic.mapTiles(s, bt);
                        game_logic.calculateScore();
                        //game_logic.root_location = root;

                        if (list[marker].score == 0)
                        {
                            if (validatePlacement(game_logic.selection).Key == true)
                            {
                                //Before adding to play candidates, check placement validity
                                list[marker].current = game_logic.selection;
                                list[marker].score = game_logic.score;
                            }
                            else
                            {
                                list[marker].score = 0;
                            }
                        }

                        else if (list[marker].score < game_logic.score && game_logic.score <= measure)
                        {
                            if (validatePlacement(game_logic.selection).Key == true)
                            {

                                list[marker].current = game_logic.selection;
                                list[marker].score = game_logic.score;
                            }
                        }
                    }
                }
                marker++;
            }

            int loc = 0;
            int tracker = 0;
            int max = 0;
            //Then get the largest of each root largest
            List<ComparableSelection> seq = new List<ComparableSelection>();

            foreach(ComparableSelection cs in list.Where(o => o.score != 0))
            {
                if(cs.score >= max)
                {
                    max = cs.score;
                    tracker = list.IndexOf(cs);
                }
                loc++;
            }


            this.cpu_tracker.updateScore(list[tracker].score);

            int n = 0;
            String output_word = "";
            marker = 0;
            List<ComparableSelection> new_items = new List<ComparableSelection>();
            new_items.Add(list[tracker]);
            if (new_items.Count == 0 || new_items[0].score == 0)
            {
                newAITray();
            }
            else
            {
                foreach (ComparableSelection item in new_items)
                {
                    //int marker = 0;
                    //Attempt to play word selection
                    //List<BoardTile> seq = item.current;
                    foreach (BoardTile t in item.current)
                    {
                        if (t.accepted_placement == false)
                        {
                            int delete_sequence = 0;
                            loc = 0;
                            foreach (BoardTile x in AITray.Children)
                            {
                                if (x.tag.letter_alpha == t.tag.letter_alpha)
                                {
                                    delete_sequence = loc;
                                }
                                loc++;
                            }

                            AITray.Children.RemoveAt(delete_sequence);
                        }

                        output_word += t.tag.letter_alpha;
                        List<BoardTile> outer_storage = new List<BoardTile>();
                        outer_storage.AddRange(item.current);

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
                                for (int i = marker; i < item.current.Count; i++)
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
                                for (int i = marker; i < item.current.Count; i++)
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
                    this.AIPlayedWords.Items.Add(new_items[0].ToString());
                }

                foreach (BoardTile t in GameBoard.Children)
                {
                    if (t.accepted_placement == false)
                    {

                    }
                    else
                    {
                        t.placement_possible = true;
                    }
                }

                this.AIScore.Content = this.cpu_tracker.score;

                game_logic.clearBuffer();
                beginAISequence();
            }
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
                        //inner_set.left = (BoardTile)GameBoard.Children[loc - 1];
                        //inner_set.right = (BoardTile)GameBoard.Children[loc + 1];
                        //inner_set.up = (BoardTile)GameBoard.Children[loc - 15];
                        //inner_set.down = (BoardTile)GameBoard.Children[loc + 15];
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

        private bool checkMapping(BoardTile t, int loc, int remaining_places, String direction)
        {  
            try
            {
                BoardTile placer = drawTile(t.tag);
                placer.id = loc;
                placer.Content = t.Content;
                BoardTile boundary = new BoardTile();
                boundary.id = -1;
                foreach(BoardTile board_tile in GameBoard.Children)
                {
                    if (loc < 255 && loc >= 0)
                    {
                        if (board_tile.id == loc)
                        {
                            if(direction.Equals("L") || direction.Equals("R"))
                            {
                                if(direction.Equals("R") && board_tile.right.tag != null)
                                {
                                    return false;
                                }
                                else if (direction.Equals("L") && board_tile.left.tag != null)
                                {
                                    return false;
                                }
                                if (board_tile.up.tag != null || board_tile.down.tag != null)
                                {
                                    return false;
                                }
                            }
                            else if (direction.Equals("U") || direction.Equals("D"))
                            {
                                if (direction.Equals("U") && board_tile.up.tag != null)
                                {
                                    return false;
                                }
                                else if (direction.Equals("D") && board_tile.down.tag != null)
                                {
                                    return false;
                                }
                                if (board_tile.left.tag != null || board_tile.right.tag != null)
                                {
                                    return false;
                                }
                            }
                                
                            if ((board_tile.id % 15 == 0 || board_tile.id % 15 == 14) && remaining_places > 0)
                            {
                                //Boundary violation set
                                int j = 0;
                                return false;
                            }
                            if ((board_tile.up == boundary || board_tile.down == boundary) && remaining_places > 0)
                            {
                                //Boundary violation set
                                int j = 0;
                                return false;
                            }
                            if(board_tile.id < 15 && remaining_places > 0)
                            {
                                return false;
                            }
                                
                            return true;
                        }
                    }
                    else
                        return false;
                }                       
            }
            catch (Exception e)
            {
                return false;
            }

            return false;
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

        private void clickViewTilePool(object sender, RoutedEventArgs e)
        {
            TilePoolViewForm view = new TilePoolViewForm(tile_sack);
            view.Show();
        }
    }
}
