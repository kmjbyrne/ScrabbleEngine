using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class GameStatistics
    {
        public int total_score_AI { get; set; }
        public int total_score_player { get; set; }
        public DateTime game_started { get; set; }
        public DateTime game_ended { get; set; }
        public int total_moves { get; set; }
        public String difficult_level { get; set; }
        public String victor { get; set; }

        public GameStatistics()
        {
            this.total_moves = 0;
            this.total_score_AI = 0;
            this.total_score_player = 0;
            this.game_started = DateTime.Now;
        }
    }
}
