using OccupOSAPI.Models;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OccupOSMonitorNew.ServiceInterfaces
{
    [Route("/temperatures")]
    public class TemperatureRequest
    {
        public int Id {get;set;}
    }

    public class TemperatureService : Service
    {
        List<SensorData> resp, response;
        public object GET(TemperatureRequest request)
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            var dbFactory = new OrmLiteConnectionFactory(
            //"Data Source=tcp:dndo40zalb.database.windows.net,1433;Initial Catalog=TestSQLDB;User ID=comp2014@dndo40zalb;Password=20041908kjH;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;",  //Connection String
                "Data Source=DANS-PC; Database=OccupOS;Trusted_Connection=True;",
            SqlServerDialect.Provider);
            using (IDbConnection db = dbFactory.OpenDbConnection())
            {
                resp = db.Select<SensorData>();
                response = resp.ToList<SensorData>();
                response = response.Where<SensorData>(x => x.SensorType == 3).ToList<SensorData>();
            }
            return response;
        }
    }
}