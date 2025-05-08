using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XICore;
using XISystem;

namespace XICore
{
    public class XIInfraQuoteReportDataComponent
    {

        public string iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sDisplayMode { get; set; }
        public int OneClickID;

        XIInfraCache oCache = new XIInfraCache();

        public CResult XILoad(List<CNV> oParams)
       {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            XIQuoteReportData ReportData = new XIQuoteReportData();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
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
                sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                iUserID = oParams.Where(m => m.sName == XIConstant.Param_UserID).Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sGUID))
                {
                    oCache.sSessionID = sSessionID;
                    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                    sGUID = ParentGUID;
                }
                var Layoutid = oParams.Where(m => m.sName == "sCallHierarchy").Select(m => m.sValue).FirstOrDefault();
                String LayoutID = "";
                if (Layoutid != null)
                {
                    var ID = Layoutid.Contains("Layout_");
                    int pFrom = Layoutid.IndexOf("Layout_") + "Layout_".Length;
                    int pTo = Layoutid.LastIndexOf(":Layout");

                    LayoutID = Layoutid.Substring(pFrom, pTo - pFrom);
                }
                var visual = oParams.Where(m => m.sName == "Visualisation").Select(m => m.sValue).FirstOrDefault();
                sDisplayMode = oParams.Where(m => m.sName == XIConstant.Param_DisplayMode).Select(m => m.sValue).FirstOrDefault();
                XIVisualisation oXIvisual = new XIVisualisation();
                oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, visual, null);
                var WrapperParms = new List<CNV>();
                var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam".ToLower())).ToList();
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
                    string sOneClickID = WrapperParms.Where(m => m.sName == XIConstant.XIP_1ClickID).Select(m => m.sValue).FirstOrDefault();
                    if (int.TryParse(sOneClickID, out OneClickID))
                    {

                    }
                    else
                    {
                        OneClickID = 0;
                    }
                }
                else if (oParams.Where(m => m.sName == XIConstant.Param_1ClickID).FirstOrDefault() != null)
                {
                    string sOneClickID = oParams.Where(m => m.sName == XIConstant.Param_1ClickID).Select(m => m.sValue).FirstOrDefault();
                    if (int.TryParse(sOneClickID, out OneClickID))
                    {

                    }
                    else
                    {
                        OneClickID = 0;
                    }
                }
                else
                {
                    OneClickID = 0;
                }
                XID1Click o1ClickD = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                 Dictionary<string, string> OneClickRes = new Dictionary<string, string>();
                Dictionary<string, XIIBO> oRes = new Dictionary<string, XIIBO>();
                //oTrace.oParams.Add(new CNV { sName = XIConstant.Param_1ClickID, sValue = OneClickID.ToString() });
                if (OneClickID > 0)
                {
                    //Get 1-Click Defintion             
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID.ToString());
                    o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    // o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                    oRes=o1ClickD.OneClick_Execute();
                    if (oRes.Count == 0){
                      oRes = null;
                    }
                    //oCR  = o1ClickD.OneClick_Executes();
                    //oTrace.oTrace.Add(oCR.oTrace);
                    //if (oCR.bOK && oCR.oResult != null)
                    //{
                        ReportData.ID = OneClickID; ReportData.ReportID = OneClickID;
                        ReportData.QueryName = o1ClickC.Name;
                        ReportData.ShowAs = o1ClickC.Title;
                        ReportData.UserID = Convert.ToInt32(iUserID);
                        ReportData.sGUID = sGUID;
                        ReportData.LayoutID = LayoutID;
                        ReportData.ReportList = oRes;
                        // ReportData.ReportList = oCR.nBOIns;
                        ReportData.IsRowClick = o1ClickC.IsRowClick;
                        ReportData.RowXiLinkID = o1ClickC.RowXiLinkID;
                        ReportData.oXIVisualisations = oXIvisual.XiVisualisationNVs;
                //        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                //        oCResult.oResult = "Success";
                //    }
                //    else
                //    {
                //        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;

                //    }
                //}
                //else
                //{
                //    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                //    oTrace.sMessage = "Mandatory Param:"+ OneClickID + "  is missing";
                }
                oCResult.oResult = ReportData;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }

            //catch (Exception ex)
            //{
            //    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Form Component" });
            //    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            //    oCResult.LogToFile();
            //}
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