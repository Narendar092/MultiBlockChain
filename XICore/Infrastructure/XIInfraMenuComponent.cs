using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XICore
{
    public class XIInfraMenuComponent : XIDefinitionBase
    {
        public string sMenuName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public int iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public int iOrgID { get; set; }
        public string RoleID { get; set; }

        XIInfraCache oCache = new XIInfraCache();
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult(); // always
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
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                //oInfo = oInfo.GetUserInfo();
                int iApplicationID = 0;
                int iOrgID = 0;
                sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                sMenuName = oParams.Where(m => m.sName == XIConstant.Param_MenuName).Select(m => m.sValue).FirstOrDefault();
                RoleID = oParams.Where(m => m.sName == "iRoleID").Select(m => m.sValue).FirstOrDefault();
                var sAppID = oParams.Where(m => m.sName == XIConstant.Param_ApplicationID).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sAppID, out iApplicationID);
                var sOrgID = oParams.Where(m => m.sName == "iOrgID").Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sOrgID, out iOrgID);
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                iUserID = oInfo.iUserID; //Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oInfo.sCoreDataBase;
                iOrgID = oInfo.iOrganizationID; //Convert.ToInt32(oParams.Where(m => m.sName == "iOrganizationID").Select(m => m.sValue).FirstOrDefault());
                List<XIMenu> oRightMenus = new List<XIMenu>();
                List<XIMenu> oRightMenuC = new List<XIMenu>();
                XIDXI oXID = new XIDXI();
                oXID.sCoreDatabase = sCoreDatabase;
                oXID.iOrgID = iOrgID;
                oXID.sUserID = iUserID.ToString();
                if (!string.IsNullOrEmpty(sMenuName))
                {
                    //oRightMenus = (List<XIMenu>)oCache.GetObjectFromCache(XIConstant.CacheMenu, sMenuName, null);
                    string sQuery = @"select M.ID, M.XIGUID, M.Name, M.OrgID, RM.RoleID, M.MenuID, M.ParentID, M.sCssClass, M.FKiBOActionIDXIGUID,
                                                M.MenuController, M.MenuAction, M.ParentIDXIGUID, M.Priority, RM.FKiApplicationID, M.RootName, M.XiLinkIDXIGUID, M.ActionType, M.sIcon 
                                        from XIMenu_T M 
	                                        left join XIRoleMenus_T RM on M.XIGUID=RM.MenuIDXIGUID and RM.FKiApplicationID = " + iApplicationID + " and (RM.FKiOrgID = " + iOrgID + " or RM.FKiOrgID is null) " +
                                                                                      " and RM.StatusTypeID = 10 and RM.XIDeleted = 0" +
                                        " where M.RootName like '" + sMenuName + "' and (M.StatusTypeID = 10 or M.StatusTypeID = 0) and M.XIDeleted = 0" +
                                        " order by M.ID";

                    //string sQuery = "select * from XIMenu_T xi left join XIRoleMenus_T xr on xi.XIGUID=xr.MenuIDXIGUID where xr.StatusTypeID=10 and xr.RoleID=" + RoleID + " and xr.FKiApplicationID=" + iApplicationID + " and xr.FKiOrgID=" + iOrgID + " order by xr.id asc";
                    string sReturnValue = string.Empty;
                    XID1Click oXI1Click = new XID1Click();
                    oXI1Click.Query = sQuery;
                    oXI1Click.Name = "XI Menu";
                    var QueryResult = oXI1Click.Execute_Query();
                    if (QueryResult.Rows.Count > 0)
                    {
                        oRightMenus = (from DataRow row in QueryResult.Rows
                                       select new XIMenu
                                       {
                                           ID = Convert.ToInt32(row["ID"]),
                                           XIGUID = new Guid(row["XIGUID"].ToString()),
                                           MenuID = row["MenuID"].ToString(),
                                           Name = row["Name"].ToString(),
                                           OrgID = Convert.ToInt32(row["OrgID"]),
                                           RoleID = row["RoleID"].ToString() == String.Empty ? -1 : int.Parse(row["RoleID"].ToString()),
                                           ParentID = row["ParentIDXIGUID"].ToString(),
                                           //ParentIDXIGUID = row["ParentIDXIGUID"].ToString() == String.Empty ? new Guid() : new Guid(row["ParentIDXIGUID"].ToString()),
                                           Priority = Convert.ToInt32(row["Priority"]),
                                           //FKiApplicationID = Convert.ToInt32(row["FKiApplicationID"]),
                                           FKiApplicationID = row["FKiApplicationID"].ToString() == String.Empty ? -1 : int.Parse(row["FKiApplicationID"].ToString()),
                                           RootName = row["RootName"].ToString(),
                                           XiLinkIDXIGUID = row["XiLinkIDXIGUID"].ToString() == String.Empty ? new Guid() : new Guid(row["XiLinkIDXIGUID"].ToString()),
                                           ActionType = row["ActionType"].ToString() == String.Empty ? 0 : int.Parse(row["ActionType"].ToString()),
                                           sIcon = row["sIcon"].ToString(),
                                           sCssClass = row["sCssClass"].ToString(),
                                           FKiBOActionIDXIGUID = row["FKiBOActionIDXIGUID"].ToString() == String.Empty ? new Guid() : new Guid(row["FKiBOActionIDXIGUID"].ToString()),
                                           MenuAction = row["MenuAction"].ToString(),
                                           MenuController = row["MenuController"].ToString()
                                       }).ToList()
                                       .GroupBy(x => new
                                       {
                                           x.ID,
                                           x.XIGUID,
                                           x.MenuID,
                                           x.Name,
                                           x.OrgID,
                                           x.ParentID,
                                           //x.ParentIDXIGUID,
                                           x.Priority,
                                           x.FKiApplicationID,
                                           x.RootName,
                                           x.XiLinkIDXIGUID,
                                           x.ActionType,
                                           x.sIcon,
                                           x.sCssClass,
                                           x.FKiBOActionIDXIGUID,
                                           x.MenuController,
                                           x.MenuAction,
                                       }).Select(g => new XIMenu
                                       {
                                           ID = g.Key.ID,
                                           XIGUID = g.Key.XIGUID,
                                           MenuID = g.Key.MenuID,
                                           Name = g.Key.Name,
                                           OrgID = g.Key.OrgID,
                                           ParentID = g.Key.ParentID,
                                           //ParentIDXIGUID = g.Key.ParentIDXIGUID,
                                           Priority = g.Key.Priority,
                                           FKiApplicationID = g.Key.FKiApplicationID,
                                           RootName = g.Key.RootName,
                                           XiLinkIDXIGUID = g.Key.XiLinkIDXIGUID,
                                           ActionType = g.Key.ActionType,
                                           sIcon = g.Key.sIcon,
                                           sCssClass = g.Key.sCssClass,
                                           FKiBOActionIDXIGUID = g.Key.FKiBOActionIDXIGUID,
                                           MenuController = g.Key.MenuController,
                                           MenuAction = g.Key.MenuAction,
                                           RoleIDs = g.ToList().Select(x => x.RoleID).ToList()
                                       }).ToList().Where(m => m.RoleIDs.All(r => r == -1) || m.RoleIDs.Any(r => oInfo.RoleIDs.Contains(r))).ToList();
                    }
                    XIDXI XIDXI = new XIDXI();
                    oRightMenus = XIDXI.CreateHierarchy(oRightMenus, oRightMenus.Where(m => m.ParentID == String.Empty || m.ParentID == Guid.Empty.ToString()).SingleOrDefault().XIGUID.ToString());
                }
                else
                {
                    oCR.sMessage = "Config Error: XIInfraMenuComponent_XILoad() : Menu Name is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                    oCR.sCode = "Config Error";
                    SaveErrortoDB(oCR);
                }
                XID1Click oD1Click = new XID1Click();
                oRightMenuC = (List<XIMenu>)oD1Click.Clone(oRightMenus);
                foreach (var Menu in oRightMenuC)
                {
                    if (Menu.XiLinkID > 0 || (Menu.XiLinkIDXIGUID != null && Menu.XiLinkIDXIGUID != Guid.Empty))
                    {
                        var oXiLink = (XILink)oCache.GetObjectFromCache(XIConstant.CacheXILink, "", Menu.XiLinkIDXIGUID.ToString());
                        if (oXiLink != null && !string.IsNullOrEmpty(oXiLink.sActive) && oXiLink.sActive.IndexOf("xi.s") >= 0)
                        {
                            //XIDScript oXIScript = new XIDScript();
                            //oXIScript.sScript = oXiLink.sActive;
                            //oCR = oXIScript.Execute_Script(sGUID, sSessionID);
                            oCR = RunScript(oXiLink.sActive, sGUID, sSessionID);
                            //  xi.s|{xi.a|'ACPolicy_T',{xi.p|iACPolicyID},'iStatus','10',''}
                            // xi.s|{if|{eq|{xi.a|'ACPolicy_T',{xi.p|iACPolicyID},'iStatus'},'190'},'Y','N'}  Cancellation 
                            // xi.s|{if|{eq|{xi.a|'ACPolicy_T',{xi.p|iACPolicyID},'iStatus'},'200'},'Y','N'}  Revoke 
                            // xi.s|{if|{gt|{xi.a|'ACPolicy_T',{xi.p|iACPolicyID},'rBalance'},'0'},'Y','N'}   Premium Finance Menus
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                string sValue = (string)oCR.oResult;
                                if (!string.IsNullOrEmpty(sValue) && sValue.ToLower() == "n")
                                {
                                    Menu.isHide = true;
                                }
                            }
                        }
                        foreach (var subMenu in Menu.SubGroups)
                        {
                            var oXiLink1 = (XILink)oCache.GetObjectFromCache(XIConstant.CacheXILink, "", Menu.XiLinkIDXIGUID.ToString());
                            var oXi1ClickC = (XILink)oD1Click.Clone(oXiLink1);
                            if (oXi1ClickC != null && !string.IsNullOrEmpty(oXi1ClickC.sActive) && oXi1ClickC.sActive.IndexOf("xi.s") >= 0)
                            {
                                XIDScript oXIScript1 = new XIDScript();
                                oXIScript1.sScript = oXi1ClickC.sActive;
                                oCR = oXIScript1.Execute_Script(sGUID, sSessionID);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    string sValue = (string)oCR.oResult;
                                    if (!string.IsNullOrEmpty(sValue) && sValue.ToLower() == "n")
                                    {
                                        subMenu.isHide = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var subMenu in Menu.SubGroups)
                        {
                            if (subMenu.XiLinkID > 0 || (subMenu.XiLinkIDXIGUID != null && subMenu.XiLinkIDXIGUID != Guid.Empty))
                            {
                                var oXiLink1 = (XILink)oCache.GetObjectFromCache(XIConstant.CacheXILink, "", subMenu.XiLinkIDXIGUID.ToString());
                                var oXi1ClickC = (XILink)oD1Click.Clone(oXiLink1);
                                if (oXi1ClickC != null && !string.IsNullOrEmpty(oXi1ClickC.sActive) && oXi1ClickC.sActive.IndexOf("xi.s") >= 0)
                                {
                                    XIDScript oXIScript1 = new XIDScript();
                                    oXIScript1.sScript = oXi1ClickC.sActive;
                                    oCR = oXIScript1.Execute_Script(sGUID, sSessionID);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        string sValue = (string)oCR.oResult;
                                        if (!string.IsNullOrEmpty(sValue) && sValue.ToLower() == "n")
                                        {
                                            subMenu.isHide = true;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                //var oResult = oXID.Get_RightMenuDefinition(sMenuName);
                //if (oResult.bOK && oResult.oResult != null)
                //{
                //    oRightMenus = (List<XIMenu>)oResult.oResult;
                //}
                oCResult.oResult = oRightMenuC;
                if (oRightMenuC != null && oRightMenuC.Count() > 0)
                {
                    oRightMenuC = oRightMenuC.OrderBy(m => m.Priority).ToList();
                    var Data = MenusByPriority(oRightMenuC);
                    oCResult.oResult = oRightMenuC;
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Menu Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public List<XIMenu> MenusByPriority(List<XIMenu> Menus)
        {
            foreach (var Menu in Menus)
            {
                Menu.SubGroups = Menu.SubGroups.OrderBy(m => m.Priority).ToList();
                if (Menu.SubGroups.Count() > 0)
                {
                    MenusByPriority(Menu.SubGroups);
                }
            }
            return Menus;
        }
        public CResult RunScript(string sScript, string sGUID, string sSessionID)
        {
            CResult oCResult = new CResult();
            try
            {
                if (sScript.IndexOf("xi.s") >= 0)
                {
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = sScript;
                    var oCR = oXIScript.Execute_Script(sGUID, sSessionID);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        string sSubScript = (string)oCR.oResult;
                        if (sSubScript.IndexOf("xi.s") >= 0)
                        {
                            oCR = RunScript(sSubScript, sGUID, sSessionID);
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = oCR.oResult;
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = sSubScript;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Menu Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
    }
}