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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scrabble_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Queue<Button> selectionQueue;
        Dictionary<char, int> letters;
        public MainWindow()
        {
            InitializeComponent();
            generateCharacterDict();
            initStartUp();
 
        }
        private void initStartUp()
        {
            for (int i = 0; i < 8; i++)
            {
                System.Windows.Controls.Button newBtn = new Button();
                newBtn.Height = 60;
                newBtn.Width = 60;
                newBtn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("tile.jpg", UriKind.Relative)) };

                int x = new Random().Next(2);

                switch (x)
                {
                    case 0:
                        newBtn.Content = "A" + " (" + letters['A'] + ")";
                        break;
                    case 1:
                        newBtn.Content = "B"+ " (" + letters['B'] + ")";
                        break;
                    case 2:
                        newBtn.Content = "C" + " (" + letters['C'] + ")";
                        break;
                }

                newBtn.Name = "Button" + i.ToString();
                newBtn.Click += tileClickListener;
                PlayerTray.Children.Add(newBtn);
            }
        }

        private void generateCharacterDict()
        {
            letters = new Dictionary<char, int>();
            letters.Add('A', 1);
            letters.Add('B', 3);
            letters.Add('C', 3);
            letters.Add('D', 2);
            letters.Add('E', 1);
            letters.Add('F', 4);
            letters.Add('G', 2);
            letters.Add('H', 4);
            letters.Add('I', 1);
            letters.Add('J', 8);
            letters.Add('K', 5);
            letters.Add('L', 1);
            letters.Add('M', 3);
            letters.Add('N', 1);
            letters.Add('O', 1);
            letters.Add('P', 3);
            letters.Add('Q', 10);
            letters.Add('R', 1);
            letters.Add('S', 1);
            letters.Add('T', 1);
            letters.Add('U', 1);
            letters.Add('V', 4);
            letters.Add('W', 4);
            letters.Add('X', 8);
            letters.Add('Y', 4);
            letters.Add('Z', 10);
        }

        private void tileClickListener(object sender, EventArgs e)
        {
            if (selectionQueue == null) 
                selectionQueue = new Queue<Button>();

            selectionQueue.Enqueue((Button)sender);
            PlayerTray.Children.Remove((Button)sender);
        }

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void returnCharactersToTray(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    PlayerTray.Children.Add(selectionQueue.Dequeue());
                }
            }
            catch (Exception)
            {

            }

        }

        private void clickResetTray(object sender, RoutedEventArgs e)
        {
            int count = selectionQueue.Count;
            for (int i = 0; i < count; i++)
                PlayerTray.Children.Add(selectionQueue.Dequeue());
        }
    }

    class Tile
    {
        public int id { get; set; }
        public char letter { get; set; }
        public int score { get; set; }

        public Tile(int id, char letter, int score)
        {
            this.id = id;
            this.letter = letter;
            this.score = score;
        }
    }
}
