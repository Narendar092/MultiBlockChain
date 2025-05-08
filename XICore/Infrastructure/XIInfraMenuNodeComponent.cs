using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using XICore;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIInfraMenuNodeComponent
    {
        public string sBOName { get; set; }
        public int iUserID { get; set; }
        public int iOrgID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public int iMenuID { get; set; }
        public string sTestName { get; set; }
        public string sConfigDatabase { get; set; }
        public string sMenuName { get; set; }

        XIInfraCache oCache = new XIInfraCache();
        List<XIAssignMenu> oRightMenuC = new List<XIAssignMenu>();
        XIAssignMenu cXIMenu = new XIAssignMenu();

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

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
            }
            try
            {
                long MenuID = 0;
                sMenuName = oParams.Where(m => m.sName == XIConstant.Param_MenuName).Select(m => m.sValue).FirstOrDefault();
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                iOrgID = Convert.ToInt32(oParams.Where(m => m.sName == "iOrgID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oParams.Where(m => m.sName == "sCoreDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                sConfigDatabase = oParams.Where(m => m.sName == "sConfigDatabase").Select(m => m.sValue).FirstOrDefault();
                if (oParams.Where(m => m.sName.ToLower() == "BO".ToLower()).Select(m => m.sValue).FirstOrDefault() != null)
                {
                    if (!oParams.Where(m => m.sName.ToLower() == "BO".ToLower()).Select(m => m.sValue).FirstOrDefault().StartsWith("{XIP|"))
                    {
                        sBOName = oParams.Where(m => m.sName == "BO").Select(m => m.sValue).FirstOrDefault();
                    }
                }
                var iAppID = 0;
                var sAppID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_ApplicationID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sAppID, out iAppID);
                cXIMenu.sCoreDatabase = sCoreDatabase;
                cXIMenu.sConfigDatabase = sConfigDatabase;
                cXIMenu.OrgID = iOrgID;
                cXIMenu.sUserID = iUserID.ToString();
                var WrapperParms = new List<CNV>();
                var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam1".ToLower())).ToList();
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

                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    var XiPMenuID = WrapperParms.Where(m => m.sName == "{-iInstanceID}").Select(m => m.sValue).FirstOrDefault(); // oXIAPI.Get_ParamVal(sSessionID, sGUID, null, Prm); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (XiPMenuID != null)
                    {
                        MenuID = Convert.ToInt32(XiPMenuID);
                    }
                    else
                    {
                        MenuID = 0;
                    }
                }
                else
                {
                    string sInstanceID = oParams.Where(m => m.sName.ToLower() == "iinstanceid").Select(m => m.sValue).FirstOrDefault();
                    if (sInstanceID != null && (sInstanceID.StartsWith("{XIP|") || sInstanceID.StartsWith("-") || sInstanceID.StartsWith("{-")))
                    {
                        sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, sInstanceID);
                    }
                    if (!string.IsNullOrEmpty(sInstanceID))
                    {
                        if (long.TryParse(sInstanceID, out MenuID))
                        {

                        }
                    }
                }
                if (!string.IsNullOrEmpty(sBOName))
                {
                    XIDXI oXI = new XIDXI();
                    oXI.sOrgDatabase = sCoreDatabase; //System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];// "xicoreqa_live";
                    XIDBO oBOD = new XIDBO();
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sBOName);
                    cXIMenu.BoName = oBOD.TableName;
                    if (sBOName.ToLower() == "XIInbox".ToLower())
                    {
                        oRightMenuC = cXIMenu.Get_AllCreatedMenuDetails(Convert.ToString(MenuID), iAppID, iOrgID);
                        if (oRightMenuC.Count() == 0)
                        {
                            oRightMenuC = new List<XIAssignMenu>();
                            oCR = oXI.Get_OrgDefinition(null, iOrgID.ToString());
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                var oOrgD = (XIDOrganisation)oCR.oResult;
                                oRightMenuC.Add(new XIAssignMenu { ParentID = "#", Name = oOrgD.Name, ID = oOrgD.ID, sType = "Org", OrgID = oOrgD.ID, OrganizationID = oOrgD.ID });
                                oCResult.oResult = oRightMenuC;
                            }
                        }
                        else
                        {
                            var Orgs = oRightMenuC.Select(m => m.OrganizationID).ToList();
                            if (Orgs != null && Orgs.Count() > 0)
                            {
                                foreach (var items in Orgs.Distinct())
                                {
                                    oCR = oXI.Get_OrgDefinition(null, items.ToString());
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        var oOrgD = (XIDOrganisation)oCR.oResult;
                                        foreach (var inbox in oRightMenuC)
                                        {
                                            if (string.IsNullOrEmpty(inbox.ParentID))
                                            {
                                                inbox.ParentID = "Org" + items;
                                            }
                                        }
                                        //oRightMenuC.Where(m => m.OrganizationID == items).ToList().ForEach(m => m.ParentID = "Org" + items);
                                        //oRightMenuC.Add(new XIAssignMenu { ParentID = "#", Name = oOrgD.Name, ID = oOrgD.ID, sType = "Org", OrgID = oOrgD.ID, OrganizationID = oOrgD.ID });
                                        oCResult.oResult = oRightMenuC;
                                    }

                                }
                            }
                        }
                    }
                    else if (sBOName.ToLower() == "xi menu")
                    {
                        if (!string.IsNullOrEmpty(sMenuName))
                        {
                            oRightMenuC = new List<XIAssignMenu>();
                            if (!string.IsNullOrEmpty(sMenuName))
                            {
                                //var Menus1 = (List<XIMenu>)oCache.GetObjectFromCache(XIConstant.CacheMenuNode, sMenuName, null);
                                var Menus = (List<XIMenu>)(oXI.Get_MenuNodeDefinition(null, sMenuName).oResult);// (List<XIMenu>)oCache.GetObjectFromCache(XIConstant.CacheMenuNode, sMenuName, null);
                                if (Menus != null && Menus.Count() > 0)
                                {
                                    foreach (var Menu in Menus.Where(m => (m.StatusTypeID == 10 || m.StatusTypeID == 0) && m.ParentIDXIGUID != null && m.ParentIDXIGUID != Guid.Empty))
                                    {
                                        oRightMenuC.Add(new XIAssignMenu { ID = Menu.ID, XIGUID = Menu.XIGUID, ParentID = Menu.ParentID, ParentIDXIGUID = Menu.ParentIDXIGUID, Name = Menu.Name, RootName = Menu.RootName, OrgID = Menu.OrgID, FKiApplicationID = Menu.FKiApplicationID, Priority = Menu.Priority });
                                    }
                                    var rootnode = Menus.Where(m => m.ParentID == "#").FirstOrDefault();
                                    oRightMenuC.Add(new XIAssignMenu { ID = rootnode.ID, XIGUID = rootnode.XIGUID, ParentID = rootnode.ParentID, ParentIDXIGUID = rootnode.ParentIDXIGUID, Name = rootnode.Name, RootName = rootnode.RootName, OrgID = rootnode.OrgID, FKiApplicationID = rootnode.FKiApplicationID, Priority = rootnode.Priority });
                                }
                            }
                            else
                            {
                                oCR.sMessage = "Config Error: XIInfraMenuComponent_XILoad() : Menu Name is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                                oCR.sCode = "Config Error";
                            }

                            if (sBOName.ToLower() == "xi menu")
                            {
                                if (oRightMenuC != null && oRightMenuC.Count() > 0)
                                {
                                    oRightMenuC = oRightMenuC.OrderBy(m => m.Priority).ToList();
                                    //var Data = MenusByPriority(oRightMenuC);
                                    oRightMenuC.FirstOrDefault().sType = "CreateMenu";
                                    oCResult.oResult = oRightMenuC;
                                }
                                else
                                {
                                    oRightMenuC = new List<XIAssignMenu>();
                                    oRightMenuC.Add(new XIAssignMenu { sType = "CreateMenu" });
                                    oCResult.oResult = oRightMenuC;
                                }
                            }
                            else
                            {
                                oCResult.oResult = oRightMenuC;
                            }

                        }
                        else
                        {
                            cXIMenu.MenuID = MenuID.ToString();
                            cXIMenu.ID = Convert.ToInt32(MenuID);
                            if (MenuID != 0)
                            {
                                var oMDef = cXIMenu.Get_AllMenuTreeDetails(null);
                                if (oMDef.bOK && oMDef.oResult != null)
                                {
                                    oRightMenuC = (List<XIAssignMenu>)oMDef.oResult;
                                }
                            }
                            else
                            {
                                if (sBOName.ToLower() == "xi menu")
                                {
                                    if (oRightMenuC != null && oRightMenuC.Count() > 0)
                                    {
                                        oRightMenuC = oRightMenuC.OrderBy(m => m.Priority).ToList();
                                        var Data = MenusByPriority(oRightMenuC);
                                        oRightMenuC.FirstOrDefault().sType = "CreateMenu";
                                        oCResult.oResult = oRightMenuC;
                                    }
                                    else
                                    {
                                        oRightMenuC = new List<XIAssignMenu>();
                                        oRightMenuC.Add(new XIAssignMenu { sType = "CreateMenu" });
                                        oCResult.oResult = oRightMenuC;
                                    }
                                }
                                //oRightMenuC.Add(new XIAssignMenu());
                                // oRightMenuC = cXIMenu.Get_AllCreatedMenuDetails();
                            }
                            oCResult.oResult = oRightMenuC;
                        }
                    }
                }
                else
                {
                    oRightMenuC.Add(new XIAssignMenu());
                }

                cXIMenu.sCoreDatabase = sCoreDatabase; //"XICoreQA_live";
                var AllOrgs = cXIMenu.Get_Organisations();
                if (AllOrgs.bOK && AllOrgs.oResult != null)
                {
                    oRightMenuC.FirstOrDefault().Organisations = (List<XIDropDown>)AllOrgs.oResult;
                }
                var AllRoles = cXIMenu.Get_XIRolesDDL();
                if (AllRoles.bOK && AllRoles.oResult != null)
                {
                    oRightMenuC.FirstOrDefault().AllRoles = (List<XIDropDown>)AllRoles.oResult;
                }
                var CreatedMenus = cXIMenu.Get_AllCreatedMenus();
                if (CreatedMenus.bOK && CreatedMenus.oResult != null)
                {
                    oRightMenuC.FirstOrDefault().CreatedMenus = (List<XIDropDown>)CreatedMenus.oResult;
                }

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Menu Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }

        public List<XIAssignMenu> MenusByPriority(List<XIAssignMenu> Menus)
        {
            foreach (var Menu in Menus)
            {
                if (Menu.SubGroups != null && Menu.SubGroups.Count() > 0)
                {
                    Menu.SubGroups = Menu.SubGroups.OrderBy(m => m.Priority).ToList();
                    if (Menu.SubGroups.Count() > 0)
                    {
                        MenusByPriority(Menu.SubGroups);
                    }
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
            }
            return oCResult;
        }

    }
}