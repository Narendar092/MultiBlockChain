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
using XIDatabase;
using System.Configuration;

namespace XICore
{
    public class XIDQSStep : XIDefinitionBase
    {
        public int ID { get; set; }
        public int FKiQSDefintionID { get; set; }
        public string sName { get; set; }
        public string sDisplayName { get; set; }
        public decimal iOrder { get; set; }
        public string sCode { get; set; }
        public int iDisplayAs { get; set; }
        public int XILinkID { get; set; }
        public Guid XILinkIDXIGUID { get; set; }
        public int FKiContentID { get; set; }
        public int iXIComponentID { get; set; }
        public int i1ClickID { get; set; }
        public string HTMLContent { get; set; }
        public bool bIsSaveNext { get; set; }
        public bool bIsSave { get; set; }
        public bool bIsBack { get; set; }
        public bool bIsSaveClose { get; set; }
        public bool bIsContinue { get; set; }
        public bool bInMemoryOnly { get; set; }
        public int iLayoutID { get; set; }
        public Guid iLayoutIDXIGUID { get; set; }
        public string sIsHidden { get; set; }
        public bool bIsHistory { get; set; }
        public bool bIsCopy { get; set; }
        public string sSaveBtnLabel { get; set; }
        public string sSaveCloseLabel { get; set; }
        public string sBackBtnLabel { get; set; }
        public bool bIsReload { get; set; }
        public bool bIsMerge { get; set; }
        public int iStage { get; set; }
        public int iLockStage { get; set; }
        public int iCutStage { get; set; }
        public string sImage { get; set; }
        public string s1ClickIDs { get; set; }
        public List<XIDropDown> XIFields { get; set; }
        public Dictionary<string, string> XILinks { get; set; }//auto complete fields
        public List<XIDropDown> ddlContent { get; set; }
        public List<string> XIFieldValues { get; set; }
        public List<XIDropDown> ddlLayouts { get; set; }
        public Dictionary<string, string> XICodes { get; set; }  //Auto Complete XILink Codes
        public int FKiAppID { get; set; }
        public int iOrgID { get; set; }
        public string sSaveBtnLabelSaveNext { get; set; }
        public string sSaveLabel { get; set; }
        public bool bIsHidden { get; set; }
        public List<XIDropDown> oQSddl { get; set; } //All QuestionSet DropDowns
        public int iOverrideStep { get; set; }
        public Guid iOverrideStepXIGUID { get; set; }
        public int FKiParentStepID { get; set; }
        public Guid XIGUID { get; set; }
        public int FkiRuleID { get; set; }
        public Guid FkiRuleIDXIGUID { get; set; }
        public string sRuleCondition { get; set; }
        public bool bIsAutoReload { get; set; }
        public bool bIsAutoSaving { get; set; }
        public Nullable<Guid> FKiQSDefintionIDXIGUID { get; set; }
        public string sJavaScript { get; set; }

        public int FKiCssID { get; set; }
        public string FKsCss { get; set; }
        public string sCssClass { get; set; }
        public Guid BOIDXIGUID { get; set; }
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        private XIDComponent oMyCompoent { get; set; }
        public XIDComponent ComponentDefinition
        {
            get { return oMyCompoent; }
            set { oMyCompoent = value; }
        }

        private XIDLayout oMyLayout;
        public XIDLayout Layout
        {
            get
            {
                return oMyLayout;
            }
            set
            {
                oMyLayout = value;
            }
        }

        private Dictionary<string, XIDValue> oMyXIValues = new Dictionary<string, XIDValue>();

        public Dictionary<string, XIDValue> XIValues
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
        public XIDValue XIValueD(string sValueName)
        {
            XIDValue oThisValueD = null/* TODO Change to default(_) if this is not a reference type */;

            // Ravi should have done a lot of this


            sValueName = sValueName.ToLower();


            if (oMyXIValues.ContainsKey(sValueName))
            {
            }
            else
            {
            }

            return oThisValueD;
        }

        public Dictionary<string, XIDQSSection> oMySections = new Dictionary<string, XIDQSSection>();
        public Dictionary<string, XIDQSSection> Sections
        {
            get
            {
                return oMySections;
            }
            set
            {
                oMySections = value;
            }
        }
        public XIDQSSection SectionD(string sSectionName)
        {
            XIDQSSection oThisSectionD = null/* TODO Change to default(_) if this is not a reference type */;

            // The sections of this Step must be loaded

            // TO DO - make this work on numbers also - eg 1 is an index position of 1

            sSectionName = sSectionName.ToLower();

            if (oMySections.ContainsKey(sSectionName) == false)
            {
            }

            if (oMySections.ContainsKey(sSectionName))
            {
            }
            else
            {
            }

            return oThisSectionD;
        }

