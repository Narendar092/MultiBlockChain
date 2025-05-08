using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using XISystem;
using xiEnumSystem;
using System.Net;
using XIDatabase;
using System.Configuration;

namespace XICore
{
    public class XIDAttribute : XIDefinitionBase
    {

        public int ID { get; set; }
        public int BOID { get; set; }
        public int FieldCreatedID { get; set; }
        public int OrganizationID { get; set; }
        public string Name { get; set; }
        public string LabelName { get; set; }
        public int TypeID { get; set; }
        public string FKTableName { get; set; }
        public string Script { get; set; }
        public string ScriiptExecutionType { get; set; }
        public string MaxLength { get; set; }
        public bool IsVisible { get; set; }
        public bool IsWhere { get; set; }
        public bool IsTotal { get; set; }
        public bool IsGroupBy { get; set; }
        public bool IsOrderBy { get; set; }
        public bool IsMail { get; set; }
        public bool IsRunTime { get; set; }
        public bool IsDBValue { get; set; }
        public string DBQuery { get; set; }
        public bool IsWhereExpression { get; set; }
        public string WhereExpression { get; set; }
        public string WhereExpreValue { get; set; }
        public bool IsExpression { get; set; }
        public string ExpressionText { get; set; }
        public string ExpressionValue { get; set; }
        public bool IsDate { get; set; }
        public string DateExpression { get; set; }
        public string DateValue { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string sDBDateFormat { get; set; }
        public string sInputMaskDateFormat { get; set; }
        public bool bIsFutureDateDisable { get; set; }
        public string sDefaultDate { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string FieldClass { get; set; }
        public int FieldClassID { get; set; }
        public string Value { get; set; }
        public string sResolvedValue { get; set; }
        public string FKWhere { get; set; }
        public string sHelpText { get; set; }
        public string sNarrowBar { get; set; }
        public int FKiType { get; set; }
        public int iMasterDataID { get; set; }
        public bool IsNull { get; set; }
        public string sXMLDataType { get; set; }
        public string sPassword { get; set; }
        public string sPrecision { get; set; }
        public string sVirtualColumn { get; set; }
        public string sLock { get; set; }
        public string sCase { get; set; }
        public string ForeignTable { get; set; }
        public string ForeignColumn { get; set; }
        public int iOutputLength { get; set; }
        public bool bISConfirm { get; set; }
        public string sAllowedValue { get; set; }
        public string DefaultValue { get; set; }
        public bool IsOptionList { get; set; }
        public int iOneClickID { get; set; }
        public Guid iOneClickIDXIGUID { get; set; }
        public int FKiFileTypeID { get; set; }
        //1/6/2018
        public Nullable<Guid> BOIDXIGUID { get; set; }
        public string sNotes { get; set; }
        public string sEncrypted { get; set; }
        public string sFKBOSize { get; set; }
        public int iDisplayType { get; set; }
        public bool IsTextArea { get; set; }
        public int iDefaultPopupID { get; set; }
        public Guid iDefaultPopupIDXIGUID { get; set; }
        public string sFKBOName { get; set; }
        public bool bIsFieldDependent { get; set; }
        public int iDependentFieldID { get; set; }
        public bool bIsFieldMerge { get; set; }
        public int iMergeFieldID { get; set; }
        public bool bIsMandatory { get; set; }
        public bool bIsHidden { get; set; }
        public string sEventHandler { get; set; }
        public string sDepBOFieldIDs { get; set; }
        public string FKiDepBOFieldIDsXIGUID { get; set; }
        public bool bIsFileProcess { get; set; }
        public int iFileProcessType { get; set; }
        private bool bIsMyFileAliasName;
        public string DataType { get; set; }
        public bool bKPI { get; set; }
        public int iSystemType { get; set; }
        public string FieldMaxLength { get; set; }
        XIInfraCache oCache = new XIInfraCache();
        public int iUserID { get; set; }
        public int FKiAppID { get; set; }
        public int iOrgID { get; set; }
        public List<XIDropDown> ddlBODataTypes { get; set; }
        public List<XIDropDown> ddlAllBOs { get; set; }
        public string sDepBoName { get; set; }
        public bool bIsScript { get; set; }
        public bool bIsTrace { get; set; }
        public string sFunctionScript { get; set; }
        public string sGUID { get; set; }
        public bool bIsIncrement { get; set; }
        public string sUIStyle { get; set; }
        public bool bIsFileAliasName
        {
            get
            {
                return bIsMyFileAliasName;
            }
            set
            {
                bIsMyFileAliasName = value;
            }
        }
        //public List<XIDropDown> FieldDDL { get; set; }
        //public List<XIDropDown> ImagePathDetails { get; set; }
        public int iEnumBOD { get; set; }
        public string sPlaceHolder { get; set; }
        public int iAttributeType { get; set; }
        public bool bIsEncrypt { get; set; }
        public bool bIsUserEncrypt { get; set; }
        public bool bFKGUID { get; set; }
        public Guid XIGUID { get; set; }
        public int iDateType { get; set; }
        public int iViewGroupID { get; set; }
        private long iMyLength;
        public bool bSelfReferenceFile { get; set; }
        public string sCSS { get; set; }
        public bool bSelectNew { get; set; }
        public bool bSelectExisting { get; set; }

        public bool bOrgFK { get; set; }
        public XI1QueryAttributeDetails OverrideAttr { get; set; }
        public long iLength
        {
            get
            {
                return iMyLength;
            }
            set
            {
                iMyLength = value;
            }
        }

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
        private List<XIDropDown> oMyFieldDDL = new List<XIDropDown>();
        public List<XIDropDown> FieldDDL
        {
            get
            {
                return oMyFieldDDL;
            }
            set
            {
                oMyFieldDDL = value;
            }
        }

        private List<XIDropDown> oMyImagePathDetails = new List<XIDropDown>();
        public List<XIDropDown> ImagePathDetails
        {
            get
            {
                return oMyImagePathDetails;
            }
            set
            {
                oMyImagePathDetails = value;
            }
        }
        private List<XIDOptionList> oMyOptionList = new List<XIDOptionList>();
        public List<XIDOptionList> OptionList
        {
            get
            {
                return oMyOptionList;
            }
            set
            {
                oMyOptionList = value;
            }
        }
        private List<XILabel> oMyLabel = new List<XILabel>();
        public List<XILabel> Label
        {
            get
            {
                return oMyLabel;
            }
            set
            {
                oMyLabel = value;
            }
        }
        public bool bTrack { get; set; }
        public bool bComponent { get; set; }
        public int FKiComponentID { get; set; }
        public Guid FKiComponentIDXIGUID { get; set; }
        public int iEditor { get; set; }
        public bool bValueType { get; set; }
        public static T GetEnumValue<T>(int intValue) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return (T)Enum.ToObject(enumType, intValue);
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
                else
                {
                    sMyMinResolvedValue = "1920-01-01";
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
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        //public BODatatypes Get_BODatatype(int iValue)
        //{
        //    BODatatypes enumvalue = GetEnumValue<BODatatypes>(iValue);
        //    return enumvalue;
        //}
        //public object CheckDatatype(string sValue, BODatatypes sDataType)
        //{
        //    XIIXI oXII = new XIIXI();
        //    switch (sDataType)
        //    {
        //        case BODatatypes.BIGINT:
        //        case BODatatypes.INT:
        //        case BODatatypes.TINYINT:
        //        case BODatatypes.SMALLINT:
        //            int iValue = 0;
        //            if(string.IsNullOrEmpty(sValue))
        //            {
        //                return true;
        //            }
        //           else if (int.TryParse(sValue, out iValue))
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        case BODatatypes.DATETIME:
        //        case BODatatypes.DATETIME2:
        //            bool IsValidDate = oXII.IsValidDate(sValue);
        //            return IsValidDate;
        //        case BODatatypes.VARCHAR:
        //        case BODatatypes.NCHAR:
        //        case BODatatypes.NVARCHAR:
        //        case BODatatypes.CHAR:
        //        case BODatatypes.TEXT:
        //        case BODatatypes.NTEXT:
        //            if (string.IsNullOrEmpty(sValue))
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                string sSValue = Convert.ToString(sValue);
        //                if(!string.IsNullOrEmpty(sValue))
        //                {
        //                    return true;
        //                }
        //            }
        //            return false;
        //        case BODatatypes.DECIMAL:
        //        case BODatatypes.MONEY:
        //        case BODatatypes.SMALLMONEY:
        //            decimal dValue;
        //            if (decimal.TryParse(sValue, out dValue))
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        case BODatatypes.FLOAT:
        //            float fValue;
        //            if (string.IsNullOrEmpty(sValue))
        //            {
        //                return true;
        //            }
        //           else if (float.TryParse(sValue, out fValue))
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        case BODatatypes.BIT:
        //            bool bValue;
        //            if (string.IsNullOrEmpty(sValue))
        //            {
        //                return true;
        //            }
        //           else if (Boolean.TryParse(sValue, out bValue))
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        default:
        //            return null;
        //    }
        //}        

        //Get BOAttributes Definition
        public CResult Get_BOAttrDefinition(string iAttrID)
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
                XIDAttribute oAttrD = new XIDAttribute();
                Dictionary<string, object> NVParams = new Dictionary<string, object>();
                NVParams["ID"] = ID;
                var AttrButes = Connection.Select<XIDAttribute>("XIBOAttribute_T_N", NVParams).ToList();
                if (AttrButes.Count > 0)
                {
                    AttrButes.ForEach(m =>
                    {
                        if (m.IsOptionList)
                        {
                            Dictionary<string, object> AttrNvs = new Dictionary<string, object>();
                            AttrNvs["BOFieldID"] = m.ID;
                            //AttrNvs["StatusTypeID"] = "10";
                            var optionList = Connection.Select<XIDOptionList>("XIBOOptionList_T_N ", AttrNvs).ToList();
                            m.OptionList = optionList;
                            AttrButes[Convert.ToInt32(m.Name)] = m;
                        }
                        else
                        {
                            AttrButes[Convert.ToInt32(m.Name)] = m;
                        }
                    });
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oAttrD;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading BO definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }

        //Get SingleBOAttributes Definition
        public CResult Get_BOAttributeDefinition()
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
                XIDAttribute oAttrD = new XIDAttribute();
                Dictionary<string, object> NVParams = new Dictionary<string, object>();
                if (ID > 0)
                {
                    NVParams["ID"] = ID;
                }
                else if (!string.IsNullOrEmpty(sGUID))
                {
                    NVParams["XIGUID"] = sGUID;
                }
                oAttrD = Connection.Select<XIDAttribute>("XIBOAttribute_T_N", NVParams).FirstOrDefault();
                if (oAttrD != null)
                {
                    if (oAttrD.IsOptionList == true)
                    {
                        Dictionary<string, object> AttrNvs = new Dictionary<string, object>();
                        AttrNvs["BOFieldID"] = ID;
                        //AttrNvs["StatusTypeID"] = "10";
                        var optionList = Connection.Select<XIDOptionList>("XIBOOptionList_T_N ", AttrNvs).ToList();
                        oAttrD.OptionList = optionList;
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oAttrD;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading BO definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
        public List<XIDropDown> Get_BODataTypes()
        {
            List<XIDropDown> oBODT = new List<XIDropDown>();
            var oDTDef = Enum.GetValues(typeof(BODatatypes));
            foreach (var name in oDTDef)
            {
                XIDropDown oXID = new XIDropDown();
                oXID.text = name.ToString();
                oXID.Value = (int)Enum.Parse(typeof(BODatatypes), name.ToString().ToUpper());
                oBODT.Add(oXID);
            }
            return oBODT;
        }
    }
}