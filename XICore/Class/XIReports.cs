using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web;
using XIDatabase;
using xiEnumSystem;
using XISystem;
namespace XICore
{
    public class XIReports
    {
        public int ReportID { get; set; }
        public int FKiReportConfigID { get; set; }
        public string Name { get; set; }
        public string Header { get; set; }
        public string sColour { get; set; }
        public string bCollapse { get; set; }
        public string rColour { get; set; }
        public int XiLinkID { get; set; }
        public string bIsSubTotal { get; set; }
        public string sSubTotalHeaders { get; set; }
        public List<DataTable> FinalResult { get; set; }
        public XIChainReports oChainReport { get; set; }
        public List<CNV> nParms { get; set; }
        public XIChainReports Script { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public List<DataTable> SplitData { get; set; }
        public List<Dictionary<string, object>> Barcolours { get; set; }
        public Dictionary<string, object> Cell { get; set; }
        public List<Dictionary<string, XIChainReports>> scri { get; set; }
        public Dictionary<string, object> Result { get; set; }
        public string SGUID { get; set; }
        public string SessionID { get; set; }


        private List<DataTable> BuildConsolidatedReport(List<XIChainReports> oChainReport, bool IsParent, XIChainReports Oparent = null, string bIsSubTotal = null, string sSubTotalHeaders=null)
        {
            DataTable dtfinal = new DataTable();
            DataTable dtActiveResult = new DataTable();
            DataTable dtActiveResult2 = new DataTable();
            DataTable dtActiveResult3 = new DataTable();
            DataTable dtActiveResult4 = new DataTable();

            List<DataTable> dtActiveResult1 = new List<DataTable>();
            int j = 0;
            try
            {
                if (oChainReport != null)
                {
                    List<XIChainReports> oChildrens = new List<XIChainReports>();
                    // Aggrgate all the Query result into one DataTable Object here
                    if (IsParent)
                    {
                        oChildrens = Oparent.oChildItems.OrderBy(d => d.Priority).ToList();
                    }
                    else
                    {
                        oChildrens = oChainReport.OrderBy(d => d.Priority).ToList();
                    }
                    foreach (var item in oChildrens)
                    {
                        if (item.NodeType == EnumReportNodeTypes.oneClick || item.NodeType == EnumReportNodeTypes.Resolved)
                        {

                            if (item.NodeType == EnumReportNodeTypes.Resolved && item.sData.Contains("{{"))
                            {
                                // CHECK WHETHER STRING CONTAINS EXPRESSIONS THAT NEED TO BE RESOLVED
                                item.QueryResult = item.sData.BuildDataTableWithString(item.DataformatType, item.IsHeader, dtActiveResult, item.IsRowTotal, item.IsCalculate, item.IsTotal, item.IsColour, item.ColumnValue);
                            }
                            if (item.IsHeader == true && item.NodeType == EnumReportNodeTypes.Resolved)
                            {
                                dtActiveResult = item.QueryResult;

                            }
                            if (item.IsHeader == true && item.NodeType == EnumReportNodeTypes.oneClick)
                            {
                                //var row22 = item.QueryResult.Rows;
                                if (string.IsNullOrEmpty(item.sData))
                                {
                                    DataRow firstRow = item.QueryResult.NewRow();
                                    List<DataColumn> Columns = new List<DataColumn>();
                                    foreach (var value in item.QueryResult.Rows[0].ItemArray)
                                    {
                                        DataColumn Column = new DataColumn();
                                        Column.AllowDBNull = true;
                                        Column.ColumnName = value.ToString();
                                        dtActiveResult.Columns.Add(Column);
                                    }
                                    item.QueryResult = dtActiveResult;
                                }
                                else
                                {
                                    dtActiveResult.Columns.Add(item.sData.ToString());
                                    foreach (DataRow row in item.QueryResult.Rows)
                                    {
                                        dtActiveResult.Columns.Add(row[0].ToString(), typeof(string));
                                    }
                                }
                            }
                            if (item.IsColour == true)
                            {
                                dtActiveResult2 = item.QueryResult;
                            }
                            if (item.bCell == true)
                            {
                                dtActiveResult3 = item.QueryResult;
                            }
                            if (dtActiveResult.Columns.Count <= 0)
                                dtActiveResult = item.QueryResult;
                            if (!string.IsNullOrEmpty(item.sDefault) && item.QueryResult.Rows.Count == 0)
                            {
                                DataColumn Column = new DataColumn();
                                Column.AllowDBNull = true;
                                Column.ColumnName = item.QueryResult.Columns[0].ToString();
                                item.QueryResult.Rows.Add(Column);
                            }
                            if (item.Pivot == EnumPivot.Yes)
                            {
                                List<XIDropDown> data = item.QueryResult.AsEnumerable().Select(row => new XIDropDown
                                {
                                    Type = row.Field<string>(0),
                                    text = String.IsNullOrEmpty(row.Field<string>(1)) ? "null" : row.Field<string>(1),
                                    Value = row.Field<int?>(2).GetValueOrDefault(),
                                }).ToList();
                                //if (item.sDefault == "week")
                                //{
                                //    data = GetWeekDays(data);
                                //}

                                item.QueryResult = data.ToPivotTable(
                                    t => t.text,
                                    t => t.Type,
                                    t => t.Any() ? t.Sum(x => x.Value) : 0);
                                //if (item.QueryResult.Columns.Contains("null"))
                                //{
                                //    foreach (DataColumn column in item.QueryResult.Columns)
                                //    {
                                //        if (column.ToString() == "null")
                                //        {
                                //            item.QueryResult.Columns.Remove(column);
                                //        }
                                //    }
                                //}
                            }

                            if (item.Appendtype == EnumReportAppendTypes.Bottom)
                                dtActiveResult = dtActiveResult.BottomMerging(item.QueryResult, item.Pivot);
                            else if (item.Appendtype == EnumReportAppendTypes.Right)
                                dtActiveResult = dtActiveResult.LeftRightMerging(item.QueryResult);
                            else if (item.Appendtype == EnumReportAppendTypes.Left)
                                dtActiveResult = item.QueryResult.LeftRightMerging(dtActiveResult);
                        }
                        if (item.TargetValue != 0)
                        {
                            DataColumn Column = new DataColumn();
                            Column.AllowDBNull = true;
                            Column.ColumnName = dtActiveResult.Columns[dtActiveResult.Columns.Count - 1].ToString();
                            dtActiveResult4.Columns.Add(Column.ColumnName);
                            //dtActiveResult4.Columns.Add("Colour" + j);
                            List<int> maxValue = dtActiveResult.AsEnumerable().Select(row => Convert.ToInt32(Math.Round(Convert.ToDecimal(row[Column.ColumnName])))).ToList();
                            for (int i = 0; i < maxValue.Count; i++)
                            {
                                if (maxValue[i] != 0)
                                {
                                    string value = Convert.ToString(Math.Round((Convert.ToDecimal(maxValue[i]) / Convert.ToDecimal(item.TargetValue)) * 100, 2)) + "%";
                                    if (j == 0)
                                    {
                                        dtActiveResult4.Rows.Add();
                                    }
                                    dtActiveResult4.Rows[i][Column.ColumnName] = value + ',' + item.coColour;
                                    // dtActiveResult4.Rows[i]["Colour" + j] = item.coColour;
                                }
                            }
                            j++;

                        }
                        if (item.oChildItems.Count() > 0)
                        {
                            // For Recursive Execution Here
                            if (item.NodeType == EnumReportNodeTypes.Aggregator)
                            {
                                var dtresult = BuildConsolidatedReport(item.oChildItems, false, null);
                                if (item.Appendtype == EnumReportAppendTypes.Bottom)
                                    dtActiveResult = dtActiveResult.BottomMerging(dtresult.FirstOrDefault(), item.Pivot);
                                else if (item.Appendtype == EnumReportAppendTypes.Right)
                                    dtActiveResult = dtActiveResult.LeftRightMerging(dtresult.FirstOrDefault());
                                else if (item.Appendtype == EnumReportAppendTypes.Left)
                                    dtActiveResult = item.QueryResult.LeftRightMerging(dtActiveResult);
                                dtActiveResult2 = dtresult[1];
                                dtActiveResult3 = dtresult[2];
                                dtActiveResult4 = dtresult[3];
                            }
                            //item.AggregatorResult = BuildConsolidatedReport(item.oChildItems, false, null);
                        }
                    }
                }
                //if (bIsSubTotal == "True")
                //{
                //    XIInfraReportComponent rc = new XIInfraReportComponent();
                //    dtActiveResult=rc.DatatableSubTotal(dtActiveResult, sSubTotalHeaders);
                //}
                    //    var columnName = dtActiveResult.Columns[0].ColumnName;
                    //    var TotalcolumnName = dtActiveResult.Columns[2].ColumnName;
                    //    if (!string.IsNullOrEmpty(sSubTotalHeaders))
                    //    {
                    //        columnName = sSubTotalHeaders.Split('|')[0];
                    //        TotalcolumnName = sSubTotalHeaders.Split('|')[1];
                    //    }
                    //    if (dtActiveResult.Rows.Count > 0)
                    //    {
                    //        var DataClone = dtActiveResult.AsEnumerable().CopyToDataTable();
                    //        var allEntries = from r in DataClone.AsEnumerable()
                    //                         select r[TotalcolumnName];
                    //        int SubTotal = 0;
                    //        for (int i = 0; i <= dtActiveResult.Rows.Count - 1; i++)
                    //        {
                    //            if (i > 0)
                    //            {
                    //                if (dtActiveResult.Rows[i][columnName].ToString().ToLower() != dtActiveResult.Rows[i - 1][columnName].ToString().ToLower())
                    //                {
                    //                    dtActiveResult.Rows.InsertAt(dtActiveResult.NewRow(), i);
                    //                    dtActiveResult.Rows[i][columnName] = "SubTotal:";
                    //                    dtActiveResult.Rows[i][TotalcolumnName] = "     " + SubTotal;
                    //                    SubTotal = 0;
                    //                    i++;
                    //                }
                    //            }
                    //            SubTotal += Convert.ToInt16(dtActiveResult.Rows[i][TotalcolumnName]);

                    //        }
                    //        dtActiveResult.Rows.Add(dtActiveResult.NewRow());
                    //        dtActiveResult.Rows[dtActiveResult.Rows.Count - 1][columnName] = "SubTotal:";
                    //        dtActiveResult.Rows[dtActiveResult.Rows.Count - 1][TotalcolumnName] = "     " + SubTotal;

                    //        //// Create and add the final total row
                    //        DataRow finalTotal = dtActiveResult.NewRow();
                    //        finalTotal[columnName] = "Total:";
                    //        finalTotal[TotalcolumnName] = "     " + allEntries.Sum(x => Convert.ToDecimal(x));
                    //        dtActiveResult.Rows.Add(finalTotal);
                    //    }
                    //}

                    dtActiveResult1.Add(dtActiveResult);
                dtActiveResult1.Add(dtActiveResult2);
                dtActiveResult1.Add(dtActiveResult3);
                dtActiveResult1.Add(dtActiveResult4);
                return dtActiveResult1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public List<XIDropDown> GetWeekDays(List<XIDropDown> oData)
        //{
        //    DateTime startOfWeek = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);

        //    var Dates = Enumerable.Range(0, 7).Select(i => startOfWeek.AddDays(i).ToString("yyyy-MM-dd")).ToList();
        //    for (int i = 0; i < Dates.Count(); i++)
        //    {
        //        if (!oData.Any(x => x.text == Dates[i]))
        //        {
        //            XIDropDown rec = new XIDropDown();
        //            rec.Type = oData[0].Type;
        //            rec.Value = 0;
        //            rec.text = Dates[i];
        //            oData.Insert(i, rec);
        //        }
        //    }
        //    return oData;
        //}
        public CResult GenerateReport(List<CNV> oParams)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            string sParGUID = string.Empty;
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
                XIReports oReport = new XIReports();
                /* Load Report Definition here.*/
                XIIXI oXII = new XIIXI();
                XIDXI oXID = new XIDXI();
                XIDBO oBOD = (XIDBO)oXID.Get_BODefinition("Reports_t").oResult;
                XIIBO oBOIReport = oXII.BOI("Reports_t", ReportID.ToString(), "id,sName,FKiReportConfigID,sHeader,IsSaveToDB,sQuery,sTableName,sColour,RowColour,XiLinkID,bCollapse,bIsSubTotal,sSubTotalHeaders");
                oReport.ReportID = oBOIReport.AttributeI("id").iValue;
                oReport.Name = oBOIReport.AttributeI("sName").sValue;
                oReport.FKiReportConfigID = oBOIReport.AttributeI("FKiReportConfigID").iValue;
                oReport.Header = oBOIReport.AttributeI("sHeader").sValue;
                oReport.sColour = oBOIReport.AttributeI("sColour").sValue;
                oReport.rColour = oBOIReport.AttributeI("RowColour").sValue;
                oReport.XiLinkID = oBOIReport.AttributeI("XiLinkID").iValue;
                oReport.bCollapse = oBOIReport.AttributeI("bCollapse").sValue;
                oReport.bIsSubTotal = oBOIReport.AttributeI("bIsSubTotal").sValue;
                oReport.sSubTotalHeaders = oBOIReport.AttributeI("sSubTotalHeaders").sValue;
                oReport.SGUID = SGUID;
                oReport.SessionID = SessionID;
                /*Build Parent and child structure here*/
                if (oReport.FKiReportConfigID > 0)
                {
                    // Get data firstLevel here
                    List<XIChainReports> ListReports = new List<XIChainReports>();
                    XIIBO oConfig = oXII.BOI("ReportConfig_t", oReport.FKiReportConfigID.ToString(), "id,sName,FKiOneclickID,FKiOneclickIDXIGUID,iParentID,iPriority,iAppendType,iNodeType,iPivot,sData,iDataFormat,sQuery,bIsRowTotal,bIsCalculate,bIsHeader,bIsTotalSum,bIsRowColour,Scripts,sDefault,bCellValue,bColumnValue,TargetValue,coColour");
                    var oParent = new XIChainReports
                    {
                        id = oConfig.AttributeI("id").iValue,
                        Name = oConfig.AttributeI("sName").sValue,
                        OneClickID = oConfig.AttributeI("FKiOneclickID").iValue,
                        sOneClickID = oConfig.AttributeI("FKiOneclickIDXIGUID").sValue,
                        ParentID = oConfig.AttributeI("iParentID").iValue,
                        Priority = oConfig.AttributeI("iPriority").iValue,
                        NodeType = (EnumReportNodeTypes)oConfig.AttributeI("iNodeType").iValue,
                        Appendtype = (EnumReportAppendTypes)oConfig.AttributeI("iAppendType").iValue,
                        DataformatType = (EnumReportAppendTypes)oConfig.AttributeI("iDataFormat").iValue,
                        Pivot = (EnumPivot)oConfig.AttributeI("iPivot").iValue,
                        sData = oConfig.AttributeI("sData").sValue,
                        IsRowTotal = oConfig.AttributeI("bIsRowTotal").bValue,
                        IsCalculate = oConfig.AttributeI("bIsCalculate").bValue,
                        IsHeader = oConfig.AttributeI("bIsHeader").bValue,
                        IsTotal = oConfig.AttributeI("bIsTotalSum").bValue,
                        IsColour = oConfig.AttributeI("bIsRowColour").bValue,
                        Scripts = oConfig.AttributeI("Scripts").sValue,
                        bCell = oConfig.AttributeI("bCellValue").bValue,
                        sDefault = oConfig.AttributeI("sDefault").sValue,
                        ColumnValue = oConfig.AttributeI("bColumnValue").bValue,
                        coColour = oConfig.AttributeI("coColour").sValue,
                        TargetValue = oConfig.AttributeI("TargetValue").iValue,
                    };
                    var finalChilditems = RecursiveTree(oParent.id, "iParentID", new List<XIChainReports> { oParent }, true, SGUID, SessionID, oParams);
                    oParent.oChildItems = finalChilditems;
                    oReport.oChainReport = oParent;
                    oReport.FinalResult = BuildConsolidatedReport(new List<XIChainReports>(), true, oReport.oChainReport, oReport.bIsSubTotal, oReport.sSubTotalHeaders);//.SetHeadersToDatatable(oReport.Header);
                }
                //if (!string.IsNullOrEmpty(oBOIReport.AttributeI("IsSaveToDB").sValue) == true)
                //{
                //    XIDBAPI oDatabase = new XIDBAPI(ConfigurationManager.AppSettings["ClientDataBase"].ToString());
                //    //var Dataresult = (DataTable)oDatabase.ExecuteReader(System.Data.CommandType.Text, oBOIReport.AttributeI("sQuery").sValue, null);
                //    List<XIIBO> Rows = new List<XIIBO>();
                //    XIIBO oBOI = new XIIBO();
                //    foreach (DataRow dr in oReport.FinalResult.Rows)
                //    {
                //        oBOI = new XIIBO();
                //        oBOI.BOD = oBOD;
                //        foreach (DataColumn col in oReport.FinalResult.Columns)
                //        {
                //            oBOI.Attributes[col.Caption] = new XIIAttribute { sName = col.Caption, sValue = dr[col.Caption].ToString(), bDirty = true };
                //        }
                //        Rows.Add(oBOI);
                //    } 
                //    XIIBO xibulk = new XIIBO();
                //    DataTable dtbulk = xibulk.MakeBulkSqlTable(Rows);
                //    var resp = xibulk.SaveBulk(dtbulk, Rows[0].BOD.iDataSource, oBOIReport.AttributeI("sTableName").sValue);
                //    //CreateTABLE("Temp",oReport.FinalResult);
                //}
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oReport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oCResult;
        }
        //public static string CreateTABLE(string tableName, DataTable table)
        //{
        //    string sqlsc;
        //    sqlsc = "CREATE TABLE " + tableName + "(";
        //    for (int i = 0; i < table.Columns.Count; i++)
        //    {
        //        sqlsc += "\n [" + table.Columns[i].ColumnName + "] ";
        //        string columnType = table.Columns[i].DataType.ToString();
        //        switch (columnType)
        //        {
        //            case "System.Int32":
        //                sqlsc += " int ";
        //                break;
        //            case "System.Int64":
        //                sqlsc += " bigint ";
        //                break;
        //            case "System.Int16":
        //                sqlsc += " smallint";
        //                break;
        //            case "System.Byte":
        //                sqlsc += " tinyint";
        //                break;
        //            case "System.Decimal":
        //                sqlsc += " decimal ";
        //                break;
        //            case "System.DateTime":
        //                sqlsc += " datetime ";
        //                break;
        //            case "System.String":
        //            default:
        //                sqlsc += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
        //                break;
        //        }
        //        if (table.Columns[i].AutoIncrement)
        //            sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
        //        if (!table.Columns[i].AllowDBNull)
        //            sqlsc += " NOT NULL ";
        //        sqlsc += ",";
        //    }
        //    return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
        //}
        private List<XIChainReports> RecursiveTree(int id, string ColumnName, List<XIChainReports> oResult, bool Isrootnode, string SGUID, string SessionID, List<CNV> oParams)
        {
            try
            {
                // We should add the existing result to the Passed result. NEED TO TEST IT  
                List<XIChainReports> ListResult = new List<XIChainReports>();
                Dictionary<string, XIIBO> oXiBo = new Dictionary<string, XIIBO>();
                QueryEngine oQE = new QueryEngine();
                string sWhereCondition = ColumnName + "=" + id;
                //string sWhereCondition = $"{ColumnName}={id}";
                var oQResult = oQE.Execute_QueryEngine("ReportConfig_t", null, sWhereCondition);
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    ListResult = ((Dictionary<string, XIIBO>)oQResult.oResult).Select(d => new XIChainReports
                    {
                        id = d.Value.AttributeI("id").iValue,
                        Name = d.Value.AttributeI("sName").sValue,
                        OneClickID = d.Value.AttributeI("FKiOneclickID").iValue,
                        sOneClickID = d.Value.AttributeI("FKiOneclickIDXIGUID").sValue,
                        ParentID = d.Value.AttributeI("iParentID").iValue,
                        Priority = d.Value.AttributeI("iPriority").iValue,
                        NodeType = (EnumReportNodeTypes)d.Value.AttributeI("iNodeType").iValue,
                        Appendtype = (EnumReportAppendTypes)d.Value.AttributeI("iAppendType").iValue,
                        DataformatType = (EnumReportAppendTypes)d.Value.AttributeI("iDataFormat").iValue,
                        Pivot = (EnumPivot)d.Value.AttributeI("iPivot").iValue,
                        Query = d.Value.AttributeI("sQuery").sValue,
                        sData = d.Value.AttributeI("sData").sValue,
                        IsRowTotal = d.Value.AttributeI("bIsRowTotal").bValue,
                        IsCalculate = d.Value.AttributeI("bIsCalculate").bValue,
                        IsHeader = d.Value.AttributeI("bIsHeader").bValue,
                        IsTotal = d.Value.AttributeI("bIsTotalSum").bValue,
                        IsColour = d.Value.AttributeI("bIsRowColour").bValue,
                        Scripts = d.Value.AttributeI("Scripts").sValue,
                        sDefault = d.Value.AttributeI("sDefault").sValue,
                        ColumnValue = d.Value.AttributeI("bColumnValue").bValue,
                        bCell = d.Value.AttributeI("bCellValue").bValue,
                        coColour = d.Value.AttributeI("coColour").sValue,
                        TargetValue = d.Value.AttributeI("TargetValue").iValue,
                    }).ToList();
                    if (ListResult.Count() <= 0)
                    {
                        return ListResult;
                    }
                    foreach (var item in ListResult)
                    {
                        // Executing Recursively here.
                        item.nParms = nParms;
                        item.QueryResult = item.ExecuteResult(item.IsHeader, SGUID, SessionID, oParams);
                        item.oChildItems = RecursiveTree(item.id, "iParentID", new List<XIChainReports>(), false, SGUID, SessionID, oParams);                        
                    }
                }
                return ListResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class XIChainReports
    {
        // NEED TO CHANGE THE BELOW CONNECTION STRING HERE
        public string _connectionstring = ConfigurationManager.AppSettings["ClientDataBase"].ToString();//@"Data Source=192.168.7.8;Initial Catalog=iPMTest;User ID=crqauser;Password=crqauser"; /* TO DO : Connectionstring need to be changed */
        public string Query { get; set; }
        public DataTable QueryResult { get; set; }
        public DataTable AggregatorResult { get; set; }
        public int id { get; set; }
        public object Value { get; set; }
        public string RowHeader { get; set; }
        public string sDefault { get; set; }
        public bool ColumnValue { get; set; }
        public int TargetValue { get; set; }
        public string coColour { get; set; }
        private int myoneclick;
        public bool ExhaustTotal { get; set; }
        public string sOneClickID{get;set;}
        public int OneClickID
        {
            get
            {
                return myoneclick;
            }
            set
            {
                myoneclick = value;
            }
        }
        private int mypriority;
        public int Priority
        {
            get
            {
                return mypriority;
            }
            set
            {
                mypriority = value;
            }
        }
        private string mysdata;
        public string sData
        {
            get
            {
                return mysdata;
            }
            set
            {
                mysdata = value;
            }
        }
        private string sMyResolvedValue { get; set; }
        public string ExecuteScript
        {
            get
            {
                Query = Query;
                if (!string.IsNullOrEmpty(Query) && Query.Contains(RowHeader.ToString()))
                {
                    CResult oCR = new CResult();
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = Query.ToString().Replace(RowHeader.ToString(), Value.ToString().Replace("%", ""));
                    oCR = oXIScript.Execute_Script("", "");
                    sMyResolvedValue = oCR.oResult.ToString();
                }
                else
                {
                    CResult oCR = new CResult();
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = Query.ToString().Replace("{}", Value.ToString());
                    oCR = oXIScript.Execute_Script("", "");
                    sMyResolvedValue = oCR.oResult.ToString();
                }
                return sMyResolvedValue;
            }
            set
            {
                sMyResolvedValue = value;
            }

        }

        public bool IsRowTotal { get; set; }
        public bool IsCalculate { get; set; }
        public bool IsHeader { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public EnumPivot Pivot { get; set; }
        public EnumReportNodeTypes NodeType { get; set; }
        public EnumReportAppendTypes Appendtype { get; set; }
        public EnumReportAppendTypes DataformatType { get; set; }
        public List<XIChainReports> oChildItems { get; set; }
        public List<CNV> nParms { get; set; }
        /* Keerthi */
        public bool IsTotal { get; set; }
        public bool IsColour { get; set; }
        public string Scripts { get; set; }
        public bool bCell { get; set; }
        public string sReportName { get; set; }
        XIDStructure oXIDStructure = new XIDStructure();
        public DataTable ExecuteResult(bool IsHeader, string SGUID, string SessionID, List<CNV> oParamsList)
        {
            DataTable dt = new DataTable();
            /* EXECUTING ONE-CLICK HERE */
            if (NodeType == EnumReportNodeTypes.oneClick && (OneClickID > 0 || !string.IsNullOrEmpty(sOneClickID)))
            {
                XIIXI oXI = new XIIXI();
                XID1Click o1ClickD = new XID1Click();
                XIInfraCache oCache = new XIInfraCache();
                if (!string.IsNullOrEmpty(sOneClickID))
                {
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, sOneClickID);
                }
                else
                {
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID.ToString());
                }
                sReportName = o1ClickD.Name;
                var iQSIID = nParms.Where(t => t.sName == "iQSDID").Select(f => f.sValue).FirstOrDefault();
                Guid QSIGUID = Guid.Empty;
                int iQSID = 0;
                Guid.TryParse(iQSIID, out QSIGUID);
                int.TryParse(iQSIID, out iQSID);
                if(iQSID == 0 && (QSIGUID == null || QSIGUID == Guid.Empty))
                {
                    iQSIID = nParms.Where(t => t.sName == "{XIP|iQSInstanceID}").Select(f => f.sValue).FirstOrDefault();
                    Guid.TryParse(iQSIID, out QSIGUID);
                    int.TryParse(iQSIID, out iQSID);
                }

                //var sSessionID= nParms.Where(t => t.sName == "sSessionID").Select(f =>(f.sValue)).FirstOrDefault();
                //var sGUID = nParms.Where(t => t.sName == "sGUID").Select(f => (f.sValue)).FirstOrDefault();
                var cachevalues = oCache.GetAllParamsUnderGUID(SessionID, SGUID, "ReportContext");
                var sVisual = oParamsList.Where(m => m.sName == "Visualisation").Select(m => m.sValue).FirstOrDefault();
                var oQSInstance = new XIIQS();
                if (QSIGUID != null && QSIGUID != Guid.Empty)
                {
                    oQSInstance = oXI.GetQSXIValuesByQSIID(QSIGUID.ToString());
                }
                else if(iQSID > 0)
                {
                    oQSInstance = oXI.GetQSXIValuesByQSIID(iQSID.ToString());
                }
                XID1Click o1ClickCopy = new XID1Click();
                o1ClickCopy = (XID1Click)o1ClickD.Clone(o1ClickD);
                List<CNV> oParams = new List<CNV>();
                CNV oParam = new CNV();

                foreach (var item in oQSInstance.XIValues)
                {
                    oParam.sName = "{XIP|" + item.Key + "}";
                    oParam.sValue = item.Value.sValue;
                    oParams.Add(oParam);
                    oCache.Set_ParamVal(SessionID, SGUID, null, oParam.sName, oParam.sValue, null, null);
                    nParms.Add(oParam);
                    oParam = new CNV();
                }
                List<XIVisualisation> oXIVisualisations = new List<XIVisualisation>();
                var oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, sVisual, null);
                if (oXIvisual != null)
                {
                    oXIVisualisations.Add(oXIvisual);
                }

                var whereparam = "";
                if (cachevalues != null)
                {
                    foreach (var item in cachevalues.NMyInstance)
                    {
                        if (!string.IsNullOrEmpty(item.Value.sValue) && item.Value.sValue != "01-Jan-0001")
                        {
                            if (oXIVisualisations.FirstOrDefault().XiVisualisationNVs != null)
                            {
                                foreach (var item1 in oXIVisualisations.FirstOrDefault().XiVisualisationNVs)
                                {
                                    if (item1.sName.ToLower() == item.Key.ToLower())
                                    {
                                        whereparam += item1.sValue + " and ";
                                    }
                                }
                            }
                            oParam.sName = "{XIP|" + item.Key + "}";
                            oParam.sValue = item.Value.sValue.ToString();
                            nParms.Add(oParam);
                            oParam = new CNV();

                        }
                    }
                    if (!string.IsNullOrEmpty(whereparam))
                        whereparam = whereparam.Substring(0, whereparam.Length - 5);
                }
                if (!string.IsNullOrEmpty(whereparam))
                {
                    var Query = o1ClickCopy.AddSearchParameters(o1ClickCopy.Query, whereparam);
                    o1ClickCopy.Query = oXIDStructure.ReplaceExpressionWithCacheValue(Query, nParms);
                }
                else
                {
                    o1ClickCopy.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickCopy.Query, nParms);
                }
                o1ClickCopy.ReplaceFKExpressions(nParms);
                var Dataresult = o1ClickCopy.Execute_Query();
                dt = Dataresult;
            }
            else if (!string.IsNullOrEmpty(Query))
            {
                XIDBAPI oDatabase = new XIDBAPI(_connectionstring);
                //Commented by Ravi
                var Dataresult = (DataTable)oDatabase.ExecuteReader(System.Data.CommandType.Text, Query, null);
                // CHECK FOR PIVOTING HERE
                dt = Dataresult;
            }
            else if (!string.IsNullOrEmpty(sData) && !sData.Contains("{{") && !sData.Contains("{XIP"))
            {
                // Append Whether it is Horizontal or not
                dt = sData.BuildDataTableWithString(DataformatType, IsHeader);
            }
            else if (!string.IsNullOrEmpty(sData) && sData.Contains("{XIP"))
            {
                sData = oXIDStructure.ReplaceExpressionWithCacheValue(sData, nParms);
                // Append Whether it is Horizontal or not
                dt = sData.BuildDataTableWithString(DataformatType, IsHeader);
            }
            return dt;
        }
    }
}