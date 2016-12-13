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
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
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
            Console.ReadKey();
        }

    }

}
