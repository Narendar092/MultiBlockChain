using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XISystem;

namespace XICore
{
    public class XIInfraSmartSearchComponent
    {
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic.
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                var sType = oParams.Where(m => m.sName.ToLower() == "type").Select(m => m.sValue).FirstOrDefault();

                XIInfraCache oCache = new XIInfraCache();
                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                var iInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iInstanceID}");
                var sBO = oCache.Get_ParamVal(sSessionID, sGUID, null, "BOIDXIGUID");
                var AttrDGuid = oCache.Get_ParamVal(sSessionID, sGUID, null, "AttrDXIGUID");
                Guid AttrDXIGuid = Guid.Empty;
                Guid.TryParse(AttrDGuid, out AttrDXIGuid);
                string sLabel = string.Empty;
                var oBODef = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, sBO);
                var oAttrD = oBODef.Attributes.Values.ToList().Where(m => m.XIGUID == AttrDXIGuid).FirstOrDefault();
                XIIBO oBOI = new XIIBO();
                oCR = oBOI.Get_BODialogLabel(sBO, iInstanceID);
                if (oCR.bOK && oCR.oResult != null)
                {
                    sLabel = (string)oCR.oResult;
                }
                else
                {
                    sLabel = iInstanceID;
                }
                Dictionary<string, object> Data = new Dictionary<string, object>();
                Data["Label"] = sLabel;
                Data["InstanceID"] = iInstanceID;
                Data["Type"] = sType;
                Data["BODGuid"] = sBO;
                Data["AttrDGuid"] = AttrDGuid;
                if(oAttrD != null)
                {
                    Data["AttrName"] = oAttrD.Name;
                }
                List<CNV> oNVs = new List<CNV>();
                XID1Click o1Click = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                o1Click = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Smart Search");
                o1ClickC = (XID1Click)o1Click.Clone(o1Click);
                if (!string.IsNullOrEmpty(sType))
                {
                    o1Click.Query = o1Click.AddSearchParameters(o1Click.Query, "sCode='" + sType + "'");
                }
                var oResult = o1Click.OneClick_Execute();
                if (oResult != null && oResult.Count() > 0)
                {
                    foreach (var obj in oResult.Values.ToList())
                    {
                        var objGUID = obj.AttributeI("FKiBODIDXIGUID").sValue;
                        var oneClickGUID = obj.AttributeI("FKi1ClickIDXIGUID").sValue;
                        var PCGUID = obj.AttributeI("FKiPCDIDXIGUID").sValue;
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, objGUID);
                        oNVs.Add(new CNV { sName = oBOD.LabelName, sValue = oneClickGUID, sType = oBOD.Name.ToString(), sContext = PCGUID, sLabel = (oBOD.BOUID == null ? null : oBOD.BOUID.iPopupID.ToString()) });
                    }
                    Data["NVs"] = oNVs;
                }
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = Data;
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
    }
}
