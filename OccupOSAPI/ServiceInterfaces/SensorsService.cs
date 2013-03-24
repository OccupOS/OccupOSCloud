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

namespace OccupOSAPI
{
    [Route("/api/v1/Sensors")]
    [Route("/api/v1/Sensors/{Id}")]
    public class SensorDataReq
    {
        public int Id { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

    }


    public class SensorDataResp
    {
        public int Id {get;set;}
        public int SensorMetadataId { get; set; }
        public int IntermediateHwMedadataId { get; set; }
        public string MeasuredData { get; set; }
        public System.DateTime MeasuredAt { get; set; }
    }

    public class Response
    {
       public SensorData SensorData { get; set; }
    }

    public class Tmp
    {
       public int Type { get; set; }
       public int Count { get; set; }
       public int URL { get; set; }
    }

    public class SensorDataService : Service
    {
        Random rand = new Random();
        List<SensorData> resp,response;
        Dictionary<int, int> urls = null;
        public object Get(SensorDataReq request)
        {         
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            var connectionStringb = new SqlConnectionStringBuilder
            {
                DataSource =
                    "tcp:dndo40zalb.database.windows.net,1433",
                Encrypt = true,
                Password = "20041908kjH",
                UserID = "comp2014@dndo40zalb",
                InitialCatalog = "TestSQLDB",
                TrustServerCertificate = false
            };
            
         //All db access now uses the above dialect provider
            var dbFactory = new OrmLiteConnectionFactory(
  //  "Data Source=tcp:dndo40zalb.database.windows.net,1433;Initial Catalog=TestSQLDB;User ID=comp2014@dndo40zalb;Password=20041908kjH;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;",  //Connection String
  "Data Source=DANS-PC; Database=OccupOS;Trusted_Connection=True;",
  SqlServerDialect.Provider);
            using (IDbConnection db = dbFactory.OpenDbConnection())
          {
              resp = db.Select<SensorData>();
              
              response = resp.ToList<SensorData>();
            //  var tmp = response.GroupBy(x => x.SensorType).Select(g => new { Type=g.Key, Count =g.Count(),URL = g.Key.GetHashCode() });
              List<Tmp> tmp = response.GroupBy(x => x.SensorType).Select(g => new Tmp { Type = g.Key, Count = g.Count(), URL = g.Key.GetHashCode() }).ToList<Tmp>();
              System.Diagnostics.Debug.WriteLine("Count: " + "one".GetHashCode());
              if (urls == null)
              {
                  foreach(Tmp row in tmp)
                  {
                      
                  }
              }
              
              int count = response.Count;
              if (request.Id > 0)
              {
                  response = response.Where<SensorData>(x => x.SensorMetadataId == request.Id).ToList<SensorData>();   
              }
              if (request.Offset > 0)
              {
                  response = response.OrderBy(x => x.MeasuredAt).Take(count - request.Offset).ToList<SensorData>();
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
                  response = response.OrderByDescending(x => x.MeasuredAt).Take(request.Limit).ToList<SensorData>();
              }
              return new HttpResult(tmp, ContentType.Json);      
          }
        }
    }
}