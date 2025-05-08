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
using static XIDatabase.XIDBAPI;
using XISystem;

namespace XICore
{
    public class XIIQSStep : XIInstanceBase
    {
        public int ID { get; set; }
        public int FKiQSInstanceID { get; set; }
        public Guid FKiQSInstanceIDXIGUID { get; set; }
        public int FKiQSStepDefinitionID { get; set; }
        public Guid FKiQSStepDefinitionIDXIGUID { get; set; }
        public Guid XIGUID { get; set; }
        [DapperIgnore]
        public bool bIsCurrentStep { get; set; }
        [DapperIgnore]
        public string sGUID { get; set; }
        [DapperIgnore]
        public List<Guid> FieldOriginIDs { get; set; }
        public Guid iOverrideStepXIGUID { get; set; }
        public int iOverrideStep { get; set; }
        public int FKiOrgID { get; set; }
        [DapperIgnore]
        public string sCssClass { get; set; }
        //public string sMessage { get; set; }
        private Dictionary<string, string> oMyXIMessages = new Dictionary<string, string>();

        public Dictionary<string, string> XiMessages
        {
            get
            {
                return oMyXIMessages;
            }
            set
            {
                oMyXIMessages = value;
            }
        }
        private Dictionary<string, List<string>> oMyXIPreMessages = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> XiPreMessages
        {
            get
            {
                return oMyXIPreMessages;
            }
            set
            {
                oMyXIPreMessages = value;
            }
        }

        private Dictionary<string, XIIValue> oMyXIValues = new Dictionary<string, XIIValue>();

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
        private List<CNV> oMyTraces = new List<CNV>();

        public List<CNV> Traces
        {
            get
            {
                return oMyTraces;
            }
            set
            {
                oMyTraces = value;
            }
        }
        public XIIValue XIValueI(string sValueName)
        {
            XIIValue oThisValueI = null/* TODO Change to default(_) if this is not a reference type */;

            // Ravi should have done a lot of this


            sValueName = sValueName.ToLower();


            if (oMyXIValues.ContainsKey(sValueName))
            {
            }
            else
            {
            }

            return oThisValueI;
        }

        private Dictionary<string, XIIQSSection> oMySections = new Dictionary<string, XIIQSSection>();

        public Dictionary<string, XIIQSSection> Sections
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
        public XIIQSSection SectionI(string sSectionName)
        {
            XIIQSSection oThisSectionI = null/* TODO Change to default(_) if this is not a reference type */;

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

            return oThisSectionI;
        }

        public CResult Load()
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
                //XIInstanceBase oInstBase = new XIInstanceBase();
                XIIQSStep oStepI = new XIIQSStep();
                XIInfraCache oCache = new XIInfraCache();
                XIDQSStep oStepD = new XIDQSStep();
                oStepD = (XIDQSStep)oDefintion;
                if (oStepD == null)
                {
                    if (FKiQSStepDefinitionIDXIGUID != null && FKiQSStepDefinitionIDXIGUID != Guid.Empty)
                    {
                        var oStepDef = (XIDQSStep)oCache.GetObjectFromCache(XIConstant.CacheQSStep, null, FKiQSStepDefinitionIDXIGUID.ToString());
                        oStepD = oStepDef;
                    }
                    else if (FKiQSStepDefinitionID > 0)
                    {
                        var oStepDef = (XIDQSStep)oCache.GetObjectFromCache(XIConstant.CacheQSStep, null, FKiQSStepDefinitionID.ToString());
                        oStepD = oStepDef;
                    }
                }
                XIDQSStep oStepC = new XIDQSStep();
                oStepC = (XIDQSStep)oStepD.Clone(oStepD);
                if (oStepC.iLayoutID > 0 || (oStepC.iLayoutIDXIGUID != null && oStepC.iLayoutIDXIGUID != Guid.Empty))
                {
                    XIDLayout oLayoutD = new XIDLayout();
                    oLayoutD.ID = oStepC.iLayoutID;
                    oLayoutD.XIGUID = oStepC.iLayoutIDXIGUID;
                    oLayoutD.sGUID = sGUID;
                    var oLayContent = oLayoutD.Load();
                    if (oLayContent.bOK && oLayContent.oResult != null)
                    {
                        oStepI.oContent[XIConstant.ContentLayout] = (XIDLayout)oLayContent.oResult;
                    }
                }
                else
                {
                    if (oStepC.Sections != null && oStepC.Sections.Count() > 0)
                    {
                        foreach (var sec in oStepC.Sections)
                        {
                            XIIQSSection oSecI = new XIIQSSection();
                            oSecI.oDefintion = sec.Value;
                            oSecI.sGUID = sGUID;
                            oSecI.sMode = "step";
                            var oSecContent = oSecI.Load();
                            if (oSecContent.bOK && oSecContent.oResult != null)
                            {
                                //oStepI.Sections[sec.Value.ID.ToString() + "_Sec"] = (XIIQSSection)(oSecContent.oResult);//.oContent[XIConstant.ContentSection];
                                oStepI.Sections[sec.Value.XIGUID.ToString() + "_Sec"] = (XIIQSSection)(oSecContent.oResult);//.oContent[XIConstant.ContentSection];
                            }
                        }
                    }
                    else
                    {
                        oStepI.Sections[0 + "_Sec"] = null;
                    }
                }
                oStepI.oDefintion = oStepC;
                oStepI.FKiQSStepDefinitionID = oStepC.ID;
                oStepI.FKiQSStepDefinitionIDXIGUID = oStepC.XIGUID;
                oStepI.iOverrideStep = oStepC.iOverrideStep;
                oStepI.sCssClass = oStepC.sCssClass;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                //oInstBase.oContent[XIConstant.ContentStep] = oStepI;
                oCResult.oResult = oStepI;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading step instance" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }


    }
}