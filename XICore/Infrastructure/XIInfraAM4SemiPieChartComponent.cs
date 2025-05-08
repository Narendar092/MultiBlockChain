using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;
using System.Data;

namespace XICore
{
    public class XIInfraAM4SemiPieChartComponent
    {
        public string iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sDisplayMode { get; set; }
        public int OneClickID;
        public Guid OneClickIDXIGUID;

        XIInfraCache oCache = new XIInfraCache();

        public CResult XILoad(List<CNV> oParams)
        {

            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            XIGraphData oXIGD = new XIGraphData();
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
                string RowXilinkID = oParams.Where(m => m.sName == XIConstant.Param_RowXilinkID).Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sGUID))
                {
                    oCache.sSessionID = sSessionID;
                    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                    sGUID = ParentGUID;
                }
                sDisplayMode = oParams.Where(m => m.sName == XIConstant.Param_DisplayMode).Select(m => m.sValue).FirstOrDefault();
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
                    if (Guid.TryParse(sOneClickID, out OneClickIDXIGUID)) { }
                    else if (int.TryParse(sOneClickID, out OneClickID))
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
                    if (Guid.TryParse(sOneClickID, out OneClickIDXIGUID)) { }
                    else if (int.TryParse(sOneClickID, out OneClickID))
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
                if (OneClickID > 0 || (OneClickIDXIGUID != Guid.Empty && OneClickIDXIGUID != null))
                {
                    //Get 1-Click Defintion             
                    if (OneClickIDXIGUID != Guid.Empty && OneClickIDXIGUID != null)
                    {
                        o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickIDXIGUID.ToString());
                    }
                    else
                    {
                        o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID.ToString());
                    }
                    o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    XIDBO oBOD = new XIDBO();
                    if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickD.BOIDXIGUID.ToString());
                    }
                    else if (o1ClickD.BOID > 0)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickD.BOID.ToString());
                    }
                    //XIGraphData oXIGD = new XIGraphData();
                    oXIGD.ID = OneClickID; oXIGD.ReportID = OneClickID;
                    oXIGD.Query = o1ClickC.Query;
                    oXIGD.BOID = o1ClickC.BOID;
                    oXIGD.iDataSourceID = oBOD.iDataSource;
                    oXIGD.DataSourceXIGUID = oBOD.iDataSourceXIGUID;
                    oXIGD.QueryName = o1ClickC.Name;
                    oXIGD.ShowAs = o1ClickC.Title;
                    oXIGD.IsColumnClick = true;
                    oXIGD.Type = "Run";
                    oXIGD.UserID = Convert.ToInt32(iUserID);
                    oXIGD.RowXilinkID = RowXilinkID;
                    oXIGD = oXIGD.Get_PieChartData(oBOD);
                    // oXIGD.CreatedDate = o1ClickC.UpdatedTime;
                    oXIGD.sLastUpdated = o1ClickC.sLastUpdate;
                }
                oCResult.oResult = oXIGD;
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