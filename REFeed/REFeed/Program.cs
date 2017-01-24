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
                Console.WriteLine("pinged DB server!");
                

            }
            catch
            {
                Console.WriteLine("ping failed!");
            }

           

            //query db for person information
            try
            {
                reader = query.ExecuteReader();
                Console.WriteLine("DB Reader executed");

                if (reader.HasRows)
                {
                    Console.WriteLine("Reader has rows\n");

                    //control the loop - use a second while condition (while i < loopControl, i++)
                    int i = 0;

                    while (i < loopControl && reader.Read())
                    {
                        //Keep Track of loop
                        Console.WriteLine("This is iteration of Loop number...." + i + "\n");
                        
                        //use SortAddresses method to sort sort addresses into Google API readable format

                        Console.WriteLine("Output Column data");

                        Console.WriteLine("Name: " + reader.GetString(10) + " " + reader.GetString(9));

                        Console.WriteLine("Class of: " + reader.GetString(39));

                        Console.WriteLine("Address (Lines): " + reader.GetString(16));

                        //use SortAddresses Method to format addresses into Google API URLS
                        unsortedAddr = reader.GetString(16);
                        
                        sortedAddr = SortAddresses(unsortedAddr, googleAPIKey);
                        Console.WriteLine("Output of SortAddresses: " + sortedAddr + "\n");

                        try
                        {
                            string html = new WebClient().DownloadString(sortedAddr);

                            Console.WriteLine(html);
                        }
                        catch
                        {
                            Console.WriteLine("Could not download JSON String\n");
                        }

                        
                        //Console.WriteLine("Address (County): " + reader.GetString(18));

                        //Console.WriteLine("Address (Country): " + reader.GetString(20));
                        Console.WriteLine("\n\n");

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
            
            //Console.WriteLine(JSONcontents);


            Console.WriteLine("*********************");
            Console.WriteLine("Deserializing....");
            Console.WriteLine("*********************");

            var JSONObj = JsonConvert.DeserializeObject<GoogleAPIJSONCode.RootObject>(JSONcontents);

            
            

            foreach (var res in JSONObj.results)
            {
                //Console.WriteLine(res.address_components);

                Console.WriteLine("*********************");
                Console.WriteLine("Outputting Address Components");
                Console.WriteLine("*********************");
                foreach (var innerRes in res.address_components)
                {

                    Console.WriteLine(innerRes.long_name);
                }
            }
            
            //GoogleAPIJSONCode.RootObject JSONObj2 = JsonConvert.DeserializeObject<GoogleAPIJSONCode.RootObject>(JSONcontents);

            

            
            //Console.WriteLine(JSONObj2.)

            Console.WriteLine("*********************");

            //console output some variables from the JSON input

            Console.WriteLine();

            cnn.Close();
            //for debugging, console stays open
            Console.WriteLine("Press any button to close....");
            Console.ReadKey();
        }

        //separating out functions into methods for cleaner code

        //method to take unsorted address details and compile them into a URL
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
