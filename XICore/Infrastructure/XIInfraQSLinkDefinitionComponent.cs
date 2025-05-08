using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraQSLinkDefinationComponent
    {
        public int iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sDisplayMode { get; set; }
        public int ID { get; set; }
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
                long iID = 0;
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sGUID))
                {
                    oCache.sSessionID = sSessionID;
                    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                    sGUID = ParentGUID;
                }
                var WrapperParms = new List<CNV>();
                var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam1".ToLower())).ToList();
                if (WatchParam.Count() > 0)
                {
                    foreach (var items in WatchParam)
                    {
                        if (!string.IsNullOrEmpty(items.sValue))
                        {
                            var Prams = oCache.Get_Paramobject(sSessionID, sGUID, null, items.sValue); //oParams.Where(m => m.sName == items.sValue).FirstOrDefault();
                            if (Prams != null)
                            {
                                WrapperParms = Prams.nSubParams;
                            }
                        }
                    }
                }
                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    var XIPiID = WrapperParms.Where(m => m.sName == "{-iInstanceID}").Select(m => m.sValue).FirstOrDefault(); // oXIAPI.Get_ParamVal(sSessionID, sGUID, null, Prm); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (XIPiID != null)
                    {
                        ID = Convert.ToInt32(XIPiID);
                    }
                    else
                    {
                        ID = 0;
                    }
                }
                else
                {
                    string sInstanceID = oParams.Where(m => m.sName == "ID").Select(m => m.sValue).FirstOrDefault();
                    if (sInstanceID != null && (sInstanceID.StartsWith("{XIP|") || sInstanceID.StartsWith("-") || sInstanceID.StartsWith("{-")))
                    {
                        sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, sInstanceID);
                    }
                    if (!string.IsNullOrEmpty(sInstanceID))
                    {
                        if (long.TryParse(sInstanceID, out iID))
                        {

                        }
                    }
                }
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                XIQSLinkDefintion oLinkDef = new XIQSLinkDefintion();
                oLinkDef.ID = ID;
                var oLDef = oLinkDef.Get_QSLinkDefinition();
                if (oLDef.bOK && oLDef.oResult != null)
                {
                    oLinkDef = (XIQSLinkDefintion)oLDef.oResult;
                }
                oCResult.oResult = oLinkDef;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Dialog Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
    }
}