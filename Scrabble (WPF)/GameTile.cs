using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrabble
{
    class GameTile : Tile
    {
        public Letter tag { get; set; }

        public GameTile()
        {
            this.tile_chip = new Button();
            this.tile_chip.FontSize = 20;
        }

        public void setTileState(Letter l)
        {
            this.tag = l;
            this.tile_chip.Content = this.tag.letter_alpha + " (" + this.tag.score_value + ")";
        }
    }
}
