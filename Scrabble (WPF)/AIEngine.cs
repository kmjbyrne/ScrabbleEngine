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
        public List<String> possible_words {get;set;}
        public Trie<String> game_tree = new Trie<String>();
        public int score_margin { get; set; }

        public AIEngine()
        {
            this.current_list = new List<BoardTile>();
            this.score_margin = 0;
        }

        public void fillGameTree(List<String> words)
        {
            int x = 0;
            foreach (String s in words)
                game_tree.Put(s, (x++).ToString());
        }

        /// <summary>
        /// Gets the super set of possible results from the input string
        /// </summary>
        public void retrieveSuperSet()
        {
            String search_input = "";
            foreach(BoardTile t in current_list)
                    search_input += t.tag.letter_alpha;

            this.possible_words = this.game_tree.AIsearch(search_input);
        }
    }
}
