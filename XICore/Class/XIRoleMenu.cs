using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIRoleMenu
    {
        public int ID { get; set; }
        public string RootName { get; set; }
        public string MenuID { get; set; }
        public string Name { get; set; }
        public string ParentID { get; set; }
        public int OrgID { get; set; }
        public int RoleID { get; set; }
        public int ActionType { get; set; }
        public int Priority { get; set; }
        public string MenuController { get; set; }
        public string MenuAction { get; set; }
        public int XiLinkID { get; set; }
        public string MenuName { get; set; }
        public List<XIRoleMenu> SubGroups { get; set; }
        public int FKiApplicationID { get; set; }
        public bool isHide { get; set; }
        public string sUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public int iOrgID { get; set; }
        public List<XIDropDown> Roles { get; set; }
        public List<int> GroupIDs { get; set; }
        public CResult oCResult = new CResult();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        public List<XIRoleMenu> Get_AllRoleMenuTreeDetails()
        {
            List<XIRoleMenu> oRightMenus = new List<XIRoleMenu>();
            try
            {
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["ParentID"] = "#";
                var NewRightMenu = Connection.Select<XIRoleMenu>("XIRoleMenus_T", Params).ToList();
                var Data = Countdata(NewRightMenu, sCoreDatabase);
                oRightMenus.AddRange(NewRightMenu);
                oRightMenus.AddRange(Data);
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oRightMenus;
        }

        public List<XIRoleMenu> Countdata(List<XIRoleMenu> Menus, string sCoreDatabase)
        {
            var oMenu = new List<XIRoleMenu>();
            foreach (var item in Menus)
            {
                var ID = item.MenuID;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["ParentID"] = ID;
                Params["StatusTypeID"] = 10;
                item.SubGroups = Connection.Select<XIRoleMenu>("XIRoleMenus_T", Params).ToList();
                var NewRightMenu = Connection.Select<XIRoleMenu>("XIRoleMenus_T", Params).ToList();
                if (item.SubGroups != null && item.SubGroups.Count() > 0)
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

    }
}