        private Dictionary<string, XIDQSStepNavigations> oMyNavigations = new Dictionary<string, XIDQSStepNavigations>();
        public Dictionary<string, XIDQSStepNavigations> Navigations
        {
            get
            {
                return oMyNavigations;
            }
            set
            {
                oMyNavigations = value;
            }
        }

        private Dictionary<string, XIDFieldDefinition> oMyFieldDefs = new Dictionary<string, XIDFieldDefinition>();
        public Dictionary<string, XIDFieldDefinition> FieldDefs
        {
            get
            {
                return oMyFieldDefs;
            }
            set
            {
                oMyFieldDefs = value;
            }
        }

        private Dictionary<string, XIDScript> oMyScripts = new Dictionary<string, XIDScript>();
        public Dictionary<string, XIDScript> Scripts
        {
            get
            {
                return oMyScripts;
            }
            set
            {
                oMyScripts = value;
            }
        }

        private Dictionary<string, XIQSLink> oMyQSLinks = new Dictionary<string, XIQSLink>();
        public Dictionary<string, XIQSLink> QSLinks
        {
            get
            {
                return oMyQSLinks;
            }
            set
            {
                oMyQSLinks = value;
            }
        }

        private Dictionary<string, CNV> oMyTraceButtons = new Dictionary<string, CNV>();
        public Dictionary<string, CNV> TraceButtons
        {
            get
            {
                return oMyTraceButtons;
            }
            set
            {
                oMyTraceButtons = value;
            }
        }

        public CResult Preview()
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
                XIInfraCache oCache = new XIInfraCache();
                XIDQSStep oStepD = new XIDQSStep();
                oStepD = (XIDQSStep)oDefintion;
                XIDQSStep oStepC = new XIDQSStep();
                oStepC = (XIDQSStep)oStepD.Clone(oStepD);
                if (oStepC.iLayoutID > 0)
                {
                    XIDLayout oLayoutD = new XIDLayout();
                    oLayoutD.ID = oStepC.iLayoutID;
                    var oLayContent = oLayoutD.Preview();
                    if (oLayContent.bOK && oLayContent.oResult != null)
                    {
                        oStepC.oContent[XIConstant.ContentLayout] = (XIDLayout)(oLayContent.oResult);
                    }
                }
                else
                {
                    if (oStepC.Sections != null && oStepC.Sections.Count() > 0)
                    {
                        Dictionary<string, XIDQSSection> Sections = new Dictionary<string, XIDQSSection>();
                        foreach (var sec in oStepC.Sections)
                        {
                            XIDQSSection oSecD = new XIDQSSection();
                            oSecD.oDefintion = sec.Value;
                            var oSecContent = oSecD.Preview();
                            if (oSecContent.bOK && oSecContent.oResult != null)
                            {
                                Sections[sec.Value.ID.ToString()] = (XIDQSSection)oSecContent.oResult;
                            }
                        }
                        oStepC.Sections = Sections;
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oStepC;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading XiLink Definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }


        #region QSStepConfigComponent

        public CResult Get_QSStepConfigDef()
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
                XIDQSStep oQSStep = new XIDQSStep();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (ID != 0)
                {
                    Params["ID"] = ID;
                    oQSStep = Connection.Select<XIDQSStep>("XIQSStepDefinition_T", Params).FirstOrDefault();
                }
                //All QuestionSet DropDowns
                List<XIDropDown> oQSDDL = new List<XIDropDown>();
                Dictionary<string, object> oQSParam = new Dictionary<string, object>();
                //oQSParam["FKiApplicationID"] = FKiAppID;
                //oQSParam["FKiOrgID"] = iOrgID;
                var oQSDef = Connection.Select<XIDQS>("XIQSDefinition_T", oQSParam).ToList();
                oQSDDL = oQSDef.Select(m => new XIDropDown { Value = m.ID, text = m.sName }).ToList();
                oQSDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oQSStep.oQSddl = oQSDDL;

                //FieldOrigins DropDowns
                List<XIDropDown> oFieldDDL = new List<XIDropDown>();
                Dictionary<string, object> oFO = new Dictionary<string, object>();
                oFO["FKiApplicationID"] = FKiAppID;
                oFO["FKiOrgID"] = iOrgID;
                var oFODef = Connection.Select<XIDFieldOrigin>("XIFieldOrigin_T", oFO).ToList();
                oFieldDDL = oFODef.Select(m => new XIDropDown { Value = m.ID, text = m.sName }).ToList();
                oQSStep.XIFields = oFieldDDL;

                //AutoComplete For Xilinks
                Dictionary<string, string> XiLinks = new Dictionary<string, string>();
                Dictionary<string, object> oLinks = new Dictionary<string, object>();
                oLinks["FKiApplicationID"] = FKiAppID;
                oLinks["OrganisationID"] = iOrgID;
                var oXiLinkDef = Connection.Select<XILink>("XILink_T", oLinks).ToList();
                foreach (var items in oXiLinkDef)
                {
                    XiLinks[items.Name] = items.Name;
                }
                oQSStep.XILinks = XiLinks;

                //AutoComplete For QSLink
                Dictionary<string, string> QSLink = new Dictionary<string, string>();
                Dictionary<string, object> oQSLinks = new Dictionary<string, object>();
                var QSLinkDef = Connection.Select<XIQSLink>("XIQSLinkDefinition_T", oQSLinks).ToList();
                var oQSLinkDef = QSLinkDef.ToList().Select(m => m.sCode).Distinct();
                foreach (var items in oQSLinkDef)
                {
                    QSLink[items] = items;
                }
                oQSStep.XICodes = QSLink;

                //FieldDefination DropDowns
                List<string> FieldValues = new List<string>();
                Dictionary<string, object> oFields = new Dictionary<string, object>();
                oFields["FKiApplicationID"] = FKiAppID;
                oFields["OrganisationID"] = iOrgID;
                oFields["FKiXIStepDefinitionID"] = ID;
                oFields["FKiStepSectionID"] = 0;
                var oFieldDef = Connection.Select<XIDFieldDefinition>("XIFieldDefinition_T", oFields).ToList();
                var lFieldDef = oFieldDef.Select(m => m.FKiXIFieldOriginID).ToList();
                foreach (var item in lFieldDef)
                {
                    var lXiFields = oFODef.Where(m => m.ID == item).ToList();
                    foreach (var items in lXiFields)
                    {
                        FieldValues.Add(items.sName);
                    }
                }
                oQSStep.XIFieldValues = FieldValues;

                //XILayout DropDowns
                List<XIDropDown> oLayoutDDL = new List<XIDropDown>();
                Dictionary<string, object> oLayout = new Dictionary<string, object>();
                oFO["FKiApplicationID"] = FKiAppID;
                oFO["OrganisationID"] = iOrgID;
                var oLayDef = Connection.Select<XIDLayout>("XILayout_T", oLayout).ToList();
                oLayoutDDL = oLayDef.Select(m => new XIDropDown { Value = m.ID, text = m.LayoutName }).ToList();
                oLayoutDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oQSStep.ddlLayouts = oLayoutDDL;
                oQSStep.ddlContent = new List<XIDropDown>();

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oQSStep;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }

