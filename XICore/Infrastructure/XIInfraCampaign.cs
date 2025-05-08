using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Caching;
using XISystem;

namespace XICore
{
    public class XIInfraCampaign : XIDefinitionBase
    {
        public CResult Send_Campaign(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            List<CNV> oTraceInfo = new List<CNV>();
            try
            {
                var MobileNos = oParams.Where(m => m.sName.ToLower() == "mobileno").Select(m => m.sValue).FirstOrDefault();
                var WhatsappAccount = oParams.Where(m => m.sName.ToLower() == "WhatsappAccount".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var BOIID = oParams.Where(m => m.sName.ToLower() == "iBOIID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var BODID = oParams.Where(m => m.sName.ToLower() == "iBODID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var PilotID = oParams.Where(m => m.sName.ToLower() == "iPilotID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oTraceInfo.Add(new CNV
                {
                    sValue = "Send_Campaign Method called for Mandatory mobile numbers: " + MobileNos
                });
                string sMobileNo = string.Empty;
                if (!string.IsNullOrEmpty(MobileNos))
                {
                    int iBOIID = 0;
                    Guid BODXIGuid = Guid.Empty;
                    int.TryParse(BOIID, out iBOIID);
                    Guid.TryParse(BODID, out BODXIGuid);
                    XIIXI oXI = new XIIXI();
                    int iPilotID = 0;
                    int.TryParse(PilotID, out iPilotID);
                    Guid QSDXIGUID = Guid.Empty;
                    if (iPilotID > 0)
                    {
                        //Load Pilot
                        var oPilotI = oXI.BOI("BSPPilot", iPilotID.ToString());
                        if (oPilotI != null && oPilotI.Attributes.Count() > 0)
                        {
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "Pilot loaded for: " + iPilotID
                            });
                            var QSDGUID = oPilotI.AttributeI("FKiQSDIDXIGUID").sValue;
                            if (Guid.TryParse(QSDGUID, out QSDXIGUID))
                            {
                                oTraceInfo.Add(new CNV
                                {
                                    sValue = "QS Definition on Pilot is: " + QSDXIGUID
                                });
                                var nMobs = MobileNos.Split(',').ToList();
                                foreach (var sMobNo in nMobs)
                                {
                                    sMobileNo = sMobNo;
                                    oTraceInfo.Add(new CNV
                                    {
                                        sValue = "Campaign started for Mobile number: " + sMobileNo
                                    });
                                    oCR = Create_Communication(sMobNo, QSDXIGUID.ToString(), BODXIGuid, iBOIID, 30);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        var oCommI = (XIIBO)oCR.oResult;
                                        var iCommID = oCommI.AttributeI("id").iValue;
                                        if (iCommID > 0)
                                        {
                                            oTraceInfo.Add(new CNV
                                            {
                                                sValue = "Comminication created with id: " + iCommID
                                            });
                                            oCR = Create_Campaign(WhatsappAccount, "");
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                var oCampI = (XIIBO)oCR.oResult;
                                                var iCampaignID = oCampI.AttributeI("id").iValue;
                                                if (iCampaignID > 0)
                                                {
                                                    oTraceInfo.Add(new CNV
                                                    {
                                                        sValue = "Campaign created with id: " + iCampaignID
                                                    });
                                                    oCR = Create_TargetCampaignMap(sMobNo, iCampaignID.ToString(), iCommID.ToString());
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        var oLCMapI = (XIIBO)oCR.oResult;
                                                        var iLCMapID = oLCMapI.AttributeI("id").iValue;
                                                        if (iLCMapID > 0)
                                                        {
                                                            oTraceInfo.Add(new CNV
                                                            {
                                                                sValue = "Target campaign mapping(BSPLeadCampaignMap) created with id: " + iLCMapID
                                                            });
                                                        }
                                                        else
                                                        {
                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                            oTraceInfo.Add(new CNV
                                                            {
                                                                sValue = "Target campaign mapping(BSPLeadCampaignMap) creation failed"
                                                            });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                        oTraceInfo.Add(new CNV
                                                        {
                                                            sValue = "Target campaign mapping(BSPLeadCampaignMap) creation failed"
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                    oTraceInfo.Add(new CNV
                                                    {
                                                        sValue = "Campaign creation failed"
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oTraceInfo.Add(new CNV
                                                {
                                                    sValue = "Campaign creation failed"
                                                });
                                            }
                                        }
                                        else
                                        {
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oTraceInfo.Add(new CNV
                                            {
                                                sValue = "Comminication creation failed"
                                            });
                                        }
                                    }
                                    else
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oTraceInfo.Add(new CNV
                                        {
                                            sValue = "Comminication creation failed"
                                        });
                                    }
                                }
                            }
                            else
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oTraceInfo.Add(new CNV
                                {
                                    sValue = "QS Definition on Pilot is not configured"
                                });
                            }
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "Pilot loading failed"
                            });
                        }
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "Mandatory parameter PilotID is not passed"
                        });
                    }
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "Mandatory parameter Mobile numbers is not passed"
                    });
                }
                if (oCResult.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                {
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "ERROR: Send_Campaign method execution failed"
                    });
                    oCResult.oTraceStack = oTraceInfo;
                    SaveErrortoDB(oCResult);
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "success";
                }
            }
            catch (Exception ex)
            {
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oTraceInfo.Add(new CNV
                {
                    sValue = oCResult.sMessage
                });
                oCResult.oTraceStack = oTraceInfo;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Create_Campaign(string WhatsappAccountID, string SMSAccountID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Check for any user shared contacts";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIBO oCampI = new XIIBO();
                string sName = "Campaign";
                int iWhatsappAccountID = 0;
                int.TryParse(WhatsappAccountID, out iWhatsappAccountID);
                int iSMSAccountID = 0;
                int.TryParse(SMSAccountID, out iSMSAccountID);
                if (iWhatsappAccountID > 0 && iSMSAccountID > 0)
                {
                    XIIBO oAccount = new XIIBO();
                    XIIXI oXII = new XIIXI();
                    List<CNV> oWhr = new List<CNV>();
                    oWhr.Add(new CNV { sName = "ID", sValue = iWhatsappAccountID.ToString() });
                    oAccount = oXII.BOI("XIWhatsappAccount", null, null, oWhr);
                    if (oAccount != null && oAccount.Attributes.Count() > 0)
                    {
                        iWhatsappAccountID = oAccount.AttributeI("id").iValue;
                        sName = oAccount.AttributeI("sName").sValue;
                        oCampI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BSPCampaign");
                        oCampI.SetAttribute("sName", sName);
                        oCampI.SetAttribute("FKiWhatsappAccountID", iWhatsappAccountID.ToString());
                        oCampI.SetAttribute("FKiSMSAccountID", iSMSAccountID.ToString());
                        oCampI.SetAttribute("iType", "40");
                        oCampI.SetAttribute("FKiAppID", "0");
                        oCampI.SetAttribute("FKiOrgID", "0");
                        oCR = oCampI.Save(oCampI);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oCampI = (XIIBO)oCR.oResult;
                            oCResult.oResult = oCampI;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
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
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Create_Communication(string sMob, string QSDXIGUID, Guid BODXIGuid, int iBOIID, int iComType)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Check for any user shared contacts";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIBO oCommI = new XIIBO();
                oCommI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommunicationI");
                oCommI.SetAttribute("sTO", sMob);
                oCommI.SetAttribute("iSendStatus", "10");
                oCommI.SetAttribute("iDirection", "20");
                oCommI.SetAttribute("iComType", iComType.ToString());
                oCommI.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID);
                oCommI.SetAttribute("XIInstanceOriginID", iBOIID.ToString());
                oCommI.SetAttribute("XIInstanceDefinitionIDXIGUID", BODXIGuid.ToString());
                oCR = oCommI.Save(oCommI);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oCommI = (XIIBO)oCR.oResult;
                    oCResult.oResult = oCommI;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oCResult.oResult = false;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Create_TargetCampaignMap(string sMob, string FKiCampaignID, string iCommID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Check for any user shared contacts";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIBO oLCMapI = new XIIBO();
                oLCMapI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BSPLeadCampaignMap");
                oLCMapI.SetAttribute("FKiCampaignID", FKiCampaignID);
                oLCMapI.SetAttribute("iStatus", "20");
                oLCMapI.SetAttribute("iScheduled", "20");
                oLCMapI.SetAttribute("FKiCommunicationID", iCommID);
                oLCMapI.SetAttribute("sMobileNo", sMob);
                oLCMapI.SetAttribute("FKiOrgID", "0");
                oLCMapI.SetAttribute("FKiOrgID", "0");
                oCR = oLCMapI.Save(oLCMapI);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oLCMapI = (XIIBO)oCR.oResult;
                    oCResult.oResult = oLCMapI;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oCResult.oResult = false;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult SendCampaign(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Send Whatsapp or SMS campaign to User";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            List<CNV> oTraceInfo = new List<CNV>();
            try
            {
                var MobileNos = oParams.Where(m => m.sName.ToLower() == "mobileno").Select(m => m.sValue).FirstOrDefault();
                var WhatsappAccount = oParams.Where(m => m.sName.ToLower() == "WhatsappAccount".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var SMSAccount = oParams.Where(m => m.sName.ToLower() == "SMSAccount".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var BOIID = oParams.Where(m => m.sName.ToLower() == "iBOIID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var BODID = oParams.Where(m => m.sName.ToLower() == "iBODID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var PilotID = oParams.Where(m => m.sName.ToLower() == "iPilotID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIScheduleInstance");
                var MobNos = MobileNos.Split(',').ToList();
                int iPilotID = 0;
                int.TryParse(PilotID, out iPilotID);
                Guid QSDXIGUID = Guid.Empty;
                if (iPilotID > 0)
                {
                    XIIXI oXI = new XIIXI();
                    //Load Pilot
                    var oPilotI = oXI.BOI("BSPPilot", iPilotID.ToString());
                    if (oPilotI != null && oPilotI.Attributes.Count() > 0)
                    {
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "Pilot loaded for: " + iPilotID
                        });
                        var QSDGUID = oPilotI.AttributeI("FKiQSDIDXIGUID").sValue;
                        if (Guid.TryParse(QSDGUID, out QSDXIGUID))
                        {
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "QS Definition on Pilot is: " + QSDXIGUID
                            });
                        }
                    }
                }
                foreach (var sMob in MobNos)
                {
                    oCR = Create_Campaign(WhatsappAccount, SMSAccount);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var oCampI = (XIIBO)oCR.oResult;
                        oCResult.oResult = oCampI;
                        int iCampID = oCampI.AttributeI("id").iValue;
                        if (iCampID > 0)
                        {
                            var oBOI = new XIIBO();
                            oBOI.BOD = oBOD;
                            oBOI.SetAttribute("sMobileNo", sMob);
                            oBOI.SetAttribute("FKiCampaignID", iCampID.ToString());
                            oBOI.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                            oBOI.SetAttribute("FKiBODIDXIGUID", BODID);
                            oBOI.SetAttribute("FKiBOIID", BOIID);
                            oBOI.SetAttribute("iType", "0");
                            oBOI.SetAttribute("iStatus", "30");
                            oBOI.SetAttribute("dRunDate", DateTime.Now.ToString());
                            oCR = oBOI.Save(oBOI);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oBOI = (XIIBO)oCR.oResult;
                                oCResult.oResult = oBOI;
                                var iScheduleID = oBOI.AttributeI("id").iValue;
                                if (iScheduleID > 0)
                                {
                                    var sSessionID = HttpContext.Current.Session.SessionID;
                                    string sNewGUID = Guid.NewGuid().ToString();
                                    List<CNV> oNVsList = new List<CNV>();
                                    XIDAlgorithm oAlogD = new XIDAlgorithm();
                                    oNVsList.Add(new CNV { sName = "-iBOIID", sValue = iScheduleID.ToString() });
                                    oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                                    oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, "CF6520DC-00F7-426B-B287-5AF1883DEDF8");
                                    oAlogD.Execute_XIAlgorithm(sSessionID, sNewGUID);
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                oCResult.oResult = false;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}