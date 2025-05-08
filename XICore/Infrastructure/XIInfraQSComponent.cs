using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XICore
{
    public class XIInfraQSComponent : XIDefinitionBase
    {
        public string QSUID { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sCurrentUserGUID { get; set; }
        public string sGUID { get; set; }
        XIInfraCache oCache = new XIInfraCache();
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

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
            try
            {
                string sSessionID = HttpContext.Current.Session.SessionID;
                QSUID = oParams.Where(m => m.sName == "iQSDID").Select(m => m.sValue).FirstOrDefault();
                int iQSDID = 0;
                Guid QSGUID = Guid.Empty;
                int.TryParse(QSUID, out iQSDID);
                Guid.TryParse(QSUID, out QSGUID);
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                sCurrentUserGUID = oParams.Where(m => m.sName == "sCurrentUserGUID").Select(m => m.sValue).FirstOrDefault();
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                int iOrgID = Convert.ToInt32(oParams.Where(m => m.sName.ToLower() == "iorgid").Select(m => m.sValue).FirstOrDefault());
                XIIQS oQSInstance = new XIIQS();
                if (iQSDID > 0 || (QSGUID != null && QSGUID != Guid.Empty))
                {
                    XIIXI oXII = new XIIXI();
                    //Get Question Set Object
                    XIIQS oQSI = new XIIQS();
                    XIDQS oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSUID.ToString(), null, null, 0, iOrgID);
                    //var oQSD = oDXI.Get_QSDefinition(null, iQSDID);
                    if (!oQSD.bInMemoryOnly)
                    {
                        CResult oQSResult = oXII.CreateQSI(null, QSUID, null, null, 0, 0, sCurrentUserGUID);
                        if (!oQSResult.bOK)
                        {
                            return oCResult;
                        }
                        oQSInstance = (XIIQS)oQSResult.oResult;
                    }
                    else
                    {
                        oQSInstance.FKiQSDefinitionID = oQSD.ID;
                        oQSInstance.FKiQSDefinitionIDXIGUID = oQSD.XIGUID;
                    }
                    //oQSD.FKiBOStructureID = 0;
                    if ((oQSD.FKiBOStructureID > 0 || (oQSD.FKiBOStructureIDXIGUID != null && oQSD.FKiBOStructureIDXIGUID != Guid.Empty)) && (oQSD.FKiParameterID > 0 || (oQSD.FKiParameterIDXIGUID != null && oQSD.FKiParameterIDXIGUID != Guid.Empty)))
                    {
                        var oStruct = (XIDStructure)oCache.GetObjectFromCache(XIConstant.CacheStructureCode, null, oQSD.FKiBOStructureIDXIGUID.ToString());
                        XIIXI oIXI = new XIIXI();
                        var ActiveBO = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}"); //oParams.Where(m => m.sName == "{XIP|ActiveBO}").Select(m => m.sValue).FirstOrDefault();
                        var Prm = "{-iInstanceID}";
                        var iBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, Prm);
                        if (!string.IsNullOrEmpty(ActiveBO) && !string.IsNullOrEmpty(iBOIID))
                        {
                            var oStructobj = oIXI.BOI(ActiveBO, iBOIID).Structure(oStruct.sCode).XILoad("partial");
                            var oXIParams = oCache.GetObjectFromCache(XIConstant.CacheXIParamater, null, oQSD.FKiParameterIDXIGUID.ToString());
                            if (oXIParams != null)
                            {
                                var oXIP = (XIParameter)oXIParams;
                                var oCopy = (XIParameter)oXIP.Clone(oXIP);
                                foreach (var param in oCopy.XiParameterNVs)
                                {
                                    if (param.Value.ToString().Contains('.') && !param.Value.StartsWith("xi.s"))
                                    {
                                        var Splitdot = param.Value.Split('.').ToList();
                                        var NodeData = oStructobj.oChildBOI(Splitdot[0]);
                                        XIIAttribute oAttrI = null;
                                        if (NodeData != null)
                                        {
                                            if (NodeData.FirstOrDefault().Attributes.TryGetValue(Splitdot[1], out oAttrI))
                                            {
                                                if (oAttrI != null)
                                                {
                                                    param.Value = oAttrI.sValue;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        XIInfraCache oCache = new XIInfraCache();
                                        string sResolvedValue = string.Empty;
                                        oCR = new CResult();
                                        oCR = oCache.ResolveMe(param.Value, sSessionID, sGUID);
                                        if (oCR.bOK)
                                        {
                                            sResolvedValue = (string)oCR.oResult;
                                            param.Value = sResolvedValue;
                                        }
                                    }
                                }
                                List<CNV> nSubParams = new List<CNV>();
                                nSubParams.AddRange(oCopy.XiParameterNVs.Select(m => new CNV { sName = m.Name, sValue = m.Value, sType = m.Type }).ToList());
                                oCache.SetXIParams(nSubParams, sGUID, sSessionID);
                                oCache.Set_ParamVal(sSessionID, sGUID, null, oCopy.Name, null, null, nSubParams);
                            }
                        }
                    }
                    oQSInstance.QSDefinition = oQSD;
                    //oQSInstance.oDefintion = oQSD;
                    XIIQSStep oStepI = new XIIQSStep();
                    oStepI.sGUID = sGUID;
                    oStepI.oDefintion = oQSD.Steps.Values.OrderBy(m => m.iOrder).FirstOrDefault();
                    var oStepIns = oStepI.Load();
                    if (oStepIns.bOK && oStepIns.oResult != null)
                    {
                        var sStepName = oQSD.Steps.Values.OrderBy(m => m.iOrder).FirstOrDefault().sName;
                        oQSInstance.Steps[sStepName] = (XIIQSStep)oStepIns.oResult;// (((XIInstanceBase)oStepIns.oResult).oContent[XIConstant.ContentStep]);
                        oQSInstance.iCurrentStepIDXIGUID = oQSD.Steps.Values.FirstOrDefault().XIGUID;
                        oQSInstance.sCurrentStepName = sStepName;
                        oQSInstance.Steps[oQSInstance.sCurrentStepName].bIsCurrentStep = true;
                        oQSInstance.sMode = oQSD.sMode;
                        oQSInstance.sGUID = sGUID;
                        oQSInstance.sCssClass = oQSD.sCssClass;
                        oQSInstance.sQSName = oQSD.sName;
                        var Secs = oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionIDXIGUID == oQSInstance.iCurrentStepIDXIGUID).Select(m => m.Sections).ToList();
                        foreach (var sec in Secs)
                        {
                            var XiValues = sec.Values.Select(m => m.XIValues).ToList();
                            foreach (var xivalue in XiValues)
                            {
                                foreach (var item in xivalue)
                                {
                                    oQSInstance.XIValues.Add(item.Key, item.Value);
                                }
                            }
                        }

                    }
                    //oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, 0);

                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|iQSInstanceID}", Convert.ToString(oQSInstance.XIGUID), "autoset", null);
                    //oQSInstance = oXIRepo.GetQuestionSetInstance(iQSDID, sGUID, null, 0, 0, iUserID, sOrgName, sDatabase, sCurrentUserGUID);
                }
                else
                {
                    oCR.sMessage = "Config Error: XIInfraQSComponent_XILoad() : QSDID is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                    oCR.sCode = "Config Error";
                    SaveErrortoDB(oCR);
                }
                if (oQSInstance.sMode == null || oQSInstance.sMode.ToLower() == "QuestionSet".ToLower())
                {
                    oCache.Set_QuestionSetCache("QuestionSetCache", sGUID, oQSInstance.XIGUID.ToString(), oQSInstance);
                    oQSInstance.sGUID = sGUID;
                    if (oQSInstance.HistoryXIGUID == null)
                    {
                        oQSInstance.HistoryXIGUID = new List<Guid>();
                    }
                    bool IsHistory = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oQSInstance.iCurrentStepIDXIGUID).Select(m => m.bIsHistory).FirstOrDefault();
                    if (oQSInstance.HistoryXIGUID.IndexOf(oQSInstance.iCurrentStepIDXIGUID) == -1 && IsHistory)
                    {
                        oQSInstance.HistoryXIGUID.Add(oQSInstance.iCurrentStepIDXIGUID);
                    }
                }
                else
                {
                    oQSInstance.QSDefinition = null;
                }
                oCResult.oResult = oQSInstance;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing QS Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
    }
}