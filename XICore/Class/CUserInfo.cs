using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XICore
{
    public class CUserInfo : XIDefinitionBase
    {
        public int iUserID { get; set; }
        public string sUserName { get; set; }
        public string sEmail { get; set; }
        public string sRoleName { get; set; }
        public int iRoleID { get; set; }

        public int iOrganizationID { get; set; }
        public string sOrgName { get; set; }
        public string sName { get; set; }
        public string AppName { get; set; }
        public string sCoreDataBase { get; set; }
        public CUserInfo GetUserInfo()
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

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            CUserInfo oUInfo = new CUserInfo();
            try
            {

                oUInfo.sUserName = HttpContext.Current.Session["sUserName"].ToString();
                oUInfo.sRoleName = HttpContext.Current.Session["sRoleName"].ToString();
                oUInfo.sEmail = HttpContext.Current.Session["sEmail"].ToString();
                oUInfo.sName = HttpContext.Current.Session["Name"].ToString();
                var UserID = HttpContext.Current.Session["UserID"].ToString();
                oUInfo.AppName = HttpContext.Current.Session["AppName"].ToString();
                if (UserID != null)
                {
                    int iUID = 0;
                    if (int.TryParse(UserID, out iUID))
                    {
                        oUInfo.iUserID = iUID;
                    }
                }
                var OrgID = HttpContext.Current.Session["OrganizationID"].ToString();
                if (OrgID != null)
                {
                    int iOrgID = 0;
                    if (int.TryParse(OrgID, out iOrgID))
                    {
                        oUInfo.iOrganizationID = iOrgID;
                    }
                }
                oUInfo.sOrgName = HttpContext.Current.Session["OrganisationName"].ToString();
                oUInfo.sCoreDataBase = HttpContext.Current.Session["CoreDatabase"].ToString();
                var RoleID = HttpContext.Current.Session["iRoleID"].ToString();
                if (RoleID != null)
                {
                    int iRoleID = 0;
                    if (int.TryParse(RoleID, out iRoleID))
                    {
                        oUInfo.iRoleID = iRoleID;
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting data from cache in CUserInfo" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oUInfo;
        }
    }
}