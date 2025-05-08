using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIMenu
    {
        public int ID { get; set; }
        public string RootName { get; set; }
        public string MenuID { get; set; }
        public string Name { get; set; }
        public string ParentID { get; set; }
        public Guid ParentIDXIGUID { get; set; }
        public int OrgID { get; set; }
        public int RoleID { get; set; }
        public int ActionType { get; set; }
        public int Priority { get; set; }
        public string MenuController { get; set; }
        public string MenuAction { get; set; }
        public int XiLinkID { get; set; }
        public string MenuName { get; set; }
        public List<XIMenu> SubGroups { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public bool isHide { get; set; }
        public string sUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public int iOrgID { get; set; }
        public List<XIDropDown> Organisations { get; set; }
        public List<XIDropDown> AllRoles { get; set; }
        public string sConfigDatabase { get; set; }
        public string sIcon { get; set; }
        public string sIFrameURL { get; set; }
        public int XIDeleted { get; set; }
        public int StatusTypeID { get; set; }
        public List<int> RoleIDs { get; set; }
        public string sXILinkGUID
        {
            get
            {
                return Utility.Get_XILinkUID(XiLinkID);
            }
        }
        public string sXILinkID { get; set; }
        public Guid XIGUID { get; set; }
        public Guid XiLinkIDXIGUID { get; set; }
        public string sCssClass { get; set; }
        public int FKiBOActionID { get; set; }
        public Guid FKiBOActionIDXIGUID { get; set; }
        public List<XIDropDown> Roles { get; set; }
        public CResult oCResult = new CResult();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public List<XIMenu> Get_MenuTreeDetails()
        {
            List<XIMenu> oRightMenus = new List<XIMenu>();
            try
            {
                if (ID > 0 || (XIGUID != null && XIGUID != Guid.Empty))
                {
                    //Get MenuName
                    Dictionary<string, object> MenuParams = new Dictionary<string, object>();
                    if (XIGUID != null && XIGUID != Guid.Empty)
                    {
                        MenuParams["XIGUID"] = XIGUID;
                    }
                    else if (ID > 0)
                    {
                        MenuParams["ID"] = ID;
                    }
                    XIMenu MenuDetails = Connection.Select<XIMenu>("XIMenu_T", MenuParams).FirstOrDefault();
                    MenuName = MenuDetails.Name;
                }
                if (!string.IsNullOrEmpty(MenuName))
                {
                    string sRoleID = string.Empty;
                    if (!string.IsNullOrEmpty(sUserID))
                    {
                        Dictionary<string, object> UserParams = new Dictionary<string, object>();
                        UserParams["UserID"] = sUserID;
                        cConnectionString oConString = new cConnectionString();
                        if (sCoreDatabase == null)
                        {
                            sCoreDatabase = sConfigDatabase;
                        }
                        string sConString = oConString.ConnectionString(sCoreDatabase);
                        XIDBAPI sConnection = new XIDBAPI(sConString);
                        sRoleID = sConnection.SelectString("RoleID", "XIAppUserRoles_AUR_T", UserParams).ToString();
                    }

                    XIMenu oRightMenuTrees = null;
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["RootName"] = MenuName;
                    Params["ParentID"] = "#";
                    Params["ID"] = ID;
                    if (!string.IsNullOrEmpty(sRoleID))
                    {
                        Params["RoleID"] = sRoleID;
                    }
                    oRightMenuTrees = Connection.Select<XIMenu>("XIMenu_T", Params).FirstOrDefault();
                    string MainID = string.Empty;
                    if (oRightMenuTrees.XIGUID != null && oRightMenuTrees.XIGUID != Guid.Empty)
                    {
                        Guid MainGUID = oRightMenuTrees.XIGUID;
                        Params["ParentIDXIGUID"] = MainID;
                    }
                    else if (oRightMenuTrees.ID > 0)
                    {
                        MainID = oRightMenuTrees.ID.ToString();
                        Params["ParentID"] = MainID;
                    }
                    Params = new Dictionary<string, object>();
                    Params["StatusTypeID"] = "10";
                    if (!string.IsNullOrEmpty(sRoleID))
                    {
                        Params["OrgID"] = iOrgID.ToString();
                        Params["RoleID"] = sRoleID;
                    }
                    var NewRightMenu = Connection.Select<XIMenu>("XIMenu_T", Params).ToList();
                    if (NewRightMenu != null && NewRightMenu.Count() > 0)
                    {
                        oRightMenus.AddRange(NewRightMenu);
                    }
                    var Data = Countdata(oRightMenus, sCoreDatabase);
                    oRightMenus.Add(oRightMenuTrees);
                    oRightMenus.AddRange(Data);
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oRightMenus;
        }
        public List<XIMenu> Countdata(List<XIMenu> Menus, string sCoreDatabase)
        {
            var oMenu = new List<XIMenu>();
            Dictionary<string, object> Params = new Dictionary<string, object>();
            foreach (var item in Menus)
            {
                string ID = string.Empty;
                if (item.XIGUID != null && item.XIGUID != Guid.Empty)
                {
                    Guid MenuXIGUID = item.XIGUID;
                    Params["ParentIDXIGUID"] = MenuXIGUID;
                }
                else if (item.ID > 0)
                {
                    ID = item.ID.ToString();
                    Params["ParentID"] = ID;
                }
                Params["StatusTypeID"] = "10";
                item.SubGroups = Connection.Select<XIMenu>("XIMenu_T", Params).ToList();
                if (item.SubGroups.Count() > 0)
                {
                    oMenu.AddRange(item.SubGroups);
                    var oBOList = Countdata(item.SubGroups, sCoreDatabase);
                    if (oBOList != null)
                    {
                        oMenu.AddRange(oBOList);
                    }
                }
            }
            return oMenu;
        }

        public CResult Get_XIRolesDDL()
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
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?

            try
            {
                List<XIInfraRoles> oXIDRoles = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI sConnection = new XIDBAPI(sConString);
                if (OrgID > 0)
                {
                    Params["FKiOrgID"] = OrgID;
                }
                oXIDRoles = sConnection.Select<XIInfraRoles>("XIAppRoles_AR_T", Params).ToList();
                var oXIDRolesDDL = oXIDRoles.Select(m => new XIDropDown { Value = Convert.ToInt32(m.RoleID), text = m.sRoleName }).ToList();
                oXIDRolesDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oCResult.oResult = oXIDRolesDDL;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        public CResult Get_AllMenuTreeDetails()
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
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                List<XIMenu> oMenuDef = new List<XIMenu>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["ID"] = ID;
                var oMD = Connection.Select<XIMenu>("XIMenu_T", Params).FirstOrDefault();
                Dictionary<string, object> MenuParam = new Dictionary<string, object>();
                MenuParam["RoleID"] = oMD.RoleID;
                MenuParam["OrgID"] = oMD.OrgID;
                MenuParam["RootName"] = oMD.RootName;
                oMenuDef = Connection.Select<XIMenu>("XIMenu_T", MenuParam).ToList();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oMenuDef;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }

        public CResult Get_Organisations()
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
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?

            try
            {
                List<XIDropDown> Organisations = new List<XIDropDown>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI sConnection = new XIDBAPI(sConString);
                if (OrgID > 0)
                {
                    Params["ID"] = OrgID;
                }
                var AllOrganisations = sConnection.Select<XIDOrganisation>("Organizations", Params).ToList();
                Organisations = AllOrganisations.Select(m => new XIDropDown { Value = m.ID, text = m.Name }).ToList();
                Organisations.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oCResult.oResult = Organisations;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }
    }
}