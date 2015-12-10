using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;


namespace Scrabble
{
    class BoardTile : Tile
    {
        public BoardTile()
        {
            this.tile_chip = new Button();
            this.tile_chip.FontSize = 15;
        }
    }
}
