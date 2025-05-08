using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIDFieldOrigin : XIDefinitionBase
    {
        public int ID { get; set; }
        public int FKiOrganisationID { get; set; }
        public int FKiApplicationID { get; set; }

        public string sName { get; set; }
        public string sInputMaskDateFormat { get; set; }
        public bool bIsPastDateDisable { get; set; }
        public bool bIsFutureDateDisable { get; set; }
        public string sDisplayName { get; set; }

        public string sAdditionalText { get; set; }
        public int iLength { get; set; }
        public string sFieldDefaultValue { get; set; }
        public int FKiDataType { get; set; }
        public Guid FKiDataTypeXIGUID { get; set; }
        public int FK1ClickID { get; set; }
        public Guid FK1ClickIDXIGUID { get; set; }
        public bool bIsOptionList { get; set; }
        public int iMasterDataID { get; set; }
        public string sIsHidden { get; set; }
        public string sDefaultValue { get; set; }
        public string sDisplayHelp { get; set; }
        public string sPlaceHolder { get; set; }
        public string sScript { get; set; }
        public int iValidationType { get; set; }
        public int iValidationDisplayType { get; set; }
        public bool bIsMandatory { get; set; }
        public int FKiBOID { get; set; }
        public Guid FKiBOIDXIGUID { get; set; }
        public bool bIsDisable { get; set; }
        public bool bIsMerge { get; set; }
        public string sMergeField { get; set; }
        public bool bIsCompare { get; set; }
        public string sCompareField { get; set; }
        public string sMergeBo { get; set; }
        public string sMergeVariable { get; set; }
        public string sMergeBoField { get; set; }
        public string sValidationMessage { get; set; }
        public bool bIsUpperCase { get; set; }
        public bool bIsLowerCase { get; set; }
        public bool bIsHelpIcon { get; set; }
        public bool bIsAutoDropDown { get; set; }
        public string sDefaultDate { get; set; }
        public bool bIsExpression { get; set; }
        public bool bIsDependency { get; set; }
        public string sEventHandler { get; set; }
        public string sFKiOrginIDs { get; set; }
        public bool bIsFieldDependent { get; set; }
        public string sDependentFieldID { get; set; }
        public bool bIsFieldMerge { get; set; }
        public int iMergeFieldID { get; set; }
        public string sCode { get; set; }
        public string sFormat { get; set; }
        public string sFormatCode { get; set; }
        public string sDependentFieldIDs { get; set; }
        public int iScriptType { get; set; }
        public bool bIsDisplay { get; set; }
        public bool bIsReload { get; set; }
        public bool bIsDBLoad { get; set; }
        public bool bIsEncrypt { get; set; }
        public bool bIsModify { get; set; }
        public string sDependentValue { get; set; }
        public Dictionary<string, string> ddlOneClicks { get; set; }
        public List<XIDropDown> ddlMasterTypes { get; set; }
        public List<XIDFieldOptionList> ddlFieldOptionList { get; set; }
        public List<XIDropDown> ddlDataTypes { get; set; }
        public XIDDataType DataTypes { get; set; }
        public string sOneClickName { get; set; }
        public List<XIDropDown> ddlApplications { get; set; }
        public List<XIDropDown> XIFields { get; set; }//auto complete fields
        public Dictionary<string, string> ddlBOs { get; set; }
        public string sBOName { get; set; }
        public bool bIsHidden { get; set; }
        public string sQSCode { get; set; }
        public bool bIsSetToCache { get; set; }
        public string sCacheName { get; set; }
        public Guid FkiRuleIDXIGUID { get; set; }
        public int FkiRuleID { get; set; }
        public string sRuleCondition { get; set; }
        public int iDateType { get; set; }
        public string sFunctionScript { get; set; }
        public bool bIsPrescriptvalidation { get; set; }
        public string sBaseDataType { get; set; }
        public Guid XIGUID { get; set; }
        public string sCSS { get; set; }
        public bool bOrgFK { get; set; }
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        private XIDDataType oMyDataType;
        public XIDDataType DataType
        {
            get
            {
                return oMyDataType;
            }
            set
            {
                oMyDataType = value;
            }
        }

        private List<XIDFieldOptionList> oMyFieldOptionList = new List<XIDFieldOptionList>();
        public List<XIDFieldOptionList> FieldOptionList
        {
            get
            {
                return oMyFieldOptionList;
            }
            set
            {
                oMyFieldOptionList = value;
            }
        }

        public bool bIsLargeBO { get; set; }

        public string sBOSize { get; set; }
        public List<XIDFieldOptionList> FieldDynamicOptionList { get; set; }

        private string sMyMinDate;
        public string sMinDate
        {
            get
            {
                return sMyMinDate;
            }
            set
            {
                sMyMinDate = value;
            }
        }

        private string sMyMaxDate;
        public string sMaxDate
        {
            get
            {
                return sMyMaxDate;
            }
            set
            {
                sMyMaxDate = value;
            }
        }
        //mindate resolving
        private string sMyMinResolvedValue;
        public string sMinResolvedValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sMinDate))
                {
                    sMyMinResolvedValue = Utility.GetDateResolvedValue(sMinDate, "yyyy-MM-dd");
                }
                return sMyMinResolvedValue;
            }
            set
            {
                sMyMinResolvedValue = value;
            }
        }
        //maxdate resolving
        private string sMyMaxResolvedValue;
        public string sMaxResolvedValue
        {
            get
            {
                if (!string.IsNullOrEmpty(sMaxDate))
                {
                    sMyMaxResolvedValue = Utility.GetDateResolvedValue(sMaxDate, "yyyy-MM-dd");
                }
                return sMyMaxResolvedValue;
            }
            set
            {
                sMyMaxResolvedValue = value;
            }
        }

        public CResult GetFieldDefinition(string sField = "")
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
                XIDFieldOrigin oFD = new XIDFieldOrigin();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(sField) && sField != "0")
                {
                    Params["ID"] = sField;
                    oFD = Connection.Select<XIDFieldOrigin>("XIFieldOrigin_T", Params).FirstOrDefault();
                }
                Dictionary<string, object> OptParam = new Dictionary<string, object>();
                OptParam["FKiQSFieldID"] = sField;
                oFD.ddlFieldOptionList = Connection.Select<XIDFieldOptionList>("XIFieldOptionList_T", OptParam).ToList();
                List<XIDDataType> oXIDT = new List<XIDDataType>();
                Dictionary<string, object> Params1 = new Dictionary<string, object>();
                oXIDT = Connection.Select<XIDDataType>("XIDataType_T", Params1).ToList();
                var oDataTypesDDL = oXIDT.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.sName }).ToList();
                oFD.ddlDataTypes = oDataTypesDDL;
                List<XIDMasterData> oXIDM = new List<XIDMasterData>();
                Dictionary<string, object> Params2 = new Dictionary<string, object>();
                Params2["Name"] = "Sys Type";
                oXIDM = Connection.Select<XIDMasterData>("XIMasterData_T", Params2).ToList();
                var oXIDMDDL = oXIDT.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.sName }).ToList();
                oFD.ddlMasterTypes = oXIDMDDL;
                Dictionary<string, object> Params3 = new Dictionary<string, object>();
                Dictionary<string, string> XIFields = new Dictionary<string, string>();
                var lXiFields = Connection.Select<XID1Click>("XI1Click_T", Params1).ToList();
                foreach (var items in lXiFields)
                {
                    XIFields[items.Name] = items.ID.ToString();
                }
                oFD.ddlOneClicks = XIFields;
                List<XIDFieldOrigin> oFDs = new List<XIDFieldOrigin>();
                Dictionary<string, object> Params4 = new Dictionary<string, object>();
                Params4["FKiOrgID"] = FKiOrganisationID;
                Params4["FKiApplicationID"] = FKiApplicationID;
                oFDs = Connection.Select<XIDFieldOrigin>("XIFieldOrigin_T", Params4).ToList();
                var oXIFields = oFDs.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.sName }).ToList();
                oFD.XIFields = oXIFields;

                Dictionary<string, object> Params5 = new Dictionary<string, object>();
                Dictionary<string, string> AllBOs = new Dictionary<string, string>();
                var oAllBOs = Connection.Select<XIDBO>("XIBO_T_N", Params5).ToList();
                foreach (var items in oAllBOs)
                {
                    AllBOs[items.Name] = items.BOID.ToString();
                }
                oFD.ddlBOs = AllBOs;
                if (oFD.ID == 0)
                {
                    oFD.FKiApplicationID = FKiApplicationID;
                }
                oCResult.oResult = oFD;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Layout definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

    }
}