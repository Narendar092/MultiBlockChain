using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XICore
{
    public class XIInfraAM4MultiBarwithLineChartComponent
    {

        public string iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sDisplayMode { get; set; }
        public int OneClickID;
        public string sOneClickID;
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
                var Colour = oParams.Where(m => m.sName.ToLower() == "sColour".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var ToolTips = Convert.ToBoolean(oParams.Where(m => m.sName.ToLower() == "bToolTip".ToLower()).Select(m => m.sValue).FirstOrDefault());
                var GridLines = Convert.ToBoolean(oParams.Where(m => m.sName.ToLower() == "bGridLines".ToLower()).Select(m => m.sValue).FirstOrDefault());
                var CursorLines = Convert.ToBoolean(oParams.Where(m => m.sName.ToLower() == "bIsCursor".ToLower()).Select(m => m.sValue).FirstOrDefault());
                var Legends = Convert.ToBoolean(oParams.Where(m => m.sName.ToLower() == "bIsLegends".ToLower()).Select(m => m.sValue).FirstOrDefault());
                var LegendPosition = oParams.Where(m => m.sName.ToLower() == "sLegendPosition".ToLower()).Select(m => m.sValue).FirstOrDefault();
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
                else if (oParams.Where(m => m.sName == XIConstant.Param_1ClickID).FirstOrDefault() != null)
                {
                    sOneClickID = oParams.Where(m => m.sName == XIConstant.Param_1ClickID).Select(m => m.sValue).FirstOrDefault();
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
                List<XIIBO> oBoList = new List<XIIBO>();
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

                    XIDStructure oXIDStructure = new XIDStructure();
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
                    var sTableResult = oBOInsdt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                    Dictionary<string, XIIValue> XIValuedictionary = new Dictionary<string, XIIValue>();
                    Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
                    List<DataRow> Rows = new List<DataRow>();
                    Rows = oBOInsdt.AsEnumerable().ToList();
                    var AllCols = oBOInsdt.Columns.Cast<DataColumn>()
                                                          .Select(x => x.ColumnName)
                                                          .ToList();
                    if (sTableResult != null)
                    {
                        foreach (DataRow row in oBOInsdt.Rows)
                        {
                            XIIBO oBOII = new XIIBO();
                            dictionary = Enumerable.Range(0, oBOInsdt.Columns.Count)
                                .ToDictionary(i => i.ToString(), i => new XIIAttribute
                                {
                                    sName = oBOInsdt.Columns[i].ColumnName,
                                    //sValue = OptionListCols.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? CheckOptionList(oBOIns.Columns[i].ColumnName, row.ItemArray[i].ToString(), oBOD) : row.ItemArray[i].ToString(),
                                    // sValue = OptionListCols.ContainsKey(oBOInsdt.Columns[i].ColumnName.ToLower()) ? o1ClickD1.CheckOptionList(oBOInsdt.Columns[i].ColumnName, row.ItemArray[i].ToString(), OptionListCols[oBOInsdt.Columns[i].ColumnName.ToLower()]) : row.ItemArray[i].ToString(),
                                    sValue = row.ItemArray[i].ToString(),
                                    //sPreviousValue = row.ItemArray[i].ToString(),
                                    // sDisplayName = oBOD.Attributes.ContainsKey(oBOInsdt.Columns[i].ColumnName) ? oBOD.AttributeD(oBOInsdt.Columns[i].ColumnName).LabelName : "",
                                    //iValue = TotalColumns.Contains(oBOIns.Columns[i].ColumnName.ToLower()) ? (!string.IsNullOrEmpty(row.ItemArray[i].ToString()) ? (Convert.ToInt32(row.ItemArray[i].ToString())) : 0) : 0,
                                }, StringComparer.CurrentCultureIgnoreCase);
                            oBOII.Attributes = dictionary;
                            XIValuedictionary = Enumerable.Range(0, oBOInsdt.Columns.Count)
                                 .ToDictionary(i => i.ToString(), i => new XIIValue
                                 {
                                     sValue = row.ItemArray[i].ToString(),
                                 }, StringComparer.CurrentCultureIgnoreCase);
                            oBOII.XIIValues = XIValuedictionary;
                            //oBOII.iBODID = oBOD.BOID;
                            //oBOII.sBOName = oBOD.TableName;
                            oBoList.Add(oBOII);
                            //sBo = oBOD.Name;

                        }
                        //nBOIns[sBo] = oBoList;
                    }
                    ComOneClick.Add(sTableResult, oBoList);
                    List<string> Values = new List<string>();
                    List<Dictionary<string, string>> Keys = new List<Dictionary<string, string>>();

                    foreach (var item in oBoList)
                    {
                        //List<int> values = new List<int>();
                        Dictionary<string, string> Keys1 = new Dictionary<string, string>();
                        foreach (var item1 in item.Attributes)
                        {
                            var Key = item1.Value.sName;
                            var val = item1.Value.sValue;
                            Values.Add(val);
                            Keys1.Add(Key, val);
                        }
                        Keys.Add(Keys1);
                    }
                    oXIGD.KeyPairs = Keys;
                    oXIGD.Count = oBoList[0].Attributes.Count;
                    oXIGD.ComOneClick = ComOneClick;
                    oXIGD.sOneClickID = sOneClickID;
                    oXIGD.RowXilinkID = RowXilinkID;
                    oXIGD.QueryName = o1ClickC.Title;
                    oXIGD.sLastUpdated = o1ClickC.sLastUpdate;
                    oXIGD.bToolTip = ToolTips;
                    oXIGD.bGridLines = GridLines;
                    oXIGD.bIsCursor = CursorLines;
                    oXIGD.bIsLegends = Legends;
                    oXIGD.sLegendPosition = LegendPosition;
                    if (!string.IsNullOrEmpty(Colour))
                    {
                        oXIGD.sColours = Colour.Split(',').ToList();
                    }
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
