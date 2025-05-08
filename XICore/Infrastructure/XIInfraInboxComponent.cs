using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;
using XICore;
using XIDatabase;
using System.Data;
using System.Data.SqlClient;

namespace XICore
{
    public class XIInfraInboxComponent : XIDefinitionBase
    {
        public int iRoleID;

        XIInfraCache oCache = new XIInfraCache();
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                var sRoleID = oParams.Where(m => m.sName.ToLower() == "iRoleID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sCoreDatabase = oParams.Where(m => m.sName.ToLower() == "sCoreDatabase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iApplicationID = 0;
                var sAppID = oParams.Where(m => m.sName == XIConstant.Param_ApplicationID).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sAppID, out iApplicationID);
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                var sOrgID = oParams.Where(m => m.sName.ToLower() == "iOrgID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sRoleID, out iRoleID);
                if (iRoleID > 0)
                {
                    //var InboxD = (List<XIDInbox>)oCache.GetObjectFromCache(XIConstant.CacheInbox, null, iRoleID.ToString());
                    // For Inbox Menu  Based on Roles
                    string sQuery = "select * from XIInbox_T xi join XIRoleInbox_T xr on xi.ID=xr.FKiInboxID where xi.StatusTypeID=10 and xi."+ XIConstant.Key_XIDeleted + "!=1 and xr.FKiApplicationID=" + iApplicationID +" and xr.RoleID=" + iRoleID + " and xr.FKiOrgID=" + sOrgID +"order by xr.XIOrder desc ";
                    string sReturnValue = string.Empty;
                    XID1Click oXI1Click = new XID1Click();
                    oXI1Click.Query = sQuery;
                    oXI1Click.Name = "XIInbox";
                    var QueryResult = oXI1Click.Execute_Query();
                    List<XIDInbox> InboxD = new List<XIDInbox>();
                    if (QueryResult.Rows.Count > 0)
                    {
                        InboxD = (from DataRow row in QueryResult.Rows
                                  select new XIDInbox
                                  {
                                      ID = Convert.ToInt32(row["ID"]),
                                      FKi1ClickID = Convert.ToInt32(row["FKi1ClickID"]),
                                      FKi1ClickIDXIGUID = Convert.ToString(row["FKi1ClickIDXIGUID"]),
                                      FKiXILinkID = Convert.ToInt32(row["FKiXILinkID"]),
                                      FKiXILinkIDXIGUID = Convert.ToString(row["FKiXILinkIDXIGUID"]),
                                      TargetResultType = Convert.ToString(row["TargetResultType"]),
                                      CountColour = Convert.ToString(row["CountColour"]),
                                      rRTagetcolor = Convert.ToDecimal(row["rRTagetcolor"]),
                                      rATagetcolor = Convert.ToDecimal(row["rATargetcolor"]),
                                      ParentID = Convert.ToString(row["ParentID"]),
                                      Name= Convert.ToString(row["Name"]),
                                      iRefreshingType = Convert.ToInt32(row["iRefreshingtype"]),
                                      iSetinterval = Convert.ToInt32(row["iSetinterval"]),
                                  }).ToList();
                    }
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                    List<CNV> nParams = new List<CNV>();
                    nParams = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    //var InboxD = (List<XIDInbox>)oCache.GetObjectFromCache(XIConstant.CacheInbox, null, iRoleID.ToString());
                    XIDefinitionBase oDef = new XIDefinitionBase();
                    XIDXI XIDXI = new XIDXI();
                    //var sCoreDatabase = sCoreDatabase;//"XiCoreQA_Live";
                    InboxD = XIDXI.InboxCountdata(InboxD, sCoreDatabase);
                    var InboxC = (List<XIDInbox>)oDef.Clone(InboxD);
                    if (InboxC != null && InboxC.Count() > 0)
                    {
                        for (int k = 0; k < InboxC.Count(); k++)
                        {
                            if (!string.IsNullOrEmpty(InboxC[k].FKi1ClickIDXIGUID) && InboxC[k].FKi1ClickIDXIGUID != Guid.Empty.ToString())
                            {
                                var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, InboxC[k].FKi1ClickIDXIGUID.ToString());
                                XID1Click o1Copy = new XID1Click();
                                o1Copy = (XID1Click)o1ClickD.Clone(o1ClickD);
                                o1Copy.Targets = InboxC[k].Target;
                                o1Copy.Type = InboxC[k].TargetResultType;
                                if (InboxC[k].TargetResultType == "HTML")
                                {
                                    //Rep.HTMLCode = Spdb.ContentEditors.Where(m => m.ID == items.TargetTemplateID).Select(m => m.Content).FirstOrDefault();
                                }
                                try
                                {
                                    o1Copy.ReplaceFKExpressions(oParams, true);
                                    o1Copy = GetInboxCounts(o1Copy, nParams);
                                }
                                catch (Exception ex)
                                {
                                    oCR.sMessage = "Config Error: XIInfraInboxComponent_XILoad() : Error While Executing 1-Click " + o1Copy.ID + " for Inbox Count - " + ex.Message + " - Trace: " + ex.StackTrace;
                                    oCR.sCode = "Config Error";
                                    SaveErrortoDB(oCR);
                                }
                                InboxC[k].o1ClickD = o1Copy;
                                InboxC[k].InboxCount = o1Copy.InboxCount;
                                if (!string.IsNullOrEmpty(InboxC[k].CountColour))
                                {
                                    if (Convert.ToInt32(InboxC[k].InboxCount) > Convert.ToInt32(InboxC[k].CountColour.Split(':')[1]))
                                    {
                                        InboxC[k].CountColour = InboxC[k].CountColour.Split(':')[0];
                                    }
                                }
                                InboxC[k].Percentage = o1Copy.Percentage;
                                if (InboxC[k].SubGroups.Count() > 0)
                                {
                                    foreach (var item in InboxC[k].SubGroups)
                                    {
                                        if (item.FKi1ClickID > 0)
                                        {
                                            var o1ClickDSub = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, item.FKi1ClickID.ToString());
                                            o1ClickDSub.Targets = item.Target;
                                            o1ClickDSub.Type = item.TargetResultType;
                                            if (item.TargetResultType == "HTML")
                                            {
                                                //Rep.HTMLCode = Spdb.ContentEditors.Where(m => m.ID == items.TargetTemplateID).Select(m => m.Content).FirstOrDefault();
                                            }
                                            try
                                            {
                                                o1ClickDSub = GetInboxCounts(o1ClickDSub, nParams);
                                            }
                                            catch (Exception ex)
                                            {
                                                oCR.sMessage = "Config Error: XIInfraInboxComponent_XILoad() : Error While Executing 1-Click " + o1ClickDSub.ID + " for Inbox Count - " + ex.Message + " - Trace: " + ex.StackTrace;
                                                oCR.sCode = "Config Error";
                                                SaveErrortoDB(oCR);
                                            }
                                            item.o1ClickD = o1ClickDSub;
                                            item.InboxCount = o1ClickDSub.InboxCount;
                                            if (!string.IsNullOrEmpty(item.CountColour))
                                            {
                                                if (Convert.ToInt32(item.InboxCount) > Convert.ToInt32(item.CountColour.Split(':')[1]))
                                                {
                                                    item.CountColour = item.CountColour.Split(':')[0];
                                                }
                                            }
                                            item.Percentage = o1ClickDSub.Percentage;
                                        }
                                        else
                                        {
                                            //item.o1ClickD = new XID1Click();
                                            //item.InboxCount = "0";
                                            //item.Percentage = "0";
                                        }
                                    }
                                }
                            }
                            else if (InboxC[k].SubGroups.Count()>0)
                            {
                                foreach(var item in InboxC[k].SubGroups)
                                {
                                    if (item.FKi1ClickID > 0)
                                    {
                                        var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, item.FKi1ClickID.ToString());
                                        o1ClickD.Targets = item.Target;
                                        o1ClickD.Type = item.TargetResultType;
                                        if (item.TargetResultType == "HTML")
                                        {
                                            //Rep.HTMLCode = Spdb.ContentEditors.Where(m => m.ID == items.TargetTemplateID).Select(m => m.Content).FirstOrDefault();
                                        }
                                        try
                                        {
                                            o1ClickD = GetInboxCounts(o1ClickD, nParams);
                                        }
                                        catch (Exception ex)
                                        {
                                            oCR.sMessage = "Config Error: XIInfraInboxComponent_XILoad() : Error While Executing 1-Click " + o1ClickD.ID + " for Inbox Count - " + ex.Message + " - Trace: " + ex.StackTrace;
                                            oCR.sCode = "Config Error";
                                            SaveErrortoDB(oCR);
                                        }
                                        item.o1ClickD = o1ClickD;
                                        item.InboxCount = o1ClickD.InboxCount;
                                        if (!string.IsNullOrEmpty(item.CountColour))
                                        {
                                            if (Convert.ToInt32(item.InboxCount) > Convert.ToInt32(item.CountColour.Split(':')[1]))
                                            {
                                                item.CountColour = item.CountColour.Split(':')[0];
                                            }
                                        }
                                       item.Percentage = o1ClickD.Percentage;
                                    }
                                    else
                                    {
                                        //item.o1ClickD = new XID1Click();
                                        //item.InboxCount = "0";
                                        //item.Percentage = "0";
                                    }
                                }
                            }
                            else
                            {
                                //InboxC[k].InboxCount = "0";
                                //InboxC[k].Percentage = "0";
                                //InboxC[k].o1ClickD = new XID1Click();
                            }
                        }
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = InboxC;
                    }
                }
                else
                {
                    oCR.sMessage = "Config Error: XIInfraInboxComponent_XILoad() : Role ID is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                    oCR.sCode = "Config Error";
                    SaveErrortoDB(oCR);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oCResult;
        }
        private XID1Click GetInboxCounts(XID1Click o1ClickD, List<CNV> nParams)
        {
            int Count = GetInboxTargetResult(o1ClickD, nParams);
            XID1Click InboxCount = GetInboxTargetCount(o1ClickD, Count);
            o1ClickD.InboxCount = InboxCount.InboxCount;
            o1ClickD.Percentage = InboxCount.Percentage;
            if (o1ClickD.Sub1Clicks.Count() > 0)
            {
                o1ClickD.Sub1Clicks.ToList().ForEach(m => m.Type = o1ClickD.Type);
                foreach (var item in o1ClickD.Sub1Clicks)
                {
                    GetInboxCounts(item, nParams);
                }
            }
            return o1ClickD;
        }

        private XID1Click GetInboxTargetCount(XID1Click items, int Count)
        {
            string InboxCount = "";
            string Percentage = "";
            if (items.Type == "Progress Bar")
            {
                if (Count > 0 && items.Targets > 0)
                {
                    double percentage = (double)Count / items.Targets;
                    InboxCount = Math.Round(percentage * 100, 2).ToString();
                    if (Convert.ToDecimal(InboxCount) > 100)
                    {
                        InboxCount = 100.ToString();
                    }
                }
                else
                {
                    InboxCount = "0";
                }
            }
            else if (items.Type == "Number")
            {
                if (items.Targets > 0)
                {
                    InboxCount = Count + "/" + items.Targets;
                }
                else
                {
                    if (items.Targets == 0)
                    {
                        InboxCount = Count.ToString();
                    }
                    else
                    {
                        InboxCount = Count + "/0";
                    }
                }
            }
            else if (items.Type == "Progress Bar & Number")
            {
                if (items.Targets > 0)
                {
                    InboxCount = Count + "/" + items.Targets;
                    double percentage = (double)Count / items.Targets;
                    Percentage = Math.Round(percentage * 100, 2).ToString();
                }
                else
                {
                    InboxCount = Count + "/0";
                    Percentage = "0";
                }
            }
            else if (items.Type == "HTML")
            {
                if (items.Targets > 0)
                {
                    InboxCount = Count + "/" + items.Targets;
                    double percentage = (double)Count / items.Targets;
                    Percentage = Math.Round(percentage * 100, 2).ToString();
                }
                else
                {
                    InboxCount = Count + "/0";
                    Percentage = "0";
                }
                if (items.HTMLCode != null)
                {
                    items.HTMLCode = items.HTMLCode.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "'").Replace("{{Target}}", Percentage);
                }
            }
            items.InboxCount = InboxCount;
            items.Percentage = Percentage;
            return items;
        }

        private int GetInboxTargetResult(XID1Click o1ClickD, List<CNV> nParams)
        {
            string iOrgID = nParams.Where(m => m.sName.ToLower() == "{XIP|iOrgID}".ToLower()).Select(m => m.sValue).FirstOrDefault();
            int iCount = 0;
            XID1Click o1ClickC = new XID1Click();
            o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
            XIDXI oXID = new XIDXI();
            XIDBAPI Connection = new XIDBAPI();
            int iDataSource = 0;
            Guid DataSourceXIGUID = Guid.Empty;
            XIDBO oBOD = new XIDBO();
            if (o1ClickC.BOID > 0 || (o1ClickC.BOIDXIGUID != null && o1ClickC.BOIDXIGUID != Guid.Empty))
            {
                if (o1ClickC.BOIDXIGUID != null && o1ClickC.BOIDXIGUID != Guid.Empty)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickC.BOIDXIGUID.ToString());
                }
                else if (o1ClickC.BOID > 0)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickC.BOID.ToString());
                }
                iDataSource = oBOD.iDataSource;
                DataSourceXIGUID = oBOD.iDataSourceXIGUID;
            }
            string sConntection = oXID.GetBODataSource(DataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
            Connection = new XIDBAPI(sConntection);
            //List<CNV> nParams = new List<CNV>();
            o1ClickC.ReplaceFKExpressions(nParams);
            var Query = o1ClickC.Query;
            string StarQuery = "";
            string SearchText = "";
            List<SqlParameter> oSQLParams = new List<SqlParameter>();
            if (oBOD.iOrgObject > 0 && Query.IndexOf("FKiOrgID", StringComparison.CurrentCultureIgnoreCase) == -1)
            {
                string sBOIndentifier = o1ClickC.GetBOIndentifierFromQuery();
                if (string.IsNullOrEmpty(sBOIndentifier))
                {
                    sBOIndentifier = oBOD.TableName;
                }
                if (oBOD.iOrgObject == 1)
                {
                    SearchText = sBOIndentifier.Length > 0 ? $"{sBOIndentifier}.FKiOrgID = @FKiOrgID" : "FKiOrgID = @FKiOrgID";
                    o1ClickC.AddSqlParameters("@FKiOrgID", iOrgID);
                    oSQLParams.Add(new SqlParameter { ParameterName = "@FKiOrgID", Value = iOrgID });
                    Query = o1ClickC.AddSearchParameters(Query, SearchText);
                }
                else if (oBOD.iOrgObject == 2)
                {
                    SearchText = sBOIndentifier.Length > 0 ? $"({sBOIndentifier}.FKiOrgID = @FKiOrgID or {sBOIndentifier}.FKiOrgID is null)" : "(FKiOrgID = @FKiOrgID or FKiOrgID is null)";
                    o1ClickC.AddSqlParameters("@FKiOrgID", iOrgID); 
                    oSQLParams.Add(new SqlParameter { ParameterName = "@FKiOrgID", Value = iOrgID });
                    Query = o1ClickC.AddSearchParameters(Query, SearchText);
                }
            }
            if (o1ClickC.bIsMultiBO == true)
            {
                StarQuery = Query;
            }
            else
            {
                StarQuery = o1ClickC.ReplaceQueryStringWithStar(Query);
            }
            string Count = Connection.GetTotalCount(CommandType.Text, StarQuery, oSQLParams);
            int.TryParse(Count, out iCount);
            //var oDictionaryBOI = o1ClickC.OneClick_Run(false);
            return iCount;
        }
    }
}