using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class AIEngine
    {
        public List<BoardTile> current_list { get; set; }
        public AIEngine()
        {
            this.current_list = new List<BoardTile>();
        }
    }
}
