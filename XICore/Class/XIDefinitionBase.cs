using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using XISystem;
using XIDatabase;
using System.Net;
using System.Configuration;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Org.BouncyCastle.Ocsp;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Data.SqlTypes;
using System.Collections;

namespace XICore
{
    public class XIDefinitionBase
    {
        private iSiganlR oSignalR = null;
        public XIDefinitionBase()
        {

        }
        public XIDefinitionBase(iSiganlR oSignalRI)
        {
            oSignalR = oSignalRI;
        }
        public int StatusTypeID { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime
        {
            get
            { return DateTime.Now; }
            set { }
        }
        public int UpdatedBy { get; set; }
        public string UpdatedBySYSID { get; set; }
        public DateTime UpdatedTime
        {
            get
            { return DateTime.Now; }
            set { }
        }
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        private object oMyParent; // CIBO
        public object oParent
        {
            get
            {
                return oMyParent;
            }
            set
            {
                oMyParent = value;
            }
        }// CIBO

        // ALERT - we don't need this as you can get through BOI, but a quick reference here for short
        private XIDBO oMyBOD;
        public XIDBO BOD
        {
            get
            {
                return oMyBOD;
            }
            set
            {
                oMyBOD = value;
            }
        }


        private XIIBO oMyBOI;
        public XIIBO BOI
        {
            get
            {
                return oMyBOI;
            }
            set
            {
                oMyBOI = value;
            }
        }

        private object oMyDefinition; // CIBO
        public object oDefintion
        {
            get
            {
                return oMyDefinition;
            }
            set
            {
                oMyDefinition = value;
            }
        }

        private Dictionary<string, object> oMyContent = new Dictionary<string, object>();

        public Dictionary<string, object> oContent
        {
            get
            {
                return oMyContent;
            }
            set
            {
                oMyContent = value;
            }
        }
        public object Clone(object obj)
        {
            try
            {
                if (obj == null)
                    return null;
                Type type = obj.GetType();

                if (type.IsValueType || type == typeof(string))
                {
                    return obj;
                }
                else if (type.IsArray)
                {
                    Type elementType = Type.GetType(
                         type.FullName.Replace("[]", string.Empty));
                    if (elementType != null)
                    {
                        var array = obj as Array;
                        Array copied = Array.CreateInstance(elementType, array.Length);
                        for (int i = 0; i < array.Length; i++)
                        {
                            copied.SetValue(Clone(array.GetValue(i)), i);
                        }
                        return Convert.ChangeType(copied, obj.GetType());
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (type.IsClass)
                {

                    object toret = Activator.CreateInstance(obj.GetType());
                    FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                                BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (FieldInfo field in fields)
                    {
                        object fieldValue = field.GetValue(obj);
                        if (fieldValue == null)
                            continue;
                        field.SetValue(toret, Clone(fieldValue));
                    }
                    return toret;
                }
                else
                    throw new ArgumentException("Unknown type");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public T DeepCopy<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public void SaveErrortoDB(CResult oCResult, int iQSInstanceID = 0, int iPolicyID = 0)
        {
            try
            {
                var iApplicationID = 0;
                int iUserID = 0;
                string sUser = string.Empty;
                XIInfraCache oCache = new XIInfraCache();
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    var sAppID = string.Empty;
                    var sApplicationID = HttpContext.Current.Session["ApplicationID"];
                    if (sApplicationID != null)
                    {
                        sAppID = sApplicationID.ToString();
                    }
                    int.TryParse(sAppID, out iApplicationID);
                    if (HttpContext.Current.Session["UserID"] != null)
                        int.TryParse(HttpContext.Current.Session["UserID"].ToString(), out iUserID);
                    if (iUserID > 0)
                    {
                        sUser = HttpContext.Current.Session["sUserName"].ToString();
                    }
                    if (string.IsNullOrEmpty(sUser))
                    {
                        sUser = "Public without Login";
                    }
                }

                //check application log level  and user log level caching
                //Need to think how to reset these values on the fly without clearing full cache / user relogin.
                //EnumXIErrorPriority { Low = 10, Medium = 20, Critical = 30 }
                int defaultAppLogLevel = 30; int defaultUserLogLevel = 30;
                String appLogLevel = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "ApplicationLogLevel");
                //appLogLevel = appLogLevel.ToLower();
                if (!int.TryParse(appLogLevel, out defaultAppLogLevel))
                {
                    defaultAppLogLevel = 30; //if parse fails
                }
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    String userLogLevel = HttpContext.Current.Session["iLogLevel"] == null ? "30" : HttpContext.Current.Session["iLogLevel"].ToString();

                    if (!int.TryParse(userLogLevel, out defaultUserLogLevel))
                    {
                        defaultUserLogLevel = 30; //if parse fails
                    }
                }
                int finalLogLevel = Math.Min(defaultAppLogLevel, defaultUserLogLevel);
                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ErrorLog");
                //EnumXIErrorPriority { Low = 10, Medium = 20, Critical = 30 }
                if (oCResult.iLogLevel >= finalLogLevel)
                {
                    //XIInfraCache oCache = new XIInfraCache();
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = BOD;
                    oBOI.SetAttribute("FKiApplicationID", iApplicationID.ToString());
                    oBOI.SetAttribute("sCategory", oCResult.sCategory);
                    oBOI.SetAttribute("iType", oCResult.iType.ToString());
                    oBOI.SetAttribute("iCriticality", oCResult.iCriticality.ToString());
                    oBOI.SetAttribute("CreatedTime", DateTime.Now.ToString());
                    oBOI.SetAttribute("Description", oCResult.sMessage);
                    oBOI.SetAttribute("CreatedBySYSID", Dns.GetHostAddresses(Dns.GetHostName())[1].ToString());
                    oBOI.SetAttribute("TypeID", "20");
                    if (oCResult.sMessage.ToLower().StartsWith("error"))
                    {
                        oBOI.SetAttribute("TypeID", "10");
                    }
                    oBOI.SetAttribute("sCode", oCResult.sCode);
                    oBOI.SetAttribute("FKiQSInstanceID", iQSInstanceID.ToString());
                    oBOI.SetAttribute("FKiPolicyID", iPolicyID.ToString());
                    oBOI.SetAttribute("CreatedByID", iUserID.ToString());
                    oBOI.SetAttribute("CreatedByName", sUser);


                    oBOI.SetAttribute("TypeID", oCResult.iLogLevel.ToString());
                    if (!string.IsNullOrEmpty(oCResult.sMessage))
                        oBOI.Save(oBOI);
                }
                //if (iApplicationID == 76)
                //{
                //    var oCR = XIMonitor(oCResult, iApplicationID);
                //}


                if (oCResult.oTraceStack != null && oCResult.oTraceStack.Count() > 0)
                {
                    string sMessage = string.Empty;
                    if (iQSInstanceID > 0)
                    {
                        sMessage = "[QSInstanceID : " + iQSInstanceID + " ] - ";
                    }
                    if (iPolicyID > 0)
                    {
                        sMessage += "[PolicyID : " + iPolicyID + " ] - ";
                    }
                    var Messages = oCResult.oTraceStack.Select(m => m.sValue).ToArray();
                    var Msg = "TraceStack: " + sMessage + string.Join("->", Messages);
                    XIIBO oTrace = new XIIBO();
                    oTrace.BOD = BOD;
                    oTrace.SetAttribute("FKiApplicationID", iApplicationID.ToString());
                    oTrace.SetAttribute("sCategory", oCResult.sCategory);
                    oTrace.SetAttribute("CreatedTime", DateTime.Now.ToString());
                    oTrace.SetAttribute("Description", Msg);
                    oTrace.SetAttribute("FKiQSInstanceID", iQSInstanceID.ToString());
                    oTrace.SetAttribute("FKiPolicyID", iPolicyID.ToString());
                    oTrace.SetAttribute("CreatedBySYSID", Utility.GetIPAddress());
                    oTrace.SetAttribute("CreatedByID", iUserID.ToString());
                    oTrace.SetAttribute("CreatedByName", sUser);
                    oTrace.SetAttribute("TypeID", "20");
                    if (oCResult.sMessage.ToLower().StartsWith("error"))
                    {
                        oTrace.SetAttribute("TypeID", "10");
                    }
                    if (!string.IsNullOrEmpty(Msg))
                        oTrace.Save(oTrace);
                }

                //XIErrorLogs ELog = new XIErrorLogs();
                //ELog.CreatedTime = DateTime.Now;
                //ELog.Description = oCResult.sMessage;
                //ELog.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                //ELog.TypeID = 20;
                //ELog.FKiQSInstanceID = iQSInstanceID;
                //ELog.FKiPolicyID = iPolicyID;
                //if (oCResult.sMessage.ToLower().StartsWith("error"))
                //{
                //    ELog.TypeID = 10;
                //}
                //ELog.sCode = oCResult.sCode;
                //if (!string.IsNullOrEmpty(ELog.Description))
                //{
                //    Connection.Insert<XIErrorLogs>(ELog, "XIErrorLog_T", "ID");
                //}
                //if (oCResult.oTraceStack != null && oCResult.oTraceStack.Count() > 0)
                //{
                //    string sMessage = string.Empty;
                //    if (iQSInstanceID > 0)
                //    {
                //        sMessage = "[QSInstanceID : " + iQSInstanceID + " ] - ";
                //    }
                //    if (iPolicyID > 0)
                //    {
                //        sMessage += "[PolicyID : " + iPolicyID + " ] - ";
                //    }
                //    var Messages = oCResult.oTraceStack.Select(m => m.sValue).ToArray();
                //    var Msg = "TraceStack: " + sMessage + string.Join("->", Messages);
                //    XIErrorLogs Trace = new XIErrorLogs();
                //    Trace.CreatedTime = DateTime.Now;
                //    Trace.Description = Msg;
                //    Trace.FKiQSInstanceID = iQSInstanceID;
                //    Trace.FKiPolicyID = iPolicyID;
                //    Trace.CreatedBySYSID = Utility.GetIPAddress();
                //    if (HttpContext.Current != null)
                //    {
                //        int iUserID = 0;
                //        int.TryParse(HttpContext.Current.Session["UserID"].ToString(), out iUserID);
                //        string sUser = string.Empty;
                //        if (iUserID > 0)
                //        {
                //            sUser = HttpContext.Current.Session["sUserName"].ToString();
                //        }
                //        if (string.IsNullOrEmpty(sUser))
                //        {
                //            sUser = "Public without Login";
                //        }
                //        Trace.CreatedByID = iUserID;
                //        Trace.CreatedByName = sUser;
                //    }
                //    Trace.TypeID = 20;
                //    if (oCResult.sMessage.ToLower().StartsWith("error"))
                //    {
                //        Trace.TypeID = 10;
                //    }
                //    if (!string.IsNullOrEmpty(Trace.Description))
                //        Connection.Insert<XIErrorLogs>(Trace, "XIErrorLog_T", "ID");
                //}
            }
            catch (Exception ex)
            {
                XIErrorLogs Trace = new XIErrorLogs();
                if (oCResult.oTraceStack != null && oCResult.oTraceStack.Count() > 0)
                {
                    string sMessage = string.Empty;
                    if (iQSInstanceID > 0)
                    {
                        sMessage = "[QSInstanceID : " + iQSInstanceID + " ] - ";
                    }
                    if (iPolicyID > 0)
                    {
                        sMessage += "[PolicyID : " + iPolicyID + " ] - ";
                    }
                    var Messages = oCResult.oTraceStack.Select(m => m.sValue).ToArray();
                    var Msg = "TraceStack: " + sMessage + string.Join("->", Messages);
                    Trace.CreatedTime = DateTime.Now;
                    Trace.Description = Msg;
                    Trace.FKiQSInstanceID = iQSInstanceID;
                    Trace.FKiPolicyID = iPolicyID;
                    Trace.CreatedBySYSID = Utility.GetIPAddress();
                    if (!string.IsNullOrEmpty(Trace.Description))
                        Connection.Insert<XIErrorLogs>(Trace, "XIErrorLog_T", "ID");
                }
                Trace.Description = "Critial ERROR: Exception in SaveErrortoDB [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                Trace.CreatedTime = DateTime.Now;
                Trace.CreatedBySYSID = Utility.GetIPAddress();
                Trace.TypeID = 20;
                Trace.FKiQSInstanceID = iQSInstanceID;
                Trace.FKiPolicyID = iPolicyID;
                if (oCResult.sMessage.ToLower().StartsWith("error"))
                {
                    Trace.TypeID = 10;
                }
                //Connection.Insert<XIErrorLogs>(Trace, "XIErrorLog_T", "ID");
            }

        }

        public CResult Log_Performance(string sMethod, double iLapsedTime, Guid DefXIGUID, int iType)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Log the performance of a method";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIBO oBOI = new XIIBO();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "xiperformance");
                oBOI.BOD = oBOD;
                oBOI.SetAttribute("sName", sMethod);
                oBOI.SetAttribute("iLapsedTime", iLapsedTime.ToString());
                oBOI.SetAttribute("FKiDefIDXIGUID", DefXIGUID.ToString());
                oBOI.SetAttribute("iType", iType.ToString());
                oCR = oBOI.Save(oBOI);
                if (oCR.bOK && oCR.oResult != null)
                {
                    Cache_Performance(sMethod, iLapsedTime, DefXIGUID, iType);
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Success";
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Cache_Performance(string sName, double iLapsedTime, Guid DefXIGUID, int iType)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Log the performance of a method";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var sCacheperformance = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "Cache performance");
                if (sCacheperformance == "on")
                {
                    XIIXI oXI = new XIIXI();
                    XIIBO oBOI = new XIIBO();
                    var sAppName = string.Empty;// HttpContext.Current.Session["AppName"].ToString();// Singleton.Instance.sAppName;
                    if (HttpContext.Current != null && HttpContext.Current.Session != null)
                    {
                        if (HttpContext.Current.Session["AppName"] != null)
                        {
                            sAppName = HttpContext.Current.Session["AppName"].ToString();
                        }
                    }
                    var sCacheperformancedate = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "Cache performance date");
                    if (string.IsNullOrEmpty(sCacheperformancedate))
                    {
                        //Set to today on the config
                        List<CNV> oWhr = new List<CNV>();
                        oWhr.Add(new CNV { sName = "sKey", sValue = "Cache performance date" });
                        oBOI = oXI.BOI("XIConfig_T", null, null, oWhr);
                        if (oBOI != null && oBOI.Attributes.Count() > 0)
                        {
                            oBOI.SetAttribute("svalue", DateTime.Now.Date.ToString("dd-MM-yyyy"));
                            oBOI.Save(oBOI);
                            var sKey = sAppName + "_Definition_configsetting_Cache performance date";
                            HttpRuntime.Cache.Remove(sKey);
                        }
                    }
                    else
                    {
                        var dDate = oBOI.ConvertToDtTime(sCacheperformancedate).Date.ToString("dd-MM-yyyy");
                        if (dDate != DateTime.Now.Date.ToString("dd-MM-yyyy"))
                        {
                            //Write cache performance of all stats to DB
                            oCR = Save_CachePerformaceData();
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                //Clear the perfromance cache information
                                string sKey = sAppName.Replace(" ", "") + "_Definition_Performance_";
                                oCache.Clear_PerformanceCache(sKey);

                                //Set date to today on the config
                                List<CNV> oWhr = new List<CNV>();
                                oWhr.Add(new CNV { sName = "sKey", sValue = "Cache performance date" });
                                oBOI = oXI.BOI("XIConfig_T", null, null, oWhr);
                                if (oBOI != null && oBOI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("svalue", DateTime.Now.Date.ToString("dd-MM-yyyy"));
                                    oBOI.Save(oBOI);
                                    sKey = sAppName + "_Definition_configsetting_Cache performance date";
                                    HttpRuntime.Cache.Remove(sKey);
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sName))
                    {
                        int iCalls = 1;
                        double iAverage = 0;
                        XIPerformance oPerformance = new XIPerformance();
                        string sCacheKey = string.Empty;
                        if (!string.IsNullOrEmpty(sAppName))
                        {
                            sCacheKey = sCacheKey + sAppName.Replace(" ", "") + "_Definition_Performance_";
                        }
                        if (DefXIGUID != null && DefXIGUID != Guid.Empty)
                            sCacheKey = sCacheKey + sName + "_" + DefXIGUID;
                        else
                            sCacheKey = sCacheKey + sName;
                        oPerformance = (XIPerformance)oCache.GetFromCache(sCacheKey);
                        string sCode = string.Empty;
                        switch (iType)
                        {
                            case 10:
                                sCode = "Process controller";
                                break;
                            case 20:
                                sCode = "XILink";
                                break;
                            case 30:
                                sCode = "1Query";
                                break;
                            case 40:
                                sCode = "Tab switch";
                                break;
                            case 50:
                                sCode = "Process controller line";
                                break;
                            case 100:
                                sCode = "Method";
                                break;
                        }
                        if (oPerformance == null)
                        {
                            oPerformance = new XIPerformance();
                            oPerformance.sName = sName;
                            oPerformance.FKiDefIDXIGUID = DefXIGUID;
                            oPerformance.iCalls = 1;
                            oPerformance.iAverage = iLapsedTime;
                            oPerformance.iMaxMS = iLapsedTime;
                            oPerformance.iMinMS = iLapsedTime;
                            oPerformance.iType = iType;
                            oPerformance.sCode = sCode;
                            oCache.InsertIntoCache(oPerformance, sCacheKey);
                        }
                        else
                        {
                            iCalls = oPerformance.iCalls;
                            iAverage = oPerformance.iAverage;
                            if (iLapsedTime > oPerformance.iMaxMS)
                            {
                                oPerformance.iMaxMS = iLapsedTime;
                            }
                            if (iLapsedTime < oPerformance.iMinMS)
                            {
                                oPerformance.iMinMS = iLapsedTime;
                            }
                            iCalls++;
                            //double iTotalMS = iAverage * iCalls;
                            //iTotalMS = iTotalMS + iLapsedTime;
                            //iAverage = iTotalMS / iCalls;
                            iAverage = iAverage * (iCalls - 1) / iCalls + iLapsedTime / iCalls;
                            oPerformance.iCalls = iCalls;
                            oPerformance.iAverage = iAverage;
                            oPerformance.sCode = sCode;
                            oCache.InsertIntoCache(oPerformance, sCacheKey);
                        }
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = "Success";
                    }
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Save_CachePerformaceData()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Log the performance of a method";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var sAppName = string.Empty;// HttpContext.Current.Session["AppName"].ToString();// Singleton.Instance.sAppName;
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["AppName"] != null)
                    {
                        sAppName = HttpContext.Current.Session["AppName"].ToString();
                    }
                }
                string sKey = sAppName.Replace(" ", "") + "_Definition_Performance_";
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIPerformanceCache");
                XIIXI oXI = new XIIXI();
                var oCurrentCache = System.Web.HttpContext.Current.Cache;
                if (oCurrentCache != null)
                {
                    var oCachedEnumr = oCurrentCache.GetEnumerator();
                    while (oCachedEnumr.MoveNext())
                    {
                        if (oCachedEnumr.Key.ToString().ToLower().StartsWith(sKey.ToLower()))
                        {
                            var oPrefObj = (XIPerformance)oCachedEnumr.Value;
                            List<CNV> oWhrPrms = new List<CNV>();
                            oWhrPrms.Add(new CNV { sName = "sName", sValue = oPrefObj.sName });
                            if (oPrefObj.FKiDefIDXIGUID != null && oPrefObj.FKiDefIDXIGUID != Guid.Empty)
                                oWhrPrms.Add(new CNV { sName = "FKiDefIDXIGUID", sValue = oPrefObj.FKiDefIDXIGUID.ToString() });
                            XIIBO oBOI = new XIIBO();
                            oBOI = oXI.BOI("XIPerformanceCache", null, null, oWhrPrms);
                            if (oBOI != null && oBOI.Attributes.Count() > 0)
                            {
                                oBOI.SetAttribute("iCalls", oPrefObj.iCalls.ToString());
                                oBOI.SetAttribute("iAverage", oPrefObj.iAverage.ToString());
                                oBOI.SetAttribute("iMaxMS", oPrefObj.iMaxMS.ToString());
                                oBOI.SetAttribute("iMinMS", oPrefObj.iMinMS.ToString());
                                oCR = oBOI.Save(oBOI);
                            }
                            else
                            {
                                oBOI = new XIIBO();
                                oBOI.BOD = oBOD;
                                oBOI.SetAttribute("sName", oPrefObj.sName);
                                if (oPrefObj.FKiDefIDXIGUID != null && oPrefObj.FKiDefIDXIGUID != Guid.Empty)
                                    oBOI.SetAttribute("FKiDefIDXIGUID", oPrefObj.FKiDefIDXIGUID.ToString());
                                oBOI.SetAttribute("iType", oPrefObj.iType.ToString());
                                oBOI.SetAttribute("sCode", oPrefObj.sCode.ToString());
                                oBOI.SetAttribute("iCalls", oPrefObj.iCalls.ToString());
                                oBOI.SetAttribute("iAverage", oPrefObj.iAverage.ToString());
                                oBOI.SetAttribute("iMaxMS", oPrefObj.iMaxMS.ToString());
                                oBOI.SetAttribute("iMinMS", oPrefObj.iMinMS.ToString());
                                oBOI.SetAttribute("dDate", DateTime.Now.Date.ToString("dd-MM-yyyy"));
                                oCR = oBOI.Save(oBOI);
                            }
                        }
                    }
                }
                oCResult.oResult = "Success";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult XIMonitor(CResult oResult, int iApplicationID)
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
                XIInfraCache oCache = new XIInfraCache();
                var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "XIMonitorByApp");
                var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                List<CNV> nParams = new List<CNV>();
                nParams.Add(new CNV { sName = "{XIP|iAppID}", sValue = iApplicationID.ToString() });
                nParams.Add(new CNV { sName = "{XIP|iType}", sValue = oResult.iType.ToString() });
                o1ClickC.ReplaceFKExpressions(nParams);
                var oRes = o1ClickC.OneClick_Run();
                if (oRes != null && oRes.Values.Count() > 0)
                {
                    var iAction = 0;
                    foreach (var items in oRes.Values.ToList())
                    {
                        var sAction = items.AttributeI("iAction").sValue;
                        var sEmail = items.AttributeI("sEmail").sValue;
                        int.TryParse(sAction, out iAction);
                        if (iAction > 0)
                        {
                            if (iAction == (int)xiEnumSystem.EnumXIMonitorAction.SignalR)
                            {
                                if (oSignalR != null)
                                {
                                    oSignalR.ShowSignalRMsg(oResult.sMessage);
                                }
                            }
                            else if (iAction == (int)xiEnumSystem.EnumXIMonitorAction.Email)
                            {
                                XIInfraEmail oEmail = new XIInfraEmail();
                                oEmail.EmailID = sEmail;
                                oEmail.sSubject = "Regulation Exception";
                                string sContent = "<h4>Alert!!! Sanction Exception</h4><p>we found the sanction activity</p><br/><p>Please check the log for more details</p>";
                                oEmail.Sendmail(5, sContent, null);
                            }
                            else if (iAction == (int)xiEnumSystem.EnumXIMonitorAction.SMS)
                            {

                            }
                        }
                    }
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

    public static class ObjectClone
    {
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
    public static class ObjectCloner
    {
        public static T DeepClone<T>(T obj)
        {
            if (obj == null) return default(T);
            return (T)DeepCloneObject(obj, new Dictionary<object, object>());
        }

        private static object DeepCloneObject(object obj, IDictionary<object, object> visited)
        {
            try
            {


                if (obj == null) return null;

                var type = obj.GetType();

                // Handle primitive types, strings, enums, and nullables
                if (type.IsValueType || type == typeof(string) || type.IsEnum)
                {
                    return obj;
                }

                // Handle circular references
                if (visited.ContainsKey(obj))
                {
                    return visited[obj];
                }

                // Handle arrays
                if (type.IsArray)
                {
                    var elementType = type.GetElementType();
                    var array = obj as Array;
                    if (array == null || elementType == null) return null;

                    var clonedArray = Array.CreateInstance(elementType, array.Length);
                    visited[obj] = clonedArray;

                    for (var i = 0; i < array.Length; i++)
                    {
                        clonedArray.SetValue(DeepCloneObject(array.GetValue(i), visited), i);
                    }
                    return clonedArray;
                }

                // Handle dictionaries
                if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    var clonedDictionary = Activator.CreateInstance(type) as IDictionary;
                    visited[obj] = clonedDictionary;

                    var dictionary = (IDictionary)obj;
                    foreach (var key in dictionary.Keys)
                    {
                        var clonedKey = DeepCloneObject(key, visited);
                        var clonedValue = DeepCloneObject(dictionary[key], visited);
                        clonedDictionary.Add(clonedKey, clonedValue);
                    }

                    return clonedDictionary;
                }

                // Handle collections
                if (typeof(IEnumerable).IsAssignableFrom(type) && !typeof(string).IsAssignableFrom(type))
                {
                    var clonedCollection = Activator.CreateInstance(type) as IList;
                    visited[obj] = clonedCollection;

                    var enumerable = (IEnumerable)obj;
                    foreach (var item in enumerable)
                    {
                        var clonedItem = DeepCloneObject(item, visited);
                        clonedCollection?.Add(clonedItem);
                    }

                    return clonedCollection;
                }

                // Handle custom objects
                var clone = Activator.CreateInstance(type);
                visited[obj] = clone;

                // Clone fields
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var fieldValue = field.GetValue(obj);
                    field.SetValue(clone, DeepCloneObject(fieldValue, visited));
                }

                // Clone properties
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (prop.CanRead && prop.CanWrite)
                    {
                        var propValue = prop.GetValue(obj);
                        prop.SetValue(clone, DeepCloneObject(propValue, visited));
                    }
                }

                return clone;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }


}