using Microsoft.Extensions.Configuration;
using Pomelo.Data.MySql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace App.Migration
{
    public class Connection
    {
        private static IConfigurationRoot Configuration { get; set; }
        private Connection() { }
        public string Password { get; set; }
        public string Database { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection GetConnection
        {
            get { return connection; }
        }

        private static Connection instance = null;
        public static Connection Instance()
        {
            try
            {
                if (instance == null)
                    instance = new Connection();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return instance;
        }

        private String ConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string hostname = Configuration["hostname"].ToString();
            string username = Configuration["username"].ToString();
            string password = Configuration["password"].ToString();
            string database = Configuration["database"].ToString();
            Database = database;
            return "Server="+hostname+";database="+database+";uid="+username+";pwd="+password+";";
        }

        public bool IsConnect()
        {
            bool result = false;
            try
            {
               
                if (GetConnection == null)
                {
                    connection = new MySqlConnection(ConnectionString());
                    connection.Open();
                    result = true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return result;
        }

        public MySqlDataReader Query(String sql)
        {
            try
            {
                connection = new MySqlConnection(ConnectionString());
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(sql, GetConnection);
                return cmd.ExecuteReader();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }



        public void Close()
        {
            connection.Close();
        }

    }
}
