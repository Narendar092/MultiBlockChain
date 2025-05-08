using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraKPICircleComponent : XIDefinitionBase
    {
        public CResult XILoad(List<CNV> oParams)
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
                var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                string iClickID = oParams.Where(m => m.sName == XIConstant.Param_i1ClickID).Select(m => m.sValue).FirstOrDefault();
                string Title = oParams.Where(m => m.sName == "sTitle").Select(m => m.sValue).FirstOrDefault();
                List<XIKPICircle> KPIs = new List<XIKPICircle>();
                //string iClickID = "10966,10981";
                var Clicks = iClickID.Split(',').ToList();
                int j = 0;
                foreach (var i1CID in Clicks)
                {
                    var TotalResult = 0;
                    XIInfraCache oCache = new XIInfraCache();
                    XID1Click o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1CID.ToString());
                    var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                    List<CNV> nParams = new List<CNV>();
                    nParams = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    o1ClickC.ReplaceFKExpressions(nParams);
                    var oResult = o1ClickC.OneClick_Run();
                    foreach (var data in oResult.Values.ToList())
                    {
                        if (data != null && data.Attributes.Count() > 0)
                        {
                            var Value = data.AttributeI("column1").sValue;
                            TotalResult = Convert.ToInt32(Value);
                        }
                    }

                    var iCount = 0;
                    var target = 20;
                    KPICircleColors colors = new KPICircleColors();
                    KPIIconColors iconscolor = new KPIIconColors();
                    List<string> color = new List<string>();
                    List<string> iconcolor = new List<string>();
                    foreach (var items in colors)
                    {
                        string str = Convert.ToString(items.KPIColor);
                        color.Add(str);
                    }
                    foreach (var items in iconscolor)
                    {
                        string str = Convert.ToString(items.KPIColor);
                        iconcolor.Add(str);
                    }
                    XIKPICircle kpi = new XIKPICircle();
                    iCount = Convert.ToInt32(TotalResult);
                    double percentage = (double)iCount / target;
                    int completed = (int)Math.Round(percentage * 100, 0);
                    if (target == 0)
                    {
                        completed = 0;
                    }
                    kpi.Name = o1ClickD.Name;
                    kpi.ShowAs = o1ClickD.Title;
                    kpi.Visibility = "true";
                    kpi.ReportID = o1ClickD.ID;
                    kpi.InnerReportID = o1ClickD.InnerReportID;
                    kpi.KPIPercent = completed;
                    kpi.KPIValue = completed + "%";
                    kpi.KPICircleColor = color[j];
                    kpi.KPIIconColor = iconcolor[j];
                    kpi.KPIIcon = Title;
                    KPIs.Add(kpi);
                    j++;
                }
                oCResult.oResult = KPIs;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;

                //ModelDbContext dbCore = new ModelDbContext(sDatabase);
                //string Query = "";
                //int target = 0, com;
                //int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
                //var Role = dbCore.XIAppRoles.Find(UserRoleID);
                //List<UserReports> Reports = new List<UserReports>();
                //List<string> Vistiblity = new List<string>();
                //int DisplayType = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.KPICircle.ToString());
                //int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
                //if (ReportID != "" && ReportID != null)
                //{
                //    var IDs = ReportID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //    foreach (var id in IDs)
                //    {
                //        int ID = Convert.ToInt32(id.Split('-')[0]);
                //        UserReports userreport = dbContext.UserReports.Where(m => m.RoleID == UserRoleID && m.ReportID == ID && m.Location == DType && m.DisplayAs == DisplayType).FirstOrDefault();
                //        Reports.Add(userreport);
                //        Vistiblity.Add(id.Split('-')[1]);
                //    }
                //}
                //else
                //{
                //    List<UserReports> AllReports = dbContext.UserReports.Where(m => m.RoleID == UserRoleID).Where(m => m.Location == DType).Where(m => m.DisplayAs == DisplayType).ToList();
                //    foreach (var item in AllReports)
                //    {
                //        Reports.Add(item);
                //        Vistiblity.Add("true");
                //    }
                //}
                //List<XIKPICircle> KPIs = new List<XIKPICircle>();
                //KPICircleColors colors = new KPICircleColors();
                //KPIIconColors iconscolor = new KPIIconColors();
                //List<string> color = new List<string>();
                //List<string> iconcolor = new List<string>();
                //Common Com = new Common();
                //foreach (var items in colors)
                //{
                //    string str = Convert.ToString(items.KPIColor);
                //    color.Add(str);
                //}
                //foreach (var items in iconscolor)
                //{
                //    string str = Convert.ToString(items.KPIColor);
                //    iconcolor.Add(str);
                //}
                //int j = 0;
                //var TotReports = dbContext.Reports.ToList();
                //var AllUserReports = dbContext.UserReports.ToList();
                //foreach (var items in Reports)
                //{
                //    Reports report = TotReports.Where(m => m.ID == items.ReportID).FirstOrDefault();
                //    Query = report.Query;
                //    UserReports ureport = AllUserReports.Where(m => m.RoleID == items.RoleID).Where(m => m.ReportID == items.ReportID).FirstOrDefault();
                //    target = ureport.Target;
                //    string UserIDs = Com.GetSubUsers(UserID, OrgID, sDatabase, sOrgName);
                //    if (Query != null && Query.Length > 0)
                //    {
                //        Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, UserID, OrgID, 0, 0);
                //        if (ClassFilter != 0 || DateFilter != 0)
                //        {
                //            Query = ServiceUtil.ModifyQuery(Query, OrgID, UserIDs, ClassFilter, DateFilter);
                //        }
                //        var Location = dbCore.XIAppUsers.Find(UserID);
                //        string BOName = dbContext.BOs.Where(m => m.BOID == report.BOID).Select(m => m.Name).FirstOrDefault();
                //        if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                //        {
                //            Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + OrgID);
                //            var LocCondition = "";
                //            var Locs = Location.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //            foreach (var Loc in Locs)
                //            {
                //                LocCondition = LocCondition + "OrgHeirarchyID='ORG" + OrgID + "_" + Loc + "' or ";
                //            }
                //            LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                //            LocCondition = "(" + LocCondition + ")";
                //            Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                //        }
                //        else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                //        {
                //            Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + OrgID);
                //            var LocCondition = "OrgHeirarchyID Like 'ORG" + OrgID + "_%'";
                //            Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                //        }
                //        Con.Open();
                //        Con.ChangeDatabase(sDatabase);
                //        cmd.Connection = Con;
                //        cmd.CommandText = Query;
                //        SqlDataReader reader = cmd.ExecuteReader();
                //        DataTable data = new DataTable();
                //        data.Load(reader);
                //        List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                //        VMKPIResult kpi = new VMKPIResult();
                //        if (TotalResult.Count() == 0)
                //        {
                //            com = 0;
                //        }
                //        else
                //        {
                //            com = Convert.ToInt32(TotalResult[0][0]);
                //        }
                //        Con.Close();
                //        double percentage = (double)com / target;
                //        int completed = (int)Math.Round(percentage * 100, 0);
                //        if (target == 0)
                //        {
                //            completed = 0;
                //        }
                //        kpi.Name = report.Name;
                //        kpi.ShowAs = report.Title;
                //        if (ReportID != null)
                //        {
                //            kpi.Visibility = Vistiblity[j];
                //        }
                //        else
                //        {
                //            kpi.Visibility = "true";
                //        }
                //        kpi.ReportID = report.ID;
                //        kpi.InnerReportID = report.InnerReportID;
                //        kpi.KPIPercent = completed;
                //        kpi.KPIValue = completed + "%";
                //        kpi.KPICircleColor = color[j];
                //        kpi.KPIIconColor = iconcolor[j];
                //        kpi.KPIIcon = ureport.Icon;
                //        KPIs.Add(kpi);
                //        j++;
                //    }
                //}
                //return KPIs;

                //oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                //if (true)//check mandatory params are passed or not
                //{
                //    //oCR = SubMethod();
                //    if (oCR.bOK && oCR.oResult != null)
                //    {
                //        oTrace.oTrace.Add(oCR.oTrace);
                //        if (oCR.bOK && oCR.oResult != null)
                //        {
                //            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                //            oCResult.oResult = "Success";
                //        }
                //        else
                //        {
                //            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                //        }
                //    }
                //    else
                //    {
                //        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                //    }
                //}
                //else
                //{
                //    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                //    oTrace.sMessage = "Mandatory Param:  is missing";
                //}
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}