using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XISystem;

namespace XICore
{
    public class XIInfraAM4FunnelComponent
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
        public string sOneClickID;

        public List<XID1ClickParameter> xID1ClickParameters { get; set; }
        public List<SqlParameter> ListSqlParams { get; set; }
        XIDStructure oXIDStructure = new XIDStructure();

        public List<CNV> NVs { get; set; }

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
                var Colour = oParams.Where(m => m.sName.ToLower() == "sColour".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var ToolTips = Convert.ToBoolean(oParams.Where(m => m.sName.ToLower() == "bToolTip".ToLower()).Select(m => m.sValue).FirstOrDefault());
                var GridLines = Convert.ToBoolean(oParams.Where(m => m.sName.ToLower() == "bGridLines".ToLower()).Select(m => m.sValue).FirstOrDefault());
                string RowXilinkID = oParams.Where(m => m.sName == XIConstant.Param_RowXilinkID).Select(m => m.sValue).FirstOrDefault();
                var Legends = Convert.ToBoolean(oParams.Where(m => m.sName.ToLower() == "bIsLegends".ToLower()).Select(m => m.sValue).FirstOrDefault());
                var LegendPosition = oParams.Where(m => m.sName.ToLower() == "sLegendPosition".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var Donutchart = Convert.ToBoolean(oParams.Where(m => m.sName.ToLower() == "bDonutchart".ToLower()).Select(m => m.sValue).FirstOrDefault());
                //  string RowXiLinkIDXIGUID = oParams.Where(m => m.sName == XIConstant.Param_RowXiLinkIDXIGUID).Select(m => m.sValue).FirstOrDefault();

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
                    sOneClickID = WrapperParms.Where(m => m.sName == XIConstant.XIP_1ClickID).Select(m => m.sValue).FirstOrDefault();
                    if (Guid.TryParse(sOneClickID, out OneClickIDXIGUID)) { }
                    else if (int.TryParse(sOneClickID, out OneClickID))
                    {

                    }
                    else
                    {
                        OneClickID = 0;
                    }
                }
                else if (oParams.Where(m => m.sName == "1Click").FirstOrDefault() != null)
                {
                    sOneClickID = oParams.Where(m => m.sName == "1Click").Select(m => m.sValue).FirstOrDefault();
                    if (Guid.TryParse(sOneClickID, out OneClickIDXIGUID)) { }
                    else if (int.TryParse(sOneClickID, out OneClickID))
                    {

                    }
                    else
                    {
                        OneClickID = 0;
                    }
                }
                if (string.IsNullOrEmpty(sOneClickID))
                {
                    if (oParams.Where(m => m.sName == "1ClickID").FirstOrDefault() != null)
                    {
                        sOneClickID = oParams.Where(m => m.sName == "1ClickID").Select(m => m.sValue).FirstOrDefault();
                        if (Guid.TryParse(sOneClickID, out OneClickIDXIGUID)) { }
                        else if (int.TryParse(sOneClickID, out OneClickID))
                        {

                        }
                        else
                        {
                            OneClickID = 0;
                        }
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
                    oXIGD.ID = o1ClickD.ID; oXIGD.ReportID = o1ClickD.ID;
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                    List<CNV> nParms = new List<CNV>();
                    nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    o1ClickC.ReplaceFKExpressions(nParms);
                    o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);
                    oXIGD.Query = o1ClickC.Query;
                    oXIGD.IsStoredProcedure = o1ClickC.IsStoredProcedure;
                    oXIGD.BOID = o1ClickC.BOID;
                    oXIGD.iDataSourceID = oBOD.iDataSource;
                    oXIGD.DataSourceXIGUID = oBOD.iDataSourceXIGUID;
                    oXIGD.QueryName = o1ClickC.Name;
                    oXIGD.ShowAs = o1ClickC.Title;
                    oXIGD.IsColumnClick = true;
                    oXIGD.Type = "Run";
                    oXIGD.RowXiLinkIDXIGUID = o1ClickC.RowXiLinkIDXIGUID;
                    oXIGD.UserID = Convert.ToInt32(iUserID);
                    oXIGD.RowXilinkID = RowXilinkID;
                    oXIGD.CreatedDate = o1ClickC.UpdatedTime;
                    oXIGD.sLastUpdated = o1ClickC.sLastUpdate;
                    oXIGD.sOneClickID = sOneClickID;
                    oXIGD.bToolTip = ToolTips;
                    oXIGD.bGridLines = GridLines;
                    oXIGD.bIsLegends = Legends;
                    oXIGD.bDonutchart = Donutchart;
                    oXIGD.sLegendPosition = LegendPosition;
                    if (!string.IsNullOrEmpty(Colour))
                    {
                        oXIGD.sColours = Colour.Split(',').ToList();
                    }

                    Dictionary<string, string> OneClickRes = new Dictionary<string, string>();
                    List<XIDashBoardGraphs> items = new List<XIDashBoardGraphs>();
                    if (o1ClickC.IsStoredProcedure)
                    {
                        var SPParms = o1ClickC.oOneClickParameters.Where(x => x.iType == 30).ToList();
                        //XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                        //List<CNV> nParms = new List<CNV>();
                        nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                        if (SPParms != null && SPParms.Count() > 0)
                        {
                            o1ClickC.NVs = o1ClickC.NVs ?? new List<CNV>();
                            foreach (var items1 in SPParms)
                            {
                                var sResValue = nParms.Where(m => m.sName == items1.sName).FirstOrDefault();
                                if (sResValue != null)
                                {
                                    o1ClickC.NVs.Add(new CNV { sName = items1.sName, sValue = sResValue.sValue });
                                }
                            }
                        }
                        o1ClickC.Query = o1ClickC.Query;
                        Dictionary<string, XIIBO> oRes = new Dictionary<string, XIIBO>();
                        oRes = o1ClickC.OneClick_Run();


                        var obj2 = oRes.Values.ToList();
                        foreach (var item in obj2.ToList())
                        {
                            XIDashBoardGraphs obj = new XIDashBoardGraphs();
                            obj.label = item.XIIValues["Name"].sValue;
                            obj.value = Convert.ToInt16(item.XIIValues["Value"].sValue);
                            //obj.Hours = item.XIIValues["TotalActualHours"].sValue;
                            items.Add(obj);
                        }
                        oXIGD.FunnelData = items;
                    }
                    else
                    {
                        //o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID.ToString());
                        //o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);

                        DataTable oDataTable = o1ClickC.Execute_Query();
                        var Percentagevalue = "";
                        foreach (DataRow row in oDataTable.Rows)
                        {
                            foreach (DataColumn col in oDataTable.Columns)
                            {
                                //XIDashBoardGraphs obj = new XIDashBoardGraphs();
                                //obj.label = col.XIIValues["Name"].sValue;
                                //obj.value = Convert.ToInt16(col.XIIValues["Value"].sValue);
                                ////obj.Hours = item.XIIValues["TotalActualHours"].sValue;
                                //items.Add(obj);

                                //OneClickRes.Add(row.ItemArray[0].ToString(), col.ColumnName);
                                //Percentagevalue = row.ItemArray[0].ToString();
                            }
                        }

                        oXIGD.FunnelData = items;
                    }
                }
                ;
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
