using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIBoDefaultDashboard
    {
        public int ID { get; set; }
        public int FKiOneClickID { get; set; }
        public int FKiBOID { get; set; }
        public string sType { get; set; }
        //public string sQuery { get; set; }
        public bool bFlag { get; set; }
        public int FKiPlaceholderID { get; set; }
        public int XiLinkTypeID { get; set; }
        public int iRowXilinkID { get; set; }
        public int FKiLayoutID { get; set; }
        public int FKiDialogID { get; set; }
        public int FKiAttributeID { get; set; }
        public string sAttrName { get; set; }
        public int FKiComponentTypeID { get; set; }
        public string sContentType { get; set; }
        public string sChartType { get; set; }
    }
}