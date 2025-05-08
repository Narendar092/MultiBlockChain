using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XICore;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIInfraGroupComponent
    {
        public string sBOName { get; set; }
        public int iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sDisplayMode { get; set; }


        XIInfraCache oCache = new XIInfraCache();
        XIConfigs oConfig = new XIConfigs();

        public CResult XILoad(List<CNV> oParams)
        {

            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            XIDGroup oXIDGroup = new XIDGroup();
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
                long iBODID = 0;
                long iBOIID = 0;
                Guid BOIDXIGUID = Guid.Empty;
                var sID = "";
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                var sIDRef = oParams.Where(m => m.sName == "sIDRef").Select(m => m.sValue).FirstOrDefault();
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
                var sParentInsID = string.Empty;
                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    sParentInsID = WrapperParms.Where(m => m.sName == "ParentInsID").Select(m => m.sValue).FirstOrDefault();
                    if (string.IsNullOrEmpty(sParentInsID))
                    {
                        var sBODID = WrapperParms.Where(m => m.sName == "iBODID").Select(m => m.sValue).FirstOrDefault();
                        long.TryParse(sBODID, out iBODID);
                        var sBOIDGUID = WrapperParms.Where(m => m.sName == "BOIDXIGUID").Select(m => m.sValue).FirstOrDefault();
                        Guid.TryParse(sBOIDGUID, out BOIDXIGUID);
                    }
                    else
                    {
                        long.TryParse(sParentInsID, out iBODID);
                        if (!string.IsNullOrEmpty(sIDRef) && sIDRef == "xiguid" && iBODID == 0)
                        {
                            var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "", sParentInsID);
                            if (BOD != null && BOD.Attributes.Count > 0 && BOD != null)
                            {
                                iBODID = BOD.BOID;
                                BOIDXIGUID = BOD.XIGUID;
                            }
                        }
                    }
                    sID = WrapperParms.Where(m => m.sName.ToLower() == XIConstant.Param_InstanceID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    //long.TryParse(sID, out iBOIID);
                }
                else
                {
                    string sInstanceID = oParams.Where(m => m.sName == XIConstant.Param_InstanceID).Select(m => m.sValue).FirstOrDefault();
                    if (sInstanceID != null && (sInstanceID.StartsWith("{XIP|") || sInstanceID.StartsWith("-") || sInstanceID.StartsWith("{-")))
                    {
                        sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, sInstanceID);
                    }
                    if (!string.IsNullOrEmpty(sInstanceID))
                    {
                        sID = sInstanceID;
                        //if (long.TryParse(sInstanceID, out iBOIID))
                        //{

                        //}
                    }
                    var sBOID = oCache.Get_ParamVal(sSessionID, sGUID, null, "iBODID");
                    long.TryParse(sBOID, out iBODID);
                }
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                XIDGroup cXIDGroup = new XIDGroup();
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Params loaded successfully for Group Component" });
                Guid BOXIGUID = Guid.Empty;
                Guid.TryParse(sParentInsID, out BOXIGUID);
                cXIDGroup.BOIDXIGUID = BOIDXIGUID;
                cXIDGroup.BOID = Convert.ToInt32(iBODID);
                cXIDGroup.sID = sID;

                var CacheKey = oCache.CacheKeyBuilder(XIConstant.CacheBO, null, BOIDXIGUID.ToString());
                XIInfraCache.RemoveCacheWithKey(CacheKey);
                oXIDGroup = cXIDGroup.Get_GroupDetails();
                if (iBODID != 0 || (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty))
                {
                    oXIDGroup.BOID = oXIDGroup.BOID;
                    if (oXIDGroup.BOID == 0 || oXIDGroup.XIGUID != Guid.Empty)
                    {
                        oXIDGroup.BOID = Convert.ToInt32(iBODID);
                        //oXIDGroup.BOIDXIGUID = BOIDXIGUID;
                    }
                }
                oXIDGroup.ddlAllBOs = oConfig.Get_AllBOs();
                oCResult.oResult = oXIDGroup;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Form Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
    }
}