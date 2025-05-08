using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDAccount
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sCode { get; set; }
        public string sFromAddress { get; set; }
        public string sAPIURL { get; set; }
        public string sUserName { get; set; }
        public string sPassword { get; set; }
        public string sSecurity { get; set; }
        public int iPort { get; set; }
        public int iType { get; set; }
        public int iStatus { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiAppID { get; set; }
        public int FKiOrgID { get; set; }
        public string sSendgridKey { get; set; }
    }
}