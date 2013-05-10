﻿using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using OccupOSAPI.Models;
using System.Data.SqlClient;
using ServiceStack.Text;

namespace OccupOSAPI {
    [Route("/api/v1/Sensors", "GET")]
    [Route("/api/v1/Sensors/{Id}", "GET")]
    public class SensorDataReq {
        public int Id { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Period { get; set; }
    }


    public class SensorDataResp {
        public string measuredData { get; set; }
        public System.DateTime measuredAt { get; set; }
        public int sensorType { get; set; }
        public System.DateTime createdAt { get; set; }
    }

    [Route("/api/v1/Sensors", "POST")]
    public class SensorDataAdd {
        public int Id { get; set; }
        public int SensorMetadataId { get; set; }
        public int IntermediateHwMedadataId { get; set; }
        public string MeasuredData { get; set; }
        public System.DateTime MeasuredAt { get; set; }
        public System.Nullable<System.DateTime> SendAt { get; set; }
        public System.Nullable<System.DateTime> PolledAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int SensorType { get; set; }
    }

    public class Response {
        public SensorData SensorData { get; set; }
    }

    public class Tmp {
        public int Type { get; set; }
        public int Count { get; set; }
        public int URL { get; set; }
        public System.DateTime createdAt { get; set; }
    }

    public class JsonResp {
        public List<SensorDataResp> sensors { get; set; }
    }

    public class SensorDataService : Service {
        Random rand = new Random();
        List<SensorData> resp, response,response1;
        List<SensorDataResp> returnData;
        JsonResp lastResp;
        DateTime now;
        JsonResp tmpResp = new JsonResp();
        const int oneMinute = 6; // sensors upload data every 10 secs;
        const int oneHour = oneMinute * 60;
        const int oneDay = oneHour * 24;
        int timeInterval;
        int responseCount;
        public object Get(SensorDataReq request) {

            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            returnData = new List<SensorDataResp>();
            var connectionStringb = new SqlConnectionStringBuilder {
                DataSource =
                    "tcp:dndo40zalb.database.windows.net,1433",
                Encrypt = true,
                Password = "20041908kjH",
                UserID = "comp2014@dndo40zalb",
                InitialCatalog = "TestSQLDB",
                TrustServerCertificate = false
            };

            //All db access now uses the above dialect provider
            //var connectionString = "Data Source=tcp:dndo40zalb.database.windows.net,1433;Initial Catalog=TestSQLDB;User ID=comp2014@dndo40zalb;Password=20041908kjH;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;",  //Connection String

            var connectionString = "Data Source=DANS-PC; Database=OccupOS;Trusted_Connection=True;";
            //now = DateTime.Parse("2013-03-27 20:13:30.0000000");
            now = DateTime.Now;
            var user = "L";

            if (user.Equals("M")) {
                
                connectionString = "Data Source=(LocalDB)\\v11.0;Initial Catalog=OccupOSTest;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
            }

            if (user.Equals("A")) {
                //now = DateTime.Parse("2013-04-11 15:49:30.0000000");
                now = DateTime.Now;
                connectionString =
                    "Data Source=tcp:dndo40zalb.database.windows.net,1433;Initial Catalog=TestSQLDB;User ID=comp2014@dndo40zalb;Password=20041908kjH;Encrypt=True;TrustServerCertificate=False";
            }
            //System.Diagnostics.Debug.WriteLine(connectionString);

