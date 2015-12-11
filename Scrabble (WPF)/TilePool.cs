using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Scrabble
{
    class TilePool
    {
        private int remaining_tiles { get; set; }
        private List<Letter> letters;
        
        public TilePool()
        {
            this.letters = new List<Letter>();
            this.letters = getLetters();
            this.remaining_tiles = getRemainingCount();
        }

        public int getRemainingCount()
        {
            int count = 0;
            foreach (Letter l in letters)
            {
                count += l.distribution_count;
            }

            return count;
        }

        public List<Letter> getLetters()
        {
            SQLConnection db = new SQLConnection();
            DataTable dt = db.getLetterData();
            List<Letter> hold = new List<Letter>();

            foreach(DataRow r in dt.Rows)
            {
                Letter let = new Letter();
                let.distribution_count = (int)r["distribution"];
                let.letter_alpha = (String)r["alpha"];
                let.score_value = (int)r["score"];
                hold.Add(let);
            }

            List<Letter> hold_long = new List<Letter>();
            foreach (Letter r in hold)
            {
                for (int i = 0; i < r.distribution_count; i++)
			    {
                    hold_long.Add(r);
			    }    
            }

            return hold_long;
        }

        public Letter getRandomLetter()
        {
            Random random = new Random((int)DateTime.Now.Ticks & (0x0000FFFF ));
            int random_base = random.Next() % letters.Count;

            Letter l = letters[random_base];
            letters[random_base].decrementCount();
            letters.Remove(l);
            return l;
        }

        public void callLetterIncrement(BoardTile t)
        {
            Letter hold = t.tag;
            foreach(Letter l in letters)
            {
                if(l.letter_alpha == hold.letter_alpha)
                {
                    l.incrementCount();
                }
            }
        }
    }
}
