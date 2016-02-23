using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class GameKeeper
    {
        public int score { get; set; }
        public int round { get; set; }

        public GameKeeper()
        {
            this.score = 0;
            this.round = 0;
        }

        public void updateScore(int value)
        {
            this.score += value;
        }

        public void updateRound()
        {
            this.round++;
        }
    }
}
