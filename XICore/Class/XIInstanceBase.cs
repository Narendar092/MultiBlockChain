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
using XISystem;
using System.Net;
using System.Configuration;
using XIDatabase;
using System.Web;

namespace XICore
{
    public class XIInstanceBase
    {
        private object oMyParent; // XIIBO
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
        }// CDBO

        public object CopyMe()
        {
            XIInstanceBase oNewMe = new XIInstanceBase();
            XIIQS oQSCopy = new XIIQS();
            return oQSCopy;
        }

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

        private List<XIVisualisation> oMyVisualisation;
        public List<XIVisualisation> oVisualisation
        {
            get
            {
                return oMyVisualisation;
            }
            set
            {
                oMyVisualisation = value;
            }
        }

        public XIIBO GetProperties(object oData, XIIBO oBOI)
        {
            Dictionary<string, string> oProperties = new Dictionary<string, string>();
            oProperties = oData.GetType()
     .GetProperties(BindingFlags.Instance | BindingFlags.Public)
          .ToDictionary(prop => prop.Name.ToLower(), prop => prop.GetValue(oData, null).ToString());
            foreach (var oAttrI in oBOI.Attributes)
            {
                if (oProperties.ContainsKey(oAttrI.Key.ToLower()))
                {
                    oBOI.Attributes[oAttrI.Key.ToLower()].sValue = oProperties[oAttrI.Key.ToLower()];
                }
            }
            return oBOI;
        }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public void SaveErrortoDB(CResult oCResult, int iQSInstanceID = 0, int iPolicyID = 0)
        {
            try
            {
                var ApplicationID = string.Empty;
                int iUserID = 0;
                string sUser = string.Empty;
                XIInfraCache oCache = new XIInfraCache();
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    ApplicationID = HttpContext.Current.Session["ApplicationID"].ToString();
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
                //EnumXIErrorPriority { Low = 10, Medium = 20, Critical = 30 }
               if (oCResult.iLogLevel >= finalLogLevel)
               { 
                //XIInfraCache oCache = new XIInfraCache();
                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ErrorLog");
                XIIBO oBOI = new XIIBO();
                oBOI.BOD = BOD;
                oBOI.SetAttribute("FKiApplicationID", ApplicationID);
                oBOI.SetAttribute("sCategory", oCResult.sCategory);
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
                if (!string.IsNullOrEmpty(oCResult.sMessage))
                    oBOI.Save(oBOI);
               }
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
                    oTrace.SetAttribute("FKiApplicationID", ApplicationID);
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
                //ELog.FKiQSInstanceID = iQSInstanceID;
                //ELog.FKiPolicyID = iPolicyID;
                //ELog.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                //ELog.TypeID = 20;
                //if (oCResult.sMessage.ToLower().StartsWith("error"))
                //{
                //    ELog.TypeID = 10;
                //}
                //ELog.sCode = oCResult.sCode;
                //if (!string.IsNullOrEmpty(ELog.Description))
                //    Connection.Insert<XIErrorLogs>(ELog, "XIErrorLog_T", "ID");
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
                //    Trace.FKiQSInstanceID = iQSInstanceID;
                //    Trace.FKiPolicyID = iPolicyID;
                //    Trace.Description = Msg;
                //    Trace.CreatedBySYSID = Utility.GetIPAddress(); //Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
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

        public object Clone(object obj)
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
        public string TimeStampToDateTime(double timeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timeStamp).ToLocalTime();
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
}