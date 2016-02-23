using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Scrabble
{
    /// <summary>
    /// Interaction logic for TilePoolViewForm.xaml
    /// </summary>
    public partial class TilePoolViewForm : Window
    {
        public TilePoolViewForm(TilePool pool)
        {
            InitializeComponent();
            displayTilePool(pool);
        }

        private void displayTilePool(TilePool pool)
        {
            Dictionary<String, int> tiles = new Dictionary<String, int>();
            foreach (Letter l in pool.letters)
            {
                if (!(tiles.ContainsKey(l.letter_alpha)))
                    tiles.Add(l.letter_alpha, l.distribution_count);
                else if (tiles.ContainsKey(l.letter_alpha))
                { //  //
                }
            }

            foreach (KeyValuePair<String, int> kvp in tiles)
            {
                this.ListBox.Items.Add(kvp.Key + " - " + kvp.Value);
            }
        }
    }
}
