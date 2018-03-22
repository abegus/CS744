using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace _744Project
{
    public class Configuration
    {

        //This will get the connection string from the file Web.config and store it as a string:
        //static string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        //The below is the server's connection string. I tried to get it dynamically but that failed. For the moment, this works:
        static string connectionString = "data source=MURCSP84\\MSSQLSERVER01;initial catalog=NetworkDB;MultipleActiveResultSets=True;App=EntityFramework;user id=sa; password=AbeNanSaleh123";
        //The below is Saleh's personal connection string:
        //static string connectionString = "data source=R14\\SALEH;initial catalog=NetworkDB;Trusted_Connection=True;MultipleActiveResultSets=True;App=EntityFramework";

        public static string getConnectionString()
        {
            SqlConnection connect = new SqlConnection(connectionString);
            return connectionString;
            //return new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        }

    }
}  