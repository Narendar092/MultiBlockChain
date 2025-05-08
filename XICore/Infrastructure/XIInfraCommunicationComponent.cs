using Microsoft.AspNet.SignalR.Messaging;
using Newtonsoft.Json;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using XISystem;

namespace XICore
{
    public class XIInfraCommunicationComponent : XIDefinitionBase
    {
        public CResult Load(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Generic communication component";//expalin about this method logic            
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var iInstanceID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_InstanceID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sSessionID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SessionID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_GUID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                oTrace.oParams.Add(new CNV { sName = "iInstanceID", sValue = iInstanceID });
                var CacheRateValue = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|-iRateLimit}");
                var CacheRateTime = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|-dtRateTime}");
                int iRateLimit = string.IsNullOrEmpty(CacheRateValue) ? 0 : Convert.ToInt32(CacheRateValue);
                double TotalMins = 0;
                if (CacheRateTime == null)
                {

                }
                else
                {
                    DateTime dtStartRateTime = string.IsNullOrEmpty(CacheRateTime) ? DateTime.Now : Convert.ToDateTime(CacheRateTime);
                    var CurrentDateTime = DateTime.Now;
                    TimeSpan span = CurrentDateTime.Subtract(dtStartRateTime);
                    TotalMins = span.TotalMinutes;
                }
                XIIXI oXI = new XIIXI();
                if (!string.IsNullOrEmpty(iInstanceID))//check mandatory params are passed or not
                {
                    var oStreamI = oXI.BOI("XICommunicationStream", iInstanceID);
                    if (oStreamI != null && oStreamI.Attributes.Count() > 0)
                    {
                        var QueryGUID = Guid.Empty;
                        var Query = oStreamI.AttributeI("FKi1QueryIDXIGUID").sValue;
                        var PCGUID = oStreamI.AttributeI("FKiProcessControllerIDXIGUID").sValue;
                        var iMaxPerminute = 0;
                        int.TryParse(oStreamI.AttributeI("iMaxPerminute").sValue, out iMaxPerminute);
                        var iMaxPerHour = 0;
                        int.TryParse(oStreamI.AttributeI("iMaxPerHour").sValue, out iMaxPerHour);
                        var iMaxPerDay = 0;
                        int.TryParse(oStreamI.AttributeI("iMaxPerDay").sValue, out iMaxPerDay);
                        var iStatus = 0;
                        int.TryParse(oStreamI.AttributeI("iStatus").sValue, out iStatus);
                        var sTestTo = oStreamI.AttributeI("sTestTo").sValue;
                        var sFromOverride = oStreamI.AttributeI("sFromOverride").sValue;
                        var RefCommID = oStreamI.AttributeI("FKiRefComTypeXIGUID").sValue;
                        Guid RefCommIDGUID = Guid.Empty;
                        if (Guid.TryParse(RefCommID, out RefCommIDGUID))
                        {
                            var RefCommI = oXI.BOI("XIrefComType", RefCommID);
                            if (RefCommI != null && RefCommI.Attributes.Count() > 0)
                            {
                                var iMaxRetry = RefCommI.AttributeI("iMaxRetry").sValue;
                                int iRetryMax = Convert.ToInt32(iMaxRetry);
                                var sSendMode = RefCommI.AttributeI("iSendMode").sValue;
                                int iSendMode = 0;
                                int.TryParse(sSendMode, out iSendMode);
                                if (iSendMode > 0)
                                {
                                    var APIGUID = Guid.Empty;
                                    var API = RefCommI.AttributeI("FKiAPIIDXIGUID").sResolvedValue;
                                    if (Guid.TryParse(API, out APIGUID))
                                    {
                                        var oAPI = oXI.BOI("XIAPI Settings", APIGUID.ToString());
                                        if (oAPI != null && oAPI.Attributes.Count() > 0)
                                        {
                                            var sAPI = oAPI.AttributeI("sname").sValue;
                                            if (!string.IsNullOrEmpty(sAPI))
                                            {
                                                var AccountGUID = Guid.Empty;
                                                var XIAccount = RefCommI.AttributeI("FKiXIAccountIDXIGUID").sValue;
                                                if (Guid.TryParse(XIAccount, out AccountGUID))
                                                {
                                                    XIDAccount oAccount = (XIDAccount)oCache.GetObjectFromCache(XIConstant.CacheXIAccount, null, AccountGUID.ToString());
                                                    if (Guid.TryParse(Query, out QueryGUID))
                                                    {
                                                        XID1Click o1ClickC = new XID1Click();
                                                        var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, QueryGUID.ToString());
                                                        o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                                                        var oResult = o1ClickD.OneClick_Run();
                                                        if (oResult != null && oResult.Count() > 0)
                                                        {
                                                            foreach (var CommI in oResult.Values.ToList())
                                                            {
                                                                var CommID = CommI.AttributeI("id").sValue;
                                                                if (!string.IsNullOrEmpty(CommID))//check mandatory params are passed or not
                                                                {
                                                                    List<CNV> oCommParams = new List<CNV>();
                                                                    oCommParams.Add(new CNV { sName = "StreamID", sValue = iInstanceID });
                                                                    oCommParams.Add(new CNV { sName = XIConstant.Param_InstanceID, sValue = CommID });
                                                                    oCommParams.Add(new CNV { sName = XIConstant.Param_SessionID, sValue = sSessionID });
                                                                    oCR = CommunicationOut(oCommParams);
                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                                                        oCResult.sMessage = "CommunicationOut method executed successfully";
                                                                    }
                                                                    else
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                        oCResult.sMessage = "CommunicationOut method retruned Error";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                    oCResult.sMessage = "CommID for XICommunicationI is received null";
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                            oCResult.sMessage = "1Query in Stream got 0 results for 1Query:" + QueryGUID;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                        oCResult.sMessage = "Query is not configured for Stream instance :" + iInstanceID;
                                                    }
                                                }
                                                else
                                                {
                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.sMessage = "Account is not configured for communication type:" + RefCommIDGUID;
                                                }
                                            }
                                            else
                                            {
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.sMessage = "API not configured for API instance:" + APIGUID;
                                            }
                                        }
                                        else
                                        {
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.sMessage = "API instance loading failed for:" + APIGUID;
                                        }
                                    }
                                    else
                                    {
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.sMessage = "API is not configured for communication type:" + RefCommIDGUID;
                                    }
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Send mode is not configured for communication type:" + RefCommIDGUID;
                                }

                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.sMessage = "XIrefComType instance loading failed for:" + RefCommIDGUID;
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "FKiComTypeID or FKiComTypeGUID is not configured for instance:" + iInstanceID;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "StreamI loading failed for instance:" + iInstanceID;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iInstanceID " + iInstanceID + " is missing";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.sMessage = "Mandatory Param: iInstanceID " + iInstanceID + " is missing";
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + oCResult.sMessage;
                    SaveErrortoDB(oCResult);
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult CommunicationOut(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            List<CNV> oTraceInfo = new List<CNV>();
            oTrace.sTask = "communication out event, sends Email or SMS or WhatsApp to user";//expalin about this method logic            
            try
            {
                oTraceInfo.Add(new CNV { sValue = "Class is XIInfraCommunicationComponent and Method is CommunicationOut" });
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                var AppID = oInfo.iApplicationID;
                var AppGUID = string.Empty;
                var sTraceLog = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + "TraceLog");
                var StreamID = oParams.Where(m => m.sName.ToLower() == "StreamID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var CommID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_InstanceID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sSessionID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SessionID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(CommID))//check mandatory param communication instance uid passed or not
                {
                    oTraceInfo.Add(new CNV { sValue = "Mandatory parameter Communication instance uid is: " + CommID });
                    var oCommI = oXI.BOI("XICommunicationI", CommID);
                    if (oCommI != null && oCommI.Attributes.Count() > 0)
                    {
                        string sTemplateID = string.Empty;
                        var iDirection = oCommI.AttributeI("iDirection").sValue;
                        var sTo = oCommI.AttributeI("sTo").sValue;
                        var sMethod = oCommI.AttributeI("iComType").sValue;
                        var sToMail = oCommI.AttributeI("sToMail").sValue;
                        var XIInstanceDefinitionID = oCommI.AttributeI("XIInstanceDefinitionID").sValue;
                        var sComTypeID = oCommI.AttributeI("FKiCommunicationTypeIDXIGUID").sValue;
                        var FKiParentCommIID = oCommI.AttributeI("FKiParentCommIID").iValue;
                        var sSubject = oCommI.AttributeI("sHeader").sValue;
                        if (!string.IsNullOrEmpty(sComTypeID))
                        {
                            var oCommTypeI = oXI.BOI("XICommunicationType", sComTypeID);
                            if (oCommTypeI != null && oCommTypeI.Attributes.Count() > 0)
                            {
                                var bConversation = oCommTypeI.AttributeI("bConversation").bValue;
                                sTemplateID = oCommTypeI.AttributeI("FKiTemplateID").sValue;
                                var RefCommID = oCommTypeI.AttributeI("FKiRefComType").sValue;
                                var RefCommIDXIGUID = oCommTypeI.AttributeI("FKiRefComTypeXIGUID").sValue;
                                AppGUID = oCommTypeI.AttributeI("FKiAppIDXIGUID").sValue;
                                if (!string.IsNullOrEmpty(XIInstanceDefinitionID) && !string.IsNullOrEmpty(sToMail))
                                {
                                    var XICommTypeInstanceDefinitionID = oCommTypeI.AttributeI("XIInstanceDefinitionID").sValue;
                                    var sToAttribute = oCommTypeI.AttributeI("sToAttribute").sValue;
                                    var XIBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "", XICommTypeInstanceDefinitionID);
                                    var oXIInstanceI = oXI.BOI(XIBOD.Name, sToMail);
                                    if (oXIInstanceI != null && oXIInstanceI.Attributes.Count() > 0)
                                    {
                                        if (!string.IsNullOrEmpty(oXIInstanceI.AttributeI(sToAttribute).sValue))
                                        {
                                            sTo = oXIInstanceI.AttributeI(sToAttribute).sValue;
                                            oCommI.SetAttribute("sTo", sTo);
                                        }
                                    }
                                    string sMail = "";
                                    if (!string.IsNullOrEmpty(oCommI.AttributeI("sCC").sValue) && !oCommI.AttributeI("sCC").sValue.Contains('@'))
                                    {
                                        List<string> list = new List<string>();
                                        if (oCommI.AttributeI("sCC").sValue.Contains(','))
                                        {
                                            list = oCommI.AttributeI("sCC").sValue.Split(',').ToList();
                                        }
                                        else
                                            list.Add(oCommI.AttributeI("sCC").sValue);
                                        foreach (var item in list)
                                        {
                                            oXIInstanceI = oXI.BOI(XIBOD.Name, item);
                                            if (oXIInstanceI != null && oXIInstanceI.Attributes.Count() > 0)
                                            {
                                                if (!string.IsNullOrEmpty(oXIInstanceI.AttributeI(sToAttribute).sValue))
                                                {
                                                    sMail += oXIInstanceI.AttributeI(sToAttribute).sValue + ",";
                                                }
                                            }
                                        }
                                        oCommI.SetAttribute("sCC", sMail.Substring(0, sMail.Length - 1));
                                    }
                                }

                                int iMethod = 0;
                                int.TryParse(sMethod, out iMethod);
                                oTraceInfo.Add(new CNV { sValue = "Method is: " + iMethod });
                                if (iMethod > 0)
                                {
                                    var sFrom = string.Empty;
                                    //if (iSendMode == 10 || iStatus == 20)
                                    //{
                                    //var sTestMail = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, oAccount.FKiAppID + "_" + oAccount.FKiOrgID + "_" + "sTestMail");
                                    //sTo = sTestTo;
                                    //}
                                    //if (!string.IsNullOrEmpty(sFromOverride))
                                    //{
                                    //    sFrom = sFromOverride;
                                    //}

                                    Guid RefCommIDGUID = Guid.Empty;
                                    Guid.TryParse(RefCommIDXIGUID, out RefCommIDGUID);
                                    int iRefCommID = 0;
                                    int.TryParse(RefCommID, out iRefCommID);
                                    oTraceInfo.Add(new CNV { sValue = "FKiRefComTypeID is: " + RefCommID });
                                    oTraceInfo.Add(new CNV { sValue = "FKiRefComTypeIDXIGUID is: " + RefCommIDGUID });
                                    //Load communication type object for account details
                                    if (iRefCommID > 0 || (RefCommIDGUID != null && RefCommIDGUID != Guid.Empty))
                                    {
                                        //var RefCommI = oXI.BOI("XIrefComType", RefCommID);
                                        var RefCommI = new XIIBO();
                                        if (RefCommIDGUID != null && RefCommIDGUID != Guid.Empty)
                                        {
                                            RefCommI = (XIIBO)oCache.GetObjectFromCache(XIConstant.CacheXIrefComType, "XIrefComType", RefCommIDGUID.ToString());
                                        }
                                        else if (iRefCommID > 0)
                                        {
                                            RefCommI = (XIIBO)oCache.GetObjectFromCache(XIConstant.CacheXIrefComType, "XIrefComType", iRefCommID.ToString());
                                        }
                                        if (RefCommI != null && RefCommI.Attributes.Count() > 0)
                                        {
                                            var iMaxRetry = RefCommI.AttributeI("iMaxRetry").sValue;
                                            int iRetryMax = Convert.ToInt32(iMaxRetry);
                                            var sSendMode = RefCommI.AttributeI("iSendMode").sValue;
                                            int iSendMode = 0;
                                            int.TryParse(sSendMode, out iSendMode);
                                            oTraceInfo.Add(new CNV { sValue = "Send mode is: " + iSendMode });
                                            //Sendmodes are Test, Test no send/receive and Live
                                            if (iSendMode > 0)
                                            {
                                                //Load API and Account details like Sendgrid or SMTP
                                                var APIGUID = Guid.Empty;
                                                var API = RefCommI.AttributeI("FKiAPIID").sResolvedValue;
                                                int iAPIID = 0;
                                                oTraceInfo.Add(new CNV { sValue = "API is: " + API });
                                                if (int.TryParse(API, out iAPIID) || Guid.TryParse(API, out APIGUID))
                                                {
                                                    //var oAPI = oXI.BOI("XIAPI Settings", API);
                                                    var oAPI = (XIIBO)oCache.GetObjectFromCache(XIConstant.CacheXIrefComType, "XIAPI Settings", API);
                                                    if (oAPI != null && oAPI.Attributes.Count() > 0)
                                                    {
                                                        var sAPI = oAPI.AttributeI("sname").sValue;
                                                        if (!string.IsNullOrEmpty(sAPI))
                                                        {
                                                            var AccountGUID = Guid.Empty;
                                                            int iAccountID = 0;
                                                            var XIAccount = RefCommI.AttributeI("FKiXIAccountID").sValue;
                                                            oTraceInfo.Add(new CNV { sValue = "XI Account is: " + XIAccount });
                                                            if (int.TryParse(XIAccount, out iAccountID) || Guid.TryParse(XIAccount, out AccountGUID))
                                                            {
                                                                oTraceInfo.Add(new CNV { sValue = "Direction is: " + iDirection });
                                                                if (iDirection == "20")
                                                                {
                                                                    var RetryCountCommI = oCommI.AttributeI("iRetryCount").sValue;
                                                                    int iRetryCountCommI = 0;
                                                                    int.TryParse(RetryCountCommI, out iRetryCountCommI);
                                                                    oTraceInfo.Add(new CNV { sValue = "Retry Count is: " + iRetryCountCommI + " and Retry Max is:" + iRetryMax });
                                                                    if (iRetryCountCommI <= iRetryMax)
                                                                    {
                                                                        if (bConversation)
                                                                        {
                                                                            var iConversationID = 0;
                                                                            string sConversationID = string.Empty;
                                                                            if (FKiParentCommIID > 0)//load parent commi instance
                                                                            {
                                                                                var oConversI = oXI.BOI("ConversationI", FKiParentCommIID.ToString());
                                                                                if (oConversI != null && oConversI.Attributes.Count() > 0)
                                                                                {
                                                                                    //set to communication instance
                                                                                    iConversationID = FKiParentCommIID;
                                                                                }
                                                                            }
                                                                            else // create new conversationi 
                                                                            {
                                                                                XIIBO oConversI = new XIIBO();
                                                                                var oConverBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ConversationI");
                                                                                oConversI.BOD = oConverBOD;
                                                                                oConversI.SetAttribute("sName", sSubject);
                                                                                oCR = oConversI.Save(oConversI);
                                                                                if (oCR.bOK && oCR.oResult != null)
                                                                                {
                                                                                    oConversI = (XIIBO)oCR.oResult;
                                                                                    iConversationID = oConversI.AttributeI("id").iValue;
                                                                                    sConversationID = oConversI.AttributeI("xiguid").sValue;
                                                                                    oConversI.Write_Activity("Conversation created:" + iConversationID.ToString(), "Conv");
                                                                                }
                                                                            }
                                                                            if (iConversationID > 0)
                                                                            {

                                                                                var sPreDelim1 = oCommTypeI.AttributeI("sPreDelim1").sValue;
                                                                                var sIdentifier1 = oCommTypeI.AttributeI("iIdentifier1").sValue;
                                                                                var sPostDelim1 = oCommTypeI.AttributeI("sPostDelim1").sValue;
                                                                                var sPreDelim2 = oCommTypeI.AttributeI("sPreDelim2").sValue;
                                                                                var sIdentifier2 = oCommTypeI.AttributeI("iIdentifier2").sValue;
                                                                                var sPostDelim2 = oCommTypeI.AttributeI("sPostDelim2").sValue;
                                                                                var iAutoConversation1 = oCommTypeI.AttributeI("iAutoConversation1").sResolvedValue;
                                                                                if (iAutoConversation1 == "yes")
                                                                                {
                                                                                    if (!string.IsNullOrEmpty(sIdentifier1))
                                                                                    {
                                                                                        if (sIdentifier1.ToLower() == "id")
                                                                                        {
                                                                                            sSubject = sSubject + sPreDelim1 + iConversationID + sPostDelim1;
                                                                                        }
                                                                                        else if (sIdentifier1.ToLower() == "xiguid")
                                                                                        {
                                                                                            sSubject = sSubject + sPreDelim1 + sConversationID + sPostDelim1;
                                                                                        }
                                                                                    }
                                                                                    oCommI.SetAttribute("sHeader", sSubject.ToString());
                                                                                }
                                                                                oCommI.SetAttribute("FKiConversationID", iConversationID.ToString());
                                                                                oCR = oCommI.Save(oCommI);
                                                                                if (oCR.bOK && oCR.oResult != null)
                                                                                {
                                                                                    //Load Stream and run the PC of it
                                                                                    //run the communication process controller from here because we need to update the conversation FKs
                                                                                    if (!string.IsNullOrEmpty(StreamID))
                                                                                    {
                                                                                        var oStreamI = oXI.BOI("XICommunicationStream", StreamID);
                                                                                        if (oStreamI != null && oStreamI.Attributes.Count() > 0)
                                                                                        {
                                                                                            var PCGUID = oStreamI.AttributeI("FKiProcessControllerIDXIGUID").sValue;
                                                                                            XIDAlgorithm oAlgoD = new XIDAlgorithm();
                                                                                            string sNewGUID = Guid.NewGuid().ToString();
                                                                                            List<CNV> oNVsList = new List<CNV>();
                                                                                            oNVsList.Add(new CNV { sName = "-iCommID", sValue = CommID });
                                                                                            oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                                                                                            oAlgoD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, PCGUID);
                                                                                            oCR = oAlgoD.Execute_XIAlgorithm(sSessionID, sNewGUID);
                                                                                            oCache.Clear_GuidCache(sSessionID, sNewGUID);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                oTraceInfo.Add(new CNV { sValue = "Conversationid is not generated" });
                                                                            }
                                                                        }
                                                                        //sTo = "ravit@systemsdna.com";
                                                                        if (iMethod == 10)//EMail
                                                                        {
                                                                            oTraceInfo.Add(new CNV { sValue = "To mail address is: " + sTo });
                                                                            //Validate To Email Address
                                                                            //TO DO: Check the email address exists or not
                                                                            bool bIsEmail = Regex.IsMatch(sTo, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                                                                            if (bIsEmail)
                                                                            {
                                                                                //Call Send grid Load method with required data for mail sending
                                                                                if (sAPI.ToLower() == "sendgrid" && (iAccountID > 0 || (AccountGUID != null && AccountGUID != Guid.Empty)) && iSendMode != 20)
                                                                                {
                                                                                    oTraceInfo.Add(new CNV { sValue = "Mail is sending through sendgrid account: " + XIAccount });
                                                                                    string sContent = oCommI.AttributeI("sContent").sValue;
                                                                                    //string sSubject = oCommI.AttributeI("sHeader").sValue;
                                                                                    var SendGridTemplate = sTemplateID;
                                                                                    XIInfraSendGridComponent oSendGrid = new XIInfraSendGridComponent();
                                                                                    oSendGrid.sAccountID = XIAccount;
                                                                                    oSendGrid.sTo = sTo;
                                                                                    oSendGrid.sSubject = sSubject;
                                                                                    oSendGrid.sBody = sContent;
                                                                                    oSendGrid.CommID = CommID;
                                                                                    var oSGParams = new List<CNV>();
                                                                                    //sTo = "ravit@systemsdna.com";
                                                                                    oSGParams.Add(new CNV() { sName = XIConstant.Param_SendGridAccountID, sValue = XIAccount });
                                                                                    oSGParams.Add(new CNV() { sName = XIConstant.Param_SendGridTemplateID, sValue = SendGridTemplate });
                                                                                    oSGParams.Add(new CNV() { sName = "sContent", sValue = sContent });
                                                                                    oSGParams.Add(new CNV() { sName = "sSubject", sValue = sSubject });
                                                                                    oSGParams.Add(new CNV() { sName = "sTo", sValue = sTo });
                                                                                    oSGParams.Add(new CNV() { sName = "sFrom", sValue = sFrom });
                                                                                    List<AttachmentSG> Attachments = new List<AttachmentSG>();
                                                                                    string AttachemntIDs = oCommI.AttributeI("FKizXDoc").sValue;
                                                                                    List<string> AttachementPath = new List<string>();
                                                                                    if (!string.IsNullOrEmpty(AttachemntIDs))
                                                                                    {
                                                                                        var sDocIds = AttachemntIDs.Split(',');
                                                                                        var sVirtualDir = System.Configuration.ConfigurationManager.AppSettings["VirtualDirectoryPath"];
                                                                                        string physicalPath = HostingEnvironment.MapPath("~\\" + sVirtualDir + "\\");
                                                                                        foreach (var DocID in sDocIds)
                                                                                        {
                                                                                            var oDocumentBOI = oXI.BOI("Documents_T", DocID);
                                                                                            if (oDocumentBOI != null && oDocumentBOI.Attributes.ContainsKey("sFullPath"))
                                                                                            {
                                                                                                var oXIDocDetails = (XIInfraDocTypes)oCache.GetObjectFromCache(XIConstant.CacheDocType, "", oDocumentBOI.Attributes["FKiDocType"].sValue);
                                                                                                var sFolderPath = oXIDocDetails.Path;
                                                                                                sFolderPath = sFolderPath == null ? "" : sFolderPath;
                                                                                                if (!string.IsNullOrEmpty(oDocumentBOI.Attributes["sFullPath"].sValue))
                                                                                                {
                                                                                                    string sFilePath = physicalPath.Substring(0, physicalPath.Length) + sFolderPath.Replace("~", "") + "\\" + oDocumentBOI.Attributes["sFullPath"].sValue;
                                                                                                    Byte[] bytes = File.ReadAllBytes(sFilePath);
                                                                                                    String fileContent = Convert.ToBase64String(bytes);
                                                                                                    Attachments.Add(new AttachmentSG { Content = fileContent, FileName = oDocumentBOI.Attributes["sAliasName"].sValue });
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    oSendGrid.Attachments = Attachments;
                                                                                    oCR = oSendGrid.Send_Mail().Result;
                                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                                    {
                                                                                        var SendgridResp = (BaseResponse)oCR.oResult;
                                                                                        var sMessageID = SendgridResp.Data.X_Message_Id;
                                                                                        var bStatus = SendgridResp.Status;
                                                                                        oTraceInfo.Add(new CNV { sValue = "Mail sent successfully through send grid Message reference is: " + sMessageID });
                                                                                        oCommI.SetAttribute("sSendGridReference", sMessageID);
                                                                                        oCommI.SetAttribute("sSendReceiveResult", "Sent");
                                                                                        if (bStatus)
                                                                                        {
                                                                                            oCommI.SetAttribute("iExternalStatus", "10");
                                                                                            oCommI.SetAttribute("iStatus", "10");
                                                                                        }
                                                                                        else
                                                                                        {

                                                                                            oCommI.SetAttribute("iExternalStatus", "20");
                                                                                            oCommI.SetAttribute("iStatus", "20");
                                                                                        }
                                                                                        oCommI.SetAttribute("iSendStatus", "20");
                                                                                        var sRetryLog = oCommI.AttributeI("sRetryLog").sValue + "Success-";
                                                                                        oCommI.SetAttribute("sRetryLog", sRetryLog);
                                                                                        oCR = oCommI.Save(oCommI);
                                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                                        {

                                                                                            XIDAlgorithm oAlgoD = new XIDAlgorithm();
                                                                                            string sNewGUID = Guid.NewGuid().ToString();
                                                                                            List<CNV> oNVsList = new List<CNV>();
                                                                                            oNVsList.Add(new CNV { sName = "-iCommID", sValue = CommID });
                                                                                            oNVsList.Add(new CNV { sName = "-iSGMessageID", sValue = sMessageID });
                                                                                            oNVsList.Add(new CNV { sName = "-AppGUID", sValue = AppGUID.ToString() });
                                                                                            oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                                                                                            oAlgoD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, "eae87e5a-e20b-4130-ad14-a92cc5bb6d7d");
                                                                                            oCR = oAlgoD.Execute_XIAlgorithm(sSessionID, sNewGUID);
                                                                                            oCache.Clear_GuidCache(sSessionID, sNewGUID);



                                                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                                                                            oCResult.sMessage = "Mail sent successfully through send grid and status updated successfully for instance:" + CommID;
                                                                                            oCResult.oResult = "Success";
                                                                                            //if (iRateLimit == 0)
                                                                                            //{
                                                                                            //    var dtRateTime = oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|-dtRateTime}", DateTime.Now.ToString(), null, null);
                                                                                            //}
                                                                                            //iRateLimit++;
                                                                                            //var oResponse = oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|-iRateLimit}", iRateLimit.ToString(), null, null);

                                                                                            //Call Process controller for each communication
                                                                                            //var sNewGUID = Guid.NewGuid().ToString();
                                                                                            //List<CNV> oNVsList = new List<CNV>();
                                                                                            //oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                                                                                            //XIDAlgorithm oAlogD = new XIDAlgorithm();
                                                                                            //oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, "");
                                                                                            //oAlogD.Execute_XIAlgorithm(sSessionID, sNewGUID);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.sMessage = "Mail sent successfully through send grid and Error while updating the send status for communication instance:" + CommID;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        oCommI.SetAttribute("iStatus", "20");
                                                                                        var sRetryLog = oCommI.AttributeI("sRetryLog").sValue + "Failure-";
                                                                                        oCommI.SetAttribute("sRetryLog", sRetryLog);
                                                                                        oCR = oCommI.Save(oCommI);
                                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                                        {
                                                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.sMessage = "Error while sending mail through send grid and Failure status successfully updated for instance:" + CommID;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.sMessage = "Error while sending mail through send grid and error while updating failure status for communication instance:" + CommID;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else if (sAPI.ToLower() == "smtp" && iSendMode != 20)//Call SMTP method with required data for Mail Sending
                                                                                {
                                                                                    oTraceInfo.Add(new CNV { sValue = "Mail is sending through SMTP account: " + XIAccount });
                                                                                    string sContent = oCommI.AttributeI("sContent").sValue;
                                                                                    //string sSubject = oCommI.AttributeI("sHeader").sValue;
                                                                                    string AttachemntIDs = oCommI.AttributeI("FKizXDoc").sValue;
                                                                                    List<string> AttachementPath = new List<string>();
                                                                                    if (!string.IsNullOrEmpty(AttachemntIDs))
                                                                                    {
                                                                                        var sDocIds = AttachemntIDs.Split(',');
                                                                                        var sVirtualDir = System.Configuration.ConfigurationManager.AppSettings["VirtualDirectoryPath"];
                                                                                        string physicalPath = HostingEnvironment.MapPath("~\\" + sVirtualDir + "\\");
                                                                                        foreach (var DocID in sDocIds)
                                                                                        {
                                                                                            var oDocumentBOI = oXI.BOI("Documents_T", DocID);
                                                                                            if (oDocumentBOI != null && oDocumentBOI.Attributes.ContainsKey("sFullPath"))
                                                                                            {
                                                                                                var oXIDocDetails = (XIInfraDocTypes)oCache.GetObjectFromCache(XIConstant.CacheDocType, "", oDocumentBOI.Attributes["FKiDocType"].sValue);
                                                                                                var sFolderPath = oXIDocDetails.Path;
                                                                                                sFolderPath = sFolderPath == null ? "" : sFolderPath;
                                                                                                if (!string.IsNullOrEmpty(oDocumentBOI.Attributes["sFullPath"].sValue))
                                                                                                {
                                                                                                    AttachementPath.Add(physicalPath.Substring(0, physicalPath.Length) + sFolderPath.Replace("~", "") + "\\" + oDocumentBOI.Attributes["sFullPath"].sValue);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    string sCC = oCommI.AttributeI("sCC").sValue;
                                                                                    string sBCC = oCommI.AttributeI("sBCC").sValue;
                                                                                    if (string.IsNullOrEmpty(sFrom))
                                                                                    {
                                                                                        sFrom = oCommI.AttributeI("sFrom").sValue;
                                                                                    }
                                                                                    XIInfraEmail oEmail = new XIInfraEmail();
                                                                                    List<CNV> oMailParams = new List<CNV>();
                                                                                    oMailParams.Add(new CNV { sName = "sTo", sValue = sTo });
                                                                                    oMailParams.Add(new CNV { sName = "sFrom", sValue = sFrom });
                                                                                    oMailParams.Add(new CNV { sName = "sContent", sValue = sContent });
                                                                                    oMailParams.Add(new CNV { sName = "sSubject", sValue = sSubject });
                                                                                    oMailParams.Add(new CNV { sName = "AccountUID", sValue = XIAccount });
                                                                                    oMailParams.Add(new CNV { sName = "sCC", sValue = sCC });
                                                                                    oMailParams.Add(new CNV { sName = "sBCC", sValue = sBCC });
                                                                                    oCR = oEmail.Send_SMTPMail(oMailParams, AttachementPath);
                                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                                    {
                                                                                        oTraceInfo.Add(new CNV { sValue = "Mail sent successfully through SMTP" });
                                                                                        oCommI.SetAttribute("sSendReceiveResult", "Sent");
                                                                                        oCommI.SetAttribute("iStatus", "10");
                                                                                        oCommI.SetAttribute("iSendStatus", "20");
                                                                                        var sRetryLog = oCommI.AttributeI("sRetryLog").sValue + "Success-";
                                                                                        oCommI.SetAttribute("sRetryLog", sRetryLog);
                                                                                        oCR = oCommI.Save(oCommI);
                                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                                        {
                                                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                                                                            oCResult.sMessage = "Mail sent successfully through SMTP and Status updated to success for communication instance:" + CommID;
                                                                                            oCResult.oResult = "Success";
                                                                                            //if (iRateLimit == 0)
                                                                                            //{
                                                                                            //    var dtRateTime = oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|-dtRateTime}", DateTime.Now.ToString(), null, null);
                                                                                            //}
                                                                                            //iRateLimit++;
                                                                                            //var oResponse = oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|-iRateLimit}", iRateLimit.ToString(), null, null);

                                                                                            //Call Process controller for each communication
                                                                                            //var sNewGUID = Guid.NewGuid().ToString();
                                                                                            // List<CNV> oNVsList = new List<CNV>();
                                                                                            //oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                                                                                            //XIDAlgorithm oAlogD = new XIDAlgorithm();
                                                                                            //oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, "");
                                                                                            // oCR = oAlogD.Execute_XIAlgorithm(sSessionID, sNewGUID);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.sMessage = "Mail sent successfully through SMTP and Error while updating the send status for communication instance:" + CommID;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        oCommI.SetAttribute("iStatus", "30");
                                                                                        var sRetryLog = oCommI.AttributeI("sRetryLog").sValue + "Failure-";
                                                                                        oCommI.SetAttribute("sRetryLog", sRetryLog);
                                                                                        oCommI.SetAttribute("iSendStatus", "60");
                                                                                        oCR = oCommI.Save(oCommI);
                                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                                        {
                                                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.sMessage = "Error while sending SMTP mail and Failure status successfully updated for instance:" + CommID;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                                            oCResult.sMessage = "Error while sending SMTP mail and error while updating failure status for communication instance:" + CommID;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else if (iSendMode == 20)
                                                                                {
                                                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                                                                    oCResult.sMessage = "Send mode is configured as Test No Send/Recieve for communication type:" + RefCommIDGUID;
                                                                                }
                                                                                else
                                                                                {
                                                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                                    oCResult.sMessage = "iType or Account or Template not configured for communication instance:" + CommID;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                                oCResult.sMessage = "To Email address is not valid:" + sTo;
                                                                            }
                                                                        }
                                                                        else if (iMethod == 20)//SMS
                                                                        {

                                                                        }
                                                                        else if (iMethod == 30)//WhatsApp
                                                                        {

                                                                        }
                                                                        else
                                                                        {

                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                                                        oCResult.sMessage = "Max retry reached for communication instance:" + CommID;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                    oCResult.sMessage = "iDirection: " + iDirection + " not configured for communication instance:" + CommID;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                oCResult.sMessage = "Account is not configured for communication type:" + RefCommIDGUID;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                            oCResult.sMessage = "API not configured for API instance:" + APIGUID;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                        oCResult.sMessage = "API instance loading failed for:" + APIGUID;
                                                    }
                                                }
                                                else
                                                {
                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.sMessage = "API is not configured for communication type:" + RefCommIDGUID;
                                                }
                                            }
                                            else
                                            {
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.sMessage = "Send mode is not configured for communication type:" + RefCommIDGUID;
                                            }
                                        }
                                        else
                                        {
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.sMessage = "XIrefComType instance loading failed for:" + RefCommIDGUID;
                                        }
                                    }
                                    else
                                    {
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.sMessage = "FKiComTypeID or FKiComTypeIDXIGUID is not configured for instance:" + CommID;
                                    }
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Communication Method is not configured";
                                }
                            }
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "XICommunicationI instance loading failed for:" + CommID;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.sMessage = "CommID for XICommunicationI is received null";
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    oTraceInfo.Add(new CNV { sValue = oCResult.sMessage });
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + oCResult.sMessage;
                    if (string.IsNullOrEmpty(sTraceLog) && sTraceLog.ToLower() == "no")
                    {
                        oCResult.oTraceStack = oTraceInfo;
                        SaveErrortoDB(oCResult);
                    }
                }
                if (!string.IsNullOrEmpty(sTraceLog) && sTraceLog.ToLower() == "yes")
                {
                    oCResult.oTraceStack = oTraceInfo;
                    SaveErrortoDB(oCResult);
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oTraceInfo.Add(new CNV { sValue = oCResult.sMessage });
                oCResult.oTraceStack = oTraceInfo;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult CommunicationStreamInbound(List<CNV> oParams, iSiganlR oSignalR)//ConversationMatch
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            XIDefinitionBase oBase = new XIDefinitionBase();
            List<CNV> oTraceInfo = new List<CNV>();
            try
            {
                //TO DO: need a default stream
                //get all streams of type inbound
                //if the identifier is from address then check from and to 
                //if there is a match break the loop
                //if comm type on stream then get pre delim and post delim, check the subject first if it matches then assign the conversationid to this communcation
                //eg:subject='hello ##1234776##'
                //eg:contains'conversion:[dfe343sdfe]'
                //if no match then use predelim2 and postdelim2, check the body if it matches then assign the conversationid to this communcation
                CUserInfo oInfo = new CUserInfo();
                XIInfraCache oCache = new XIInfraCache();
                XIInfraUsers oUser = new XIInfraUsers();
                oInfo = oUser.Get_UserInfo();
                var AppID = oInfo.iApplicationID;
                var AppGUID = string.Empty;
                var sTraceLog = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + "TraceLog");
                XID1Click o1ClickD = new XID1Click();
                XIIXI oXI = new XIIXI();
                XIAPI oAPI = new XIAPI();
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Inbound Stream Import");//this will get all the streams with iDirection=10(inbound)
                if (o1ClickD != null)
                {
                    bool bMatch = false;
                    var sSubject = string.Empty;
                    var sFrom = string.Empty;
                    var sTo = string.Empty;
                    var sBody = string.Empty;
                    var sOriginAccount = string.Empty;
                    var sComTypeID = string.Empty;
                    var oCommI = new XIIBO();
                    var oCommTypeI = new XIIBO();
                    string PCGUID = string.Empty;
                    var CommIID = string.Empty;
                    var Response = o1ClickD.OneClick_Execute();
                    if (Response != null && Response.Count() > 0)
                    {
                        var sStream = "";
                        var sRegexCode = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "InboundRegexOverride");
                        foreach (var stream in Response.Values.ToList())
                        {
                            sStream = stream.AttributeI("sName").sValue;
                            PCGUID = stream.AttributeI("FKiProcessControllerIDXIGUID").sValue;
                            //Regex on from and to address and subject and body, we need to check all
                            //Examples: *@systemdna.com
                            //Example2: ravit@systemsdna.com,dan.s@systemsdna.com
                            CommIID = oParams.Where(m => m.sName.ToLower() == "commiid").Select(m => m.sValue).FirstOrDefault();
                            oCommI = oXI.BOI("XICommunicationI", CommIID);
                            if (oCommI != null && oCommI.Attributes.Count() > 0)
                            {
                                sSubject = oCommI.AttributeI("sHeader").sValue;
                                sBody = oCommI.AttributeI("sContent").sValue;
                                sTo = oCommI.AttributeI("sTo").sValue;
                                sFrom = oCommI.AttributeI("sFrom").sValue;
                                sOriginAccount = oCommI.AttributeI("sOriginAccount").sValue;
                                sComTypeID = oCommI.AttributeI("FkiComTypeID").sValue;
                                var StreamComType = stream.AttributeI("FKiCommunicationTypeIDXIGUID").sValue;
                                if (!string.IsNullOrEmpty(StreamComType))
                                {
                                    sComTypeID = StreamComType;
                                }
                                oCommTypeI = oXI.BOI("XICommunicationType", sComTypeID);
                                //if iIdentifier is not null and > 0 then call the match method with iIdentifier and sIdentifier, 
                                var iIdentifier = stream.AttributeI("iIdentifier").iValue;
                                var iIdentifier2 = stream.AttributeI("iIdentifier2").iValue;
                                var iIdentifier3 = stream.AttributeI("iIdentifier3").iValue;
                                var iIdentifier4 = stream.AttributeI("iIdentifier4").iValue;
                                var iIdentifier5 = stream.AttributeI("iIdentifier5").iValue;
                                var sIdentifierMatch = stream.AttributeI("sIdentifierMatch").sValue;
                                var sIdentifierMatch2 = stream.AttributeI("sIdentifierMatch2").sValue;
                                var sIdentifierMatch3 = stream.AttributeI("sIdentifierMatch3").sValue;
                                var sIdentifierMatch4 = stream.AttributeI("sIdentifierMatch4").sValue;
                                var sIdentifierMatch5 = stream.AttributeI("sIdentifierMatch5").sValue;
                                List<CNV> oSrchParams = new List<CNV>();
                                int iCurrentIdentifier = 0;
                                string sCurrentIdentifier = String.Empty;
                                string sCurrentTarget = String.Empty;
                                if (iIdentifier > 0)
                                {
                                    for (int j = 1; j < 6; j++)
                                    {
                                        if (j == 1)
                                        {
                                            iCurrentIdentifier = iIdentifier;
                                            sCurrentIdentifier = sIdentifierMatch;
                                        }
                                        else if (j == 2)
                                        {
                                            iCurrentIdentifier = iIdentifier2;
                                            sCurrentIdentifier = sIdentifierMatch2;
                                        }
                                        else if (j == 3)
                                        {
                                            iCurrentIdentifier = iIdentifier3;
                                            sCurrentIdentifier = sIdentifierMatch3;
                                        }
                                        else if (j == 4)
                                        {
                                            iCurrentIdentifier = iIdentifier4;
                                            sCurrentIdentifier = sIdentifierMatch4;
                                        }
                                        else if (j == 5)
                                        {
                                            iCurrentIdentifier = iIdentifier5;
                                            sCurrentIdentifier = sIdentifierMatch5;
                                        }
                                        if (iCurrentIdentifier > 0)
                                        {
                                            if (iCurrentIdentifier == 10)
                                            {
                                                if (sTo.Contains('<'))
                                                {
                                                    var sTOIndex = sTo.IndexOf("<");
                                                    sTo = sTo.Substring(sTOIndex + 1, sTo.Length - sTOIndex - 1);
                                                    if (sTo.EndsWith(">"))
                                                    {
                                                        sTo = sTo.Substring(0, sTo.Length - 1);
                                                    }
                                                }
                                                sCurrentTarget = sTo;
                                            }
                                            else if (iCurrentIdentifier == 20)
                                            {
                                                if (sFrom.Contains('<'))
                                                {
                                                    var sFromIndex = sFrom.IndexOf("<");
                                                    sFrom = sFrom.Substring(sFromIndex + 1, sFrom.Length - sFromIndex - 1);
                                                    if (sFrom.EndsWith(">"))
                                                    {
                                                        sFrom = sFrom.Substring(0, sFrom.Length - 1);
                                                    }
                                                }
                                                sCurrentTarget = sFrom;
                                            }
                                            else if (iCurrentIdentifier == 30)
                                            {
                                                sCurrentTarget = sSubject;
                                            }
                                            else if (iCurrentIdentifier == 40)
                                            {
                                                sCurrentTarget = sBody;
                                            }
                                            else if (iCurrentIdentifier == 50)
                                            {
                                                sCurrentTarget = sOriginAccount;
                                            }
                                            if (!string.IsNullOrEmpty(sCurrentTarget) && !string.IsNullOrEmpty(sCurrentIdentifier))
                                            {
                                                oSrchParams = new List<CNV>();
                                                oSrchParams.Add(new CNV { sName = "sStringToMatch", sValue = sCurrentTarget });
                                                oSrchParams.Add(new CNV { sName = "sMatchSearchFor", sValue = sCurrentIdentifier });
                                                //oSrchParams.Add(new CNV { sName = "sRegex", sValue = sRegexCode });
                                                oCR = MatchIdentifier(oSrchParams);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {
                                                    bMatch = (bool)oCR.oResult;
                                                    if (bMatch)
                                                    {
                                                        break;
                                                    }
                                                }
                                                else
                                                {

                                                }
                                            }

                                        }
                                    }
                                    if (bMatch)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        if (bMatch)
                        {
                            //Write activity
                            oCommI.Write_Activity("Match found on stream:" + sStream, "Conv");
                            if (oCommTypeI != null && oCommTypeI.Attributes.Count() > 0)
                            {
                                var sPreDelim1 = oCommTypeI.AttributeI("sPreDelim1").sValue;
                                var sIdentifier1 = oCommTypeI.AttributeI("iIdentifier1").sValue;
                                var sPostDelim1 = oCommTypeI.AttributeI("sPostDelim1").sValue;
                                var sPreDelim2 = oCommTypeI.AttributeI("sPreDelim2").sValue;
                                var sIdentifier2 = oCommTypeI.AttributeI("iIdentifier2").sValue;
                                var sPostDelim2 = oCommTypeI.AttributeI("sPostDelim2").sValue;
                                bool bFound = false;
                                if (!string.IsNullOrEmpty(sSubject))//check subject
                                {
                                    int iStart = sSubject.IndexOf(sPreDelim1);
                                    if (iStart >= 0)
                                    {
                                        var sAfterStart = sSubject.Substring(iStart + 2, sSubject.Length - iStart - 2);
                                        int iEnd = sAfterStart.IndexOf(sPostDelim1);
                                        if (iEnd >= 0)
                                        {
                                            var sMatchCode = sAfterStart.Substring(0, iEnd);
                                            if (!string.IsNullOrEmpty(sMatchCode) && sMatchCode.Length <= 36)
                                            {
                                                bFound = true;
                                                bool bExist = false;
                                                //Check Conversation object for sMatchCode
                                                oCR = FindConversation(sMatchCode);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {
                                                    bExist = (bool)oCR.oResult;
                                                }
                                                if (bExist)
                                                {
                                                    oCommI.SetAttribute("FKiConversationID", sMatchCode);
                                                    oCR = oCommI.Save(oCommI);
                                                    oCommI.Write_Activity("Linked with Conversation:" + sMatchCode, "Conv");
                                                }
                                                else
                                                {
                                                    oCR.sMessage = "Conversation not found for code:" + sMatchCode;
                                                    oBase.SaveErrortoDB(oCR);
                                                }
                                            }
                                            else if (!string.IsNullOrEmpty(sMatchCode))
                                            {
                                                oCR.sMessage = "Invalid conversation identifier found:" + sMatchCode;
                                                oBase.SaveErrortoDB(oCR);
                                            }
                                            else
                                            {
                                                //No conversation found
                                            }
                                        }
                                    }
                                }
                                if (!bFound)//check body
                                {
                                    if (!string.IsNullOrEmpty(sBody))
                                    {
                                        int iStart = sBody.IndexOf(sPreDelim2);
                                        if (iStart >= 0)
                                        {
                                            var sAfterStart = sBody.Substring(iStart, sBody.Length);
                                            int iEnd = sAfterStart.IndexOf(sPostDelim2);
                                            if (iEnd >= 0 && iEnd > iStart)
                                            {
                                                var sMatchCode = sBody.Substring(iStart, iEnd - iStart);
                                                if (!string.IsNullOrEmpty(sMatchCode) && sMatchCode.Length <= 36)
                                                {
                                                    bFound = true;
                                                    bool bExist = false;
                                                    //Check Conversation object for sMatchCode
                                                    oCR = FindConversation(sMatchCode);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        bExist = (bool)oCR.oResult;
                                                    }
                                                    if (bExist)
                                                    {
                                                        oCommI.SetAttribute("FKiConversationID", sMatchCode);
                                                        oCR = oCommI.Save(oCommI);
                                                    }
                                                    else
                                                    {
                                                        oCR.sMessage = "Conversation not found for code:" + sMatchCode;
                                                        oBase.SaveErrortoDB(oCR);
                                                    }
                                                }
                                                else if (!string.IsNullOrEmpty(sMatchCode))
                                                {
                                                    oCR.sMessage = "Invalid conversation identifier found:" + sMatchCode;
                                                    oBase.SaveErrortoDB(oCR);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            Guid PCXIGUID = Guid.Empty;
                            Guid.TryParse(PCGUID, out PCXIGUID);
                            if (PCXIGUID != null && PCXIGUID != Guid.Empty)
                            {
                                oTraceInfo.Add(new CNV { sValue = "Process controller execution started for PCGUID:" + PCXIGUID });
                                XIDAlgorithm oAlgoD = new XIDAlgorithm();
                                string sSessionID = Guid.NewGuid().ToString();
                                string sNewGUID = Guid.NewGuid().ToString();
                                if (sFrom.Contains('<'))
                                {
                                    var sFromIndex = sFrom.IndexOf("<");
                                    sFrom = sFrom.Substring(sFromIndex + 1, sFrom.Length - sFromIndex - 1);
                                    if (sFrom.EndsWith(">"))
                                    {
                                        sFrom = sFrom.Substring(0, sFrom.Length - 1);
                                    }
                                }
                                List<CNV> oNVsList = new List<CNV>();
                                oNVsList.Add(new CNV { sName = "-iBODID", sValue = oCommI.iBODID.ToString() });
                                oNVsList.Add(new CNV { sName = "-iBOIID", sValue = CommIID });
                                oNVsList.Add(new CNV { sName = "-sFrom", sValue = sFrom });
                                oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                                oAlgoD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, PCXIGUID.ToString());
                                //Write activity
                                oCommI.Write_Activity("Process controller :" + oAlgoD.sName + " is executed", "Conv");
                                oCR = oAlgoD.Execute_XIAlgorithm(sSessionID, sNewGUID, null, oSignalR);
                                oCache.Clear_GuidCache(sSessionID, sNewGUID);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    //if error update to the process error
                                    //if success update to process completed
                                    oCommI.SetAttribute("iProcessStatus", "30");
                                    oCR = oCommI.Save(oCommI);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                        oTraceInfo.Add(new CNV { sValue = "Process status set to 30 for communication instance:" + CommIID });
                                    }
                                    else
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oCR.sMessage = "Communication instance saving failed while setting iProcessStatus to 30";
                                        oTraceInfo.Add(new CNV { sValue = "Communication instance saving failed while setting iProcessStatus to 30" });
                                    }
                                }
                                else
                                {
                                    oCommI.SetAttribute("iProcessStatus", "20");
                                    oCR = oCommI.Save(oCommI);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                        oTraceInfo.Add(new CNV { sValue = "Process status set to 20 for communication instance:" + CommIID });
                                    }
                                    else
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oCR.sMessage = "Communication instance saving failed while setting iProcessStatus to 20";
                                        oTraceInfo.Add(new CNV { sValue = "Communication instance saving failed while setting iProcessStatus to 20" });
                                    }
                                }
                            }
                            else
                            {
                                //update status of comm instance to No Process required-10, Process Error-20, Process completed-30
                                oCommI.SetAttribute("iProcessStatus", "10");
                                oCR = oCommI.Save(oCommI);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    oTraceInfo.Add(new CNV { sValue = "Process status set to 10 for communication instance:" + CommIID });
                                }
                                else
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCR.sMessage = "Communication instance saving failed while setting iProcessStatus to 10";
                                    oTraceInfo.Add(new CNV { sValue = "Communication instance saving failed while setting iProcessStatus to 10" });
                                }
                            }
                        }
                        else
                        {
                            oCommI.SetAttribute("iProcessStatus", "100");
                            oCR = oCommI.Save(oCommI);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                oTraceInfo.Add(new CNV { sValue = "Process status set to 100 for communication instance:" + CommIID });
                            }
                            else
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCR.sMessage = "Communication instance saving failed while setting iProcessStatus to 100";
                                oTraceInfo.Add(new CNV { sValue = "Communication instance saving failed while setting iProcessStatus to 100" });
                            }
                        }
                    }
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.oResult = "Success";
                }
                else
                {
                    oCResult.oTraceStack = oTraceInfo;
                    SaveErrortoDB(oCResult);
                }
                if (!string.IsNullOrEmpty(sTraceLog) && sTraceLog.ToLower() == "yes")
                {
                    oCResult.oTraceStack = oTraceInfo;
                    SaveErrortoDB(oCResult);
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
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

        public CResult FindConversation(string sUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            XIDefinitionBase oBase = new XIDefinitionBase();
            try
            {
                XIIXI oXI = new XIIXI();
                var ConversationI = oXI.BOI("ConversationI", sUID);
                if (ConversationI != null && ConversationI.Attributes.Count() > 0)
                {
                    oCResult.oResult = true;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
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
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult MatchIdentifier(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            XIDefinitionBase oBase = new XIDefinitionBase();
            try
            {
                XIAPI oAPI = new XIAPI();
                oCR = oAPI.MatchString(oParams);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oCResult.oResult = (bool)oCR.oResult;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
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
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult Match_Communication(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "check idenfiter against comm subject and link the communication to lead/client/policy";//expalin about this method logic
            try
            {
                var CommID = oParams.Where(m => m.sName.ToLower() == "iCommID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iCommID = 0;
                int.TryParse(CommID, out iCommID);
                oTrace.oParams.Add(new CNV { sName = CommID, sValue = CommID });
                if (iCommID > 0)//check mandatory params are passed or not
                {
                    XIInfraCache oCache = new XIInfraCache();
                    XIIXI oXI = new XIIXI();
                    var oCommI = oXI.BOI("XICommunicationI", iCommID.ToString());
                    if (oCommI != null && oCommI.Attributes.Count() > 0)
                    {
                        var sSubject = oCommI.AttributeI("sHeader").sValue;
                        var sFrom = oCommI.AttributeI("sFrom").sValue;
                        var sTo = oCommI.AttributeI("sTo").sValue;
                        var sContent = oCommI.AttributeI("sContent").sValue;
                        var CommMatchBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XCommunicationMatch");
                        if (!string.IsNullOrEmpty(sSubject))
                        {
                            int iMatchStatus = 0;
                            XID1Click o1ClickD = new XID1Click();
                            XID1Click o1ClickC = new XID1Click();
                            o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Matching Identifier");
                            o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                            var Results = o1ClickC.OneClick_Run();
                            if (Results != null && Results.Count > 0)
                            {
                                foreach (var match in Results.Values.ToList())
                                {
                                    var iMatchID = match.AttributeI("ID").sValue;
                                    var sIdentifier = match.AttributeI("sIdentifier").sValue;
                                    var iType = match.AttributeI("iType").iValue;
                                    string sTargetMatch = string.Empty;
                                    if (iType == 10)
                                    {
                                        sTargetMatch = sSubject;
                                    }
                                    else if (iType == 20)
                                    {
                                        sTargetMatch = sFrom;
                                    }
                                    else if (iType == 30)
                                    {
                                        sTargetMatch = sTo;
                                    }
                                    else if (iType == 40)
                                    {
                                        sTargetMatch = sContent;
                                    }
                                    else
                                    {
                                        sTargetMatch = sSubject;
                                    }
                                    if (!string.IsNullOrEmpty(sTargetMatch) && !string.IsNullOrEmpty(sIdentifier))
                                    {
                                        if (sTargetMatch.ToLower().Contains(sIdentifier.ToLower()))
                                        {
                                            iMatchStatus = 10;
                                            XIIBO oCommMatch = new XIIBO();
                                            oCommMatch.BOD = CommMatchBOD;
                                            var FKiBODIDXIGUID = match.AttributeI("FKiBODIDXIGUID").sValue;
                                            var FKiBOIID = match.AttributeI("FKiBOIID").sValue;
                                            XIIXI oXII = new XIIXI();
                                            XIIBO oBOI = new XIIBO();
                                            List<CNV> oWhrParams = new List<CNV>();
                                            oWhrParams.Add(new CNV() { sName = "FKiBOIID", sValue = FKiBOIID });
                                            oWhrParams.Add(new CNV() { sName = "FKiBODIDXIGUID", sValue = FKiBODIDXIGUID });
                                            oWhrParams.Add(new CNV() { sName = "FKiCommunicationID", sValue = iCommID.ToString() });
                                            oBOI = oXI.BOI("XCommunicationMatch", null, null, oWhrParams);
                                            if (oBOI != null && oBOI.Attributes.Count() > 0)
                                            {

                                            }
                                            else
                                            {
                                                oBOI = new XIIBO();
                                                var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FKiBODIDXIGUID);
                                                oCR = oBOI.Get_BODialogLabel(FKBOD.XIGUID.ToString(), FKiBOIID);
                                                var FKsBOIID = (string)oCR.oResult;
                                                oCommMatch.SetAttribute("FKiMatchIdentifierID", iMatchID);
                                                oCommMatch.SetAttribute("FKiBODIDXIGUID", FKiBODIDXIGUID);
                                                oCommMatch.SetAttribute("FKiBOIID", FKiBOIID);
                                                oCommMatch.SetAttribute("FKsBODID", FKBOD.LabelName);
                                                oCommMatch.SetAttribute("FKsBOIID", FKsBOIID);
                                                oCommMatch.SetAttribute("FKiCommunicationID", iCommID.ToString());
                                                oCommMatch.SetAttribute("iMatchStatus", iMatchStatus.ToString());
                                                oCR = oCommMatch.Save(oCommMatch);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {

                                                }
                                                else
                                                {
                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                                        oCResult.sMessage = "Mandatory Param: sTargetMatch is missing";
                                    }
                                }
                                oCommI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommunicationI");
                                if (iMatchStatus == 0)
                                {
                                    oCommI.SetAttribute("iMatchStatus", "20");
                                }
                                else
                                {
                                    oCommI.SetAttribute("iMatchStatus", iMatchStatus.ToString());
                                }
                                oCommI.SetAttribute("iProcessStatus", "30");
                                oCR = oCommI.Save(oCommI);
                                if (oCR.bOK && oCR.oResult != null)
                                {

                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.sMessage = "There are no matching indentifier defined";
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Subject is missing for instance:" + iCommID;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Communication instance is not loaded for :" + iCommID;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.sMessage = "Mandatory Param: CommID is missing";
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    SaveErrortoDB(oCResult);
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

