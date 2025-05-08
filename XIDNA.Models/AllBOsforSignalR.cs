using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class AllBOsforSignalR
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string sName { get; set; }
        public int Count { get; set; }
        public double rPrice { get; set; }
        public int iQuoteStatus { get; set; }
        public int FKiProductVersionID { get; set; }
        public int iStatus { get; set; }
        public string sGroupQuote { get; set; }
        public int FKiBoid { get; set; }
        public string sAlertType { get; set; }
        public string sAlertMessage { get; set; }
        public int iUserID { get; set; }
        public int iRoleID { get; set; }
        public int iSentMail { get; set; }
        public Guid FKiBoidXIGUID { get; set; }
        public string sBOName { get; set; }
        public Guid XIGUID { get; set; }
        public int iInstanceID { get; set; }
        public Guid iInstanceIDXIGUID { get; set; }
        public double fPercentComplete { get; set; }
        public double rTotalAdmin { get; set;}
        public string sRefID { get; set; }
        public Guid FKiMasterNotificationIDXIGUID { get; set; }
        public int FKiMasterNotificationID { get; set; }
        public int FKiOrgID { get; set; }
        public string sCode { get; set; }
        public int iImportType { get; set; }
        public string sSubject { get; set; }
        public string sNotes { get; set; }
        public int FKiBOIID { get; set; }
        public Guid FKiBODIDXIGUID { get; set; }
        public string sTheme { get; set; }
        public bool bIsSnoozed { get; set; }
        public int? iSendToUserID { get; set; }
        public int? iSendToRoleID { get; set; }  
    }

    //public class NotificationMaster
    //{
    //    public int ID { get; set; }
    //    public string sCode { get; set; }
    //    public string sName { get; set; }
    //    public string sDescription { get; set; }
    //    public Guid XIGUID { get; set; }
    //    public string sRoleDistributionList { get; set; }
    //    public string sUserDistributionList { get; set; }

    //}
    public class XIAppUser
    {
        public int UserID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public int FKiOrgID { get; set; }
        public string sTotpSecretKey { get; set; }
        public string sUserName { get; set; }
        public string sPasswordHash { get; set; }
        public string sDatabaseName { get; set; }
        public string sCoreDatabaseName { get; set; }
        public string sAppName { get; set; }
        public string sLocation { get; set; }
        public string sPhoneNumber { get; set; }
        public string sEmail { get; set; }
        public string sFirstName { get; set; }
        public string sLastName { get; set; }
        public int iReportTo { get; set; }
        public int iPaginationCount { get; set; }
        public string sMenu { get; set; }
        public int iInboxRefreshTime { get; set; }
        public string SecurityStamp { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string sTemporaryPasswordHash { get; set; }
        public string sAccessCode { get; set; }
        public string sHierarchy { get; set; }
        public string sInsertDefaultCode { get; set; }
        public string sUpdateHierarchy { get; set; }
        public string sViewHierarchy { get; set; }
        public string sDeleteHierarchy { get; set; }
        public int FKiTeamID { get; set; }
        public bool bDebug { get; set; }
        public int XIDeleted { get; set; }
        public string sOTP { get; set; }
        public int iLogLevel { get; set; } = 30; //better move this to userpreference so that cache can be updated without signout(need to implement for user preferences)
        public long FKiRoleID { get; set; }
        public string sShortName { get; set; }
        public int iLevel { get; set; }
    }

    public class UserRoles
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public bool XIDeleted { get; set; }
    }


}
