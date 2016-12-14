﻿using System;
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
            
            cnnString = "Server=lazarus.ucc.ie;Database=master;Trusted_Connection=True;";
            DBQuery = "SELECT * FROM OPENQUERY(ITSPRD, 'SELECT count(*) FROM RAISERS_EDGE_EXTRACT')";

            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand query = new SqlCommand();
            SqlDataReader reader;

            query.CommandText = DBQuery;
            query.CommandType = CommandType.Text;
            query.Connection = cnn;

            //open cnn
            try
            {
                cnn.Open();
                Console.WriteLine("pinged server!");
                

            }
            catch
            {
                Console.WriteLine("ping failed!");
            }

            //query db
            try
            {
                reader = query.ExecuteReader();
                Console.WriteLine("Reader executed");

                if (reader.HasRows)
                {
                    Console.WriteLine("Reader has rows");
                    while (reader.Read())
                    {
                        Console.WriteLine("Count(*) of CONSTITUENT is... " + reader.GetInt32(0));
                    }
                    reader.Close();
                }
                Console.WriteLine("queried the DB");
            }
            catch
            {
                Console.WriteLine("query failed");
            }

            //output reader data
            

            cnn.Close();
            //for debugging, console stays open
            Console.WriteLine("Press any button to close....");
            Console.ReadKey();
        }

    }

}
