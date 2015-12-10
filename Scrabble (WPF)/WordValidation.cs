using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class WordValidation
    {
        public bool isWord(String s)
        {
            SQLConnection connect = new SQLConnection();
            int dt = connect.getWords(s).Rows.Count;

            if(dt == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int getScore(List<BoardTile> inputTiles)
        {
            int score = 0;
            foreach(BoardTile t in inputTiles)
            {
                score += t.tag.score_value;
                score *= t.bonus_multiplier;
            }

            return score;
        }
    }
}
