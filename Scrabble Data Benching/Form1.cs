using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Data.SqlServerCe;
using System.IO;

namespace Scrabble_Data_Benching
{
    public partial class Form1 : Form
    {
        private int word_count ;
        private String database_path = @"Data Source=C:\Scrabble\ScrabbleEngine\Scrabble (WPF)\bin\Debug\Data\ScrabbleDatabase.sdf;Password=root";
        private String database_path_MDF = "C:\\Scrabble\\Database.mdf";
        private String boot_files = "C:\\Scrabble\\Boot Files";
        private Dictionary<char, int> letters;

        public Form1()
        {
            InitializeComponent();
            CheckDefaultDirectory();
            generateCharacterDict();
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

        private void CheckDefaultDirectory()
        {
            if(Directory.Exists(@"C:\Scrabble"))
            {
                //Exists
            }
            else
            {
                Directory.CreateDirectory(@"C:\Scrabble");
            }
        }
        private void clickCreateDatabase(object sender, EventArgs e)
        {
            if (File.Exists(database_path))
            {
                MessageBox.Show("Database already exists", "Info", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            String connectionString = string.Format(
              "DataSource=\"{0}\"; Password='{1}'", database_path, "12345");
            SqlCeEngine en = new SqlCeEngine(connectionString);
            en.CreateDatabase();

            SqlCeConnection conn = new SqlCeConnection(connectionString);

            SqlCeCommand sqlcomm = new SqlCeCommand("CREATE TABLE WORD_MASTER (WORD_ID int, WORD NVARCHAR(200))", conn);

            try
            {
                conn.Open();
                sqlcomm.ExecuteNonQuery();
            }
            catch(Exception)
            {

            }
        }
        private void clickCreateDatabaseMDF(object sender, EventArgs e)
        {
            if (File.Exists(database_path_MDF))
            {
                MessageBox.Show("Database already exists", "Info", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            String connectionString = string.Format(
              "DataSource=\"{0}\"; Password='{1}'", database_path_MDF, "12345");
            SqlCeEngine en = new SqlCeEngine(connectionString);
            en.CreateDatabase();

            SqlCeConnection conn = new SqlCeConnection(connectionString);

            SqlCeCommand sqlcomm = new SqlCeCommand("CREATE TABLE WORD_MASTER (WORD_ID int, WORD NVARCHAR(200))", conn);

            try
            {
                conn.Open();
                sqlcomm.ExecuteNonQuery();
            }
            catch (Exception)
            {

            }
        }
        
        /// <summary>
        /// Method is used for CSV seperate files.
        /// One CSV file per alphabetic letter leader
        /// </summary>
        private void loadWords()
        {
            String[] file_array = Directory.GetFiles("C:\\Scrabble\\Boot Files\\");

            SqlCeConnection conn = new SqlCeConnection(database_path);

            try
            {
                conn.Open();
            }
            catch(Exception)
            {

            }

            foreach(String s in file_array)
            {
                //For each file in the files array
                //Iterate through the delimited files
                //Add each word to the database
                String[] lines = File.ReadAllLines(s);

                //while (!reader.EndOfStream)
                //{
                    foreach (String p in lines)
                    {
                        string sqlquery = "INSERT INTO WORDS (WORD)" + " VALUES(@word)";
                        SqlCeCommand cmd = new SqlCeCommand(sqlquery, conn);

                        cmd.Parameters.AddWithValue("@word", p);
                        cmd.ExecuteNonQuery();
                        word_count++;
                        cmd.Dispose();
                    }
            }
        }

        private void ClickLoadWords(object sender, EventArgs e)
        {
            loadWords();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            listBox1.Items.Clear();

            String word = textBox1.Text.ToString();

            String connectionString = database_path;
            SqlCeConnection conn = new SqlCeConnection(connectionString);

            long now = DateTime.Now.Millisecond;

            String query = "SELECT * FROM WORDS WHERE WORD = '" + word + "'";

            SqlCeCommand comm = new SqlCeCommand(query, conn);

            SqlCeDataAdapter da = new SqlCeDataAdapter(comm);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Not a word", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            query = "SELECT * FROM WORDS WHERE WORD LIKE '" + word + "%'";

            comm = new SqlCeCommand(query, conn);

            da = new SqlCeDataAdapter(comm);

            dt.Clear();
            da.Fill(dt);

            foreach(DataRow r in dt.Rows)
            {
                int total = 0;
                foreach(char c in r["WORD"].ToString().ToUpper())
                {
                    if(letters.ContainsKey(c))
                    {
                        total += letters[c];
                    }
                }
                listBox1.Items.Add("" + r["WORD"].ToString() + " : " + total);
            }

            long then = DateTime.Now.Millisecond;
            this.Text = "Time Taken: " + (now - then).ToString() + " milliseconds";
        }
    }
}
