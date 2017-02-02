using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> column;


                 cnnString = "Server=lazarus.ucc.ie;Database=UCC;Trusted_Connection=True;";
            DBQuery = "select * from ITSPRD..RENC_IF.RAISERS_EDGE_EXTRACT where ESRCLASSOF = '2016' order by LastName ASC";
            

            

            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand query = new SqlCommand();
            SqlDataReader reader;
            

            query.CommandText = DBQuery;
            query.CommandType = CommandType.Text;
            query.Connection = cnn;

            string url = "http://www.google.com";

            //connect to existing students table in RE to match REFlag results
            SqlConnection cnn2 = new SqlConnection(cnnString);
            

            //open cnn
            try
            {
                cnn.Open();
                Console.WriteLine("pinged DB server!\n");
                

            }
            catch
            {
                Console.WriteLine("ping failed!");
            }

            try
            {
                cnn2.Open();
                Console.WriteLine("connected to  Raisers Edge DB server for entry matching!\n");
            }
            catch(Exception e)
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

                    

                    //control the loop - use a second while condition (while i < loopControl, i++)
                    int i = 0;

                    while (i < loopControl && reader.Read())
                    {
                        //Keep Track of loop
                        Console.WriteLine("This is iteration of Loop number...." + i + "\n");

                        string jsonFilePath = @"C:\Users\ccreaghpeschau\Documents\REFeed\JSON" + i + ".json";

                        //use SortAddresses method to sort sort addresses into Google API readable format

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
                            column["AddrLines"] = reader["AddrLines"].ToString();
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

                        
                        
                        //match against existing RE records!!!
                        
                        string IDtoMatch = column["ConsID"];
                        string REQuery = "select FIRST_NAME,MIDDLE_NAME,KEY_NAME,TEXT from[UCC].[dbo].[CONSTITUENT] inner join[UCC].[dbo].[ConstituentAttributes] on[CONSTITUENT].RECORDS_ID = [ConstituentAttributes].PARENTID inner join[UCC].[dbo].[AttributeTypes] on[ConstituentAttributes].ATTRIBUTETYPESID = [AttributeTypes].ATTRIBUTETYPESID where upper(DESCRIPTION)  like '%STUDENT%ID%' AND TEXT = '" + IDtoMatch + "'";
                        string REFlag = "false";




                        Console.WriteLine(IDtoMatch);

                        try
                        {
                            SqlCommand REquery = new SqlCommand();
                            SqlDataReader REreader;

                            REquery.CommandText = REQuery;
                            REquery.CommandType = CommandType.Text;
                            REquery.Connection = cnn2;
                            
                            REreader = REquery.ExecuteReader();

                            if (REreader.HasRows == true)
                            {
                                while(REreader.Read())
                                {
                                    //Console.WriteLine(REreader["FIRST_NAME"].ToString());
                                    //Console.WriteLine(REreader["MIDDLE_NAME"].ToString());
                                    //Console.WriteLine(REreader["KEY_NAME"].ToString());
                                    //Console.WriteLine(REreader["TEXT"].ToString());
                                    

                                    REFlag = "true";

                                }
                            }
                            else //reader has no rows, user is not in Raisers Edge
                            {
                                REFlag = "false";
                            }




                            REreader.Close();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                        }


                        //Make requests to Google API



                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                string s = client.DownloadString(url);

                                Console.WriteLine(s);

                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                        }



                        //add Admin1, Admin2 and Locality to code if returned

                        Console.WriteLine("jsonFilePath is: " + jsonFilePath + "\n\n");
                        
                        string Admin1 = JSONDeserializer("administrative_area_level_1", jsonFilePath);
                        string Admin2 = JSONDeserializer("administrative_area_level_2", jsonFilePath);
                        string Locality = JSONDeserializer("locality", jsonFilePath);

                        column["administrative_area_level_1"] = Admin1;
                        column["administrative_area_level_2"] = Admin2;
                        column["Locality"] = Locality;
                        column["OnREFlag"] = REFlag;


                        rows.Add(column);

                        

                        

                        

                        
                        //Console.WriteLine("Address (County): " + reader.GetString(18));

                        //Console.WriteLine("Address (Country): " + reader.GetString(20));
                        Console.WriteLine("\n\n");





                        i++;
                        
                    }
                    reader.Close();
                }
                Console.WriteLine("queried the ITSDB");
            }
            catch
            {
                Console.WriteLine("query failed");
            }



            Console.WriteLine("Showing Output of Dictionary...");
            Console.WriteLine("*********************");

            string csvPath = @"C:\Users\ccreaghpeschau\Documents\REFeed\csvoutput.csv";

            File.WriteAllText(csvPath, "");

            StringBuilder csvFormatted = new StringBuilder();

            foreach (Dictionary<string, string> columnRead in rows)
            {
                foreach (string colVal in columnRead.Values)
                {
                    csvFormatted.Append(colVal + "|");
                    
                }

                File.WriteAllText(csvPath, (csvFormatted.ToString() + Environment.NewLine));

                csvFormatted.Append("\n");

                
            }

            Console.WriteLine(csvFormatted.ToString());
            
            
            Console.WriteLine("*********************");

            //console output some variables from the JSON input
            
           
            cnn.Close();
            //for debugging, console stays open
            Console.WriteLine("Press any button to close....");
            Console.ReadKey();
        }

        //separating out functions into methods for cleaner code

        //method to take unsorted address details and compile them into a URL
        static string SortAddresses (string unsortedAddress, string APIKey)
        {
            
            string googleURL = "https://maps.googleapis.com/maps/api/geocode/json?address=";
            StringBuilder outputAddress = new StringBuilder(googleURL);
            string finalAddress = "";

            string[] delimiterStrings = {" /n"," "};

            string[] words = unsortedAddress.Split(delimiterStrings, StringSplitOptions.None);

            APIKey = "&key" + APIKey;

            foreach (string s in words)
            {
                outputAddress.Append(s + "+"); 
            }

            outputAddress.Append(APIKey);

            finalAddress = outputAddress.ToString();



            //sortedAddress = unsortedAddress;

            return finalAddress;
        }

        static string JSONDeserializer (string reqType, string filePath)
        {
            
            Console.WriteLine("Deserializing....");
            Console.WriteLine("*********************");

            string JSONcontents = File.ReadAllText(filePath);
            string outputData = "";
            bool lastLocality = false;
            string localityHolder = "";
            var JSONObj = JsonConvert.DeserializeObject<GoogleAPIJSONCode.RootObject>(JSONcontents);

            foreach (var res in JSONObj.results)
            {
                //Console.WriteLine(res.address_components);

                
                foreach (var innerRes in res.address_components)
                {
                    
                    if(innerRes.types[0].Equals(reqType))
                    {
                       
                        if (innerRes.types[0].Equals("locality") && lastLocality == true)
                        {
                            outputData = localityHolder + ", " + innerRes.long_name;
                            Console.WriteLine(outputData);
                            return outputData;

                        }
                        else if (innerRes.types[0].Equals("locality") && lastLocality == false)
                        {
                            localityHolder = innerRes.long_name;
                            
                            lastLocality = true;
                        }
                        else //not type locality
                        {

                            outputData = innerRes.long_name;
                            lastLocality = false;
                            
                        }
                        

                    }
                    
                }
                
            }
            Console.WriteLine(reqType + ": " + outputData);
            return outputData;

        }

         bool entryMatch(string toMatch, string queryToMatch)
        {


            return false;
        }   
            

    }

    
}
