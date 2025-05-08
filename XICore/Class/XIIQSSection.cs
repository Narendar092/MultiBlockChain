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
    public class XIIQSSection : XIInstanceBase
    {
        public int ID { get; set; }
        public int FKiStepSectionDefinitionID { get; set; }
        public int FKiStepInstanceID { get; set; }
        public Guid FKiStepInstanceIDXIGUID { get; set; }
        public int FKiFieldInstance { get; set; }
        public int FKiXIComponentInstance { get; set; }
        public int FKiOneClickInstance { get; set; }
        public Guid FKiStepSectionDefinitionIDXIGUID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiOrgID { get; set; }
        [DapperIgnore]
        public string sCssClass { get; set; }
        [DapperIgnore]
        public string sGUID { get; set; }
        [DapperIgnore]
        public string sMode { get; set; }

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
        private List<CNV> oMyFormValues = new List<CNV>();
        public List<CNV> FormValues
        {
            get
            {
                return oMyFormValues;
            }
            set
            {
                oMyFormValues = value;
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
                XIInfraCache oCache = new XIInfraCache();
                XIIQSSection oSecI = new XIIQSSection();
                XIDQSSection oSecD = new XIDQSSection();
                oSecD = (XIDQSSection)oDefintion;
                XIDQSSection oSecC = new XIDQSSection();
                oSecC = (XIDQSSection)oSecD.Clone(oSecD);
                oSecI.oDefintion = oSecD;
                if (oSecC.iXIComponentID > 0 || (oSecC.iXIComponentIDXIGUID != null && oSecC.iXIComponentIDXIGUID != Guid.Empty))
                {
                    XIIComponent oCompI = new XIIComponent();
                    XIDComponent oCompD = new XIDComponent();
                    oCompD = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, null, oSecC.iXIComponentIDXIGUID.ToString());
                    var oCompC = (XIDComponent)oCompD.Clone(oCompD);
                    if(oSecC.XIGUID != null && oSecC.XIGUID != Guid.Empty)
                    {
                        oCompC.GetComponentParams("section", oSecC.XIGUID.ToString());
                    }
                    else if(oSecC.ID > 0)
                    {
                        oCompC.GetComponentParams("section", oSecC.ID.ToString());
                    }                    
                    oCompC.ID = oSecD.iXIComponentID;
                    oCompI.oDefintion = oCompC;
                    oCompI.sGUID = sGUID;
                    oCompI.FKiStepDefinitonID = oSecC.FKiStepDefinitionID;
                    oCompI.FKiStepDefinitonIDXIGUID = oSecC.FKiStepDefinitionIDXIGUID;
                    oCompI.sCallHierarchy = "Step_" + oSecC.FKiStepDefinitionID + ":Sec_" + oSecC.ID + ":Component_" + oSecC.iXIComponentID;
                    var oCompContent = oCompI.Load();
                    if (oCompContent.bOK && oCompContent.oResult != null)
                    {
                        oSecI.oContent[XIConstant.ContentXIComponent] = oCompContent.oResult;
                    }
                }
                else if (!string.IsNullOrEmpty(oSecC.HTMLContent))
                {
                    oSecI.oContent[XIConstant.ContentHTML] = oSecC.HTMLContent;
                }
                else if (oSecC.iDisplayAs == 30)
                {
                    if (oSecC.FieldDefs != null && oSecC.FieldDefs.Count() > 0)
                    {
                        Dictionary<string, XIIValue> nSecFieldValues = new Dictionary<string, XIIValue>();
                        var Def = oSecC.FieldDefs.Values.OrderBy(m => m.ID).ToList();
                        oSecI = new XIIQSSection();
                        oSecI.FKiStepSectionDefinitionID = oSecC.ID;
                        foreach (var items in Def)
                        {
                            //.Select(m => new XIIValue { FKiFieldDefinitionID = m.ID }).ToList();
                            if (items.FieldOrigin.bIsMerge && !String.IsNullOrEmpty(items.FieldOrigin.sMergeField))
                            {
                                nSecFieldValues[items.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.ID) };
                                //nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = oQSInstance.XIIValues(def.FieldOrigin.sMergeField) };
                            }
                            else
                            {
                                nSecFieldValues[items.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.ID) };
                            }
                        }
                        oSecI.XIValues = nSecFieldValues;
                    }
                }
                oSecI.sGUID = sGUID;
                oSecI.FKiStepSectionDefinitionID = oSecD.ID;
                oSecI.FKiStepSectionDefinitionIDXIGUID = oSecC.XIGUID;
                oSecI.sCssClass = oSecC.sCssClass;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                //oInstBase.oContent[XIConstant.ContentSection] = oSecI;
                oCResult.oResult = oSecI;
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

        public CResult LoadV2()
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
                XIInfraCache oCache = new XIInfraCache();
                XIIQSSection oSecI = new XIIQSSection();
                XIDQSSection oSecD = new XIDQSSection();
                oSecD = (XIDQSSection)oDefintion;
                XIDQSSection oSecC = new XIDQSSection();
                oSecC = (XIDQSSection)oSecD.Clone(oSecD);
                oSecI.oDefintion = oSecD;
                if (oSecC.iXIComponentID > 0)
                {
                    XIIComponent oCompI = new XIIComponent();
                    XIDComponent oCompD = new XIDComponent();
                    oCompD = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, null, oSecC.iXIComponentID.ToString());
                    var oCompC = (XIDComponent)oCompD.Clone(oCompD);
                    oCompC.GetComponentParams("section", oSecC.ID.ToString());
                    oCompC.ID = oSecD.iXIComponentID;
                    oCompI.oDefintion = oCompC;
                    oCompI.sGUID = sGUID;
                    oCompI.FKiStepDefinitonID = oSecC.FKiStepDefinitionID;
                    oCompI.sCallHierarchy = "Step_" + oSecC.FKiStepDefinitionID + ":Sec_" + oSecC.ID + ":Component_" + oSecC.iXIComponentID;
                    if (sMode == "step")
                    {
                        oSecI.oContent[XIConstant.ContentXIComponent] = oCompD.sName;// oCompContent.oResult;
                    }
                    else
                    {
                        var oCompContent = oCompI.Load();
                        if (oCompContent.bOK && oCompContent.oResult != null)
                        {
                            oSecI.oContent[XIConstant.ContentXIComponent] = oCompContent.oResult;
                        }
                    }

                }
                else if (!string.IsNullOrEmpty(oSecC.HTMLContent))
                {
                    oSecI.oContent[XIConstant.ContentHTML] = oSecC.HTMLContent;
                }
                else if (oSecC.iDisplayAs == 30)
                {
                    if (oSecC.FieldDefs != null && oSecC.FieldDefs.Count() > 0)
                    {
                        Dictionary<string, XIIValue> nSecFieldValues = new Dictionary<string, XIIValue>();
                        var Def = oSecC.FieldDefs.Values.OrderBy(m => m.ID).ToList();
                        oSecI = new XIIQSSection();
                        oSecI.FKiStepSectionDefinitionID = oSecC.ID;
                        foreach (var items in Def)
                        {
                            //.Select(m => new XIIValue { FKiFieldDefinitionID = m.ID }).ToList();
                            if (items.FieldOrigin.bIsMerge && !String.IsNullOrEmpty(items.FieldOrigin.sMergeField))
                            {
                                nSecFieldValues[items.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.ID) };
                                //nSecFieldValues[def.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(def.ID), sValue = oQSInstance.XIIValues(def.FieldOrigin.sMergeField) };
                            }
                            else
                            {
                                nSecFieldValues[items.FieldOrigin.sName] = new XIIValue { FKiFieldDefinitionID = Convert.ToInt32(items.ID) };
                            }
                        }
                        oSecI.XIValues = nSecFieldValues;
                    }
                }
                oSecI.sGUID = sGUID;
                oSecI.FKiStepSectionDefinitionID = oSecD.ID;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                //oInstBase.oContent[XIConstant.ContentSection] = oSecI;
                oCResult.oResult = oSecI;
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
                XIDefinitionBase oDefinition = new XIDefinitionBase();
                XIDQSSection oSecD = new XIDQSSection();
                oSecD = (XIDQSSection)oDefintion;
                XIDQSSection oSecC = new XIDQSSection();
                oSecC = (XIDQSSection)oSecD.Clone(oSecD);
                if (oSecC.iXIComponentID > 0)
                {
                    XIDComponent oCompD = new XIDComponent();
                    oCompD = (XIDComponent)oSecC.ComponentDefinition.Clone(oSecC);
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oSecC.oContent[XIConstant.ContentXIComponent] = oCompD;
                    oDefinition.oContent[XIConstant.ContentSection] = oSecC;
                    oCResult.oResult = oDefinition;
                }
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
    }
}