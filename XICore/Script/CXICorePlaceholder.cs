using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class CXICorePlaceholder
    {
        XIInfraCache oCache = new XIInfraCache();

        public CResult BOCreate(string sBOName)
        {
            CResult oCResult = new CResult();
            XIIBO oBOI = new XIIBO();
            var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
            oBOI.BOD = BOD;
            oBOI.iBODID = BOD.BOID;
            oBOI.sBOName = BOD.Name;
            oCResult.oResult = oBOI;
            return oCResult;
        }

        public CResult BOLoad(string sBOName, List<CNV> WhrParams)
        {
            CResult oCResult = new CResult();
            //List<CNV> WhrParams = new List<CNV>();
            //if (sUID.Contains('='))
            //{
            //    var NVs = sUID.Split('=').ToList();
            //    WhrParams.Add(new CNV { sName = NVs[0], sValue = NVs[1] });
            //    sUID = string.Empty;
            //}

            XIIXI oXI = new XIIXI();
            XIIBO oBOI = new XIIBO();
            XIDBO oBOD = new XIDBO();
            Guid BOIDXIGUID = Guid.Empty;
            Guid.TryParse(sBOName, out BOIDXIGUID);
            if (WhrParams != null && WhrParams.Count() > 0)
            {
                var Convert = WhrParams.Where(m => m.sName.Contains("date(")).ToList();
                if (Convert != null)
                {
                    foreach (var items in Convert)
                    {
                        var Param = items.sName.Replace("date(", "").Replace(")", "");
                        Param = "Convert(date, " + Param + ")";
                        WhrParams.Where(m => m.sName == items.sName).FirstOrDefault().sName = Param;
                    }
                }
                oBOI = oXI.BOI(sBOName, null, null, WhrParams);
            }
            else if(BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
            {
                oBOI = new XIIBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOIDXIGUID.ToString());
                oBOI.BOD = oBOD;
            }
            else
            {
                oBOI = new XIIBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                oBOI.BOD = oBOD;
            }

            if (oBOI == null || oBOI.Attributes.Count() == 0)
            {
                if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
                {
                    oBOI = new XIIBO();
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOIDXIGUID.ToString());
                    oBOI.BOD = oBOD;
                }
                else
                {
                    oBOI = new XIIBO();
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                    oBOI.BOD = oBOD;
                }
                
            }
            oCResult.oResult = oBOI;
            return oCResult;
        }

        public CResult BOSave(XIIBO oBO, string sGroup = "", bool bValidate = true, iSiganlR oSignalR = null, bool bAction=true, string ContextValue="")
        {
            CResult oCResult = new CResult();
            // Warning!!! Optional parameters not supported
            if (oBO.BOD == null && oBO.iBODID > 0)
            {
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, oBO.iBODID.ToString());
                oBO.BOD = oBOD;
            }
            oBO.oSignalR = oSignalR;
            var oCR = oBO.Save(oBO, bValidate, null, null, null, bAction, ContextValue);
            oBO.sBOName = ("SAVE: " + oBO.sBOName);
            oCResult.oResult = "xiSuccess";
            return oCResult;
        }

        // BO1Click
        public CResult BO1Click(string s1ClickName, string WhrParams, string sSessionID = "", string sGUID = "")
        {
            CResult oCResult = new CResult();
            var o1ClickD = oCache.GetObjectFromCache(XIConstant.Cache1Click, s1ClickName);
            XIDefinitionBase oBase = new XIDefinitionBase();
            var o1ClickC = (XID1Click)oBase.Clone(o1ClickD);
            if (!string.IsNullOrEmpty(WhrParams))
            {
                var Query = o1ClickC.AddSearchParameters(o1ClickC.Query, WhrParams);
                o1ClickC.Query = Query;
            }            
            XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
            List<CNV> nParms = new List<CNV>();
            nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
            o1ClickC.ReplaceFKExpressions(nParms);
            var oCR = o1ClickC.OneClick_Execute();
            //return oCResult;


            //long j;
            //XIIBO oBOI = null;
            BOInstanceGridPlaceholder oNewBOInstPlaceholder = new BOInstanceGridPlaceholder();
            oNewBOInstPlaceholder.oBOInstances = new Dictionary<string, XIIBO>();
            oNewBOInstPlaceholder.oBOInstances = oCR;
            oCResult.oResult = oNewBOInstPlaceholder;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            return oCResult;

            //oNewBOInstPlaceholder.sName = "1 Click";
            //for (j = 1; (j <= 2); j++)
            //{
            //    oBOI = new XIIBO();
            //    //oBOI.sName = ("Sub Item " + j);
            //    oNewBOInstPlaceholder.oBOInstances.Add(("Item" + j), oBOI);
            //}

            //CResult oCResult = new CResult();
            //long j;
            //XIIBO oBOI = null;
            //BOInstanceGridPlaceholder oNewBOInstPlaceholder = new BOInstanceGridPlaceholder();
            //oNewBOInstPlaceholder.sName = "1 Click";
            //XID1Click oXID1Click = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, s1ClickName, null);
            //XID1Click oXID1ClickClone = (XID1Click)oXID1Click.Clone(oXID1Click);
            //oCResult.oResult = oXID1ClickClone.OneClick_Execute();
            //for (j = 1; (j <= ((Dictionary<string, XIIBO>)oCResult.oResult).Count()); j++)
            //{
            //    oBOI = new XIIBO();
            //    //oBOI.sName = ("Sub Item " + j);
            //    oNewBOInstPlaceholder.oBOInstances.Add(("Item" + j), oBOI);
            //}
            ////oCResult.oResult = oXID1Click; // oNewBOInstPlaceholder;
            //return oCResult;
        }

        public CResult BO1ClickRun(string s1ClickName, string WhrParams, string sSessionID = "", string sGUID = "")
        {
            CResult oCResult = new CResult();
            var o1ClickD = oCache.GetObjectFromCache(XIConstant.Cache1Click, s1ClickName);
            XIDefinitionBase oBase = new XIDefinitionBase();
            var o1ClickC = (XID1Click)oBase.Clone(o1ClickD);
            if (!string.IsNullOrEmpty(WhrParams))
            {
                var Query = o1ClickC.AddSearchParameters(o1ClickC.Query, WhrParams);
                o1ClickC.Query = Query;
            }
            XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
            List<CNV> nParms = new List<CNV>();
            nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
            o1ClickC.ReplaceFKExpressions(nParms);
            var oCR = o1ClickC.OneClick_Run();
            //return oCResult;


            //long j;
            //XIIBO oBOI = null;
            BOInstanceGridPlaceholder oNewBOInstPlaceholder = new BOInstanceGridPlaceholder();
            oNewBOInstPlaceholder.oBOInstances = new Dictionary<string, XIIBO>();
            oNewBOInstPlaceholder.oBOInstances = oCR;
            oCResult.oResult = oNewBOInstPlaceholder;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            return oCResult;

            //oNewBOInstPlaceholder.sName = "1 Click";
            //for (j = 1; (j <= 2); j++)
            //{
            //    oBOI = new XIIBO();
            //    //oBOI.sName = ("Sub Item " + j);
            //    oNewBOInstPlaceholder.oBOInstances.Add(("Item" + j), oBOI);
            //}

            //CResult oCResult = new CResult();
            //long j;
            //XIIBO oBOI = null;
            //BOInstanceGridPlaceholder oNewBOInstPlaceholder = new BOInstanceGridPlaceholder();
            //oNewBOInstPlaceholder.sName = "1 Click";
            //XID1Click oXID1Click = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, s1ClickName, null);
            //XID1Click oXID1ClickClone = (XID1Click)oXID1Click.Clone(oXID1Click);
            //oCResult.oResult = oXID1ClickClone.OneClick_Execute();
            //for (j = 1; (j <= ((Dictionary<string, XIIBO>)oCResult.oResult).Count()); j++)
            //{
            //    oBOI = new XIIBO();
            //    //oBOI.sName = ("Sub Item " + j);
            //    oNewBOInstPlaceholder.oBOInstances.Add(("Item" + j), oBOI);
            //}
            ////oCResult.oResult = oXID1Click; // oNewBOInstPlaceholder;
            //return oCResult;
        }
        public CResult BOCopy(XIIBO oBOFrom, XIIBO oBOTo, string sCopyGroup = "")
        {
            CResult oCResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Dim oNewBOPlaceholder As New CBOPlaceholder
            // oNewBOPlaceholder.sName = "BO COPY"
            // oCResult.oResult = oNewBOPlaceholder
            var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, oBOFrom.iBODID.ToString());
            foreach (var attr in oBOFrom.Attributes.Values.ToList())
            {
                oBOTo.SetAttribute(attr.sName, attr.sValue);
            }
            oBOTo.SetAttribute(BOD.sPrimaryKey, "");
            oBOTo.sBOName = oBOTo.sBOName + ": COPY FROM: " + oBOFrom.sBOName;
            oCResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnum.xiFuncResult.xiSuccess;
            oCResult.oResult = "xiSuccess";
            return oCResult;
        }

        public CResult BOSetValue(XIIBO oBO, string sValueMethod)
        {
            CResult oCResult = new CResult();
            sValueMethod = sValueMethod.Replace("[", "").Replace("]", "");

            if (sValueMethod.Contains('='))
            {
                var NV = sValueMethod.Split('=').ToList();
                if (NV.Count() > 2)
                {
                    oBO.SetAttribute(NV[0], sValueMethod.Replace(NV[0] + "=", ""));
                }
                else if (NV.Count() == 2)
                {
                    if (string.IsNullOrEmpty(NV[1]))
                    {
                        NV[1] = null;
                    }
                    oBO.SetAttribute(NV[0], NV[1]);
                }
            }

            oBO.sBOName = oBO.sBOName + ": S:" + sValueMethod;
            oCResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnum.xiFuncResult.xiSuccess;
            oCResult.oResult = "xiSuccess";
            return oCResult;
        }

        //QS Load
        public CResult QSLoad(string sUID, string sSessionID, string sGUID)
        {
            CResult oCResult = new CResult();
            CXIAPI oAPI = new CXIAPI();
            var oCR = oAPI.QSI(sUID, sGUID, sSessionID);
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess)
            {
                oCResult.oResult = oCR.oResult;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            else
            {
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            }
            return oCResult;
        }

        //QS Insert
        public CResult QSInsert(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                CXIAPI oAPI = new CXIAPI();
                oCR = oAPI.Insert_QSI(oParams);
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
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        //Copy Structure
        public CResult Copy_Structure(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                CXIAPI oAPI = new CXIAPI();
                oCR = oAPI.Copy_Structure(oParams);
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
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        //Copy Structure
        public CResult Call_OM(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                CXIAPI oAPI = new CXIAPI();
                oCR = oAPI.Call_OM(oParams);
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
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        //Get FK attr to Ojbect
        public CResult Get_FKToObject(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                CXIAPI oAPI = new CXIAPI();
                oCR = oAPI.Get_FKToObject(oParams);
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
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}