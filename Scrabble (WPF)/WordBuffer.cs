using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class WordBuffer
    {
        private List<String> words { get; set; }
        public List<BoardTile> selection { get; set; }
        public int sequence_value { get; set; }
        public int count { get; set; }
        public List<int> grid_coordinates { get; set; }
        public int score { get; set; }
        public String current { get; set; }

        public WordBuffer()
        {
            this.selection = new List<BoardTile>();
            this.words = this.getAllWords();
            this.sequence_value = 0;
            this.count = 0;
            this.grid_coordinates = new List<int>();
            this.score = 0;
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
                }
                else if(t.bonus_multiplier.Key =='X') 
                {
                    //No bonus present on tile
                    score += t.tag.score_value;
                }
            }

            if(total_bonus == 0)
                this.score = score;
            else if(total_bonus == 1)
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
            SQLConnection connect = new SQLConnection();
            List<String> output = connect.getWords();
            return output;
        }
        public void clearBuffer()
        {
            this.selection.Clear();
        }
    }
}
