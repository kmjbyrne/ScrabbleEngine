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
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void clickSubmitTrigger(object sender, MouseButtonEventArgs e)
        {
            SQLConnection connection = new SQLConnection();
            if (!connection.checkUserExists(this.UsernameInput.Text.ToString()))
                MessageBox.Show("This username already exists!", "Alert", 
                    MessageBoxButton.OK, MessageBoxImage.Hand);
            else
            {
                String u = this.UsernameInput.Text.ToString();
                String p = this.PasswordInput.Password.ToString();
                connection.insertNewUser(u, p);
            }
        }
    }
}
