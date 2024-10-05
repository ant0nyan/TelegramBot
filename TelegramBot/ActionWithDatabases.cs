using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotExperiments;

namespace TelegramBot
{
    internal class ActionWithDatabases : DatabaseClass
    {   static DateTime time = DateTime.Now;
        
        //Information Table
        public static void InsertingInformation(string query)
        {
            
            OpenConnection();
            SqlCommand(query);
            CloseConnection();
        }

        public static string CheckingAndReturning(string query,string userTelegramId)
        {
            OpenConnection();
            SqlDataReader reader = SqlDataReader(query);
            string allText = string.Empty;
            if (reader.HasRows)
            {
                while (reader.Read()) 
                {
                    string dataBaseUserTelegramId = (string)reader.GetValue(3);
                    if (dataBaseUserTelegramId == userTelegramId)
                    {
                        allText = reader.GetValue(4).ToString();
                        break;
                    }                    
                }
            }
            CloseConnection();
            return allText;
   

        }
       
        public static void InsertingAllInformationOrOnlyText(string checkingQuery,string newText ,string query,string userTelegramId) 
        {
            string allText = CheckingAndReturning(checkingQuery, userTelegramId);
            string textQuery = $"UPDATE Information SET query = (N'{ AddingNewMessage(newText,allText)}') WHERE user_telegram_id = '{userTelegramId}'";
            if (allText!=string.Empty)
            {
                InsertingInformation(textQuery);
            }
            else
            {
                InsertingInformation(query);
            }
            
        }

        public static string AddingNewMessage(string newMessage,string allText) 
        {
           return allText =$" {allText} \n[{newMessage}] \n";

        }

        public static void InsertJsonValuesOnInformationTable( Root myDeserializedClass) 
        {
            string query = $"INSERT INTO Information (first_name,username,user_telegram_id,query) VALUES (" +
        $"N'{myDeserializedClass.message.chat.first_name}'," +
        $"N'{myDeserializedClass.message.chat.username}'," +
        $"N'{myDeserializedClass.message.from.id}'," + $"" +
        $"N'{myDeserializedClass.message.text}')";

            string checkingQuery = $"SELECT * FROM Information";

            InsertingAllInformationOrOnlyText(checkingQuery, myDeserializedClass.message.text, query, myDeserializedClass.message.from.id.ToString())   ;
        }

        public static string GetEncod(string original)
        {
            byte[] uft8Data = Encoding.UTF8.GetBytes(original);
            string output = Encoding.UTF8.GetString(uft8Data);
            return output;


        }
        //TokenCount

        public static void InsertingTokenCountAndUser(string userTelegramId ,int tokenCount) 
        {
            string query = $"INSERT INTO TokenCount (user_telegram_id,token_count,count_update_datetime) VALUES ('{userTelegramId}','{tokenCount}','{SqlDateTimeFormat(time)}')";
            InsertingInformation(query);


        }
        static public string SqlDateTimeFormat(DateTime dates)
        {
            return Convert.ToDateTime(dates).ToString("MM-dd-yyyy HH:mm:ss");
        }
        static public DateTime dotNetDateTimeFormat(string dates)
        {
            return DateTime.ParseExact(dates, "dd/MM/yyyy HH:mm:ss", null);
        }

        public static bool CheckingUsername(string userTelegramId ) 
        {
            string query = "SELECT user_telegram_id FROM TokenCount";
            OpenConnection();
            SqlDataReader reader = SqlDataReader(query);
            bool flag =  false;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string? dataBaseUsername = reader.GetValue(0).ToString();
                    if (dataBaseUsername == userTelegramId)
                    {
                        flag = true;
                       
                    }
                }
            }
            CloseConnection();
            return flag; ;
        }
        public static int ReturningTokenCount(string  userTelegramId) 
        {
            string query = "SELECT  user_telegram_id,token_count FROM TokenCount";
            OpenConnection();
            SqlDataReader reader = SqlDataReader(query);
            int tokenCount = 0;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string? dataBaseUserTelegramId = reader.GetValue(0).ToString();
                    if (dataBaseUserTelegramId == userTelegramId)
                    {
                        tokenCount = (int)reader.GetValue(1);
                    }
                }
            }
            CloseConnection();
            return tokenCount; ;
        }

        public static void SubtractionOperationFromTokenCount(string userTelegramId) 
        {
            string query = $"UPDATE TokenCount SET token_count = (token_count - 1) WHERE user_telegram_id ='{userTelegramId}'";
            InsertingInformation(query);
            UpdateingTokenCountTime(userTelegramId);
        }

        public static void GiftEveryDayToken(string userTelegramId, int tokenCount , int conditionTokenCount) 
        {   
            string query = $"UPDATE TokenCount SET token_count = {tokenCount} WHERE token_count < {conditionTokenCount} AND user_telegram_id ='{userTelegramId}'";
            InsertingInformation(query);
            UpdateingTokenCountTime(userTelegramId);
        }
        public static void UpdateingTokenCountTime(string userTelegramId) 
        {
            Console.WriteLine(userTelegramId);
            string query = $"UPDATE TokenCount SET count_update_datetime = '{SqlDateTimeFormat(time)}' WHERE user_telegram_id = '{userTelegramId}'";
            InsertingInformation(query);
        }
        public static DateTime SelectingTokenUpdateingTime(string userTelegramId) 
        {
            string query = "SELECT  user_telegram_id,count_update_datetime FROM TokenCount";
            OpenConnection();
            SqlDataReader reader = SqlDataReader(query);
            DateTime returningTime = DateTime.Now ;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string? dataBaseUserTelegramId = (string)reader.GetValue(0);
                    if (dataBaseUserTelegramId == userTelegramId)
                    {
                        returningTime = (DateTime)reader.GetValue(1);
                    }
                }
            }
            CloseConnection();
            return returningTime; ;


        }


    }
}
