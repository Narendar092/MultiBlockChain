using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraDynamicTreeComponent : XIDefinitionBase
    {
        string sBOName = string.Empty;
        string sTableName = string.Empty;
        List<XIIBO> oResult2 = new List<XIIBO>();
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            List<XIDStructure> oTree = new List<XIDStructure>();
            XIInfraCache oMyCache = new XIInfraCache();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                sBOName = oParams.Where(m => m.sName.ToLower() == "bo").Select(m => m.sValue).FirstOrDefault();
                var sSessionID = oParams.Where(m => m.sName.ToLower() == "ssessionid").Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName.ToLower() == "sguid").Select(m => m.sValue).FirstOrDefault();
                Dictionary<string, XIIBO> oResult1 = new Dictionary<string, XIIBO>();
                var InstanceID = oParams.Where(m => m.sName.ToLower() == "instanceid").Select(m => m.sValue).FirstOrDefault();
                var Mode = oParams.Where(m => m.sName.ToLower() == "mode").Select(m => m.sValue).FirstOrDefault();
                string sCondition = string.Empty;
                    if (!string.IsNullOrEmpty(Mode) && !string.IsNullOrEmpty(sBOName))
                    {
                        if(Mode.ToLower() == "view")
                        {
                            sCondition = "FKiCampaignID=" + InstanceID;
                        }
                        else if (Mode.ToLower() == "config")
                        {
                            if (string.IsNullOrEmpty(InstanceID))
                            {
                                InstanceID = oMyCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|" + sBOName + "}");
                            }
                                 sCondition = "ID=" + InstanceID;
                        }
                    }
                
             
                if (!string.IsNullOrEmpty(sCondition))
                {

                    oTrace.oParams.Add(new CNV { sName = "sBOName", sValue = sBOName });
                    if (!string.IsNullOrEmpty(sBOName))//check mandatory params are passed or not
                    {
                        XIInfraCache oCache = new XIInfraCache();
                        var oBOD =(XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                        sTableName = oBOD.TableName;
                        XIIXI oXI = new XIIXI();
                        XID1Click oD1Click = new XID1Click();
                        string sQuery = string.Empty;
                        oD1Click.sParentWhere = sCondition;
                        sQuery = "select * from " + sTableName + " WHERE iParentID ='0' and "+ XIConstant.Key_XIDeleted + " = 0";
                        oD1Click.Query = sQuery;
                        oD1Click.Name = sBOName;
                        oResult1 = oD1Click.OneClick_Execute();
                        TreeBuilding(oResult1, sBOName, sTableName);
                        if (oResult2.Count() > 0)
                        {
                            foreach (var item in oResult2)
                            {
                                oTree.Add(new XIDStructure { ID = item.AttributeI("id").iValue, FKiParentID = item.AttributeI("iParentID").sValue, sName = item.AttributeI("sName").sValue, sBO = sBOName });
                            }
                        }
                        oCR.oResult = oTree;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.oTrace.Add(oCR.oTrace);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                            
                                oCResult.oResult = oCR.oResult;
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                        oTrace.sMessage = "Mandatory Param:  is missing";
                    }
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult TreeBuilding(Dictionary<string, XIIBO> oList, string sBOName, string sTableName)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            List<XIDStructure> oTree = new List<XIDStructure>();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                if (oList != null)
                {
                    foreach (var items in oList.Values)
                    {
                        if (items.Attributes.Values.ToList().Any(m => m.sName.ToLower() == "iparentid" && m.sValue == "0"))
                        {
                            items.Attributes.Values.ToList().Where(m => m.sName.ToLower() == "iparentid" && m.sValue == "0").FirstOrDefault().sValue = "#";
                        }
                        var ID = items.Attributes["id"].sValue;
                        XID1Click oC1Click = new XID1Click();
                        string sCQuery = string.Empty;
                        sCQuery = "select * from " + sTableName + " WHERE iParentID ='" + ID + "' and "+ XIConstant.Key_XIDeleted + " = 0";
                        oC1Click.Query = sCQuery;
                        oC1Click.Name = sBOName;
                        var oCRes = oC1Click.OneClick_Run(false);
                        if (oCRes != null && oCRes.Values.Count() > 0)
                        {
                            items.bHasChilds = true;
                            //oResul2.Add(oCRes.Values);
                            oResult2.Add(items);
                            TreeBuilding(oCRes, sBOName, sTableName);
                        }
                        else
                        {
                            oResult2.Add(items);
                        }
                    }
                    oCR.oResult = oResult2;
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.oTrace.Add(oCR.oTrace);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = oCR.oResult;
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param:  is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}