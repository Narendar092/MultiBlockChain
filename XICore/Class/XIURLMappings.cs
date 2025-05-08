using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using XIDatabase;
using System.Configuration;
using XISystem;

namespace XICore
{
    [Table("XIUrlMappings_T")]
    public class XIURLMappings : XIDefinitionBase
    {
        [Key]
        public int ID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        [Required(ErrorMessage = "Enter URL name")]
        [StringLength(128, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 1)]
        [Remote("IsExistsUrlMappingName", "XIApplications", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "Url name already exists. Please enter a different Name.")]
        public string sUrlName { get; set; }
        [Required(ErrorMessage = "Enter Actual Url Name")]
        public string sActualUrl { get; set; }
        public string sType { get; set; }
        public string sValidateKey { get; set; }
        public int FKiSourceID { get; set; }
        public int StatusTypeID { get; set; }
        public string sPage { get; set; }
        public bool bIsValidateKeyMandatory { get; set; }

        [NotMapped]
        public List<XIDropDown> ddlApplications { get; set; }
        public List<XIDropDown> SourceList { get; set; }
        public int OrganisationID { get; set; }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        public CResult Get_URLDefinition(string sUrlName = "")
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

            try
            {
                XIURLMappings oURL = new XIURLMappings();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (ID > 0)
                {
                    Params["ID"] = ID;
                    oURL = Connection.Select<XIURLMappings>("XIUrlMappings_T", Params).FirstOrDefault();
                }

                //All Sources DropDowns
                List<XIDropDown> oSourceDDL = new List<XIDropDown>();
                Dictionary<string, object> oSourceParam = new Dictionary<string, object>();
                //oSourceParam["FKiApplicationID"] = FKiApplicationID;
                //oSourceParam["OrganisationID"] = OrganisationID;
                var oSourceDef = Connection.Select<XIDSource>("XISource_T", oSourceParam).ToList();
                oSourceDDL = oSourceDef.Select(m => new XIDropDown { Value = m.ID, text = m.sName }).ToList();
                oSourceDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oURL.SourceList = oSourceDDL;
                if (oURL.ID == 0)
                {
                    oURL.FKiApplicationID = FKiApplicationID;
                }
                oCResult.oResult = oURL;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading URL definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }
    }
}