            //All db access now uses the above dialect provider
            var dbFactory = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);
            while(true)
            {
            try
            {
                using (IDbConnection db = dbFactory.OpenDbConnection())
                {
                    resp = db.Select<SensorData>();

                    // now = DateTime.Now;
                    //response = resp.OrderByDescending(x => x.MeasuredAt).ToList<SensorData>();
                    List<SensorData> tempor = new List<SensorData>();
                    switch (request.Period)
                    {
                        case 1:
                            response = resp.Where(x => x.MeasuredAt > now.AddHours(-1).AddDays(0) && x.MeasuredAt <= now && x.sensorType == 3).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response.Count;
                            for (int i = 0; i < responseCount; i += oneMinute)
                            {
                                tempor.Add(response.ElementAt(i));
                            }
                            response1 = resp.Where(x => x.MeasuredAt > now.AddHours(-1).AddDays(0) && x.MeasuredAt <= now && x.sensorType == 9).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response1.Count;
                            for (int i = 0; i < responseCount; i += oneMinute)
                            {
                                tempor.Add(response1.ElementAt(i));
                            }
                            response = tempor;
                            break;
                        case 2:
                            response = resp.Where(x => x.MeasuredAt > now.AddHours(-24) && x.MeasuredAt <= now && x.sensorType == 3).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response.Count;
                            for (int i = 0; i < responseCount; i += oneMinute * 10)
                            {
                                tempor.Add(response.ElementAt(i));
                                System.Diagnostics.Debug.WriteLine(response.ElementAt(i).MeasuredAt);
                            }
                            response1 = resp.Where(x => x.MeasuredAt > now.AddHours(-24) && x.MeasuredAt <= now && x.sensorType == 9).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response1.Count;
                            for (int i = 0; i < responseCount; i += oneMinute * 10)
                            {
                                tempor.Add(response1.ElementAt(i));
                            }
                            response = tempor;
                            break;
                        case 3:
                            response = resp.Where(x => x.MeasuredAt > now.AddDays(-7) && x.MeasuredAt <= now && x.sensorType == 3).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response.Count;
                            for (int i = 0; i < responseCount; i += oneHour * 3)
                            {
                                tempor.Add(response.ElementAt(i));
                            }
                            response1 = resp.Where(x => x.MeasuredAt > now.AddDays(-7) && x.MeasuredAt <= now && x.sensorType == 9).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response.Count;
                            for (int i = 0; i < responseCount; i += oneHour * 3)
                            {
                                tempor.Add(response1.ElementAt(i));
                            }
                            response = tempor;
                            break;
                        case 4:
                            response = resp.Where(x => x.MeasuredAt > now.AddMonths(-1) && x.MeasuredAt <= now && x.sensorType == 3).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response.Count;
                            for (int i = 0; i < responseCount; i += oneHour * 12)
                            {
                                tempor.Add(response.ElementAt(i));
                            }
                            response1 = resp.Where(x => x.MeasuredAt > now.AddMonths(-1) && x.MeasuredAt <= now && x.sensorType == 9).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response1.Count;
                            for (int i = 0; i < responseCount; i += oneHour * 12)
                            {
                                tempor.Add(response1.ElementAt(i));
                            }
                            response = tempor;
                            break;
                        case 5:
                            response = resp.Where(x => x.MeasuredAt > now.AddYears(-1) && x.MeasuredAt <= now && x.sensorType == 3).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response.Count;
                            for (int i = 0; i < responseCount; i += oneDay * 2)
                            {
                                tempor.Add(response.ElementAt(i));
                            }
                            response1 = resp.Where(x => x.MeasuredAt > now.AddYears(-1) && x.MeasuredAt <= now && x.sensorType == 9).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response1.Count;
                            for (int i = 0; i < responseCount; i += oneDay * 2)
                            {
                                tempor.Add(response1.ElementAt(i));
                            }
                            response = tempor;
                            break;
                        case 0:
                            response = resp.Where(x => x.MeasuredAt > now.AddMinutes(-10) && x.MeasuredAt <= now).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            break;
                        default:
                            response = resp.Where(x => x.MeasuredAt > now.AddHours(-1).AddDays(0) && x.MeasuredAt <= now && x.sensorType == 3).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response.Count;
                            for (int i = 0; i < responseCount; i += oneMinute)
                            {
                                tempor.Add(response.ElementAt(i));
                            }
                            response1 = resp.Where(x => x.MeasuredAt > now.AddHours(-1).AddDays(0) && x.MeasuredAt <= now && x.sensorType == 9).OrderBy(x => x.MeasuredAt).ToList<SensorData>();
                            responseCount = response1.Count;
                            for (int i = 0; i < responseCount; i += oneMinute)
                            {
                                tempor.Add(response1.ElementAt(i));
                            }
                            response = tempor;
                            break;
                    }
                    /*  if (request.Period == 1)
                      {
                    
                         // System.Diagnostics.Debug.WriteLine(now.AddHours(-1));
                    
                    
                      }
                      else
                          response = resp.Where(x => x.MeasuredAt > now.AddHours(-5).AddDays(0) && x.MeasuredAt <= now).OrderByDescending(x => x.MeasuredAt).ToList<SensorData>();
                      */
                    //  var tmp = response.GroupBy(x => x.SensorType).Select(g => new { Type=g.Key, Count =g.Count(),URL = g.Key.GetHashCode() });
                    // List<Tmp> tmp = response.GroupBy(x => x.SensorType).Select(g => new Tmp { Type = g.Key, Count = g.Count(), URL = g.Key.GetHashCode() }).ToList<Tmp>();
                    //  System.Diagnostics.Debug.WriteLine("Count: " + response.Count);
                    /*    if (urls == null)
                        {
                            foreach(Tmp row in tmp)
                            {
                      }
                  }*/

                    // int count = responseCount;
                    if (request.Id > 0)
                    {
                        response = response.Where<SensorData>(x => x.SensorMetadataId == request.Id).ToList<SensorData>();
                    }
                    if (request.Offset > 0)
                    {
                        response = response.OrderBy(x => x.MeasuredAt).Take(responseCount - request.Offset).ToList<SensorData>();
                    }

                    if (request.From.Year > 1)
                    {
                        response = response.Where<SensorData>(x => x.MeasuredAt.CompareTo(request.From) > 0).ToList<SensorData>();
                    }
                    if (request.To.Year > 1)
                    {
                        response = response.Where<SensorData>(x => x.MeasuredAt.CompareTo(request.To) < 0).ToList<SensorData>();
                    }
                    if (request.Limit > 0)
                    {
                        if (request.Limit == 1)
                        {
                            List<Tmp> tmp = response.GroupBy(x => x.sensorType).Select(g => new Tmp { Type = g.Key, Count = g.Count() }).ToList<Tmp>();
                            foreach (Tmp t in tmp)
                            {
                                List<SensorData> temp = response.OrderByDescending(x => x.MeasuredAt).Where(x => x.sensorType == t.Type).Take(1).ToList<SensorData>();
                                SensorDataResp value = new SensorDataResp();
                                value.measuredData = temp[0].measuredData;
                                value.createdAt = temp[0].CreatedAt;
                                value.measuredAt = temp[0].MeasuredAt;
                                value.sensorType = temp[0].sensorType;
                                returnData.Add(value);
                            }
                            tmpResp.sensors = returnData;
                            tmpResp.ToJson<JsonResp>();
                            return new HttpResult(tmpResp, ContentType.Json);
                        }
                        else
                        {
                            response = response.OrderByDescending(x => x.MeasuredAt).Take(request.Limit).ToList<SensorData>();
                        }
                    }
                    foreach (SensorData tmp in response)
                    {
                        SensorDataResp value = new SensorDataResp();
                        value.measuredData = tmp.measuredData;
                        value.measuredAt = tmp.MeasuredAt;
                        value.sensorType = tmp.sensorType;
                        value.createdAt = tmp.CreatedAt;
                        returnData.Add(value);
                    }
                    tmpResp.sensors = returnData;
                    //     tmpResp.ToJson<JsonResp>();
                    
                    // return tmpResp;
                    return new HttpResult(tmpResp, ContentType.Json);
                }

            }
            catch (SqlException ex)
            {
                lastResp = new JsonResp();
                SensorDataResp value = new SensorDataResp();
                value.measuredData = "NaN";
                value.measuredAt = DateTime.Now;
                value.sensorType = 0;
                value.createdAt = DateTime.Now;
                returnData.Add(value);
                lastResp.sensors = returnData;
                return new HttpResult(lastResp, ContentType.Json);
            }
        }
        }
    }

    public class SensorDataAddService : Service {
        public object POST(SensorDataAdd request) {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            long id;
            var dbFactory = new OrmLiteConnectionFactory(
                //  "Data Source=tcp:dndo40zalb.database.windows.net,1433;Initial Catalog=TestSQLDB;User ID=comp2014@dndo40zalb;Password=20041908kjH;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;",  //Connection String
                "Data Source=DANS-PC; Database=OccupOS;Trusted_Connection=True;",
                 SqlServerDialect.Provider);
            using (IDbConnection db = dbFactory.OpenDbConnection()) {
                SensorData data = new SensorData { MeasuredAt = request.MeasuredAt, measuredData = request.MeasuredData, CreatedAt = request.CreatedAt, IntermediateHwMedadataId = request.IntermediateHwMedadataId, PolledAt = request.PolledAt, SendAt = request.SendAt, SensorMetadataId = request.SensorMetadataId, sensorType = request.SensorType, UpdatedAt = request.UpdatedAt };
                db.Insert<SensorData>(data);
                id = db.GetLastInsertId();

            }
            return new HttpResult(id, ContentType.Json);

        }
    }
}