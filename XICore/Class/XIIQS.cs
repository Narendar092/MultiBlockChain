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
using XIDatabase;
using System.Configuration;
using static XIDatabase.XIDBAPI;
using System.Data.SqlClient;
using Dapper;
using XISystem;
using xiEnumSystem;
using System.Web;
using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace XICore
{
    public class XIIQS : XIInstanceBase
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiQSDefinitionID { get; set; }
        public Guid FKiQSDefinitionIDXIGUID { get; set; }
        public int iCurrentStepID { get; set; }
        public Guid iCurrentStepIDXIGUID { get; set; }
        [DapperIgnore]
        public string sCurrentStepName { get; set; }
        [DapperIgnore]
        public string sQSType { get; set; }
        public string sQSName { get; set; }
        public string FKiUserCookieID { get; set; }
        public int FKiBODID { get; set; }
        public int iBOIID { get; set; }
        public int FKiOriginID { get; set; }
        [DapperIgnore]
        //public int iActiveStepID { get; set; }
        public Guid iActiveStepIDXIGUID { get; set; }
        [DapperIgnore]
        public string sOrgDatabase { get; set; }
        [DapperIgnore]
        public string sMode { get; set; }
        [DapperIgnore]
        public string sGUID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int FKiSourceID { get; set; }
        public string sExternalRefID { get; set; }
        public int FKiClassID { get; set; }
        public int XIDeleted { get; set; }
        public bool bAdminTakeOver { get; set; }
        public int iStage { get; set; }
        public int? iOverrideStep { get; set; }
        public int FKiOrgID { get; set; }
        public int iParentQSIID { get; set; }
        [DapperIgnore]
        public int iLeadStatus { get; set; }
        [DapperIgnore]
        public string sCssClass { get; set; }
        public bool bIsAutoReload { get; set; }
        public bool bIsAutoSaving { get; set; }
        public bool bIsDisable { get; set; }
        //[DapperIgnore]
        //private string osMode { get; set; }
        //[DapperIgnore]
        //public string sMode
        //{
        //    get
        //    {
        //        return osMode;
        //    }
        //    set
        //    {
        //        osMode = "QuestionSet";
        //    }
        //}
        public List<int> History { get; set; }
        public List<Guid> HistoryXIGUID { get; set; }
        public CResult oCResult = new CResult();
        private XIDQS oMyDefinition;
        public XIDQS QSDefinition
        {
            get
            {
                return oMyDefinition;
            }
            set
            {
                oMyDefinition = value;
            }
        }
        public object oDynamicObject { get; set; }
        [DapperIgnore]
        public string sHtmlPage { get; set; }
        private Dictionary<string, XIIQSStep> oMySteps = new Dictionary<string, XIIQSStep>();

        public Dictionary<string, XIIQSStep> Steps
        {
            get
            {
                return oMySteps;
            }
            set
            {
                oMySteps = value;
            }
        }
        private Dictionary<string, XIIValue> oMyXIValues = new Dictionary<string, XIIValue>(StringComparer.CurrentCultureIgnoreCase);

        public Dictionary<string, XIIValue> XIValues
        {
            get
            {
                return oMyXIValues;
            }
            set
            {
                oMyXIValues = value;
            }
        }
        private Dictionary<string, List<XIIBO>> oStructureInstance = new Dictionary<string, List<XIIBO>>();

        public XIIQSStep StepI(string sStepName)
        {
            XIIQSStep oThisStepI = null/* TODO Change to default(_) if this is not a reference type */;

            // The steps of this QS must be loaded

            sStepName = sStepName.ToLower();

            if (oMySteps.ContainsKey(sStepName) == false)
            {
            }

            if (oMySteps.ContainsKey(sStepName))
            {
            }
            else
            {
            }

            return oThisStepI;
        }

        public Dictionary<string, XIIBO> Get_Collection(string sStepName = "", string sSectionName = "")
        {
            Dictionary<string, XIIBO> oResults = new Dictionary<string, XIIBO>();

            // TO DO - run the 1-click which is on this step (in a property on the 1-click component)


            return oResults;
        }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public XIIQS Save(XIIQS oQSInstance, string sCurrentGuestUser)
        {
            // TO DO - return type should be an object

            // TO DO - this method means, there is a full object model in memory with steps and maybe sections
            // and this code needs to persist this into the DB
            XIIQS oQSIns = null;
            Dictionary<string, object> Params = new Dictionary<string, object>();
            //Params["FKiQSDefinitionID"] = oQSInstance.FKiQSDefinitionID;
            if (oQSInstance.FKiBODID > 0)
            {
                Params["FKiBODID"] = oQSInstance.FKiBODID;
                Params["iBOIID"] = oQSInstance.iBOIID;
                oQSIns = Connection.Select<XIIQS>("XIQSInstance_T", Params).FirstOrDefault();
            }
            else
            {
                if (oQSInstance.XIGUID != null && oQSInstance.XIGUID != Guid.Empty)
                {
                    Params["XIGUID"] = oQSInstance.XIGUID;
                    oQSIns = Connection.Select<XIIQS>("XIQSInstance_T", Params).FirstOrDefault();
                }
                else if (oQSInstance.ID > 0)
                {
                    Params["ID"] = oQSInstance.ID;
                    oQSIns = Connection.Select<XIIQS>("XIQSInstance_T", Params).FirstOrDefault();
                }
            }
            CResult oCR = new CResult();
            XIInfraCache oCache = new XIInfraCache();
            if (oQSIns == null)
            {
                oQSIns = new XIIQS();
                XIIBO oBOI = new XIIBO();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "QS Instance");
                oBOI.BOD = oBOD;
                oBOI.SetAttribute("sQSName", oQSInstance.QSDefinition.sName);
                oBOI.SetAttribute("iCurrentStepIDXIGUID", oQSInstance.iCurrentStepIDXIGUID.ToString());
                oBOI.SetAttribute("FKiQSDefinitionID", oQSInstance.FKiQSDefinitionID.ToString());
                oBOI.SetAttribute("FKiQSDefinitionIDXIGUID", oQSInstance.FKiQSDefinitionIDXIGUID.ToString());
                oBOI.SetAttribute("FKiUserCookieID", sCurrentGuestUser);
                oBOI.SetAttribute("FKiBODID", oQSInstance.FKiBODID.ToString());
                oBOI.SetAttribute("iBOIID", oQSInstance.iBOIID.ToString());
                oBOI.SetAttribute("FKiClassID", oQSInstance.QSDefinition.FKiClassID.ToString());
                oBOI.SetAttribute("CreatedTime", DateTime.Now.ToString());
                oBOI.SetAttribute("FKiOrgID", oQSInstance.FKiOrgID.ToString());
                var oStep = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oQSInstance.iCurrentStepIDXIGUID).FirstOrDefault();
                oBOI.SetAttribute("iStage", oStep.iStage.ToString());
                oCR = oBOI.Save(oBOI);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oBOI = (XIIBO)oCR.oResult;
                    var ID = oBOI.AttributeI("id").sValue;
                    var XIGUID = oBOI.AttributeI("xiguid").sValue;
                    int iQSIID = 0;
                    int.TryParse(ID, out iQSIID);
                    Guid QSGUID = Guid.Empty;
                    Guid.TryParse(XIGUID, out QSGUID);
                    oQSInstance.ID = iQSIID;
                    oQSInstance.XIGUID = QSGUID;
                }
                //oQSIns = Connection.Insert<XIIQS>(oQSIns, "XIQSInstance_T", "ID");
                //InsertIntoAggregations(oQSIns.ID, sDatabase);
            }
            else
            {
                XIIBO oBOI = new XIIBO();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "QS Instance");
                oBOI.BOD = oBOD;
                oBOI.SetAttribute("id", oQSIns.ID.ToString());
                oBOI.SetAttribute("xiguid", oQSIns.XIGUID.ToString());
                int iStage = 0;
                var oStep = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oQSInstance.iCurrentStepIDXIGUID).FirstOrDefault();
                if (oQSIns.iStage <= oStep.iStage)
                {
                    oBOI.SetAttribute("iStage", oStep.iStage.ToString());
                    iStage = oStep.iStage;
                }
                else if (oQSIns.iStage > oStep.iStage && oStep.iCutStage <= oQSIns.iStage && oQSIns.iStage < oStep.iLockStage)
                {
                    oBOI.SetAttribute("iStage", oStep.iCutStage.ToString());
                    iStage = oStep.iCutStage;
                }
                oBOI.SetAttribute("sQSName", oQSInstance.sQSName);
                oBOI.SetAttribute("iCurrentStepIDXIGUID", oQSInstance.iCurrentStepIDXIGUID.ToString());
                oBOI.SetAttribute("FKiQSDefinitionID", oQSInstance.FKiQSDefinitionID.ToString());
                oBOI.SetAttribute("FKiQSDefinitionIDXIGUID", oQSInstance.FKiQSDefinitionIDXIGUID.ToString());
                oBOI.SetAttribute("FKiOrgID", oQSInstance.FKiOrgID.ToString());
                oCR = oBOI.Save(oBOI);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oQSInstance.ID = oQSIns.ID;
                    oQSInstance.XIGUID = oQSIns.XIGUID;
                    oQSInstance.iStage = iStage;
                }
                //oQSIns = Connection.Update<XIIQS>(oQSIns, "XIQSInstance_T", "ID");

            }
            foreach (var oStep in oQSInstance.Steps.Values.Where(m => m.bIsCurrentStep == true))
            {
                List<XIIBO> oBulkBO = new List<XIIBO>();
                XIIBO BulkInsert = new XIIBO();
                XIIQSStep oQSStepIns;
                XIInfraEncryption oEncrypt = new XIInfraEncryption();
                Dictionary<string, object> StepParams = new Dictionary<string, object>();
                StepParams["FKiQSInstanceIDXIGUID"] = oQSIns.XIGUID;
                StepParams["FKiQSStepDefinitionIDXIGUID"] = oStep.FKiQSStepDefinitionIDXIGUID;
                oQSStepIns = Connection.Select<XIIQSStep>("XIQSStepInstance_T", StepParams).FirstOrDefault(); ;// dbContext.QSStepInstance.Where(m => m.FKiQSInstanceID == oQSIns.ID && m.FKiQSStepDefinitionID == oStep.FKiQSStepDefinitionID).FirstOrDefault();
                if (oQSStepIns == null)
                {
                    oQSStepIns = new XIIQSStep();
                    XIIBO oStepI = new XIIBO();
                    var oStepBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIQSStepInstance_T");
                    oStepI.BOD = oStepBOD;
                    oStepI.SetAttribute("FKiQSInstanceID", oQSIns.ID.ToString());
                    oStepI.SetAttribute("FKiQSInstanceIDXIGUID", oQSIns.XIGUID.ToString());
                    oStepI.SetAttribute("FKiQSStepDefinitionID", oStep.FKiQSStepDefinitionID.ToString());
                    oStepI.SetAttribute("FKiQSStepDefinitionIDXIGUID", oStep.FKiQSStepDefinitionIDXIGUID.ToString());
                    oStepI.SetAttribute("FKiOrgID", oQSIns.FKiOrgID.ToString());
                    oCR = oStepI.Save(oStepI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oStepI = (XIIBO)oCR.oResult;
                        var ID = oStepI.AttributeI("id").sValue;
                        var XIGUID = oStepI.AttributeI("xiguid").sValue;
                        int iQSStepIID = 0;
                        int.TryParse(ID, out iQSStepIID);
                        Guid QSStepGUID = Guid.Empty;
                        Guid.TryParse(XIGUID, out QSStepGUID);
                        oStep.ID = iQSStepIID;
                        oStep.XIGUID = QSStepGUID;
                        oQSStepIns.ID = iQSStepIID;
                        oQSStepIns.XIGUID = QSStepGUID;
                        oQSStepIns.FKiOrgID = oQSIns.FKiOrgID;
                    }
                    //oQSStepIns.FKiQSStepDefinitionIDXIGUID = oStep.XIGUID.
                    // oQSStepIns = Connection.Insert<XIIQSStep>(oQSStepIns, "XIQSStepInstance_T", "ID");
                }
                //var AllFVlaueInstances = dbContext.XIFieldInstance.Where(m => m.FKiQSInstanceID == oQSIns.ID && m.FKiQSStepDefinitionID == oStep.FKiQSStepDefinitionID).ToList();
                //dbContext.XIFieldInstance.RemoveRange(AllFVlaueInstances);
                //dbContext.SaveChanges();

                if (oStep.Sections != null && oStep.Sections.Values.Count() > 0)
                {
                    foreach (var sec in oStep.Sections)
                    {
                        XIIQSSection oSecIns = new XIIQSSection();
                        Dictionary<string, object> SecParams = new Dictionary<string, object>();
                        SecParams["FKiStepSectionDefinitionIDXIGUID"] = sec.Value.FKiStepSectionDefinitionIDXIGUID;
                        SecParams["FKiStepInstanceIDXIGUID"] = oStep.XIGUID;
                        oSecIns = Connection.Select<XIIQSSection>("XIStepSectionInstance_T", SecParams).FirstOrDefault();
                        //oSecIns = dbContext.StepSectionInstance.Where(m => m.FKiStepSectionDefinitionID == sec.FKiStepSectionDefinitionID && m.FKiStepInstanceID == oStep.ID).FirstOrDefault();
                        if (oSecIns == null)
                        {
                            oSecIns = new XIIQSSection();
                            XIIBO oSecI = new XIIBO();
                            var oSectionBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIStepSectionInstance_T");
                            oSecI.BOD = oSectionBOD;
                            oSecI.SetAttribute("FKiStepSectionDefinitionID", sec.Value.FKiStepSectionDefinitionID.ToString());
                            oSecI.SetAttribute("FKiStepInstanceID", oQSStepIns.ID.ToString());
                            oSecI.SetAttribute("FKiStepInstanceIDXIGUID", oQSStepIns.XIGUID.ToString());
                            oSecI.SetAttribute("FKiOrgID", oQSStepIns.FKiOrgID.ToString());
                            oSecI.SetAttribute("FKiStepSectionDefinitionIDXIGUID", sec.Value.FKiStepSectionDefinitionIDXIGUID.ToString());
                            oCR = oSecI.Save(oSecI);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oSecI = (XIIBO)oCR.oResult;
                                var ID = oSecI.AttributeI("id").sValue;
                                var XIGUID = oSecI.AttributeI("xiguid").sValue;
                                int iQSSecIID = 0;
                                int.TryParse(ID, out iQSSecIID);
                                Guid QSSecGUID = Guid.Empty;
                                Guid.TryParse(XIGUID, out QSSecGUID);
                                oSecIns.ID = iQSSecIID;
                                oSecIns.XIGUID = QSSecGUID;
                                sec.Value.ID = iQSSecIID;
                                sec.Value.XIGUID = QSSecGUID;
                                sec.Value.FKiStepInstanceID = oQSStepIns.ID;
                                sec.Value.FKiStepInstanceIDXIGUID = oQSStepIns.XIGUID;
                            }

                            //oSecIns = Connection.Insert<XIIQSSection>(oSecIns, "XIStepSectionInstance_T", "ID");
                        }

                        if (sec.Value.XIValues != null && sec.Value.XIValues.Count() > 0)
                        {
                            //var SecFValueInstances = dbContext.XIFieldInstance.Where(m => m.FKiQSInstanceID == oQSIns.ID && m.FKiQSSectionDefinitionID == sec.FKiStepSectionDefinitionID).ToList();
                            //dbContext.XIFieldInstance.RemoveRange(SecFValueInstances);
                            //dbContext.SaveChanges();
                            foreach (var items in sec.Value.XIValues)
                            {
                                items.Value.sValue = items.Value.sValue == null ? null : items.Value.sValue.Trim();
                                XIIValue oFIns = new XIIValue();
                                var StepDef = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oStep.FKiQSStepDefinitionIDXIGUID).FirstOrDefault();
                                var SecDef = StepDef.Sections.Values.Where(m => m.XIGUID == sec.Value.FKiStepSectionDefinitionIDXIGUID).FirstOrDefault();
                                var FieldOrigin = SecDef.FieldDefs.Values.Where(m => m.XIGUID == items.Value.FKiFieldDefinitionIDXIGUID).FirstOrDefault().FieldOrigin;
                                if (FieldOrigin.bIsMandatory && items.Value.sValue == null)
                                {
                                    //critical failure and write hack alert to Log
                                    oCR = new CResult();
                                    oCR.sCode = "Hack";
                                    oCR.sMessage = "Critical Failure: [XIIQS_Save()] there is a mandatory field with no value in it. FieldOrigin ID:" + FieldOrigin.XIGUID + " - FieldOrigin.Name:" + FieldOrigin.sName;
                                    SaveErrortoDB(oCR);
                                }
                                if (FieldOrigin.bIsOptionList)
                                {
                                    items.Value.sDerivedValue = FieldOrigin.FieldOptionList.Where(m => m.sOptionValue == items.Value.sValue).Select(m => m.sOptionName).FirstOrDefault();
                                }
                                Dictionary<string, object> fieldParams = new Dictionary<string, object>();
                                fieldParams["FKiQSInstanceIDXIGUID"] = oQSIns.XIGUID;
                                //fieldParams["FKiQSSectionDefinitionID"] = sec.Value.FKiStepSectionDefinitionID;
                                fieldParams["FKiFieldOriginIDXIGUID"] = items.Value.FKiFieldOriginIDXIGUID;
                                oFIns = Connection.Select<XIIValue>("XIFieldInstance_T", fieldParams).FirstOrDefault();

                                //oFIns = dbContext.XIFieldInstance.Where(m => m.FKiQSInstanceID == oQSIns.ID && m.FKiQSSectionDefinitionID == sec.FKiStepSectionDefinitionID && m.FKiFieldDefinitionID == items.FKiFieldDefinitionID).FirstOrDefault();
                                if (oFIns != null)
                                {
                                    if (items.Value.sValue != items.Value.sPreviousValue)
                                    {
                                        XIIBO oFInsI = new XIIBO();
                                        var oFieldBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T");
                                        oFInsI.BOD = oFieldBOD;
                                        oFInsI.SetAttribute("ID", oFIns.ID.ToString());
                                        oFInsI.SetAttribute("xiguid", oFIns.XIGUID.ToString());
                                        oFInsI.SetAttribute("FKiQSInstanceID", oQSInstance.ID.ToString());
                                        oFInsI.SetAttribute("FKiQSInstanceIDXIGUID", oQSInstance.XIGUID.ToString());
                                        oFInsI.SetAttribute("FKiStepInstanceID", oStep.ID.ToString());
                                        oFInsI.SetAttribute("FKiSectionInstanceID", oSecIns.ID.ToString());
                                        oFInsI.SetAttribute("FKiQSSectionDefinitionID", sec.Value.FKiStepSectionDefinitionID.ToString());
                                        oFInsI.SetAttribute("FKiQSStepDefinitionID", oStep.FKiQSStepDefinitionID.ToString());
                                        oFInsI.SetAttribute("FKiStepInstanceIDXIGUID", oStep.XIGUID.ToString());
                                        oFInsI.SetAttribute("FKiSectionInstanceIDXIGUID", oSecIns.XIGUID.ToString());
                                        oFInsI.SetAttribute("FKiQSSectionDefinitionIDXIGUID", sec.Value.FKiStepSectionDefinitionIDXIGUID.ToString());
                                        oFInsI.SetAttribute("FKiQSStepDefinitionIDXIGUID", oStep.FKiQSStepDefinitionIDXIGUID.ToString());
                                        oFInsI.SetAttribute("iStage", StepDef.iStage.ToString());
                                        oFInsI.SetAttribute("bIsDisplay", FieldOrigin.bIsDisplay.ToString());
                                        oFInsI.SetAttribute("bIsModify", FieldOrigin.bIsModify.ToString());
                                        oFInsI.SetAttribute("FKiOrgID", oSecIns.FKiOrgID.ToString());
                                        if (FieldOrigin.DataType.sBaseDataType.ToLower() == "int")
                                        {
                                            int ival;
                                            if (int.TryParse(items.Value.sValue, out ival))
                                            {
                                                oFInsI.SetAttribute("iValue", ival.ToString());
                                            }
                                            else
                                            {
                                                oFInsI.SetAttribute("iValue", 0.ToString());
                                                oFInsI.SetAttribute("sValue", "0");
                                            }
                                        }
                                        else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "datetime")
                                        {
                                            //if (!string.IsNullOrEmpty(items.Value.sValue))
                                            //{
                                            //    oFIns.dValue = Convert.ToDateTime(items.Value.sValue);
                                            //}
                                        }
                                        else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "decimal")
                                        {
                                            decimal rval;
                                            if (decimal.TryParse(items.Value.sValue, out rval))
                                            {
                                                oFInsI.SetAttribute("rValue", rval.ToString());
                                            }
                                            else
                                            {
                                                oFInsI.SetAttribute("rValue", 0.ToString());
                                            }
                                        }
                                        else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "boolean")
                                        {
                                            if (items.Value.sValue == "on")
                                            {
                                                oFInsI.SetAttribute("bValue", true.ToString());
                                            }
                                            else
                                            {
                                                oFInsI.SetAttribute("bValue", false.ToString());
                                            }
                                        }
                                        oFIns.sDerivedValue = items.Value.sDerivedValue;
                                        oFInsI.SetAttribute("sDerivedValue", items.Value.sDerivedValue);
                                        oFIns.sValue = items.Value.sValue;
                                        if (FieldOrigin.bIsEncrypt)
                                        {
                                            oFInsI.SetAttribute("sValue", oEncrypt.EncryptData(oFIns.sValue, true, oFIns.ID.ToString()));
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(items.Value.sValue))
                                            {
                                                oFInsI.SetAttribute("sValue", items.Value.sValue);
                                            }
                                            else if (items.Value.sValue != items.Value.sPreviousValue)
                                            {
                                                oFInsI.SetAttribute("sValue", items.Value.sValue);
                                            }
                                            //else
                                            //{
                                            //    if (string.IsNullOrEmpty(items.Value.sValue))
                                            //    {
                                            //        oFInsI.SetAttribute("sValue", items.Value.sValue);
                                            //    }
                                            //}
                                        }
                                        if (string.IsNullOrEmpty(oFIns.sDerivedValue))
                                        {
                                            oFInsI.SetAttribute("sDerivedValue", items.Value.sValue);
                                        }
                                        oFInsI.SetAttribute("dValue", DateTime.Now.ToString());
                                        //oFIns.dValue =Convert.ToDateTime(items.Value.sValue);
                                        //oFIns.sDerivedValue = items.Value.sValue;    
                                        oFInsI.SetAttribute("XIDeleted", 0.ToString());
                                        oCR = oFInsI.Save(oFInsI);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {

                                        }
                                        //oFIns = Connection.Update<XIIValue>(oFIns, "XIFieldInstance_T", "ID");
                                    }
                                }
                                else  /*TO DO INSERT BUIL XIIBO List Object here*/
                                {

                                    XIIBO oxiibo = new XIIBO();
                                    oFIns = new XIIValue();
                                    if (FieldOrigin.DataType.sBaseDataType.ToLower() == "int")
                                    {
                                        int ival;
                                        if (int.TryParse(items.Value.sValue, out ival))
                                        {
                                            oFIns.iValue = ival;
                                        }
                                        else
                                        {
                                            oFIns.iValue = 0;
                                        }
                                    }
                                    else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "datetime")
                                    {
                                        //if (!string.IsNullOrEmpty(items.Value.sValue))
                                        //{
                                        //    oFIns.dValue = Convert.ToDateTime(items.Value.sValue);
                                        //}
                                    }
                                    else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "decimal")
                                    {
                                        decimal rval;
                                        if (decimal.TryParse(items.Value.sValue, out rval))
                                        {
                                            oFIns.rValue = rval;
                                        }
                                        else
                                        {
                                            oFIns.rValue = 0;
                                        }
                                    }
                                    else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "boolean")
                                    {
                                        if (items.Value.sValue == "on")
                                        {
                                            oFIns.bValue = true;
                                        }
                                        else
                                        {
                                            oFIns.bValue = false;
                                        }

                                    }
                                    oxiibo.SetAttribute("rValue", oFIns.rValue.ToString());
                                    oxiibo.SetAttribute("bValue", oFIns.bValue.ToString());
                                    oxiibo.SetAttribute("iValue", oFIns.iValue.ToString());
                                    //if (oFIns.dValue.ToString() == "1/1/0001 12:00:00 AM")
                                    //{
                                    //    oFIns.dValue = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                                    //}

                                    oxiibo.SetAttribute("sDerivedValue", items.Value.sDerivedValue);
                                    oxiibo.SetAttribute("sValue", items.Value.sValue);
                                    oxiibo.SetAttribute("FKiQSSectionDefinitionID", sec.Value.FKiStepSectionDefinitionID.ToString());
                                    oxiibo.SetAttribute("FKiQSInstanceID", oQSIns.ID.ToString());
                                    oxiibo.SetAttribute("FKiFieldDefinitionID", items.Value.FKiFieldDefinitionID.ToString());
                                    oxiibo.SetAttribute("FKiQSStepDefinitionID", oStep.FKiQSStepDefinitionID.ToString());
                                    oxiibo.SetAttribute("FKiStepInstanceID", oStep.ID.ToString());
                                    oxiibo.SetAttribute("FKiSectionInstanceID", oSecIns.ID.ToString());
                                    oxiibo.SetAttribute("FKiFieldOriginID", FieldOrigin.ID.ToString());
                                    oxiibo.SetAttribute("FKiQSSectionDefinitionIDXIGUID", sec.Value.FKiStepSectionDefinitionIDXIGUID.ToString());
                                    oxiibo.SetAttribute("FKiQSInstanceIDXIGUID", oQSIns.XIGUID.ToString());
                                    oxiibo.SetAttribute("FKiFieldDefinitionIDXIGUID", items.Value.FKiFieldDefinitionIDXIGUID.ToString());
                                    oxiibo.SetAttribute("FKiQSStepDefinitionIDXIGUID", oStep.FKiQSStepDefinitionIDXIGUID.ToString());
                                    oxiibo.SetAttribute("FKiStepInstanceIDXIGUID", oStep.XIGUID.ToString());
                                    oxiibo.SetAttribute("FKiSectionInstanceIDXIGUID", oSecIns.XIGUID.ToString());
                                    oxiibo.SetAttribute("FKiFieldOriginIDXIGUID", FieldOrigin.XIGUID.ToString());
                                    oxiibo.SetAttribute("iStage", StepDef.iStage.ToString());
                                    oxiibo.SetAttribute("bIsDisplay", FieldOrigin.bIsDisplay.ToString());
                                    oxiibo.SetAttribute("bIsModify", FieldOrigin.bIsModify.ToString());
                                    oxiibo.SetAttribute("dValue", DateTime.Now.ToString());
                                    oxiibo.SetAttribute("FKiOrgID", oSecIns.FKiOrgID.ToString());
                                    oFIns.FKiOrgID = oSecIns.FKiOrgID;
                                    oFIns.sDerivedValue = items.Value.sDerivedValue;
                                    oFIns.sValue = items.Value.sValue;
                                    if (string.IsNullOrEmpty(oFIns.sDerivedValue))
                                    {
                                        if (FieldOrigin.DataType.sBaseDataType.ToLower() == "boolean")
                                        {
                                            oFIns.sDerivedValue = oFIns.bValue.ToString();
                                            oxiibo.SetAttribute("sDerivedValue", oFIns.bValue.ToString());
                                        }
                                        else
                                        {
                                            oFIns.sDerivedValue = items.Value.sValue;
                                            oxiibo.SetAttribute("sDerivedValue", items.Value.sValue);
                                        }
                                    }

                                    oFIns.FKiQSSectionDefinitionID = sec.Value.FKiStepSectionDefinitionID;//oSecIns.ID;
                                    oFIns.FKiQSInstanceID = oQSIns.ID;
                                    oFIns.FKiFieldDefinitionID = items.Value.FKiFieldDefinitionID;
                                    oFIns.FKiQSStepDefinitionID = oStep.FKiQSStepDefinitionID;// oStep.ID;
                                    oFIns.FKiStepInstanceID = oStep.ID;
                                    oFIns.FKiSectionInstanceID = oSecIns.ID;
                                    oFIns.FKiFieldOriginID = FieldOrigin.ID;
                                    oFIns.FKiQSSectionDefinitionIDXIGUID = sec.Value.FKiStepSectionDefinitionIDXIGUID;//oSecIns.ID;
                                    oFIns.FKiQSInstanceIDXIGUID = oQSIns.XIGUID;
                                    oFIns.FKiFieldDefinitionIDXIGUID = items.Value.FKiFieldDefinitionIDXIGUID;
                                    oFIns.FKiQSStepDefinitionIDXIGUID = oStep.FKiQSStepDefinitionIDXIGUID;// oStep.ID;
                                    oFIns.FKiStepInstanceIDXIGUID = oStep.XIGUID;
                                    oFIns.FKiSectionInstanceIDXIGUID = oSecIns.XIGUID;
                                    oFIns.FKiFieldOriginIDXIGUID = FieldOrigin.XIGUID;
                                    oFIns.iStage = StepDef.iStage;
                                    oFIns.bIsDisplay = FieldOrigin.bIsDisplay;
                                    oFIns.bIsModify = FieldOrigin.bIsModify;
                                    oFIns.dValue = DateTime.Now;
                                    //oFIns = Connection.Insert<XIIValue>(oFIns, "XIFieldInstance_T", "ID");
                                    oBulkBO.Add(oxiibo);
                                } //end for INSERT
                                items.Value.ID = oFIns.ID;
                                items.Value.FKiQSStepDefinitionID = oStep.FKiQSStepDefinitionID;
                                items.Value.FKiQSSectionDefinitionID = sec.Value.FKiStepSectionDefinitionID;
                                items.Value.XIGUID = oFIns.XIGUID;
                                items.Value.FKiQSStepDefinitionIDXIGUID = oStep.FKiQSStepDefinitionIDXIGUID;
                                items.Value.FKiQSSectionDefinitionIDXIGUID = sec.Value.FKiStepSectionDefinitionIDXIGUID;

                            } // end for foreach

                            // Execute bulk Query Here Create Data Table and Load Defination 

                            /* TO DO NEED TO UNCOMMENT THE CODE AND NEED TO TEST FOR SQL BULK COPY INSERTION*/

                        }
                        else
                        {
                        }
                    }
                    if (oBulkBO != null && oBulkBO.Count() > 0)
                    {
                        var BoD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T", null);
                        oBulkBO.ForEach(f => f.BOD = BoD);
                        var MakeDatatble = BulkInsert.MakeBulkSqlTable(oBulkBO);
                        BulkInsert.SaveBulk(MakeDatatble, BoD.iDataSourceXIGUID.ToString(), "XIFieldInstance_T");
                        foreach (var obo in oBulkBO)
                        {
                            Dictionary<string, object> fieldParams = new Dictionary<string, object>();
                            fieldParams["FKiQSInstanceIDXIGUID"] = oQSIns.XIGUID;
                            //fieldParams["FKiQSSectionDefinitionID"] = sec.Value.FKiStepSectionDefinitionID;
                            fieldParams["FKiFieldOriginIDXIGUID"] = obo.Attributes["FKiFieldOriginIDXIGUID"].sValue;
                            var oFIns = Connection.Select<XIIValue>("XIFieldInstance_T", fieldParams).FirstOrDefault();
                            if (oFIns != null)
                            {
                                var StepDef = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oStep.FKiQSStepDefinitionIDXIGUID).FirstOrDefault();
                                var SecDef = StepDef.Sections.Values.Where(m => m.XIGUID == oFIns.FKiQSSectionDefinitionIDXIGUID).FirstOrDefault();
                                var FieldOrigin = SecDef.FieldDefs.Values.Where(m => m.XIGUID == oFIns.FKiFieldDefinitionIDXIGUID).FirstOrDefault().FieldOrigin;
                                if (FieldOrigin.bIsEncrypt)
                                {
                                    XIIBO oFInsI = new XIIBO();
                                    var oFieldBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T");
                                    oFInsI.BOD = oFieldBOD;
                                    oFInsI.SetAttribute("id", oFIns.ID.ToString());
                                    oFInsI.SetAttribute("xiguid", oFIns.XIGUID.ToString());
                                    oFInsI.SetAttribute("sValue", oEncrypt.EncryptData(oFIns.sValue, true, oFIns.XIGUID.ToString()));
                                    oFInsI.SetAttribute("sDerivedValue", oFIns.sValue);
                                    oCR = oFInsI.Save(oFInsI);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (oStep.XIValues != null)
                    {
                        foreach (var items in oStep.XIValues.Values)
                        {
                            var StepDef = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oStep.FKiQSStepDefinitionIDXIGUID).FirstOrDefault();
                            var FieldOrigin = StepDef.FieldDefs.Values.Where(m => m.XIGUID == items.FKiFieldDefinitionIDXIGUID).FirstOrDefault().FieldOrigin;
                            XIIValue oFIns = new XIIValue();
                            XIIBO oFInsI = new XIIBO();
                            var oFieldBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T");
                            oFInsI.BOD = oFieldBOD;
                            if (FieldOrigin.bIsOptionList)
                            {
                                oFInsI.SetAttribute("sDerivedValue", FieldOrigin.FieldOptionList.Where(m => m.sOptionValue == items.sValue).Select(m => m.sOptionName).FirstOrDefault());
                            }
                            if (FieldOrigin.DataType.sBaseDataType.ToLower() == "int")
                            {
                                oFInsI.SetAttribute("iValue", Convert.ToInt32(items.sValue).ToString());
                            }
                            else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "datetime")
                            {
                                //if (items.sValue != null)
                                //{
                                //    oFIns.dValue = Convert.ToDateTime(items.sValue);
                                //}
                            }
                            else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "decimal")
                            {
                                oFInsI.SetAttribute("rValue", Convert.ToDecimal(items.sValue).ToString());
                            }
                            oFInsI.SetAttribute("sValue", items.sValue);
                            oFInsI.SetAttribute("KiQSStepDefinitionID", oStep.FKiQSStepDefinitionID.ToString());// oQSStepIns.ID;
                            oFInsI.SetAttribute("FKiQSInstanceID", oQSIns.ID.ToString());
                            oFInsI.SetAttribute("FKiFieldDefinitionID", items.FKiFieldDefinitionID.ToString());
                            oFInsI.SetAttribute("FKiStepInstanceID", oStep.ID.ToString());
                            oFInsI.SetAttribute("FKiQSStepDefinitionIDXIGUID", oStep.FKiQSStepDefinitionIDXIGUID.ToString());// oQSStepIns.ID;
                            oFInsI.SetAttribute("FKiQSInstanceIDXIGUID", oQSIns.XIGUID.ToString());
                            oFInsI.SetAttribute("FKiFieldDefinitionIDXIGUID", items.FKiFieldDefinitionIDXIGUID.ToString());
                            oFInsI.SetAttribute("FKiStepInstanceIDXIGUID", oStep.XIGUID.ToString());
                            oFInsI.SetAttribute("iStage", StepDef.iStage.ToString());
                            oFInsI.SetAttribute("bIsDisplay", FieldOrigin.bIsDisplay.ToString());
                            oFInsI.SetAttribute("bIsModify", FieldOrigin.bIsModify.ToString());
                            oFInsI.SetAttribute("dValue", DateTime.Now.ToString());
                            oCR = oFInsI.Save(oFInsI);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oFInsI = (XIIBO)oCR.oResult;
                                var ID = oFInsI.AttributeI("id").sValue;
                                var XIGUID = oFInsI.AttributeI("xiguid").sValue;
                                int iQSFIID = 0;
                                int.TryParse(ID, out iQSFIID);
                                Guid QSFIGUID = Guid.Empty;
                                Guid.TryParse(XIGUID, out QSFIGUID);
                                items.ID = iQSFIID;
                                items.FKiQSStepDefinitionID = oStep.FKiQSStepDefinitionID;// oStep.ID;
                                items.XIGUID = QSFIGUID;
                                items.FKiQSStepDefinitionIDXIGUID = oStep.FKiQSStepDefinitionIDXIGUID;// oStep.ID;
                            }
                            //oFIns = Connection.Insert<XIIValue>(oFIns, "XIFieldInstance_T", "ID");

                            //dbContext.XIFieldInstance.Add(oFIns);
                            //dbContext.SaveChanges();
                        }
                    }
                    else
                    {

                    }
                }
                if (oStep.FieldOriginIDs != null && oStep.FieldOriginIDs.Count() > 0)
                {
                    foreach (var FieldOriginID in oStep.FieldOriginIDs)
                    {
                        Dictionary<string, object> fieldParams = new Dictionary<string, object>();
                        fieldParams["FKiQSInstanceIDXIGUID"] = oQSIns.XIGUID;
                        fieldParams["FKiFieldOriginIDXIGUID"] = FieldOriginID;
                        var oFieldInstance = Connection.Select<XIIValue>("XIFieldInstance_T", fieldParams).FirstOrDefault();
                        XIIBO oFInsI = new XIIBO();
                        var oFieldBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T");
                        oFInsI.BOD = oFieldBOD;
                        oFInsI.SetAttribute("id", oFieldInstance.ID.ToString());
                        oFInsI.SetAttribute("xiguid", oFieldInstance.XIGUID.ToString());
                        oFInsI.SetAttribute("XIDeleted", "1");
                        oCR = oFInsI.Save(oFInsI);
                        if (oCR.bOK && oCR.oResult != null)
                        {

                        }

                        //oFieldInstance = Connection.Update<XIIValue>(oFieldInstance, "XIFieldInstance_T", "ID");
                    }
                }
            }
            return oQSInstance;
        }

        #region QSNotations
        public string GetQuestionSetParamValue(XIIQS oQSInstance, string sQSNotation)
        {
            string sReplacetext = sQSNotation.Replace("{{", "").Replace("}}", "");
            string CommText = ""; string sReturnValue = "";
            List<string[]> Rows = new List<string[]>();
            string sValue = "";
            string sSelectType = ""; int iStepID = 0;
            int FieldID = 0;
            IDictionary<string, string> DictionaryList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(sReplacetext))
            {
                if (!sReplacetext.Contains("."))
                {
                    sSelectType = Convert.ToString(oQSInstance.ID);
                    DictionaryList.Add("FKiQSInstanceID", sSelectType);
                    sReplacetext = "XIField(" + sReplacetext + ").sValue";
                }
                string[] sQSNotations = sReplacetext.Split('.');
                if (sQSNotations != null)
                {
                    foreach (var str in sQSNotations)
                    {
                        string[] sType = str.Split('(');
                        if (sType.Count() == 2)
                        {
                            if (sType[0] == "QS")
                            {
                                sType[1] = sType[1].TrimEnd(')');
                                if (sType[1] == "iMyQSID")
                                {
                                    sSelectType = Convert.ToString(oQSInstance.ID);
                                    DictionaryList.Add("FKiQSInstanceID", sSelectType);
                                }
                            }
                            if (sType[0] == "StepName")
                            {
                                sType[1] = sType[1].TrimEnd(')');
                                sType[1] = sType[1].Trim('\'');
                                iStepID = oQSInstance.QSDefinition.Steps.Where(x => x.Value.sName == sType[1]).Select(x => x.Value.ID).FirstOrDefault();
                                sSelectType = Convert.ToString(iStepID);
                                DictionaryList.Add("FKiQSStepDefinitionID", sSelectType);
                            }
                            if (sType[0] == "XIField")
                            {
                                sType[1] = sType[1].TrimEnd(')');
                                sType[1] = sType[1].Trim('\'');
                                foreach (var sec in oQSInstance.QSDefinition.Steps)
                                {
                                    FieldID = sec.Value.FieldDefs.Where(m => m.Value.FieldOrigin.sName == sType[1]).Select(m => m.Value.ID).FirstOrDefault();
                                    if (FieldID > 0)
                                    {
                                        sSelectType = Convert.ToString(FieldID);
                                        DictionaryList.Add("FKiFieldDefinitionID", sSelectType);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            sSelectType = sType[0];
                            DictionaryList.Add("sSelectValue", sSelectType);
                            CommText = "select " + sType[0] + " from XIFieldInstance_T where 1=1";
                        }

                    }
                    string s = sSelectType;
                }
            }

            if (DictionaryList.ContainsKey("FKiFieldDefinitionID"))
            {
                sValue = GetQuestionSetParamWithValues(oQSInstance, DictionaryList);
                //Get Field Options list Values  if field have Options
                string OptionsValue = FieldHasOptions(FieldID, sValue);
                if (!string.IsNullOrEmpty(OptionsValue))
                {
                    sValue = OptionsValue;
                }
            }
            return sValue;
        }

        public string GetQuestionSetParamWithValues(XIIQS oQSInstance, IDictionary<string, string> DictionaryList)
        {
            XIDXI oDXI = new XIDXI();

            oDXI.sOrgDatabase = sOrgDatabase;
            string str = ""; string sSelectString = ""; string sReturnValue = ""; string sReturnType = "";
            var sBODataSource = string.Empty;
            foreach (var item in DictionaryList)
            {
                if (item.Key != "sSelectValue")
                {
                    str += " and " + item.Key + "=" + item.Value + "";
                }
                else
                {
                    sSelectString = "select CONVERT(varchar," + item.Value + "),SQL_VARIANT_PROPERTY(" + item.Value + ",'BaseType') AS 'Base Type' from XIFieldInstance_T where 1=1";
                }

            }
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "";
                cmd.Connection = Con;
                Con.Open();
                cmd.CommandText = str;
                cmd.CommandText = cmd.CommandText.Insert(0, sSelectString);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //sReturnValue = reader.GetString(0);
                    //sReturnType = reader.GetString(1);
                    sReturnValue = reader.IsDBNull(0) ? "" : reader.GetValue(0).ToString();
                    sReturnType = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString();
                }
                Con.Dispose();
                if (sReturnType == "datetime")
                {
                    DateTime dtvalue = Convert.ToDateTime(sReturnValue);
                    sReturnValue = dtvalue.ToString("MM/dd/yyyy");
                }
                return sReturnValue;
            }
        }
        public string FieldHasOptions(int FieldID, string sValue)
        {
            string Value = string.Empty;
            string Query = "SELECT  sOptionName FROM XIFieldOptionList_T WHERE FKiQSFieldID = (SELECT FKiXIFieldOriginID  FROM XIFieldDefinition_T WHERE id =" + FieldID + " ) AND sOptionValue ='" + sValue + "'";
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "";
                cmd.Connection = Con;
                Con.Open();
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Value = reader["sOptionName"].ToString();
                }
                Con.Dispose();
            }
            return Value;
        }
        #endregion
        //public XIIQS QSI(string sInstanceID, XIDStructure oStructure)
        //{


        //    int iInstanceID = 0; XIIQS oQsInstance = new XIIQS();
        //    XIDStructure oStruture = new XIDStructure();
        //    if (!string.IsNullOrEmpty(sInstanceID))
        //    {
        //        iInstanceID = Convert.ToInt32(sInstanceID);
        //        if (iInstanceID > 0)
        //        {
        //            XIIXI oXII = new XIIXI();
        //            var oQSD = oXII.GetQSInstanceByID(iInstanceID);
        //            oQsInstance = oXII.GetQuestionSetInstanceByID(oQSD.FKiQSDefinitionID, iInstanceID, null, 0, 0, null);
        //            if (oQsInstance != null)
        //            {
        //                Dictionary<string, XIIValue> oXIIValues = (Dictionary<string, XIIValue>)oStruture.GetQSNamevaluepairs(oQsInstance);
        //                oQsInstance.XIValues = oXIIValues;
        //            }

        //        }
        //    }
        //    if (oStructure != null)
        //    {
        //        var oStructureInstance = (Dictionary<string, List<XIIBO>>)oStructure.oParent;
        //        oQsInstance.oStructureInstance = oStructureInstance;

        //    }
        //    return oQsInstance;
        //}
        public XIIQS QSI(XIBOInstance oStructure)
        {
            XIIQS oQsInstance = new XIIQS();
            try
            {
                string sInstanceID = oStructure.BOI.AttributeI("id").sValue;
                int iInstanceID = 0;
                XIDStructure oStruture = new XIDStructure();
                if (!string.IsNullOrEmpty(sInstanceID))
                {
                    iInstanceID = Convert.ToInt32(sInstanceID);
                    if (iInstanceID > 0)
                    {
                        XIIXI oXII = new XIIXI();
                        var oQSD = oXII.GetQSInstanceByID(iInstanceID.ToString());
                        if (oQSD != null)
                        {
                            oQsInstance = oXII.GetQuestionSetInstanceByID(oQSD.FKiQSDefinitionIDXIGUID.ToString(), iInstanceID.ToString(), null, 0, 0, null);
                        }
                        if (oQsInstance != null)
                        {
                            var oQSNVPairs = oStruture.GetQSNamevaluepairs(oQsInstance);
                            if (oQSNVPairs.xiStatus == 0 && oQSNVPairs.oResult != null)
                            {
                                Dictionary<string, XIIValue> oXIIValues = (Dictionary<string, XIIValue>)oQSNVPairs.oResult;
                                oQsInstance.XIValues = oXIIValues;
                            }
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                    }
                }
                if (oStructure != null)
                {
                    var oStructureInstance = (Dictionary<string, List<XIIBO>>)oStructure.oParent;
                    oQsInstance.oStructureInstance = oStructureInstance;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return oQsInstance;
        }
        public string XIIValues(string sName)
        {
            string sValueType = string.Empty;
            if (sName.Contains('^'))
            {
                sName = sName.Split('^')[1];
                sValueType = "dv";
            }
            var oQsInstance = this; string sReturnValue = "";
            //XIDStructure oStruture = new XIDStructure();
            if (oQsInstance != null)
            {
                Dictionary<string, XIIValue> oXIIValues = new Dictionary<string, XIIValue>(StringComparer.CurrentCultureIgnoreCase);
                oXIIValues = this.XIValues;
                if (!string.IsNullOrEmpty(sValueType) && sValueType.ToLower() == "dv")
                {
                    sReturnValue = oXIIValues[sName].sDerivedValue;
                }
                else if (oXIIValues.ContainsKey(sName))
                {
                    sReturnValue = oXIIValues[sName].sValue;
                }
            }
            return sReturnValue;
        }
        public XIIValue GetXIIValue(string sName)
        {
            XIIValue xivalue = new XIIValue();
            var oQsInstance = this;
            if (oQsInstance != null)
            {
                Dictionary<string, XIIValue> oXIIValues = new Dictionary<string, XIIValue>(StringComparer.CurrentCultureIgnoreCase);
                oXIIValues = this.XIValues;
                if (oXIIValues.ContainsKey(sName))
                {
                    xivalue = oXIIValues[sName];
                }
            }
            return xivalue;
        }
        //public Dictionary<string, string> GetNotationValue(Dictionary<string, XIIValue> oXIValues, Dictionary<string, List<XIIBO>> oSubStructuresList, Dictionary<string, string> sNotationList)
        //{
        //    string sReturnValue = "";
        //    Dictionary<string, string> sNotationsResults = sNotationList;
        //    XIIQS oXIQS = new XIIQS();
        //    // Dictionary<string, XIIValue> oXIIValues = (Dictionary<string, XIIValue>)XILoad;
        //    foreach (var sNotation in sNotationList.ToList())
        //    {
        //        if (string.IsNullOrEmpty(sNotation.Value))
        //        {
        //            string sString = sNotation.Key.Replace("{{", "").Replace("}}", "").Replace("\"", "");
        //            if (sString.Contains(","))
        //            {
        //                string sKey = sString.Split(',')[0];
        //                string sAttributeName = sString.Split(',')[1];
        //                if (sKey == "QS Instance")
        //                {
        //                    if (oXIValues.ContainsKey(sAttributeName))
        //                    {
        //                        sReturnValue = oXIValues[sAttributeName].sValue;
        //                        string OptionsValue = oXIQS.FieldHasOptions(oXIValues[sAttributeName].FKiFieldDefinitionID, sReturnValue);
        //                        if (!string.IsNullOrEmpty(OptionsValue))
        //                        {
        //                            sReturnValue = OptionsValue;
        //                        }
        //                        sNotationsResults[sNotation.Key] = sReturnValue;
        //                    }
        //                    else
        //                    {
        //                        if (sAttributeName.Contains("Collection"))
        //                        {
        //                            string sKeyname = sAttributeName.Split(' ')[0];
        //                            var oSubstructureInstance = oSubStructuresList.Values.FirstOrDefault().Select(x => x.SubChildI).FirstOrDefault();
        //                            if (oSubstructureInstance.ContainsKey(sKeyname))
        //                            {
        //                                sReturnValue = ReplaceHtmlContentWithData(null, null, oSubstructureInstance[sKeyname]);
        //                                sNotationsResults[sNotation.Key] = sReturnValue;
        //                            }
        //                            else
        //                            {
        //                                XIIQS oXIIQS = new XIIQS();
        //                                oXIIQS.XIValues = oXIValues;
        //                                oXIIQS.oStructureInstance = oSubstructureInstance;
        //                                GetNotationValue(oXIValues, oSubStructuresList, sNotationList);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (sAttributeName.Contains("Collection"))
        //                    {
        //                        string sKeyname = sAttributeName.Split(' ')[0];
        //                        var oSubstructureInstance = oXIQS.oStructureInstance.Values.FirstOrDefault().Select(x => x.SubChildI).FirstOrDefault();
        //                        if (oSubstructureInstance.ContainsKey(sKeyname))
        //                        {
        //                            sReturnValue = ReplaceHtmlContentWithData(null, null, oSubstructureInstance[sKeyname]);
        //                            sNotationsResults[sNotation.Key] = sReturnValue;
        //                        }
        //                        else
        //                        {
        //                            XIIQS oXIIQS = new XIIQS();
        //                            oXIIQS.XIValues = oXIValues;
        //                            oXIIQS.oStructureInstance = oSubstructureInstance;
        //                            GetNotationValue(oXIValues, oSubStructuresList, sNotationList);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (oXIValues.ContainsKey(sAttributeName))
        //                        {
        //                            sReturnValue = oXIValues[sAttributeName].sValue;
        //                            string OptionsValue = oXIQS.FieldHasOptions(oXIValues[sAttributeName].FKiFieldDefinitionID, sReturnValue);
        //                            if (!string.IsNullOrEmpty(OptionsValue))
        //                            {
        //                                sReturnValue = OptionsValue;
        //                            }
        //                            sNotationsResults[sNotation.Key] = sReturnValue;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    //foreach (var sNotation in sNotationList.ToList())
        //    //{
        //    //    if (string.IsNullOrEmpty(sNotation.Value))
        //    //    {
        //    //        string sString = sNotation.Key.Replace("{{", "").Replace("}}", "");
        //    //        if (sString.Contains(","))
        //    //        {
        //    //            string sKey = sString.Split(',')[0];
        //    //            string sAttributeName = sString.Split(',')[1];
        //    //            //if(sAttributeName.Contains("("))
        //    //            //{ }
        //    //            if (XILoad.ContainsKey(sKey))
        //    //            {
        //    //                var oAttributes = XILoad.Values.FirstOrDefault().Select(x => x.Attributes).FirstOrDefault();
        //    //                if (oAttributes.ContainsKey(sAttributeName))
        //    //                {
        //    //                    sReturnValue = oAttributes.Where(x => x.Key.ToLower() == sAttributeName.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
        //    //                    sNotationsResults[sNotation.Key] = sReturnValue;
        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                if(XILoad.Count()>0)
        //    //                {
        //    //                    Dictionary<string, List<XIIBO>> sSubstructureInstance = XILoad.Values.FirstOrDefault().Select(x => x.Class).FirstOrDefault();
        //    //                    if (sSubstructureInstance.ContainsKey(sKey))
        //    //                    {
        //    //                        var oAttributes = sSubstructureInstance.Values.FirstOrDefault().Select(x => x.Attributes).FirstOrDefault();
        //    //                        if (oAttributes.ContainsKey(sAttributeName))
        //    //                        {
        //    //                            sReturnValue = oAttributes.Where(x => x.Key.ToLower() == sAttributeName.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
        //    //                            sNotationsResults[sNotation.Key] = sReturnValue;
        //    //                        }
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        NotationValue(sSubstructureInstance, sNotationsResults);
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    return sNotationsResults;
        //}
        //public string ReplaceHtmlContentWithData(string sHtmlContent, string sReplaceString, List<XIIBO> oResult)
        //{
        //        //here we get html content string with data
        //        int i = 0;
        //        string sFinalHtmlContent = "<table style='width:100%;' cellpadding='4' cellspacing='0' border='1'>";
        //        foreach (var item in oResult)
        //        {
        //            if (i == 0)
        //            {
        //                sFinalHtmlContent += "<tr>";
        //                // sFinalHtmlContent += "<thead>";
        //                foreach (var attribute in item.Attributes)
        //                {

        //                    sFinalHtmlContent += "<th style='background-color:#e7e7e7;'>" + attribute.Key + "</th>";
        //                }
        //                sFinalHtmlContent += "</tr>";
        //            }

        //            sFinalHtmlContent += "<tr>";
        //            foreach (var attribute in item.Attributes)
        //            {
        //                if (string.IsNullOrEmpty(attribute.Value.sValue))
        //                {
        //                    sFinalHtmlContent += "<td style='background-color:#c0c0c0;'>" + "-" + "</td>";
        //                }
        //                else
        //                {
        //                    if (attribute.Key == "DOB")
        //                    {
        //                        sFinalHtmlContent += "<td style='background-color:#c0c0c0;'>" + Convert.ToDateTime(attribute.Value.sValue).Date.ToString("dd-MMM-yyyy") + "</td>";
        //                    }
        //                    else
        //                    {
        //                        sFinalHtmlContent += "<td style='background-color:#c0c0c0;'>" + attribute.Value.sValue + "</td>";
        //                    }
        //                }
        //            }
        //            sFinalHtmlContent += "</tr>";
        //            i++;
        //        }
        //        sFinalHtmlContent += "</table>";
        //        //sHtmlContent = sHtmlContent.Replace(sReplaceString, sFinalHtmlContent);
        //        return sFinalHtmlContent;
        //    }
        public XIIQS LoadStepInstance(XIIQS oQSInstance, string StepID = "", string sGUID = "", int iOrgID=0)
        {
            int iStepID = 0;
            Guid StepXIGUID = Guid.Empty;
            int.TryParse(StepID, out iStepID);
            Guid.TryParse(StepID, out StepXIGUID);
            XIInfraCache xiCache = new XIInfraCache();
            var Step = new XIDQSStep();

            var sSessionID = string.Empty;
            if (HttpContext.Current == null)
            {

            }
            else
            {
                sSessionID = HttpContext.Current.Session.SessionID;
            }
            List<Guid> FieldOriginIDs = new List<Guid>();
            XIDQS oQSD = (XIDQS)xiCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, oQSInstance.QSDefinition.XIGUID.ToString(), null, null, 0, iOrgID);
            XIDQS oQSDC = (XIDQS)oQSD.GetCopy();
            oQSInstance.QSDefinition = oQSDC;
            if (oQSInstance.QSDefinition.Steps.Count() > 0)
            {
                XIIQSStep oStepInstance = new XIIQSStep();
                if (StepXIGUID != null && StepXIGUID != Guid.Empty)
                {
                    Step = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == StepXIGUID).OrderBy(m => m.iOrder).FirstOrDefault();
                }
                else if (iStepID > 0)
                {
                    Step = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == iStepID).OrderBy(m => m.iOrder).FirstOrDefault();
                }
                else
                {
                    Step = oQSInstance.QSDefinition.Steps.Values.Where(m => m.sIsHidden == "off" || m.sIsHidden == null).OrderBy(m => m.iOrder).FirstOrDefault();
                }
                if (oQSInstance.Steps != null)
                {
                    oStepInstance = oQSInstance.Steps.Where(m => m.Value.FKiQSStepDefinitionIDXIGUID == Step.XIGUID).FirstOrDefault().Value;
                }
                if (oStepInstance == null)
                {
                    oStepInstance = new XIIQSStep();
                }
                oStepInstance.iOverrideStepXIGUID = Step.iOverrideStepXIGUID;
                //Loading Fields for Step
                Dictionary<string, XIIValue> nFieldValues = new Dictionary<string, XIIValue>();
                if (((xiSectionContent)Step.iDisplayAs).ToString() == xiSectionContent.Fields.ToString())
                {
                    if (Step.FieldDefs != null)
                    {
                        var Defs = Step.FieldDefs.ToList();//.Select(m => new XIIValue { FKiFieldDefinitionID = m.ID }).ToList();
                        foreach (var def in Defs)
                        {
                            nFieldValues[def.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.Value.ID, FKiFieldDefinitionIDXIGUID = def.Value.XIGUID };
                        }
                        oStepInstance.XIValues = nFieldValues;
                    }
                }
                //Load Step Layout Content
                else if (Step.iLayoutID > 0 || (Step.iLayoutIDXIGUID != null && Step.iLayoutIDXIGUID != Guid.Empty))
                {
                    XIDLayout oLayoutD = new XIDLayout();
                    oLayoutD.ID = Step.iLayoutID;
                    oLayoutD.XIGUID = Step.iLayoutIDXIGUID;
                    oStepInstance.oDefintion = Step;
                    var oStepContent = oStepInstance.Load();
                    if (oStepContent.bOK && oStepContent.oResult != null)
                    {
                        var Instance = ((XIIQSStep)(((XIInstanceBase)oStepContent.oResult).oContent[XIConstant.ContentStep])).oContent[XIConstant.ContentLayout];
                        oStepInstance.oContent[XIConstant.ContentLayout] = Instance;//((XIDQSStep)((XIInstanceBase)oStepContent.oResult)).oContent[XIConstant.ContentStep];

                    }
                }
                //Loading Sections for Step
                else if (((xiSectionContent)Step.iDisplayAs).ToString() == xiSectionContent.Sections.ToString())
                {
                    Dictionary<string, XIIQSSection> nSecIns = new Dictionary<string, XIIQSSection>();
                    if (Step.Sections != null && Step.Sections.Count() > 0)
                    {
                        foreach (var sec in Step.Sections.Values.OrderBy(m => m.iOrder))
                        {
                            XIIQSSection oSecIns = null;
                            if (oStepInstance.Sections != null && oStepInstance.Sections.Count() > 0)
                            {
                                oSecIns = oStepInstance.Sections[sec.XIGUID.ToString() + "_Sec"];
                                //oSecIns = oStepInstance.Sections[sec.ID.ToString() + "_Sec"];
                            }
                            if (oSecIns == null)
                            {
                                oSecIns = new XIIQSSection();
                                oSecIns.FKiStepSectionDefinitionID = sec.ID;
                                oSecIns.FKiStepSectionDefinitionIDXIGUID = sec.XIGUID;
                                if (((xiSectionContent)sec.iDisplayAs).ToString() == xiSectionContent.Fields.ToString())
                                {
                                    Dictionary<string, XIIValue> nSecFieldValues = new Dictionary<string, XIIValue>();
                                    if (sec.FieldDefs != null && sec.FieldDefs.Count() > 0)
                                    {
                                        var Def = sec.FieldDefs.Values.OrderBy(m => m.ID).ToList();//.Select(m => new XIIValue { FKiFieldDefinitionID = m.ID }).ToList();
                                        foreach (var def in Def)
                                        {
                                            if ((def.FieldOrigin.bIsMerge && !String.IsNullOrEmpty(def.FieldOrigin.sMergeField) && (!oQSInstance.XIValues.ContainsKey(def.FieldOrigin.sName) || (oQSInstance.XIValues.ContainsKey(def.FieldOrigin.sName) && def.FieldOrigin.bIsReload))))
                                            {
                                                if (def.FieldOrigin.sMergeField.StartsWith("{XIP"))
                                                {
                                                    XIInfraCache oCache = new XIInfraCache();
                                                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                                                    nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = oGUIDParams.NMyInstance.Where(x => x.Key == def.FieldOrigin.sMergeField).Select(t => t.Value.sValue).FirstOrDefault(), FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                }
                                                else
                                                {
                                                    nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = oQSInstance.XIIValues(def.FieldOrigin.sMergeField), FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                }
                                            }
                                            else if (def.FieldOrigin.bIsMerge && !String.IsNullOrEmpty(def.FieldOrigin.sMergeBo) && !String.IsNullOrEmpty(def.FieldOrigin.sMergeBoField) && !string.IsNullOrEmpty(def.FieldOrigin.sMergeVariable) && (!oQSInstance.XIValues.ContainsKey(def.FieldOrigin.sName) || (oQSInstance.XIValues.ContainsKey(def.FieldOrigin.sName) && def.FieldOrigin.bIsReload)))
                                            {
                                                XIInfraCache oCache = new XIInfraCache();
                                                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, def.FieldOrigin.sMergeBo, null);
                                                string sMergeCacheField = def.FieldOrigin.sMergeVariable;
                                                var InstanceID = (string)oCache.Get_ParamVal(sSessionID, sGUID, null, sMergeCacheField);
                                                int iInstanceiD = 0;
                                                if (int.TryParse(InstanceID, out iInstanceiD))
                                                {
                                                    XIIXI oXIIXI = new XIIXI();
                                                    if (iInstanceiD != 0)
                                                    {
                                                        var oBOI = oXIIXI.BOI(oBOD.Name, iInstanceiD.ToString());
                                                        if (oBOI.Attributes.ContainsKey(def.FieldOrigin.sMergeBoField))
                                                        {
                                                            if (oBOD.Name == "EnumReconciliations_T")
                                                            {
                                                                if (oBOI.Attributes.ContainsKey(def.FieldOrigin.sMergeBoField))
                                                                {
                                                                    nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = oBOI.Attributes[def.FieldOrigin.sMergeBoField].sValue + " - " + DateTime.Now.ToString(XIConstant.DateTimeFull_Format), FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var sBOValue = oBOI.Attributes[def.FieldOrigin.sMergeBoField].sValue;
                                                                if (def.FieldOrigin.DataType != null && def.FieldOrigin.DataType.sBaseDataType.ToLower() == "datetime")
                                                                {
                                                                    sBOValue = Utility.GetDefaultDateResolvedValue(sBOValue, XIConstant.Date_Format);
                                                                }
                                                                nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = sBOValue, FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                            }
                                                        }
                                                        else
                                                        {
                                                            List<XIWhereParams> oWParams = new List<XIWhereParams>();
                                                            string[] operators = new string[] { "+", "-", "/", "%", "(", ")" };
                                                            List<string[]> lstItems = def.FieldOrigin.sMergeBoField.Split(',').Select(f => new { Field = f }).Select(c => c.Field.Trim().Split(operators, StringSplitOptions.RemoveEmptyEntries)).ToList();
                                                            for (int i = 0; i < lstItems.Count; i++)
                                                            {
                                                                if (lstItems[i].Count() > 1)
                                                                {
                                                                    Regex pattern = new Regex(Regex.Escape(lstItems[i][0]) + "(.+?)" + Regex.Escape(lstItems[i][1]));
                                                                    var getoperators = pattern.Matches(def.FieldOrigin.sMergeBoField).Cast<Match>().Select(s => s.Groups[1].Value).Distinct().ToList();
                                                                    if (getoperators.Count() > 0)
                                                                    {
                                                                        nSecFieldValues[def.FieldOrigin.sName] = new XIIValue
                                                                        {
                                                                            FKiFieldDefinitionID = def.ID,
                                                                            FKiFieldDefinitionIDXIGUID = def.XIGUID,
                                                                            sValue = (oBOI.Attributes[lstItems[i][0]].doValue - oBOI.Attributes[lstItems[i][1]].doValue).ToString(),
                                                                            FKiFieldOriginID = def.FKiXIFieldOriginID,
                                                                            FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID,
                                                                            bIsDisplay = def.FieldOrigin.bIsDisplay
                                                                        };
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = "", FKiFieldOriginID = def.FKiXIFieldOriginID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = "", FKiFieldOriginID = def.FKiXIFieldOriginID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                    }
                                                }
                                                else
                                                {
                                                    nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = "", FKiFieldOriginID = def.FKiXIFieldOriginID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                }
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(def.FieldOrigin.sDefaultDate))
                                                {
                                                    if (def.FieldOrigin.sDefaultDate.Contains("xi.s"))
                                                    {
                                                        XIDScript oXIScript = new XIDScript();
                                                        oXIScript.sScript = def.FieldOrigin.sDefaultDate.ToString();
                                                        var oCResult = oXIScript.Execute_Script(sGUID, sSessionID);
                                                        if (oCResult.bOK && oCResult.oResult != null)
                                                        {
                                                            def.FieldOrigin.sDefaultDate = (string)oCResult.oResult;
                                                        }
                                                        nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = Utility.GetDefaultDateResolvedValue(def.FieldOrigin.sDefaultDate, XIConstant.Date_Format), FKiFieldOriginID = def.FKiXIFieldOriginID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                    }
                                                    else
                                                    {
                                                        if (oQSInstance.XIValues.ContainsKey(def.FieldOrigin.sName) && !string.IsNullOrEmpty(oQSInstance.XIValues[def.FieldOrigin.sName].sValue))
                                                        {
                                                            nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = oQSInstance.XIValues[def.FieldOrigin.sName].sValue, FKiFieldOriginID = def.FKiXIFieldOriginID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                        }
                                                        else
                                                        {
                                                            nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = Utility.GetDefaultDateResolvedValue(def.FieldOrigin.sDefaultDate, XIConstant.Date_Format), FKiFieldOriginID = def.FKiXIFieldOriginID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (oQSInstance.XIValues.ContainsKey(def.FieldOrigin.sName.ToString()) && Step.bIsMerge && !def.FieldOrigin.bIsReload)
                                                    {
                                                        if (def.FKiXIFieldOriginIDXIGUID != oQSInstance.XIValues[def.FieldOrigin.sName.ToString()].FKiFieldOriginIDXIGUID)
                                                        {
                                                            FieldOriginIDs.Add(oQSInstance.XIValues[def.FieldOrigin.sName.ToString()].FKiFieldOriginIDXIGUID);
                                                        }
                                                        nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, sValue = oQSInstance.XIValues[def.FieldOrigin.sName.ToString()].sValue, FKiFieldOriginID = def.FKiXIFieldOriginID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                    }
                                                    else
                                                    {
                                                        nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                    }
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(def.FieldOrigin.sMinDate) && (def.FieldOrigin.sMinDate.Contains("xi.s")))
                                            {
                                                XIDScript oXIScript = new XIDScript();
                                                oXIScript.sScript = def.FieldOrigin.sMinDate.ToString();
                                                var oCResult = oXIScript.Execute_Script(sGUID, sSessionID);
                                                if (oCResult.bOK && oCResult.oResult != null)
                                                {
                                                    var sVal = (string)oCResult.oResult;
                                                    sVal = Utility.GetDateResolvedValue(sVal, "yyyy-MM-dd");
                                                    def.FieldOrigin.sMinDate = sVal;
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(def.FieldOrigin.sMaxDate) && (def.FieldOrigin.sMaxDate.Contains("xi.s")))
                                            {
                                                XIDScript oXIScript = new XIDScript();
                                                oXIScript.sScript = def.FieldOrigin.sMaxDate.ToString();
                                                var oCResult = oXIScript.Execute_Script(sGUID, sSessionID);
                                                if (oCResult.bOK && oCResult.oResult != null)
                                                {
                                                    var sVal = (string)oCResult.oResult;
                                                    def.FieldOrigin.sMaxDate = Utility.GetDateResolvedValue(sVal, "yyyy-MM-dd");
                                                    CResult oSResult = new CResult();
                                                    oSResult.sMessage = def.FieldOrigin.sName + "_" + def.FieldOrigin.XIGUID + " : Script Value:" + sVal + " & Resolved Value:" + def.FieldOrigin.sMaxDate;
                                                    SaveErrortoDB(oSResult, oQSInstance.ID);
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(def.FieldOrigin.sScript))
                                            {
                                                string sVal = string.Empty;
                                                if (nSecFieldValues.ContainsKey(def.FieldOrigin.sName))
                                                {
                                                    sVal = nSecFieldValues[def.FieldOrigin.sName].sValue;
                                                }
                                                XIDScript oScriptD = new XIDScript();
                                                oScriptD.sScript = def.FieldOrigin.sScript;
                                                if (oScriptD != null)
                                                {
                                                    var oRes = oScriptD.Execute_Script(sGUID, sSessionID);
                                                    if (oRes.bOK && oRes.oResult != null)
                                                    {
                                                        string result = (string)oRes.oResult;
                                                        if (def.FieldOrigin.iScriptType == 50)
                                                        {
                                                            if (!string.IsNullOrEmpty(result) && result.ToLower() == "true")
                                                            {
                                                                def.FieldOrigin.bIsDisable = true;
                                                            }
                                                            else
                                                            {
                                                                def.FieldOrigin.bIsDisable = false;
                                                                nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, sValue = sVal, FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                            }
                                                        }
                                                        else if (def.FieldOrigin.iScriptType == 40)
                                                        {
                                                            if (!string.IsNullOrEmpty(result) && result.ToLower() == "true")
                                                            {
                                                                def.FieldOrigin.bIsHidden = true;
                                                                def.FieldOrigin.sIsHidden = "on";
                                                                def.FieldOrigin.bIsMandatory = false;
                                                            }
                                                            else
                                                            {
                                                                def.FieldOrigin.bIsHidden = false;
                                                                def.FieldOrigin.sIsHidden = "off";
                                                                nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, sValue = sVal, FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                            }
                                                        }
                                                        if (!string.IsNullOrEmpty(def.FieldOrigin.sFieldDefaultValue))
                                                        {
                                                            nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = def.ID, FKiFieldDefinitionIDXIGUID = def.XIGUID, FKiFieldOriginIDXIGUID = def.FKiXIFieldOriginIDXIGUID, sValue = def.FieldOrigin.sFieldDefaultValue, FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                                        }
                                                    }
                                                }
                                            }
                                            if (!oQSInstance.XIValues.ContainsKey(def.FieldOrigin.sName))
                                            oQSInstance.XIValues[def.FieldOrigin.sName] = nSecFieldValues[def.FieldOrigin.sName];
                                            if ((def.FieldOrigin.FK1ClickID > 0 || (def.FieldOrigin.FK1ClickIDXIGUID != null && def.FieldOrigin.FK1ClickIDXIGUID != Guid.Empty)) && (def.FieldOrigin.FKiBOID > 0 || (def.FieldOrigin.FKiBOIDXIGUID != null && def.FieldOrigin.FKiBOIDXIGUID != Guid.Empty)))
                                            {
                                                var BOID = def.FieldOrigin.FKiBOID;
                                                Dictionary<string, object> Params = new Dictionary<string, object>();
                                                if (def.FieldOrigin.FKiBOIDXIGUID != null && def.FieldOrigin.FKiBOIDXIGUID != Guid.Empty)
                                                {
                                                    Params["XIGUID"] = def.FieldOrigin.FKiBOIDXIGUID;
                                                }
                                                else if (def.FieldOrigin.FKiBOID > 0)
                                                {
                                                    Params["BOID"] = BOID;
                                                }
                                                string sSelectFields = string.Empty;
                                                sSelectFields = "Name,BOID,iDataSource,sSize,TableName,sPrimaryKey,sType";
                                                var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", Params, sSelectFields).FirstOrDefault();
                                                def.FieldOrigin.sBOSize = FKBOD.sSize;
                                            }
                                        }
                                    }
                                    oSecIns.XIValues = nSecFieldValues;
                                    oStepInstance.FieldOriginIDs = FieldOriginIDs;
                                }
                            }
                            else
                            {
                                oSecIns.FKiStepSectionDefinitionID = sec.ID;
                                if (((xiSectionContent)sec.iDisplayAs).ToString() == xiSectionContent.Fields.ToString())
                                {
                                    var Oldfields = oSecIns.XIValues;
                                    if (Oldfields == null)
                                    {
                                        Oldfields = new Dictionary<string, XIIValue>();
                                    }
                                    Dictionary<string, XIIValue> nSecFieldValues = new Dictionary<string, XIIValue>();
                                    var Def = sec.FieldDefs.OrderBy(m => m.Value.ID).ToList();
                                    foreach (var items in Def)
                                    {
                                        if (Oldfields.Count() == 0 || Oldfields[items.Value.FieldOrigin.sName] == null || items.Value.FieldOrigin.bIsReload)
                                        {
                                            nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = items.Value.ID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                        }
                                        else
                                        {
                                            nSecFieldValues[items.Value.FieldOrigin.sName] = Oldfields[items.Value.FieldOrigin.sName];
                                        }
                                        if ((items.Value.FieldOrigin.bIsMerge && !String.IsNullOrEmpty(items.Value.FieldOrigin.sMergeField) && (Oldfields.Count() == 0 || (Oldfields.Count() > 0 && items.Value.FieldOrigin.bIsReload))))
                                        {
                                            nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = oQSInstance.XIIValues(items.Value.FieldOrigin.sMergeField), FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                        }
                                        if (!string.IsNullOrEmpty(items.Value.FieldOrigin.sScript))
                                        {
                                            string sVal = string.Empty;
                                            if (nSecFieldValues.ContainsKey(items.Value.FieldOrigin.sName))
                                            {
                                                sVal = nSecFieldValues[items.Value.FieldOrigin.sName].sValue;
                                            }
                                            XIDScript oScriptD = new XIDScript();
                                            oScriptD.sScript = items.Value.FieldOrigin.sScript;
                                            if (oScriptD != null)
                                            {
                                                var oRes = oScriptD.Execute_Script(sGUID, sSessionID);
                                                if (oRes.bOK && oRes.oResult != null)
                                                {
                                                    string result = (string)oRes.oResult;
                                                    if (items.Value.FieldOrigin.iScriptType == 50)
                                                    {
                                                        if (!string.IsNullOrEmpty(result) && result.ToLower() == "true")
                                                        {
                                                            items.Value.FieldOrigin.bIsDisable = true;
                                                        }
                                                        else
                                                        {
                                                            items.Value.FieldOrigin.bIsDisable = false;
                                                            nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = sVal, FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                                        }
                                                    }
                                                    else if (items.Value.FieldOrigin.iScriptType == 40)
                                                    {
                                                        if (!string.IsNullOrEmpty(result) && result.ToLower() == "true")
                                                        {
                                                            items.Value.FieldOrigin.bIsHidden = true;
                                                            items.Value.FieldOrigin.sIsHidden = "on";
                                                            items.Value.FieldOrigin.bIsMandatory = false;
                                                        }
                                                        else
                                                        {
                                                            items.Value.FieldOrigin.bIsHidden = false;
                                                            items.Value.FieldOrigin.sIsHidden = "off";
                                                            nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = sVal, FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(items.Value.FieldOrigin.sFieldDefaultValue))
                                                    {
                                                        nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = items.Value.FieldOrigin.sFieldDefaultValue, FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                                    }
                                                    else if ((items.Value.FieldOrigin.bIsMerge && !String.IsNullOrEmpty(items.Value.FieldOrigin.sMergeBo) && !String.IsNullOrEmpty(items.Value.FieldOrigin.sMergeBoField) && !string.IsNullOrEmpty(items.Value.FieldOrigin.sMergeVariable) && (Oldfields.Count() == 0 || (Oldfields.Count() > 0 && items.Value.FieldOrigin.bIsReload))))
                                                    {
                                                        XIInfraCache oCache = new XIInfraCache();
                                                        XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, items.Value.FieldOrigin.sMergeBo, null);
                                                        string sMergeCacheField = items.Value.FieldOrigin.sMergeVariable;
                                                        var InstanceID = (string)oCache.Get_ParamVal(sSessionID, sGUID, null, sMergeCacheField);
                                                        int iInstanceiD = 0;
                                                        if (int.TryParse(InstanceID, out iInstanceiD))
                                                        {
                                                            XIIXI oXIIXI = new XIIXI();
                                                            if (iInstanceiD != 0)
                                                            {
                                                                var oBOI = oXIIXI.BOI(oBOD.Name, iInstanceiD.ToString());
                                                                if (oBOI.Attributes.ContainsKey(items.Value.FieldOrigin.sMergeBoField))
                                                                {
                                                                    if (oBOD.Name == "EnumReconciliations_T")
                                                                    {
                                                                        if (oBOI.Attributes.ContainsKey(items.Value.FieldOrigin.sMergeBoField))
                                                                        {
                                                                            nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = oBOI.Attributes[items.Value.FieldOrigin.sMergeBoField].sValue + " - " + DateTime.Now.ToString(XIConstant.DateTimeFull_Format), FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = oBOI.Attributes[items.Value.FieldOrigin.sMergeBoField].sValue, FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if ((items.Value.FieldOrigin.bIsMerge && !String.IsNullOrEmpty(items.Value.FieldOrigin.sMergeBo) && !String.IsNullOrEmpty(items.Value.FieldOrigin.sMergeBoField) && !string.IsNullOrEmpty(items.Value.FieldOrigin.sMergeVariable) && (Oldfields.Count() == 0 || (Oldfields.Count() > 0 && items.Value.FieldOrigin.bIsReload))))
                                        {
                                            XIInfraCache oCache = new XIInfraCache();
                                            XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, items.Value.FieldOrigin.sMergeBo, null);
                                            string sMergeCacheField = items.Value.FieldOrigin.sMergeVariable;
                                            var InstanceID = (string)oCache.Get_ParamVal(sSessionID, sGUID, null, sMergeCacheField);
                                            int iInstanceiD = 0;
                                            if (int.TryParse(InstanceID, out iInstanceiD))
                                            {
                                                XIIXI oXIIXI = new XIIXI();
                                                if (iInstanceiD != 0)
                                                {
                                                    var oBOI = oXIIXI.BOI(oBOD.Name, iInstanceiD.ToString());
                                                    if (oBOI.Attributes.ContainsKey(items.Value.FieldOrigin.sMergeBoField))
                                                    {
                                                        if (oBOD.Name == "EnumReconciliations_T")
                                                        {
                                                            if (oBOI.Attributes.ContainsKey(items.Value.FieldOrigin.sMergeBoField))
                                                            {
                                                                nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = oBOI.Attributes[items.Value.FieldOrigin.sMergeBoField].sValue + " - " + DateTime.Now.ToString(XIConstant.DateTimeFull_Format), FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                                            }
                                                        }
                                                        else
                                                        {
                                                            nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = oBOI.Attributes[items.Value.FieldOrigin.sMergeBoField].sValue, FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                                        }
                                                    }
                                                    else
                                                    {
                                                        List<XIWhereParams> oWParams = new List<XIWhereParams>();
                                                        string[] operators = new string[] { "+", "-", "/", "%", "(", ")" };
                                                        List<string[]> lstItems = items.Value.FieldOrigin.sMergeBoField.Split(',').Select(f => new { Field = f }).Select(c => c.Field.Trim().Split(operators, StringSplitOptions.RemoveEmptyEntries)).ToList();
                                                        for (int i = 0; i < lstItems.Count; i++)
                                                        {
                                                            if (lstItems[i].Count() > 1)
                                                            {
                                                                Regex pattern = new Regex(Regex.Escape(lstItems[i][0]) + "(.+?)" + Regex.Escape(lstItems[i][1]));
                                                                var getoperators = pattern.Matches(items.Value.FieldOrigin.sMergeBoField).Cast<Match>().Select(s => s.Groups[1].Value).Distinct().ToList();
                                                                if (getoperators.Count() > 0)
                                                                {
                                                                    nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue
                                                                    {
                                                                        FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID),
                                                                        sValue = (oBOI.Attributes[lstItems[i][0]].doValue - oBOI.Attributes[lstItems[i][1]].doValue).ToString(),
                                                                        FKiFieldOriginID = items.Value.FKiXIFieldOriginID,
                                                                        bIsDisplay = items.Value.FieldOrigin.bIsDisplay
                                                                    };
                                                                }
                                                            }
                                                            else
                                                            {
                                                                nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = "", FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = "", FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                                }
                                            }
                                            else
                                            {
                                                nSecFieldValues[items.Value.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.Value.ID), sValue = "", FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(items.Value.FieldOrigin.sMinDate) && (items.Value.FieldOrigin.sMinDate.Contains("xi.s")))
                                        {
                                            XIDScript oXIScript = new XIDScript();
                                            oXIScript.sScript = items.Value.FieldOrigin.sMinDate.ToString();
                                            var oCResult = oXIScript.Execute_Script(sGUID, sSessionID);
                                            if (oCResult.bOK && oCResult.oResult != null)
                                            {
                                                var sVal = (string)oCResult.oResult;
                                                sVal = Utility.GetDateResolvedValue(sVal, "yyyy-MM-dd");
                                                items.Value.FieldOrigin.sMinDate = sVal;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(items.Value.FieldOrigin.sMaxDate) && (items.Value.FieldOrigin.sMaxDate.Contains("xi.s")))
                                        {
                                            XIDScript oXIScript = new XIDScript();
                                            oXIScript.sScript = items.Value.FieldOrigin.sMaxDate.ToString();
                                            var oCResult = oXIScript.Execute_Script(sGUID, sSessionID);
                                            if (oCResult.bOK && oCResult.oResult != null)
                                            {
                                                var sVal = (string)oCResult.oResult;
                                                items.Value.FieldOrigin.sMaxDate = Utility.GetDateResolvedValue(sVal, "yyyy-MM-dd");
                                                CResult oSResult = new CResult();
                                                oSResult.sMessage = items.Value.FieldOrigin.sName + "_" + items.Value.FieldOrigin.ID + " : Script Value:" + sVal + " & Resolved Value:" + items.Value.FieldOrigin.sMaxDate;
                                                SaveErrortoDB(oSResult, oQSInstance.ID);
                                            }
                                        }
                                        if (items.Value.FieldOrigin.bIsDBLoad == true)
                                        {
                                            Dictionary<string, object> fieldParams = new Dictionary<string, object>();
                                            fieldParams["FKiFieldOriginIDXIGUID"] = items.Value.FieldOrigin.XIGUID;
                                             fieldParams["FKiQSInstanceIDXIGUID"] = oQSInstance.XIGUID;
                                            var oFIns = Connection.Select<XIIValue>("XIFieldInstance_T", fieldParams).FirstOrDefault();
                                            var Val= "";
                                            if (oFIns != null)
                                            {
                                                 Val = oFIns.sValue;
                                            }
                                            else
                                            {
                                                Val = nSecFieldValues[items.Value.FieldOrigin.sName].sValue;
                                            }
                                            nSecFieldValues[items.Value.FieldOrigin.sName].sValue = Val; /*new XIIValue { FKiFieldDefinitionID = items.Value.ID, FKiFieldDefinitionIDXIGUID = items.Value.XIGUID, FKiFieldOriginIDXIGUID = items.Value.FKiXIFieldOriginIDXIGUID, sValue = Val, FKiFieldOriginID = items.Value.FKiXIFieldOriginID, bIsDisplay = items.Value.FieldOrigin.bIsDisplay };*/
                                        }

                                        oQSInstance.XIValues[items.Value.FieldOrigin.sName] = nSecFieldValues[items.Value.FieldOrigin.sName];
                                    }
                                    oSecIns.XIValues = nSecFieldValues;
                                }
                            }
                            if (!string.IsNullOrEmpty(sec.sName))
                            {
                                //nSecIns[sec.ID.ToString() + "_Sec"] = oSecIns;
                                nSecIns[sec.XIGUID.ToString() + "_Sec"] = oSecIns;
                                //nSecIns[sec.sName] = oSecIns;
                            }
                            else
                            {
                                //nSecIns[sec.ID.ToString() + "_Sec"] = oSecIns;
                                nSecIns[sec.XIGUID.ToString() + "_Sec"] = oSecIns;
                            }

                        }
                        //oStepInstance.nSectionInstances = Step.Sections;
                    }
                    oStepInstance.Sections = nSecIns;
                }
                else
                {

                }
                if (oQSInstance.Steps.Count() > 0)
                {
                    var StepIns = oQSInstance.Steps.Where(m => m.Value.FKiQSStepDefinitionIDXIGUID == Step.XIGUID).FirstOrDefault();
                    if (StepIns.Value != null)
                    {
                        oStepInstance.ID = StepIns.Value.ID;
                        oStepInstance.XIGUID = StepIns.Value.XIGUID;
                    }
                }

                oStepInstance.FKiQSStepDefinitionID = Step.ID;
                oStepInstance.FKiQSStepDefinitionIDXIGUID = Step.XIGUID;
                //oStepInstance.nFieldInstances = nFieldValues;
                //oStepInstance.QSStepDefiniton = Step;
                // Step Traces
                //Trace attribute on Step instance
                //Load Next Valid traces from refValidTraces
                string sTraceID = "";
                XIIXI oXII = new XIIXI();
                XIInfraCache oChe = new XIInfraCache();
                if (!string.IsNullOrEmpty(sTraceID))
                {
                    //oXII.iSwitchDataSrcID = oBOD.iDataSource;
                    //oXII.SwitchDataSrcIDXIGUID = oBOD.iDataSourceXIGUID;
                    var oTraceI = oXII.BOI("refValidTrace_T", sTraceID);
                    if (oTraceI != null && oTraceI.Attributes.Count() > 0)
                    {
                        var bButtons = oTraceI.AttributeI("bShowButtons").sValue;
                        var sTraceVisibleGroup = oTraceI.AttributeI("sVisibleGroup").sValue;
                        var sTraceLockGroup = oTraceI.AttributeI("sLockGroup").sValue;
                        var sTraceSummaryGroup = oTraceI.AttributeI("sSummaryGroup").sValue;
                        //if (!string.IsNullOrEmpty(sTraceVisibleGroup))
                        //{
                        //    sGroupName = sTraceVisibleGroup;
                        //}
                        //if (!string.IsNullOrEmpty(sTraceLockGroup))
                        //{
                        //    sLockGroup = sTraceLockGroup;
                        //}
                        if (!string.IsNullOrEmpty(bButtons) && bButtons.ToLower() == "true")
                        {
                            var sCurrentTrace = oTraceI.AttributeI("sName").sValue;
                            var iCount = sCurrentTrace.Count(m => m == '_');
                            XID1Click o1Click = new XID1Click();
                            o1Click.BOID = oTraceI.BOD.BOID;
                            //     XIDXI oXID = new XIDXI();
                            //oXID.sOrgDatabase = sCoreDB;
                            var oDataSource = (XIDataSource)oChe.GetObjectFromCache(XIConstant.CacheDataSource, null, "");
                            o1Click.sSwitchDB = oDataSource.sName;
                            o1Click.Query = "select * from refValidTrace_T where sname like '" + sCurrentTrace + "%' and FKiStepDID=" + Step.ID + " and FKiQSDID=" + QSDefinition.ID;
                            var Response = o1Click.OneClick_Run();
                            if (Response != null && Response.Count() > 0)
                            {
                                foreach (var BOI in Response.Values.ToList())
                                {
                                    var sNextTrace = BOI.AttributeI("sName").sValue;
                                    var iNextCount = sNextTrace.Count(m => m == '_');
                                    if (sNextTrace != sCurrentTrace && iNextCount == (iCount + 1))
                                    {
                                        var index = sNextTrace.LastIndexOf("_");
                                        var Code = sNextTrace.Substring(index, sNextTrace.Length - index);
                                        if (!string.IsNullOrEmpty(Code))
                                        {
                                            List<CNV> oParam = new List<CNV>();
                                            oParam.Add(new CNV { sName = "sName", sValue = Code.Replace("_", "") });
                                            oParam.Add(new CNV { sName = "FKiStepDID", sValue = Step.ID.ToString() });
                                            oParam.Add(new CNV { sName = "FKiQSDID", sValue = QSDefinition.ID.ToString() });
                                            var oStageI = oXII.BOI("RefTraceStage", null, null, oParam);
                                            if (oStageI != null && oStageI.Attributes.Count() > 0)
                                            {
                                                var Name = oStageI.AttributeI("sDescription").sValue;
                                                var FKiXILinkID = oStageI.AttributeI("FKiXILinkID").sValue;
                                                int iXILinkID = 0;
                                                int.TryParse(FKiXILinkID, out iXILinkID);
                                                if (oStepInstance.Traces.Count() == 0)
                                                {
                                                    //oStepInstance.Traces.Add(new XIVisualisation() { Name = "tracebuttons" });
                                                    oStepInstance.Traces = new List<CNV>();
                                                }
                                                oStepInstance.Traces.Add(new CNV { sName = "", sValue = "" });
                                                //var Vis = Visualisations.FirstOrDefault().NVs.Where(m => m.sName.ToLower() == Name.ToLower()).FirstOrDefault();
                                                //if (Vis == null)
                                                //{
                                                //    Visualisations.FirstOrDefault().NVs.Add(new XIVisualisationNV { sName = Name + "-" + sAttrName, sType = "TraceBtn", sValue = iXILinkID + "-" + oStageI.AttributeI("iStatusValue1").sValue });
                                                //}
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (oQSInstance.Steps != null && oQSInstance.Steps.Count() > 0)
                {
                    if (oQSInstance.Steps.ContainsKey(Step.sName))
                    {
                        oQSInstance.Steps[Step.sName] = oStepInstance;//Adding Step Instance to nInstances
                    }
                    else
                    {
                        //var StepIndex = oQSInstance.Steps.Values.FindIndex(m => m.FKiQSStepDefinitionID == Step.ID);
                        //oQSInstance.Steps.Remove(oQSInstance.Steps.Where(m => m.FKiQSStepDefinitionID == Step.ID).FirstOrDefault());
                        oQSInstance.Steps[Step.sName] = oStepInstance;
                    }
                }
                else
                {
                    oQSInstance.Steps = new Dictionary<string, XIIQSStep>();
                    if (!oQSInstance.Steps.ContainsKey(Step.sName))
                    {
                        oQSInstance.Steps[Step.sName] = oStepInstance;//Adding Step Instance to nInstances
                    }
                }
                //Setting Current Step Property to the step to be displayed
                oQSInstance.Steps.ToList().ForEach(m => m.Value.bIsCurrentStep = false);
                if (StepXIGUID != null && StepXIGUID != Guid.Empty)
                {
                    oQSInstance.Steps.Where(m => m.Value.FKiQSStepDefinitionIDXIGUID == StepXIGUID).FirstOrDefault().Value.bIsCurrentStep = true;
                }
                else if (iStepID > 0)
                {
                    oQSInstance.Steps.Where(m => m.Value.FKiQSStepDefinitionID == iStepID).FirstOrDefault().Value.bIsCurrentStep = true;
                }
                else
                {
                    oQSInstance.Steps.FirstOrDefault().Value.bIsCurrentStep = true;
                }
                //if (oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == StepID).FirstOrDefault().Value != null)
                //{
                //    oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == StepID).FirstOrDefault().Value.sIsHidden = "off";
                //}

            }
            if (oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == Step.ID).Select(m => m.Value.bInMemoryOnly).FirstOrDefault() == false)
            {
                //SaveQSInstances(oQSInstance, sGUID);
            }

            //oCache.UpdateCacheObject("QuestionSet", sGUID, oQSInstance, sDatabase, oQSInstance.FKiQSDefinitionID);
            return oQSInstance;
        }


        public Guid GetActiveStepID(string iStepID, string sGUID = "")
        {
            int StepID = 0;
            Guid StepXIGUID = Guid.Empty;
            int.TryParse(iStepID, out StepID);
            Guid.TryParse(iStepID, out StepXIGUID);
            var sSessionID = HttpContext.Current.Session.SessionID;
            string bIsActive = string.Empty;
            XIDQSStep oStepD = new XIDQSStep();
            if (StepID >= 0 || (StepXIGUID != null && StepXIGUID != Guid.Empty))
            {
                if (StepXIGUID != null && StepXIGUID != Guid.Empty)
                {
                    oStepD = QSDefinition.Steps.Values.Where(m => m.XIGUID == StepXIGUID).FirstOrDefault();
                }
                else if (StepID > 0)
                {
                    oStepD = QSDefinition.Steps.Values.Where(m => m.ID == StepID).FirstOrDefault();
                }
                else
                {
                    oStepD = QSDefinition.Steps.Values.ToList().OrderBy(m => m.iOrder).FirstOrDefault();
                }
                //oStepD = QSDefinition.Steps.Values.ToList().OrderBy(m => m.iOrder).FirstOrDefault();
                if (oStepD != null && oStepD.Scripts != null && oStepD.Scripts.Count() > 0 && oStepD.Scripts.ContainsKey("Step Visibility"))
                {
                    XIDScript oScriptD = new XIDScript();
                    oScriptD = oStepD.Scripts["Step Visibility"];
                    if (oScriptD != null && oScriptD.sLanguage.ToLower() == "XIScript".ToLower())
                    {
                        var oRes = oScriptD.Execute_Script(sGUID, sSessionID);
                        if (oRes.bOK && oRes.oResult != null)
                        {
                            bIsActive = (string)oRes.oResult;
                            if (bIsActive == "false")
                            {
                                StepXIGUID = QSDefinition.Steps.Values.ToList().OrderBy(m => m.iOrder).Where(m => m.iOrder > oStepD.iOrder).Select(m => m.XIGUID).FirstOrDefault();
                                iActiveStepIDXIGUID = StepXIGUID;
                                GetActiveStepID(StepXIGUID.ToString(), sGUID);
                            }
                            else
                            {
                                iActiveStepIDXIGUID = oStepD.XIGUID;
                                //if (!string.IsNullOrEmpty(sTransactionType))
                                //{
                                //    iActiveStepID = oStepD.ID;
                                //}
                                //else
                                //{
                                //    iStepID = QSDefinition.Steps.Values.ToList().OrderBy(m => m.iOrder).Where(m => m.iOrder > oStepD.iOrder).Select(m => m.ID).FirstOrDefault();
                                //    iActiveStepID = iStepID;
                                //    GetActiveStepID(iStepID, sTransactionType);
                                //}
                            }
                        }
                    }
                }
                else if (oStepD != null && !string.IsNullOrEmpty(oStepD.sRuleCondition))
                {
                    string sRuleCondition = oStepD.sRuleCondition;
                    foreach (var item in XIValues)
                    {
                        if (sRuleCondition.ToLower().Contains("qs." + item.Key.ToLower()))
                        {
                            sRuleCondition = sRuleCondition.ToLower().Replace("qs." + item.Key.ToLower(), item.Value.sValue);
                        }
                    }
                    XIDScript oScriptD = new XIDScript();
                    oScriptD.sScript = sRuleCondition;
                    var oRes = oScriptD.Execute_Script(sGUID, sSessionID);
                    if (oRes.bOK && oRes.oResult != null)
                    {
                        bIsActive = (string)oRes.oResult;
                        if (bIsActive == "false")
                        {
                            StepXIGUID = QSDefinition.Steps.Values.ToList().OrderBy(m => m.iOrder).Where(m => m.iOrder > oStepD.iOrder).Select(m => m.XIGUID).FirstOrDefault();
                            iActiveStepIDXIGUID = StepXIGUID;
                            GetActiveStepID(StepXIGUID.ToString(), sGUID);
                        }
                        else
                        {
                            iActiveStepIDXIGUID = oStepD.XIGUID;
                        }
                    }
                }
                else
                {
                    if (oStepD != null)
                    {
                        if (oStepD.bIsContinue)
                        {
                            StepXIGUID = QSDefinition.Steps.Values.ToList().OrderBy(m => m.iOrder).Where(m => m.iOrder > oStepD.iOrder).Select(m => m.XIGUID).FirstOrDefault();
                            iActiveStepIDXIGUID = StepXIGUID;
                        }
                        else
                        {
                            iActiveStepIDXIGUID = oStepD.XIGUID;
                        }
                    }
                }
            }
            return iActiveStepIDXIGUID;
        }

        public XIIQSSection LoadSectionInstance(XIDQSSection oXIDQS, string sGUID = "")
        {
            var sSessionID = HttpContext.Current.Session.SessionID;
            XIDQSSection sec = oXIDQS;
            XIIQSSection oSecIns = null;
            try
            {
                if (oSecIns == null)
                {
                    oSecIns = new XIIQSSection();
                    oSecIns.FKiStepSectionDefinitionID = oXIDQS.ID;
                    if (((xiSectionContent)sec.iDisplayAs).ToString() == xiSectionContent.Fields.ToString())
                    {
                        Dictionary<string, XIIValue> nSecFieldValues = new Dictionary<string, XIIValue>();
                        if (sec.FieldDefs != null && sec.FieldDefs.Count() > 0)
                        {
                            var Def = sec.FieldDefs.Values.OrderBy(m => m.ID).ToList();//.Select(m => new XIIValue { FKiFieldDefinitionID = m.ID }).ToList();
                            foreach (var def in Def)
                            {
                                if (def.FieldOrigin.bIsMerge && !String.IsNullOrEmpty(def.FieldOrigin.sMergeField))
                                {
                                    //nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = oQSInstance.XIIValues(def.FieldOrigin.sMergeField), FKiFieldOriginID = def.FKiXIFieldOriginID };
                                }
                                else if (def.FieldOrigin.bIsMerge && !String.IsNullOrEmpty(def.FieldOrigin.sMergeBo) && !String.IsNullOrEmpty(def.FieldOrigin.sMergeBoField) && !string.IsNullOrEmpty(def.FieldOrigin.sMergeVariable))
                                {
                                    XIInfraCache oCache = new XIInfraCache();
                                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, def.FieldOrigin.sMergeBo, null);
                                    string sMergeCacheField = def.FieldOrigin.sMergeVariable;
                                    var InstanceID = (string)oCache.Get_ParamVal(sSessionID, sGUID, null, sMergeCacheField);
                                    int iInstanceiD = 0;
                                    if (int.TryParse(InstanceID, out iInstanceiD))
                                    {
                                        XIIXI oXIIXI = new XIIXI();
                                        var oBOI = oXIIXI.BOI(oBOD.Name, iInstanceiD.ToString());
                                        if (oBOI.Attributes.ContainsKey(def.FieldOrigin.sMergeBoField))
                                        {
                                            if (def.FieldOrigin.DataType.sName.ToLower() == "date")
                                            {
                                                string sFormat = def.FieldOrigin.sFormat;
                                                if (string.IsNullOrEmpty(sFormat))
                                                {
                                                    sFormat = XIConstant.Date_Format; //"dd-MMM-yyyy";
                                                }
                                                if (!string.IsNullOrEmpty(oBOI.Attributes[def.FieldOrigin.sMergeBoField].sValue))
                                                {
                                                    string sValue = oBOI.Attributes[def.FieldOrigin.sMergeBoField].sValue;
                                                    oBOI.Attributes[def.FieldOrigin.sMergeBoField].sValue = String.Format("{0:" + sFormat + "}", Convert.ToDateTime(sValue));
                                                }
                                            }
                                            nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = oBOI.Attributes[def.FieldOrigin.sMergeBoField].sValue, FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                        }
                                        else
                                        {
                                            nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = "", FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                        }
                                    }
                                    else
                                    {
                                        nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = "", FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(def.FieldOrigin.sDefaultDate))
                                    {
                                        nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = Utility.GetDefaultDateResolvedValue(def.FieldOrigin.sDefaultDate, XIConstant.Date_Format), FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                    }
                                    else
                                    {
                                        //if (oQSInstance.XIValues.ContainsKey(def.FieldOrigin.sName.ToString()))
                                        //{
                                        //    nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = oQSInstance.XIValues[def.FieldOrigin.sName.ToString()].sValue, FKiFieldOriginID = def.FKiXIFieldOriginID };
                                        //}
                                        //else
                                        //{
                                        //    nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), FKiFieldOriginID = def.FKiXIFieldOriginID };
                                        //}
                                    }
                                }
                                if (!string.IsNullOrEmpty(def.FieldOrigin.sScript))
                                {
                                    string sVal = string.Empty;
                                    if (nSecFieldValues.ContainsKey(def.FieldOrigin.sName))
                                    {
                                        sVal = nSecFieldValues[def.FieldOrigin.sName].sValue;
                                    }
                                    XIDScript oScriptD = new XIDScript();
                                    oScriptD.sScript = def.FieldOrigin.sScript;
                                    if (oScriptD != null)
                                    {
                                        var oRes = oScriptD.Execute_Script(sGUID, sSessionID);
                                        if (oRes.bOK && oRes.oResult != null)
                                        {
                                            string result = (string)oRes.oResult;
                                            if (!string.IsNullOrEmpty(result) && result.ToLower() == "true")
                                            {
                                                def.FieldOrigin.bIsDisable = true;
                                            }
                                            else
                                            {
                                                def.FieldOrigin.bIsDisable = false;
                                                nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = sVal, FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                            }
                                            if (!string.IsNullOrEmpty(def.FieldOrigin.sFieldDefaultValue))
                                            {
                                                nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = def.FieldOrigin.sFieldDefaultValue, FKiFieldOriginID = def.FKiXIFieldOriginID, bIsDisplay = def.FieldOrigin.bIsDisplay };
                                            }
                                        }
                                    }
                                }
                                //oQSInstance.XIValues[def.FieldOrigin.sName] = nSecFieldValues[def.FieldOrigin.sName];
                            }
                        }
                        oSecIns.XIValues = nSecFieldValues;
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return oSecIns;
        }

        public XIIQS SaveQSAPI(XIIQS oQSInstance)
        {
            // TO DO - return type should be an object

            // TO DO - this method means, there is a full object model in memory with steps and maybe sections
            // and this code needs to persist this into the DB
            XIIQS oQSIns = null;
            Dictionary<string, object> Params = new Dictionary<string, object>();
            //Params["FKiQSDefinitionID"] = oQSInstance.FKiQSDefinitionID;
            oQSIns = new XIIQS();
            oQSIns.sQSName = oQSInstance.QSDefinition.sName;
            oQSIns.iCurrentStepIDXIGUID = oQSInstance.iCurrentStepIDXIGUID;
            oQSIns.FKiQSDefinitionID = oQSInstance.FKiQSDefinitionID;
            oQSIns.FKiQSDefinitionIDXIGUID = oQSInstance.FKiQSDefinitionIDXIGUID;
            oQSIns.FKiBODID = oQSInstance.FKiBODID;
            oQSIns.iBOIID = oQSInstance.iBOIID;
            oQSIns.FKiClassID = oQSInstance.QSDefinition.FKiClassID;
            oQSIns.CreatedTime = DateTime.Now;
            oQSIns = Connection.Insert<XIIQS>(oQSIns, "XIQSInstance_T", "ID");
            oQSInstance.ID = oQSIns.ID;
            XIInfraCache oCache = new XIInfraCache();
            XIIBO BulkInsert = new XIIBO();
            List<XIIBO> oBulkBO = new List<XIIBO>();
            XIIValue oFIns = new XIIValue();
            oFIns = new XIIValue();
            foreach (var items in oQSInstance.XIValues)
            {
                XIIBO oxiibo = new XIIBO();
                var FieldOrigin = oQSInstance.QSDefinition.XIDFieldOrigin.Values.Where(m => m.ID == items.Value.FKiFieldOriginID).FirstOrDefault();
                if (FieldOrigin.DataType.sBaseDataType.ToLower() == "int")
                {
                    int ival;
                    if (int.TryParse(items.Value.sValue, out ival))
                    {
                        oFIns.iValue = ival;
                    }
                    else
                    {
                        oFIns.iValue = 0;
                    }
                }
                else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "datetime")
                {
                    //if (!string.IsNullOrEmpty(items.Value.sValue))
                    //{
                    //    oFIns.dValue = Convert.ToDateTime(items.Value.sValue);
                    //}
                }
                else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "decimal")
                {
                    decimal rval;
                    if (decimal.TryParse(items.Value.sValue, out rval))
                    {
                        oFIns.rValue = rval;
                    }
                    else
                    {
                        oFIns.rValue = 0;
                    }
                }
                else if (FieldOrigin.DataType.sBaseDataType.ToLower() == "boolean")
                {
                    if (items.Value.sValue == "on")
                    {
                        oFIns.bValue = true;
                    }
                    else
                    {
                        oFIns.bValue = false;
                    }

                }
                oxiibo.SetAttribute("rValue", oFIns.rValue.ToString());
                oxiibo.SetAttribute("bValue", oFIns.bValue.ToString());
                oxiibo.SetAttribute("iValue", oFIns.iValue.ToString());
                //if (oFIns.dValue.ToString() == "1/1/0001 12:00:00 AM")
                //{
                //    oFIns.dValue = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                //}

                oxiibo.SetAttribute("sDerivedValue", items.Value.sDerivedValue);
                oxiibo.SetAttribute("sValue", items.Value.sValue);
                oxiibo.SetAttribute("FKiQSInstanceID", oQSIns.ID.ToString());
                oxiibo.SetAttribute("FKiFieldOriginID", FieldOrigin.ID.ToString());
                oxiibo.SetAttribute("dValue", DateTime.Now.ToString());

                oFIns.sDerivedValue = items.Value.sDerivedValue;
                oFIns.sValue = items.Value.sValue;
                if (string.IsNullOrEmpty(oFIns.sDerivedValue))
                {
                    oFIns.sDerivedValue = items.Value.sValue;
                    oxiibo.SetAttribute("sDerivedValue", items.Value.sValue);
                }

                oFIns.FKiQSInstanceID = oQSIns.ID;
                oFIns.FKiFieldDefinitionID = items.Value.FKiFieldDefinitionID;
                oFIns.FKiFieldOriginID = FieldOrigin.ID;
                oFIns.dValue = DateTime.Now;
                //oFIns = Connection.Insert<XIIValue>(oFIns, "XIFieldInstance_T", "ID");
                oBulkBO.Add(oxiibo);
            }
            if (oBulkBO != null && oBulkBO.Count() > 0)
            {
                var BoD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T", null);
                oBulkBO.ForEach(f => f.BOD = BoD);
                var MakeDatatble = BulkInsert.MakeBulkSqlTable(oBulkBO);
                BulkInsert.SaveBulk(MakeDatatble, BoD.iDataSourceXIGUID.ToString(), "XIFieldInstance_T");
            }
            return oQSInstance;
        }

        #region WhatsappAPI

        public XIIQS API_SaveQSInstances(XIIQS oQSInstance, string sGUID, string sSessionID)
        {
            XIInfraCache oCache = new XIInfraCache();
            try
            {
                string sCurrentGuestUser = string.Empty;
                var CurrentStepID = oQSInstance.iCurrentStepIDXIGUID;
                var oStepD = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == CurrentStepID).FirstOrDefault();
                XIIQSStep oStepI = oQSInstance.Steps[oStepD.sName];
                //var CurrentStepID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, "CurrentStepID_" + oQSInstance.FKiQSDefinitionID));
                //var CurrentStepID = oQSInstance.iCurrentStepIDXIGUID;
                if (oQSInstance.Steps.ContainsKey(oStepD.sName))
                {
                    //oQSInstance.Steps[oStepD.sName] = oStepI;
                    foreach (var Section in oQSInstance.Steps[oStepD.sName].Sections)
                    {
                        var oFieldDef = oStepD.Sections[Section.Key].FieldDefs;
                        foreach (var Field in Section.Value.XIValues)
                        {
                            var oFiledOrigin = oFieldDef[Field.Key].FieldOrigin;
                            if (oFiledOrigin.bIsSetToCache)
                            {
                                string sCacheParam = oFiledOrigin.sName;
                                if (!string.IsNullOrEmpty(oFiledOrigin.sCacheName))
                                {
                                    sCacheParam = oFiledOrigin.sCacheName;
                                }
                                oCache.Set_ParamVal(sSessionID, sGUID, "", sCacheParam, Field.Value.sValue, "", null);
                            }
                            if (!string.IsNullOrEmpty(oFiledOrigin.sDependentFieldIDs) && oFiledOrigin.sIsHidden == "on")
                            {
                                var oDependentFields = oFiledOrigin.sDependentFieldIDs.Split(',');
                                if (oFiledOrigin.DataType.sBaseDataType == "datetime")
                                {
                                    XIIBO oBOI = new XIIBO();
                                    string sDay = string.Empty; string sMonth = string.Empty; string sYear = string.Empty; string sResult = string.Empty;
                                    string sFormat = oFiledOrigin.sFormat;
                                    if (string.IsNullOrEmpty(sFormat))
                                    {
                                        sFormat = XIConstant.Date_Format; //"dd-MMM-yyyy";
                                    }
                                    foreach (var sOriginID in oDependentFields)
                                    {
                                        Guid iOriginId = Guid.Empty;
                                        if (Guid.TryParse(sOriginID, out iOriginId))
                                        {
                                            //int iOriginId = Convert.ToInt32(sOriginID);
                                            var oXIValues = Section.Value.XIValues.Values.ToList();
                                            var oDependentFieldDef = oFieldDef.Values.ToList();
                                            var oDependentFiledOrigin = oDependentFieldDef.Where(x => x.FKiXIFieldOriginIDXIGUID == iOriginId).Select(x => x.FieldOrigin).FirstOrDefault();
                                            if (oDependentFiledOrigin.sCode.ToLower() == "month")
                                            {
                                                sMonth = oXIValues.Where(x => x.FKiFieldOriginIDXIGUID == iOriginId).Select(x => x.sValue).FirstOrDefault();
                                                //if (!string.IsNullOrEmpty(sMonth) && !sMonth.StartsWith("0"))
                                                //{
                                                //int iMonth = Convert.ToInt32(sMonth);
                                                int iMonth = 0;
                                                if (int.TryParse(sMonth, out iMonth))
                                                {
                                                    if (iMonth <= 9 && !sMonth.StartsWith("0"))
                                                    {
                                                        sMonth = "0" + sMonth;
                                                    }
                                                }
                                                //}
                                            }
                                            if (oDependentFiledOrigin.sCode.ToLower() == "year")
                                            {
                                                sYear = oXIValues.Where(x => x.FKiFieldOriginIDXIGUID == iOriginId).Select(x => x.sValue).FirstOrDefault();
                                            }
                                            if (oDependentFiledOrigin.sCode.ToLower() == "date")
                                            {
                                                sDay = oXIValues.Where(x => x.FKiFieldOriginIDXIGUID == iOriginId).Select(x => x.sValue).FirstOrDefault();
                                                //if (!string.IsNullOrEmpty(sDay) && !sDay.StartsWith("0"))
                                                //{
                                                //int iDay = Convert.ToInt32(sDay);
                                                int iDay = 0;
                                                if (int.TryParse(sDay, out iDay))
                                                {
                                                    if (iDay <= 9 && !sDay.StartsWith("0"))
                                                    {
                                                        sDay = "0" + sDay;
                                                    }
                                                }
                                                //}
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(sDay) && !string.IsNullOrEmpty(sMonth) && !string.IsNullOrEmpty(sYear))
                                        {
                                            sResult = sDay + "-" + sMonth + "-" + sYear;
                                        }
                                        else if (string.IsNullOrEmpty(sDay) && !string.IsNullOrEmpty(sMonth) && !string.IsNullOrEmpty(sYear))
                                        {
                                            sResult = "01" + "-" + sMonth + "-" + sYear;
                                        }
                                        if (!string.IsNullOrEmpty(sResult))
                                        {
                                            Field.Value.sValue = String.Format("{0:" + sFormat + "}", oBOI.ConvertToDtTime(sResult));
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var sOriginID in oDependentFields)
                                    {
                                        int iOriginId = 0;
                                        if (int.TryParse(sOriginID, out iOriginId))
                                        {
                                            //int iOriginId = Convert.ToInt32(sOriginID);
                                            var oXIValues = Section.Value.XIValues.Values.ToList();
                                            var oDependentFieldDef = oFieldDef.Values.ToList();
                                            var oDependentFiledOrigin = oDependentFieldDef.Where(x => x.FKiXIFieldOriginID == iOriginId).Select(x => x.FieldOrigin).FirstOrDefault();
                                            if (oDependentFiledOrigin != null && !oDependentFiledOrigin.bIsHidden)
                                            {
                                                var oFieldI = oXIValues.Where(m => m.FKiFieldOriginID == iOriginId).FirstOrDefault();
                                                if (oFieldI != null && !string.IsNullOrEmpty(oFieldI.sValue))
                                                {
                                                    Field.Value.sValue = oFieldI.sValue;
                                                    Field.Value.sDerivedValue = oFieldI.sDerivedValue;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            oQSInstance.XIValues[Field.Key].sValue = Field.Value.sValue;
                            oQSInstance.XIValues[Field.Key].sDerivedValue = Field.Value.sDerivedValue;
                        }
                    }
                }
                var Response = Save(oQSInstance, sCurrentGuestUser);
                oCache.Set_QuestionSetCache("QuestionSetCache", sGUID, oQSInstance.XIGUID.ToString(), oQSInstance);
                //var Response = XiLinkRepository.SaveQSInstance(oQSInstance, CurrentStepID, iUserID, sOrgName, sDatabase, sCurrentGuestUser);
                return oQSInstance;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion WhatsappAPI

        public CResult Load_ReqQSDefParams(string sSessionID, string sGUID, string DefUid, string sType)//sType is QS/Step
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Check and load the required QS definition params before loading the question set";//expalin about this method logic
            try
            {
                //Get all required params of the DefGuid it can be QS/Step
                //{XIP|iClientID} Param value
                int iID = 0;
                Guid XIGUID = Guid.Empty;
                int.TryParse(DefUid, out iID);
                Guid.TryParse(DefUid, out XIGUID);
                if (iID > 0 || (XIGUID != null && XIGUID != Guid.Empty))
                {
                    XIInfraCache oCache = new XIInfraCache();
                    List<XIRequiredParamDef> oQSParams = (List<XIRequiredParamDef>)oCache.GetObjectFromCache(XIConstant.CacheXIQSParams, sType, DefUid.ToString());
                    if (oQSParams != null && oQSParams.Count > 0)
                    {
                        oCResult.oResult = oQSParams;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: DefUid is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Load_ReqQSInsParams(string sSessionID, string sGUID, string InsUid, string sType)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Check and load the required QS instance params before loading the question set";//expalin about this method logic
            try
            {
                //Get all required params of the DefGuid it can be QS/Step
                //{XIP|iClientID} Param value
                int iID = 0;
                Guid XIGUID = Guid.Empty;
                int.TryParse(InsUid, out iID);
                Guid.TryParse(InsUid, out XIGUID);
                if (iID > 0 || (XIGUID != null && XIGUID != Guid.Empty))
                {
                    XID1Click o1ClickD = new XID1Click();
                    o1ClickD.Query = "select * from XIPersistedParams_T where FKiQSIIDXIGUID='" + InsUid + "'";
                    o1ClickD.BOIDXIGUID = new Guid("43E34A2D-8E81-4137-A74E-25DA18896C57");
                    var oRes = o1ClickD.OneClick_Run();
                    if (oRes != null && oRes.Count > 0)
                    {
                        List<CNV> oNVs = new List<CNV>();
                        foreach (var oBOI in oRes.Values.ToList())
                        {
                            int ID = Convert.ToInt32(oBOI.AttributeI("id").sValue);
                            var sName = oBOI.AttributeI("sName").sValue;
                            var sValue = oBOI.AttributeI("sValue").sValue;
                            oNVs.Add(new CNV { sName = sName, sValue = sValue, ID = ID });
                        }
                        oCResult.oResult = oNVs;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: DefUid is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Save_ReqQSParams(string QSUid, string StepUid, string sName, string sValue)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "save required QS params to load before reloading the question set";//expalin about this method logic
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIBO oBOI = new XIIBO();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIPersistedParams");
                oBOI.BOD = oBOD;
                oBOI.SetAttribute("FKiQSIIDXIGUID", QSUid);
                oBOI.SetAttribute("FKiStepIIDXIGUID", StepUid);
                oBOI.SetAttribute("sName", sName);
                oBOI.SetAttribute("sValue", sValue);
                oCR = oBOI.Save(oBOI);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oCResult.oResult = "Success";
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}