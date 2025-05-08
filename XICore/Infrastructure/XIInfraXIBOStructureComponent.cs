using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XICore
{
    public class XIInfraXIBOStructureComponent
    {
        public int iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sBOID { get; set; }
        public string sDisplayMode { get; set; }
        XIInfraCache oCache = new XIInfraCache();
        XIConfigs oConfig = new XIConfigs();

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
                string sID = "";
                int iBODID = 0;
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                string sType = oParams.Where(m => m.sName == "sType").Select(m => m.sValue).FirstOrDefault();
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
                    sBOID = WrapperParms.Where(m => m.sName == "iBOIID").Select(m => m.sValue).FirstOrDefault();
                    //if (int.TryParse(sBOID, out iBODID))
                    //{

                    //}
                    var XIPiFieldID = WrapperParms.Where(m => m.sName == "{-iInstanceID}").Select(m => m.sValue).FirstOrDefault(); // oXIAPI.Get_ParamVal(sSessionID, sGUID, null, Prm); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (XIPiFieldID != null)
                    {
                        sID=XIPiFieldID;
                    }
                    else
                    {
                        sID = "";
                    }
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
                        sInstanceID = sID;
                    }
                    if (String.IsNullOrEmpty(sID))
                    {
                        sID = oParams.Where(m => m.sName == "ID").Select(m => m.sValue).FirstOrDefault();
                    }
                    sBOID = oParams.Where(m => m.sName == XIConstant.Param_BODID).Select(m => m.sValue).FirstOrDefault();
                    int.TryParse(sBOID, out iBODID);
                    if (iBODID == 0)
                    {
                        var sBOID = oCache.Get_ParamVal(sSessionID, sGUID, null, "iBODID");
                        int.TryParse(sBOID, out iBODID);
                    }
                }
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oParams.Where(m => m.sName == "sCoreDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                var IDEParams = oCache.Get_Paramobject(sSessionID, sGUID, null, "IDEParams");
                if (IDEParams != null && IDEParams.nSubParams != null && IDEParams.nSubParams.Count() > 0)
                {
                    var sParentBO = IDEParams.nSubParams.Where(m => m.sName.ToLower() == XIConstant.Param_ParentBO.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sParentBO))
                    {
                        int iParentBOIID = 0;
                        var sParentBOIID = IDEParams.nSubParams.Where(m => m.sName.ToLower() == XIConstant.Param_ParentBOIID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        int.TryParse(sParentBOIID, out iParentBOIID);
                        if (iParentBOIID > 0)
                        {
                            iBODID = iParentBOIID;
                        }
                        sBOID = sParentBOIID;
                    }
                }
                List<XIDStructure> oFDef = new List<XIDStructure>();
                XIDXI oXD = new XIDXI();

                if (string.IsNullOrEmpty(sType))
                {
                    sType = "Create";
                    if (!string.IsNullOrEmpty(sID))
                    {
                        sType = "Edit";
                    }
                }

                if (sType == "Refresh")
                {
                    var ExistingNodes = oXD.Get_XIBOStructureDefinition(sBOID, sID, sType);
                    var AllNodes = oXD.Get_XIBOStructureDefinition(sBOID, "", "Create");
                    var All = new List<XIDStructure>();
                    var Existing = new List<XIDStructure>();
                    var Remaining = new List<XIDStructure>();
                    if (AllNodes.bOK && AllNodes.oResult != null)
                    {
                        All = (List<XIDStructure>)AllNodes.oResult;
                    }
                    if (ExistingNodes.bOK && ExistingNodes.oResult != null)
                    {
                        Existing = (List<XIDStructure>)ExistingNodes.oResult;
                    }
                    if (All != null && All.Count() > 0)
                    {
                        foreach (var items in All)
                        {
                            var Exists = Existing.Where(m => m.BOID == items.BOID).FirstOrDefault();
                            if (Exists == null)
                            {
                                Remaining.Add(items);
                            }
                        }
                    }
                    oFDef = Remaining;
                }
                else
                {
                    var oComDef = oXD.Get_XIBOStructureDefinition(sBOID, sID, sType);
                    if (oComDef.bOK && oComDef.oResult != null)
                    {
                        oFDef = (List<XIDStructure>)oComDef.oResult;
                        if (sType == "Create")
                        {
                            oFDef.ForEach(m => m.sSavingType = "Add");
                        }
                    }
                }
                if(oFDef != null && oFDef.Count() == 0){
                    oFDef.Add(new XIDStructure());
                }
                if (sBOID != "0")
                {
                    oFDef.FirstOrDefault().BOID = oFDef.FirstOrDefault().BOID;
                    if (oFDef.FirstOrDefault().BOID == 0)
                    {
                        if (iBODID > 0)
                        {
                            oFDef.FirstOrDefault().BOID = Convert.ToInt32(sBOID);
                        }
                        else
                        {
                            var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "", sBOID);
                            oFDef.FirstOrDefault().BOID = oBOD.BOID;
                        }
                    }
                }
                oFDef.FirstOrDefault().ddlAllBOs = oConfig.Get_AllBOs();
                oCResult.oResult = oFDef;
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