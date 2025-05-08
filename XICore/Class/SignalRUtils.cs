using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using XISystem;
using XIDataBase.Hubs;
using System.Web.Mvc;

namespace XICore
{
    public class NotificationMaster
    {
        public int ID { get; set; }
        public string sCode { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public Guid XIGUID { get; set; }
        public string sRoleDistributionList { get; set; }
        public string sUserDistributionList { get; set; }
        public Guid FKiLayoutIDXIGUID { get; set; }
        public int iType { get; set; }
        public int iCategory { get; set; }
        public string sTheme { get; set; }
        public bool bIsImportant { get; set; }
        public int? iLeft { get; set; }
        public int? iTop { get; set; }
        public int? iWidth { get; set; }
        public int? iHeight { get; set; }
    }

    public class RoleNotificationMaster { 
        public int ID { get; set; }
        public int XIGUID { get; set; }
        public int iRoleID { get; set; }
        public Guid FKiNotificationMasterIDXIGUID { get; set; }
    }

    public class SignalRUtils
    {
        public static NotificationMaster GetNotificationMaster(string sGUID, int iOrgID)
        {
            XID1Click o1click = new XID1Click();
            o1click.Name = "XINotificationMasterOrg";
            string sQuery = @"with U as(select XINotificationMasterOrg.ID, XINotificationMasterOrg.FKiNotificationMasterIDXIGUID, XINotificationMasterOrg.sCode, XINotificationMasterOrg.sName, XINotificationMasterOrg.sDescription, XINotificationMasterOrg.FKiLayoutIDXIGUID, XINotificationMasterOrg.iLeft, XINotificationMasterOrg.iTop, XINotificationMasterOrg.iWidth, XINotificationMasterOrg.iHeight, XINotificationMasterOrg.sTheme, XINotificationMasterOrg.bIsImportant, XINotificationMasterOrg.iCategory, string_agg(cast(XIUserNotificationMasterOrg.iUserID as nvarchar(10)), ',') as UserIDs from XINotificationMasterOrg join XIUserNotificationMasterOrg on XINotificationMasterOrg.FKiNotificationMasterIDXIGUID=XIUserNotificationMasterOrg.FKiNotificationMasterIDXIGUID where XINotificationMasterOrg.FKiNotificationMasterIDXIGUID='" + sGUID + "' and XINotificationMasterOrg.FKiOrgID=" + iOrgID.ToString() + " and XIUserNotificationMasterOrg.FKiOrgID=" + iOrgID.ToString() + " group by XINotificationMasterOrg.ID, XINotificationMasterOrg.FKiNotificationMasterIDXIGUID, XINotificationMasterOrg.sCode, XINotificationMasterOrg.sName, XINotificationMasterOrg.sDescription, XINotificationMasterOrg.FKiLayoutIDXIGUID, XINotificationMasterOrg.iLeft, XINotificationMasterOrg.iTop, XINotificationMasterOrg.iWidth, XINotificationMasterOrg.iHeight, XINotificationMasterOrg.sTheme, XINotificationMasterOrg.bIsImportant, XINotificationMasterOrg.iCategory), R as (select XINotificationMasterOrg.ID, XINotificationMasterOrg.FKiNotificationMasterIDXIGUID, XINotificationMasterOrg.sCode, XINotificationMasterOrg.sName, XINotificationMasterOrg.sDescription, XINotificationMasterOrg.FKiLayoutIDXIGUID, XINotificationMasterOrg.iLeft, XINotificationMasterOrg.iTop, XINotificationMasterOrg.iWidth, XINotificationMasterOrg.iHeight, XINotificationMasterOrg.sTheme, XINotificationMasterOrg.bIsImportant, XINotificationMasterOrg.iCategory, string_agg(cast(XIRoleNotificationMasterOrg.iRoleID as nvarchar(10)), ',') as RoleIDs from XINotificationMasterOrg join XIRoleNotificationMasterOrg on XINotificationMasterOrg.FKiNotificationMasterIDXIGUID=XIRoleNotificationMasterOrg.FKiNotificationMasterIDXIGUID where XINotificationMasterOrg.FKiNotificationMasterIDXIGUID='" + sGUID + "' and XINotificationMasterOrg.FKiOrgID=" + iOrgID.ToString() + " and XIRoleNotificationMasterOrg.FKiOrgID=" + iOrgID.ToString() + " group by XINotificationMasterOrg.ID, XINotificationMasterOrg.FKiNotificationMasterIDXIGUID, XINotificationMasterOrg.sCode, XINotificationMasterOrg.sName, XINotificationMasterOrg.sDescription, XINotificationMasterOrg.FKiLayoutIDXIGUID, XINotificationMasterOrg.iLeft, XINotificationMasterOrg.iTop, XINotificationMasterOrg.iWidth, XINotificationMasterOrg.iHeight, XINotificationMasterOrg.sTheme, XINotificationMasterOrg.bIsImportant, XINotificationMasterOrg.iCategory) select coalesce(U.ID, null, R.ID) as ID, coalesce(U.FKiNotificationMasterIDXIGUID, null, R.FKiNotificationMasterIDXIGUID) as XIGUID, coalesce(U.sCode, null, R.sCode) as sCode, coalesce(U.sName, null, R.sName) as sName, coalesce(U.sDescription, null, R.sDescription) as sDescription, coalesce(U.FKiLayoutIDXIGUID, null, R.FKiLayoutIDXIGUID) as FKiLayoutIDXIGUID, coalesce(U.iLeft, null, R.iLeft) as iLeft, coalesce(U.iTop, null, R.iTop) as iTop, coalesce(U.iWidth, null, R.iWidth) as iWidth, coalesce(U.iHeight, null, R.iHeight) as iHeight, coalesce(U.sTheme, null, R.sTheme) as sTheme, coalesce(U.bIsImportant, null, R.bIsImportant) as bIsImportant, coalesce(U.iCategory, null, R.iCategory) as iCategory, U.UserIDs, R.RoleIDs from U full outer join R on U.FKiNotificationMasterIDXIGUID=R.FKiNotificationMasterIDXIGUID";
            o1click.Query = sQuery;
            var oOneClick = o1click.Execute_Query();
            NotificationMaster oMasterNotification = new NotificationMaster();
            if (oOneClick.Rows.Count > 0)
            {
                oMasterNotification = (from DataRow row in oOneClick.Rows
                                       select new NotificationMaster
                                       {
                                           ID = Convert.ToInt32(row["ID"]),
                                           XIGUID = row["XIGUID"].ToString() == string.Empty ? Guid.Empty : Guid.Parse(row["XIGUID"].ToString()),
                                           sName = row["sName"].ToString(),
                                           sDescription = row["sDescription"].ToString(),
                                           sCode = row["sCode"].ToString(),
                                           sTheme = row["sTheme"].ToString(),
                                           bIsImportant = Convert.ToBoolean(row["bIsImportant"].ToString() == string.Empty ? "False" : row["bIsImportant"]),
                                           FKiLayoutIDXIGUID = row["FKiLayoutIDXIGUID"].ToString() == string.Empty ? Guid.Empty : Guid.Parse(row["FKiLayoutIDXIGUID"].ToString()),
                                           sUserDistributionList = row["UserIDs"].ToString(),
                                           sRoleDistributionList = row["RoleIDs"].ToString(),
                                           iLeft = row["iLeft"].ToString() == String.Empty ? (int?)null : Convert.ToInt32(row["iLeft"]),
                                           iTop = row["iTop"].ToString() == String.Empty ? (int?)null : Convert.ToInt32(row["iTop"]),
                                           iWidth = row["iWidth"].ToString() == String.Empty ? (int?)null : Convert.ToInt32(row["iWidth"]),
                                           iHeight = row["iHeight"].ToString() == String.Empty ? (int?)null : Convert.ToInt32(row["iHeight"]),
                                           iCategory = Convert.ToInt32(row["iCategory"])
                                       }).SingleOrDefault();
            }
            return oMasterNotification;
        }

