using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraDistributionComponent : XIDefinitionBase
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
            oTrace.sTask = "Distribution Component gets the object instance, runs against 1Query, if 1Q condition is Success then executes Process controller";//expalin about this method logic
            try
            {
                var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var objInstanceID = oParams.Where(m => m.sName == XIConstant.Param_InstanceID).Select(m => m.sValue).FirstOrDefault();
                var DistributionDefID = oParams.Where(m => m.sName == "iDistributionDefID").Select(m => m.sValue).FirstOrDefault();
                var sGUID = Guid.NewGuid().ToString();
                //bool bMatch = false;
                oTrace.oParams.Add(new CNV { sName = "objInstanceID", sValue = objInstanceID });
                oTrace.oParams.Add(new CNV { sName = "DistributionDefID", sValue = DistributionDefID });
                if (!string.IsNullOrEmpty(objInstanceID) && !string.IsNullOrEmpty(DistributionDefID))//check mandatory params are passed or not
                {
                    XIInfraCache oCache = new XIInfraCache();
                    var oDistDef = (XIDDistribute)oCache.GetObjectFromCache(XIConstant.CacheDistribution, null, DistributionDefID);
                    if (oDistDef != null && oDistDef.DistributeLines.Count() > 0)
                    {
                        bool bSwitch = false;
                        bool bEach = false;
                        if (oDistDef.iType == (Int32)Enum.Parse(typeof(xiEnumSystem.EnumDistributionType), xiEnumSystem.EnumDistributionType.Switch.ToString()))
                        {
                            bSwitch = true;
                        }
                        else if (oDistDef.iType == (Int32)Enum.Parse(typeof(xiEnumSystem.EnumDistributionType), xiEnumSystem.EnumDistributionType.Each.ToString()))
                        {
                            bEach = true;
                        }
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, oDistDef.FKiBODID.ToString());
                        XIIXI oXI = new XIIXI();
                        var oBOI = oXI.BOI(oBOD.Name, objInstanceID);
                        if (oBOI != null && oBOI.Attributes.Count() > 0)
                        {
                            foreach (var item in oDistDef.DistributeLines.OrderBy(m => m.XIfOrder))
                            {
                                var i1ClickID = item.FKi1ClickIDXIGUID;
                                var iPCID = item.FKiProcessControllerIDXIGUID;
                                if (i1ClickID != null)
                                {
                                    var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                                    if (o1ClickD != null && o1ClickD.ID > 0)
                                    {
                                        var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                                        var oBODef = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickC.BOID.ToString());
                                        o1ClickC.bIsResolveFK = false;
                                        string sCondtion = oBODef.sPrimaryKey + "=" + objInstanceID;
                                        if (item.FKiRuleID > 0 || (item.FKiRuleIDXIGUID != null && item.FKiRuleIDXIGUID != Guid.Empty))
                                        {
                                            string sRuleCondition = string.Empty;
                                            List<string> Conditions = new List<string>();
                                            var oRuleD = (XIDRule)oCache.GetObjectFromCache(XIConstant.CacheXIRule, null, item.FKiRuleIDXIGUID.ToString());
                                            if (oRuleD != null && oRuleD.RuleGroups != null && oRuleD.RuleGroups.Count() > 0)
                                            {
                                                foreach (var rule in oRuleD.RuleGroups)
                                                {
                                                    if (rule.BOAttributeID > 0 && !string.IsNullOrEmpty(rule.sOperator) && !string.IsNullOrEmpty(rule.sValue))
                                                    {
                                                        var Attr = rule.BOAttributeID;
                                                        var oAttrD = oBOD.Attributes.Values.ToList().Where(m => m.ID == Attr).FirstOrDefault();
                                                        if (oAttrD != null && oAttrD.XIGUID != null && oAttrD.XIGUID != Guid.Empty)
                                                        {
                                                            var InstanceVal = oBOI.AttributeI(oAttrD.Name).sValue;
                                                            if (!string.IsNullOrEmpty(InstanceVal))
                                                            {
                                                                if (oAttrD.TypeID == 150)
                                                                {
                                                                    var dtActual = Convert.ToDateTime(InstanceVal).Date;
                                                                    if (rule.sValue.ToLower().EndsWith("weeks"))
                                                                    {
                                                                        var Weeks = rule.sValue.ToLower().Replace(" weeks", "").Replace("weeks", "");
                                                                        if (!string.IsNullOrEmpty(Weeks))
                                                                        {
                                                                            string sDTCondtion = string.Empty;
                                                                            int iWeek = 0;
                                                                            int.TryParse(Weeks, out iWeek);
                                                                            DateTime newDate = new DateTime();
                                                                            newDate.AddDays(iWeek * 7);
                                                                            sDTCondtion = newDate.Date.ToString();
                                                                            Conditions.Add(oAttrD.Name + rule.sOperator + sDTCondtion);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Conditions.Add(oAttrD.Name + rule.sOperator + rule.sValue);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (Conditions.Count() > 0)
                                                {
                                                    sRuleCondition = string.Join(" and ", Conditions);
                                                    sCondtion = sCondtion + " and " + sRuleCondition;
                                                }
                                            }
                                        }
                                        o1ClickC.sParentWhere = sCondtion;
                                        var oResult = o1ClickC.OneClick_Execute();
                                        if (oResult != null && oResult.Values.Count() > 0)
                                        {
                                            // bMatch = true;
                                            //Execute Process Controller
                                            var oNVsList = new List<CNV>();
                                            oNVsList.Add(new CNV { sName = "-sBO", sValue = oBODef.Name });
                                            oNVsList.Add(new CNV { sName = "-iBOIID", sValue = objInstanceID });
                                            oCache.SetXIParams(oNVsList, sGUID, sSessionID);
                                            XIDAlgorithm oAlgoD = new XIDAlgorithm();
                                            oAlgoD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, iPCID.ToString());
                                            XIDAlgorithm oAlgoC = new XIDAlgorithm();
                                            oAlgoC = (XIDAlgorithm)oAlgoD.Clone(oAlgoD);
                                            //oAlogD.sOrigin = "BOAction";
                                            //oAlogD.sOriginType = "UI";
                                            var oAlogParams = new List<CNV>();
                                            if (item.FKiParameterID > 0 || (item.FKiParameterIDXIGUID != null && item.FKiParameterIDXIGUID != Guid.Empty))
                                            {
                                                XIParameter oParamD = new XIParameter();
                                                if (item.FKiParameterIDXIGUID != null && item.FKiParameterIDXIGUID != Guid.Empty)
                                                {
                                                    oParamD = (XIParameter)oCache.GetObjectFromCache(XIConstant.CacheXIParamater, null, item.FKiParameterIDXIGUID.ToString());
                                                }
                                                else if (item.FKiParameterID > 0)
                                                {
                                                    oParamD = (XIParameter)oCache.GetObjectFromCache(XIConstant.CacheXIParamater, null, item.FKiParameterID.ToString());
                                                }
                                                if (oParamD != null && oParamD.XiParameterNVs != null && oParamD.XiParameterNVs.Count() > 0)
                                                {
                                                    foreach (var nv in oParamD.XiParameterNVs)
                                                    {
                                                        oAlogParams.Add(new CNV { sName = nv.Name, sValue = nv.Value });
                                                    }
                                                }
                                            }
                                            oCR = oAlgoC.Execute_XIAlgorithm(sSessionID, sGUID, oAlogParams);
                                            oCache.Clear_GuidCache(sSessionID, sGUID);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                //Set override or success status
                                                if (item.iOverrideStauts > 0 || !string.IsNullOrEmpty(oDistDef.iSuccessStatus))
                                                {
                                                    if (item.iOverrideStauts > 0)
                                                    {
                                                        oBOI.SetAttribute(oDistDef.sStatusAttribute, item.iOverrideStauts.ToString());
                                                    }
                                                    else if (!string.IsNullOrEmpty(oDistDef.iSuccessStatus))
                                                    {
                                                        oBOI.SetAttribute(oDistDef.sStatusAttribute, oDistDef.iSuccessStatus.ToString());
                                                    }
                                                    oCR = oBOI.Save(oBOI);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                                        oCResult.oResult = "Success";
                                                        if (bSwitch == true)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                        oTrace.sMessage = "Error while updating the success or override status";
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                                    oCResult.oResult = "Success";
                                                    if (bSwitch == true)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oTrace.sMessage = "Error in process controller execution";
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                }
                                SaveErrortoDB(oCResult);
                            }
                            //if (!bMatch)
                            //{
                            //    //Set Non assigned status
                            //    if (oDistDef.iNonAssignedStatus > 0)
                            //    {
                            //        oBOI.SetAttribute(oDistDef.sStatusAttribute, oDistDef.iNonAssignedStatus.ToString());
                            //        oCR = oBOI.Save(oBOI);
                            //        if (oCR.bOK && oCR.oResult != null)
                            //        {
                            //            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            //            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            //            oCResult.sMessage = "Error 8 while updating the success or override status LeaadID:" + objInstanceID + ", DistributionID:" + DistributionDefID;
                            //            oCResult.oResult = "Success";
                            //        }
                            //        else
                            //        {
                            //            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            //            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            //            oCResult.sMessage = "Error 9 while updating the success or override status LeaadID:" + objInstanceID + ", DistributionID:" + DistributionDefID;
                            //            oTrace.sMessage = "Error while updating non assigned status";
                            //        }
                            //    }
                            //    SaveErrortoDB(oCResult);
                            //}
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oTrace.sMessage = "Error while getting Distribution Definition";
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.sMessage = "Mandatory Params: objInstanceID or DistributionDefID missing";
                    oTrace.sMessage = "Mandatory Params: objInstanceID or DistributionDefID missing";
                }
                SaveErrortoDB(oCResult);
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
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