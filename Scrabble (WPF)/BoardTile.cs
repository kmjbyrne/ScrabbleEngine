using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;


namespace Scrabble
{
    class BoardTile : Button
    {
        public bool occupied { get; set; }
        public int id { get; set; }
        public Letter tag { get; set; }

        public BoardTile()
        {
            this.Width = 40;
            this.Height = 40;
        }

        public override string ToString()
        {
            return this.tag.letter_alpha + " (" + this.tag.score_value +")";
        }
    }
}
