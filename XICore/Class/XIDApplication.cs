using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDatabase;
using XISystem;
using System.Configuration;
using System.Data.SqlClient;

namespace XICore
{
    [Table("XIApplication_T")]
    public class XIDApplication : XIDefinitionBase
    {
        [Key]
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public string sApplicationName { get; set; }
        public string sLogo { get; set; }
        public string sDatabaseName { get; set; }
        public string sDescription { get; set; }
        [NotMapped]
        public string XIAppUserName { get; set; }
        [NotMapped]
        public string sShortName { get; set; }
        [NotMapped]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Must be 6 characters long & combination of digits, upper and lower case letters")]
        [DataType(DataType.Password)]
        public string XIAppPassword { get; set; }
        [NotMapped]
        [Compare("XIAppPassword", ErrorMessage = "The password and confirm password do not match.")]
        [DataType(DataType.Password)]
        public string XIAppConfirmPassword { get; set; }
        public string sConnectionString { get; set; }
        public string sTheme { get; set; }
        public string sUserName { get; set; }
        public string sOTP { get; set; }
        public bool bNannoApp { get; set; }
        //My Code
        [XIDBAPI.DapperIgnore]
        public List<XIDropDown> ddlXIThemes { get; set; }
        public XIDApplicationOTPSetting OTPSetting { get; set; }
        public int FKiOrgID { get; set; }
        public CResult oCResult = new CResult();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        public XIDApplication Get_XIApplicationDetails()
        {
            XIDApplication oXIDApp = new XIDApplication();
            //XIInfraUsers oXIDUsers = new XIInfraUsers();
            try
            {
                if (this.oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    if (ID != 0)
                    {
                        Params["ID"] = ID;
                        oXIDApp = Connection.Select<XIDApplication>("XIApplication_T", Params).FirstOrDefault();
                        oXIDApp.XIAppUserName = oXIDApp.sUserName;
                    }
                    //Themes
                    List<XIDropDown> ThemeTypes = new List<XIDropDown>();
                    ///check if theme class exists
                    List<XIDMasterData> oXIMasterData = new List<XIDMasterData>();
                    Dictionary<string, object> XIThemeParams = new Dictionary<string, object>();
                    oXIMasterData = Connection.Select<XIDMasterData>("XIMasterData_T", XIThemeParams).ToList();
                    ThemeTypes = oXIMasterData.Where(m => m.Name.ToLower() == "Themes".ToLower()).ToList().Select(m => new XIDropDown { Value = m.ID, text = m.Expression }).ToList();
                    ThemeTypes.Insert(0, new XIDropDown
                    {
                        text = "--Select--",
                        Value = 0
                    });
                    oXIDApp.ddlXIThemes = ThemeTypes;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oXIDApp;
        }

    }

    public class XIDApplicationOTPSetting
    {
        public int ID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid XIGUID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sCode { get; set; }
        public int iStatus { get; set; }
        public int iType { get; set; }
        public bool bSMS { get; set; }
        public bool bMail { get; set; }
        public int iOTPType { get; set; }
        public int iOTPCase { get; set; }
        public int iOTPLength { get; set; }
    }

}