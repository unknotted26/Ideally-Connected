﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using IdeallyConnected.Utility;
using CsvHelper;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace IdeallyConnected.Database
{
    public class LocationRecord
    {
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string StateAbbreviation { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public int Population { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class CSVParser
    {
        private string locationsCSVFile = "C:\\Users\\kp12g_000\\Documents\\Visual Studio 2017\\Projects\\CSharpFinalProject\\IdeallyConnected.Utility\\uscitiesv1.1.csv";

        /// <summary>
        /// Load the data from a csv file representing geolocation data. Procedure duration is approximately 800 records / second.
        /// CSV data provided by http://simplemaps.com/data/us-cities.
        /// Results are not accessed until they are enumerated.
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public List<LocationRecord> LoadLocationsCSVFile(string fileLocation)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            List<LocationRecord> result = new List<LocationRecord>();
            StreamReader fstream = File.OpenText(fileLocation); 
            CsvReader csvstream = new CsvReader(fstream);
            csvstream.Read();

            while (csvstream.Read())
            {
                LocationRecord record = new LocationRecord();
                record.ZipCode = csvstream.GetField(0);
                record.State = csvstream.GetField(1); //"state_name");
                record.StateAbbreviation = csvstream.GetField(2); //("state_id");
                record.City = csvstream.GetField(3);
                record.County = csvstream.GetField(4); //("county_name");
                int population;
                record.Population = csvstream.TryGetField<int>(5, out population) ? population : 0;
                //record.Population = csvstream. .GetField<int>(5);
                record.Latitude = csvstream.GetField<decimal>(6); //("lat");
                record.Longitude = csvstream.GetField<decimal>(7); //("lng");
                // Create a new tuple for each zip code.
                /*
                if (record.ZipCode.Length > 5)
                {
                    foreach (string zipcode in record.ZipCode.Split(' ').ToList())
                    {
                        record.ZipCode = zipcode;
                        result.Add(record);
                    }
                }
                */
                //else
                //{
                    result.Add(record);
                //}
            }

            return result;
        }

        /// <summary>
        /// This loads about 25,000 records / second into the database.
        /// </summary>
        public void QuickLoadLocations()
        {
            List<LocationRecord> recordsToLoad = LoadLocationsCSVFile(locationsCSVFile);
            string connectionString = "Server=(localdb)\\MSSQLLocalDB; Database=LocationsDb; Trusted_Connection=True;";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(); // based on ADO.NET
                sqlCommand.CommandText = "AddLocations";
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Connection = sqlConnection;
                SqlParameter sqlParameter = new SqlParameter();
                sqlParameter.ParameterName = "@LocationData";
                sqlParameter.SqlDbType = System.Data.SqlDbType.Structured;
                sqlCommand.Parameters.Add(sqlParameter);

                // Load data
                DataTable locationDataTable = new DataTable("LocationData");
                locationDataTable.Columns.Add("ZipCode", typeof(string)).MaxLength = 5;
                locationDataTable.Columns.Add("State", typeof(string)).MaxLength = 100;
                locationDataTable.Columns.Add("StateAbbreviation", typeof(string)).MaxLength = 100;
                locationDataTable.Columns.Add("City", typeof(string)).MaxLength = 200;
                locationDataTable.Columns.Add("County", typeof(string)).MaxLength = 200;
                locationDataTable.Columns.Add("Population", typeof(int));
                locationDataTable.Columns.Add("Latitude", typeof(decimal));
                locationDataTable.Columns.Add("Longitude", typeof(decimal));
                foreach(LocationRecord locationRecord in recordsToLoad)
                {
                    DataRow row = locationDataTable.NewRow();
                    row["ZipCode"] = locationRecord.ZipCode.Length > 5 ? locationRecord.ZipCode.Substring(0,5) : locationRecord.ZipCode;
                    row["State"] = locationRecord.State;
                    row["StateAbbreviation"] = locationRecord.StateAbbreviation;
                    row["City"] = locationRecord.City;
                    row["County"] = locationRecord.County;
                    row["Population"] = locationRecord.Population;
                    row["Latitude"] = locationRecord.Latitude;
                    row["Longitude"] = locationRecord.Longitude;

                    if (locationRecord.ZipCode.Length > 5)
                    {
                        foreach (string zipcode in locationRecord.ZipCode.Split(' ').ToList())
                        {
                            row["ZipCode"] = zipcode; 
                            locationDataTable.Rows.Add(row.ItemArray);
                        }
                    }
                    else
                    {
                        locationDataTable.Rows.Add(row);
                    }
                }

                sqlCommand.Parameters["@LocationData"].Value = locationDataTable;
                int queriesExecuted = 0;
                try
                {
                    queriesExecuted = sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    Console.WriteLine($"\nTotal queries executed: {queriesExecuted}");
                }
            }
        }

        public void LoadLocationCSVData()
        {
            List<LocationRecord> records = LoadLocationsCSVFile(locationsCSVFile);
            string connectionString = "Server=(localdb)\\MSSQLLocalDB; Database=LocationsDb; Trusted_Connection=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "LocationReader";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                foreach(var locationRecord in records)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ZipCode", locationRecord.ZipCode);
                    cmd.Parameters.AddWithValue("@State", locationRecord.State);
                    cmd.Parameters.AddWithValue("@StateAbbreviation", locationRecord.StateAbbreviation);
                    cmd.Parameters.AddWithValue("@City", locationRecord.City);
                    cmd.Parameters.AddWithValue("@County", locationRecord.County);
                    cmd.Parameters.AddWithValue("@Population", locationRecord.Population);
                    cmd.Parameters.AddWithValue("@Latitude", locationRecord.Latitude);
                    cmd.Parameters.AddWithValue("@Longitude", locationRecord.Longitude);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }

    public class Driver
    {
        public static void Main(string[] args)
        {
            CSVParser cobj = new CSVParser();
            cobj.QuickLoadLocations();
        }
    }
}