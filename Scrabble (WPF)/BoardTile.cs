using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;


namespace Scrabble
{
    public class BoardTile : Button
    {
        public bool accepted_placement { get; set; }
        public KeyValuePair<char, int>bonus_multiplier { get; set; }
        public bool placement_possible { get; set; }
        public bool occupied { get; set; }
        public int id { get; set; }
        public Letter tag { get; set; }
        public int tray_location { get; set; }

        public BoardTile left { get; set; }
        public BoardTile right { get; set; }
        public BoardTile up { get; set; }
        public BoardTile down { get; set; }
                

        public BoardTile()
        {
            this.accepted_placement = false;
            this.Width = 40;
            this.Height = 40;
            this.bonus_multiplier = new KeyValuePair<char, int>('X', 0);
        }

        public override string ToString()
        {
            return this.tag.letter_alpha + " (" + this.tag.score_value + ")";
        }
    }
}
