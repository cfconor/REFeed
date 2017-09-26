//import necessary libraries
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Text.RegularExpressions;
using System.Timers;

//create namespace (required; allows easy referencing and multiple classes within a namespace)
namespace REFeed

{
    //Generic naming of class, one class in programme so not an issue
    public class Program

    {

        static void Main(string[] args)
        {

            //refeedconfig logfile should always be located in User\REFeed\
            string userprof = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string customConfigFilePath = myDocsPath + @"\REFeed\refeedconfig.txt";
            //check if logfile exists in above location
            CheckCustomConfigFileExists(customConfigFilePath);

            string ESRClassOf = ReadCusConfig(customConfigFilePath, "ESRCLASSOF");
            ESRClassOf = RemoveSpecialCharacters(ESRClassOf);


            //initialized variables
            string csvPath = myDocsPath + @"\REFeed\csvoutput.csv";
            string cnnString = null;
            string DBQuery = null;
            
            string loopControlStr = ReadCusConfig(customConfigFilePath, "Loop_Control");

            loopControlStr = RemoveSpecialCharacters(loopControlStr);

            int loopControl = Convert.ToInt32(loopControlStr);
            string googleAPIKey = ReadCusConfig(customConfigFilePath, "APIKey");
            
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> column;

            List<string> headersInput = new List<string>();

            //DB connection and Query strings
            cnnString = "Server=lazarus.ucc.ie;Database=UCC;Trusted_Connection=True;";
            DBQuery = "select * from ITSPRD..RENC_IF.RAISERS_EDGE_EXTRACT where ESRCLASSOF = '" + ESRClassOf + "' order by LastName ASC";
            
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand query = new SqlCommand();
            SqlDataReader reader;
            
            query.CommandText = DBQuery;
            query.CommandType = CommandType.Text;
            query.Connection = cnn;
            
            //connect to existing students table in RE to match REFlag results
            SqlConnection cnn2 = new SqlConnection(cnnString);
            
            //open connection to ITS Database or Throw exception
            try
            {
                cnn.Open();
                Console.WriteLine("pinged DB Server\n");


            }
            catch(Exception e)
            {
                Console.WriteLine("ping failed!");
                Console.WriteLine(e);
            }

            //open second connection to ITS Database for entry matching with Raisers Edge
            try
            {
                cnn2.Open();
                Console.WriteLine("connected to  Raisers Edge DB server for entry matching! Please Wait...\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }



            //query db for person information
            try
            {
                reader = query.ExecuteReader();
                Console.WriteLine("DB Reader executed");

                if (reader.HasRows)
                {
                    Console.WriteLine("Reader has rows\n");

                    
                    //control the loop - use a second while condition (while i < loopControl, i++) (if loopControl is 0, read all records)
                    int i = 0;

                    while ((loopControl == 0 || i < loopControl) && reader.Read())
                    {
                        //Keep Track of loop
                        Console.WriteLine("*********************");
                        Console.WriteLine("This is iteration of Loop number...." + (i + 1) + "\n");
                        
                        column = new Dictionary<string, string>();

                        //assign all ITSDB columns into dictionary for easy manipulation
                        try
                        {
                            column["IBSNQFLEV"] = reader["IBSNQFLEV"].ToString();
                            column["ESRDateEnt"] = reader["ESRDateEnt"].ToString();
                            column["ImportId"] = reader["ImportId"].ToString();
                            column["AddrImpid"] = reader["AddrImpid"].ToString();
                            column["PhoneAddrImpId"] = reader["PhoneAddrImpId"].ToString();
                            column["PhoneImpId"] = reader["PhoneImpId"].ToString();
                            column["CAttrImpId"] = reader["CAttrImpId"].ToString();
                            column["ESRImpId"] = reader["ESRImpId"].ToString();
                            column["KeyInd"] = reader["KeyInd"].ToString();
                            column["PrimSalEdit"] = reader["PrimSalEdit"].ToString();
                            column["LastName"] = reader["LastName"].ToString();
                            column["FirstName"] = reader["FirstName"].ToString();
                            column["PrimSalText"] = reader["PrimSalText"].ToString();
                            column["NickName"] = reader["NickName"].ToString();
                            column["MidName"] = reader["MidName"].ToString();
                            column["Titl1"] = reader["Titl1"].ToString();
                            column["Gender"] = reader["Gender"].ToString();
                            column["AddrLines"] = @reader["AddrLines"].ToString();
                            column["AddrCity"] = reader["AddrCity"].ToString();
                            column["AddrCounty"] = reader["AddrCounty"].ToString();
                            column["LEN_COUNTY"] = reader["LEN_COUNTY"].ToString();
                            column["AddrCountry"] = reader["AddrCountry"].ToString();
                            column["LEN_COUNTRY"] = reader["LEN_COUNTRY"].ToString();
                            column["AddrZip"] = reader["AddrZip"].ToString();
                            column["BDay"] = reader["BDay"].ToString();
                            column["PhoneNum1"] = reader["PhoneNum1"].ToString();
                            column["PhoneType1"] = reader["PhoneType1"].ToString();
                            column["PhoneNum2"] = reader["PhoneNum2"].ToString();
                            column["PhoneType2"] = reader["PhoneType2"].ToString();
                            column["ConsID"] = reader["ConsID"].ToString();
                            column["CAttrCat1"] = reader["CAttrCat1"].ToString();
                            column["CAttrDesc1"] = reader["CAttrDesc1"].ToString();
                            column["CAttrCat2"] = reader["CAttrCat2"].ToString();
                            column["CAttrDesc2"] = reader["CAttrDesc2"].ToString();
                            column["CAttrCat3"] = reader["CAttrCat3"].ToString();
                            column["CAttrDesc3"] = reader["CAttrDesc3"].ToString();
                            column["CAttrCat4"] = reader["CAttrCat4"].ToString();
                            column["CAttrDesc4"] = reader["CAttrDesc4"].ToString();
                            column["ESRCampus"] = reader["ESRCampus"].ToString();
                            column["ESRDegree"] = reader["ESRDegree"].ToString();
                            column["ESRClassOf"] = reader["ESRClassOf"].ToString();
                            column["ESRDateEnt"] = reader["ESRDateEnt"].ToString();
                            column["PrimAddID"] = reader["PrimAddID"].ToString();
                            column["ConsCode"] = reader["ConsCode"].ToString();
                            column["ESRSchoolName"] = reader["ESRSchoolName"].ToString();
                            column["ESRPrimAlum"] = reader["ESRPrimAlum"].ToString();
                            column["Suff1"] = reader["Suff1"].ToString();
                            column["QUAL_TYPE_DESC"] = reader["QUAL_TYPE_DESC"].ToString();
                            column["UNDER_POSTGRAD"] = reader["UNDER_POSTGRAD"].ToString();
                            column["QUAL_LEVEL_TYPE"] = reader["QUAL_LEVEL_TYPE"].ToString();
                            column["POSTGRAD_TYPE"] = reader["POSTGRAD_TYPE"].ToString();
                            column["MASTERS_DOCTORAL"] = reader["MASTERS_DOCTORAL"].ToString();
                            column["NQF_LEVEL"] = reader["NQF_LEVEL"].ToString();
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Cannot assign Dictionary elems");
                            Console.WriteLine(e);
                        }


                        //match against existing RE records
                        string IDtoMatch = column["ConsID"];
                        string REQuery = "select FIRST_NAME,MIDDLE_NAME,KEY_NAME,TEXT from[UCC].[dbo].[CONSTITUENT] inner join[UCC].[dbo].[ConstituentAttributes] on[CONSTITUENT].RECORDS_ID = [ConstituentAttributes].PARENTID inner join[UCC].[dbo].[AttributeTypes] on[ConstituentAttributes].ATTRIBUTETYPESID = [AttributeTypes].ATTRIBUTETYPESID where upper(DESCRIPTION)  like '%STUDENT%ID%' AND TEXT = '" + IDtoMatch + "'";
                        string REFlag = "false";

                        //query Database again to match against existing RE records
                        try
                        {
                            SqlCommand REquery = new SqlCommand();
                            SqlDataReader REreader;
                            //uses same connection query as cnn1
                            REquery.CommandText = REQuery;
                            REquery.CommandType = CommandType.Text;
                            REquery.Connection = cnn2;

                            REreader = REquery.ExecuteReader();

                            if (REreader.HasRows == true)
                            {
                                while (REreader.Read())
                                {
                                    REFlag = "true";
                                }
                            }
                            else //reader has no rows, user is not in Raisers Edge
                            {
                                REFlag = "false";
                            }
                            
                            REreader.Close();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                        //use SortAddresses method to sort sort addresses into Google API readable format
                        string url;
                        url = SortAddresses(column["AddrLines"], googleAPIKey);
                        string pageContents = "";
                        //debug output
                        Console.WriteLine("The Output address for this user is..... " + url);
                        
                        //download JSON object from Google Geocoding API
                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                pageContents = client.DownloadString(url);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                        //add Admin1, Admin2 and Locality from JSON Object to variables if returned
                        string Admin1 = JSONDeserializer("administrative_area_level_1", pageContents);
                        string Admin2 = JSONDeserializer("administrative_area_level_2", pageContents);
                        string Locality = JSONDeserializer("locality", pageContents);

                        //refine AddrLines in special cases (Cork City, Cork County, Dublin)
                        if(column["AddrLines"].Contains("Cork"))
                        {
                            string corkAddr = IsCorkCityorCountry(column["AddrLines"]);
                            Admin1 = corkAddr;
                        }

                        if (column["AddrLines"].Contains("Dublin"))
                        {
                            Admin1 = column["AddrCounty"];
                        }

                        //assign dictionary elements
                        column["administrative_area_level_1"] = Admin1;
                        column["administrative_area_level_2"] = Admin2;
                        column["Locality"] = Locality;
                        column["OnREFlag"] = REFlag;

                        //first run, write headers to CSV output file
                        if (i == 0)
                        {
                            foreach(string val in column.Keys)
                            {
                                headersInput.Add(val + ",");
                            }
                            Console.WriteLine("Sent column data to headersInput");
                        }

                        //add column to rows dictionary
                        rows.Add(column);

                        
                        Console.WriteLine("\n\n");
                        //iterate loop
                        i++;
                        
                        //add timer to prevent overloading web requests to Google Maps API
                        //System.Threading.Timer timer = new System.Threading.Timer(1000);
                        

                    }
                    reader.Close();
                }
                Console.WriteLine("queried the ITSDB");
            }
            catch (Exception e)
            {
                Console.WriteLine("query failed");
                Console.WriteLine(e);
            }

            

            //Outputting Dictionary Contents
            Console.WriteLine("Showing Output of Dictionary...");
            Console.WriteLine("*********************");

            
            string trimmedOutput = "";
            //string trimmedHeadersOutput = "";

            File.WriteAllText(csvPath, "");

            using(StreamWriter csvWrite = File.AppendText(csvPath))
            {
                foreach (string entry in headersInput)
                {
                    
                    csvWrite.Write(entry);
                }

                csvWrite.Write("\n");

                foreach (Dictionary<string, string> columnRead in rows)
                {
                    StringBuilder rowFormatted = new StringBuilder();

                    foreach (string colVal in columnRead.Values)
                    {

                        trimmedOutput = Regex.Replace(colVal, @"[,]", "");

                        rowFormatted.Append(trimmedOutput + ",");

                    }
                    csvWrite.WriteLine(rowFormatted.ToString());

                    
                }
            }
                
            
            
            
            

            //console output some variables from the JSON input
            
            cnn.Close();
            //console stays open, allow user to record the output filepath
            Console.WriteLine("*********************");
            Console.WriteLine("The CSV file containing user data can be found at: " + csvPath);
            Console.WriteLine("Press any button to close....");
            Console.ReadKey();
        }
        //method to take unsorted address details and compile them into a URL
        static string SortAddresses(string unsortedAddress, string inputAPIKey)
        {

            string googleURL = "https://maps.googleapis.com/maps/api/geocode/json?address=";
            StringBuilder outputAddress = new StringBuilder(googleURL);
            string finalAddress = "";
            string stringifiedOutput = "";

            string[] delimiterStrings = { " /n", " " };

            string[] words = unsortedAddress.Split(delimiterStrings, StringSplitOptions.None);

            string APIKey = "&key" + inputAPIKey;

            foreach (string s in words)
            {
                
                outputAddress.Append(RemoveSpecialCharacters(s) + "+");
                
            }

            outputAddress.Append(APIKey);

            stringifiedOutput = outputAddress.ToString();
            
            finalAddress = stringifiedOutput;
            
            return finalAddress;
        }
        //turn JSON object into string data
        static string JSONDeserializer(string reqType, string urlcontents)
        {

            

            string JSONcontents = urlcontents;
            string outputData = "NULL";
            var collectedStuff = new List<string>();
            StringBuilder outputStuff = new StringBuilder();

            var JSONObj = JsonConvert.DeserializeObject<GoogleAPIJSONCode.RootObject>(JSONcontents);

            //if not OK, no elements in JSONObj.results, implies the query had had no data for the address. i.e no returned data for Admin1, Admin2, Locality
            if (JSONObj.status.Equals("OK"))
            {
                foreach (var res in JSONObj.results)
                {
                    foreach (var innerRes1 in res.address_components)
                    {
                        if (innerRes1.types.Contains(reqType))
                        {

                            collectedStuff.Add(innerRes1.long_name);
                            
                        }
                    }
                }
                
                foreach (string elem in collectedStuff)
                {
                    if (elem.Equals("NULL"))
                    {

                    }
                    else //push in the data from the collection to the StringBuilder object
                    {
                        outputStuff.Append(elem);
                        outputStuff.Append(", ");
                    }

                }

                outputData = outputStuff.ToString();

                if (outputData.Length != 0)
                {
                    outputData = outputData.Remove(outputData.Length - 2);
                }
                //push array strings into single string then set as outPutData
                
                return outputData;
            }
            
            return outputData;

        }
        //Remove special Characters from Code
        public static string RemoveSpecialCharacters(string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", System.Text.RegularExpressions.RegexOptions.Compiled);
        }
        //verify if a config file exists
        public static string CheckCustomConfigFileExists(string configFilePath)
        {
            
            
            
            

            if(!File.Exists(configFilePath))
            {
                Console.WriteLine(configFilePath + " does not exist!");

                using (StreamWriter sw = File.CreateText(configFilePath))
                {
                    sw.Close();
                }

            }


            return configFilePath;
        }
        //read specific parameter from user config file, returns matching value (user config data must have form parameterDescription=parameter)
        public static string ReadCusConfig(string configFilePath, string ReqParameter)
        {
            
            string output = "";
            
            try
            {
                string[] readText = File.ReadAllLines(configFilePath);
                
                foreach (string s in readText)
                {
                    
                    if (s.Contains(ReqParameter))
                    {
                       
                        string[] spltStr = s.Split('=');
                        output = spltStr[1];
                        


                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read from custom Config file! \n" + e);
            }
            
            return output;

        }
        //Specify the Address type for any address in Cork
        public static string IsCorkCityorCountry(string inputAddr)
        {
            string output = "";
            bool isCounty;
            bool isCity;
            string corkAddr;

            corkAddr = inputAddr.Replace("/n", string.Empty);
            corkAddr = inputAddr.Replace(".", string.Empty);

            
            if (corkAddr.Contains("Co Cork") || corkAddr.Contains("Co. Cork") || corkAddr.Contains("CoCork") || corkAddr.Contains("Co  Cork"))
            {
                isCounty = true;

                corkAddr = inputAddr.Replace("Co Cork", string.Empty);
            }
            else
            {
                isCounty = false;
            }

            if(corkAddr.Contains("Cork"))
            {
                isCity = true;
            }else
            {
                isCity = false;
            }

            if(isCity == true)
            {
                output = "Cork";
            }
            else
            {
                output = "Co. Cork";
            }

            
            return output;
        }
        
    }
}
    

