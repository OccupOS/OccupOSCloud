// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLServerHelper.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

 //using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Table;
namespace OccupOSCloud
{
    using System;
    using System.Data.SqlClient;
    using System.Text;

    public class SQLServerHelper
    {
        private string connectionString;

        private SqlConnectionStringBuilder connectionStringb;

        public SQLServerHelper(string dataSource, string userName, string password, string databaseName)
        {
            this.connectionStringb = new SqlConnectionStringBuilder
                                         {
                                             DataSource = dataSource, 
                                             Encrypt = true, 
                                             Password = password, 
                                             UserID = userName, 
                                             InitialCatalog = databaseName, 
                                             TrustServerCertificate = false
                                         };
            this.connectionString = this.connectionStringb.ConnectionString;
        }

        public SQLServerHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int InsertHwControllerMetadata(
            string externalId, string departmentName, string buildingName, string roomId, int floorNr)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionStringb.ConnectionString))
            {
                string queryString =
                    string.Format(
                        "INSERT INTO HwControllerMetadata (ExternalId, DepartmentName, BuildingName, RoomId, FloorNr) VALUES ('{0}','{1}','{2}','{3}','{4}');", 
                        externalId, 
                        departmentName, 
                        buildingName, 
                        roomId, 
                        floorNr);
                SqlCommand command = new SqlCommand(queryString, connection);
                return ExecuteSQLCommand(command);
            }
        }

        public int InsertSensorData(
            int sensorMetadataId, 
            int intermediateHwMetadataId, 
            string measuredData, 
            DateTime measuredAt, 
            DateTime polledAt, 
            int sensorType)
        {
            // using (SqlConnection connection = new SqlConnection(connectionStringb.ConnectionString))
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                string queryString =
                    string.Format(
                        "INSERT INTO SensorData (SensorMetadataId, IntermediateHwMedadataId, MeasuredData, MeasuredAt, SendAt, PolledAt, UpdatedAt, CreatedAt,  SensorType) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", 
                        sensorMetadataId, 
                        intermediateHwMetadataId, 
                        measuredData, 
                        measuredAt.ToLongDateString() + " " + measuredAt.ToLongTimeString(), 
                        measuredAt.ToLongDateString() + " " + measuredAt.ToLongTimeString(), 
                        polledAt.ToLongDateString() + " " + polledAt.ToLongTimeString(), 
                        measuredAt.ToLongDateString() + " " + measuredAt.ToLongTimeString(), 
                        measuredAt.ToLongDateString() + " " + measuredAt.ToLongTimeString(), 
                        sensorType);
                SqlCommand command = new SqlCommand(queryString, connection);
                return ExecuteSQLCommand(command);
            }
        }

        public int InsertSensorMetadata(
            string externalId, 
            string sensorName, 
            string roomId, 
            int floorNr, 
            float geoLongitude, 
            float geoLatidude, 
            int hwControllerMetadataId)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionStringb.ConnectionString))
            {
                string queryString =
                    string.Format(
                        "INSERT INTO SensorMetadata (ExternalId, SensorName, RoomId, FloorNr, GeoLongitude, GeoLatidude, HwControllerMetadataId) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", 
                        externalId, 
                        sensorName, 
                        roomId, 
                        floorNr, 
                        geoLongitude, 
                        geoLatidude, 
                        hwControllerMetadataId);
                SqlCommand command = new SqlCommand(queryString, connection);
                return ExecuteSQLCommand(command);
            }
        }

        private static int ExecuteSQLCommand(SqlCommand command)
        {
            StringBuilder errorMessages = new StringBuilder();

            try
            {
                command.Connection.Open();
                int res = command.ExecuteNonQuery();
                command.Connection.Close();
                return res;
            }
            catch (SqlException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append(
                        "Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: "
                                  + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n"
                                  + "Procedure: " + ex.Errors[i].Procedure + "\n");
                }

                Console.WriteLine(errorMessages.ToString());
                Console.Read();
                return 0;
            }
        }

        /*  public void InsertDataIntoStorage(SensorDataTest data)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable sensorDataTable = tableClient.GetTableReference("SensorData");

            TableOperation insertData = TableOperation.Insert(data);

            sensorDataTable.Execute(insertData);
            Console.WriteLine("Entity inserted");
        }*/
    }
}