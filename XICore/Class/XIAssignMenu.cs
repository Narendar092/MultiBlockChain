using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIAssignMenu
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public string RootName { get; set; }
        public string MenuID { get; set; }
        public string Name { get; set; }
        public string sName { get; set; }
        public string ParentID { get; set; }
        public Guid ParentIDXIGUID { get; set; }
        public int OrgID { get; set; }
        public int RoleID { get; set; }
        public int iActionType { get; set; }
        public int ActionType { get; set; }
        public int Priority { get; set; }
        public string sMenuController { get; set; }
        public string sMenuAction { get; set; }
        public string MenuController { get; set; }
        public string MenuAction { get; set; }
        public int iXiLinkID { get; set; }
        public int XiLinkID { get; set; }
        public string MenuName { get; set; }
        public List<XIAssignMenu> SubGroups { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganizationID { get; set; }
        public bool isHide { get; set; }
        public string sUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public int iOrgID { get; set; }
        public List<XIDropDown> Organisations { get; set; }
        public List<XIDropDown> AllRoles { get; set; }
        public List<XIDropDown> CreatedMenus { get; set; }
        public string sConfigDatabase { get; set; }
        public int FKiMenuID { get; set; }
        public string sType { get; set; }
        public string BoName { get; set; }
        public int XIDeleted { get; set; }
        public string sXILinkGUID
        {
            get
            {
                return Utility.Get_XILinkUID(XiLinkID);
            }
        }
        public List<XIDropDown> Roles { get; set; }
        public CResult oCResult = new CResult();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        //public List<XIAssignMenu> Get_MenuTreeDetails()
        //{
        //    List<XIAssignMenu> oRightMenus = new List<XIAssignMenu>();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(MenuID))
        //        {
        //            //Get MenuName
        //            Dictionary<string, object> MenuParams = new Dictionary<string, object>();
        //            MenuParams["ID"] = MenuID;
        //            XIAssignMenu MenuDetails = Connection.Select<XIAssignMenu>("XIMenuMappings_T", MenuParams).FirstOrDefault();
        //            MenuName = MenuDetails.Name;
        //            ParentID = MenuDetails.ParentID;
        //        }
        //        if (!string.IsNullOrEmpty(MenuName))
        //        {
        //            string sRoleID = string.Empty;
        //            if (!string.IsNullOrEmpty(sUserID))
        //            {
        //                Dictionary<string, object> UserParams = new Dictionary<string, object>();
        //                UserParams["UserID"] = sUserID;
        //                cConnectionString oConString = new cConnectionString();
        //                if (sCoreDatabase == null)
        //                {
        //                    sCoreDatabase = sConfigDatabase;
        //                }
        //                string sConString = oConString.ConnectionString(sCoreDatabase);
        //                XIDBAPI sConnection = new XIDBAPI(sConString);
        //                sRoleID = sConnection.SelectString("RoleID", "XIAppUserRoles_AUR_T", UserParams).ToString();
        //            }

        //            XIAssignMenu oRightMenuTrees = new XIAssignMenu();
        //            Dictionary<string, object> Params = new Dictionary<string, object>();
        //            Params["RootName"] = MenuName;
        //            if (ParentID == "#" || ParentID == null)
        //            {
        //                Params["ParentID"] = "#";
        //            }
        //            else
        //            {
        //                Params["ParentID"] = ParentID;
        //            }

        //            //Params["ID"] = MenuID;
        //            if (!string.IsNullOrEmpty(sRoleID))
        //            {
        //                Params["RoleID"] = sRoleID;
        //            }
        //            oRightMenuTrees = Connection.Select<XIAssignMenu>("XIMenuMappings_T", Params).FirstOrDefault();
        //            if (oRightMenuTrees != null)
        //            {
        //                string MainID = oRightMenuTrees.MenuID;
        //                Params = new Dictionary<string, object>();
        //                Params["ParentID"] = MainID;
        //                Params["StatusTypeID"] = "10";
        //                if (!string.IsNullOrEmpty(sRoleID))
        //                {
        //                    Params["OrgID"] = iOrgID.ToString();
        //                    Params["RoleID"] = sRoleID;
        //                }
        //                var NewRightMenu = Connection.Select<XIAssignMenu>("XIMenuMappings_T", Params).ToList();
        //                if (NewRightMenu != null && NewRightMenu.Count() > 0)
        //                {
        //                    oRightMenus.AddRange(NewRightMenu);
        //                }
        //            }
        //            var Data = Countdata(oRightMenus, sCoreDatabase);
        //            oRightMenus.Add(oRightMenuTrees);
        //            oRightMenus.AddRange(Data);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
        //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
        //        oCResult.LogToFile();
        //    }
        //    return oRightMenus;
        //}
        //public List<XIAssignMenu> Countdata(List<XIAssignMenu> Menus, string sCoreDatabase)
        //{
        //    var oMenu = new List<XIAssignMenu>();
        //    foreach (var item in Menus)
        //    {
        //        var ID = item.MenuID;
        //        Dictionary<string, object> Params = new Dictionary<string, object>();
        //        Params["ParentID"] = ID;
        //        Params["StatusTypeID"] = "10";
        //        item.SubGroups = Connection.Select<XIAssignMenu>("XIMenuMappings_T", Params).ToList();
        //        if (item.SubGroups.Count() > 0)
        //        {
        //            oMenu.AddRange(item.SubGroups);
        //            var oBOList = Countdata(item.SubGroups, sCoreDatabase);
        //            if (oBOList != null)
        //            {
        //                oMenu.AddRange(oBOList);
        //            }
        //        }
        //    }
        //    return oMenu;
        //}

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
                List<XIInfraRoles> oXIDRoles = new List<XIInfraRoles>();
                List<XIDropDown> oXIDRolesDDL = new List<XIDropDown>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI sConnection = new XIDBAPI(sConString);
                if (OrgID > 0)
                {
                    Params["FKiOrgID"] = OrgID;
                    oXIDRoles = sConnection.Select<XIInfraRoles>("XIAppRoles_AR_T", Params).ToList();
                    oXIDRolesDDL = oXIDRoles.Select(m => new XIDropDown { Value = Convert.ToInt32(m.RoleID), text = m.sRoleName }).ToList();
                    oXIDRolesDDL.Insert(0, new XIDropDown
                    {
                        text = "--Select--",
                        Value = 0
                    });
                }

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

        public CResult Get_AllMenuTreeDetails(string sRootName)
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
                int iMenuID = 0;
                List<XIAssignMenu> oMenuDef = new List<XIAssignMenu>();
                if (!string.IsNullOrEmpty(sRootName))
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["Name"] = sRootName;
                    Params["ParentID"] = "#";
                    var oMDef = Connection.Select<XIMenu>("XIMenuMappings_T", Params).FirstOrDefault();
                    iMenuID = oMDef.ID;
                }
                Dictionary<string, object> Params1 = new Dictionary<string, object>();
                Params1["ID"] = iMenuID;
                var oMD = Connection.Select<XIMenu>("XIMenuMappings_T", Params1).FirstOrDefault();
                Dictionary<string, object> MenuParam = new Dictionary<string, object>();
                MenuParam["RoleID"] = oMD.RoleID;
                //MenuParam["OrgID"] = oMD.OrgID;
                MenuParam["RootName"] = oMD.RootName;
                oMenuDef = Connection.Select<XIAssignMenu>("XIMenuMappings_T", MenuParam).ToList();
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
                    var AllOrganisations = sConnection.Select<XIDOrganisation>("Organizations", Params).ToList();
                    Organisations = AllOrganisations.Select(m => new XIDropDown { Value = m.ID, text = m.Name }).ToList();
                    Organisations.Insert(0, new XIDropDown
                    {
                        text = "--Select--",
                        Value = 0
                    });
                }

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

        public XIAssignMenu Get_AssignedTreeDetails(int ID)
        {
            XIAssignMenu oMenu = new XIAssignMenu();
            Dictionary<string, object> MenuParams = new Dictionary<string, object>();
            MenuParams["ID"] = ID;
            oMenu = Connection.Select<XIAssignMenu>("XIMenuMappings_T", MenuParams).FirstOrDefault();
            return oMenu;
        }

        public CResult Get_AllCreatedMenus()
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
                List<XIAssignMenu> oCMenus = new List<XIAssignMenu>();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                oCMenus = Connection.Select<XIAssignMenu>("XICreateMenu_T", Params).ToList();
                var oCMenusDDL = oCMenus.Select(m => new XIDropDown { Value = m.ID, text = m.sName }).ToList();
                oCMenusDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oCResult.oResult = oCMenusDDL;
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

        public CResult Get_AssingedMenuDetails(int ID)
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
                XIAssignMenu oCMenus = new XIAssignMenu();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (ID > 0)
                {
                    Params["ID"] = ID;
                }
                oCMenus = Connection.Select<XIAssignMenu>("XIMenuMappings_T", Params).First();
                oCResult.oResult = oCMenus;
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

        public CResult Get_RightMenuDefinition(string MenuName = "")
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

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
            try
            {
                List<XIAssignMenu> oRightMenus = new List<XIAssignMenu>();

                if (!string.IsNullOrEmpty(MenuName))
                {
                    string sRoleID = string.Empty;
                    if (!string.IsNullOrEmpty(sUserID))
                    {
                        Dictionary<string, object> UserParams = new Dictionary<string, object>();
                        UserParams["UserID"] = sUserID;
                        cConnectionString oConString = new cConnectionString();
                        string sConString = oConString.ConnectionString(sCoreDatabase);
                        XIDBAPI sConnection = new XIDBAPI(sConString);
                        sRoleID = sConnection.SelectString("RoleID", "XIAppUserRoles_AUR_T", UserParams).ToString();
                    }

                    XIAssignMenu oRightMenuTrees = null;
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["RootName"] = MenuName;
                    Params["ParentID"] = "#";
                    if (!string.IsNullOrEmpty(sRoleID))
                    {
                        Params["RoleID"] = sRoleID;
                    }
                    oRightMenuTrees = Connection.Select<XIAssignMenu>("XIMenuMappings_T", Params).FirstOrDefault();
                    string MainID = oRightMenuTrees.MenuID;
                    Params = new Dictionary<string, object>();
                    Params["ParentID"] = MainID;
                    Params["StatusTypeID"] = "10";
                    if (!string.IsNullOrEmpty(sRoleID))
                    {
                        Params["OrgID"] = iOrgID.ToString();
                        Params["RoleID"] = sRoleID;
                    }
                    oRightMenus = Connection.Select<XIAssignMenu>("XIMenuMappings_T", Params).ToList();
                    oRightMenus = Countdata(oRightMenus, sCoreDatabase);
                }

                oCResult.oResult = oRightMenus;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Menu definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }
        public List<XIAssignMenu> Countdata(List<XIAssignMenu> Menus, string sCoreDatabase)
        {
            foreach (var items in Menus)
            {
                var ID = items.MenuID;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["ParentID"] = ID;
                Params["StatusTypeID"] = "10";
                items.SubGroups = Connection.Select<XIAssignMenu>("XIMenuMappings_T", Params).ToList();
                if (items.SubGroups.Count() > 0)
                {
                    Countdata(items.SubGroups, sCoreDatabase);
                }
            }
            return Menus;
        }

        public XIAssignMenu Get_CreatedMenuDetails(int FKiMenuID)
        {
            XIAssignMenu oCMenus = new XIAssignMenu();
            try
            {
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (FKiMenuID > 0)
                {
                    Params["ID"] = FKiMenuID;
                }
                oCMenus = Connection.Select<XIAssignMenu>("XICreateMenu_T", Params).First();
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCMenus; // always
        }
        //For MenuTree(Keerthi)
        public List<XIAssignMenu> Get_AllCreatedMenuDetails(string MenuID,int iAppID, int iOrgID)
        {
            List<XIAssignMenu> oCMenus = new List<XIAssignMenu>();
            try
            {
                
                Dictionary<string, object> UserParams = new Dictionary<string, object>();
                UserParams["UserID"] = sUserID;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI sConnection = new XIDBAPI(sConString);
                Dictionary<string, object> MenuParam = new Dictionary<string, object>();
                if (BoName == "XIMenu_T")
                {
                    MenuParam["organizationid"] = OrgID;
                    var Rootname = Connection.Select<XIAssignMenu>("XIMenu_T", MenuParam).Where(s => s.ID == Convert.ToInt32(MenuID)).Select(s => s.RootName).FirstOrDefault();
                    int sRoleID = Convert.ToInt32(sConnection.SelectString("RoleID", "XIAppUserRoles_AUR_T", UserParams).ToString());
                    oCMenus = Connection.Select<XIAssignMenu>("XIMenu_T", MenuParam).Where(m => m.RootName == Rootname).Where(e => e.XIDeleted == 0 || XIDeleted == null).OrderBy(m => m.Priority).ToList();
                    // dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID).Where(m => m.RootName == RootNode).Select(m => m.ID).ToList();
                }
                else
                {
                    MenuParam = new Dictionary<string, object>();
                    MenuParam["organizationid"] = OrgID;
                    MenuParam["fkiapplicationid"] = iAppID;
                    MenuParam["statustypeid"] = 10;
                    oCMenus = Connection.Select<XIAssignMenu>(BoName, MenuParam).Where(e=>e.XIDeleted == 0 || XIDeleted == null).ToList();
                }

            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCMenus; // always
        }
    }
}