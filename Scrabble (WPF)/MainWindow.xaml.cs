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

namespace Scrabble__WPF_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClickLoginTrigger(object sender, MouseButtonEventArgs e)
        {
            String username = UsernameInputField.Text;
            String password = PasswordInputField.Password;
            //Proceed to login
            String connectionString = string.Format("DataSource=\"{0}\"; Password='{1}'", @"Data\ScrabbleDatabase.sdf", "root");

            SqlCeConnection conn = new SqlCeConnection(connectionString);
            String query = "SELECT * FROM USERS WHERE username = @username";
            DataTable dt = new DataTable();

            try
            {
                conn.Open();
                SqlCeCommand comm = new SqlCeCommand(query, conn);
                comm.Parameters.AddWithValue("@username", username);
                comm.ExecuteNonQuery();

                SqlCeDataAdapter da = new SqlCeDataAdapter(comm);
                da.Fill(dt);
            }
            catch(Exception)
            {
            }

            if (dt.Rows.Count == 0)
            {
                HeaderLogin.Content = "Invalid Username or Password";
            }
            else
            {
                foreach (DataRow r in dt.Rows)
                {
                    if(r[2].ToString().Equals(password))
                    {
                        //Match
                        break;
                    }
                }
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
    }
}
