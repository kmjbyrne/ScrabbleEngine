using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.SqlServerCe;
using System.IO;


namespace Scrabble_Data_Benching
{
    public partial class Form1 : Form
    {
        private int word_count;
        private String database_path = "C:\\Scrabble\\Database.sdf";
        private String boot_files = "C:\\Scrabble\\Boot Files";

        public Form1()
        {
            InitializeComponent();
            CheckDefaultDirectory();
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
        
        /// <summary>
        /// Method is used for CSV seperate files.
        /// One CSV file per alphabetic letter leader
        /// </summary>
        private void loadWords()
        {
            String[] file_array = Directory.GetFiles("C:\\Scrabble\\Boot Files\\");

            String connectionString = string.Format(
              "DataSource=\"{0}\"; Password='{1}'", database_path, "12345");

            SqlCeConnection conn = new SqlCeConnection(connectionString);

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
                var reader = new StreamReader(File.OpenRead(s));

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    String lines = reader.ReadToEnd();

                    if(line != null)
                    {
                        string sqlquery = "INSERT INTO WORD_MASTER (WORD_ID, WORD)" + " VALUES(@id,@word)";
                        SqlCeCommand cmd = new SqlCeCommand(sqlquery, conn);

                        cmd.Parameters.AddWithValue("@id", word_count);
                        cmd.Parameters.AddWithValue("@word", line);

                        cmd.ExecuteNonQuery();

                        word_count++;
                    }
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

            String connectionString = string.Format(
              "DataSource=\"{0}\"; Password='{1}'", database_path, "12345");
            SqlCeConnection conn = new SqlCeConnection(connectionString);

            String query = "SELECT * FROM WORD_MASTER WHERE WORD = '" + word + "'";

            SqlCeCommand comm = new SqlCeCommand(query, conn);

            SqlCeDataAdapter da = new SqlCeDataAdapter(comm);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Not a word", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            query = "SELECT * FROM WORD_MASTER WHERE WORD LIKE '" + word + "%'";

            comm = new SqlCeCommand(query, conn);

            da = new SqlCeDataAdapter(comm);

            dt.Clear();
            da.Fill(dt);

            foreach(DataRow r in dt.Rows)
            {
                listBox1.Items.Add(r["WORD"].ToString());
            }
        }
    }
}