        public static List<XIInfraUsers> GetUserNotificationDistributionList(string sUserIDs, int FKiOrgID)
        {
            List<XIInfraUsers> users = new List<XIInfraUsers>();
            if (sUserIDs != null && sUserIDs != String.Empty)
            {
                XID1Click o1click = new XID1Click();
                o1click.Name = "XIAppUsers";
                string sQuery = @"select * from XIAPPUsers_AU_T where UserID in (" + sUserIDs + ") and FKiOrgID = " + FKiOrgID.ToString();
                o1click.Query = sQuery;
                //var oOneClick = o1click.OneClick_Execute(null, o1click);
                var oOneClick = o1click.Execute_Query();
                //users = oOneClick.Values.Select(x => new XIInfraUsers
                //{
                //    UserID = x.Attributes["UserID"].iValue,
                //    sUserName = x.Attributes["sUserName"].sValue,
                //}).ToList();
                users = (from DataRow row in oOneClick.Rows
                         select new XIInfraUsers
                         {
                             UserID = Convert.ToInt32(row["UserID"]),
                             sUserName = row["sUserNAme"].ToString()
                         }).ToList();
            }
            return users;
        }

        public static List<XIInfraUsers> GetUsersFromRoleList(string sRoleIDs, string sOrgID)
        {
            XID1Click o1click = new XID1Click();
            List<XIInfraUsers> users = new List<XIInfraUsers>();
            o1click.Name = "XIAPPUsers_AU_T";
            string sQuery = @"select AU.UserID, UR.RoleID, AU.sUserName from XIAPPUsers_AU_T AU join XIAppUserRoles_AUR_T UR on AU.UserID = UR.UserID where AU.FKiOrgID = " + sOrgID + " and UR.RoleID in (" + sRoleIDs + ")";
            o1click.Query = sQuery;
            var oQueryResult = o1click.Execute_Query();

            if (oQueryResult.Rows.Count > 0)
            {
                users = (from DataRow row in oQueryResult.Rows
                         select new XIInfraUsers
                         {
                             sUserName = row["sUserName"].ToString(),
                             UserID = Convert.ToInt32(row["UserID"])
                         }).ToList();
            }
            return users;
        }

