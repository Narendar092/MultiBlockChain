using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIQSLink
    {
        public long ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiSectionDefinitionID { get; set; }
        public Guid FKiSectionDefinitionIDXIGUID { get; set; }
        public int FKiXILinkID { get; set; }
        public int FKiStepDefinitionID { get; set; }
        public string sCode { get; set; }
        public int FKiQSDefinitionID { get; set; }
        public List<XIDropDown> ddlQSLinkCodes { get; set; }
        public List<XIDropDown> ddlQuestionSets { get; set; }
        public List<XIDropDown> ddlQSStteps { get; set; }
        public List<XIDropDown> ddlSections { get; set; }
        public List<XIDropDown> ddlApplications { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        private Dictionary<string, XIQSLinkDefintion> oMyXILink = new Dictionary<string, XIQSLinkDefintion>();
        public Dictionary<string, XIQSLinkDefintion> XiLink
        {
            get
            {
                return oMyXILink;
            }
            set
            {
                oMyXILink = value;
            }
        }

        #region QSLink

        public CResult Get_QSLinkDetails()
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
                XIQSLink oLink = new XIQSLink();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (ID != 0)
                {
                    Params["ID"] = ID;
                    oLink = Connection.Select<XIQSLink>("XIQSLink_T", Params).FirstOrDefault();
                }
                //List<XIDropDown> oLinkCodes = new List<XIDropDown>();
                //Dictionary<string, object> CodeParams = new Dictionary<string, object>();
                //var oCodeList = Connection.Select<XIQSLink>("XIQSLink_T", CodeParams).ToList().Select(m => m.sCode).Distinct();
                //foreach (var code in oCodeList)
                //{
                //    oLinkCodes.Insert(0, new XIDropDown
                //    {
                //        text = code,
                //        Expression = code
                //    });
                //}
                //oLink.ddlQSLinkCodes = oLinkCodes;
                //List<XIDropDown> QSDef = new List<XIDropDown>();
                //Dictionary<string, object> QSDefParams = new Dictionary<string, object>();
                //var oQSDefList = Connection.Select<XIDQS>("XIQSDefinition_T", QSDefParams).ToList();
                //QSDef = oQSDefList.Select(m => new XIDropDown { Value = m.ID, text = m.sName }).ToList();
                //QSDef.Insert(0, new XIDropDown
                //{
                //    text = "--Select--",
                //    Value = 0
                //});
                //oLink.ddlQuestionSets = QSDef;
                //List<XIDropDown> QSSteps = new List<XIDropDown>();
                //Dictionary<string, object> QSStepsParams = new Dictionary<string, object>();
                //if (FKiQSDefinitionID > 0)
                //{
                //    QSStepsParams["FKiQSDefintionID"] = FKiQSDefinitionID;
                //}
                //var oQSStepsList = Connection.Select<XIDQSStep>("XIQSStepDefinition_T", QSStepsParams).ToList();
                //QSSteps = oQSStepsList.Select(m => new XIDropDown { Value = m.ID, text = m.sName }).ToList();
                //QSSteps.Insert(0, new XIDropDown
                //{
                //    text = "--Select--",
                //    Value = 0
                //});
                //oLink.ddlQSStteps = QSSteps;
                //List<XIDropDown> QSSecs = new List<XIDropDown>();
                //Dictionary<string, object> QSSecsParams = new Dictionary<string, object>();
                //if (FKiStepDefinitionID > 0)
                //{
                //    QSSecsParams["FKiStepDefinitionID"] = FKiStepDefinitionID;
                //}
                //var oQSSecsList = Connection.Select<XIDQSSection>("XIStepSectionDefinition_T", QSSecsParams).ToList();
                //QSSecs = oQSSecsList.Select(m => new XIDropDown { Value = m.ID, text = m.sName }).ToList();
                //QSSecs.Insert(0, new XIDropDown
                //{
                //    text = "--Select--",
                //    Value = 0
                //});
                //oLink.ddlSections = QSSecs;
                //if (oLink.ID == 0)
                //{
                //    oLink.FKiApplicationID = FKiApplicationID;
                //}
                oCResult.oResult = oLink;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading QSLink definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        #endregion QSLink
    }
}