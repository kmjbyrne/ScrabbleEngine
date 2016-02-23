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
    class WordBuffer
    {
        private List<String> words { get; set; }
        public List<BoardTile> selection { get; set; }
        public int sequence_value { get; set; }
        public int root_location { get; set; }
        public int count { get; set; }
        public List<int> grid_coordinates { get; set; }
        public int score { get; set; }
        public String current { get; set; }
        private Dictionary<String, int> scoring_keys;

        public WordBuffer()
        {
            this.selection = new List<BoardTile>();
            this.words = this.getAllWords();
            this.sequence_value = 0;
            this.count = 0;
            this.grid_coordinates = new List<int>();
            this.score = 0;
            this.scoring_keys = this.getScoringIndex();
        }

        public void addToSequence(BoardTile t)
        {
            this.selection.Add(t);
        }
        public BoardTile getFromSequence()
        {
            BoardTile t = this.selection[selection.Count];
            this.selection.Remove(t);
            return t;
        }

        public void sortInputList()
        {
            this.selection = selection.OrderBy(o => o.id).ToList();
        }

        public void calculateScore()
        {
            int score = 0;
            int total_bonus = 0;
            foreach(BoardTile t in selection)
            {
                if(t.bonus_multiplier.Key == 'L')
                {
                    score += t.tag.score_value * t.bonus_multiplier.Value;
                }
                else if(t.bonus_multiplier.Key == 'W')
                {
                    total_bonus = t.bonus_multiplier.Value;
                    score += t.tag.score_value;
                }
                else if(t.bonus_multiplier.Key =='X') 
                {
                    //No bonus present on tile
                    score += t.tag.score_value;
                }
            }

            if(total_bonus == 0)
                this.score = score;
            else if(total_bonus > 0)
                this.score = score * total_bonus;    
        }

        public bool checkCurrentEntry()
        {
            String s = "";
            foreach(BoardTile t in this.selection)
            {
                s += t.tag.letter_alpha;
            }
            this.current = s;

            return isWordObj(s);
        }

        public bool isWordDB(String s)
        {
            SQLConnection connect = new SQLConnection();
            int dt = connect.getWords(s).Rows.Count;

            if (dt == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool isWordObj(String value)
        {
            foreach(String s in this.words)
            {
                if (s.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public List<String> getAllWords()
        {
            long counter_pre = DateTime.Now.Millisecond;
            SQLConnection connect = new SQLConnection();
            List<String> output = connect.getWords();
            long counter_post= DateTime.Now.Millisecond;
            return output;
        }

        private Dictionary<String, int> getScoringIndex()
        {
            SQLConnection connect = new SQLConnection();
            return connect.getScoreIndex();
        }

        public void clearBuffer()
        {
            this.selection.Clear();
            this.count = 0;
            this.current = "";
            this.score = 0;
            this.sequence_value = 0;
            this.grid_coordinates.Clear();
        }

        public List<BoardTile> getTilesFromString(String s, BoardTile root, int loc, List<BoardTile> score_index, int tray_location)
        {
            int counter=0;
            List<BoardTile> return_set = new List<BoardTile>();
            foreach (Char c in s)
            {
                if (c != null && c != '\0')
                {
                    if (loc == counter)
                    {
                        return_set.Add(root);
                    }
                    else
                    {
                        BoardTile temp = new BoardTile();
                        temp.Content = c.ToString();
                        temp.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };
                        temp.tray_location = tray_location;
                        temp.FontWeight = FontWeights.UltraBold;
                        temp.tag = this.getScoreKey(c.ToString());
                        return_set.Add(temp);
                    }
                    counter++;
                }
            }
            return return_set;
        }

        private Letter getScoreKey(String letter)
        {
            Letter tag = new Letter();
            foreach(KeyValuePair<String, int> kvp in this.scoring_keys)
            {
                if(letter.Equals(kvp.Key))
                {
                    tag.letter_alpha = kvp.Key;
                    tag.score_value = kvp.Value;
                    return tag;
                }
            }
            return new Letter();
        }

        private Letter getTag(Char input)
        {
            //Iterate through global selection list
            //To identify the score value for the character
            return new Letter(0, input.ToString(), 5);
        }
    }
}
