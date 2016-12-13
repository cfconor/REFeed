using System;
using System.Data;
using System.Data.SqlClient;



namespace Database_Connection_Application

{

    public class Program

    {

        static void Main(string[] args)
        {
            string cnnString = null;
            string DBQuery = null;
            
            

            cnnString = "Server=lazarus.ucc.ie;Database=UCC;Trusted_Connection=True;";
            DBQuery = "select COUNT(*) from CONSTITUENT";
            

        }

    }

}
