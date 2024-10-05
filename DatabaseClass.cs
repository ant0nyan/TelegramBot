using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace TelegramBot
{
    internal class DatabaseClass
    {
        protected static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\User\\Documents\\HayqGPTDatabase.mdf;Integrated Security=True;Connect Timeout=30";
        protected static SqlConnection connection = new SqlConnection(connectionString);

       

        public static void OpenConnection()
        {
            connection.Open();
        }

        public static void CloseConnection()
        {
            connection.Close();
        }

        public static void SqlCommand(string query) 
        {
            SqlCommand cmd = new SqlCommand(query,connection);
            cmd.ExecuteNonQuery();
        }
        public static SqlDataReader SqlDataReader(string query) 
        {
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
           
            return reader;
        }

        public static SqlConnection ReturnConnection()
        {
            return connection;
        }
        


    }
}
