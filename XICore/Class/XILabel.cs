using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XILabel
    {
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sCode { get; set; }
        public int FKiAttributeID { get; set; }
        public Guid FKiAttributeIDXIGUID { get; set; }
        public string sLanguage { get; set; }
        public int FKiOrgID { get; set; }
        public Guid FKiOrgIDXIGUID { get; set; }
        public int iScript { get; set; }
        public string sOutputLabel { get; set; }
    }
}