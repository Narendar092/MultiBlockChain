using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using XISystem;

namespace XICore
{
    public class XIInfraReportComponent
    {
        public int iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sDisplayMode { get; set; }
        public int iApplicationID { get; set; }

        XIInfraCache oCache = new XIInfraCache();
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
                long ReportID = 0;
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
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
                    var iReportID = WrapperParms.Where(m => m.sName == "{-iInstanceID}").Select(m => m.sValue).FirstOrDefault(); // oXIAPI.Get_ParamVal(sSessionID, sGUID, null, Prm); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (iReportID != null)
                    {
                        ReportID = Convert.ToInt32(iReportID);
                    }
                    else
                    {
                        ReportID = 0;
                    }
                }
                else
                {
                    string sInstanceID = oParams.Where(m => m.sName == "ReportID").Select(m => m.sValue).FirstOrDefault();
                    if (sInstanceID != null && (sInstanceID.StartsWith("{XIP|") || sInstanceID.StartsWith("-") || sInstanceID.StartsWith("{-")))
                    {
                        sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, sInstanceID);
                    }
                    if (!string.IsNullOrEmpty(sInstanceID))
                    {
                        if (long.TryParse(sInstanceID, out ReportID))
                        {

                        }
                    }
                }
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                XIReports oXIReports = new XIReports();
                if (ReportID > 0)
                {
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                    List<CNV> nParms = new List<CNV>();
                    nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    //o1ClickC.ReplaceFKExpressions(nParms);
                    //o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Params loaded successfully for Group Component" });
                    oXIReports.ReportID = Convert.ToInt32(ReportID);
                    oXIReports.nParms = nParms;
                    oXIReports.SGUID = sGUID;
                    oXIReports.SessionID = sSessionID;
                    var oXIDRepo = oXIReports.GenerateReport(oParams);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    List<Dictionary<string, XIChainReports>> Scripts = new List<Dictionary<string, XIChainReports>>();
                    List<Dictionary<string, object>> BarCColour = new List<Dictionary<string, object>>();
                    Dictionary<string, object> Result = new Dictionary<string, object>();
                    Dictionary<string, object> row;
                    Dictionary<string, XIChainReports> Script;
                    List<Dictionary<string, object>> Cell = new List<Dictionary<string, object>>();
                    List<DataTable> dt = ((XIReports)oXIDRepo.oResult).FinalResult;
                    Dictionary<string, object> columnNames = dt[0].Columns.Cast<DataColumn>().ToDictionary(x => x.ColumnName, x => x.DefaultValue);
                    rows.Add(columnNames);
                    var HeaderKeys = columnNames.Keys.ToList();
                    XIChainReports obj = new XIChainReports();
                    foreach (var i in ((XIReports)oXIDRepo.oResult).oChainReport.oChildItems)
                    {
                        Script = new Dictionary<string, XIChainReports>();
                        if (i.Scripts != "")
                        {
                            obj = new XIChainReports();
                            obj.Query = i.Scripts;
                            Script.Add(i.Name, obj);
                            Scripts.Add(Script);
                        }

                    }
                    List<object> Colours = new List<object>();
                    string Colcolour = "";
                    string Rowcolour = "";
                    string Collapse = "";
                    Dictionary<string, object> Cellvalue = new Dictionary<string, object>();
                    XIIXI oXII = new XIIXI();
                    XIIBO oBOIReport = oXII.BOI("Reports_t", ReportID.ToString());
                    int SplitRows = Convert.ToInt32(oBOIReport.AttributeI("iSplitFields").sValue);
                    string SubTotal = oBOIReport.AttributeI("bIsSubTotal").sValue;
                    string SubHeader = oBOIReport.AttributeI("sSubTotalHeaders").sValue;
                    //string RowClick = oBOIReport.AttributeI("bIsRowClick").sValue;
                    string CellTypes = oBOIReport.AttributeI("sCellClickList").sValue;
                    string CellXilink = oBOIReport.AttributeI("CellXilinkIDXIGUID").sValue;
                    string RowType = oBOIReport.AttributeI("sRowClickList").sValue;
                    string RowXilink = oBOIReport.AttributeI("RowXilinkIDXIGUID").sValue;
                    string TotalExhaust = oBOIReport.AttributeI("bIsExhaustTotal").sValue;
                    List<DataTable> RowResult = new List<DataTable>();
                    List<string> HeadersData = new List<string>();
                    List<string> ColHeadersData = new List<string>();
                    String PreviousRowValue = "";
                    DataTable ResultSet = dt[0];
                    List<DataTable> datatables = new List<DataTable>();
                    if (SplitRows != 0)
                     {
                        DataTable currentTable=null;
                        foreach (DataRow Dr in ResultSet.Rows)
                        {
                            String CurrentRowValue = "";
                            for (int c = 0; c < SplitRows; c++)
                            {
                                CurrentRowValue += Dr[c].ToString()+"|";//ResultSet.Rows[Datarow][c].ToString() + "|";
                            }
                            CurrentRowValue = CurrentRowValue.TrimEnd('|');
                            if (PreviousRowValue != CurrentRowValue)
                            {
                                HeadersData.Add(CurrentRowValue);
                                PreviousRowValue = CurrentRowValue;
                                //create new table and assign to currentTable
                                // generate the data you want to insert
                                 currentTable = new DataTable();
                                //currentTable = new DataTable();
                                foreach (var item in HeaderKeys)
                                {
                                    currentTable.Columns.Add(item, typeof(string));
                                }
                                datatables.Add(currentTable);
                            }
                            currentTable.Rows.Add(Dr.ItemArray);
                           
                        }
                    }

                    else
                    {
                        if (dt[1].Rows.Count != 0)
                        {
                            foreach (DataRow row1 in dt[1].Rows)
                            {
                                Colours = row1.ItemArray.ToList();
                            }
                        }
                        if (dt[2].Rows.Count != 0)
                        {
                            foreach (DataRow row1 in dt[2].Rows)
                            {

                                Cellvalue.Add(row1.ItemArray[0].ToString(), row1.ItemArray[1]);
                            }
                            Cell.Add(Cellvalue);

                        }
                        //if (dt[2].Rows.Count != 0)
                        //{
                        //    foreach (DataRow row1 in dt[2].Rows)
                        //    {
                        //        Rows = row1.ItemArray.ToList();
                        //    }
                        //}
                        if (SubTotal == "True" && !string.IsNullOrEmpty(SubHeader))
                        {
                            dt[0] = DatatableSubTotal(dt[0], SubHeader);
                        }
                        foreach (DataRow dr in dt[0].Rows)
                        {
                            row = new Dictionary<string, object>();
                            foreach (DataColumn col in dt[0].Columns)
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }
                            rows.Add(row);
                        }
                        foreach (DataRow dr in dt[3].Rows)
                        {
                            row = new Dictionary<string, object>();
                            foreach (DataColumn col in dt[3].Columns)
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }
                            BarCColour.Add(row);
                        }
                    }
                    if (datatables.Count > 0) {
                        foreach (DataTable dttable in datatables)
                        {
                            if (SubTotal == "True" && !string.IsNullOrEmpty(SubHeader))
                            {
                                DataTable DResult = new DataTable();
                                DResult = DatatableSubTotal(dttable, SubHeader,SplitRows);
                                RowResult.Add(DResult);
                            }
                            else
                            {
                                RowResult = datatables;
                            }
                        }
                    }
                    Colcolour = ((XIReports)oXIDRepo.oResult).sColour;
                    Collapse = ((XIReports)oXIDRepo.oResult).bCollapse;
                    Rowcolour = ((XIReports)oXIDRepo.oResult).rColour;
                    var ReportName = ((XIReports)oXIDRepo.oResult).Name;

                    //Scripts= ((XIReports)oXIDRepo.oResult).oChainReport.oChildItems.;
                    Result.Add("Data", rows);
                    Result.Add("Header", HeadersData);
                    Result.Add("ColHeader", ColHeadersData);
                    Result.Add("ColumnCount", SplitRows);
                    Result.Add("Data1", RowResult);
                    Result.Add("Colours", Colours);
                    Result.Add("CellValue", Cellvalue);
                    Result.Add("Colcolour", Colcolour);
                    Result.Add("Rowcolour", Rowcolour);
                    Result.Add("Collapse", Collapse);
                    Result.Add("Scripts", Scripts);
                    Result.Add("Barcolours", BarCColour);
                    Result.Add("ReportName", ReportName);
                    //Result.Add("bIsRowClick", RowClick);
                    //Result.Add("RowClickID", RowXilink);
                    Result.Add("CellCount", CellTypes);
                    Result.Add("CellXilink", CellXilink);
                    Result.Add("RowCount", RowType);
                    Result.Add("RowXilink", RowXilink);
                    Result.Add("TotalExhaust", TotalExhaust);
                    oXIReports.Result = Result;
                    oCResult.oResult = oXIReports;
                    ((XIReports)oCResult.oResult).XiLinkID = ((XIReports)oXIDRepo.oResult).XiLinkID;
                }

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
        public DataTable DatatableSubTotal(DataTable dtActiveResult, string sSubTotalHeaders, int SplitCount = 0)
        {
            int TotalColcount = dtActiveResult.Columns.Count;
            var columnName = dtActiveResult.Columns[0].ColumnName;
            var TotalcolumnName = dtActiveResult.Columns[2].ColumnName;
            string SubTotalColName = string.Empty;
            if (!string.IsNullOrEmpty(sSubTotalHeaders))
            {
                var SubTotal = sSubTotalHeaders.Split('|').ToList();
                if (SubTotal.Count > 1)
                {
                    columnName = sSubTotalHeaders.Split('|')[0];
                    TotalcolumnName = sSubTotalHeaders.Split('|')[1];
                }
                else
                {
                    TotalcolumnName = sSubTotalHeaders;
                }
            }
            if (SplitCount > 0)
            {
                var columnNameIndex = dtActiveResult.Columns[TotalcolumnName].Ordinal - 1;
                SubTotalColName = dtActiveResult.Columns[(int)columnNameIndex].ColumnName;
            }
            else
            {
                SubTotalColName = columnName;
            }
            if (dtActiveResult.Rows.Count > 0)
            {
                var DataClone = dtActiveResult.AsEnumerable().CopyToDataTable();
                var allEntries = from r in DataClone.AsEnumerable()
                                 select r[TotalcolumnName];
                int SubTotal = 0;
                if (TotalColcount - SplitCount > 1) {
                for (int i = 0; i <= dtActiveResult.Rows.Count - 1; i++)
                {
                    if (i > 0)
                    {
                        if (dtActiveResult.Rows[i][columnName].ToString().ToLower() != dtActiveResult.Rows[i - 1][columnName].ToString().ToLower())
                        {
                            dtActiveResult.Rows.InsertAt(dtActiveResult.NewRow(), i);
                            dtActiveResult.Rows[i][SubTotalColName] = "SubTotal:";
                            dtActiveResult.Rows[i][TotalcolumnName] = SubTotal;
                            SubTotal = 0;
                            i++;
                        }
                    }
                    SubTotal += Convert.ToInt16(dtActiveResult.Rows[i][TotalcolumnName]);

                }
                dtActiveResult.Rows.Add(dtActiveResult.NewRow());
                dtActiveResult.Rows[dtActiveResult.Rows.Count - 1][SubTotalColName] = "SubTotal:";
                dtActiveResult.Rows[dtActiveResult.Rows.Count - 1][TotalcolumnName] = SubTotal;
                    if (SplitCount == 0)
                    {
                        // Create and add the final total row
                        DataRow finalTotal = dtActiveResult.NewRow();
                        finalTotal[SubTotalColName] = "Total:";
                        finalTotal[TotalcolumnName] = "     " + allEntries.Sum(x => Convert.ToDecimal(x));
                        dtActiveResult.Rows.Add(finalTotal);
                    }
                }
            }

            return dtActiveResult;
        }

    }
}