using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class ComparableSelection
    {
        public List<BoardTile> current { get; set; }
        public int score { get; set; }

        public ComparableSelection(List<BoardTile> input, int score)
        {
            this.current = input;
            this.score = score;
        }
        public ComparableSelection(int i)
        {
            this.score = i;
        }

        public override string ToString()
        {
            String s = "";
            foreach(BoardTile t in this.current)
            {
                s += t.tag.letter_alpha;
            }

            s += " - " + this.score;
            return s;
        }
    }
}
