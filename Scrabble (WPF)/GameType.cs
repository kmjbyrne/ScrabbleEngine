using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    public class GameType
    {
        public String name { get; set; }
        public int formula { get; set; }

        public GameType(String name, int formula)
        {
            this.name = name;
            this.formula = formula;
        }
    }
}
