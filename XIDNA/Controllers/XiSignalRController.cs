using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using XICore;
using XIDataBase.Hubs;
using XIDNA.Models;
using XIDNA.Repository;
using XISystem;
using XIDatabase;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using WebGrease.Css.Extensions;

using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base.Delegates;
using static iTextSharp.text.pdf.AcroFields;
using Microsoft.Ajax.Utilities;
using System.Web.Caching;
using Microsoft.AspNet.SignalR.Hosting;
using XIDNA.ViewModels;
using xiEnumSystem;
namespace XIDNA.Controllers
{
    public class XiSignalRController : Controller
    {
        internal static SqlDependency dependency = null;
        internal static SqlDependency Leaddependency = null;
        internal static SqlCommand cmd = null;
        internal static SqlCommand leadCommand = null;
        internal static SqlDependency dependency_ONChange = null;
        readonly string _connString = ServiceUtil.GetClientConnectionString();
        string sDatabase = SessionManager.CoreDatabase;
        CommonRepository Common = new CommonRepository();
        ModelDbContext dbContext = new ModelDbContext();
        XIDefinitionBase oXID = new XIDefinitionBase();
        internal static List<string> OneclickGUidsforInbox = new List<string>();
        internal static List<string> OneclickGUidsforKPI = new List<string>();
        //GET: SignalR
        private Object _lockOnMe = new Object();

