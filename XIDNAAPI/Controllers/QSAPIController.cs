using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using XICore;
using XIDNA.Models;
using xiEnumSystem;
using XISystem;
using System.Web;
using System.Web.Script.Serialization;
using XIDNAAPI;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace XIAPI.Controllers
{
    [RoutePrefix("api/QSAPI")]
    public class QSAPIController : ApiController
    {
        [BasicAuthentication]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("CreateQS")]
        public HttpResponseMessage CreateQS(string QSD, string sUniqueRef)
        {
            var ResponseObj = new CNV();
            try
            {
                XIInfraCache xifCache = new XIInfraCache();
                //var sSessionID = HttpContext.Current.Session.SessionID;
                XIIQS oQSInstance = new XIIQS();
                string sGUID = Guid.NewGuid().ToString();
                XIIXI oIXI = new XIIXI();
                string iQSDID = "563250A3-4561-46B3-96D4-DD0776A386EC";
                if (!string.IsNullOrEmpty(QSD))
                {
                    iQSDID = QSD;
                }
                sUniqueRef = sUniqueRef.Trim();
                XIDQS oQSD = (XIDQS)xifCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSDID.ToString(), "", sGUID, 0, 0);
                XIDQS oQSDC = (XIDQS)oQSD.Clone(oQSD);
                var oCR = oIXI.CreateQSI(null, iQSDID, null, null, 0, 0, null, 0, null, 0, sGUID);
                oQSInstance = (XIIQS)oCR.oResult;
                oQSInstance.QSDefinition = oQSDC;
                Guid iActiveStepIDXIGUID = oQSDC.Steps.Values.ToList().OrderBy(m => m.iOrder).FirstOrDefault().XIGUID;
                oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, iActiveStepIDXIGUID.ToString(), sGUID);
                xifCache.Set_ParamVal("3124A96A-3F40-4079-91A8-A4425E4052C7", sGUID, null, "{XIP|iQSInstanceID}", oQSInstance.XIGUID.ToString(), "autoset", null);
                xifCache.Set_QuestionSetCache("QuestionSetCache", sGUID, oQSInstance.XIGUID.ToString(), oQSInstance);
                XIQSMedium oQSMedium = new XIQSMedium();
                oCR = oQSMedium.RenderResponse(oQSInstance, sGUID);
                if (oCR.bOK && oCR.oResult != null)
                {
                    ResponseObj = (CNV)oCR.oResult;
                    if (!string.IsNullOrEmpty(sUniqueRef))
                    {
                        //PC to create Lead
                        string sSessionID = Guid.NewGuid().ToString();
                        XIDAlgorithm oAlgoD = new XIDAlgorithm();
                        string sNewGUID = Guid.NewGuid().ToString();
                        List<CNV> oNVsList = new List<CNV>();
                        oNVsList.Add(new CNV { sName = "-iQSIID", sValue = oQSInstance.ID.ToString() });
                        oNVsList.Add(new CNV { sName = "-sMobileNo", sValue = sUniqueRef });
                        xifCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                        oAlgoD = (XIDAlgorithm)xifCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, "1347eb8e-8166-4088-9fa8-ee6e8c1d371d");
                        oCR = oAlgoD.Execute_XIAlgorithm(sSessionID, sNewGUID);
                    }
                    //XIDFormatMapping ResponseObj = new XIDFormatMapping();
                    var json = JsonConvert.SerializeObject(ResponseObj);
                    //var json = new JavaScriptSerializer().Serialize(ResponseObj);
                    return Request.CreateResponse(HttpStatusCode.OK, ResponseObj, Configuration.Formatters.JsonFormatter);
                }
                return Request.CreateResponse(HttpStatusCode.OK, ResponseObj, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                ResponseObj.sName = "Failure" + ex.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, ResponseObj, Configuration.Formatters.JsonFormatter);
            }
        }

        [BasicAuthentication]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("OneQ")]
        public HttpResponseMessage OneQ(string QID)
        {
            List<CNV> ResponseObj = new List<CNV>();
            XIInfraCache xifCache = new XIInfraCache();
            try
            {
                XID1Click o1ClickD = new XID1Click();
                o1ClickD = (XID1Click)xifCache.GetObjectFromCache(XIConstant.Cache1Click, null, QID);
                if (o1ClickD.iAPI == 10)
                {
                    Dictionary<string, XIIBO> oResult = new Dictionary<string, XIIBO>();
                    oResult = o1ClickD.OneClick_Execute();
                    if (oResult != null && oResult.Values.Count() > 0)
                    {
                        foreach (var boi in oResult.Values.ToList())
                        {
                            CNV oNV = new CNV();
                            oNV.sName = boi.AttributeI("sname").sValue;
                            oNV.sValue = boi.AttributeI("xiguid").sValue;
                            ResponseObj.Add(oNV);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ResponseObj.sName = "Failure" + ex.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, ResponseObj, Configuration.Formatters.JsonFormatter);
            }
            return Request.CreateResponse(HttpStatusCode.OK, ResponseObj, Configuration.Formatters.JsonFormatter);
        }

        //[BasicAuthentication]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ResponseQS")]
        public HttpResponseMessage ResponseQS([FromBody] CNV oQSNV)
        {
            //var json = new JavaScriptSerializer().Deserialize<CNV>(sJsonData);
            var ResponseObj = new CNV();
            try
            {
                CResult oCR = new CResult();
                oCR = API_Inbound(oQSNV);
                if (oCR.bOK && oCR.oResult != null)
                {
                    var oQSI = (XIIQS)oCR.oResult;
                    XIQSMedium oQSMedium = new XIQSMedium();
                    var sType = oQSNV.sType;
                    string sGUID = sType.Split('|')[1];
                    oCR = oQSMedium.RenderResponse(oQSI, sGUID);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        ResponseObj = (CNV)oCR.oResult;
                        //XIDFormatMapping ResponseObj = new XIDFormatMapping();
                        var json = JsonConvert.SerializeObject(ResponseObj);
                        //var json = new JavaScriptSerializer().Serialize(ResponseObj);
                        return Request.CreateResponse(HttpStatusCode.OK, ResponseObj, Configuration.Formatters.JsonFormatter);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ResponseObj.sName = "Failure-" + ex.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, ResponseObj, Configuration.Formatters.JsonFormatter);
            }

        }

        public CResult API_Inbound(CNV oQSNV)
        {
            XIInfraCache oCache = new XIInfraCache();
            int iQSIID = 0;
            Guid QSIGUID = Guid.Empty;
            int.TryParse(oQSNV.sValue, out iQSIID);
            Guid.TryParse(oQSNV.sValue, out QSIGUID);
            var sType = oQSNV.sType;
            XIIQS oQSI = new XIIQS();
            string sGUID = sType.Split('|')[1];
            if (oQSNV.NNVs != null && oQSNV.NNVs.Count() > 0)
            {
                foreach (var oStepI in oQSNV.NNVs.Values.ToList())
                {
                    XIIQSStep oQSStepI = new XIIQSStep();
                    if (oStepI.NNVs != null && oStepI.NNVs.Count() > 0)
                    {
                        foreach (var oSecI in oStepI.NNVs.Values.ToList())
                        {
                            XIIQSSection oSectionI = new XIIQSSection();
                            var sSecIKey = oSecI.sContext.Split('|')[0];
                            var SecDefXIGUID = oSecI.sContext.Split('|')[1];
                            if (oSecI.NNVs != null && oSecI.NNVs.Count() > 0)
                            {
                                foreach (var oXIVal in oSecI.NNVs.Values.ToList())
                                {
                                    if (!string.IsNullOrEmpty(oXIVal.sType) && oXIVal.sType == "XIValue")
                                    {
                                        XIIValue oXIValue = new XIIValue();
                                        var FieldDefXIGUID = oXIVal.sContext.Split('|')[0];
                                        var FieldOriginXIGUID = oXIVal.sContext.Split('|')[1];
                                        var oFieldOriginD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, FieldOriginXIGUID);
                                        oXIValue.sDisplayName = oFieldOriginD.sName;
                                        oXIValue.FKiFieldOriginIDXIGUID = oFieldOriginD.XIGUID;
                                        oXIValue.FKiFieldDefinitionIDXIGUID = new Guid(FieldDefXIGUID);
                                        oXIValue.sValue = oXIVal.sValue;
                                        //oXIValue.FKiQSSectionDefinitionIDXIGUID = new Guid(oSecI.sContext);
                                        oXIValue.FKiQSStepDefinitionIDXIGUID = new Guid(oStepI.sContext);
                                        oXIValue.FKiQSInstanceIDXIGUID = QSIGUID;
                                        oSectionI.XIValues[oXIVal.sName] = oXIValue;
                                        oSectionI.FKiStepSectionDefinitionIDXIGUID = new Guid(SecDefXIGUID);
                                    }
                                }
                            }
                            //oSectionI.FKiStepSectionDefinitionIDXIGUID = new Guid(oSecI.sContext);
                            oQSStepI.Sections[sSecIKey] = oSectionI;
                        }
                    }
                    oQSStepI.FKiQSStepDefinitionIDXIGUID = new Guid(oStepI.sContext);
                    oStepI.sName = oStepI.sName;
                    oQSStepI.FKiQSInstanceID = iQSIID;
                    oQSStepI.FKiQSInstanceIDXIGUID = QSIGUID;
                    oQSI.FKiQSDefinitionIDXIGUID = new Guid(oQSNV.sContext);
                    oQSI.XIGUID = QSIGUID;
                    oQSStepI.bIsCurrentStep = true;
                    oQSI.Steps[oStepI.sName] = oQSStepI;
                }
            }
            var oCR = GetNextStep_Whatsapp(oQSI, sGUID, "API", oQSI.XIGUID.ToString());
            if (oCR.bOK && oCR.oResult != null)
            {
                var oQSRes = (XIIQS)oCR.oResult;
                oCR.oResult = oQSRes;
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                return oCR;
            }
            return (CResult)oCR.oResult;
        }


        public CResult GetNextStep_Whatsapp(XIIQS oQSI, string sGUID, string sType, string iQSIID = "")
        {
            List<string> Info = new List<string>();
            CResult oCR = new CResult();
            try
            {
                XIIXI oIXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                XIIQS oQSInstance = new XIIQS();
                XIIQSStep oStepI = oQSI.Steps.Values.ToList().LastOrDefault();
                var oQSIns = oIXI.GetQSInstanceByID(iQSIID.ToString());
                oQSInstance = (XIIQS)oCache.Get_QuestionSetCache("QuestionSetCache", sGUID, iQSIID);
                if (oQSInstance.QSDefinition == null)
                {
                    Info.Add("sGUID " + sGUID);
                    Info.Add("iQSIID " + iQSIID);
                    oQSInstance = oIXI.GetQuestionSetInstanceByID("0", iQSIID.ToString(), null, 0, 0, null);
                }
                XIDQS oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, oQSInstance.FKiQSDefinitionIDXIGUID.ToString(), null, null, 0, 0);
                XIDQS oQSDC = (XIDQS)oQSD.Clone(oQSD);
                oQSInstance.QSDefinition = oQSDC;
                var oStepD = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oStepI.FKiQSStepDefinitionIDXIGUID).FirstOrDefault();
                if (oQSInstance.iCurrentStepIDXIGUID == Guid.Empty || oQSInstance.iCurrentStepIDXIGUID == null)
                {
                    oQSInstance.iCurrentStepIDXIGUID = oStepD.XIGUID;
                }
                if (oQSInstance.Steps.ContainsKey(oStepD.sName))
                {
                    oQSInstance.Steps[oStepD.sName] = oStepI;
                    foreach (var Section in oQSInstance.Steps[oStepD.sName].Sections)
                    {
                        foreach (var Field in Section.Value.XIValues)
                        {
                            oQSInstance.XIValues[Field.Key].sValue = Field.Value.sValue;
                        }
                    }
                }
                //oQSInstance.Steps.Values.Where(m=>m.ID == oStepI.ID).FirstOrDefault()
                string sCurrentGuestUser = string.Empty;
                var sSessionID = Guid.NewGuid().ToString();// HttpContext.Current.Session.SessionID;
                oCache.Set_ParamVal(sSessionID, sGUID, "", "-iQSIID", iQSIID.ToString(), "", null);
                //var CurrentStepID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, "CurrentStepID_" + oQSInstance.FKiQSDefinitionID));
                var CurrentStepID = oQSInstance.iCurrentStepIDXIGUID;
                var LastStepID = oQSInstance.QSDefinition.Steps.Select(m => m.Value.XIGUID).LastOrDefault();
                bool IsSaveStep = true;
                if (oStepD.iLockStage != 0 && oStepD.iLockStage <= oQSInstance.iStage && oQSInstance.QSDefinition.bIsStage)
                {
                    IsSaveStep = false;
                }
                //Checking Database Save type for Question Set
                if (oQSInstance.QSDefinition.SaveType.ToLower() == "Save at end".ToLower() && CurrentStepID == LastStepID && IsSaveStep)
                {
                    if (oQSInstance.QSDefinition.Steps.Where(m => m.Value.XIGUID == CurrentStepID).Select(m => m.Value.bInMemoryOnly).FirstOrDefault() == false)
                    {
                        oQSI.API_SaveQSInstances(oQSInstance, sGUID, sSessionID);
                    }
                }
                else if (oQSInstance.QSDefinition.SaveType.ToLower() == "Save as Populated".ToLower() && IsSaveStep)
                {
                    if (oQSInstance.QSDefinition.Steps.Where(m => m.Value.XIGUID == CurrentStepID).Select(m => m.Value.bInMemoryOnly).FirstOrDefault() == false)
                    {
                        oQSI.API_SaveQSInstances(oQSInstance, sGUID, sSessionID);
                    }
                }
                XIDScript oXIScript = new XIDScript();
                var oCurrentStep = oQSInstance.QSDefinition.Steps.Where(m => m.Value.XIGUID == CurrentStepID).Select(m => m.Value).FirstOrDefault();
                var oRefStageDef = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "RefTraceStage");
                oRefStageDef.sNameAttribute = "sName";
                string sStageName = string.Empty;
                var CurrentOrder = oQSInstance.QSDefinition.Steps.Where(m => m.Value.XIGUID == CurrentStepID).Select(m => m.Value.iOrder).FirstOrDefault();
                //oQSInstance.nStepInstances = oQSInstance.nStepInstances;
                int NextStepID = 0;
                //Checking Navigations of a step to decide next step
                if (oCurrentStep.Navigations != null && oCurrentStep.Navigations.Count() > 0)
                {
                    foreach (var Navs in oCurrentStep.Navigations)
                    {
                        if (NextStepID == 0)
                        {
                            var Nav = Navs.Value;
                            if (Nav.sField != null && Nav.sOperator != null && Nav.sValue != null)
                            {
                                var oStepIns = oQSInstance.Steps.Where(m => m.Value.FKiQSStepDefinitionIDXIGUID == CurrentStepID).FirstOrDefault();
                                var StepDef = oQSInstance.QSDefinition.Steps.Where(m => m.Value.XIGUID == oStepIns.Value.FKiQSStepDefinitionIDXIGUID).FirstOrDefault();
                                var FieldDef = StepDef.Value.FieldDefs.Where(m => m.Value.FieldOrigin.sName == Nav.sField).FirstOrDefault();
                                var FieldValue = oStepIns.Value.XIValues.Where(m => m.Value.FKiFieldDefinitionIDXIGUID == FieldDef.Value.XIGUID).FirstOrDefault();
                                //var Result = EvaluateExpression(FieldValue.Value, Nav.sOperator, Nav.sValue, FieldDef.FieldOrigin);
                                //if (Result)
                                //{
                                //    NextStepID = Nav.iNextStepID;
                                //}//Commented
                            }
                        }
                    }
                }
                //Getting Next Step
                var NextSteps = new List<XIDQSStep>();
                var NextStep = new XIDQSStep();
                string sNextStep = oCache.Get_ParamVal(sSessionID, sGUID, null, "NextStep");
                string sIsQsLoad = oCache.Get_ParamVal(sSessionID, sGUID, null, "IsQsLoad");
                Info.Add("NextStep from Cache: " + sNextStep);
                //Common.SaveErrorLog("sNextStep:" + sNextStep, "XIDNA");
                if (!string.IsNullOrEmpty(sNextStep))
                {
                    NextStep = oQSInstance.QSDefinition.StepD(sNextStep);
                    if (NextStep != null)
                    {
                        NextStepID = NextStep.ID;
                    }
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", "", null, null);
                }

                if (NextStepID == 0)
                {
                    NextSteps = oQSInstance.QSDefinition.Steps.Values.OrderBy(m => m.iOrder).Where(m => m.iOrder > CurrentOrder).ToList();
                    NextStep = NextSteps.FirstOrDefault();
                }
                else
                {
                    NextStep = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == NextStepID).FirstOrDefault();
                    NextSteps = oQSInstance.QSDefinition.Steps.Values.OrderBy(m => m.iOrder).Where(m => m.iOrder > NextStep.iOrder).ToList();
                }
                //Getting Next Step Instance
                Guid iActiveStepID = Guid.Empty;
                if (NextStep != null)
                {
                    iActiveStepID = oQSInstance.GetActiveStepID(NextStep.XIGUID.ToString(), sGUID);
                    var oNextStep = oQSInstance.QSDefinition.Steps.Where(m => m.Value.XIGUID == iActiveStepID).Select(m => m.Value).FirstOrDefault();
                    oQSInstance.iCurrentStepIDXIGUID = iActiveStepID;
                    var oStep = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oQSInstance.iCurrentStepIDXIGUID).FirstOrDefault();
                    if (oQSIns.iStage < oStep.iStage)
                    {
                        oQSInstance.iStage = oStep.iStage;
                    }
                    oQSInstance.sCurrentStepName = oStep.sName;
                    //XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                    //oQSInstance = Connection.Update<XIIQS>(oQSInstance, "XIQSInstance_T", "XIGUID");

                    if (!string.IsNullOrEmpty(sIsQsLoad))
                    {
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "IsQsLoad", "", null, null);
                    }
                    if (oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionIDXIGUID == iActiveStepID).FirstOrDefault() == null || oNextStep.bIsReload)
                    {
                        oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, iActiveStepID.ToString(), sGUID);
                    }
                    else
                    {
                        oQSInstance.Steps.Values.ToList().Where(m => m.FKiQSStepDefinitionIDXIGUID == iActiveStepID).FirstOrDefault().bIsCurrentStep = true;
                    }
                    oQSInstance.Steps.Values.ToList().ForEach(m => m.bIsCurrentStep = false);
                    oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionIDXIGUID == iActiveStepID).FirstOrDefault().bIsCurrentStep = true;
                    oQSInstance.iCurrentStepIDXIGUID = iActiveStepID;
                    oQSInstance.sCurrentStepName = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == iActiveStepID).Select(m => m.sName).FirstOrDefault();// NextStep.sName;
                    Info.Add("Step Execution: Started " + oQSInstance.sCurrentStepName + " Step");
                    //Common.SaveErrorLog("Step Execution: Started " + oQSInstance.sCurrentStepName + " Step", "XIDNA");
                    //oQSInstance.oDynamicObject = oDynamicObject;
                    //oQSInstance.sHtmlPage = sHtmlPage;
                    var oStepMessage = oCache.Get_ObjectSetCache("StepMessage", sGUID, sSessionID);//(sSessionID, sGUID, null, "StepMessage");
                    int oDecision = Convert.ToInt32(oCache.Get_ObjectSetCache("Decision", sGUID, sSessionID));
                    oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionIDXIGUID == iActiveStepID).FirstOrDefault().XiMessages = new Dictionary<string, string>();
                    if (oStepMessage != null)
                    {
                        if (NextStep != null)
                        {
                            Dictionary<string, string> dictmsgs = new Dictionary<string, string>();
                            dictmsgs = (Dictionary<string, string>)oStepMessage;
                            oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionIDXIGUID == iActiveStepID).FirstOrDefault().XiMessages = dictmsgs;
                            if (oDecision == 3 || oDecision == 5 || oDecision == 6)
                            {
                                oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == iActiveStepID).FirstOrDefault().bIsBack = true;
                            }
                            else if (oDecision == 1 || oDecision == 2 || oDecision == 4)
                            {
                                oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == iActiveStepID).FirstOrDefault().bIsBack = false;
                            }
                            HttpRuntime.Cache.Remove("StepMessage" + "_" + sGUID + "_" + sSessionID);
                        }
                    }
                    var oStepPrevalition = oCache.Get_ObjectSetCache("StepPreValidationMessage", sGUID, sSessionID);
                    oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionIDXIGUID == iActiveStepID).FirstOrDefault().XiPreMessages = new Dictionary<string, List<string>>();
                    if (oStepPrevalition != null)
                    {
                        if (NextStep != null)
                        {
                            Dictionary<string, List<string>> dictmsgs = new Dictionary<string, List<string>>();
                            dictmsgs = (Dictionary<string, List<string>>)oStepPrevalition;
                            oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionIDXIGUID == iActiveStepID).FirstOrDefault().XiPreMessages = dictmsgs;
                            HttpRuntime.Cache.Remove("StepPreValidationMessage" + "_" + sGUID + "_" + sSessionID);
                        }
                    }
                    //oQSInstanceNew.nStepInstances = oQSInstance.nStepInstances;
                    //oCache.Set_ParamVal(sSessionID, sGUID, "CurrentStepID_" + oQSInstance.FKiQSDefinitionID, NextStep.ID.ToString());
                    //oCache.UpdateCacheObject("QuestionSet", sGUID, oQSInstance, sDatabase, oQSInstance.FKiQSDefinitionID);
                }
                else
                {
                }
                bool IsHistory = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == iActiveStepID).Select(m => m.bIsHistory).FirstOrDefault();
                if (oQSInstance.HistoryXIGUID == null)
                {
                    oQSInstance.HistoryXIGUID = new List<Guid>();
                }
                if (oQSInstance.HistoryXIGUID.IndexOf(oQSInstance.iCurrentStepIDXIGUID) == -1 && IsHistory)
                {
                    oQSInstance.HistoryXIGUID.Add(oQSInstance.iCurrentStepIDXIGUID);
                }
                oCache.Set_QuestionSetCache("QuestionSetCache", sGUID, oQSInstance.XIGUID.ToString(), oQSInstance);
                oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|iQSInstanceID}", Convert.ToString(oQSInstance.XIGUID), "autoset", null);
                string IsOverRideQuote = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|IsOverRideQuote}");
                int LastStep = oQSInstance.QSDefinition.Steps.Values.Where(m => m.sName.ToLower() == "Your Quotes".ToLower()).Select(m => m.ID).FirstOrDefault();
                var iCurrentStepID = oQSInstance.iCurrentStepIDXIGUID;
                var oCurrentStepD = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == iCurrentStepID).FirstOrDefault();
                //if (oCurrentStepD != null)
                //{
                //    Dictionary<string, XIDQSStep> Steps = new Dictionary<string, XIDQSStep>();
                //    foreach (var Step in oQSDC.Steps.Values.ToList())
                //    {
                //        Steps[Step.sName] = new XIDQSStep() { ID = Step.ID, XIGUID = Step.XIGUID, sName = Step.sName, iOrder = Step.iOrder, sDisplayName = Step.sDisplayName, sIsHidden = Step.sIsHidden, iStage = Step.iStage, iLockStage = Step.iLockStage, iCutStage = Step.iCutStage };
                //    }
                //    oQSInstance.QSDefinition.Steps = Steps;
                //    oQSInstance.QSDefinition.Steps[oCurrentStepD.sName] = oCurrentStepD;
                //}
                oCR.oResult = oQSInstance;
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                string sMedium = string.Empty;
                if (sType.ToLower() == "public")
                {
                    Info.Add("Step Execution: Returned " + oQSInstance.sCurrentStepName + " Step");
                    //Common.SaveErrorLog("Step Execution: Returned " + oQSInstance.sCurrentStepName + " Step", "XIDNA");
                    //return PartialView("_QuestionSet", oQSInstance);
                }
                else if (sType.ToLower() == "api")
                {
                    var sFinalString = string.Empty;
                    switch (sMedium.ToLower())
                    {
                        //TO DO: Whatsapp etc handle other kinds of APIs or switching between mediums at runtime
                        case "whatsapp":
                            //XIQSMedium oQSMedium = new XIQSMedium();
                            //CResult oCR = oQSMedium.RenderResponse(oQSInstance);
                            //if (oCR.bOK && oCR.oResult != null)
                            //{
                            //    CNV oResponse = (CNV)oCR.oResult;
                            //    oCR = oQSMedium.PrintToText(oResponse);
                            //    if (oCR.bOK && oCR.oResult != null)
                            //    {
                            //        var sFinalString = (string)oCR.oResult;
                            //        return PartialView("_FAInbox", sFinalString);
                            //    }
                            //}
                            break;
                        default:
                            //XIQSMedium oQSMedium = new XIQSMedium();
                            //var oCResponse = oQSMedium.RenderResponse(oQSInstance, sGUID);
                            //oCR.oResult = (CNV)oCResponse.oResult;
                            //if (oCR.bOK && oCR.oResult != null)
                            //{
                            //    CNV oResponse = (CNV)oCR.oResult;
                            //    oCR = oQSMedium.PrintToText(oResponse);
                            //    if (oCR.bOK && oCR.oResult != null)
                            //    {
                            //        sFinalString = (string)oCR.oResult;
                            //    }
                            //}
                            break;
                    }
                    //return PartialView("_FAInbox", sFinalString);
                }
                else
                {
                    //return PartialView("_QuestionSetInternal", oQSInstance);
                }

            }
            catch (Exception ex)
            {
                string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                //return PartialView("~/views/Shared/Error.cshtml");
            }
            return oCR;
        }
    }
}
