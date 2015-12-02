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
using System.Data.SqlServerCe;
using System.Data;

namespace Scrabble_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void ClickLoginTrigger(object sender, MouseButtonEventArgs e)
        {
            String username = UsernameInputField.Text;
            String password = PasswordInputField.Password;
            //Proceed to login
            SQLConnection connect = new SQLConnection();
            if (connect.checkUserCredentials(username, password))
            {
                int y = 0;
                MainWindow frame = new MainWindow();
                frame.ShowDialog();
            }
            else
            {
                HeaderLogin.Content = "Invalid Username or Password";
            }
        }

        private void MouseLeavel(object sender, MouseEventArgs e)
        {
            AboutTrigger.Foreground = System.Windows.Media.Brushes.White;
        }

        private void MouseAboutEnter(object sender, MouseEventArgs e)
        {
            AboutTrigger.Foreground = System.Windows.Media.Brushes.Yellow;
        }

        private void clickSignUp(object sender, MouseButtonEventArgs e)
        {
            SignUp form = new SignUp();
            form.ShowDialog();
        }
    }
}
