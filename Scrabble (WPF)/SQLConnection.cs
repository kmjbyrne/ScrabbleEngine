﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using System.Data;

namespace Scrabble
{
    class SQLConnection
    {
        SqlCeConnection conn;
        String query;
        String connectionString = string.Format("DataSource=\"{0}\"; Password='{1}'", @"Data\ScrabbleDatabase.sdf", "root");
        
        public SQLConnection()
        {
            try
            {
                this.conn = new SqlCeConnection(connectionString);
                conn.Open();
            }
            catch(Exception e)
            {

            }
        }

        public bool checkUserExists(String username)
        {
            DataTable dt = new DataTable();
            try
            {
                String query = "SELECT * FROM USERS WHERE username = @username";
                SqlCeCommand command = new SqlCeCommand(query, conn);
                command.Parameters.AddWithValue("@username", username);
                DataTable check = runQuery(command);
                if (check.Rows.Count > 0)
                    return false;
                else
                    return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool checkUserCredentials(String username, String password)
        {
            DataTable dt = new DataTable();
            try
            {
                String query = "SELECT * FROM USERS WHERE USERNAME = @username AND PASSWORD = @password";
                SqlCeCommand command = new SqlCeCommand(query, conn);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                DataTable check = runQuery(command);
                if (check.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void insertNewUser(String username, String password)
        {
            try
            {
                String query = "INSERT INTO USERS (USERNAME, PASSWORD)values(@username, @password)";
                SqlCeCommand command = new SqlCeCommand(query, conn);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                runQuery(command);
            }
            catch(Exception)
            {

            }
        }
        public DataTable runQuery(SqlCeCommand comm)
        {
            DataTable dt = new DataTable();
            comm.ExecuteNonQuery();
            SqlCeDataAdapter da = new SqlCeDataAdapter(comm);
            da.Fill(dt);
            return dt;
        }



        public DataTable getLetterData()
        {
            DataTable dt = new DataTable();

            String query = "SELECT * FROM LETTER";

            SqlCeCommand comm = new SqlCeCommand(query, conn);
            comm.ExecuteNonQuery();
            SqlCeDataAdapter da = new SqlCeDataAdapter(comm);
            da.Fill(dt);
            return dt;
        }

        public List<String> getWords()
        {
            DataTable dt = new DataTable();

            String query = "SELECT * FROM WORDS";

            SqlCeCommand comm = new SqlCeCommand(query, conn);
            comm.ExecuteNonQuery();
            SqlCeDataAdapter da = new SqlCeDataAdapter(comm);
            da.Fill(dt);
            List<String> output = new List<string>();
            foreach(DataRow r in dt.Rows)
            {
                output.Add(r["WORD"].ToString().ToUpper());
            }
            return output;
        }
        public DataTable getWords(String s)
        {
            DataTable dt = new DataTable();

            String query = "SELECT * FROM WORDS WHERE WORD = @word";

            SqlCeCommand comm = new SqlCeCommand(query, conn);
            comm.Parameters.AddWithValue("@word", s);
            comm.ExecuteNonQuery();
            SqlCeDataAdapter da = new SqlCeDataAdapter(comm);
            da.Fill(dt);
            return dt;
        }
        public Dictionary<String, int> getScoreIndex()
        {
            DataTable dt = new DataTable();
            Dictionary<string, int> return_set = new Dictionary<string, int>();

            String query = "SELECT * FROM LETTER";

            SqlCeCommand comm = new SqlCeCommand(query, conn);
            comm.ExecuteNonQuery();
            SqlCeDataAdapter da = new SqlCeDataAdapter(comm);
            da.Fill(dt);

            foreach (DataRow r in dt.Rows)
            {
                return_set.Add(r[2].ToString(), Convert.ToInt32(r[3]));
            }

            return return_set;
        }
    }
}
