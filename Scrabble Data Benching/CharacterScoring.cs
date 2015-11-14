using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble_Data_Benching
{
    class CharacterScoring
    {
        public int point_value { get; set; }
        public char character { get; set; }
        public int index { get; set; }

        public CharacterScoring(char c)
        {
            this.character = c;
            getValue();
        }

        private void getValue()
        {

        }
    }
}
