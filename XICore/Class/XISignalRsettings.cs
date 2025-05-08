using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XISignalRsettings
    {
        public int ID { get; set; }
        public string Config { get; set; }
        public DateTime? NotificationOFFFrom { get; set; }
        public DateTime? NotificationOFFTo { get; set; }
        public bool Flag { get; set; }
        public string ConstantID { get; set; }
        public int UserID { get; set; }
        public int ShowCount { get; set; }
        public int Count { get; set; }
        public int NotificationBO { get; set; }
        public string BellID { get; set; }
        public string sSelectedFields { get; set; }
        public string BOName { get; set; }
        public int NewSignalrQueryID { get; set; }
        public int OneClick { get; set; }
        public int ResultOneClick { get; set; }
        public string sWhereFields { get; set; }
        public int BOID { get; set; }
        public string BOSelectedFields { get; set; }
        public string OneClickOrBO { get; set; }
        public string AlertText { get; set; }
        public string AlertInfo { get; set; }
        public List<string> PopMessages { get; set; }
        public List<XIIBO> Records { get; set; }
        public string SentMail { get; set; }
        public object MasterID { get; set; }
        public int FkiOneClick { get; set; }
        public int RoleID { get; set; }
        public XIGraphData GraphData { get; set; }
        public XIQuoteReportData QuoteReportData { get; set; }
        public List<XID1Click> BarGraphvalues { get; set; }
        public List<string> BarGraphKeys { get; set; }
        public Dictionary<string, string> DashboardAmountValues { get; set; }
        public int iRoleID { get; set; }
        public int FKiAlgorithmID { get; set; }
        public int iCategory { get; set; }
        public Guid FKiBOIDXIGUID { get; set; }
        public int iType { get; set; }
        public string sTargetArea { get; set; }
        public Guid FKiTemplateIDXIGUID { get; set; }
        public string sStructureCode { get; set; }

    }
}