        public CResult NewSignalROneClick(int iClickID, int iRoleID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "NewSignalROneClick is dependency Working on OneClicks";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "iClickID", sValue = iClickID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "iRoleID", sValue = iRoleID.ToString() });
                if (iClickID > 0 && iRoleID > 0)//check mandatory params are passed or not
                {

                    SqlConnection con = new SqlConnection(_connString);
                    SqlCommand command = new SqlCommand();
                    XIInfraCache oCache = new XIInfraCache();
                    XID1Click o1ClickDe = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, iClickID.ToString());
                    XID1Click o1ClickCr = (XID1Click)o1ClickDe.Clone(o1ClickDe);
                    var BOID = o1ClickCr.BOID;
                    XIDBO oBOD = new XIDBO();
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, BOID.ToString());
                    var TableName = oBOD.TableName;
                    string SelectedFields = "";
                    if (oBOD.Attributes != null)
                    {
                        var BoAttributs = oBOD.Attributes.Where(r => r.Value.IsVisible == true).Select(u => u.Key).ToList();
                        foreach (var item1 in BoAttributs)
                        {
                            SelectedFields += item1 + ',';
                        }
                        SelectedFields = SelectedFields.TrimEnd(',');
                    }
                    // string SelectedFields = ""; string TableName = "";
                    command = new SqlCommand(@"SELECT " + SelectedFields + " FROM [dbo].[" + TableName + "]", con);
                    dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler((sender, e) => dependency_OnChangesave(sender, e, iClickID, iRoleID, SelectedFields, TableName));
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    command.ExecuteReader();
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iClickID= " + iClickID + " or iRoleID= " + iRoleID + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;

        }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        public ActionResult ReqirementOSCount()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "ReqirementOSCount method is used to get the list of Onceclicks";//expalin about this method logic
            try
            {
                if (dependency == null)
                {
                    XIInfraCache oCacheO = new XIInfraCache();
                    List<XIIBO> ListOfSignalRMasterTable = new List<XIIBO>();
                    string sOneClickName = "SignalRRoleIDs";
                    XID1Click o1ClickD = (XID1Click)oCacheO.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                    XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    var Result = o1ClickC.GetList();
                    if (Result.bOK == true && Result.oResult != null)
                    {
                        ListOfSignalRMasterTable = ((Dictionary<string, XIIBO>)Result.oResult).Values.ToList();
                        var Role = ListOfSignalRMasterTable.Select(y => y.Attributes).ToList();
                        if (Role.Count > 0)
                        {
                            foreach (var item in Role)
                            {
                                var id = item.Select(t => t.Value).Select(y => y.sValue).FirstOrDefault();
                                int iRoleID = Convert.ToInt32(id);
                                List<string> CountofList = new List<string>();
                                using (SqlConnection con = new SqlConnection(_connString))
                                {
                                    con.Open();

                                    // CResult Result = new CResult();
                                    XIInfraCache oCache = new XIInfraCache();
                                    XID1Click o1ClickD1 = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "XIInboxList", null);
                                    XID1Click o1ClickC1 = (XID1Click)o1ClickD1.Clone(o1ClickD1);
                                    XIDStructure oXIDStructure = new XIDStructure();
                                    List<CNV> nParams = new List<CNV>();
                                    CNV nvpairs = new CNV();
                                    nvpairs.sName = "{XIP|iRoleID}";
                                    nvpairs.sValue = iRoleID.ToString();
                                    nParams.Add(nvpairs);
                                    o1ClickC1.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC1.Query, nParams);
                                    Result = o1ClickC1.GetList();
                                    if (Result.bOK == true && Result.oResult != null)
                                    {
                                        var resultTest = ((Dictionary<string, XIIBO>)Result.oResult);
                                        foreach (var result in resultTest)
                                        {
                                            var fki1ClickID = result.Value.Attributes.Values.Where(n => n.sName.ToLower() == "fki1ClickID".ToLower()).Select(i => i.sValue).FirstOrDefault();
                                            XID1Click o1ClickDe = (XID1Click)oCacheO.GetObjectFromCache(XIConstant.Cache1Click, null, fki1ClickID);
                                            XID1Click o1ClickCr = (XID1Click)o1ClickDe.Clone(o1ClickDe);
                                            var BOID = o1ClickCr.BOID;
                                            XIDBO oBOD = new XIDBO();
                                            oBOD = (XIDBO)oCacheO.GetObjectFromCache(XIConstant.CacheBO_All, null, BOID.ToString());
                                            var TableName = oBOD.TableName;
                                            string SelectedFields = "";
                                            if (TableName != null && oBOD.Attributes.Count > 0)
                                            {
                                                if (oBOD.Attributes != null)
                                                {
                                                    var BoAttributs = oBOD.Attributes.Where(r => r.Value.IsVisible == true).Select(u => u.Key).ToList();
                                                    foreach (var item1 in BoAttributs)
                                                    {
                                                        SelectedFields += item1 + ',';
                                                    }
                                                    SelectedFields = SelectedFields.TrimEnd(',');
                                                }
                                                SqlCommand command = new SqlCommand();
                                                command = new SqlCommand(@"SELECT " + SelectedFields + " FROM [dbo].[" + TableName + "]", con);
                                                DataTable dt = new DataTable();
                                                dependency = new SqlDependency(command);
                                                dependency.OnChange += new OnChangeEventHandler((sender, e) => dependency_OnChangesave(sender, e, fki1ClickID.FirstOrDefault(), iRoleID, SelectedFields, TableName));
                                                if (con.State == ConnectionState.Closed)
                                                    con.Open();
                                                command.ExecuteReader();
                                            }
                                        }
                                    }
                                    con.Close();
                                }
                            }
                            oCResult.oResult = Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            //return (oCResult);
            return Json(oCResult.oResult, JsonRequestBehavior.AllowGet);

        }

        public CResult dependency_OnChangesave(object sender, SqlNotificationEventArgs e, int OneClick, int RoleID, string sSelectedFields, string sBoName)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "dependency_OnChangesave method executes, when the required parameters are passed to create the oneclicks";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "OneClick", sValue = OneClick.ToString() });
                oTrace.oParams.Add(new CNV { sName = "RoleID", sValue = RoleID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "sSelectedFields", sValue = sSelectedFields });
                oTrace.oParams.Add(new CNV { sName = "sBoName", sValue = sBoName });
                if (OneClick > 0 && RoleID > 0 && !string.IsNullOrEmpty(sSelectedFields) && !string.IsNullOrEmpty(sBoName))//check mandatory params are passed or not
                {
                    if (dependency != null)
                    {
                        dependency.OnChange -= new OnChangeEventHandler((Sender, r) => dependency_OnChangesave(sender, e, OneClick, RoleID, sSelectedFields, sBoName));
                        dependency = null;
                    }
                    if (e.Type == SqlNotificationType.Change || e.Info == SqlNotificationInfo.Insert)
                    {
                        //Call the EntireSignalR method because Count gain From the SP
                        oCR = EntireSignalR(OneClick, RoleID, sSelectedFields, sBoName);
                        //oCR = SubMethod();
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.oTrace.Add(oCR.oTrace);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                oCResult.oResult = "Success";
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: oneClick = " + OneClick + " or RoleID = " + RoleID + " or sSelectedFields = " + sSelectedFields + " or sBoName = " + sBoName + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            oCResult.oResult = (int)xiEnumSystem.xiFuncResult.xiSuccess;
            return oCResult;
        }

        public CResult EntireSignalR(int OneClick, int RoleID, string sSelectedFields, string BoName)
        {

            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "EntireSignalR is sub method for dependency_OnChangesave is used to hit the boname from sSelectedFields";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "OneClick", sValue = OneClick.ToString() });
                oTrace.oParams.Add(new CNV { sName = "RoleID", sValue = RoleID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "sSelectedFields", sValue = sSelectedFields });
                oTrace.oParams.Add(new CNV { sName = "BoName", sValue = BoName });
                if (OneClick > 0 && RoleID > 0 && !string.IsNullOrEmpty(sSelectedFields) && !string.IsNullOrEmpty(BoName))//check mandatory params are passed or not
                {

                    XIInfraCache oCacheO = new XIInfraCache();
                    using (SqlConnection con = new SqlConnection(_connString))
                    {
                        con.Open();
                        string count = "";
                        if (dependency == null)
                        {
                            SqlCommand command = new SqlCommand();
                            command = new SqlCommand(@"SELECT " + sSelectedFields + " FROM [dbo].[" + BoName + "]", con);
                            DataTable dt = new DataTable();
                            dependency = new SqlDependency(command);
                            dependency.OnChange += new OnChangeEventHandler((sender, e) => dependency_OnChangesave(sender, e, OneClick, RoleID, sSelectedFields, BoName));
                            command.ExecuteReader();
                        }
                        string sOneClickName = "SignalRRoleIDs";
                        XID1Click o1ClickD = (XID1Click)oCacheO.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                        XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                        var Result = o1ClickC.GetList();
                        if (Result.bOK == true && Result.oResult != null)
                        {
                            var Role = ((Dictionary<string, XIIBO>)Result.oResult).Values.Where(y => y.Attributes.Select(u => u.Value.sValue).ToList().Contains(RoleID.ToString())).ToList();

                            // CResult Result = new CResult();
                            XIInfraCache oCache = new XIInfraCache();
                            XID1Click o1ClickD1 = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "XIInbox", null);
                            XID1Click o1ClickC1 = (XID1Click)o1ClickD1.Clone(o1ClickD1);
                            XIDStructure oXIDStructure = new XIDStructure();
                            List<CNV> nParams = new List<CNV>();
                            CNV nvpairs = new CNV();
                            nvpairs.sName = "{XIP|OneClick}";
                            nvpairs.sValue = OneClick.ToString();
                            nParams.Add(nvpairs);
                            CNV nvpairs1 = new CNV();
                            nvpairs1.sName = "{XIP|RoleID}";
                            nvpairs1.sValue = RoleID.ToString();
                            nParams.Add(nvpairs1);
                            o1ClickC1.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC1.Query, nParams);
                            Result = o1ClickC1.GetList();
                            if (Result.bOK == true && Result.oResult != null)
                            {
                                var resultTest = ((Dictionary<string, XIIBO>)Result.oResult);
                                foreach (var result in resultTest)
                                {
                                    var SignalR = result.Value.Attributes.Values.Where(r => r.sName.ToLower() == "bSignalR".ToLower()).Select(s => s.sValue).FirstOrDefault().ToString();

                                    if (SignalR == "1" && Role.Count > 0)
                                    {

                                        SqlCommand objCmd = new SqlCommand("sp_XIInsuranceInboxCount", con);
                                        objCmd.CommandType = CommandType.StoredProcedure;
                                        objCmd.Parameters.AddWithValue("@oneClickID", OneClick);
                                        objCmd.Parameters.AddWithValue("@RoleID", RoleID);
                                        using (SqlDataReader reader = objCmd.ExecuteReader())
                                        {
                                            string resultList = ((IObjectContextAdapter)dbContext)
                                                                                           .ObjectContext
                                                                                           .Translate<string>(reader)
                                                                                           .FirstOrDefault();
                                            count = resultList;
                                        }
                                        con.Close();
                                        //SignalR
                                        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                                        context.Clients.All.EntireSolution(count);
                                    }
                                }
                            }
                            oCResult = Result;
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = " Mandatory Param: OneClick = " + OneClick + " or RoleID = " + RoleID + " or sSelectedFields = " + sSelectedFields + " or BoName = " + BoName + "  is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public ActionResult GetNotificationCount()
        {
            try
            {
                List<Dictionary<string, XIIBO>> TotalBOList = new List<Dictionary<string, XIIBO>>();
                SqlCommand depCommand = new SqlCommand();
                CResult Result = new CResult();
                XIInfraCache oCache1 = new XIInfraCache();
                string sOneClickName = "SignalR Settings Master Table";
                XIInfraCache oCacheO = new XIInfraCache();
                XIIComponent oCompI = new XIIComponent();
                List<XIIBO> ListOfSignalRMasterTable = new List<XIIBO>();
                XID1Click o1ClickD = (XID1Click)oCacheO.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                Result = o1ClickC.GetList();
                if (Result.bOK == true && Result.oResult != null)
                {
                    ListOfSignalRMasterTable = ((Dictionary<string, XIIBO>)Result.oResult).Values.ToList();
                    if (dependency_ONChange == null)
                    {
                        lock (_lockOnMe)
                        {
                            if (dependency_ONChange == null)
                            {
                                if (ListOfSignalRMasterTable.Count() > 0)
                                {
                                    using (var connection = new SqlConnection(_connString))
                                    {
                                        foreach (var ite in ListOfSignalRMasterTable)
                                        {
                                            connection.Open();
                                            string TableID = ite.Attributes["iTableID"].sValue;
                                            string QueryID = ite.Attributes["ID"].sValue;
                                            string QuerySelectedFields = ite.Attributes["sSelectedFields"].sValue;
                                            XIDBO oBOD = new XIDBO();
                                            XIInfraCache oCache = new XIInfraCache();
                                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, TableID.ToString());
                                            var TableName = oBOD.TableName;
                                            var BoName = oBOD.Name;
                                            SqlDependency.Start(_connString);
                                            cmd = new SqlCommand(@"SELECT " + QuerySelectedFields + " FROM [dbo].[" + TableName + "]", connection);
                                            cmd.Notification = null;
                                            dependency_ONChange = new SqlDependency(cmd);
                                            dependency_ONChange.OnChange += new OnChangeEventHandler((sender, e) => dependency_OnChangenotification(sender, e, BoName, TableName, QuerySelectedFields, QueryID));
                                            if (connection.State == ConnectionState.Closed)
                                                connection.Open();
                                            cmd.ExecuteReader();
                                            SqlDependency.Stop(_connString);
                                            connection.Close();

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                XID1Click o1ClickD1 = (XID1Click)oCache1.GetObjectFromCache(XIConstant.Cache1Click, "XISignalR Users Settings Result List", null);
                XID1Click o1ClickC1 = (XID1Click)o1ClickD1.Clone(o1ClickD1);
                CResult OcResult = new CResult();
                XIDStructure oXIDStructure = new XIDStructure();
                List<CNV> nParams = new List<CNV>();
                CNV nvpairs = new CNV();
                nvpairs.sName = "";
                nvpairs.sValue = "";
                nParams.Add(nvpairs);
                o1ClickC1.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC1.Query, nParams);
                Result = o1ClickC1.GetList();

                if (Result.bOK == true && Result.oResult != null)
                {
                    var resultTest = ((Dictionary<string, XIIBO>)Result.oResult);
                    foreach (var item in resultTest)
                    {

                        var sWhereFields = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "sWhereFields".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var sOneClickOrBO = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "sOneClickOrBO".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var iUserID = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "iUserID".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var sConfig = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "sConfig".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var sConstantID = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "sConstantID".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var iNotificationBO = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "iNotificationBO".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var fkiOneClick = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "fkiOneClick".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var fkiBOID = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "fkiBOID".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var iShowCount = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "iShowCount".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var sBOSelectedFields = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "sBOSelectedFields".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var iRoleID = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "iRoleID".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var sSelectedFields = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "sSelectedFields".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        var sCount = item.Value.Attributes.Values.Where(n => n.sName.ToLower() == "iCount".ToLower()).Select(i => i.sValue).FirstOrDefault();
                        int iCount = 0;

                        using (var depConnection = new SqlConnection(_connString))
                        {
                            depConnection.Open();
                            string selected = "";
                            string where = "";
                            string TableName = "";
                            if (sOneClickOrBO == "oneclick")
                            {
                                //// var tableid = item1.BOID;
                                XIDBO oBOD = new XIDBO();
                                XIInfraCache oCache = new XIInfraCache();
                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, fkiBOID.ToString());
                                TableName = oBOD.TableName;
                                where = sWhereFields;
                                selected = sSelectedFields;
                            }
                            //else if (item1.OneClickOrBO == "bo")
                            else
                            {
                                var tableid = iNotificationBO;
                                where = sWhereFields;
                                selected = sSelectedFields;
                                XIDBO oBOD = new XIDBO();
                                XIInfraCache oCache = new XIInfraCache();
                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, tableid.ToString());
                                // var SelectedItem = oBOD.Attributes.ToList();
                                TableName = oBOD.TableName;
                            }
                            if (selected != null && selected != "")
                            {
                                if (where != null && where != "0")
                                {
                                    //depCommand = new SqlCommand(@"SELECT " + selected + " FROM [dbo].[" + TableName + "] WHERE " + where, depConnection);
                                    depCommand = new SqlCommand(@"SELECT Count(*) as count FROM [dbo].[" + TableName + "] WHERE " + where, depConnection);
                                }
                                else
                                {
                                    //depCommand = new SqlCommand(@"SELECT " + selected + " FROM [dbo].[" + TableName + "]", depConnection);
                                    depCommand = new SqlCommand(@"SELECT Count(*) as Count FROM [dbo].[" + TableName + "]", depConnection);
                                }
                            }

                            iCount = (Int32)depCommand.ExecuteScalar();
                            item.Value.Attributes["iCount"].sValue = iCount.ToString();

                            depConnection.Close();

                        }

                    }

                    OcResult.oResult = resultTest;

                }
                return Json(OcResult.oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog("ErrorLog: XiSignalR GetNotificationCount method" + ex.ToString(), sDatabase);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public CResult dependency_OnChangenotification(object sender2, SqlNotificationEventArgs e2, string BoName, string TableName, string selectdFields, string QueryID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "dependency_OnChangenotification is used to notify the oneclick changes along with the default fields";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "BoName", sValue = BoName });
                oTrace.oParams.Add(new CNV { sName = "TableName", sValue = TableName });
                oTrace.oParams.Add(new CNV { sName = "selectdFields", sValue = selectdFields });
                oTrace.oParams.Add(new CNV { sName = "QueryID", sValue = QueryID });
                if (!string.IsNullOrEmpty(BoName) && !string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(selectdFields) && !string.IsNullOrEmpty(QueryID))//check mandatory params are passed or not
                {
                    if (e2.Info == SqlNotificationInfo.Insert || e2.Type == SqlNotificationType.Change)
                    {
                        if (dependency_ONChange != null)
                        {
                            dependency_ONChange.OnChange -= new OnChangeEventHandler((sender, e) => dependency_OnChangenotification(sender, e, BoName, TableName, selectdFields, QueryID));
                            dependency_ONChange = null;
                            //oCR = SubMethod();
                            oCR = CommonNotificationCount(BoName, TableName, selectdFields, QueryID);


                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oTrace.oTrace.Add(oCR.oTrace);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                    oCResult.oResult = "Success";
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = " Mandatory Param: BoName = " + BoName + " or TableName = " + TableName + " or selectdFields = " + selectdFields + " or QueryID = " + QueryID + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult CommonNotificationCount(string BoName, string TableName, string selectdFields, string QueryID)
        {

            string sDatabase = SessionManager.CoreDatabase;
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "CommonNotificationCount is a submethod for dependency_OnChangenotification, it calls when the main method is excuted successfully along with the required parameters";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "BoName", sValue = BoName });
                oTrace.oParams.Add(new CNV { sName = "TableName", sValue = TableName });
                oTrace.oParams.Add(new CNV { sName = "selectdFields", sValue = selectdFields });
                oTrace.oParams.Add(new CNV { sName = "QueryID", sValue = QueryID });
                if (!string.IsNullOrEmpty(BoName) && !string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(selectdFields) && !string.IsNullOrEmpty(QueryID))//check mandatory params are passed or not
                {
                    CResult Result = new CResult();
                    XIInfraCache oCache = new XIInfraCache();
                    XID1Click o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "OneClickSignalRuserSettingTable", null);
                    XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    XIDStructure oXIDStructure = new XIDStructure();
                    List<CNV> nParams = new List<CNV>();
                    CNV nvpairs = new CNV();
                    nvpairs.sName = "{XIP|values}";
                    nvpairs.sValue = QueryID;
                    nParams.Add(nvpairs);
                    o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParams);
                    Result = o1ClickC.GetList();
                    List<Dictionary<string, XIIBO>> ListofBos = new List<Dictionary<string, XIIBO>>();
                    if (Result.bOK == true && Result.oResult != null)
                    {
                        var resultList = ((Dictionary<string, XIIBO>)Result.oResult);
                        foreach (var item in resultList)
                        {
                            var sQueryID = item.Value.Attributes["FkiNewSignalrQueryID"].sValue;
                            if (sQueryID == QueryID)
                            {
                                Dictionary<string, XIIBO> ListofBod = new Dictionary<string, XIIBO>();
                                var dictionary = new Dictionary<string, XIIBO> { { item.Key, item.Value } };
                                ListofBos.Add(dictionary);
                            }
                        }
                    }
                    XIInfraCache oCache1 = new XIInfraCache();

                    using (SqlConnection connection = new SqlConnection(_connString))
                    {
                        connection.Open();
                        //SqlDependency.Start(_connString);
                        SqlCommand depCommand = new SqlCommand();
                        SqlCommand Notifycmd = new SqlCommand();
                        Notifycmd = new SqlCommand(@"SELECT " + selectdFields + " FROM [dbo].[" + TableName + "]", connection);
                        XIDBO oBOD = new XIDBO();
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, BoName, "");
                        int iDataSource = oBOD.iDataSource;
                        Guid DataSourceXIGUID = oBOD.iDataSourceXIGUID;
                        XID1Click D1CQuery = new XID1Click();
                        D1CQuery.sBOName = oBOD.Name;
                        D1CQuery.SelectFields = selectdFields;
                        D1CQuery.Query = Notifycmd.CommandText;
                        XIDXI oXIDc = new XIDXI();
                        string sConntection = oXIDc.GetBODataSource(DataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                        Connection = new XIDBAPI(sConntection);
                        if (dependency_ONChange == null)
                        {
                            dependency_ONChange = new SqlDependency(Notifycmd);
                            dependency_ONChange.OnChange += new OnChangeEventHandler((sender, e) => dependency_OnChangenotification(sender, e, BoName, TableName, selectdFields, QueryID));
                            if (connection.State == ConnectionState.Closed)
                                connection.Open();
                            Notifycmd.ExecuteReader();
                            //SqlDependency.Stop(_connString);
                        }
                        else
                        {
                        }

                        if (ListofBos.Count > 0)
                        {
                            List<Dictionary<string, XIIBO>> list = new List<Dictionary<string, XIIBO>>();

                            List<List<XIIAttribute>> ListAttr = new List<List<XIIAttribute>>();
                            var warefields = "";
                            foreach (var itemb in ListofBos)
                            {
                                foreach (var item1 in itemb)
                                {
                                    var sWhereFields = item1.Value.Attributes["sWhereFields"].sValue;
                                    warefields += sWhereFields + ",";
                                }
                            }

                            var wareFieldList = warefields.TrimEnd(',').Split(',');
                            var CommonFieldsList = wareFieldList.Distinct().ToList();

                            string sFlashCount = "";
                            foreach (var ListItem in CommonFieldsList)
                            {
                                var sUserID = "";
                                foreach (var itemRes in ListofBos)
                                {
                                    var sWhereFields = itemRes.Values.Select(u => u.Attributes["sWhereFields"].sValue).FirstOrDefault();
                                    var SelectedFields = itemRes.Values.Select(u => u.Attributes["sSelectedFields"].sValue).FirstOrDefault();
                                    var NotificationBO = itemRes.Values.Select(u => u.Attributes["iNotificationBO"].sValue).FirstOrDefault();
                                    var BOSelectedFields = itemRes.Values.Select(u => u.Attributes["sBOSelectedFields"].sValue).FirstOrDefault();
                                    var Config = itemRes.Values.Select(u => u.Attributes["sConfig"].sValue).FirstOrDefault();
                                    var SentMail = itemRes.Values.Select(u => u.Attributes["sSentMail"].sValue).FirstOrDefault();
                                    var fkiBOID = itemRes.Values.Select(u => u.Attributes["fkiBOID"].sValue).FirstOrDefault().ToString();
                                    var userId = itemRes.Values.Select(u => u.Attributes["iUserID"].sValue).FirstOrDefault().ToString();
                                    var sCount = itemRes.Values.Select(u => u.Attributes["iCount"].sValue).FirstOrDefault().ToString();
                                    //var MasterID = itemRes.Values.Select(u => u.Attributes["iMasterID"].sValue).FirstOrDefault().ToString();
                                    var ResultOneClick = itemRes.Values.Select(u => u.Attributes["fkidepOneClick"].sValue).FirstOrDefault().ToString();
                                    var iRoleID = itemRes.Values.Select(u => u.Attributes["iRoleID"].sValue).FirstOrDefault().ToString();
                                    int iCount = 0;
                                    if (ListItem == sWhereFields)
                                    {
                                        if (Config != "QuoteReportData")
                                        {
                                            DataTable dt = new DataTable();
                                            string where = sWhereFields;
                                            string selectdFields1 = selectdFields;
                                            if (selectdFields1 == "0" && selectdFields1 == null)
                                            {
                                                var tableid = NotificationBO;
                                                selectdFields1 = BOSelectedFields;
                                            }
                                            if (where != null && where != "0")
                                            {
                                                //depCommand = new SqlCommand(@"SELECT " + selectdFields1 + " FROM [dbo].[" + TableName + "] WHERE " + where, connection);
                                                depCommand = new SqlCommand(@"SELECT Count(*) FROM [dbo].[" + TableName + "] WHERE " + where, connection);
                                            }
                                            else
                                            {
                                                depCommand = new SqlCommand(@"SELECT Count(*) FROM [dbo].[" + TableName + "]", connection);
                                            }
                                            iCount = (Int32)depCommand.ExecuteScalar();
                                            itemRes.Values.Select(u => u.Attributes["iCount"].sValue = iCount.ToString()).FirstOrDefault();
                                            // item.BOName = TableName;
                                            XID1Click o1ClickD1 = new XID1Click();
                                            string updatedRecord = "";
                                            string iBOIID = string.Empty;
                                            if (where != null && where != "0")
                                            {
                                                //updatedRecord = (@"SELECT TOP 1 " + selectdFields + " FROM " + TableName + " Where " + where + "  ORDER BY XIUpdatedWhen desc");
                                                updatedRecord = D1CQuery.Query;
                                                updatedRecord = updatedRecord.Replace("SELECT", "SELECT TOP 1");
                                                updatedRecord = updatedRecord + "Where " + where + " ORDER BY XIUpdatedWhen desc";
                                                // updatedRecord = D1CQuery.Query;
                                                DataTable oBOInsdt = new DataTable();
                                                string Count = "";
                                                Count = Connection.GetTotalCount(CommandType.Text, updatedRecord, null);
                                                oBOInsdt = (DataTable)Connection.ExecuteQuery(updatedRecord);
                                                Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
                                                Dictionary<string, XIIValue> XIValuedictionary = new Dictionary<string, XIIValue>();
                                                Dictionary<string, List<XIIBO>> nBOIns = new Dictionary<string, List<XIIBO>>();
                                                string sBo = string.Empty;
                                                if (oBOD != null)
                                                {
                                                    List<XIIBO> oBoList = new List<XIIBO>();
                                                    List<DataRow> Rows = new List<DataRow>();
                                                    Rows = oBOInsdt.AsEnumerable().ToList();
                                                    Dictionary<string, XIDBO> OptionListCols = new Dictionary<string, XIDBO>();
                                                    var AllCols = oBOInsdt.Columns.Cast<DataColumn>()
                                                                     .Select(x => x.ColumnName)
                                                                     .ToList();
                                                    OptionListCols = OptionListCols ?? new Dictionary<string, XIDBO>();
                                                    if (Rows.Count > 0)
                                                    {
                                                        foreach (DataRow row in Rows)
                                                        {
                                                            XIIBO oBOII = new XIIBO();
                                                            dictionary = Enumerable.Range(0, oBOInsdt.Columns.Count)
                                                                .ToDictionary(i => oBOInsdt.Columns[i].ColumnName.ToLower(), i => new XIIAttribute
                                                                {
                                                                    sName = oBOInsdt.Columns[i].ColumnName,
                                                                    //sValue = OptionListCols.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), oBOD) : row.ItemArray[i].ToString(),
                                                                    sValue = OptionListCols.ContainsKey(oBOInsdt.Columns[i].ColumnName.ToLower()) ? o1ClickD1.CheckOptionList(oBOInsdt.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOInsdt.Columns[i].ColumnName.ToLower()], oBOInsdt.Columns, row.ItemArray) : row.ItemArray[i].ToString(),
                                                                    sPreviousValue = row.ItemArray[i].ToString(),
                                                                    sDisplayName = oBOD.Attributes.ContainsKey(oBOInsdt.Columns[i].ColumnName) ? oBOD.AttributeD(oBOInsdt.Columns[i].ColumnName).LabelName : "",
                                                                    //iValue = TotalColumns.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? (!string.IsNullOrEmpty(row.ItemArray[i].ToString()) ? (Convert.ToInt32(row.ItemArray[i].ToString())) : 0) : 0,
                                                                }, StringComparer.CurrentCultureIgnoreCase);
                                                            oBOII.Attributes = dictionary;
                                                            XIValuedictionary = Enumerable.Range(0, oBOInsdt.Columns.Count)
                                                                 .ToDictionary(i => oBOInsdt.Columns[i].ColumnName, i => new XIIValue
                                                                 {
                                                                     sValue = row.ItemArray[i].ToString(),
                                                                     sDisplayName = oBOD.Attributes.ContainsKey(oBOInsdt.Columns[i].ColumnName) ? oBOD.AttributeD(oBOInsdt.Columns[i].ColumnName).LabelName : "",
                                                                 }, StringComparer.CurrentCultureIgnoreCase);
                                                            oBOII.XIIValues = XIValuedictionary;
                                                            oBOII.iBODID = oBOD.BOID;
                                                            oBOII.sBOName = oBOD.TableName;
                                                            oBOII.sPrimaryKey = oBOD.sPrimaryKey;
                                                            oBoList.Add(oBOII);
                                                            sBo = oBOD.Name;
                                                            iBOIID = oBOII.AttributeI(oBOD.sPrimaryKey).sValue;
                                                        }
                                                        nBOIns[sBo] = oBoList;

                                                        var MailFlag = nBOIns.FirstOrDefault();

                                                        var sSentMail = MailFlag.Value.FirstOrDefault().Attributes.Where(y => y.Key.ToLower() == "isentmail").Select(u => u.Value).Select(t => t.sValue).FirstOrDefault();
                                                        var Status = MailFlag.Value.FirstOrDefault().Attributes.Where(y => y.Key.ToLower() == "istatus").Select(u => u.Value).Select(t => t.sValue).FirstOrDefault();
                                                        var sPrimaryKey = MailFlag.Value.FirstOrDefault().sPrimaryKey;

                                                        var NewID = nBOIns.FirstOrDefault();
                                                        var sNewID = NewID.Value.FirstOrDefault().Attributes.Where(y => y.Key.ToLower() == "id").Select(u => u.Value).Select(t => t.sValue).FirstOrDefault();
                                                        int ID = Convert.ToInt32(sNewID);

                                                        sUserID = NewID.Value.FirstOrDefault().Attributes.Where(y => y.Key.ToLower() == "fkiuserid").Select(u => u.Value).Select(t => t.sValue).FirstOrDefault();
                                                        if (!string.IsNullOrEmpty(sUserID))
                                                        {
                                                            itemRes.Values.Select(u => u.Attributes["iUserID"].sValue = sUserID);
                                                        }
                                                        //var MailFlagTrue = MailFlag.Value.FirstOrDefault().Attributes.Where(y => y.Key.ToLower() == "bmailflag").Select(u => u.Value).Select(t => t.sValue).FirstOrDefault();
                                                        if (!string.IsNullOrEmpty(SentMail) /*&& FlagMail == true*/)
                                                        {
                                                            XIDefinitionBase oXID = new XIDefinitionBase();
                                                            // CResult oCResult = new CResult();
                                                            string sTemplateID = SentMail;
                                                            XIContentEditors oContent = new XIContentEditors();
                                                            List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                                                            if (!string.IsNullOrEmpty(sTemplateID))
                                                            {
                                                                oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, sTemplateID);

                                                                if (oContentDef != null && oContentDef.Count() > 0)
                                                                {
                                                                    oContent = oContentDef.FirstOrDefault();
                                                                }
                                                            }
                                                            XIInfraEmail oEmail = new XIInfraEmail();
                                                            List<string> sEmails = new List<string>();
                                                            string sEmail = "";
                                                            CResult oUseDef = new CResult();
                                                            XIInfraUsers oUsers = new XIInfraUsers();
                                                            var UserID = userId.ToString().FirstOrDefault();
                                                            if (UserID == -1)
                                                            {
                                                                oUseDef = oUsers.Get_AllUserDetails(sDatabase, UserID);
                                                                if (oUseDef.bOK == true && oUseDef.oResult != null)
                                                                {
                                                                    List<XIInfraUsers> user = (List<XIInfraUsers>)oUseDef.oResult;
                                                                    sEmails = user.Select(u => u.sEmail).ToList();
                                                                }
                                                            }
                                                            else if (UserID > 0)
                                                            {
                                                                oUseDef = oUsers.Get_UserDetails(sDatabase, UserID);
                                                                if (oUseDef.bOK == true && oUseDef.oResult != null)
                                                                {
                                                                    XIInfraUsers user = (XIInfraUsers)oUseDef.oResult;
                                                                    sEmail = user.sEmail;
                                                                    sEmails.Add(sEmail);
                                                                }
                                                            }

                                                            foreach (var sUserEmail in sEmails)
                                                            {
                                                                oEmail.EmailID = sUserEmail;
                                                                string sContext = XIConstant.Lead_Transfer;
                                                                string sNewGUID = Guid.NewGuid().ToString();
                                                                XIBOInstance oBOIns = new XIBOInstance();
                                                                oBOIns.oStructureInstance = nBOIns;
                                                                XIContentEditors oConent = new XIContentEditors();
                                                                oEmail.sSubject = oContent.sSubject;
                                                                Result = new CResult();
                                                                Result = oConent.MergeContentTemplate(oContent, oBOIns);
                                                                oCResult.oTraceStack.Add(new CNV { sName = "Mail Sending started", sValue = "Sending mail started, email:" + oEmail.EmailID + "" });
                                                                var oMailResult = oEmail.Sendmail(oContent.OrganizationID, Result.oResult.ToString(), null, 0, sContext, 0, null, 0, oContent.bIsBCCOnly);//send mail with attachments
                                                                if (oMailResult.bOK && oMailResult.oResult != null)
                                                                {
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully to email:" + oEmail.EmailID + "" });
                                                                    oXID.SaveErrortoDB(oCResult);
                                                                }
                                                            }
                                                        }
                                                        if (sSentMail == "0")
                                                        {
                                                            SqlCommand updaterecord = new SqlCommand("UPDATE " + TableName + " SET iSentMail=10 WHERE " + sPrimaryKey + "=" + ID, connection);
                                                            if (connection.State == ConnectionState.Closed)
                                                                connection.Open();
                                                            updaterecord.ExecuteNonQuery();

                                                            itemRes.Values.ForEach(u => u.Attributes["iMasterID"].sValue = sNewID.ToString());
                                                            sFlashCount = sNewID;
                                                        }
                                                        else if (sSentMail == "10" && (where == "[ACPolicy_T].iStatus = '200'" || where == "[ACPolicy_T].iStatus = '25'"))
                                                        {
                                                            SqlCommand updaterecord = new SqlCommand("UPDATE " + TableName + " SET iSentMail=20 WHERE " + sPrimaryKey + "=" + ID, connection);
                                                            if (connection.State == ConnectionState.Closed)
                                                                connection.Open();
                                                            updaterecord.ExecuteNonQuery();
                                                            itemRes.Values.ForEach(u => u.Attributes["iMasterID"].sValue = sNewID.ToString());
                                                            sFlashCount = sNewID;
                                                        }
                                                        else
                                                        {
                                                            sFlashCount = sNewID;
                                                        }
                                                        if (!string.IsNullOrEmpty(sFlashCount) && sFlashCount != "0")
                                                        {
                                                            //itemRes.Values.FirstOrDefault().Attributes.Where(u => u.Key.ToLower() == "iMasterID".ToLower()).ForEach(t=>t.Value.sValue= sFlashCount);
                                                            itemRes.Values.ForEach(u => u.Attributes["iMasterID"].sValue = sFlashCount);
                                                        }
                                                    }
                                                }
                                            }
                                            //DashBoard Related SignalR
                                            List<CNV> oParams = new List<CNV>();
                                            oParams.Add(new CNV { sName = "sGUID", sValue = null });
                                            oParams.Add(new CNV { sName = "sSessionID", sValue = null });
                                            oParams.Add(new CNV { sName = "fkidepOneClick", sValue = ResultOneClick });

                                            string sSessionID = Guid.NewGuid().ToString();
                                            string sGUID = Guid.NewGuid().ToString();
                                            List<CNV> oNVsList = new List<CNV>();
                                            XIDAlgorithm oAlogD = new XIDAlgorithm();
                                            if (Config.ToLower() == "ProcessController".ToLower())
                                            {
                                                oNVsList.Add(new CNV { sName = "-iSignalRID", sValue = itemRes.Values.Select(u => u.Attributes["ID"].sValue).FirstOrDefault() });
                                                oNVsList.Add(new CNV { sName = "-iBODID", sValue = oBOD.BOID.ToString() });
                                                oNVsList.Add(new CNV { sName = "-sBO", sValue = oBOD.Name });
                                                oNVsList.Add(new CNV { sName = "-iBOIID", sValue = iBOIID });
                                                oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                                                if (oBOD.Name == "Notifications")
                                                {
                                                    itemRes.Values.Select(u => u.Attributes["FKiAlgorithmID"].sValue = "2028").FirstOrDefault();
                                                    //itemRes.FKiAlgorithmID = 2028;
                                                }
                                                else
                                                {
                                                    // itemRes.Values.Select(u => u.Attributes["FKiAlgorithmID"].sValue = "3031").FirstOrDefault();
                                                    //itemRes.FKiAlgorithmID = 3031;
                                                }
                                                oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, itemRes.Values.Select(u => u.Attributes["FKiAlgorithmID"].sValue).FirstOrDefault());
                                                oAlogD.Execute_XIAlgorithm(sSessionID, sGUID);
                                            }
                                            else if (Config.ToLower() == "FlashNotification".ToLower())
                                            {
                                                XIIXI oXI = new XIIXI();
                                                var NotificationI = oXI.BOI("Notifications", iBOIID);
                                                if (NotificationI != null && NotificationI.Attributes.Count() > 0)
                                                {
                                                    var sMessage = NotificationI.AttributeI("sMessage").sValue;
                                                    itemRes.Values.Select(u => u.Attributes["AlertText"].sValue = sMessage).FirstOrDefault();
                                                    //itemRes.AlertText = sMessage;
                                                    var iBODID = NotificationI.AttributeI("FKiBODID").sValue;
                                                    var BOIID = NotificationI.AttributeI("FKiBOIID").sValue;
                                                    itemRes.Values.Select(u => u.Attributes["AlertInfo"].sValue = iBODID + ":" + BOIID).FirstOrDefault();
                                                    //itemRes.AlertInfo = iBODID + ":" + BOIID;
                                                }
                                            }

                                            //XIIXI oXIXI = new XIIXI();
                                            //var SignalRNotificationI = oXIXI.BOI("Notifications", iBOIID);
                                            //oNVsList = new List<CNV>();
                                            //oNVsList.Add(new CNV { sName = "ID", sValue = itemRes.Values.Select(u => u.Attributes["ID"].sValue).FirstOrDefault() });
                                            //oNVsList.Add(new CNV { sName = "FKiBoid", sValue = oBOD.BOID.ToString() });
                                            //oNVsList.Add(new CNV { sName = "FKiBoidXIGUID", sValue = oBOD.BOID.ToString() });
                                            //oNVsList.Add(new CNV { sName = "sAlertType", sValue = Config });
                                            //oNVsList.Add(new CNV { sName = "sAlertMessage", sValue = itemRes.Values.Select(u => u.Attributes["sAlertText"].sValue).FirstOrDefault() });


                                            //XIIBO oBOI = new XIIBO();
                                            //var oMainBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null,fkiBOID);
                                            //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XINotifications", null);
                                            //oBOI.SetAttribute("FKiBoid", fkiBOID);
                                            ////oBOI.SetAttribute("sBOName", oBOD.BOID.ToString());
                                            ////oBOI.SetAttribute("FKiBoidXIGUID", oBOD.BOID.ToString());
                                            //oBOI.SetAttribute("sAlertType", Config);
                                            //oBOI.SetAttribute("sAlertMessage", itemRes.Values.Select(u => u.Attributes["AlertText"].sValue).FirstOrDefault());
                                            //oBOI.SetAttribute("sAlertType", Config);
                                            //oBOI.SetAttribute("iUserID", userId);
                                            //oBOI.SetAttribute("iRoleID", iRoleID);
                                            //oBOI.BOD = oBOD;
                                            //oCR = oBOI.Save(oBOI);





                                            //oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                                            //if (oBOD.Name == "Notifications")
                                            //{
                                            //    itemRes.Values.Select(u => u.Attributes["FKiAlgorithmID"].sValue = "2028").FirstOrDefault();
                                            //    //itemRes.FKiAlgorithmID = 2028;
                                            //}
                                            //else
                                            //{
                                            //    // itemRes.Values.Select(u => u.Attributes["FKiAlgorithmID"].sValue = "3031").FirstOrDefault();
                                            //    //itemRes.FKiAlgorithmID = 3031;
                                            //}
                                            //oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, itemRes.Values.Select(u => u.Attributes["FKiAlgorithmID"].sValue).FirstOrDefault());
                                            //oAlogD.Execute_XIAlgorithm(sSessionID, sGUID);
                                            //SignalR
                                            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                                            context.Clients.All.BroadcastNotification(itemRes);
                                        }
                                        //*******   Plz don't delete below comment code --Heat map related     ********
                                        //else if (item.Config == "QuoteReportData")
                                        //{
                                        //    XIInfraQuoteReportDataComponent report = new XIInfraQuoteReportDataComponent();
                                        //    List<CNV> oParams1 = new List<CNV>();
                                        //    oParams1.Add(new CNV { sName = "1ClickID", sValue = item.OneClick.ToString() });
                                        //    oParams1.Add(new CNV { sName = "Visualisation", sValue = "MinuteReportColours" });
                                        //    var res = report.XILoad(oParams1);
                                        //    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                                        //    context.Clients.All.LeadData(item.QuoteReportData);
                                        //}
                                        // }
                                    }
                                }

                            }
                        }

                        connection.Close();
                    }
                    oCResult.oResult = ListofBos;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;

                    oTrace.sMessage = " Mandatory Param: BoName = " + BoName + " or TableName = " + TableName + " or selectdFields = " + selectdFields + " or QueryID = " + QueryID + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        [HttpPost]
        public ActionResult SignalROneClickContent(int userID, string oneclick, string PopupOrDialog)
        {
            string sGUID = Guid.NewGuid().ToString();
            var sSessionID = HttpContext.Session.SessionID;
            XIInfraOneClickComponent oOCC = new XIInfraOneClickComponent();
            XIIComponent oCompI = new XIIComponent();
            XIDComponent oCompD = new XIDComponent();
            XIInfraCache oCache = new XIInfraCache();
            var oParams = oCompD.Params.Select(m => new CNV { sName = m.sName, sValue = m.sValue }).ToList();
            oParams = (List<CNV>)oCache.ResolveParameters(oParams, null, null);
            oParams.Add(new CNV { sName = "sGUID", sValue = sGUID });
            oParams.Add(new CNV { sName = "sSessionID", sValue = sSessionID });
            oParams.Add(new CNV { sName = "iUserID", sValue = userID.ToString() });
            oParams.Add(new CNV { sName = "1ClickID", sValue = oneclick.ToString() });
            var oCR = oOCC.XILoad(oParams);
            if (oCR.bOK && oCR.oResult != null)
            {
                oCompI.oContent[XIConstant.OneClickComponent] = oCR.oResult;
                if (PopupOrDialog == "Popup") oCompI.bFlag = true;
                else oCompI.bFlag = false;
            }
            return PartialView("~\\Views\\XIComponents\\_OneClickComponent.cshtml", oCompI);
        }
        List<XID1Click> o1ClickL = new List<XID1Click>();
        List<CNV> oWhrParam = new List<CNV>();

        internal static SqlTableDependency<AllBOsforSignalR> _Leaddependency = null;
        [HttpPost]
        public ActionResult LeadTrace(string sOneClick, bool Flag = false)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "LeadTrace is used to traceout the generated oneclicks";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sOneClick", sValue = sOneClick });
                oTrace.oParams.Add(new CNV { sName = "Flag", sValue = Flag.ToString() });
                var UserorgID = HttpContext.Session["iUserOrg"];
                var oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "{UserorgID}", sValue = UserorgID.ToString() });
                if (!string.IsNullOrEmpty(sOneClick))//check mandatory params are passed or not
                {

                    XID1Click o1ClickD = new XID1Click();
                    XID1Click o1ClickC = new XID1Click();
                    XIInfraCache oCache = new XIInfraCache();
                    var splitSOneClick = sOneClick.Split(',');
                    foreach (var item in splitSOneClick)
                    {
                        using (SqlConnection connection = new SqlConnection(_connString))
                        {
                            connection.Open();
                            //SqlDependency.Start(_connString);
                            o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, item);
                            o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                            if (o1ClickC != null)
                            {
                                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickC.BOIDXIGUID.ToString());
                                SqlCommand sleadCommand = new SqlCommand();
                                if (o1ClickC.WhereFields == null)
                                {
                                    sleadCommand = new SqlCommand(@"SELECT " + o1ClickC.SelectFields + " FROM [dbo].[" + o1ClickC.FromBos + "]", connection);
                                }
                                else if (o1ClickC.SelectFields == null)
                                {
                                    sleadCommand = new SqlCommand(o1ClickC.Query, connection);
                                    o1ClickC.ReplaceFKExpressions(oParams);
                                    sleadCommand.CommandText = o1ClickC.Query;
                                }
                                else
                                {
                                    sleadCommand = new SqlCommand(@"SELECT " + o1ClickC.SelectFields + " FROM [dbo].[" + o1ClickC.FromBos + "] Where " + o1ClickC.WhereFields + "", connection);
                                }
                                if (_Leaddependency == null)
                                {
                                    lock (_lockOnMe)
                                    {
                                        if (_Leaddependency == null)
                                        {
                                            if (o1ClickC.SelectFields == null)
                                            {
                                                leadCommand = new SqlCommand(o1ClickC.Query, connection);
                                            }
                                            else
                                            {
                                                leadCommand = new SqlCommand(@"SELECT " + o1ClickC.SelectFields + " FROM [dbo].[" + o1ClickC.FromBos + "] ", connection);
                                            }
                                            var TableName = oBOD.TableName;
                                            var BOName = oBOD.Name;
                                            _Leaddependency = new SqlTableDependency<AllBOsforSignalR>(_connString, TableName);
                                            //_Leaddependency.OnChanged += new ChangedEventHandler<AllBOsforSignalR>((sender, e) => Leaddependency_OnChange(sender, e, sOneClick));
                                            _Leaddependency.OnChanged += new ChangedEventHandler<AllBOsforSignalR>((sender, e) => _Dependency_OnChangedForInbox(sender, e, BOName, sOneClick, "", "Trace"));

                                            //Leaddependency = new SqlDependency(leadCommand);
                                            //Leaddependency.OnChange += new OnChangeEventHandler((sender, e) => Leaddependency_OnChange(sender, e, sOneClick));
                                            if (connection.State == ConnectionState.Closed)
                                                connection.Open();
                                            _Leaddependency.Start();
                                            connection.Close();
                                            //leadCommand.ExecuteNonQuery();
                                        }
                                    }
                                }
                                DataTable dt = new DataTable();
                                var reader = sleadCommand.ExecuteReader();
                                dt.Load(reader);
                                //if (dt.Rows.Count > 0)
                                //{
                                o1ClickC.LeadCount = dt.Rows.Count;
                                o1ClickL.Add(o1ClickC);
                                var leadcount = dt.Rows.Count;
                                foreach (DataRow items in dt.Rows)
                                {
                                    if (items.ItemArray.Count() > 0)
                                    {
                                        oWhrParam.Add(new CNV { sName = items.ItemArray[0].ToString(), sValue = items.ItemArray[1].ToString() });
                                    }
                                }
                            }
                            // }
                            //SignalR
                            //SqlDependency.Stop(_connString);
                            //connection.Close();

                        }
                    }
                    IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    LeadContext.Clients.All.LeadtraceFlowChat(oWhrParam, Flag);
                }

                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = " Mandatory Param: sOneClick = " + sOneClick + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            oCResult.oResult = (int)xiEnumSystem.xiFuncResult.xiSuccess;
            //return oCResult;
            return Json(oWhrParam, JsonRequestBehavior.AllowGet);
        }
        public CResult Leaddependency_OnChange(object sender, SqlNotificationEventArgs e, string sOneClick)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Leaddependency_OnChange is a main method for leadtrace it excutes when we updates the oneclicks";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sOneClick", sValue = sOneClick });
                if (!string.IsNullOrEmpty(sOneClick))//check mandatory params are passed or not
                {
                    if (Leaddependency != null)
                    {
                        Leaddependency.OnChange -= new OnChangeEventHandler((Sender, r) => Leaddependency_OnChange(sender, e, sOneClick));
                        Leaddependency = null;
                    }
                    if (e.Info == SqlNotificationInfo.Update || e.Info == SqlNotificationInfo.Insert)
                    {
                        bool Flag = true;
                        //oCR = SubMethod();
                        LeadTrace(sOneClick, Flag);

                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.oTrace.Add(oCR.oTrace);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                oCResult.oResult = "Success";
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = " Mandatory Param: sOneClick =  " + sOneClick + "  is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        internal static SqlDependency leadQuoteDependecy = null;

        public CResult GetLeadData(string ID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "GetLeadData is used to get the oneclick data with the following Id";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "ID", sValue = ID });
                if (!string.IsNullOrEmpty(ID))//check mandatory params are passed or not
                {
                    using (var connection = new SqlConnection(_connString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(@"SELECT " + XIConstant.Key_XICrtdWhn + ",FKiSourceID,FKiClassID FROM [dbo].[Lead_T] WHERE FKiSourceID in(1,2,9)", connection))
                        {
                            command.Notification = null;

                            if (leadQuoteDependecy == null)
                            {
                                leadQuoteDependecy = new SqlDependency(command);
                                leadQuoteDependecy.OnChange += new OnChangeEventHandler((Sender, r) => dependency_OnChangeLead(Sender, r, ID));
                                // leadQuoteDependecy.OnChange += new OnChangeEventHandler(dependency_OnChangeLead);
                            }
                            if (connection.State == ConnectionState.Closed)
                                connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: ID = " + ID + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult dependency_OnChangeLead(object sender, SqlNotificationEventArgs e, string OneClickID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "dependency_OnChangeLead is main method for LeadData to get the data using oneclickid";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "OneClickID", sValue = OneClickID });
                if (!string.IsNullOrEmpty(OneClickID))//check mandatory params are passed or not
                {

                    if (leadQuoteDependecy != null)
                    {
                        leadQuoteDependecy = null;
                    }
                    var ID = OneClickID.Split(',').ToList();
                    foreach (var item in ID)
                    {
                        XIInfraQuoteReportDataComponent report = new XIInfraQuoteReportDataComponent();
                        List<CNV> oParams = new List<CNV>();
                        oParams.Add(new CNV { sName = "1ClickID", sValue = item });
                        oParams.Add(new CNV { sName = "Visualisation", sValue = "MinuteReportColours" });
                        var ers = report.XILoad(oParams);
                        if (e.Type == SqlNotificationType.Change || e.Info == SqlNotificationInfo.Insert)
                        {
                            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                            context.Clients.All.LeadData(ers.oResult, true);
                        }
                    }
                    //oCR = SubMethod();
                    oCR = GetLeadData(OneClickID);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.oTrace.Add(oCR.oTrace);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = "Success";
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: OneClickID = " + OneClickID + "  is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        [HttpPost]
        public ActionResult HeatMapReports1(string OneclicksID)
        {
            try
            {
                CResult ers = new CResult();
                var ListOneclicksID = OneclicksID.Split(',').ToList();
                foreach (var item in ListOneclicksID)
                {
                    XIInfraQuoteReportDataComponent report = new XIInfraQuoteReportDataComponent();
                    List<CNV> oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "1ClickID", sValue = item });
                    oParams.Add(new CNV { sName = "Visualisation", sValue = "MinuteReportColours" });
                    ers = report.XILoad(oParams);
                }
                return Json(ers.oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string sDatabase = SessionManager.CoreDatabase;
                Common.SaveErrorLog("ErrorLog: HeatMapReports method" + ex.ToString(), sDatabase);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        internal static SqlDependency MatrixDependency = null;

        public CResult GetMatrixTransactionData()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "GetMatrixTransactionData is used to get the tansaction data";//expalin about this method logic
            try
            {
                using (var connection = new SqlConnection(_connString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(@"SELECT sCode FROM [dbo].[XIMatrixTransaction_T]", connection))
                    {
                        command.Notification = null;
                        if (MatrixDependency == null)
                        {
                            MatrixDependency = new SqlDependency(command);
                            MatrixDependency.OnChange += new OnChangeEventHandler((sender, e) => dependency_MatrixTransactionOnChange(sender, e));
                            if (connection.State == ConnectionState.Closed)
                                connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult dependency_MatrixTransactionOnChange(object sender, SqlNotificationEventArgs e)
        {

            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "dependency_MatrixTransactionOnChange is used to update the transactions and it calls the GetMatrixTransactionData method";//expalin about this method logic
            try
            {
                if (MatrixDependency != null)
                {
                    MatrixDependency = null;
                    XIInfraQuoteReportDataComponent report = new XIInfraQuoteReportDataComponent();
                    var id = "7696".Split(',').ToList();
                    foreach (var item in id)
                    {
                        List<CNV> oParams = new List<CNV>();
                        oParams.Add(new CNV { sName = "1ClickID", sValue = item });
                        oParams.Add(new CNV { sName = "Visualisation", sValue = "MatrixReportColors" });
                        var ers = report.XILoad(oParams);
                        if (e.Info == SqlNotificationInfo.Insert)
                        {
                            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                            context.Clients.All.LeadData(ers.oResult, true);
                        }
                    }
                    //oCR = SubMethod();
                    oCR = GetMatrixTransactionData();
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.oTrace.Add(oCR.oTrace);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = "Success";
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        internal static SqlTableDependency<AllBOsforSignalR> _dependency = null;


        public ActionResult SqlDependencyTableForNotification(string sConnectionID = null)
        {
            List<string> Info = new List<string>();
            try
            {
                Info.Add("SqlDependencyTable method calling");
                if (_dependency == null)
                {
                    lock (_lockOnMe)
                    {
                        if (_dependency == null)
                        {
                            Info.Add("SqlDependencyTable method _dependency firing 1st time");
                            SqlCommand depCommand = new SqlCommand();
                            CResult Result = new CResult();
                            XIInfraCache oCache1 = new XIInfraCache();
                            string sOneClickName = "SignalR Settings Master Table";
                            XIInfraCache oCacheO = new XIInfraCache();
                            XIIComponent oCompI = new XIIComponent();
                            List<XIIBO> ListOfSignalRMasterTable = new List<XIIBO>();
                            XID1Click o1ClickD = (XID1Click)oCacheO.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                            XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                            Result = o1ClickC.GetList();
                            if (Result.bOK == true && Result.oResult != null)
                            {
                                ListOfSignalRMasterTable = ((Dictionary<string, XIIBO>)Result.oResult).Values.ToList();
                                if (ListOfSignalRMasterTable.Count() > 0)
                                {
                                    //XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                                    //var Con = "Data Source=LAPTOP-KPR78BKA\\SQLEXPRESS;initial catalog=New_NW_Setting_UKServerDev1;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True";
                                    using (var connection = new SqlConnection(_connString))
                                    {
                                        foreach (var ite in ListOfSignalRMasterTable)
                                        {
                                            connection.Open();
                                            string TableID = ite.Attributes["iTableID"].sValue;
                                            string QueryID = ite.Attributes["ID"].sValue;
                                            string QuerySelectedFields = ite.Attributes["sSelectedFields"].sValue;
                                            XIDBO oBOD = new XIDBO();
                                            XIInfraCache oCache = new XIInfraCache();
                                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, TableID.ToString());
                                            var TableName = oBOD.TableName;
                                            var BoName = oBOD.Name;

                                            Info.Add("SqlDependencyTable method Dependency table : " + TableName);
                                            _dependency = new SqlTableDependency<AllBOsforSignalR>(_connString, TableName);
                                            _dependency.OnChanged += new ChangedEventHandler<AllBOsforSignalR>((sender, e) => _Dependency_OnChanged(sender, e, BoName));
                                            _dependency.Start();
                                            connection.Close();
                                        }
                                    }
                                }
                                // }
                            }
                        }

                    }
                }
                string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                Common.SaveErrorLog(sInfo, sDatabase);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                Common.SaveErrorLog(sInfo, sDatabase);
                Common.SaveErrorLog("Exception: " + ex.ToString(), sDatabase);
                Common.SaveErrorLog("ErrorLog: XiSignalR SqlDependencyTable method" + ex.ToString(), sDatabase);
                throw ex;
            }
        }
        public static void _Dependency_OnChanged(object sender, RecordChangedEventArgs<AllBOsforSignalR> e, string BOName)
        {
            var HTML = "<ul class='list-group'>";
            if (_dependency != null && !string.IsNullOrEmpty(BOName))
            {
                //_dependency.OnChanged -= new ChangedEventHandler<AllBOsforSignalR>((sender1, e1) => _Dependency_OnChanged(sender, e, BOName));
                ////_dependency.Stop();
                ////connection.Close();
                //_dependency = null;

                XIInfraCache oCache = new XIInfraCache();
                //var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, BOName, null);
                //if (oBOD != null && !string.IsNullOrEmpty(oBOD.TableName))
                //{
                //    var Tablename = oBOD.TableName;
                //    string DataSourceID = oBOD.iDataSourceXIGUID.ToString();
                //    XIDXI oXID = new XIDXI();
                //    string sConnectionString = oXID.GetBODataSource(DataSourceID, oBOD.FKiApplicationID);
                //    if (!string.IsNullOrEmpty(sConnectionString))
                //    {
                //        using (var connection = new SqlConnection(sConnectionString))
                //        {
                //            connection.Open();
                //            _dependency = new SqlTableDependency<AllBOsforSignalR>(sConnectionString, Tablename);
                //            _dependency.OnChanged += new ChangedEventHandler<AllBOsforSignalR>((sender2, e2) => _Dependency_OnChanged(sender, e, BOName));
                //            //_dependency.Start();
                //            connection.Close();
                //        }
                //    }
                //}
                if (BOName.ToLower() == "aggregations")
                {
                    List<CNV> oparams = new List<CNV>();
                    CNV param = new CNV();
                    param.sName = "{XIP|" + BOName + ".id}";
                    param.sValue = e.Entity.ID.ToString();
                    oparams.Add(param);
                    //int iClickID = 25414;//Risk Data
                    XIInfraCache oCacheRisk = new XIInfraCache();
                    XID1Click o1ClickDe = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "SignalR Risk Data", null);
                    XID1Click o1ClickCr = (XID1Click)o1ClickDe.Clone(o1ClickDe);
                    o1ClickCr.ReplaceFKExpressions(oparams);
                    var Res = o1ClickCr.OneClick_Execute();
                    if (Res != null && Res.Count > 0)
                    {
                        foreach (var item in Res.Values)
                        {
                            HTML += "<div class=\"list-item\">" + item.Attributes["sfactorname"].sValue + " <span class=\"badge\">" + item.Attributes["message"].sValue + "</span></div>";
                        }
                    }
                }
                HTML += "</ul> ";
                NotifyHub NotificationHub = new NotifyHub();
                NotificationHub.NotificationMessages(e, BOName, HTML);
                //IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                //LeadContext.Clients.Client(sConnectionID).SignalRMessages(e, BOName, HTML);
            }
        }


        internal static SqlTableDependency<AllBOsforSignalR> _Flashdependency = null;
        public ActionResult GetFlashNotification()
        {
            try
            {
                if (_Flashdependency == null)
                {
                    lock (_lockOnMe)
                    {

                        if (_Flashdependency == null)
                        {
                            XIInfraCache oCache = new XIInfraCache();
                            var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XINotifications", null);
                            var Tablename = oBOD.TableName;
                            var BOName = oBOD.Name;
                            var DataSourceXIGUID = oBOD.iDataSourceXIGUID.ToString();
                            XIDXI oXID = new XIDXI();
                            string sConnectionString = oXID.GetBODataSource(DataSourceXIGUID, oBOD.FKiApplicationID);
                            //XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                            //var Con = "Data Source=LAPTOP-KPR78BKA\\SQLEXPRESS;initial catalog=New_NW_Setting_UKServerDev1;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True";
                            using (var connection = new SqlConnection(sConnectionString))
                            {

                                _Flashdependency = new SqlTableDependency<AllBOsforSignalR>(sConnectionString, Tablename);
                                _Flashdependency.OnChanged += new ChangedEventHandler<AllBOsforSignalR>((sender, e) => Dependency_ONChangedforNotification(sender, e, BOName));
                                _Flashdependency.Start();
                                //_dependency.Stop();
                                connection.Close();
                            }
                        }
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string sDatabase = SessionManager.CoreDatabase;
                Common.SaveErrorLog("ErrorLog: XiSignalR GetFlashNotification method" + ex.ToString(), sDatabase);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public static void SendCallbackRequest()
        {
            try
            {
                NotifyHub NotificationHub = new NotifyHub();
                List<XINotification> notificationList = new List<XINotification>();
                string sQuery = @"select ID, XIGUID, sSubject, sNotes, sCode, FKiBOIID, FKiOrgID, FKiBODIDXIGUID, FKiMasterNotificationIDXIGUID from XINotifications_T where sCode='CALLBACK' and dtSendWhen < GETDATE() and bIsSent = 0";
                XID1Click oXI1Click = new XID1Click();
                oXI1Click.Query = sQuery;
                oXI1Click.Name = "XINotifications";
                var QueryResult = oXI1Click.Execute_Query();
                if (QueryResult.Rows.Count > 0)
                {
                    notificationList = (from DataRow row in QueryResult.Rows
                                        select new XINotification
                                        {
                                            ID = Convert.ToInt32(row["ID"]),
                                            XIGUID = new Guid(row["XIGUID"].ToString()),
                                            FKiBODIDXIGUID = row["FKiBODIDXIGUID"].ToString() == String.Empty ? new Guid() : new Guid(row["FKiBODIDXIGUID"].ToString()),
                                            FKiBOIID = Convert.ToInt32(row["FKiBOIID"]),
                                            FKiOrgID = Convert.ToInt32(row["FKiOrgID"]),
                                            sCode = row["sCode"].ToString(),
                                            sSubject = row["sSubject"].ToString(),
                                            sNotes = row["sNotes"].ToString(),
                                            FKiMasterNotificationIDXIGUID = row["FKiMasterNotificationIDXIGUID"].ToString() == String.Empty ? new Guid() : new Guid(row["FKiMasterNotificationIDXIGUID"].ToString()),
                                        }).ToList();

                    notificationList.ForEach(n =>
                    {
                        NotificationMaster oMasterNotification = SignalRUtils.GetNotificationMaster(n.FKiMasterNotificationIDXIGUID.ToString(), n.FKiOrgID);
                        List<XIInfraUsers> users = SignalRUtils.GetUserNotificationDistributionList(oMasterNotification?.sUserDistributionList, n.FKiOrgID);
                        List<string> userToastNotifications = NotificationHub.SendNotificationMessages(users.Select(x => x.sUserName).ToList(), oMasterNotification?.sRoleDistributionList, n.FKiOrgID.ToString(), n, null, oMasterNotification.sTheme, oMasterNotification.bIsImportant, oMasterNotification.FKiLayoutIDXIGUID);
                        if(userToastNotifications.Count > 0)
                        {
                            SignalRUtils.InsertUserNotifications(userToastNotifications, users, oMasterNotification.sRoleDistributionList, n.FKiOrgID.ToString(), n.ID.ToString());
                            SignalRUtils.SetNotificationSent(n.XIGUID);
                        }
                        
                    });

                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void CheckCallbackRequest()
        {
            try
            {
                NotifyHub NotificationHub = new NotifyHub();
                List<XINotification> notificationList = new List<XINotification>();
                string sQuery = @"select XIGUID, iUserID, sSubject, sNotes, sCode, FKiBOIID, FKiOrgID, FKiBODIDXIGUID, FKiMasterNotificationIDXIGUID, sTheme, bIsSnoozed from XINotificationSchedule_T where sCode='CALLBACK' and dtScheduled < GETUTCDATE() and bIsProcessed = 0";
                XID1Click oXI1Click = new XID1Click();
                oXI1Click.Query = sQuery;
                oXI1Click.Name = "Insert NotificationSchedule";
                var QueryResult = oXI1Click.Execute_Query();
                if (QueryResult.Rows.Count > 0)
                {
                    notificationList = (from DataRow row in QueryResult.Rows
                                        select new XINotification
                                        {
                                            XIGUID = new Guid(row["XIGUID"].ToString()),
                                            FKiBODIDXIGUID = row["FKiBODIDXIGUID"].ToString() == String.Empty ? new Guid() : new Guid(row["FKiBODIDXIGUID"].ToString()),
                                            FKiBOIID = Convert.ToInt32(row["FKiBOIID"]),
                                            FKiOrgID = Convert.ToInt32(row["FKiOrgID"]),
                                            sCode = row["sCode"].ToString(),
                                            sSubject = row["sSubject"].ToString(),
                                            sNotes = row["sNotes"].ToString(),
                                            FKiMasterNotificationIDXIGUID = row["FKiMasterNotificationIDXIGUID"].ToString() == String.Empty ? new Guid() : new Guid(row["FKiMasterNotificationIDXIGUID"].ToString()),
                                            sTheme = row["sTheme"].ToString(),
                                            bIsSnoozed = Convert.ToBoolean(row["bIsSnoozed"].ToString() == String.Empty ? "False" : row["bIsSnoozed"]),
                                            iUserID = Convert.ToInt32(row["iUserID"]),
                                        }).ToList();

                    notificationList.ForEach(n =>
                    {
                        XIInfraCache oCache = new XIInfraCache();
                        XIDBO oNotifBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XINotifications");


                        XIIBO oBO = new XIIBO();
                        oBO.BOD = oNotifBOD;
                        oBO.Attributes["FKiBODIDXIGUID"] = new XIIAttribute
                        {
                            sName = "FKiBODIDXIGUID",
                            sValue = n.FKiBODIDXIGUID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["FKiBOIID"] = new XIIAttribute
                        {
                            sName = "FKiBOIID",
                            sValue = n.FKiBOIID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["FKiOrgID"] = new XIIAttribute
                        {
                            sName = "FKiOrgID",
                            sValue = n.FKiOrgID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["sCode"] = new XIIAttribute
                        {
                            sName = "sCode",
                            sValue = n.sCode,
                            bDirty = true,

                        };
                        oBO.Attributes["sSubject"] = new XIIAttribute
                        {
                            sName = "sSubject",
                            sValue = n.sSubject,
                            bDirty = true,
                        };
                        oBO.Attributes["sNotes"] = new XIIAttribute
                        {
                            sName = "sNotes",
                            sValue = n.sNotes,
                            bDirty = true,
                        };
                        oBO.Attributes["sTheme"] = new XIIAttribute
                        {
                            sName = "sTheme",
                            sValue = n.sTheme,
                            bDirty = true,
                        };
                        oBO.Attributes["FKiMasterNotificationIDXIGUID"] = new XIIAttribute
                        {
                            sName = "FKiMasterNotificationIDXIGUID",
                            sValue = n.FKiMasterNotificationIDXIGUID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["bIsSnoozed"] = new XIIAttribute
                        {
                            sName = "bIsSnoozed",
                            sValue = n.bIsSnoozed.ToString(),
                            bValue = n.bIsSnoozed,
                            bDirty = true,
                        };
                        oBO.Attributes["iUserID"] = new XIIAttribute
                        {
                            sName = "iUserID",
                            sValue = n.iUserID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["iCategory"] = new XIIAttribute
                        {
                            sName = "iCategory",
                            sValue = "20",
                            bDirty = true,
                        };
                        oBO.Attributes["sAlertMessage"] = new XIIAttribute
                        {
                            sName = "sAlertMessage",
                            sValue = n.sSubject ?? n.sCode,
                            bDirty = true,
                        };
                        oBO.Save(oBO);
                        SignalRUtils.SetNotificationSent(n.XIGUID);
                    });

                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void CheckNotificationSchedule(string sCode)
        {
            try
            {
                NotifyHub NotificationHub = new NotifyHub();
                List<XINotification> notificationList = new List<XINotification>();
                string sQuery = @"select XIGUID, iUserID, sSubject, sNotes, sCode, FKiBOIID, FKiOrgID, FKiBODIDXIGUID, FKiMasterNotificationIDXIGUID, sTheme, bIsSnoozed, iSendToUserID, iSendToRoleID from XINotificationSchedule_T where sCode='" + sCode + "' and dtScheduled < GETUTCDATE() and bIsProcessed = 0";
                XID1Click oXI1Click = new XID1Click();
                oXI1Click.Query = sQuery;
                oXI1Click.Name = "Insert NotificationSchedule";
                var QueryResult = oXI1Click.Execute_Query();
                if (QueryResult.Rows.Count > 0)
                {
                    notificationList = (from DataRow row in QueryResult.Rows
                                        select new XINotification
                                        {
                                            XIGUID = new Guid(row["XIGUID"].ToString()),
                                            FKiBODIDXIGUID = row["FKiBODIDXIGUID"].ToString() == string.Empty ? new Guid() : new Guid(row["FKiBODIDXIGUID"].ToString()),
                                            FKiBOIID = Convert.ToInt32(row["FKiBOIID"].ToString() == string.Empty ? "0" : row["FKiBOIID"]),
                                            FKiOrgID = Convert.ToInt32(row["FKiOrgID"]),
                                            sCode = row["sCode"].ToString(),
                                            sSubject = row["sSubject"].ToString(),
                                            sNotes = row["sNotes"].ToString(),
                                            FKiMasterNotificationIDXIGUID = row["FKiMasterNotificationIDXIGUID"].ToString() == String.Empty ? new Guid() : new Guid(row["FKiMasterNotificationIDXIGUID"].ToString()),
                                            sTheme = row["sTheme"].ToString(),
                                            bIsSnoozed = Convert.ToBoolean(row["bIsSnoozed"].ToString() == string.Empty ? "False" : row["bIsSnoozed"]),
                                            iUserID = Convert.ToInt32(row["iUserID"].ToString() == string.Empty ? "0" : row["iUserID"]),
                                            iSendToUserID = row["iSendToUserID"].ToString() == string.Empty ? (int?)null : Convert.ToInt32(row["iSendToUserID"]),
                                            iSendToRoleID = row["iSendToRoleID"].ToString() == string.Empty ? (int?)null : Convert.ToInt32(row["iSendToRoleID"])
                                        }).ToList();

                    notificationList.ForEach(n =>
                    {
                        XIInfraCache oCache = new XIInfraCache();
                        XIDBO oNotifBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XINotifications");


                        XIIBO oBO = new XIIBO();
                        oBO.BOD = oNotifBOD;
                        oBO.Attributes["FKiBODIDXIGUID"] = new XIIAttribute
                        {
                            sName = "FKiBODIDXIGUID",
                            sValue = n.FKiBODIDXIGUID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["FKiBOIID"] = new XIIAttribute
                        {
                            sName = "FKiBOIID",
                            sValue = n.FKiBOIID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["FKiOrgID"] = new XIIAttribute
                        {
                            sName = "FKiOrgID",
                            sValue = n.FKiOrgID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["sCode"] = new XIIAttribute
                        {
                            sName = "sCode",
                            sValue = n.sCode,
                            bDirty = true,

                        };
                        oBO.Attributes["sSubject"] = new XIIAttribute
                        {
                            sName = "sSubject",
                            sValue = n.sSubject,
                            bDirty = true,
                        };
                        oBO.Attributes["sNotes"] = new XIIAttribute
                        {
                            sName = "sNotes",
                            sValue = n.sNotes,
                            bDirty = true,
                        };
                        oBO.Attributes["sTheme"] = new XIIAttribute
                        {
                            sName = "sTheme",
                            sValue = n.sTheme,
                            bDirty = true,
                        };
                        oBO.Attributes["FKiMasterNotificationIDXIGUID"] = new XIIAttribute
                        {
                            sName = "FKiMasterNotificationIDXIGUID",
                            sValue = n.FKiMasterNotificationIDXIGUID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["bIsSnoozed"] = new XIIAttribute
                        {
                            sName = "bIsSnoozed",
                            sValue = n.bIsSnoozed.ToString(),
                            bValue = n.bIsSnoozed,
                            bDirty = true,
                        };
                        oBO.Attributes["iUserID"] = new XIIAttribute
                        {
                            sName = "iUserID",
                            sValue = n.iUserID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["iCategory"] = new XIIAttribute
                        {
                            sName = "iCategory",
                            sValue = "20",
                            bDirty = true,
                        };
                        oBO.Attributes["sAlertMessage"] = new XIIAttribute
                        {
                            sName = "sAlertMessage",
                            sValue = n.sSubject ?? n.sCode,
                            bDirty = true,
                        };
                        oBO.Attributes["iSendToUserID"] = new XIIAttribute
                        {
                            sName = "iSendToUserID",
                            sValue = n.iSendToUserID == null ? "" : n.iSendToUserID.ToString(),
                            bDirty = true,
                        };
                        oBO.Attributes["iSendToRoleID"] = new XIIAttribute
                        {
                            sName = "iSendToRoleID",
                            sValue = n.iSendToRoleID == null ? "" : n.iSendToRoleID.ToString(),
                            bDirty = true,
                        };
                        oBO.Save(oBO);
                        SignalRUtils.SetNotificationSent(n.XIGUID);
                    });

                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void Dependency_ONChangedforNotification(object sender, RecordChangedEventArgs<AllBOsforSignalR> e, string BOName)
        {
            //Console.Write("Dependency on change with BoName :" + BOName + " With id is : " + e.Entity.ID);
            //_Flashdependency = new SqlTableDependency<AllBOsforSignalR>(_connString, Tablename);
            //_Flashdependency.OnChanged -= new ChangedEventHandler<AllBOsforSignalR>((sender, e) => Dependency_ONChangedforNotification(sender, e, BOName));
            if (_Flashdependency != null && e.ChangeType.ToString() == "Insert")
            {
                //_Flashdependency.OnChanged -= new ChangedEventHandler<AllBOsforSignalR>((sender1, e1) => Dependency_ONChangedforNotification(sender, e, BOName));
                //_Flashdependency.Stop();
                ////_dependency.Stop();
                ////connection.Close();
                //_Flashdependency = null;
                //XiSignalRController scr = new XiSignalRController();
                //scr.GetFlashNotification();
                // }

                //IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                //LeadContext.Clients.All.GetFlashMessages(e, BOName);

                //LeadContext.Clients.User(e.Entity.iUserID.ToString()).GetFlashMessages(e, BOName);
                
                NotifyHub NotificationHub = new NotifyHub();

                if (e.Entity.bIsSnoozed) {
                    NotificationMaster oMasterNotification = SignalRUtils.GetNotificationMaster(e.Entity.FKiMasterNotificationIDXIGUID.ToString(), e.Entity.FKiOrgID);
                    List<XIInfraUsers> users = SignalRUtils.GetUserNotificationDistributionList(e.Entity.iUserID.ToString(), e.Entity.FKiOrgID);
                    List<string> userToastNotifications = NotificationHub.SendNotificationMessages(users.Select(x => x.sUserName).ToList(), null, e.Entity.FKiOrgID.ToString(), e, BOName, oMasterNotification.sTheme, oMasterNotification.bIsImportant);
                    SignalRUtils.InsertUserNotifications(userToastNotifications, users, null, e.Entity.FKiOrgID.ToString(), e.Entity.ID.ToString());
                    List<string> userNames = users.Select(x => x.sUserName).ToList();
                    userNames = new HashSet<string>(userNames).ToList();
                    NotificationHub.SendUpdateNotificationCountSignal(userNames, oMasterNotification.iCategory);
                }
                else
                {
                    if (e.Entity.sCode == "MANUAL")
                    {
                        NotificationMaster oMasterNotification = SignalRUtils.GetNotificationMaster(e.Entity.FKiMasterNotificationIDXIGUID.ToString(), e.Entity.FKiOrgID);
                        List<XIInfraUsers> users = SignalRUtils.GetUserNotificationDistributionList(e.Entity.iSendToUserID.ToString(), e.Entity.FKiOrgID);
                        List<string> userToastNotifications = NotificationHub.SendNotificationMessages(users.Select(x => x.sUserName).ToList(), e.Entity.iSendToRoleID.ToString(), e.Entity.FKiOrgID.ToString(), e, BOName,
                                                                oMasterNotification.sTheme, oMasterNotification.bIsImportant,
                                                                oMasterNotification.FKiLayoutIDXIGUID, oMasterNotification.iLeft, oMasterNotification.iTop, oMasterNotification.iWidth, oMasterNotification.iHeight);
                        SignalRUtils.InsertUserNotifications(userToastNotifications, users, e.Entity.iSendToRoleID.ToString(), e.Entity.FKiOrgID.ToString(), e.Entity.ID.ToString());
                        List<string> userNames = users.Select(x => x.sUserName).ToList();
                        userNames.AddRange(SignalRUtils.GetUsersFromRoleList(e.Entity.iSendToRoleID.ToString(), e.Entity.FKiOrgID.ToString()).Select(x => x.sUserName).ToList());
                        userNames = new HashSet<string>(userNames).ToList();
                        NotificationHub.SendUpdateNotificationCountSignal(userNames, oMasterNotification.iCategory);
                    }
                    else
                    {
                        NotificationMaster oMasterNotification = SignalRUtils.GetNotificationMaster(e.Entity.FKiMasterNotificationIDXIGUID.ToString(), e.Entity.FKiOrgID);
                        List<XIInfraUsers> users = SignalRUtils.GetUserNotificationDistributionList(oMasterNotification?.sUserDistributionList, e.Entity.FKiOrgID);
                        List<string> userToastNotifications = NotificationHub.SendNotificationMessages(users.Select(x => x.sUserName).ToList(), oMasterNotification?.sRoleDistributionList, e.Entity.FKiOrgID.ToString(), e, BOName,
                            oMasterNotification.sTheme, oMasterNotification.bIsImportant,
                            oMasterNotification.FKiLayoutIDXIGUID, oMasterNotification.iLeft, oMasterNotification.iTop, oMasterNotification.iWidth, oMasterNotification.iHeight);
                        SignalRUtils.InsertUserNotifications(userToastNotifications, users, oMasterNotification.sRoleDistributionList, e.Entity.FKiOrgID.ToString(), e.Entity.ID.ToString());
                        List<string> userNames = users.Select(x => x.sUserName).ToList();
                        userNames.AddRange(SignalRUtils.GetUsersFromRoleList(oMasterNotification.sRoleDistributionList, e.Entity.FKiOrgID.ToString()).Select(x => x.sUserName).ToList());
                        userNames = new HashSet<string>(userNames).ToList();
                        NotificationHub.SendUpdateNotificationCountSignal(userNames, oMasterNotification.iCategory);
                        
                    }
                }

                //XIIBO oBOI = new XIIBO();
                //XIDBO oBOD = new XIDBO();
                //XIInfraCache oCache = new XIInfraCache();
                //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, BOName, null);

                //oBOI.SetAttribute("ID", e.Entity.ID.ToString());
                //oBOI.SetAttribute("XIGUID", e.Entity.XIGUID.ToString());
                //oBOI.SetAttribute("iStatus", "10");
                //oBOI.BOD = oBOD;
                //oBOI.Save(oBOI);

                ////why this code - we are showing notification - again we are inseting into -Insert NotificationSchedule 
                ////is it only if user want to show after X time right, not always
                //oBOI = new XIIBO();
                //oBOD = new XIDBO();
                //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Insert NotificationSchedule", null);
                //oBOI.SetAttribute("FKiBoid", e.Entity.FKiBoid.ToString());
                ////oBOI.SetAttribute("sBOName", e.Entity.sBOName.ToString());
                //oBOI.SetAttribute("FKiBoidXIGUID", e.Entity.FKiBoidXIGUID.ToString());
                //oBOI.SetAttribute("sAlertType", e.Entity.sAlertType.ToString());
                //oBOI.SetAttribute("sAlertMessage", e.Entity.sAlertMessage.ToString() + e.Entity.iInstanceID.ToString());
                //oBOI.SetAttribute("iUserID", e.Entity.iUserID.ToString());
                //oBOI.SetAttribute("iRoleID", e.Entity.iRoleID.ToString());
                //oBOI.SetAttribute("FKiNotificationID", e.Entity.ID.ToString());
                //oBOI.SetAttribute("FKiNotificationIDXIGUID", e.Entity.XIGUID.ToString());
                //oBOI.BOD = oBOD;
                //oBOI.Save(oBOI);
            }
        }
        //[HttpPost]
        //public ActionResult SavingSnooze(string NotificationID, int SnoozeTime)
        //{
        //    XIIBO oBOI = new XIIBO();
        //    XIInfraCache oCache = new XIInfraCache();

        //    XIIXI oXIXI = new XIIXI();
        //    CNV Params = new CNV();
        //    List<CNV> ParamsList = new List<CNV>();
        //    Params.sName = "FKiNotificationIDXIGUID";
        //    Params.sValue = NotificationID;
        //    ParamsList.Add(Params);
        //    var SignalRNotificationI = oXIXI.BOI("Insert NotificationSchedule", "", "", ParamsList);

        //    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Insert NotificationSchedule", null);
        //    oBOI.SetAttribute("ID", SignalRNotificationI.Attributes["ID"].sValue);
        //    oBOI.SetAttribute("XIGUID", SignalRNotificationI.Attributes["XIGUID"].sValue);
        //    //var gettime=DateTime.Now;
        //    var Schedulartime = DateTime.Now.AddMinutes(SnoozeTime);
        //    oBOI.SetAttribute("dtScheduled", Schedulartime.ToString());
        //    oBOI.BOD = oBOD;
        //    oBOI.Save(oBOI);
        //    return Json("Success");
        //}
        [HttpPost]
        public ActionResult SavingSnooze(string NotificationID, int SnoozeTime)
        {
            List<string> Info = new List<string>();
            CResult oCResult = new CResult();
            try
            {
                Info.Add("Saving snooze notification schedule");
                XIIBO oBOI = new XIIBO();
                XIInfraCache oCache = new XIInfraCache();
                int iUserID = SessionManager.UserID;

                XIIXI oXIXI = new XIIXI();
                CNV Params = new CNV();
                List<CNV> ParamsList = new List<CNV>();
                Params.sName = "XIGUID";
                Params.sValue = NotificationID;
                ParamsList.Add(Params);
                var oNotif = oXIXI.BOI("XINotifications", "", "", ParamsList);

                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Insert NotificationSchedule", null);
                DateTime Schedulartime = DateTime.UtcNow.AddMinutes(SnoozeTime);

                Info.Add("Schedular time: " + Schedulartime.ToString("yyyy-MM-dd HH:mm:ss"));
                Info.Add("iUserID: " + iUserID.ToString());

                string sInfo = string.Join(",\r\n ", Info);
                oCResult.sMessage = " INFO: " + sInfo;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.iLogLevel = (int)EnumXIErrorPriority.Low;
                oXID.SaveErrortoDB(oCResult);


                XIIBO oBO = new XIIBO();
                oBO.BOD = oBOD;
                oBO.Attributes["iUserID"] = new XIIAttribute
                {
                    sName = "iUserID",
                    sValue = iUserID.ToString(),
                    bDirty = true,
                };
                oBO.Attributes["FKiBODIDXIGUID"] = new XIIAttribute
                {
                    sName = "FKiBODIDXIGUID",
                    sValue = oNotif.Attributes["FKiBODIDXIGUID"].sValue,
                    bDirty = true,
                };
                oBO.Attributes["FKiBOIID"] = new XIIAttribute
                {
                    sName = "FKiBOIID",
                    sValue = oNotif.Attributes["FKiBOIID"].sValue,
                    bDirty = true,
                };
                oBO.Attributes["FKiOrgID"] = new XIIAttribute
                {
                    sName = "FKiOrgID",
                    sValue = oNotif.Attributes["FKiOrgID"].sValue,
                    bDirty = true,
                };
                oBO.Attributes["sCode"] = new XIIAttribute
                {
                    sName = "sCode",
                    sValue = oNotif.Attributes["sCode"].sValue,
                    bDirty = true,
                };
                oBO.Attributes["sSubject"] = new XIIAttribute
                {
                    sName = "sSubject",
                    sValue = oNotif.Attributes["sSubject"].sValue,
                    bDirty = true,
                };
                oBO.Attributes["sNotes"] = new XIIAttribute
                {
                    sName = "sNotes",
                    sValue = oNotif.Attributes["sNotes"].sValue,
                    bDirty = true,
                };
                oBO.Attributes["sTheme"] = new XIIAttribute
                {
                    sName = "sTheme",
                    sValue = oNotif.Attributes["sTheme"].sValue,
                    bDirty = true,
                };
                oBO.Attributes["FKiMasterNotificationIDXIGUID"] = new XIIAttribute
                {
                    sName = "FKiMasterNotificationIDXIGUID",
                    sValue = oNotif.Attributes["FKiMasterNotificationIDXIGUID"].sValue,
                    bDirty = true,
                };
                oBO.Attributes["dtScheduled"] = new XIIAttribute
                {
                    sName = "dtScheduled",
                    sValue = Schedulartime.ToString("yyyy-MM-dd HH:mm:ss"),
                    bDirty = true,
                };
                oBO.Attributes["bIsProcessed"] = new XIIAttribute
                {
                    sName = "bIsProcessed",
                    sValue = "False",
                    bValue = false,
                    bDirty = true,
                };
                oBO.Attributes["bIsSnoozed"] = new XIIAttribute
                {
                    sName = "bIsSnoozed",
                    sValue = "True",
                    bValue = true,
                    bDirty = true,
                };
                CResult oCR = oBO.Save(oBO);
                if (oCR.bOK && oCR.oResult != null)
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failure");
                }
            }
            catch(Exception ex)
            {
                string sInfo = string.Join(",\r\n ", Info);
                oCResult.sMessage = "Error: " + ex.Message + " INFO: " + sInfo;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                oXID.SaveErrortoDB(oCResult);
                return Json("Failure");
            }
        }


        internal static SqlTableDependency<AllBOsforSignalR> _dependencyForInbox = null;
        public ActionResult InboxResult(string i1ClickID, string NodeID)
        {
            List<string> Info = new List<string>();
            try
            {
                Info.Add("InboxResult method calling");
                lock (OneclickGUidsforInbox)
                {
                    if (!OneclickGUidsforInbox.Contains(i1ClickID))
                    {
                        Info.Add("InboxResult method _dependency firing 1st time");
                        SqlCommand depCommand = new SqlCommand();
                        CResult Result = new CResult();
                        XIInfraCache oCacheO = new XIInfraCache();
                        List<XIIBO> ListOfSignalRMasterTable = new List<XIIBO>();
                        XID1Click o1ClickD = (XID1Click)oCacheO.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID);
                        XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                        using (var connection = new SqlConnection(_connString))
                        {
                            connection.Open();
                            XIDBO oBOD = new XIDBO();
                            XIInfraCache oCache = new XIInfraCache();
                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickC.BOIDXIGUID.ToString());
                            var TableName = oBOD.TableName;
                            var BoName = oBOD.Name;
                            var UserID = SessionManager.UserID;
                            Info.Add("InboxResult method Dependency table : " + TableName);
                            _dependencyForInbox = new SqlTableDependency<AllBOsforSignalR>(_connString, TableName);
                            _dependencyForInbox.OnChanged += new ChangedEventHandler<AllBOsforSignalR>((sender, e) => _Dependency_OnChangedForInbox(sender, e, BoName, i1ClickID, NodeID, null, UserID.ToString()));
                            OneclickGUidsforInbox.Add(i1ClickID);
                            _dependencyForInbox.Start();
                            connection.Close();
                        }
                    }
                }
                string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                Common.SaveErrorLog(sInfo, sDatabase);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                Common.SaveErrorLog(sInfo, sDatabase);
                Common.SaveErrorLog("Exception: " + ex.ToString(), sDatabase);
                Common.SaveErrorLog("ErrorLog: XiSignalR InboxResult method" + ex.ToString(), sDatabase);
                throw ex;
            }
        }
        public static void _Dependency_OnChangedForInbox(object sender, RecordChangedEventArgs<AllBOsforSignalR> e, string BOName, string o1ClickID, string NodeID, string TraceMap = null, string UserID = null)
        {
            NotifyHub NotificationHub = new NotifyHub();

            if (_dependencyForInbox != null && !string.IsNullOrEmpty(BOName) && !string.IsNullOrEmpty(NodeID) && string.IsNullOrEmpty(TraceMap))
            {
                //XiLinkController Xilink = new XiLinkController();
                //var Count = Xilink.AutoRefereshing(o1ClickID, UserID);
                //IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                NotificationHub.SendInboxCountUpdateSignal(e.Entity.FKiOrgID.ToString(), o1ClickID, NodeID);
                //LeadContext.Clients.All.ListAddRefresh(new { BOName = "Lead_T", BODID = "d9ae2558-9acc-44f9-918d-aea8ad0ff962", BOIID = 10  });
            }
            else if (_dependencyForInbox != null && !string.IsNullOrEmpty(TraceMap) && TraceMap.ToLower() == "trace")
            {

                XID1Click o1ClickD = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                List<CNV> oWhrParam = new List<CNV>();
                XIInfraCache oCache = new XIInfraCache();
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, o1ClickID);
                o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                o1ClickC.Name = BOName;
                o1ClickC.Query = o1ClickC.Query;
                DataTable dt = o1ClickC.Execute_Query();
                var leadcount = dt.Rows.Count;
                foreach (DataRow items in dt.Rows)
                {
                    if (items.ItemArray.Count() > 0)
                    {
                        oWhrParam.Add(new CNV { sName = items.ItemArray[0].ToString(), sValue = items.ItemArray[1].ToString() });
                    }
                }
                //IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                //LeadContext.Clients.All.LeadtraceFlowChat(oWhrParam, true);
                NotificationHub.SendLeadTraceFlowChartSignal(e.Entity.FKiOrgID.ToString(), oWhrParam, true);
            }
        }

        internal static SqlTableDependency<AllBOsforSignalR> _dependencyForKPI = null;
        public ActionResult KPIData(string i1ClickID, string KPIID, string iXIComponentID, List<CNV> oParams)
        {
            List<string> Info = new List<string>();
            try
            {
                Info.Add("KPIData method calling");
                lock (OneclickGUidsforKPI)
                {
                    if (!OneclickGUidsforKPI.Contains(i1ClickID))
                    {
                        Info.Add("KPIData method _dependency firing 1st time");
                        SqlCommand depCommand = new SqlCommand();
                        CResult Result = new CResult();
                        XIInfraCache oCacheO = new XIInfraCache();
                        List<XIIBO> ListOfSignalRMasterTable = new List<XIIBO>();
                        XID1Click o1ClickD = (XID1Click)oCacheO.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID);
                        XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                        using (var connection = new SqlConnection(_connString))
                        {
                            connection.Open();
                            XIDBO oBOD = new XIDBO();
                            XIInfraCache oCache = new XIInfraCache();
                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickC.BOIDXIGUID.ToString());
                            var TableName = oBOD.TableName;
                            var BoName = oBOD.Name;

                            Info.Add("KPIData method Dependency table : " + TableName);
                            _dependencyForKPI = new SqlTableDependency<AllBOsforSignalR>(_connString, TableName);
                            _dependencyForKPI.OnChanged += new ChangedEventHandler<AllBOsforSignalR>((sender, e) => _Dependency_OnChangedForKPI(sender, e, BoName, i1ClickID, KPIID, iXIComponentID, oParams));
                            OneclickGUidsforKPI.Add(i1ClickID);
                            _dependencyForKPI.Start();
                            connection.Close();
                        }
                    }
                }
                string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                Common.SaveErrorLog(sInfo, sDatabase);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                Common.SaveErrorLog(sInfo, sDatabase);
                Common.SaveErrorLog("Exception: " + ex.ToString(), sDatabase);
                Common.SaveErrorLog("ErrorLog: XiSignalR KPIData method" + ex.ToString(), sDatabase);
                throw ex;
            }
        }
        public static void _Dependency_OnChangedForKPI(object sender, RecordChangedEventArgs<AllBOsforSignalR> e, string BOName, string o1ClickID, string KPIID, string iXIComponentID, List<CNV> oParam)
        {
            NotifyHub notifyHub = new NotifyHub();
            if (_dependencyForKPI != null && !string.IsNullOrEmpty(BOName))
            {
                //IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                //LeadContext.Clients.All.KPIResult(iXIComponentID, Guid.NewGuid().ToString(), oParam, KPIID);
                notifyHub.SendKPIUpdateSignal(e.Entity.FKiOrgID.ToString(), iXIComponentID, Guid.NewGuid().ToString(), oParam, KPIID);
            }
        }

        internal static SqlTableDependency<AllBOsforSignalR> _dependencyForCommunication = null;

        public ActionResult CommunicationProcessing(string PCGUIDs)
        {
            List<string> Info = new List<string>();
            try
            {
                Info.Add("CommunicationProcessing method calling");
                if (_dependencyForCommunication == null)
                {
                    lock (_lockOnMe)
                    {
                        if (_dependencyForCommunication == null)
                        {
                            var PCIDXIGUID = PCGUIDs.Split(',').ToList();
                            Info.Add("CommunicationProcessing method _dependency firing 1st time");
                            SqlCommand depCommand = new SqlCommand();
                            CResult Result = new CResult();
                            XIInfraCache oCacheO = new XIInfraCache();
                            XIDBO oBOD = new XIDBO();
                            XIInfraCache oCache = new XIInfraCache();
                            var sSessionID = Guid.NewGuid().ToString();
                            string sNewGUID = Guid.NewGuid().ToString();
                            SignalR oSignalR = new SignalR();
                            //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommunicationI", null);
                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommEmailIn", null);

                            var TableName = oBOD.TableName;
                            var BoName = oBOD.Name;
                            var DataSourceXIGUID = oBOD.iDataSourceXIGUID.ToString();
                            XIDXI oXID = new XIDXI();
                            string sConnectionString = oXID.GetBODataSource(DataSourceXIGUID, oBOD.FKiApplicationID);
                            using (var connection = new SqlConnection(sConnectionString))
                            {
                                connection.Open();
                                Info.Add("CommunicationProcessing method Dependency table : " + TableName);
                                _dependencyForCommunication = new SqlTableDependency<AllBOsforSignalR>(sConnectionString, TableName);
                                _dependencyForCommunication.OnChanged += new ChangedEventHandler<AllBOsforSignalR>((sender, e) => _Dependency_OnChangedForCommunication(sender, e, BoName, PCIDXIGUID, sSessionID, sNewGUID));
                                _dependencyForCommunication.Start();
                                connection.Close();
                            }
                            string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                            Common.SaveErrorLog(sInfo, sDatabase);
                        }
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                Common.SaveErrorLog(sInfo, sDatabase);
                Common.SaveErrorLog("Exception: " + ex.ToString(), sDatabase);
                Common.SaveErrorLog("ErrorLog: XiSignalR CommunicationProcessing method" + ex.ToString(), sDatabase);
                // as we got into exception reseting _dependencyForCommunication
                //try
                //{
                //    if (_dependencyForCommunication != null){
                //        _dependencyForCommunication.Dispose();
                //        _dependencyForCommunication = null;
                //    }
                //}
                //catch (Exception e) {
                //    Common.SaveErrorLog("Exception while disposing: " + e.ToString(), sDatabase);
                //}
                //throw ex;
            }
            return null;
        }
        public static void _Dependency_OnChangedForCommunication(object sender, RecordChangedEventArgs<AllBOsforSignalR> e, string BOName, List<string> PCIDXIGUID, string sSessionID, string sNewGUID)
        {
            XIInfraCache oCache = new XIInfraCache();
            XIDAlgorithm oAlogD = new XIDAlgorithm();
            SignalR oSignalR = new SignalR();

            List<CNV> oNVsList = new List<CNV>();

            //oNVsList.Add(new CNV { sName = "{-FKiOrgID}", sValue = e.Entity.FKiOrgID.ToString() });old
            oNVsList.Add(new CNV { sName = "{XIP|iOrgID}", sValue = e.Entity.FKiOrgID.ToString() });
            oNVsList.Add(new CNV { sName = "{XIP|iInstanceID}", sValue = e.Entity.ID.ToString() });
            oNVsList.Add(new CNV { sName = "{-iInstanceID}", sValue = e.Entity.ID.ToString() });
            var Importtype = e.Entity.iImportType.ToString();
            oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
            if (Convert.ToInt32(Importtype) != 10)
            {
                if (_dependencyForCommunication != null && !string.IsNullOrEmpty(BOName))
                {
                    foreach (var PCGUID in PCIDXIGUID)
                    {
                        if (PCGUID != null)
                        {
                            oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, PCGUID.ToString());
                        }
                        var Response = oAlogD.Execute_XIAlgorithm(sSessionID, sNewGUID, null, oSignalR);
                    }
                    oCache.Clear_GuidCache(sSessionID, sNewGUID);
                    IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    LeadContext.Clients.All.EmailCommunication();
                }
            }
        }


    }
}
