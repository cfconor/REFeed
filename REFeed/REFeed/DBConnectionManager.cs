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


        private void DBConnection()
        {
            SqlConnection cnn;
            connectString = "Data Source=lazarus.ucc.ie; Initial Catalog=UCC;Integrated Security=SSPI;Server=localhost/lazarus.ucc.ie";

            cnn = new SqlConnection(connectString);

            try
            {
                cnn.Open;
            }
        }
       

        


    }
}
