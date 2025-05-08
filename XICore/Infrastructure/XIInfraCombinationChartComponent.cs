using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;
using System.Data;

namespace XICore
{
    public class XIInfraCombinationChartComponent
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
                string sVisualisation = oParams.Where(m => m.sName.ToLower() == "Visualisation".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sVisualisation) && sVisualisation != "0")
                {
                    int sVisualisationID = 0;
                    if (int.TryParse(sVisualisation, out sVisualisationID))
                    {
                        if (sVisualisationID != 0)
                        {
                            sVisualisation = "";
                        }
                    }
                    var oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, sVisualisation, sVisualisationID.ToString());
                    oXIGD.oXIVisualisations = oXIvisual.XiVisualisationNVs;
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
                List<XIIBO> oBoList = new List<XIIBO>();
                if (OneClickID > 0)
                {

                    XIDStructure oXIDStructure = new XIDStructure();
                    //Get 1-Click Defintion             
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID.ToString());
                    o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    XIDBO oBOD = new XIDBO();
                    if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString());
                    }
                    else if (o1ClickD.BOID > 0)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                    }
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                    List<CNV> nParms = new List<CNV>();
                    nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    o1ClickC.ReplaceFKExpressions(nParms);
                    o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);

                    DataTable oBOInsdt = o1ClickC.Execute_Query();
                    Dictionary<List<string>, List<XIIBO>> ComOneClick = new Dictionary<List<string>, List<XIIBO>>();
                    //Dictionary<string, List<XIIBO>> nBOIns = new Dictionary<string, List<XIIBO>>();
                    var sTableResult = oBOInsdt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                    Dictionary<string, XIIValue> XIValuedictionary = new Dictionary<string, XIIValue>();
                    //Dictionary<string, List<string>> OptionListCols = new Dictionary<string, List<string>>();
                    Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
                    
                    List<DataRow> Rows = new List<DataRow>();
                    Rows = oBOInsdt.AsEnumerable().ToList();
                    var AllCols = oBOInsdt.Columns.Cast<DataColumn>()
                                                          .Select(x => x.ColumnName)
                                                          .ToList();
                    //OptionListCols = OptionListCols ?? new Dictionary<string, List<string>>();
                    if (sTableResult != null)
                    {
                        foreach (DataRow row in oBOInsdt.Rows)
                        {
                            XIIBO oBOII = new XIIBO();
                            dictionary = Enumerable.Range(0, oBOInsdt.Columns.Count)
                                .ToDictionary(i => oBOInsdt.Columns[i].ColumnName, i => new XIIAttribute
                                {
                                    sName = oBOInsdt.Columns[i].ColumnName,
                                    //sValue = OptionListCols.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), oBOD) : row.ItemArray[i].ToString(),
                                    // sValue = OptionListCols.ContainsKey(oBOInsdt.Columns[i].ColumnName.ToLower()) ? o1ClickD1.CheckOptionList(oBOInsdt.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOInsdt.Columns[i].ColumnName.ToLower()]) : row.ItemArray[i].ToString(),
                                    sValue = row.ItemArray[i].ToString(),
                                    sPreviousValue = row.ItemArray[i].ToString(),
                                    sDisplayName = oBOD.Attributes.ContainsKey(oBOInsdt.Columns[i].ColumnName) ? oBOD.AttributeD(oBOInsdt.Columns[i].ColumnName).LabelName : "",
                                    //iValue = TotalColumns.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? (!string.IsNullOrEmpty(row.ItemArray[i].ToString()) ? (Convert.ToInt32(row.ItemArray[i].ToString())) : 0) : 0,
                                }, StringComparer.CurrentCultureIgnoreCase);
                            oBOII.Attributes = dictionary;
                            XIValuedictionary = Enumerable.Range(0, oBOInsdt.Columns.Count)
                                 .ToDictionary(i => oBOInsdt.Columns[i].ColumnName, i => new XIIValue
                                 {
                                     sValue = row.ItemArray[i].ToString(),
                                     //sDisplayName = oBOD.Attributes.ContainsKey(oBOInsdt.Columns[i].ColumnName) ? oBOD.AttributeD(oBOInsdt.Columns[i].ColumnName).LabelName : "",
                                 }, StringComparer.CurrentCultureIgnoreCase);
                            oBOII.XIIValues = XIValuedictionary;
                           // oBOII.iBODID = oBOD.BOID;
                            //oBOII.sBOName = oBOD.TableName;
                            oBoList.Add(oBOII);
                           // sBo = oBOD.Name;

                        }
                       // nBOIns[sBo] = oBoList;
                    }
                    ComOneClick.Add(sTableResult, oBoList);
                    oXIGD.ComOneClick = ComOneClick;
                    oXIGD.iOneClickID = OneClickID;
                    oXIGD.RowXilinkID = RowXilinkID;
                    //oXIGD.CreatedDate = o1ClickC.UpdatedTime;
                    oXIGD.QueryName = o1ClickC.Name;
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