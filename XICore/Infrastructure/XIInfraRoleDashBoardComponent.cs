using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;
using System.Data;
using Microsoft.SqlServer.Management.Smo;

namespace XICore
{
    public class XIInfraRoleDashBoardComponent
    {
        public string iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sDisplayMode { get; set; }
        public int OneClickID;
        public Guid OneClickIDXIGUID;
        public string sOneClickID;

        XIInfraCache oCache = new XIInfraCache();

        public CResult XILoad(List<CNV> oParams)
        {

            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            XIGraphData oXIGD = new XIGraphData();
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
                sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                iUserID = oParams.Where(m => m.sName == XIConstant.Param_UserID).Select(m => m.sValue).FirstOrDefault();
                var sRoleID = oParams.Where(m => m.sName.ToLower() == "iRoleID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oParams.Add(new CNV { sName = "{-iRoleID}", sValue = sRoleID });
                oParams.Add(new CNV { sName = "{XIP|iRoleID}", sValue = sRoleID });
                var o1ClickID = oParams.Where(m => m.sName.ToLower() == "s1ClickID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iRoleID = 0;

                //CUserInfo oInfo = new CUserInfo();

                //XIInfraUsers oUser = new XIInfraUsers();
                //oInfo = oUser.Get_UserInfo();

                //var sMainDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                //var sConfigDatabase = oInfo.sDatabaseName;
                //var sCoreDatabase = oInfo.sCoreDataBase;
                //oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|sMainDatabase}", sMainDatabase, null, null);
                //oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|sConfigDatabase}", sConfigDatabase, null, null);
                //oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|sCoreDatabase}", sCoreDatabase, null, null);
                int.TryParse(sRoleID, out iRoleID);

                if (!string.IsNullOrEmpty(sGUID))
                {
                    oCache.sSessionID = sSessionID;
                    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                    sGUID = ParentGUID;
                }
                sDisplayMode = oParams.Where(m => m.sName == XIConstant.Param_DisplayMode).Select(m => m.sValue).FirstOrDefault();
                var WrapperParms = new List<CNV>();
                var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam".ToLower())).ToList();
                if (WatchParam.Count() > 0)
                {
                    foreach (var items in WatchParam)
                    {
                        if (!string.IsNullOrEmpty(items.sValue))
                        {
                            var Prams = oCache.Get_Paramobject(sSessionID, sGUID, null, items.sValue); //oParams.Where(m => m.sName == items.sValue).FirstOrDefault();
                            if (Prams != null)
                            {
                                WrapperParms = Prams.nSubParams;
                            }
                        }
                    }
                }
                XID1Click o1ClickD = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                XID1Click oXI1ClickC = new XID1Click();
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, o1ClickID);
                oXI1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                oXI1ClickC.ReplaceFKExpressions(oParams);
                var QueryResult = oXI1ClickC.Execute_Query();
                List<XIDRoleDashboard> KPIs = new List<XIDRoleDashboard>();
                if (QueryResult.Rows.Count > 0)
                {
                    KPIs = (from DataRow row in QueryResult.Rows
                            select new XIDRoleDashboard
                            {
                                XIGUID = Convert.ToString(row["XIGUID"]),
                                FKiLayoutIDXIGUID = Convert.ToString(row["FKiLayoutIDXIGUID"]),
                                FKiRoleID = Convert.ToInt32(row["FKiRoleID"])
                            }).ToList();
                }
                XILink oXILinkI = new XILink();
                foreach (var item in KPIs)
                {
                    XIDLayout oLayoutD = new XIDLayout();
                    oLayoutD.XIGUID =new Guid(item.FKiLayoutIDXIGUID);
                    //oLayoutD.XIGUID = LayoutGUID;
                    if (oLayoutD != null)
                    {
                        oLayoutD.oLayoutParams = oParams;
                        oLayoutD.sGUID = sGUID;
                        var oLayContent = oLayoutD.Load();
                        if (oLayContent.bOK && oLayContent.oResult != null)
                        {
                            oXILinkI.oContent[XIConstant.ContentLayout] = oLayContent.oResult;
                        }
                        oXILinkI.sNewGUID = sGUID;
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                //var GroupofKPI = KPIs.GroupBy(k => k.FKiRoleID).ToDictionary(t => t.Key, t => t.ToList());
                oCResult.oResult = oXILinkI;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Form Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }

    }
}