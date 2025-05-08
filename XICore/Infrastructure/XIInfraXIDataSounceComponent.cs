using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XICore
{
    public class XIInfraXIDataSounceComponent
    {
        public int iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sDisplayMode { get; set; }
        public int iApplicationID { get; set; }
        public int iBOIID { get; set; }
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
                int ID = 0;
                Guid XIGUID = Guid.Empty;
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                var AppGUID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_ApplicationXIGUID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var AppXIGUID = Guid.Empty;
                Guid.TryParse(AppGUID, out AppXIGUID);
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
                    var XIPiFieldID = WrapperParms.Where(m => m.sName == "{-iInstanceID}").Select(m => m.sValue).FirstOrDefault(); // oXIAPI.Get_ParamVal(sSessionID, sGUID, null, Prm); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    int.TryParse(XIPiFieldID, out ID);
                    Guid.TryParse(XIPiFieldID, out XIGUID);
                    //var sBOIID = WrapperParms.Where(m => m.sName == XIConstant.Param_BOIID).Select(m => m.sValue).FirstOrDefault();
                    //if (sBOIID != null)
                    //{
                    //    iBOIID = Convert.ToInt32(sBOIID);
                    //}
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
                        if (int.TryParse(sInstanceID, out ID))
                        {

                        }
                    }
                }
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                XIDataSource oFDef = new XIDataSource();
                XIDXI oXD = new XIDXI();
                oXD.sTypeCyption = "Decrypt";
                //oXD.FKiApplicationID = iBOIID;
                if(XIGUID != null && XIGUID != Guid.Empty)
                {
                    oCR = oXD.Get_DataSourceDefinition(null, XIGUID.ToString());
                }
                else if (ID > 0)
                {
                    oCR = oXD.Get_DataSourceDefinition(null, ID.ToString());
                }
                //var oComDef = oXD.Get_DataSourceDefinition(null, ID.ToString());
                if (oCR.bOK && oCR.oResult != null)
                {
                    oFDef = (XIDataSource)oCR.oResult;
                }
                oFDef.ddlApplications = oConfig.Get_AllApplications();
                oFDef.FKiApplicationIDXIGUID = AppXIGUID;
                oCResult.oResult = oFDef;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Data source Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
    }
}