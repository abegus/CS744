﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace _744Project
{
    public class Configuration
    {
        
        //This will the connection string from the file Web.config and store it as a string:
        String connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;        

        public string getConnectionString()
        {
            SqlConnection connect = new SqlConnection(connectionString);
            return connectionString;
        }

        
    }
}  