using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace REFeed

{

    public class Program

    {

        static void Main(string[] args)
        {
            string cnnString = null;
            string DBQuery = null;
            int loopControl = 5;
            string googleAPIKey = "AIzaSyDGtABIyvMtekqCCD5dKSDGCn3mANVpvME";
            string unsortedAddr = null;
            string sortedAddr = null;

            cnnString = "Server=lazarus.ucc.ie;Database=UCC;Trusted_Connection=True;";
            DBQuery = "select * from ITSPRD..RENC_IF.RAISERS_EDGE_EXTRACT where ESRCLASSOF = '2016'";

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

            //query db for person information
            try
            {
                reader = query.ExecuteReader();
                Console.WriteLine("Reader executed");

                if (reader.HasRows)
                {
                    Console.WriteLine("Reader has rows");

                    //control the loop - use a second while condition (while i < loopControl, i++)
                    int i = 0;

                    while (i < loopControl && reader.Read())
                    {
                        //use SortAddresses method to sort sort addresses into Google API readable format

                        Console.WriteLine("Output Column data");

                        Console.WriteLine("Name: " + reader.GetString(10) + " " + reader.GetString(9));

                        Console.WriteLine("Class of: " + reader.GetString(39));

                        Console.WriteLine("Address (Lines): " + reader.GetString(16));

                        //use SortAddresses Method to format addresses into Google API URLS
                        unsortedAddr = reader.GetString(16);
                        
                        sortedAddr = SortAddresses(unsortedAddr, googleAPIKey);
                        Console.WriteLine("Output of SortAddresses: " + sortedAddr);
                        /*
                        string googleURL = sortedAddr;
                        string returncode = "";

                        
                        WebRequest req = HttpWebRequest.Create(googleURL);
                        req.Method = "GET";

                        string source;
                        using (StreamReader URLreader = new StreamReader(req.GetResponse().GetResponseStream()))
                        {
                            source = URLreader.ReadToEnd();
                        }
                        

                        Console.WriteLine(source);

                        Console.WriteLine(returncode);
                        */
                        //Console.WriteLine("Address (County): " + reader.GetString(18));

                        //Console.WriteLine("Address (Country): " + reader.GetString(20));
                        Console.WriteLine(" ");

                        i++;
                        
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

            //declare variables for deserialized JSON

            string JSONcontents = File.ReadAllText(@"C:\Users\ccreaghpeschau\Documents\REFeed\JSON1.json");

            Console.WriteLine(JSONcontents);

            GoogleAPIJSONCode ConvertedJSON = new GoogleAPIJSONCode();

            ConvertedJSON  = JsonConvert.DeserializeObject<GoogleAPIJSONCode>(JSONcontents);

            //string someJSON = GoogleAPIJSONCode.Location

            Console.WriteLine("*********************");
            
            string text = GoogleAPIJSONCode.AddressComponent.Equals

            //console output some variables from the JSON input

            Console.WriteLine();

            cnn.Close();
            //for debugging, console stays open
            Console.WriteLine("Press any button to close....");
            Console.ReadKey();
        }

        static string SortAddresses (string unsortedAddress, string APIKey)
        {
            
            string googleURL = "https://maps.googleapis.com/maps/api/geocode/json?";
            StringBuilder outputAddress = new StringBuilder(googleURL);
            string finalAddress = "";

            string[] delimiterStrings = {" /n"," "};

            string[] words = unsortedAddress.Split(delimiterStrings, StringSplitOptions.None);

            foreach (string s in words)
            {
                outputAddress.Append(s + "+"); 
            }
            
            finalAddress = outputAddress.ToString();

            //sortedAddress = unsortedAddress;

            return finalAddress;
        }



    }
    
    
}
