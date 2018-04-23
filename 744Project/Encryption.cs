using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace _744Project
{
    public class Encryption
    {
        //This is to get the connection string from the configuration file:
        static string connString = getConnection();
        SqlConnection connect = new SqlConnection(connString);
        //SqlConnection connect = Configuration.getConnectionString();
        public Encryption()
        {
            connString = getConnection();
            PasswordHash = "P@@Sw0rd";
            SaltKey = "S@LT&KEY";
            VIKey = "@1B2c3D4e5F6g7H8";
        }
        public static string getConnection()
        {
            string conn = Configuration.getConnectionString();
            return conn;
        }

        public void encryptAndStoreTransaction(string transactionId)
        {
            //Open a new connection to the database:
            connect.Open();
            //Call a CreateCommand method from the SQLCommand class to use it for writing queries:
            SqlCommand cmd = connect.CreateCommand();
            //Define the type of SQL commands as text:
            cmd.CommandType = CommandType.Text;
            //Make sure that the transaction exists to avoid SQL exceptions:
            cmd.CommandText = "select count(*) from Transactions where transactionId = '"+transactionId+"' ";
            //Store the result as an integer value:
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            //Check if the transaction is already encrypted or not:
            int isEncrypted = checkIfEncrypted(transactionId);
            //IDs are unique and there will be either one ID or none. If the ID exists, the result will be 1:
            if (count > 0 && isEncrypted != 1)
                storeAsEncrypted(transactionId);
            //Close the connection to the database:
            connect.Close();
        }
        public int checkIfEncrypted(string transactionId)
        {
            int isEncrypted = 3; //Assuming that it's null. 3 = null.
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select encryptedFlag from Transactions where transactionId = '" + transactionId + "' ";
            string strIsEncryptedValue = cmd.ExecuteScalar().ToString();            
            if (!string.IsNullOrEmpty(strIsEncryptedValue))
            {
                //isEncryptedValue = Convert.ToInt32(strIsEncryptedValue);
                if (strIsEncryptedValue.Equals("False")) //If decrypted
                    isEncrypted = 0;
                else if (strIsEncryptedValue.Equals("True")) // If encrypted
                    isEncrypted = 1;
                //Otherwise, isEncrypted will remain 3 as NULL in my code.
            }
            return isEncrypted;
        }
        public void storeAsEncrypted(string transactionId)
        {
                       
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            //Get the transaction information from the database:
            cmd.CommandText = "select transactionAmount from Transactions where transactionId = '" + transactionId + "' ";
            string transactionAmount = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select transactionType from Transactions where transactionId = '" + transactionId + "' ";
            string transactionType = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select transactionMerchant from Transactions where transactionId = '" + transactionId + "' ";
            string transactionMerchant = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select transactionStatus from Transactions where transactionId = '" + transactionId + "' ";
            string transactionStatus = cmd.ExecuteScalar().ToString();
            //cmd.CommandText = "select storeIP from Transactions where transactionId = '" + transactionId + "' ";
            //string storeIp = cmd.ExecuteScalar().ToString();
            //encrypt each string in the transactions except the transactionID, cardID, and AccountID. 
            //I also noticed that we still have the "connectionID". I just ignored it.
            transactionAmount = encrypt(transactionAmount);
            transactionType = encrypt(transactionType);
            transactionMerchant = encrypt(transactionMerchant);
            //storeIp = encrypt(storeIp);
            //transactionStatus = encrypt(transactionStatus); //The transaction status cannot be encrypted because its value is either True or False.
            //Now update the selected transaction:
            cmd.CommandText = "update Transactions set transactionAmount = '" + transactionAmount + "' ,    " +
                "transactionType = '" + transactionType + "' , transactionMerchant = '" + transactionMerchant + "' , " +
                "transactionStatus = '" + transactionStatus + "' , encryptedFlag = 1 " +
                //", storeIP = '" + storeIp + "' " +
                "where transactionId = '" + transactionId + "' ";
            cmd.ExecuteScalar();
            
        }
        public void decryptAndStoreTransaction(string transactionId)
        {
            //Open a new connection to the database:
            connect.Open();
            //Call a CreateCommand method from the SQLCommand class to use it for writing queries:
            SqlCommand cmd = connect.CreateCommand();
            //Define the type of SQL commands as text:
            cmd.CommandType = CommandType.Text;
            //Make sure that the transaction exists to avoid SQL exceptions:
            cmd.CommandText = "select count(*) from Transactions where transactionId = '" + transactionId + "' ";
            //Store the result as an integer value:
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            //Check if the transaction is already encrypted or not:
            int isEncrypted = checkIfEncrypted(transactionId);
            //IDs are unique and there will be either one ID or none. If the ID exists, the result will be 1:
            if (count > 0 && isEncrypted != 0)
                storeAsDecrypted(transactionId);
            //Close the connection to the database:
            connect.Close();
        }
        public void storeAsDecrypted(string transactionId)
        {            
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            //Get the transaction information from the database:
            cmd.CommandText = "select transactionAmount from Transactions where transactionId = '" + transactionId + "' ";
            string transactionAmount = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select transactionType from Transactions where transactionId = '" + transactionId + "' ";
            string transactionType = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select transactionMerchant from Transactions where transactionId = '" + transactionId + "' ";
            string transactionMerchant = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select transactionStatus from Transactions where transactionId = '" + transactionId + "' ";
            string transactionStatus = cmd.ExecuteScalar().ToString();
            //cmd.CommandText = "select storeIP from Transactions where transactionId = '" + transactionId + "' ";
            //string storeIp = cmd.ExecuteScalar().ToString();
            //encrypt each string in the transactions except the transactionID, cardID, and AccountID. 
            //I also noticed that we still have the "connectionID". I just ignored it.
            transactionAmount = decrypt(transactionAmount);
            transactionType = decrypt(transactionType);
            transactionMerchant = decrypt(transactionMerchant);
            //The below is to avoid the single quitations and replcae them with 2 single quotations
            transactionMerchant = transactionMerchant.Replace("'", "''");
            //
            //storeIp = decrypt(storeIp);
            //transactionStatus = decrypt(transactionStatus); //The transaction status cannot be decrypted because its value is either True or False.
            //Now update the selected transaction:
            cmd.CommandText = "update Transactions set transactionAmount = '" + transactionAmount + "' ,    " +
                "transactionType = '" + transactionType + "' , transactionMerchant = '" + transactionMerchant + "' , " +
                "transactionStatus = '" + transactionStatus + "' , encryptedFlag = 0 " +
                //", storeIP = '" + storeIp + "' " +
                "where transactionId = '" + transactionId + "' ";
                
            cmd.ExecuteScalar();
        }

        /*
         The below solution was made publicly by the Visual Studio Forums. It was copied 
         and slightly modified to suit our needs. The code for encryption and decryption 
         was tested in a C# windows application and it performed as expected. The PasswordHash, 
         SaltKey, and VIKey are supposed to be secured in a different location other than 
         having them in the same class file. It is suggested to store the keys in the 
         configuration file. For the purpose of our project, I chose to leave them 
         here. The link to the original code is:
         https://social.msdn.microsoft.com/Forums/vstudio/en-US/d6a2836a-d587-4068-8630-94f4fb2a2aeb/encrypt-and-decrypt-a-string-in-c?forum=csharpgeneral             
         */
        //------------------------ENCRYPTION KEYS------------------------------
        //static readonly string PasswordHash = "P@@Sw0rd";
        //static readonly string SaltKey = "S@LT&KEY";
        //static readonly string VIKey = "@1B2c3D4e5F6g7H8";    
        static string PasswordHash;
        static string SaltKey;
        static string VIKey;
        //------------------------ENCRYPTION METHOD------------------------------
        public static string encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        //------------------------DECRYPTION METHOD------------------------------
        public static string decrypt(string encryptedText)
        {
            try
            {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
            }
            catch (Exception)
            {
                //decryptedByteCount = 0;
                return encryptedText;
            }
            //
            //memoryStream.Close();
            //cryptoStream.Close();
            //return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }


    }
}