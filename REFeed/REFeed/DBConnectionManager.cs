using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace REFeed
{
    class DBConnectionManager
    {
        //Host methods used to connect and feed data from DB
        string connectString = null;


        private void DBConnection(EventArgs e)
        {
            SqlConnection cnn;
            string connectString = null;
            SqlCommand command;
            string sql = null;

            connectString = "Data Source=lazarus.ucc.ie; Initial Catalog=UCC;Integrated Security=SSPI";
            sql = "use UCC select COUNT(*) from CONSTITUENT";
            cnn = new SqlConnection(connectString);
            
            try
            {
                cnn.Open();
                command = new SqlCommand(sql, cnn);
                command.ExecuteNonQuery();
                command.Dispose();
                cnn.Close();
                Console.WriteLine(" ExecuteNonQuery in SqlCommand executed !!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can not open connection ! ");
            }

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
       

        


    }
}
