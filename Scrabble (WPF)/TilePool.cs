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
            int random_base = new Random().Next(0, letters.Count);
            int random_mod = new Random().Next(new Random().Next(100, 945345));
            int random = random_base % random_mod;


            Letter l = letters[random];
            letters[random].decrementCount();
            letters.Remove(l);
            return l;
        }
    }
}