        public static void InsertUserNotifications(List<string> toastNotifications, List<XIInfraUsers> users, string sRoleIDs, string sOrgID, string sNotificationID)
        {
            List<XIInfraUsers> roleUsers = new List<XIInfraUsers>();
            if (sRoleIDs != null)
            {
                roleUsers = GetUsersFromRoleList(sRoleIDs, sOrgID);
            }
            users.AddRange(roleUsers);  

            XIInfraCache oCache = new XIInfraCache();
            XIDBO oUserNotifBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIUserNotifications_T");

            for(int i = 0; i < users.Count; i++)
            {
                XIIBO oBO = new XIIBO();
                oBO.BOD = oUserNotifBOD;
                oBO.Attributes["iUserID"] = new XIIAttribute
                {
                    sName = "iUserID",
                    sValue = users[i].UserID.ToString(),
                    bDirty = true,
                };
                oBO.Attributes["FKiNotificationID"] = new XIIAttribute
                {
                    sName = "FKiNotificationID",
                    sValue = sNotificationID,
                    bDirty = true,
                };
                oBO.Attributes["bIsToastSent"] = new XIIAttribute
                {
                    sName = "bIsToastSent",
                    sValue = toastNotifications.Any(x => x == users[i].sUserName).ToString(),
                    bDirty = true,
                };
                oBO.Save(oBO);
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
                        NotificationMaster oMasterNotification = GetNotificationMaster(n.FKiMasterNotificationIDXIGUID.ToString(), n.FKiOrgID);
                        List<XIInfraUsers> users = GetUserNotificationDistributionList(oMasterNotification?.sUserDistributionList, n.FKiOrgID);
                        List<string> userToastNotifications = NotificationHub.SendNotificationMessages(users.Select(x => x.sUserName).ToList(), oMasterNotification?.sRoleDistributionList, n.FKiOrgID.ToString(), n, null,
                                oMasterNotification.sTheme, oMasterNotification.bIsImportant,
                                oMasterNotification.FKiLayoutIDXIGUID, oMasterNotification.iLeft, oMasterNotification.iTop, oMasterNotification.iWidth, oMasterNotification.iHeight);
                        InsertUserNotifications(userToastNotifications, users, oMasterNotification.sRoleDistributionList, n.FKiOrgID.ToString(), n.ID.ToString());
                        SetNotificationSent(n.XIGUID);
                    });

                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void SetNotificationSent(Guid gNotificationIDXIGUID)
        {
            XIIXI oXII = new XIIXI();
            List<CNV> oWhrParams = new List<CNV>();
            oWhrParams.Add(new CNV { sName = "XIGUID", sValue = gNotificationIDXIGUID.ToString() });
            XIIBO oNotif = oXII.BOI("Insert NotificationSchedule", "", "Callback Request Notification", oWhrParams);
            oNotif.Attributes.Where(x => x.Value.sName == "bIsProcessed").SingleOrDefault().Value.bValue = true;
            oNotif.Attributes.Where(x => x.Value.sName == "bIsProcessed").SingleOrDefault().Value.sValue = "True";
            oNotif.Attributes.Where(x => x.Value.sName == "bIsProcessed").SingleOrDefault().Value.bDirty = true;
            oNotif.Save(oNotif);
        }

        

    }
}