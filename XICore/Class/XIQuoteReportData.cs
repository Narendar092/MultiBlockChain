using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIQuoteReportData
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ReportID { get; set; }
        public string Query { get; set; }
        public string DisplayAs { get; set; }
        public string QueryName { get; set; }
        public string ShowAs { get; set; }
        public int BOID { get; set; }
        public int OrgID { get; set; }
        public string Database { get; set; }
        public string Name { get; set; }
        public int iDataSourceID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> OneClickRes { get; set; }
        public List<XIVisualisationNV> oXIVisualisations { get; set; }
        public Dictionary<List<string>, List<XID1Click>> ComOneClick { get; set; }
        public Dictionary<string, XIIBO> oRes { get; set; }
        public Dictionary<string, XIIBO> ReportList { get; set; }
        public string sGUID { get; set; }
        public bool IsRowClick { get; set; }
        public int RowXiLinkID { get; set; }
        public string sBOName { get; set; }
        public string LayoutID { get; set; }
    }
}