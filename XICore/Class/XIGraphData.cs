using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XIDatabase;

namespace XICore
{
    public class XIGraphData
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int? TabID { get; set; }
        public int? DDLClassValue { get; set; }
        public int ReportID { get; set; }
        public string Query { get; set; }
        public string DisplayAs { get; set; }
        public string SectionName { get; set; }
        public int? DDLDateValue { get; set; }
        public List<XIDashboardContent> KPICount { get; set; }
        public int ClassFilter { get; set; }
        public int DateFilter { get; set; }
        public List<XIDropDown> ClassDDL { get; set; }
        public List<XIDropDown> DateDDL { get; set; }
        public List<XIDashBoardGraphs> PieData { get; set; }
        public List<XIDashBoardGraphs> FunnelData { get; set; }

        public XILineGraph BarData { get; set; }
        public string QueryName { get; set; }
        public string ShowAs { get; set; }
        public bool IsColumnClick { get; set; }
        public string OnClickColumn { get; set; }
        public int OnClickResultID { get; set; }
        public int BOID { get; set; }
        public string ResultIn { get; set; }
        public int OrgID { get; set; }
        public string Database { get; set; }
        public string Name { get; set; }
        public int iDataSourceID { get; set; }
        public Guid DataSourceXIGUID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int OnColumnClickValue { get; set; }
        public Dictionary<string, string> OneClickRes { get; set; }
        public List<XIVisualisationNV> oXIVisualisations { get; set; }
        public Dictionary<List<string>, List<XIIBO>> ComOneClick { get; set; }
        public Dictionary<string, XIIBO> oRes { get; set; }
        public string sLastUpdated { get; set; }
        public string RowXilinkID { get; set; }
        public int iOneClickID { get; set; }
        public string sOneClickID { get; set; }
        public string GaugeChartvalue { get; set; }
        public List<string> sColours { get; set; }
        public bool bToolTip { get; set; }
        public bool bGridLines { get; set; }
        public bool bIsCursor { get; set; }
        public bool bIsLegends { get; set; }
        public bool bDonutchart { get; set; }
        public string sLegendPosition { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsStoredProcedure { get; set; }
        public List<Dictionary<string, string>> KeyPairs{get;set;}
        //public string sOneClickID { get; set; }
        public int Count { get; set; }
        public Guid RowXiLinkIDXIGUID { get; set; }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        public XIGraphData Get_PieChartData(XIDBO oBOD)
        {
            XIGraphData GraphData = new XIGraphData();
            List<XIDashBoardGraphs> PieData = new List<XIDashBoardGraphs>();
            List<string[]> results = new List<string[]>();
            XIDXI oDXI = new XIDXI();
            string sConntection = oDXI.GetBODataSource(DataSourceXIGUID.ToString(),oBOD.FKiApplicationID);
            Connection = new XIDBAPI(sConntection);
            //string Count = Connection.GetTotalCount(CommandType.Text, Query, null);
            string[] value = null;
            var sBODataSource = string.Empty;
            sBODataSource = oDXI.GetBODataSource(DataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
            List<XIDropDown> FKDDL = new List<XIDropDown>();
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                int count = reader.FieldCount;
                string[] rows = new string[count];
                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!reader.IsDBNull(i))
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        else
                        {
                            values.Add("0");
                        }
                    }
                    string[] result = values.ToArray();
                    results.Add(result);
                    value = result;
                }
                int j = 0;
                var Keys = GetForeginkeyValues(Query);
                if (reader.HasRows == true)
                {
                    foreach (var items1 in results)
                    {
                        for (int i = 0; i < items1.Count(); i++)
                        {
                            XIDashBoardGraphs Values = new XIDashBoardGraphs();
                            var ID = items1[0];
                            if (Keys.Count() > 0)
                            {
                                //var Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], items1[0], Chart.Database);
                                var Name = "Live";
                                if (Name != null)
                                {
                                    Values.label = Name;
                                    Values.value = Convert.ToInt32(items1[1]);
                                    PieData.Add(Values);
                                }
                            }
                            else
                            {
                                Values.label = items1[0];
                                Values.value = Convert.ToInt32(items1[1]);
                                PieData.Add(Values);
                            }
                        }
                        j++;
                    }
                }
                else
                {
                    XIDashBoardGraphs model1 = new XIDashBoardGraphs();
                    PieData.Add(model1);
                }
                GraphData.ReportID = ID;
                GraphData.PieData = PieData;
                GraphData.QueryName = QueryName;
                GraphData.UserID = UserID;
                GraphData.ShowAs = ShowAs;
                GraphData.Type = Type;
                GraphData.ID = ID;
                GraphData.RowXiLinkIDXIGUID= RowXiLinkIDXIGUID;
                GraphData.RowXilinkID= RowXilinkID;
                if (IsColumnClick)
                {
                    GraphData.IsColumnClick = true;
                    GraphData.OnClickColumn = OnClickColumn;
                    GraphData.OnClickResultID = OnColumnClickValue;
                }
                Con.Close();
            }
            return GraphData;
        }

        public static List<XIDropDown> GetForeginkeyValues(string Query)
        {
            List<XIDropDown> AllVlaues = new List<XIDropDown>();
            var iCover = Query.IndexOf("iCover", StringComparison.OrdinalIgnoreCase);
            if (iCover > 0)
            {
                AllVlaues.Add(new XIDropDown
                {
                    text = "iCover",
                    Value = iCover
                });
            }
            List<XIDropDown> SortedList = AllVlaues.OrderBy(o => o.Value).ToList();
            return SortedList;
        }
    }
    public class XIDashboardContent
    {
        public int ReportID { get; set; }
        public string Type { get; set; }
        public int? ClassID { get; set; }
        public int? dImportedOn { get; set; }
        public string ReportName { get; set; }
    }
    public class XIDashBoardGraphs
    {
        public string color { get; set; }
        public string highlight { get; set; }
        public string label { get; set; }
        public int value { get; set; }
        public string Hours { get; set; }
        public List<XIDropDown> ClassDDL { get; set; }
        public List<XIDropDown> DateDDL { get; set; }
        public string QueryName { get; set; }
        public string ShowAs { get; set; }
        public bool IsColumnClick { get; set; }
        public string OnClickColumn { get; set; }
        public int OnClickResultID { get; set; }
    }

    public class XILineGraph
    {
        public List<List<string>> Data { get; set; }
        public int? TabID { get; set; }
        public string SectionName { get; set; }
        public int Rank { get; set; }
        public List<XIDropDown> ClassDDL { get; set; }
        public List<XIDropDown> DateDDL { get; set; }
        public string Type { get; set; }
        public string QueryName { get; set; }
        public int ReportID { get; set; }
        public int UserID { get; set; }
        //public VMViewPopup Popup { get; set; }
        public string PreviewType { get; set; }
        //public ViewRecord ViewRecord { get; set; }
        //public List<SectionsData> SectionsData { get; set; }
        public int InnerReportID { get; set; }
        public string ShowAs { get; set; }
        public bool IsColumnClick { get; set; }
        public string OnClickColumn { get; set; }
        public string OnClickParameter { get; set; }
        public int OnClickResultID { get; set; }
        public string OnClickCell { get; set; }
    }
}
