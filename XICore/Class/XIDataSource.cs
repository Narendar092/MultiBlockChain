using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDataSource
    {
        public int ID { get; set; }

        public Guid XIGUID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sConnectionString { get; set; }
        public int FKiOrgID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public string sType { get; set; }
        private string sQSType { get; set; }
        private string DSType { get; set; }
        public int StatusTypeID { get; set; }
        public string sQueryType { get; set; }
        public string sServer { get; set; }
        public string sDatabase { get; set; }
        public List<XIDropDown> ddlApplications { get; set; }
        public string sDataSourceType { set; get; }
    }
}