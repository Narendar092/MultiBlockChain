using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using XISystem;

namespace XIDataBase
{
    public class XIDBMongoDB
    {
        //Column Names and Data are Case sensitive and DataType sensitive

        //Single Condition
        //var Builder = Builders<BsonDocument>.Filter.Eq("sName", "Test");


        //Multiple Condition
        //var Builder = Builders<BsonDocument>.Filter;
        //var Filter = Builder.Eq("sName", "Test") & Builder.Eq("iStatus", 10);

        public string sServer { get; set; }
        public string sDatabase { get; set; }
        public string sTable { get; set; }
        public string sUID { get; set; }
        public List<CNV> oWhrParams { get; set; }
        public string sPrimaryKey { get; set; }
        public CResult Get_Data()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                var Connection = new MongoClient(sServer);
                var Database = Connection.GetDatabase(sDatabase);
                var collection = Database.GetCollection<BsonDocument>(sTable);
                var Data = new List<BsonDocument>(); //collection.Find(new BsonDocument()).ToList();
                if (!string.IsNullOrEmpty(sPrimaryKey))
                {
                    var filter = Builders<BsonDocument>.Filter;
                    var build = filter.Eq(sPrimaryKey.ToLower(), sUID);
                    Data = collection.Find(build).ToList();
                }
                else
                {
                    Data = collection.Find(new BsonDocument()).ToList();
                }
                oCResult.oResult = Data;
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public async Task<CResult> Insert_Data(string Data)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                var Connection = new MongoClient(sServer);
                var Database = Connection.GetDatabase(sDatabase);
                var collection = Database.GetCollection<BsonDocument>(sTable);
                var bsonDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(Data);
                await collection.InsertOneAsync(bsonDoc);
                oCResult.oResult = "Success";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Update_Data(List<CNV> Attrs)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                var Connection = new MongoClient(sServer);
                var Database = Connection.GetDatabase(sDatabase);
                var collection = Database.GetCollection<BsonDocument>(sTable);
                if (!string.IsNullOrEmpty(sPrimaryKey) && !string.IsNullOrEmpty(sUID))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq(sPrimaryKey.ToLower(), sUID);                    
                    foreach (var attr in Attrs)
                    {
                        var update = Builders<BsonDocument>.Update.Set(attr.sName, attr.sValue);
                        collection.UpdateOne(filter, update);
                    }
                    oCResult.oResult = "Success";
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}