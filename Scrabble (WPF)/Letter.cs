using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class Letter
    {
        public int distribution_count { get; set; }
        public String letter_alpha { get; set; }
        public int score_value { get; set; }

        public Letter(){}
        public Letter(int x, String c, int s)
        {
            this.distribution_count = x;
            this.letter_alpha = c;
            this.score_value = s;
        }

        public void decrementCount()
        {
            this.distribution_count -= 1;
        }
        public void incrementCount()
        {
            this.distribution_count++;
        }
        public override string ToString()
        {
            return this.letter_alpha + " (" + this.score_value + ")";
        }
    }
}
