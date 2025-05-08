using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIDBOAction : XIDefinitionBase
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public string sName { get; set; }
        public string sCode { get; set; }
        public int iType { get; set; }
        public int iRunType { get; set; }
        public int iStatus { get; set; }
        public int iSystemAction { get; set; }
        public int FKiBOID { get; set; }
        public int FKiActionID { get; set; }
        public int FKiScriptID { get; set; }
        public int FKiAlgorithmID { get; set; }
        public int FKiXILinkID { get; set; }
        public int FKiQSDefinitionID { get; set; }
        public bool bMatrix { get; set; }
        public Guid FKiQSDefinitionIDXIGUID { get; set; }
        public Guid FKiActionIDXIGUID { get; set; }
        public Guid FKiScriptIDXIGUID { get; set; }
        public Guid FKiAlgorithmIDXIGUID { get; set; }
        public bool bISConfirm { get; set; }
        public string sConfirmAttributes { get; set; }
        public string sKeyBoardAccess { get; set; }
        public Guid FKiXILinkIDXIGUID { get; set; }
        public string sMethodProject { get; set; }
        public string sMethodURL { get; set; }
        public string sMethodName { get; set; }
        public List<XIDBOActionNV> ActionNV { get; set; }
        public CResult Execute_Action(XIIBO oBOI, string sPrevGUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Executes BO action like calling Process controller after saving the instance";//expalin about this method logic
            XIInfraCache oCache = new XIInfraCache();
            List<CNV> oTraceInfo = new List<CNV>();
            XIInfraUsers oUser = new XIInfraUsers();
            CUserInfo oInfo = new CUserInfo();
            oInfo = oUser.Get_UserInfo();
            var AppID = oInfo.iApplicationID;
            var iOrgID = oInfo.iOrganizationID;
            var iRoleID = oInfo.iRoleID;
            var sTraceLog = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + "TraceLog");
            try
            {
                oTrace.oParams.Add(new CNV { sName = "ID", sValue = ID.ToString() });
                oTraceInfo.Add(new CNV { sValue = "Mandatory parameter bo action instance uid is: " + ID });
                if (ID > 0)//check mandatory params are passed or not
                {
                    var sGUID = Guid.NewGuid().ToString();
                    List<CNV> oNVsList = new List<CNV>();
                    string sSessionID = string.Empty;
                    sSessionID = HttpContext.Current.Session.SessionID;
                    if (ActionNV != null && ActionNV.Count() > 0)
                    {
                        foreach (var items in ActionNV)
                        {
                            if (items.sValue.StartsWith("{XIP"))
                            {
                                oCR.oResult = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sPrevGUID, null, items.sValue)).ToString();
                            }
                            else
                            {
                            oCR = oBOI.Resolve_Notation(items.sValue);
                            }
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oTrace.oTrace.Add(oCR.oTrace);
                                var sResolvedVal = (string)oCR.oResult;
                                oNVsList.Add(new CNV { sName = items.sName, sValue = sResolvedVal });
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            }
                        }

                    }
                    if (oTrace.iStatus != (int)xiEnumSystem.xiFuncResult.xiError)
                    {
                        if (string.IsNullOrEmpty(sSessionID))
                        {
                            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session.SessionID != null)
                            {
                                sSessionID = HttpContext.Current.Session.SessionID;
                            }
                        }
                        string sIDRef = string.Empty;
                        sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + iOrgID + "_" + iRoleID + "_" + XIConstant.IDRef_internal);
                        if (sIDRef == "xiguid")
                        {
                            var Value = oBOI.Attributes.FirstOrDefault().Value;
                            oTraceInfo.Add(new CNV { sValue = "Mandatory parameter process controller instance uid is: " + FKiAlgorithmID });
                            oNVsList.Add(new CNV { sName = "-iBOIID", sValue = Value.iValue.ToString() });
                            oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                        }
                        else
                        { 
                        oTraceInfo.Add(new CNV { sValue = "Mandatory parameter process controller instance uid is: " + FKiAlgorithmID });
                        oNVsList.Add(new CNV { sName = "-iBOIID", sValue = oBOI.Attributes[oBOI.BOD.sPrimaryKey.ToLower()].sValue });
                        oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                        }
                        XIDAlgorithm oAlogD = new XIDAlgorithm();
                        if(FKiAlgorithmIDXIGUID != null && FKiAlgorithmIDXIGUID != Guid.Empty)
                        {
                            oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, FKiAlgorithmIDXIGUID.ToString());
                        }
                        else if(FKiAlgorithmID > 0)
                        {
                            oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, FKiAlgorithmID.ToString());
                        }                        
                        oCR = oAlogD.Execute_XIAlgorithm(sSessionID, sGUID);
                        oCache.Clear_GuidCache(sSessionID, sGUID);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oTrace.sMessage = "Success";
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oTrace.sMessage = "Error while executing process controller:" + oAlogD.ID;
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oTrace.sMessage = "Mandatory Param: BO Action ID: " + ID + " is missing";
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
                //oTraceInfo.Add(new CNV { sValue = oCResult.sMessage });
                SaveErrortoDB(oCResult);
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
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }

    public class XIDBOActionNV
    {
        public int ID { get; set; }
        public int FKiBOActionID { get; set; }
        public Guid FKiBOActionIDXIGUID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
    }
}