        #endregion QSStepConfigComponent
        public CResult Get_QSStepTraceButtons()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get next trace buttons based on current step code";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sCode", sValue = sCode });
                if (!string.IsNullOrEmpty(sCode))//check mandatory params are passed or not
                {
                    XIIXI oXII = new XIIXI();
                    Dictionary<string, CNV> oCNV = new Dictionary<string, CNV>();
                    var sCurrentTrace = sCode;
                    var iCount = sCurrentTrace.Count(m => m == '_');
                    XID1Click o1Click = new XID1Click();
                    o1Click.BOIDXIGUID = new Guid("67F06BDE-3313-4705-B2A7-7088B6A68DCC");
                    o1Click.Query = "select * from refValidTrace_T where sname like '" + sCode + "%' and FKiQSDIDXIGUID='" + FKiQSDefintionIDXIGUID + "'";
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
                                    oParam.Add(new CNV { sName = "FKiQSDIDXIGUID", sValue = FKiQSDefintionIDXIGUID.ToString() });
                                    var oStageI = oXII.BOI("RefTraceStage", null, null, oParam);
                                    if (oStageI != null && oStageI.Attributes.Count() > 0)
                                    {
                                        var Name = oStageI.AttributeI("sDescription").sValue;
                                        var FKiXILinkIDGUID = oStageI.AttributeI("FKiXILinkIDXIGUID").sValue;
                                        oCNV.Add(Name, new CNV { sName = Name, sValue = FKiXILinkIDGUID });
                                    }
                                }
                            }
                        }
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = oCNV;
                    TraceButtons = oCNV;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param:  is missing";
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
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult Update_QSStepLifeCycle(string QSIID, string sCurrentStep, string sNextStep)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Update the QS step navigations";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "QSIID", sValue = QSIID });
                oTrace.oParams.Add(new CNV { sName = "sCurrentStep", sValue = sCurrentStep });
                oTrace.oParams.Add(new CNV { sName = "sNextStep", sValue = sNextStep });
                if (!string.IsNullOrEmpty(QSIID) && !string.IsNullOrEmpty(sCurrentStep) && !string.IsNullOrEmpty(sNextStep))//check mandatory params are passed or not
                {
                    XIInfraCache oCache = new XIInfraCache();
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "LifeCycle");
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("FKiQSIIDXIGUID", QSIID);
                    oBOI.SetAttribute("sFrom", sCurrentStep);
                    oBOI.SetAttribute("sTo", sNextStep);
                    oCR = oBOI.Save(oBOI);
                    if(oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = "Success";
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
                    oTrace.sMessage = "Mandatory Param:QSIID or sCurrentStep or sNextStep is missing";
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
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }

}